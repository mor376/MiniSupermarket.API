using System;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            // Start the application with the login form so user can obtain JWT token
            Application.Run(new FormLogin());
        }
    }
}
