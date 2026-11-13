using System;
using System.Drawing;
using System.Windows.Forms;
using MunicipalServicesApp.Data;
using MunicipalServicesApp.Models;
using MunicipalServicesApp.Services;

namespace MunicipalServicesApp.Forms
{
    public sealed class NotificationForm : Form
    {
        private readonly ListView lvNotifications;
        private readonly TextBox txtWatch;
        private readonly Label lblEmpty;
        private readonly Label lblHint;
        private bool bindingList;

        public NotificationForm()
        {
            Text = MunicipalityBrand.WindowTitle("Notifications");
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(520, 480);
            BackColor = UITheme.Background;
            Font = UITheme.BodyFont;

            var title = new Label
            {
                Text = "Your updates",
                Font = UITheme.TitleFont,
                ForeColor = UITheme.Navy,
                AutoSize = true,
                Location = new Point(20, 16)
            };

            lblHint = new Label
            {
                Text = "Reports you submit on this computer are watched for you. No account is required.",
                ForeColor = UITheme.Muted,
                Location = new Point(20, 48),
                Size = new Size(480, 20)
            };

            lvNotifications = new ListView
            {
                Location = new Point(20, 76),
                Size = new Size(480, 250),
                View = View.Details,
                FullRowSelect = true,
                HeaderStyle = ColumnHeaderStyle.None,
                BorderStyle = BorderStyle.None,
                BackColor = Color.White,
                MultiSelect = false
            };
            lvNotifications.Columns.Add("Update", 460);
            lvNotifications.SelectedIndexChanged += LvNotifications_SelectedIndexChanged;

            lblEmpty = new Label
            {
                Text = "No updates yet. Submit a report, or watch a reference below.",
                ForeColor = UITheme.Muted,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Italic),
                Location = new Point(32, 160),
                Size = new Size(440, 40)
            };

            var btnRead = new Button
            {
                Text = "Mark all read",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = UITheme.Navy,
                Location = new Point(350, 334),
                Size = new Size(150, 30)
            };
            btnRead.Click += delegate
            {
                NotificationCentre.Instance.MarkAllRead();
                BindList();
            };

            var lblWatch = new Label
            {
                Text = "Watch a reference",
                Font = UITheme.LabelFont,
                AutoSize = true,
                Location = new Point(20, 378)
            };

            txtWatch = new TextBox
            {
                Location = new Point(20, 402),
                Size = new Size(250, 27)
            };

            var btnWatch = new Button
            {
                Text = "Watch",
                BackColor = UITheme.Navy,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(280, 400),
                Size = new Size(100, 30)
            };
            btnWatch.FlatAppearance.BorderSize = 0;
            btnWatch.Click += BtnWatch_Click;

            Controls.Add(title);
            Controls.Add(lblHint);
            Controls.Add(lvNotifications);
            Controls.Add(lblEmpty);
            Controls.Add(btnRead);
            Controls.Add(lblWatch);
            Controls.Add(txtWatch);
            Controls.Add(btnWatch);
            BindList();
        }

        private void BtnWatch_Click(object sender, EventArgs e)
        {
            string message;
            bool watched = NotificationCentre.Instance.TryWatch(txtWatch.Text, out message);
            MessageBox.Show(this, message, "Notifications", MessageBoxButtons.OK,
                watched ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (watched)
            {
                txtWatch.Clear();
                BindList();
            }
        }

        private void LvNotifications_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (bindingList || lvNotifications.SelectedItems.Count == 0)
            {
                return;
            }

            var notice = lvNotifications.SelectedItems[0].Tag as ResidentNotification;
            if (notice == null)
            {
                return;
            }

            NotificationCentre.Instance.MarkRead(notice.Id);
            BindList();
            IssueReport report = FindReport(notice.RequestId);
            if (report == null)
            {
                return;
            }

            using (var details = new IssueDetailsForm(report))
            {
                details.ShowDialog(this);
            }
        }

        private void BindList()
        {
            bindingList = true;
            lvNotifications.Items.Clear();
            var notices = NotificationCentre.Instance.Notifications;
            for (int i = 0; i < notices.Count; i++)
            {
                ResidentNotification notice = notices[i];
                string prefix = notice.IsRead ? string.Empty : "New  ·  ";
                var item = new ListViewItem(prefix + notice.CreatedAt.ToString("dd MMM HH:mm") + "  ·  " + notice.Title + " — " + notice.Message);
                item.Tag = notice;
                if (!notice.IsRead)
                {
                    item.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
                }

                lvNotifications.Items.Add(item);
            }

            lblEmpty.Visible = notices.Count == 0;
            lvNotifications.Visible = notices.Count > 0;
            bindingList = false;
        }

        private static IssueReport FindReport(string requestId)
        {
            var issues = IssueRepository.Instance.Issues;
            for (int i = 0; i < issues.Count; i++)
            {
                if (string.Equals(issues[i].RequestId, requestId, StringComparison.OrdinalIgnoreCase))
                {
                    return issues[i];
                }
            }

            return null;
        }
    }
}
