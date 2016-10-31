using System;
using System.Linq;

namespace OC
{
    public class function_file
    {
        public static void create_file(string name, string path, string user_name)
        {
            file f = new file();
            f.name = name; // задаем имя
                           // поиск свободного inode в блоках
            f.attributes = function_inode.Search_free_inode();
            f.attributes.inode_number = 2;
            f.attributes.free = false; // устанавливаем inode как занятый
            f.attributes.di_size = 1024;
            f.attributes.di_uid = user_name;
            f.attributes.di_mtime = DateTime.Now;
            f.attributes.di_ctime = DateTime.Now;
            f.attributes.dimode.type = "file";
            f.attributes.dimode.rights = "rw---";
            f.attributes.di_addr[0] = function_inode.Search_free_cluster();
            function_inode.SetFalse(f.attributes.di_addr[0]);

            //Если файл с таким именем еще нет, то создаем, иначе оповещаем пользователя
            if (!Is_file(name, path))
            {
                // cоздание папки по пути
                string[] path_l = path.Split('/');
                catalog temp = new catalog();
                temp.attributes = Main.Root.attributes;
                temp.name = Main.Root.name;
                temp.List = Main.Root.List;

                for (int i = 0; i < path_l.Count() - 1; i++)
                {
                    temp.name = path_l[i];
                    for (int j = 0; j < temp.List.Count; j++)
                    {
                        if ((temp.List[j].name == path_l[i]) && (i != path_l.Count()))
                        {
                            temp.attributes = temp.List[j].attributes;
                            temp.name = temp.List[j].name;
                            catalog ab = (catalog)temp.List[j];
                            temp.List = ab.List;
                        }
                    }
                }
                temp.List.Add(f);
                Program.myForm.Log.Text += "Файл '" + name + "' создан \n\n";
            }
            else
            {
                Program.myForm.Log.Text += "Файл с именем '" + f.name + "' уже существует!\n\n";
            }
        }
        public static void modify_file(file f, string text = "")
        {
            //Сохраняем консоль
            Form1.history = Program.myForm.Log.Text;
            //Очищаем консоль и позволяем пользователю писать в ней,
            //при этом блокируем поле для ввода команд
            Program.myForm.Log.Text = "";
            Program.myForm.Log.ReadOnly = false;
            Program.myForm.Command.ReadOnly = true;
            Program.myForm.Log.Select();
            //Наполняем содержимым файла, если мы его редактируем
            if(text != ""){
                Program.myForm.Log.Text = text;
            }
            f.attributes.di_mtime = DateTime.Now;
            //Указываем файл, в который будем записывать
            Form1.record = f;
        }
        public static Boolean Is_file(string f, string path)
        {
            string[] path_l = path.Split('/');
            catalog temp = new catalog();
            temp.attributes = Main.Root.attributes;
            temp.name = Main.Root.name;
            temp.List = Main.Root.List;

            for (int i = 0; i < path_l.Count() - 1; i++)
            {
                temp.name = path_l[i];
                for (int j = 0; j < temp.List.Count; j++)
                {
                    if ((temp.List[j].name == path_l[i]) && (i != path_l.Count()))
                    {
                        temp.attributes = temp.List[j].attributes;
                        temp.name = temp.List[j].name;
                        catalog ab = (catalog)temp.List[j];
                        temp.List = ab.List;
                    }
                }
            }
            for (int y = 0; y < temp.List.Count; y++)
            {
                if ((f == temp.List[y].name))
                {
                    return true; //файл существует
                }
            }
            return false; //файла не существует

        }
        public static void search_file(string f, string path)
        {
            string[] path_l = path.Split('/');
            catalog temp = new catalog();
            temp.attributes = Main.Root.attributes;
            temp.name = Main.Root.name;
            temp.List = Main.Root.List;

            for (int i = 0; i < path_l.Count() - 1; i++)
            {
                temp.name = path_l[i];
                for (int j = 0; j < temp.List.Count; j++)
                {
                    if ((temp.List[j].name == path_l[i]) && (i != path_l.Count()))
                    {
                        temp.attributes = temp.List[j].attributes;
                        temp.name = temp.List[j].name;
                        catalog ab = (catalog)temp.List[j];
                        temp.List = ab.List;
                    }
                }
            }
            for (int y = 0; y < temp.List.Count; y++)
            {
                if (f == temp.List[y].name)
                {
                    file t = (file)temp.List[y];
                    t.attributes.di_mtime = edit_file(t);
                }
            }
        }
        public static file search_file_retf(string f, string path)
        {
            string[] path_l = path.Split('/');
            catalog temp = new catalog();
            temp.attributes = Main.Root.attributes;
            temp.name = Main.Root.name;
            temp.List = Main.Root.List;

            for (int i = 0; i < path_l.Count() - 1; i++)
            {
                temp.name = path_l[i];
                for (int j = 0; j < temp.List.Count; j++)
                {
                    if ((temp.List[j].name == path_l[i]) && (i != path_l.Count()))
                    {
                        temp.attributes = temp.List[j].attributes;
                        temp.name = temp.List[j].name;
                        catalog ab = (catalog)temp.List[j];
                        temp.List = ab.List;
                    }
                }
            }
            for (int y = 0; y < temp.List.Count; y++)
            {
                if (f == temp.List[y].name)
                {
                    file t = (file)temp.List[y];
                    return t;
                }
            }
            file r = new file();
            return r;
        }

        public static TypeOf search_Type_Of(string f, string path)
        {
            string[] path_l = path.Split('/');
            catalog temp = new catalog();
            temp.attributes = Main.Root.attributes;
            temp.name = Main.Root.name;
            temp.List = Main.Root.List;

            for (int i = 0; i < path_l.Count() - 1; i++)
            {
                temp.name = path_l[i];
                for (int j = 0; j < temp.List.Count; j++)
                {
                    if ((temp.List[j].name == path_l[i]) && (i != path_l.Count()))
                    {
                        temp.attributes = temp.List[j].attributes;
                        temp.name = temp.List[j].name;
                        catalog ab = (catalog)temp.List[j];
                        temp.List = ab.List;
                    }
                }
            }
            for (int y = 0; y < temp.List.Count; y++)
            {
                if (f == temp.List[y].name)
                {
                    TypeOf t = temp.List[y];
                    return t;
                }
            }
            TypeOf r = new TypeOf();
            return r;
        }

        public static DateTime edit_file(file t)
        {
            string l = Program.myForm.Log.Text;
            Program.myForm.Log.Text = "";
            //Если учетная запись админа или если создателю можно записывать, или если другим пользователям можно записывать
            /*if ((Main.Sess.user_name == "admin") ||
                ((t.attributes.di_uid == Main.Sess.user_name) && (function_inode.rights_for_all(t)[1])) ||
                ((function_inode.rights_for_all(t)[3])))
            {*/
                if (l.Length <= Main.claster) // размер не превышает размеры кластера
                {
                    for (int i = 0; i < t.attributes.di_addr.Count(); i++)
                    {
                        int k;
                        k = t.attributes.di_addr[i];
                        for (int j = 0; j < Main.Super.Blocks.Count; j++)
                        {
                            for (int y = 0; y < Main.Super.Blocks[j].claster.Count(); y++)
                            {
                                if (k == Main.Super.Blocks[j].claster[y].number)
                                {
                                    Main.Super.Blocks[j].claster[y].data = l;
                                    return DateTime.Now;
                                }
                            }
                        }
                    }
                }

                //если выходит за пределы кластера

                //начальный адрес выделения подстроки
                int count = Main.claster;
                string q = l.Substring(0, count);

                int n = t.attributes.di_addr[0];
                for (int j = 0; j < Main.Super.Blocks.Count; j++)
                    for (int y = 0; y < Main.Super.Blocks[j].claster.Count(); y++)
                    {
                        if (n == Main.Super.Blocks[j].claster[y].number)
                        {
                            {
                                Main.Super.Blocks[j].claster[y].data = q;
                            }
                        }
                    }


                while ((l.Length > count + Main.claster)) //пока позиция меньше или равна количеству символов в строке
                {
                    q = l.Substring(count, Main.claster); //выделяем подстроку               
                    {
                        int u = function_inode.Search_free_cluster(); //ищем свободный кластер
                        for (int e = 0; e < t.attributes.di_addr.Count(); e++) //проход по списку кластеров
                        {
                            if (t.attributes.di_addr[e] == 0) //свободная запись для номера кластера
                            {
                                t.attributes.di_addr[e] = u; //присваиваем новый номер
                                function_inode.SetFalse(u); //устанавливаем его в false
                                break;
                            }
                        }
                        for (int j = 0; j < Main.Super.Blocks.Count; j++)
                        {
                            for (int y = 0; y < Main.Super.Blocks[j].claster.Count(); y++)
                            {
                                if (u == Main.Super.Blocks[j].claster[y].number)
                                {
                                    {
                                        Main.Super.Blocks[j].claster[y].data = q;
                                    }
                                }
                            }
                        }
                    }
                    count += Main.claster; // следующая позиция
                }
                if (l.Count() > 0)
                {
                    q = l.Substring(count);
                    int u = function_inode.Search_free_cluster(); //ищем свободный кластер
                    for (int e = 0; e < t.attributes.di_addr.Count(); e++) //проход по списку кластеров
                    {
                        if (t.attributes.di_addr[e] == 0) //свободная запись для номера кластера
                        {
                            t.attributes.di_addr[e] = u; //присваиваем новый номер
                            function_inode.SetFalse(u); //устанавливаем его в false
                            break;
                        }
                    }
                    for (int j = 0; j < Main.Super.Blocks.Count; j++)
                    {
                        for (int y = 0; y < Main.Super.Blocks[j].claster.Count(); y++)
                        {
                            if (u == Main.Super.Blocks[j].claster[y].number)
                            {
                                {
                                    Main.Super.Blocks[j].claster[y].data = q;
                                }
                            }
                        }
                    }
                }
                Program.myForm.Log.Text += "Файл '" + t.name + "' записан\n\n";
            /*}
            else
            {
                Program.myForm.Log.Text += "Вам не разрешено записывать в этот файл. \nВы не являетесь владельцем данного файла.\n\n";
            }*/
            return DateTime.Now;
        }

        public static string see_file(string dir, string path)
        {

            string t = ""; //результирующая строка
            file f = new file(); //нужный файл
            f = search_file_retf(dir, path);//поиск нужного файла
            //Если учетная запись админа или если создателю можно просматривать, или если другим пользователям можно просматривать
            if ((Main.Sess.user_name == "admin") ||
                ((f.attributes.di_uid == Main.Sess.user_name) && (function_inode.rights_for_all(f)[0])) ||
                ((function_inode.rights_for_all(f)[2])))
            {
                int i = 0;
                while ((f.attributes.di_addr[i] != 0) || (f.attributes.di_addr.Count() < i))
                {
                    t += function_inode.ClusterData(f.attributes.di_addr[i]);
                    i++;
                }
            }
            else
            {
                t += Program.myForm.Log.Text += "Вам не разрешено просматривать этот файл. \nВы не являетесь владельцем данного файла.";
            }
            return t;

        }

        public static string attr(string dir, string path)
        {
            TypeOf t = search_Type_Of(dir, path);
            string res = string.Empty;
            res += "Имя файла: " + t.name + "\nТип: " + t.attributes.dimode.type + "\nРазмер: " + t.attributes.di_size + "\nВладелец: " + t.attributes.di_uid;
            res += "\nСоздан: " + t.attributes.di_ctime + "\nИзменен: " + t.attributes.di_mtime + "\nПрава: " + t.attributes.dimode.rights + "\n\n";

            return res;
        }

    }
}
