using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace OC
{
    public partial class Form1 : Form
    {
        public Main Sys1 = new Main();
        public static  Identificate y;
        private List<String> prev_comm; //История команд
        private int num_comm = 0; //Номер команд в истории 
        public static file record; //Флаг для выявления записи в файл 
        public static string history; //Вся история консоли
        public Form1()
        {
            InitializeComponent();
            Log.Text = "";
            Command.Text = "";
            y = new Identificate(this);
            prev_comm = new List<String> {""};
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Sys1.MainMenu();
        }

        private void Log_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Accept_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //Выводим текущий path и введенную команду
                Program.myForm.Log.Text += Main.Sess.path + ">" + Program.myForm.Command.Text + "\n";
                //Передаем команду для обработки
                Sys1.MainMenu();
                //Добавляем команду в историю команд
                prev_comm.Remove("");
                prev_comm.Add(Program.myForm.Command.Text);
                prev_comm.Add("");
                num_comm = prev_comm.Count - 1;
                //Очищаем поле
                Program.myForm.Command.Text = "";
                //Прокрутка консоли в самый низ
                Program.myForm.Log.SelectionStart = Program.myForm.Log.Text.Length;
                Program.myForm.Log.ScrollToCaret();
            }
            else if (e.KeyCode == Keys.Up)
            {
                num_comm--;
                if (num_comm < 0) num_comm = prev_comm.Count - 1;
                Program.myForm.Command.Text = prev_comm[num_comm];
                //Устанавливаем курсор в конец
                Program.myForm.Command.SelectionStart = Program.myForm.Command.Text.Length;
            }
            else if (e.KeyCode == Keys.Down)
            {
                num_comm++;
                if (num_comm >= prev_comm.Count) num_comm = 0;
                Program.myForm.Command.Text = prev_comm[num_comm];
                //Устанавливаем курсор в конец
                Program.myForm.Command.SelectionStart = Program.myForm.Command.Text.Length;
            }
        }
        private void Command_TextChanged(object sender, EventArgs e)
        {

        }

        private void Record(object sender, KeyEventArgs e)
        {
            //Ввод в консоль для записи в файл
            if ((e.KeyCode == Keys.Escape) && (record != null))
            {
                if (Program.myForm.Log.Text != "") {
                    //Запись в файл
                    function_file.edit_file(record);
                }
                //Восстанавливаем записи консоли
                Program.myForm.Log.Text = history;
                Program.myForm.Log.ReadOnly = true;
                //Прокрутка консоли в самый низ
                Program.myForm.Log.SelectionStart = Program.myForm.Log.Text.Length;
                Program.myForm.Log.ScrollToCaret();

                Program.myForm.Command.ReadOnly = false;
                Program.myForm.Command.Select();
            }
        }
    }

    
}
