using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OC
{
    public class function_user
    {
        public static void Create_user(string str)
        {
            Regex regex = new Regex(@"^(?<command>[A-Za-z_]+)[ ](?<login>[A-Za-z0-9]+)[|](?<password>[A-Za-z0-9]+)");
            Match matches = regex.Match(str);
            string name = String.Empty;
            string password = String.Empty;
            if (matches.Success)
            {
                name = matches.Groups["login"].Value.ToString();
                password = matches.Groups["password"].Value.ToString();

                User user = getUser(name);
                //Если такого пользователя еще нет...
                if (user == null)
                {
                    OC.User t = new OC.User();
                    t.user_id = Main.Sess.user_list.Count + 1;
                    t.user_name = name;
                    t.password = password;
                    Main.Sess.user_list.Add(t);
                    Serializing_User.Ser(Main.Sess.user_list);
                    Program.myForm.Log.Text += "Пользователь внесен в базу!\n\n";
                }
                else
                {
                    Program.myForm.Log.Text += "Пользователь уже существует!\n\n";
                }
            }
            else
            {
                Program.myForm.Log.Text += "Ошибка в команде!\n\n";
            }
        }
        public static Boolean Delete_user(string str)
        {
            Regex regex = new Regex(@"^(?<command>[A-Za-z_]+)[ ](?<login>[A-Za-z0-9]+)");
            Match matches = regex.Match(str);
            string name = String.Empty;
            if (matches.Success)
            {
                name = matches.Groups["login"].Value.ToString();

                User user = getUser(name);
                if (user != null)
                {
                    Main.Sess.user_list.Remove(user);
                    Serializing_User.Ser(Main.Sess.user_list);
                    Program.myForm.Log.Text += "Пользователь удален!\n\n";
                }
                else
                {
                    Program.myForm.Log.Text += "Пользователь не существует!\n\n";
                    return false;
                }
                return true;
            }
            else
            {
                throw new Exception("Ошибка в команде!\n\n");
            }
        }
        public static Boolean resetPass(string str)
        {
            Regex regex = new Regex(@"^(?<command>[A-Za-z_]+)[ ](?<login>[A-Za-z0-9]+)[|](?<password>[A-Za-z0-9]+)[|](?<newpassword>[A-Za-z0-9]+)");
            Match matches = regex.Match(str);
            string name = String.Empty;
            string password = String.Empty;
            string newpassword = String.Empty;

            if (matches.Success)
            {
                name = matches.Groups["login"].Value.ToString();
                password = matches.Groups["password"].Value.ToString();
                newpassword = matches.Groups["newpassword"].Value.ToString();

                User user = getUser(name);
                if (user != null)
                {
                    //Если пользователь есть и пароли совпадают
                    if (user.password == password)
                    {
                        user.password = newpassword;
                        Serializing_User.Ser(Main.Sess.user_list);
                        Program.myForm.Log.Text += "Пароль изменен!\n\n";
                    }
                    else
                    {
                        Program.myForm.Log.Text += "Неверный пароль!\n\n";
                    }
                }
                else
                {
                    Program.myForm.Log.Text += "Пользователя не существует!\n\n";
                    return false;
                }
                return true;
            }
            else
            {
                Program.myForm.Log.Text += "Ошибка в команде!\n\n";
                return false;
            }
        }
        public static void new_session()
        {
            List<OC.User> t = Main.Sess.user_list;
            string name = Main.Sess.user_name;
            Main.Sess.user_name = "";
            Main.Sess.user_id = 0;
            Main.Sess.path = "root/";
            Program.myForm.Log.Text += "Работа пользователя " + name + " завершена!\n\n";

        }
        public static User getUser(string name)
        {
            for (int i = 0; i < Main.Sess.user_list.Count; i++)
            {
                if ((Main.Sess.user_list[i].user_name == name))
                {
                    return Main.Sess.user_list[i];
                }
            }
            return null;
        }
    }
}