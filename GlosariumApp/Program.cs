using System;
using System.Windows.Forms;
using GlosariumApp.Forms;

namespace GlosariumApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Menjalankan Form Utama (Code-Only)
            Application.Run(new MainMenuForm());
        }
    }
}