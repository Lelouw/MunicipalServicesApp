using System;
using System.Drawing;
using System.Windows.Forms;
using MunicipalServicesApp.Data;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Forms
{
    public sealed class PublishAnnouncementForm : Form
    {
        private readonly TextBox txtTitle;
        private readonly ComboBox cmbCategory;
        private readonly TextBox txtLocation;
        private readonly DateTimePicker dtpWhen;
        private readonly ComboBox cmbPriority;
        private readonly RadioButton radAnnouncement;
        private readonly RadioButton radEvent;
        private readonly TextBox txtDescription;
        private readonly Label lblError;

        public PublishAnnouncementForm()
        {
            Text = MunicipalityBrand.WindowTitle("New announcement");
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(520, 660);
            BackColor = Color.White;
            Font = UITheme.BodyFont;

            var title = new Label
            {
                Text = "Publish a notice",
                Font = UITheme.TitleFont,
                ForeColor = UITheme.Navy,
                AutoSize = true,
                Location = new Point(24, 16)
            };

            var explain = new Label
            {
                Text = "The notices already on the calendar were loaded as sample data when the app started. This adds a new one to those same lists, including priority order and the order it was issued.",
                ForeColor = UITheme.Muted,
                Location = new Point(24, 48),
                Size = new Size(472, 48)
            };

            int y = 104;
            Controls.Add(title);
            Controls.Add(explain);
            txtTitle = AddField("Title", y, out y);
            cmbCategory = AddCombo("Category", y, out y);
            LoadCategories();
            txtLocation = AddField("Place", y, out y);
            y = AddLabel("Date", y);
            dtpWhen = new DateTimePicker
            {
                Location = new Point(24, y),
                Size = new Size(220, 27),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd MMM yyyy",
                Value = DateTime.Today.AddDays(1)
            };
            Controls.Add(dtpWhen);
            y += 40;
            cmbPriority = AddCombo("Priority", y, out y);
            cmbPriority.Items.AddRange(new object[] { "Standard", "Notice", "Important", "High", "Urgent" });
            cmbPriority.SelectedIndex = 2;

            radAnnouncement = new RadioButton
            {
                Text = "Announcement",
                Checked = true,
                AutoSize = true,
                Location = new Point(24, y)
            };
            radEvent = new RadioButton
            {
                Text = "Event",
                AutoSize = true,
                Location = new Point(180, y)
            };
            Controls.Add(radAnnouncement);
            Controls.Add(radEvent);
            y += 32;
            y = AddLabel("Details", y);
            txtDescription = new TextBox
            {
                Location = new Point(24, y),
                Size = new Size(472, 80),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };
            Controls.Add(txtDescription);
            y += 88;

            lblError = new Label
            {
                ForeColor = UITheme.Danger,
                Location = new Point(24, y),
                Size = new Size(472, 36)
            };
            Controls.Add(lblError);

            var footer = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 72,
                BackColor = Color.White
            };
            footer.Paint += (sender, args) =>
            {
                using (var line = new Pen(UITheme.Gold, 2))
                {
                    args.Graphics.DrawLine(line, 0, 1, footer.Width, 1);
                }
            };

            var btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                BackColor = Color.White,
                ForeColor = UITheme.Navy,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(228, 18),
                Size = new Size(120, 36)
            };
            btnCancel.FlatAppearance.BorderColor = UITheme.Navy;

            var btnPublish = new Button
            {
                Text = "Publish",
                BackColor = UITheme.Navy,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(358, 18),
                Size = new Size(138, 36)
            };
            btnPublish.FlatAppearance.BorderSize = 0;
            btnPublish.Click += BtnPublish_Click;

            footer.Controls.Add(btnCancel);
            footer.Controls.Add(btnPublish);
            Controls.Add(footer);
            CancelButton = btnCancel;
            AcceptButton = btnPublish;
        }

        private void BtnPublish_Click(object sender, EventArgs e)
        {
            string titleText = txtTitle.Text.Trim();
            string category = cmbCategory.Text.Trim();
            string location = txtLocation.Text.Trim();
            string description = txtDescription.Text.Trim();
            if (titleText.Length == 0 || category.Length == 0 || location.Length == 0 || description.Length == 0)
            {
                lblError.Text = "Enter a title, category, place, and the details.";
                return;
            }

            var item = new LocalEvent
            {
                Title = titleText,
                Category = category,
                Location = location,
                Description = description,
                StartDate = dtpWhen.Value.Date,
                IsAnnouncement = radAnnouncement.Checked,
                Priority = PriorityFromSelection()
            };

            EventRepository.Instance.Publish(item);
            DialogResult = DialogResult.OK;
            Close();
        }

        private int PriorityFromSelection()
        {
            switch (cmbPriority.SelectedIndex)
            {
                case 0:
                    return LocalEvent.PriorityLow;
                case 1:
                    return LocalEvent.PriorityNormal;
                case 3:
                    return LocalEvent.PriorityHigh;
                case 4:
                    return LocalEvent.PriorityUrgent;
                default:
                    return LocalEvent.PriorityImportant;
            }
        }

        private void LoadCategories()
        {
            var categories = EventRepository.Instance.GetCategories();
            for (int i = 0; i < categories.Count; i++)
            {
                cmbCategory.Items.Add(categories[i]);
            }

            if (cmbCategory.Items.Count > 0)
            {
                cmbCategory.SelectedIndex = 0;
            }
        }

        private TextBox AddField(string caption, int y, out int nextY)
        {
            nextY = AddLabel(caption, y);
            var box = new TextBox
            {
                Location = new Point(24, nextY),
                Size = new Size(472, 27)
            };
            Controls.Add(box);
            nextY += 36;
            return box;
        }

        private ComboBox AddCombo(string caption, int y, out int nextY)
        {
            nextY = AddLabel(caption, y);
            var box = new ComboBox
            {
                Location = new Point(24, nextY),
                Size = new Size(280, 27),
                DropDownStyle = ComboBoxStyle.DropDown
            };
            Controls.Add(box);
            nextY += 36;
            return box;
        }

        private int AddLabel(string caption, int y)
        {
            Controls.Add(new Label
            {
                Text = caption,
                Font = UITheme.LabelFont,
                AutoSize = true,
                Location = new Point(24, y)
            });
            return y + 22;
        }
    }
}
