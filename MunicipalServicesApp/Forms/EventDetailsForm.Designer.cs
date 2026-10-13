using System.Drawing;
using System.Windows.Forms;

namespace MunicipalServicesApp.Forms
{
    partial class EventDetailsForm
    {
        private Panel pnlHeader;
        private Label lblTitle;
        private Label lblSubtitle;
        private Panel pnlBody;
        private Label lblWhen;
        private Label lblValueWhen;
        private Label lblWhere;
        private Label lblValueWhere;
        private Label lblCategory;
        private Label lblValueCategory;
        private Label lblPriority;
        private Label lblDescription;
        private RichTextBox rtbDescription;
        private Button btnClose;

        private void InitializeComponent()
        {
            pnlHeader = new Panel();
            lblTitle = new Label();
            lblSubtitle = new Label();
            pnlBody = new Panel();
            lblPriority = new Label();
            lblWhen = new Label();
            lblValueWhen = new Label();
            lblWhere = new Label();
            lblValueWhere = new Label();
            lblCategory = new Label();
            lblValueCategory = new Label();
            lblDescription = new Label();
            rtbDescription = new RichTextBox();
            btnClose = new Button();
            pnlHeader.SuspendLayout();
            pnlBody.SuspendLayout();
            SuspendLayout();

            pnlHeader.BackColor = UITheme.Navy;
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 96;
            pnlHeader.Name = "pnlHeader";

            lblTitle.AutoEllipsis = true;
            lblTitle.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(24, 16);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(560, 32);
            lblTitle.Text = "Event";

            lblSubtitle.AutoSize = true;
            lblSubtitle.Font = UITheme.SubHeaderFont;
            lblSubtitle.ForeColor = UITheme.Gold;
            lblSubtitle.Location = new Point(24, 54);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Text = "Details";

            pnlBody.BackColor = Color.White;
            pnlBody.Controls.Add(btnClose);
            pnlBody.Controls.Add(rtbDescription);
            pnlBody.Controls.Add(lblDescription);
            pnlBody.Controls.Add(lblPriority);
            pnlBody.Controls.Add(lblValueCategory);
            pnlBody.Controls.Add(lblCategory);
            pnlBody.Controls.Add(lblValueWhere);
            pnlBody.Controls.Add(lblWhere);
            pnlBody.Controls.Add(lblValueWhen);
            pnlBody.Controls.Add(lblWhen);
            pnlBody.Dock = DockStyle.Fill;
            pnlBody.Name = "pnlBody";
            pnlBody.Padding = new Padding(24);

            lblWhen.AutoSize = true;
            lblWhen.Font = UITheme.LabelFont;
            lblWhen.ForeColor = UITheme.Muted;
            lblWhen.Location = new Point(24, 20);
            lblWhen.Text = "When";

            lblValueWhen.AutoSize = true;
            lblValueWhen.Font = UITheme.BodyFont;
            lblValueWhen.ForeColor = UITheme.Text;
            lblValueWhen.Location = new Point(24, 42);
            lblValueWhen.Name = "lblValueWhen";

            lblWhere.AutoSize = true;
            lblWhere.Font = UITheme.LabelFont;
            lblWhere.ForeColor = UITheme.Muted;
            lblWhere.Location = new Point(24, 74);
            lblWhere.Text = "Where";

            lblValueWhere.AutoEllipsis = true;
            lblValueWhere.Font = UITheme.BodyFont;
            lblValueWhere.ForeColor = UITheme.Text;
            lblValueWhere.Location = new Point(24, 96);
            lblValueWhere.Name = "lblValueWhere";
            lblValueWhere.Size = new Size(560, 22);

            lblCategory.AutoSize = true;
            lblCategory.Font = UITheme.LabelFont;
            lblCategory.ForeColor = UITheme.Muted;
            lblCategory.Location = new Point(24, 128);
            lblCategory.Text = "Category";

            lblValueCategory.AutoSize = true;
            lblValueCategory.Font = UITheme.BodyFont;
            lblValueCategory.ForeColor = UITheme.Text;
            lblValueCategory.Location = new Point(24, 150);
            lblValueCategory.Name = "lblValueCategory";

            lblPriority.AutoSize = true;
            lblPriority.Font = UITheme.LabelFont;
            lblPriority.Location = new Point(220, 150);
            lblPriority.Name = "lblPriority";

            lblDescription.AutoSize = true;
            lblDescription.Font = UITheme.LabelFont;
            lblDescription.ForeColor = UITheme.Muted;
            lblDescription.Location = new Point(24, 186);
            lblDescription.Text = "Details";

            rtbDescription.BackColor = Color.White;
            rtbDescription.BorderStyle = BorderStyle.None;
            rtbDescription.Font = UITheme.BodyFont;
            rtbDescription.ForeColor = UITheme.Text;
            rtbDescription.Location = new Point(24, 210);
            rtbDescription.Name = "rtbDescription";
            rtbDescription.ReadOnly = true;
            rtbDescription.Size = new Size(560, 140);
            rtbDescription.TabStop = false;

            btnClose.BackColor = UITheme.Navy;
            btnClose.Cursor = Cursors.Hand;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Font = UITheme.ButtonFont;
            btnClose.ForeColor = Color.White;
            btnClose.Location = new Point(484, 364);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(100, 36);
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = false;
            btnClose.Click += BtnClose_Click;

            AcceptButton = btnClose;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            CancelButton = btnClose;
            ClientSize = new Size(620, 520);
            Controls.Add(pnlBody);
            Controls.Add(pnlHeader);
            Font = UITheme.BodyFont;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "EventDetailsForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = MunicipalityBrand.WindowTitle("Event details");
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlBody.ResumeLayout(false);
            pnlBody.PerformLayout();
            ResumeLayout(false);
        }
    }
}
