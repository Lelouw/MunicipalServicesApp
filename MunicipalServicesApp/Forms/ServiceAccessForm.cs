using System;
using System.Drawing;
using System.Windows.Forms;
using MunicipalServicesApp.Services;

namespace MunicipalServicesApp.Forms
{
    public sealed class ServiceAccessForm : Form
    {
        private readonly Panel cardCitizen;
        private readonly Panel cardDesk;
        private readonly Panel pnlCode;
        private readonly TextBox txtCode;
        private readonly Label lblError;
        private bool asDesk;

        public ServiceAccessForm()
        {
            Text = MunicipalityBrand.WindowTitle("Service Request Status");
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(560, 430);
            BackColor = Color.White;
            Font = UITheme.BodyFont;

            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 78,
                BackColor = UITheme.Navy
            };
            header.Controls.Add(new Label
            {
                Text = "Service Request Status",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(24, 14),
                BackColor = UITheme.Navy
            });
            header.Controls.Add(new Label
            {
                Text = "Tell us who is opening this page.",
                Font = UITheme.SubHeaderFont,
                ForeColor = UITheme.Gold,
                AutoSize = true,
                Location = new Point(24, 46),
                BackColor = UITheme.Navy
            });

            var prompt = new Label
            {
                Text = "Are you viewing these requests as a resident, or are you at the service desk?",
                Font = UITheme.LabelFont,
                ForeColor = UITheme.Text,
                Location = new Point(24, 96),
                Size = new Size(510, 22)
            };

            cardCitizen = MakeCard(
                "Resident",
                "Track a reference and read the progress. You cannot change a status.",
                new Point(24, 128));
            cardDesk = MakeCard(
                "Service desk",
                "Update the next status. We will check your desk code against the staff list.",
                new Point(292, 128));
            cardCitizen.Click += delegate { SelectRole(false); };
            cardDesk.Click += delegate { SelectRole(true); };

            pnlCode = new Panel
            {
                Location = new Point(24, 268),
                Size = new Size(512, 78),
                Visible = false
            };
            pnlCode.Controls.Add(new Label
            {
                Text = "Desk code",
                Font = UITheme.LabelFont,
                AutoSize = true,
                Location = new Point(0, 0)
            });
            txtCode = new TextBox
            {
                Location = new Point(0, 24),
                Size = new Size(240, 27),
                UseSystemPasswordChar = true
            };
            pnlCode.Controls.Add(txtCode);
            pnlCode.Controls.Add(new Label
            {
                Text = "JOZI-DESK shows Jozi. The other codes show their own area.",
                ForeColor = UITheme.Muted,
                Location = new Point(0, 54),
                Size = new Size(500, 20)
            });

            lblError = new Label
            {
                ForeColor = UITheme.Danger,
                Location = new Point(24, 352),
                Size = new Size(320, 36)
            };

            var btnContinue = new Button
            {
                Text = "Continue",
                BackColor = UITheme.Navy,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = UITheme.ButtonFont,
                Location = new Point(400, 368),
                Size = new Size(136, 38)
            };
            btnContinue.FlatAppearance.BorderSize = 0;
            btnContinue.Click += BtnContinue_Click;

            AcceptButton = btnContinue;
            Controls.Add(header);
            Controls.Add(prompt);
            Controls.Add(cardCitizen);
            Controls.Add(cardDesk);
            Controls.Add(pnlCode);
            Controls.Add(lblError);
            Controls.Add(btnContinue);
            SelectRole(false);
        }

        public bool AsServiceDesk { get; private set; }

        public string DeskCode { get; private set; }

        private void BtnContinue_Click(object sender, EventArgs e)
        {
            if (!asDesk)
            {
                AsServiceDesk = false;
                DialogResult = DialogResult.OK;
                Close();
                return;
            }

            if (!MunicipalDesk.Matches(txtCode.Text))
            {
                lblError.Text = "That code is not on the service-desk list.";
                txtCode.SelectAll();
                txtCode.Focus();
                return;
            }

            AsServiceDesk = true;
            DeskCode = MunicipalDesk.Canonical(txtCode.Text);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void SelectRole(bool desk)
        {
            asDesk = desk;
            lblError.Text = string.Empty;
            pnlCode.Visible = desk;
            cardCitizen.BackColor = desk ? Color.White : Color.FromArgb(239, 246, 255);
            cardDesk.BackColor = desk ? Color.FromArgb(239, 246, 255) : Color.White;
            cardCitizen.Invalidate();
            cardDesk.Invalidate();
            if (desk)
            {
                txtCode.Focus();
            }
        }

        private Panel MakeCard(string title, string detail, Point location)
        {
            var card = new Panel
            {
                Location = location,
                Size = new Size(244, 124),
                BackColor = Color.White,
                Cursor = Cursors.Hand,
                Tag = title
            };
            card.Paint += Card_Paint;
            var lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = UITheme.Navy,
                AutoSize = true,
                Location = new Point(16, 16),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            var lblDetail = new Label
            {
                Text = detail,
                ForeColor = UITheme.Text,
                Location = new Point(16, 46),
                Size = new Size(210, 60),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            lblTitle.Click += delegate { SelectRole(title == "Service desk"); };
            lblDetail.Click += delegate { SelectRole(title == "Service desk"); };
            card.Controls.Add(lblTitle);
            card.Controls.Add(lblDetail);
            return card;
        }

        private void Card_Paint(object sender, PaintEventArgs e)
        {
            var card = (Panel)sender;
            bool selected = (card == cardDesk && asDesk) || (card == cardCitizen && !asDesk);
            Color border = selected ? UITheme.Navy : Color.FromArgb(210, 214, 220);
            int thickness = selected ? 2 : 1;
            using (var pen = new Pen(border, thickness))
            {
                e.Graphics.DrawRectangle(pen, 1, 1, card.Width - 3, card.Height - 3);
            }

            if (selected)
            {
                using (var brush = new SolidBrush(UITheme.Gold))
                {
                    e.Graphics.FillRectangle(brush, 1, 1, 5, card.Height - 3);
                }
            }
        }
    }
}
