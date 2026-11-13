using System.Drawing;
using System.Windows.Forms;

namespace MunicipalServicesApp.Forms
{
    public sealed class StatusUpdateForm : Form
    {
        public StatusUpdateForm(string reference, string category, string current, string next, string blockedReason)
        {
            Text = MunicipalityBrand.WindowTitle("Update status");
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(540, 280);
            BackColor = Color.White;
            Font = UITheme.BodyFont;

            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 64,
                BackColor = UITheme.Navy
            };
            header.Controls.Add(new Label
            {
                Text = string.IsNullOrEmpty(blockedReason) ? "Mark as " + next : "Cannot update yet",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 18),
                BackColor = UITheme.Navy
            });
            Controls.Add(header);

            Controls.Add(new Label
            {
                Text = reference + "  ·  " + category,
                Font = UITheme.LabelFont,
                ForeColor = UITheme.Navy,
                AutoSize = true,
                Location = new Point(24, 84)
            });

            if (!string.IsNullOrEmpty(blockedReason))
            {
                Controls.Add(new Label
                {
                    Text = blockedReason,
                    ForeColor = UITheme.Text,
                    Location = new Point(24, 120),
                    Size = new Size(392, 80)
                });
                AddButton("Close", 336, true, DialogResult.Cancel);
                CancelButton = AcceptButton;
                return;
            }

            Controls.Add(MakeChip(current, new Point(24, 124), false));
            Controls.Add(new Label
            {
                Text = "→",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = UITheme.Gold,
                AutoSize = true,
                Location = new Point(168, 126)
            });
            Controls.Add(MakeChip(next, new Point(210, 124), true));
            Controls.Add(new Label
            {
                Text = "This records the next update for the resident who is watching this reference.",
                ForeColor = UITheme.Muted,
                Location = new Point(24, 176),
                Size = new Size(392, 40)
            });

            var cancel = AddButton("Cancel", 236, false, DialogResult.Cancel);
            AddButton("Mark as " + next, 336, true, DialogResult.OK);
            CancelButton = cancel;
        }

        private Button AddButton(string text, int left, bool primary, DialogResult result)
        {
            var button = new Button
            {
                Text = text,
                DialogResult = result,
                FlatStyle = FlatStyle.Flat,
                Font = UITheme.ButtonFont,
                Location = new Point(left, 224),
                Size = new Size(primary ? 180 : 90, 36),
                BackColor = primary ? UITheme.Navy : Color.White,
                ForeColor = primary ? Color.White : UITheme.Navy
            };
            button.FlatAppearance.BorderSize = primary ? 0 : 1;
            button.FlatAppearance.BorderColor = UITheme.Navy;
            Controls.Add(button);
            if (primary)
            {
                AcceptButton = button;
            }

            return button;
        }

        private static Label MakeChip(string status, Point location, bool next)
        {
            Color back;
            Color fore;
            StatusBadge.Colors(status, out back, out fore);
            if (next)
            {
                back = UITheme.Navy;
                fore = Color.White;
            }

            return new Label
            {
                Text = status,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                BackColor = back,
                ForeColor = fore,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = location,
                Size = new Size(130, 32)
            };
        }
    }
}
