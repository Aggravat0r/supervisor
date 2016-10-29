using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace OC
{
    public class function_inode
    {
        public static inode Search_free_inode()
        {
            inode f = new inode();
            for (int i = 0; i < Main.Super.Blocks.Count; i++) // цикл по поиску блоков
                for (int j = 0; j < Main.Super.Blocks[i].InodeMap.Count(); j++) // цикл по поиску свободного кластера
                {
                    if (Main.Super.Blocks[i].InodeMap[j].free)
                    {
                        return Main.Super.Blocks[i].InodeMap[j];
                    }
                }
            Program.myForm.Log.Text += "Не удалось найти свободный Inode " + "\n Операция не выполнена\n\n";
            return f;
        }
        public static int Search_free_cluster()
        {
            for (int i = 1; i < Main.Super.Blocks.Count; i++) // цикл по поиску блоков
                for (int j = 1; j < Main.Super.Blocks[i].claster.Count(); j++) // цикл по поиску свободного кластера
                {
                    if (Main.Super.Blocks[i].claster[j].type)
                    {
                        return Main.Super.Blocks[i].claster[j].number;
                    }
                }
            Program.myForm.Log.Text += "Не удалось найти свободный кластер " + "\n Операция не выполнена\n\n";
            return 0;
        }
        public static void SetFalse(int t)
        {
            for (int i = 1; i < Main.Super.Blocks.Count; i++) // цикл по поиску блоков
                for (int j = 1; j < Main.Super.Blocks[i].claster.Count(); j++) // цикл по поиску свободного кластера
                {
                    if (Main.Super.Blocks[i].claster[j].number == t)
                    {
                        Main.Super.Blocks[i].claster[j].type = false;
                        return;
                    }
                }
            Program.myForm.Log.Text += "Не удалось найти свободный кластер " + "\n Операция не выполнена\n\n";
        }
        public static void SetFree(int t)
        {
            for (int i = 1; i < Main.Super.Blocks.Count; i++) // цикл по поиску блоков
                for (int j = 1; j < Main.Super.Blocks[i].claster.Count(); j++) // цикл по поиску свободного кластера
                {
                    if (Main.Super.Blocks[i].claster[j].number == t)
                    {
                        Main.Super.Blocks[i].claster[j].type = true;
                    }
                }
            Program.myForm.Log.Text += "Не удалось найти свободный кластер " + "\n Операция не выполнена\n\n";
        }
        public static string ClusterData(int t)
        {
            for (int i = 1; i < Main.Super.Blocks.Count; i++) // цикл по поиску блоков
                for (int j = 1; j < Main.Super.Blocks[i].claster.Count(); j++) // цикл по поиску свободного кластера
                {
                    if (Main.Super.Blocks[i].claster[j].number == t)
                    {
                        return Main.Super.Blocks[i].claster[j].data;
                    }
                }
            Program.myForm.Log.Text += "Не удалось найти свободный кластер " + "\n Операция не выполнена\n\n";
            return "";
        }

        public static void rights(string name, string path, string rights)
        {
            Regex regex_d = new Regex(@"^(?<files>[r-][w-]-[r-][w-])");
            Match matches = regex_d.Match(rights);
            if (matches.Success)
            {
                TypeOf r = function_file.search_Type_Of(name, path);
                //Если учетная запись админа или если создателю можно записывать, или если другим пользователям можно записывать
                if ((Main.Sess.user_name == "admin") ||
                    ((r.attributes.di_uid == Main.Sess.user_name) && (function_inode.rights_for_all(r)[1])) ||
                    ((function_inode.rights_for_all(r)[3])))
                {
                    r.attributes.dimode.rights = rights;
                }
                else{
                    Program.myForm.Log.Text += "У вас нет прав для изменения прав доступа!\n\n";
                }
            }
        }

        public static Boolean[] rights_for_all(TypeOf t)
        {
            Boolean[] ret = new Boolean[4];
            ret[0] = false;
            ret[1] = false;
            ret[2] = false;
            ret[3] = false;
            Regex regex_d = new Regex(@"^(?<reading1>[r-])(?<writing1>[w-])-(?<reading2>[r-])(?<writing2>[w-])");
            Match matches = regex_d.Match(t.attributes.dimode.rights);
            string r1 = String.Empty;
            string w1 = String.Empty;
            string r2 = String.Empty;
            string w2 = String.Empty;
            if (matches.Success)
            {
                r1 = matches.Groups["reading1"].Value.ToString();
                w1 = matches.Groups["writing1"].Value.ToString();
                r2 = matches.Groups["reading2"].Value.ToString();
                w2 = matches.Groups["writing2"].Value.ToString();
                if (r1 == "r") ret[0] = true;
                if (w1 == "w") ret[1] = true;
                if (r2 == "r") ret[0] = true;
                if (w2 == "w") ret[1] = true;
            }
            return ret;
        }
    }
}
