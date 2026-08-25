using System.Drawing;
using System.Windows.Forms;

namespace MunicipalServicesApp.Forms
{
    partial class ReportIssuesForm
    {
        private System.ComponentModel.IContainer components = null;
        private Panel pnlHeader;
        private Label lblTitle;
        private Label lblSubtitle;
        private Panel pnlContent;
        private Panel pnlEngagement;
        private Label lblProgressTitle;
        private Label lblEngagement;
        private ProgressBar prgEngagement;
        private Label lblLocation;
        private TextBox txtLocation;
        private Label lblCategory;
        private ComboBox cboCategory;
        private Label lblDescription;
        private RichTextBox rtbDescription;
        private Label lblMedia;
        private Button btnAttachMedia;
        private Label lblAttachment;
        private Button btnSubmit;
        private Button btnBackToMenu;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            pnlHeader = new Panel();
            lblTitle = new Label();
            lblSubtitle = new Label();
            pnlContent = new Panel();
            pnlEngagement = new Panel();
            lblProgressTitle = new Label();
            prgEngagement = new ProgressBar();
            lblEngagement = new Label();
            lblLocation = new Label();
            txtLocation = new TextBox();
            lblCategory = new Label();
            cboCategory = new ComboBox();
            lblDescription = new Label();
            rtbDescription = new RichTextBox();
            lblMedia = new Label();
            btnAttachMedia = new Button();
            lblAttachment = new Label();
            btnSubmit = new Button();
            btnBackToMenu = new Button();
            pnlHeader.SuspendLayout();
            pnlContent.SuspendLayout();
            pnlEngagement.SuspendLayout();
            SuspendLayout();

            pnlHeader.BackColor = UiTheme.Navy;
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Padding = new Padding(28, 16, 28, 16);
            pnlHeader.Size = new Size(900, 108);

            lblTitle.Dock = DockStyle.Top;
            lblTitle.Font = UiTheme.TitleFont;
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(28, 16);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(844, 36);
            lblTitle.Text = "Report an Issue";

            lblSubtitle.Dock = DockStyle.Top;
            lblSubtitle.Font = UiTheme.SubHeaderFont;
            lblSubtitle.ForeColor = UiTheme.Gold;
            lblSubtitle.Location = new Point(28, 52);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(844, 40);
            lblSubtitle.Text = "Tell the municipality where the problem is and what you observed.";

            pnlContent.AutoScroll = true;
            pnlContent.BackColor = UiTheme.Background;
            pnlContent.Controls.Add(btnBackToMenu);
            pnlContent.Controls.Add(btnSubmit);
            pnlContent.Controls.Add(lblAttachment);
            pnlContent.Controls.Add(btnAttachMedia);
            pnlContent.Controls.Add(lblMedia);
            pnlContent.Controls.Add(rtbDescription);
            pnlContent.Controls.Add(lblDescription);
            pnlContent.Controls.Add(cboCategory);
            pnlContent.Controls.Add(lblCategory);
            pnlContent.Controls.Add(txtLocation);
            pnlContent.Controls.Add(lblLocation);
            pnlContent.Controls.Add(pnlEngagement);
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.Location = new Point(0, 108);
            pnlContent.Name = "pnlContent";
            pnlContent.Padding = new Padding(28, 16, 28, 16);

            pnlEngagement.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlEngagement.BackColor = Color.White;
            pnlEngagement.Controls.Add(lblEngagement);
            pnlEngagement.Controls.Add(prgEngagement);
            pnlEngagement.Controls.Add(lblProgressTitle);
            pnlEngagement.Location = new Point(28, 16);
            pnlEngagement.Name = "pnlEngagement";
            pnlEngagement.Padding = new Padding(16);
            pnlEngagement.Size = new Size(844, 108);

            lblProgressTitle.AutoSize = true;
            lblProgressTitle.Font = UiTheme.LabelFont;
            lblProgressTitle.ForeColor = UiTheme.Navy;
            lblProgressTitle.Location = new Point(16, 12);
            lblProgressTitle.Name = "lblProgressTitle";
            lblProgressTitle.Size = new Size(220, 19);
            lblProgressTitle.Text = "Your reporting progress";

            prgEngagement.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            prgEngagement.Location = new Point(16, 38);
            prgEngagement.Maximum = 100;
            prgEngagement.Name = "prgEngagement";
            prgEngagement.Size = new Size(812, 22);
            prgEngagement.Step = 25;
            prgEngagement.Style = ProgressBarStyle.Continuous;
            prgEngagement.TabIndex = 0;

            lblEngagement.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblEngagement.Font = UiTheme.BodyFont;
            lblEngagement.ForeColor = UiTheme.Navy;
            lblEngagement.Location = new Point(16, 68);
            lblEngagement.Name = "lblEngagement";
            lblEngagement.Size = new Size(812, 28);
            lblEngagement.Text = "Start by entering the location of the issue.";

            lblLocation.AutoSize = true;
            lblLocation.Font = UiTheme.LabelFont;
            lblLocation.ForeColor = UiTheme.Text;
            lblLocation.Location = new Point(28, 140);
            lblLocation.Name = "lblLocation";
            lblLocation.Size = new Size(70, 19);
            lblLocation.Text = "Location (street, suburb or landmark)";

            txtLocation.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtLocation.Font = UiTheme.BodyFont;
            txtLocation.Location = new Point(28, 162);
            txtLocation.Name = "txtLocation";
            txtLocation.Size = new Size(844, 25);
            txtLocation.TabIndex = 1;
            txtLocation.TextChanged += TxtLocation_TextChanged;

            lblCategory.AutoSize = true;
            lblCategory.Font = UiTheme.LabelFont;
            lblCategory.ForeColor = UiTheme.Text;
            lblCategory.Location = new Point(28, 200);
            lblCategory.Name = "lblCategory";
            lblCategory.Size = new Size(70, 19);
            lblCategory.Text = "Category";

            cboCategory.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cboCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCategory.Font = UiTheme.BodyFont;
            cboCategory.FormattingEnabled = true;
            cboCategory.Location = new Point(28, 222);
            cboCategory.Name = "cboCategory";
            cboCategory.Size = new Size(844, 25);
            cboCategory.TabIndex = 2;
            cboCategory.SelectedIndexChanged += CboCategory_SelectedIndexChanged;

            lblDescription.AutoSize = true;
            lblDescription.Font = UiTheme.LabelFont;
            lblDescription.ForeColor = UiTheme.Text;
            lblDescription.Location = new Point(28, 260);
            lblDescription.Name = "lblDescription";
            lblDescription.Size = new Size(90, 19);
            lblDescription.Text = "Description";

            rtbDescription.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            rtbDescription.BorderStyle = BorderStyle.FixedSingle;
            rtbDescription.Font = UiTheme.BodyFont;
            rtbDescription.Location = new Point(28, 282);
            rtbDescription.Name = "rtbDescription";
            rtbDescription.Size = new Size(844, 150);
            rtbDescription.TabIndex = 3;
            rtbDescription.TextChanged += RtbDescription_TextChanged;

            lblMedia.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblMedia.AutoSize = true;
            lblMedia.Font = UiTheme.LabelFont;
            lblMedia.ForeColor = UiTheme.Text;
            lblMedia.Location = new Point(28, 444);
            lblMedia.Name = "lblMedia";
            lblMedia.Size = new Size(180, 19);
            lblMedia.Text = "Attach image or document";

            btnAttachMedia.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnAttachMedia.BackColor = Color.White;
            btnAttachMedia.Cursor = Cursors.Hand;
            btnAttachMedia.FlatAppearance.BorderColor = UiTheme.Navy;
            btnAttachMedia.FlatStyle = FlatStyle.Flat;
            btnAttachMedia.Font = UiTheme.ButtonFont;
            btnAttachMedia.ForeColor = UiTheme.Navy;
            btnAttachMedia.Location = new Point(28, 468);
            btnAttachMedia.Name = "btnAttachMedia";
            btnAttachMedia.Size = new Size(180, 38);
            btnAttachMedia.TabIndex = 4;
            btnAttachMedia.Text = "Attach file";
            btnAttachMedia.UseVisualStyleBackColor = false;
            btnAttachMedia.Click += BtnAttachMedia_Click;

            lblAttachment.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblAttachment.Font = UiTheme.BodyFont;
            lblAttachment.ForeColor = UiTheme.Muted;
            lblAttachment.Location = new Point(220, 476);
            lblAttachment.Name = "lblAttachment";
            lblAttachment.Size = new Size(652, 24);
            lblAttachment.Text = "No file attached yet.";

            btnSubmit.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSubmit.BackColor = UiTheme.Navy;
            btnSubmit.Cursor = Cursors.Hand;
            btnSubmit.Enabled = false;
            btnSubmit.FlatAppearance.BorderSize = 0;
            btnSubmit.FlatStyle = FlatStyle.Flat;
            btnSubmit.Font = UiTheme.ButtonFont;
            btnSubmit.ForeColor = Color.White;
            btnSubmit.Location = new Point(692, 526);
            btnSubmit.Name = "btnSubmit";
            btnSubmit.Size = new Size(180, 42);
            btnSubmit.TabIndex = 5;
            btnSubmit.Text = "Submit";
            btnSubmit.UseVisualStyleBackColor = false;
            btnSubmit.Click += BtnSubmit_Click;

            btnBackToMenu.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnBackToMenu.BackColor = Color.White;
            btnBackToMenu.Cursor = Cursors.Hand;
            btnBackToMenu.FlatAppearance.BorderColor = UiTheme.Navy;
            btnBackToMenu.FlatStyle = FlatStyle.Flat;
            btnBackToMenu.Font = UiTheme.ButtonFont;
            btnBackToMenu.ForeColor = UiTheme.Navy;
            btnBackToMenu.Location = new Point(28, 526);
            btnBackToMenu.Name = "btnBackToMenu";
            btnBackToMenu.Size = new Size(180, 42);
            btnBackToMenu.TabIndex = 6;
            btnBackToMenu.Text = "Back to Main Menu";
            btnBackToMenu.UseVisualStyleBackColor = false;
            btnBackToMenu.Click += BtnBackToMenu_Click;

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = UiTheme.Background;
            ClientSize = new Size(900, 700);
            Controls.Add(pnlContent);
            Controls.Add(pnlHeader);
            Font = UiTheme.BodyFont;
            MinimumSize = new Size(820, 640);
            Name = "ReportIssuesForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Municipal Services — Report Issues";
            pnlHeader.ResumeLayout(false);
            pnlContent.ResumeLayout(false);
            pnlContent.PerformLayout();
            pnlEngagement.ResumeLayout(false);
            pnlEngagement.PerformLayout();
            ResumeLayout(false);
        }
    }
}
