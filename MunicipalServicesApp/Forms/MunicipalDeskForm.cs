using System;
using System.Drawing;
using System.Windows.Forms;
using MunicipalServicesApp.Services;

namespace MunicipalServicesApp.Forms
{
    public sealed class MunicipalDeskForm : Form
    {
        private readonly TextBox txtCode;
        private readonly Label lblError;

        public MunicipalDeskForm()
        {
            Text = MunicipalityBrand.WindowTitle("Municipal desk");
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(460, 250);
            BackColor = Color.White;
            Font = UITheme.BodyFont;

            var title = new Label
            {
                Text = "Municipal desk",
                Font = UITheme.TitleFont,
                ForeColor = UITheme.Navy,
                AutoSize = true,
                Location = new Point(24, 20)
            };

            var explain = new Label
            {
                Text = "Residents do not register. This desk is for City of Jozi staff publishing a notice or updating a report. It stays open until you close the app.",
                ForeColor = UITheme.Text,
                Location = new Point(24, 58),
                Size = new Size(412, 52)
            };

            var lblCode = new Label
            {
                Text = "Desk code",
                Font = UITheme.LabelFont,
                AutoSize = true,
                Location = new Point(24, 118)
            };

            txtCode = new TextBox
            {
                Location = new Point(24, 142),
                Size = new Size(250, 27),
                UseSystemPasswordChar = true
            };

            lblError = new Label
            {
                ForeColor = UITheme.Danger,
                Location = new Point(24, 172),
                Size = new Size(412, 22),
                Text = string.Empty
            };

            var btnUnlock = new Button
            {
                Text = "Open desk",
                BackColor = UITheme.Navy,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(286, 138),
                Size = new Size(150, 32)
            };
            btnUnlock.FlatAppearance.BorderSize = 0;
            btnUnlock.Click += BtnUnlock_Click;

            var hint = new Label
            {
                Text = "The sample code for this assignment is JOZI-DESK.",
                ForeColor = UITheme.Muted,
                Location = new Point(24, 204),
                Size = new Size(412, 22)
            };

            AcceptButton = btnUnlock;
            Controls.Add(title);
            Controls.Add(explain);
            Controls.Add(lblCode);
            Controls.Add(txtCode);
            Controls.Add(lblError);
            Controls.Add(btnUnlock);
            Controls.Add(hint);
        }

        private void BtnUnlock_Click(object sender, EventArgs e)
        {
            if (!MunicipalDesk.Matches(txtCode.Text))
            {
                lblError.Text = "That desk code was not recognised.";
                txtCode.SelectAll();
                txtCode.Focus();
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
