using System.Drawing;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    internal sealed class FormModulePlaceholder : Form
    {
        public FormModulePlaceholder(string title, string message)
        {
            BackColor = Color.FromArgb(244, 245, 247);
            Padding = new Padding(32);

            var heading = new Label
            {
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(24, 30, 48),
                Height = 48,
                Text = title
            };

            var description = new Label
            {
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.DimGray,
                Height = 40,
                Text = message
            };

            Controls.Add(description);
            Controls.Add(heading);
        }
    }
}
