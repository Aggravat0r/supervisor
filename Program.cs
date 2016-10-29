using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace OC
{

    static class Program
    {
        public static Form1 myForm;
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            myForm = new Form1();

            Application.Run(myForm);
        }
    }
}



