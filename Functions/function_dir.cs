using System;
using System.Collections.Generic;
using System.Linq;

namespace OC
{
    public class function_dir
    {
        public static void create_dir(string name, string path, string user_name)
        {
            catalog f = new catalog();
            f.name = name; // задаем имя
            f.List = new List<TypeOf>(); // очищаем список файлов
                                         // поиск свободного inode в блоках
            f.attributes = function_inode.Search_free_inode();
            f.attributes.inode_number = 2;
            f.attributes.free = false; // устанавливаем inode как занятый
            f.attributes.di_size = 0;
            f.attributes.di_uid = user_name;
            f.attributes.di_mtime = DateTime.Now;
            f.attributes.di_ctime = DateTime.Now;
            f.attributes.dimode.type = "folder";
            f.attributes.dimode.rights = "rw---";

            //Если директории с таким именем еще нет, то создаем, иначе оповещаем пользователя
            if (function_dir.Search_folder(name, path) == "root/")
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
                //Main.Sess.path = Main.Sess.path + f.name + "/";
                Program.myForm.Log.Text += "Папка '" + f.name + "' создана \n\n";
            }
            else
            {
                Program.myForm.Log.Text += "Папка с именем '" + f.name + "' уже существует!\n\n";
            }
        }

        public static string Search_folder(string name, string path)
        {
            string path_res = "root/";
            path += name + "/";

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
                        path_res += temp.List[j].name + "/";
                        temp.attributes = temp.List[j].attributes;
                        temp.name = temp.List[j].name;
                        catalog ab = (catalog)temp.List[j];
                        temp.List = ab.List;
                    }
                }
            }
            return path_res;
        }
        public static catalog See_folder(string path)
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
            return temp;
        }
        public static void ren_folder(string path, string rname)
        {
            string[] path_l = path.Split('/');
            catalog temp = new catalog();
            temp.attributes = Main.Root.attributes;
            temp.name = Main.Root.name;
            temp.List = Main.Root.List;

            for (int i = 0; i < path_l.Count() - 2; i++)
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
            //Флаг для обнаружения успешного выполнения
            bool isset = false;
            for (int i = 0; i < temp.List.Count; i++)
            {
                //Если нашли файл, который нужно переименовать, и если учетная запись админа 
                //или если создателю можно переименовывать, или если другим пользователям можно переименовывать
                if ((temp.List[i].name == path_l[path_l.Count() - 2]) &&
                   (((Main.Sess.user_name == "admin") ||
                   ((temp.List[i].attributes.di_uid == Main.Sess.user_name) && (function_inode.rights_for_all(temp.List[i])[1])) ||
                   ((function_inode.rights_for_all(temp.List[i])[3])))))
                {
                    //Меняем выводимый результат в завичимости от типа
                    string result = "Папка успешно переименована\n\n";
                    if (temp.List[i].attributes.dimode.type == "file")
                    {
                        result = "Файл успешно переименован\n\n";
                    }
                    temp.List[i].name = rname;

                    isset = true;
                    Program.myForm.Log.Text += result;
                }
                else if (temp.List[i].name == path_l[path_l.Count() - 2])
                {
                    isset = true;
                    if (temp.List[i].attributes.dimode.type == "file")
                        Program.myForm.Log.Text += "У вас нет прав на переименование этого файла!\nВы не являетесь владельцем данного файла.\n\n";
                    else Program.myForm.Log.Text += "У вас нет прав на переименование этой директории!\nВы не являетесь владельцем данной директории.\n\n";
                }
            }
            if (!isset)
            {
                Program.myForm.Log.Text += "Данной директории или файла не существует!\n\n";
            }
        }

        public static void Dell_folder(string path, string delname)
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
            //Флаг для обнаружения успешного выполнения
            bool isset = false;
            for (int i = 0; i < temp.List.Count; i++)
            {
                //if (temp.List[i].name.Equals(delname))
                //Если нашли файл, который нужно удалить, и если учетная запись админа или если создателю можно удалять, или если другим пользователям можно удалять
                if ((temp.List[i].name.Equals(delname)) &&
                    (((Main.Sess.user_name == "admin") ||
                    ((temp.List[i].attributes.di_uid == Main.Sess.user_name) && (function_inode.rights_for_all(temp.List[i])[1])) ||
                    ((function_inode.rights_for_all(temp.List[i])[3])))))
                {
                    //Меняем выводимый результат в зависимости от типа
                    string result = "Папка '" + delname + "' удалена\n\n";
                    if (temp.List[i].attributes.dimode.type == "file")
                    {
                        result = "Файл '" + delname + "' удален\n\n";
                    }
                    temp.List[i].attributes.di_uid = "root";
                    temp.List[i].attributes.free = true;
                    temp.List[i].attributes.dimode.rights = "rwx-rwx";
                    temp.List[i].attributes.dimode.type = " ";
                    for (int j = 0; j < temp.List[i].attributes.di_addr.Count(); j++)
                    {
                        temp.List[i].attributes.di_addr[j] = 0;
                    }
                    temp.List.RemoveAt(i);
                    isset = true;
                    Program.myForm.Log.Text += result;
                }
                else if (temp.List[i].name.Equals(delname)) {
                    isset = true;
                    if (temp.List[i].attributes.dimode.type == "file")
                        Program.myForm.Log.Text += "У вас нет прав на удаление этого файла!\nВы не являетесь владельцем данного файла.\n\n";
                    else Program.myForm.Log.Text += "У вас нет прав на удаление этой директории!\nВы не являетесь владельцем данной директории.\n\n";
                }
            }
            if (!isset)
            {
                Program.myForm.Log.Text += "Данной директории или файла не существует!\n\n";
            }
        }
    }
}
