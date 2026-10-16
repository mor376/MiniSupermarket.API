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

            while (true)
            {
                using var loginForm = new FormLogin();
                if (loginForm.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                using var shell = new FormMainShell();
                Application.Run(shell);

                if (!string.IsNullOrEmpty(SessionManager.JwtToken))
                {
                    return;
                }
            }
        }
    }
}