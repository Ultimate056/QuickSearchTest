using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuickSearchTest
{
    static class Program
    {
        public static string mainConnection = @"Server=DBSRV\DBSRV;Database=test;Integrated Security=SSPI;Connect Timeout=600";

        public static readonly int lengthToken = 3;
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
