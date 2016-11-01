using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Text.RegularExpressions;

namespace OC
{
    public class Main
    {
        double StartMemoryHDD = 51200; //5 байт памяти сначала на диске
        int Blocks_HDD = 10240; // стандартный размер блока в байтах
        int CountBlocks = 0; //количество созданных блоков
        int inode_count = 0;//количество inode 
        int Inode_Memory = 1024; //один inode 
        public static int claster = 2048; //размер кластера в байтах
        public static Superblock Super;
        public static Core core;

        public static catalog Root=new catalog();
                
        public static Session Sess = new Session();
        public void MainMenu()
        {
            if (Sess.user_id!=0)
            {
                switch (Program.myForm.Command.Text)
                {
                    case "process":
                        {
                            Core core = new Core(4);
                            core.init();
                            Core.proc[1].Memory = Core.proc[0].Memory;
                            Core.mem[0].process.Add(Core.proc[1].PID);
                            core.Planning();                            

                            break;
                        }
                    case "mount":
                        {
                            mount();
                            break;
                        }
                    case "user_to_file":
                        {
                            Serializing_User.Ser(Sess.user_list);
                            break;
                        }
                    case "inode_map":
                        {
                            Program.myForm.Log.Text += "Block.Inode"+ "\n";
                            List<string> t = Super.FreeInode();
                            for (int i = 0; i < t.Count; i++)
                            {
                                Program.myForm.Log.Text += t[i] + "\n";
                            }
                                break;
                        }
                    case "claster_map":
                        {
                            Program.myForm.Log.Text += "Block.Claster" + "\n";
                            List<string> t = Super.FreeClaster();
                            for (int i = 0; i < t.Count; i++)
                            {
                                Program.myForm.Log.Text += t[i] + "\n";
                            }
                            break;
                        }
                    case "exit":
                        {
                            function_user.new_session();
                            break;
                        }
                    case "lf":
                        {
                            catalog t = new catalog();
                            t=function_dir.See_folder(Sess.path);
                            if (t.List.Count == 0)
                            {
                                Program.myForm.Log.Text += "В папке '" + t.name + "' пусто!\n\n";
                            }
                            else
                            {
                                Program.myForm.Log.Text += "Папка '" + t.name + "' содержит:" + "\n";
                                for (int i = 0; i < t.List.Count; i++)
                                {
                                    Program.myForm.Log.Text += t.List[i].name + " (" + t.List[i].attributes.dimode.type + ")\n";
                                }
                                Program.myForm.Log.Text += "\n";
                            }
                            break;
                        }
                    default:
                        {
                            Regex regex = new Regex(@"^(?<command>[A-Za-z_]+)[ ]*");
                            Match matches = regex.Match(Program.myForm.Command.Text);
                            string command = String.Empty;
                            if (matches.Success)
                            {
                                command = matches.Groups["command"].Value.ToString();
                            }
                            switch (command){
//--------------------------------------- пользователи
                                case "create_user":
                                    {
                                        if (Main.Sess.user_name == "admin")
                                        {
                                            function_user.Create_user(Program.myForm.Command.Text);
                                        } else
                                        {
                                            Program.myForm.Log.Text += "Для добавления учетной записи нужны права администратора!\n\n";
                                        }
                                        break;
                                    }
                                case "delete_user":
                                    {
                                        if (Main.Sess.user_name == "admin")
                                        {
                                            function_user.Delete_user(Program.myForm.Command.Text);
                                        } else
                                        {
                                            Program.myForm.Log.Text += "Для удаления учетной записи нужны права администратора!\n\n";
                                        }
                                        break;
                                    }
                                case "reset_pass":
                                    {
                                        if (Main.Sess.user_name == "admin")
                                        {
                                            function_user.resetPass(Program.myForm.Command.Text);
                                        } else
                                        {
                                            Program.myForm.Log.Text += "Для изменения пароля учетной записи нужны права администратора!\n\n";
                                        }
                                        break;
                                    }
//--------------------------------------- папки
                                case "create_dir":
                                    {
                                        Regex regex_d = new Regex(@"^(?<command>[A-Za-z_]+)[ ](?<dir>[A-Za-z0-9]+)");
                                        matches = regex_d.Match(Program.myForm.Command.Text);
                                        string dir = String.Empty;
                                        if (matches.Success)
                                        {
                                            dir = matches.Groups["dir"].Value.ToString();
                                            function_dir.create_dir(dir, Sess.path, Sess.user_name);
                                            Serializing.Ser(Super.Blocks);
                                        } else {
                                            Program.myForm.Log.Text += "Некорректое имя директории!\n\n";
                                        }
                                        break;
                                    }
                                case "way":
                                    {
                                        Regex regex_d = new Regex(@"^(?<command>[A-Za-z_]+)[ ](?<dir>[./A-Za-z0-9]+)");
                                        matches = regex_d.Match(Program.myForm.Command.Text);
                                        string dir = String.Empty;
                                        if (matches.Success)
                                        {
                                            dir = matches.Groups["dir"].Value.ToString();
                                            if (dir == "./")
                                            {
                                                string[] match = Sess.path.Split('/');
                                                Sess.path = "";
                                                for (int i = 0; i < match.Count() - 2; i++)
                                                    Sess.path += match[i] + '/';
                                            }
                                            else
                                            {
                                                Sess.path=function_dir.Search_folder(dir, Sess.path);
                                            }
                                        } else {
                                            Program.myForm.Log.Text += "Некорректый путь!\n\n";
                                        }
                                        break;
                                    }
                                case "rename":
                                    {
                                        Regex regex_d = new Regex(@"^(?<command>[A-Za-z_]+)[ ](?<folder>[A-Za-z0-9]+)[|](?<rename>[A-Za-z0-9]+)");
                                        matches = regex_d.Match(Program.myForm.Command.Text);
                                        string dir = String.Empty;
                                        if (matches.Success)
                                        {
                                            dir = matches.Groups["folder"].Value.ToString();
                                            string rename = matches.Groups["rename"].Value.ToString();
                                            {
                                                string c = Sess.path + dir + "/";
                                                function_dir.ren_folder(c,rename);
                                                Serializing.Ser(Super.Blocks);
                                            }
                                        } else {
                                            Program.myForm.Log.Text += "Некорректно введена команда!\nВведите rename <oldName>|<newName>\n\n";
                                        }
                                        break;
                                    }
                                case "del_dir":
                                {
                                    Regex regex_d = new Regex(@"^(?<command>[A-Za-z_]+)[ ](?<folder>[A-Za-z0-9]+)");
                                    matches = regex_d.Match(Program.myForm.Command.Text);
                                    string dir = String.Empty;
                                    if (matches.Success)
                                    {
                                        dir = matches.Groups["folder"].Value.ToString(); 
                                        {
                                            function_dir.Dell_folder(Sess.path, dir);
                                            Serializing.Ser(Super.Blocks);
                                        }
                                    } else {
                                        Program.myForm.Log.Text += "Некорректно введено имя директории!\n\n";
                                    }
                                    break;
                                }

//--------------------------------------- файлы
                                case "create_file":
                                {
                                    Regex regex_d = new Regex(@"^(?<command>[A-Za-z_]+)[ ](?<fil>[A-Za-z0-9]+)");
                                    matches = regex_d.Match(Program.myForm.Command.Text);
                                    string fil = String.Empty;
                                    if (matches.Success)
                                    {
                                        fil = matches.Groups["fil"].Value.ToString();
                                        function_file.create_file(fil, Sess.path, Sess.user_name);
                                        Serializing.Ser(Super.Blocks);
                                    } else {
                                        Program.myForm.Log.Text += "Некорректно введено имя файла!\n\n";
                                    }
                                    break;
                                }
                                case "del_file":
                                {
                                    Regex regex_d = new Regex(@"^(?<command>[A-Za-z_]+)[ ](?<fil>[A-Za-z0-9]+)");
                                    matches = regex_d.Match(Program.myForm.Command.Text);
                                    string fil = String.Empty;
                                    if (matches.Success){
                                        fil = matches.Groups["fil"].Value.ToString();
                                        {
                                            string c = Sess.path;
                                            function_dir.Dell_folder(c, fil);
                                            Serializing.Ser(Super.Blocks);
                                        }
                                    } else {
                                        Program.myForm.Log.Text += "Некорректно введено имя файла!\n\n";
                                    }
                                    break;
                                }
                                case "insert_file":
                                    {
                                        Regex regex_d = new Regex(@"^(?<command>[A-Za-z_]+)[ ](?<files>[A-Za-z0-9]+)");
                                        matches = regex_d.Match(Program.myForm.Command.Text);
                                        string dir = String.Empty;
                                        if (matches.Success)
                                        {
                                            dir = matches.Groups["files"].Value.ToString();
                                            Boolean tr = function_file.Is_file(dir, Main.Sess.path);
                                            if (tr)
                                            {
                                                file f = new file(); //нужный файл
                                                f = function_file.search_file_retf(dir, Main.Sess.path);//поиск нужного файла
                                                                                                        //Если учетная запись админа или если создателю можно записывать, или если другим пользователям можно записывать
                                                if ((Main.Sess.user_name == "admin") ||
                                                    ((f.attributes.di_uid == Main.Sess.user_name) && (function_inode.rights_for_all(f)[1])) ||
                                                    ((function_inode.rights_for_all(f)[3])))
                                                {
                                                    function_file.modify_file(f);
                                                }
                                                else {
                                                    Program.myForm.Log.Text += "Вам не разрешено редактировать этот файл. \nВы не являетесь владельцем данного файла.\n\n";
                                                }
                                            } else {
                                                Program.myForm.Log.Text += "Файл не существует или у вас нет прав для доступа к нему\n\n";
                                            }
                                        } else {
                                            Program.myForm.Log.Text += "Некорректно введено имя файла!\n\n";
                                        }
                                        break;
                                    }
                                        
                                
                                case "edit_file":
                                {
                                    Regex regex_d = new Regex(@"^(?<command>[A-Za-z_]+)[ ](?<files>[A-Za-z0-9]+)");
                                    matches = regex_d.Match(Program.myForm.Command.Text);
                                    string dir = String.Empty;
                                    if (matches.Success)
                                    {
                                        dir = matches.Groups["files"].Value.ToString();
                                        Boolean tr = function_file.Is_file(dir, Main.Sess.path);
                                        if (tr)
                                        {
                                            //Находим текст файла
                                            file f = new file(); //нужный файл
                                            f = function_file.search_file_retf(dir, Main.Sess.path);//поиск нужного файла
                                            //Если учетная запись админа или если создателю можно записывать, или если другим пользователям можно записывать
                                            if ((Main.Sess.user_name == "admin") ||
                                                ((f.attributes.di_uid == Main.Sess.user_name) && (function_inode.rights_for_all(f)[1])) ||
                                                ((function_inode.rights_for_all(f)[3])))
                                            {
                                                string text = function_file.see_file(dir, Main.Sess.path);
                                                function_file.modify_file(f, text);
                                            }
                                            else
                                            {
                                                Program.myForm.Log.Text += "Вам не разрешено редактировать этот файл. \nВы не являетесь владельцем данного файла.\n\n";
                                            }
                                        }
                                        else {
                                            Program.myForm.Log.Text += "Файл не существует или у вас нет прав на его редактирование\n\n";
                                        }
                                        Serializing.Ser(Super.Blocks);
                                    } else {
                                        Program.myForm.Log.Text += "Некорректно введено имя файла!\n\n";
                                    }
                                    break;
                                }
                                case "watch_file":
                                {
                                    Regex regex_d = new Regex(@"^(?<command>[A-Za-z_]+)[ ](?<files>[A-Za-z0-9]+)");
                                    matches = regex_d.Match(Program.myForm.Command.Text);
                                    string dir = String.Empty;
                                    if (matches.Success)
                                    {
                                        dir = matches.Groups["files"].Value.ToString();
                                        Program.myForm.Log.Text += function_file.see_file(dir, Main.Sess.path) + "\n\n";
                                        Serializing.Ser(Super.Blocks);
                                    } else {
                                        Program.myForm.Log.Text += "Некорректно введено имя файла!\n\n";
                                    }
                                    break;
                                }
                                case "attr":
                                {
                                    Regex regex_d = new Regex(@"^(?<command>[A-Za-z_]+)[ ](?<files>[A-Za-z0-9]+)");
                                    matches = regex_d.Match(Program.myForm.Command.Text);
                                    string dir = String.Empty;
                                    if (matches.Success)
                                    {
                                        dir = matches.Groups["files"].Value.ToString();
                                        Program.myForm.Log.Text += function_file.attr(dir, Main.Sess.path);
                                        Serializing.Ser(Super.Blocks);
                                    } else {
                                        Program.myForm.Log.Text += "Некорректно введено имя!\n\n";
                                    }
                                    break;
                                }
                                case "rights":
                                {
                                    Regex regex_d = new Regex(@"^(?<command>[A-Za-z_]+)[ ](?<files>[A-Za-z0-9]+)[ ](?<rights>[r-][w-]-[r-][w-])");
                                    matches = regex_d.Match(Program.myForm.Command.Text);
                                    string dir = String.Empty;
                                    string rights = String.Empty;
                                    if (matches.Success)
                                    {
                                        dir = matches.Groups["files"].Value.ToString();
                                        rights = matches.Groups["rights"].Value.ToString();
                                        function_inode.rights(dir, Main.Sess.path,rights);
                                        Serializing.Ser(Super.Blocks);
                                    } else {
                                        Program.myForm.Log.Text += "Некорректно введено имя или права!\n\n";
                                    }
                                    break;
                                } 
                                default:
                                    {
                                        Program.myForm.Log.Text += Program.myForm.Command.Text + ": Неизвестная команда\n\n";
                                        break;
                                    }
                            }
                            break;
                        }
                }

            }
            else {
                if (Program.myForm.Command.Text != "")
                {
                    bool result = Start(Program.myForm.Command.Text);
                    if(!result) Program.myForm.Log.Text += "Необходимо войти в систему!\n\n";
                }
            }
        }
        public void mount()
        {
            Program.myForm.Log.Text += "Начало монтирования системы\n";
            CountBlocks = (int)(StartMemoryHDD / Blocks_HDD); //Вычисление количества блоков
            inode_count = Blocks_HDD / Inode_Memory; //Вычисление количества инодов
            Program.myForm.Log.Text += "Количество памяти в байтах: " + StartMemoryHDD + "\n";
            Program.myForm.Log.Text += "Размер блока: " + Blocks_HDD + " байт\n";
            Program.myForm.Log.Text += "Размер inode: " + Inode_Memory + " байт\n\n";
            Program.myForm.Log.Text += "Количество созданных блоков: " + CountBlocks + "\n";
            Program.myForm.Log.Text += "Количество inode в блоках: " + inode_count + "\n";
            Program.myForm.Log.Text += "Размер одного кластера: " + claster + "\n\n";

            List<Block> Blocks = new List<Block>(CountBlocks);
            for (int i = 0; i < CountBlocks; i++)
            {
                inode[] ino = new inode[inode_count];
                for (int j = 0; j < inode_count; j++)
                {
                    ino[j] = new inode();

                }
                
                Block inod = new Block(ino, Blocks_HDD/claster);
                for (int j = 0; j < Blocks_HDD / claster; j++)
                {
                    inod.claster[j].SetNumber(i, j);
                }
                int c = 0;
                foreach (inode t in inod.InodeMap)
                {
                    c++;
                    t.inode_number = c;
                }
                Blocks.Add(inod);                
            }
            Super = new Superblock(Blocks);

            Root.List=new List<TypeOf>();
            Root.name="Root";
            Root.attributes=function_inode.Search_free_inode();
            Root.attributes.di_uid = "admin";
            Root.attributes.dimode.type = "folder";
            Root.attributes.dimode.rights = "rw-rw";
            Root.attributes.di_size = 0;
            Root.attributes.free = false;

            Serializing.Ser(Blocks);
        }

        //Авторизация и вход в систему
        public bool Start(string str)
        {
            List<User> t = new List<User>();
            t = Serializing_User.Deser("User.txt");            
            Sess.path = "root/";
            Sess.user_list = t;

            Regex regex = new Regex(@"^(?<login>[A-Za-z0-9]+)[|](?<password>[A-Za-z0-9]+)");
            Match matches = regex.Match(str);
            string name=String.Empty;
            string password = String.Empty;
            if (matches.Success)
            {
                name = matches.Groups["login"].Value.ToString();
                password = matches.Groups["password"].Value.ToString();
            }
            else{
                //throw new Exception("Ошибка в команде!");
                return false;
            }
            return Search_User(name, password);
            
        }
        public bool Search_User(string name, string password)
        {
            for (int i = 0; i < Sess.user_list.Count; i++)
            {
                if ((Sess.user_list[i].user_name == name) && (Sess.user_list[i].password == password))
                {
                    Program.myForm.Log.Text += "Добро пожаловать," + name + "!\n\n";
                    Sess.user_name = Sess.user_list[i].user_name;
                    Sess.user_id = Sess.user_list[i].user_id;
                    return true;
                }

            }
            Program.myForm.Log.Text += "Ошибка в пароле или имени пользователя!\n";
            return false;
        }
        
    }
    public class Superblock
    {
        //стандартные поля суперблока
        public List<Block> Blocks;
        public Superblock(List<Block> Block1) 
        {
            Blocks=Block1;            
        }

        public List<string> FreeInode()
        {
            List<string> str=new List<string>();
            
                for (int i=0; i<Blocks.Count; i++)
                {
                    string r = i.ToString();
                    foreach (inode t in Blocks[i].InodeMap)
                    {

                        if (t.free == true)
                        {
                            str.Add(r+"."+t.inode_number.ToString());
                        }
                    }                    
                }
            return str;

        }
        public List<string> FreeClaster()
        {
            List<string> str = new List<string>();

            for (int i = 0; i < Blocks.Count; i++)
            {
                string r = i.ToString();
                foreach (claster_str t in Blocks[i].claster)
                {

                    if (t.type == true)
                    {
                        str.Add(r + "." + t.number.ToString());
                    }
                }
            }
            return str;

        }
    }

    public class Block
    {       
        public claster_str[] claster; //кол-во занятых кластеров в блоке
        public inode[] InodeMap; //карта занятых-свободных inode

        public Block(inode[] inod,int clast) 
        {
            InodeMap=inod;
            claster=new claster_str[clast];
            for (int i = 0; i < clast; i++)
            {
                claster[i].number = i;
                claster[i].type = true;
            }
        }
               

    }
    
    public class inode //информация о файле
    {
        public int inode_number; // идентификатор инода
        public di_mode /*string*/ dimode; //аттрибуты файла
        public string di_uid; //Идентификаторы владельца
        public string di_gid; //Идентификаторы владельца-группы
        public double di_size; //размер фалйа в байтах
        public DateTime di_atime;	//Время последнего доступа к файлу.
        public DateTime di_mtime;	//Время последней модификации.
        public DateTime di_ctime;	//Время последней модификации inode (кроме модификации полей di_atime, dijntime).
        public Boolean free; // свободен или занят
        public int[] di_addr = new int[13];	//Массив адресов дисковых блоков хранения данных.                        


        public inode()
        {
            inode_number = 1;
            dimode=new di_mode();
                {
                    dimode.type="";//папка или файл (dir или txt)
                    dimode.rights="rw-rw";
                    dimode.attributes="";//
                }; //аттрибуты файла
            di_uid=""; //Идентификаторы владельца
            di_size=2; //размер файла в байтах
            di_atime = DateTime.Now;	//Время последнего доступа к файлу.
            di_mtime = DateTime.Now;	//Время последней модификации.
            di_ctime = DateTime.Now;	//Время последней модификации inode (кроме модификации полей di_atime, dijntime).
            free = true;
            int[] di_addr = new int[13]{0,0,0,0,0,0,0,0,0,0,0,0,0}; //кластеры для адресации
        }
    }

   public struct di_mode
    {
       public string type;//тип файла
       public string rights; //форматированная строка по типу rw---
       public string attributes; // дополнительные аттрибуты выполнения
    }

   public struct claster_str
   {
       public int number;//порядковый номер (адрес)
       public Boolean type; //занят или нет
       public string data; // содержимое

       public void SetNumber(int block, int i)
       {
           string s = block.ToString() + "0" + i.ToString();
           number = int.Parse(s);           
       }
   }

   public class TypeOf
   {
       public inode attributes;
       public string name;

       public TypeOf(inode attribut, string pname)
       { 
           attributes=attribut;
           name=pname;

       }
       public TypeOf()
       {
           attributes = new inode();
           name = "";
       }
   }

   public class file : TypeOf
   {
       public file(inode attribut, string pname)
           : base(attribut, pname)
       {
           attributes = attribut;
           name = pname;
       }

       public file()
       {
           attributes = new inode();
           name = "";
       }


   }
   public class catalog : TypeOf
   {
       public List<TypeOf> List; // список содержащихся в папке файлов  

       public catalog(inode attribut, string pname)
           : base(attribut, pname)
       {
           attributes = attribut;
           name = pname;
           List = new List<TypeOf>();
       }
       public catalog()          
       {
           attributes = new inode();
           name = "";
           List = new List<TypeOf>();
       }

       public List<string> files() // вернуть количество файлов
       {
           List<string> str=new List<string>();
           for (int i = 0; i < List.Count; i++)
           {
               str.Add(List[i].name);
           }
           return str;
       }       
   }
   
   public class Serializing
    {
        public static string Ser(List<Block> Super)
        {
            string path="main_settings.txt";

            try
            {
                //Сериализуем полученное
                
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = ("    ");
                settings.NewLineChars = "\n";
                using (XmlWriter writer = XmlWriter.Create(path, settings))
                {
                    writer.WriteStartElement("Superblock");
                    for (int i = 0; i < Super.Count(); i++)
                    {
                        writer.WriteStartElement("Block");
                        writer.WriteAttributeString("id", i.ToString());
                        for (int k = 0; k < Super[i].claster.Count(); k++)
                        {
                            writer.WriteStartElement("claster");
                            writer.WriteAttributeString("id", k.ToString());
                            writer.WriteAttributeString("number", Super[i].claster[k].number.ToString());
                            writer.WriteAttributeString("type", Super[i].claster[k].type.ToString());
                            writer.WriteValue(Super[i].claster[k].data);
                            writer.WriteEndElement();
                        }

                        for (int j = 0; j < Super[i].InodeMap.Count(); j++)
                        {
                            writer.WriteStartElement("inode_number");
                            writer.WriteAttributeString("id", Super[i].InodeMap[j].inode_number.ToString());

                            writer.WriteStartElement("dimode");
                            {
                                writer.WriteAttributeString("type", Super[i].InodeMap[j].dimode.type);
                                writer.WriteAttributeString("rights", Super[i].InodeMap[j].dimode.rights);
                                writer.WriteAttributeString("attributes", Super[i].InodeMap[j].dimode.attributes);
                            }
                            writer.WriteEndElement();

                            writer.WriteStartElement("di_uid");
                            writer.WriteAttributeString("attr", Super[i].InodeMap[j].di_uid);
                            writer.WriteEndElement();

                            writer.WriteStartElement("di_size");
                            writer.WriteAttributeString("attr", Super[i].InodeMap[j].di_size.ToString());
                            writer.WriteEndElement();

                            writer.WriteStartElement("di_atime");
                            writer.WriteAttributeString("attr", Super[i].InodeMap[j].di_atime.ToString());
                            writer.WriteEndElement();

                            writer.WriteStartElement("di_mtime");
                            writer.WriteAttributeString("attr", Super[i].InodeMap[j].di_mtime.ToString());
                            writer.WriteEndElement();

                            writer.WriteStartElement("di_ctime");
                            writer.WriteAttributeString("attr", Super[i].InodeMap[j].di_ctime.ToString());
                            writer.WriteEndElement();

                            writer.WriteStartElement("free");
                            writer.WriteAttributeString("attr", Super[i].InodeMap[j].free.ToString());
                            writer.WriteEndElement();

                            writer.WriteStartElement("di_addr");
                            for (int k = 0; k < Super[i].InodeMap[j].di_addr.Length; k++)
                            {
                                writer.WriteAttributeString("addr" + k, Super[i].InodeMap[j].di_addr[k].ToString());
                            }
                            writer.WriteEndElement();
                            writer.WriteEndElement();
                        }                           

                        writer.WriteEndElement();
                    }                    
                    writer.WriteEndElement();
                    writer.Flush();
                }

               // Program.myForm.Log.Text += "\n \n Объект успешно сериализован в файл " + path + "\n Операция выполнена";
            }
            catch (Exception exc)
            {
                //Program.myForm.Log.Text += "\n \n Ошибка";
            }
            return path;
        }

        public static Superblock DeSer(string path)
        {
                FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                IFormatter bf = new BinaryFormatter(); 
                fs = new FileStream("1.txt", FileMode.Open, FileAccess.Read);
                Superblock Super = (Superblock)bf.Deserialize(fs);
                fs.Close();
                Program.myForm.Log.Text += "Объект успешно десериализован из файла " + path + "\nОперация выполнена\n\n";

           return Super;
        }
    }
}