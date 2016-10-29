using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace OC
{
    public  class Session
    {
        public int user_id;
        public string user_name;
        public string path;
        public List<User> user_list;
    }
    public  class User
    {
        public int user_id;
        public string user_name;
        public string password;

        public int p_user_id
            {
                get { return user_id; }
                set { user_id = value; }
            }
        public string p_user_name
            {
                get { return user_name; }
                set { user_name = value; }
            }
        public string p_password
            {
                get { return password; }
                set { password = value; }
            }
        
    }

    public static class Serializing_User
    {
        public static string Ser(List<User> t)
        {
           string path = "User.txt";

           try{
                XmlSerializer writer = new XmlSerializer(typeof(List<User>));
                FileStream sw = new FileStream(path,FileMode.Create,FileAccess.ReadWrite);
                StreamWriter file = new StreamWriter(sw);
                writer.Serialize(file, t);
                file.Close();
                Program.myForm.Log.Text += "\n\nСписок пользователей успешно сериализован в файл " + path + "\nОперация выполнена\n";
            }
            catch (Exception ex)
            {
                Program.myForm.Log.Text += "Ошибка: " + ex.Message;
            }
            return path;
        }
        public static List<User> Deser(string path)
        {
            List<User> Seria=new List<User>();
            try
            {            
                XmlSerializer serializer = new XmlSerializer(typeof(List<User>));
                StreamReader reader = new StreamReader(path);
                Seria = (List<User>)serializer.Deserialize(reader);
                reader.Close();
            }
            catch (Exception ex)
            {
                Program.myForm.Log.Text += "Ошибка: " + ex.Message;
            }
            return Seria;
        }
    }

}


