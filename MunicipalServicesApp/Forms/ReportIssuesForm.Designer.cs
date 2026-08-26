using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MunicipalServicesApp.Forms
{
    partial class ReportIssuesForm
    {
        private System.ComponentModel.IContainer components = null;
        private Panel pnlShell;
        private Panel pnlCard;
        private Panel pnlHeader;
        private Label lblTitle;
        private Label lblSubtitle;
        private Panel pnlBody;
        private Panel pnlFooter;
        private Panel pnlFooterLine;
        private Label lblProgressTitle;
        private Panel pnlProgressTrack;
        private Panel pnlProgressFill;
        private Label lblEngagement;
        private Label lblLocation;
        private TextBox txtLocation;
        private Label lblLocationHint;
        private Label lblCategoryError;
        private Label lblDescriptionError;
        private ListBox lstLocationSuggestions;
        private Label lblCategory;
        private Panel pnlCategoryHost;
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
            Color navy = Color.FromArgb(26, 54, 93);
            Color gold = Color.FromArgb(240, 180, 60);
            Color muted = Color.FromArgb(120, 120, 120);
            Color text = Color.FromArgb(20, 20, 20);
            Color border = Color.FromArgb(210, 214, 220);
            Color page = Color.FromArgb(232, 234, 238);

            pnlShell = new Panel();
            pnlCard = new Panel();
            pnlHeader = new Panel();
            lblTitle = new Label();
            lblSubtitle = new Label();
            pnlBody = new Panel();
            pnlFooter = new Panel();
            pnlFooterLine = new Panel();
            lblProgressTitle = new Label();
            pnlProgressTrack = new Panel();
            pnlProgressFill = new Panel();
            lblEngagement = new Label();
            lblLocation = new Label();
            txtLocation = new TextBox();
            lblLocationHint = new Label();
            lblCategoryError = new Label();
            lblDescriptionError = new Label();
            lstLocationSuggestions = new ListBox();
            lblCategory = new Label();
            pnlCategoryHost = new Panel();
            cboCategory = new ComboBox();
            lblDescription = new Label();
            rtbDescription = new RichTextBox();
            lblMedia = new Label();
            btnAttachMedia = new Button();
            lblAttachment = new Label();
            btnSubmit = new Button();
            btnBackToMenu = new Button();

            pnlShell.SuspendLayout();
            pnlCard.SuspendLayout();
            pnlHeader.SuspendLayout();
            pnlBody.SuspendLayout();
            pnlFooter.SuspendLayout();
            pnlCategoryHost.SuspendLayout();
            SuspendLayout();

            // Outer page background
            pnlShell.BackColor = page;
            pnlShell.Dock = DockStyle.Fill;
            pnlShell.Name = "pnlShell";
            pnlShell.Padding = new Padding(28, 24, 28, 24);
            pnlShell.Controls.Add(pnlCard);
            pnlShell.Resize += (s, e) => LayoutCard();

            // Floating white card
            pnlCard.BackColor = Color.White;
            pnlCard.Name = "pnlCard";
            pnlCard.Size = new Size(700, 560);
            pnlCard.Controls.Add(pnlBody);
            pnlCard.Controls.Add(pnlFooter);
            pnlCard.Controls.Add(pnlHeader);
            pnlCard.Paint += PnlCard_Paint;

            // Header inside card
            pnlHeader.BackColor = navy;
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 88;
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Padding = new Padding(28, 16, 28, 16);

            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(28, 14);
            lblTitle.Name = "lblTitle";
            lblTitle.Text = "Report an issue";

            lblSubtitle.AutoSize = true;
            lblSubtitle.Font = new Font("Segoe UI", 10F);
            lblSubtitle.ForeColor = gold;
            lblSubtitle.Location = new Point(28, 52);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Text = MunicipalityBrand.ReportSubtitle;

            // Body
            pnlBody.BackColor = Color.White;
            pnlBody.Dock = DockStyle.Fill;
            pnlBody.Name = "pnlBody";
            pnlBody.Padding = new Padding(28, 18, 28, 12);
            pnlBody.AutoScroll = true;
            pnlBody.Controls.Add(lstLocationSuggestions);
            pnlBody.Controls.Add(lblAttachment);
            pnlBody.Controls.Add(btnAttachMedia);
            pnlBody.Controls.Add(lblMedia);
            pnlBody.Controls.Add(lblDescriptionError);
            pnlBody.Controls.Add(rtbDescription);
            pnlBody.Controls.Add(lblDescription);
            pnlBody.Controls.Add(lblCategoryError);
            pnlBody.Controls.Add(pnlCategoryHost);
            pnlBody.Controls.Add(lblCategory);
            pnlBody.Controls.Add(lblLocationHint);
            pnlBody.Controls.Add(txtLocation);
            pnlBody.Controls.Add(lblLocation);
            pnlBody.Controls.Add(lblEngagement);
            pnlBody.Controls.Add(pnlProgressTrack);
            pnlBody.Controls.Add(lblProgressTitle);

            lblProgressTitle.AutoSize = true;
            lblProgressTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblProgressTitle.ForeColor = text;
            lblProgressTitle.Location = new Point(28, 18);
            lblProgressTitle.Name = "lblProgressTitle";
            lblProgressTitle.Text = "Your reporting progress";

            pnlProgressTrack.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlProgressTrack.BackColor = Color.FromArgb(235, 237, 240);
            pnlProgressTrack.Controls.Add(pnlProgressFill);
            pnlProgressTrack.Location = new Point(28, 42);
            pnlProgressTrack.Name = "pnlProgressTrack";
            pnlProgressTrack.Size = new Size(644, 10);

            pnlProgressFill.BackColor = navy;
            pnlProgressFill.Location = new Point(0, 0);
            pnlProgressFill.Name = "pnlProgressFill";
            pnlProgressFill.Size = new Size(0, 10);

            lblEngagement.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblEngagement.Font = new Font("Segoe UI", 9.5F);
            lblEngagement.ForeColor = navy;
            lblEngagement.Location = new Point(28, 58);
            lblEngagement.Name = "lblEngagement";
            lblEngagement.Size = new Size(644, 22);
            lblEngagement.Text = "Start by entering the location of the issue.";

            lblLocation.AutoSize = true;
            lblLocation.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblLocation.ForeColor = text;
            lblLocation.Location = new Point(28, 92);
            lblLocation.Name = "lblLocation";
            lblLocation.Text = "Location (street, suburb or landmark)";

            txtLocation.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtLocation.BorderStyle = BorderStyle.FixedSingle;
            txtLocation.Font = new Font("Segoe UI", 10F);
            txtLocation.Location = new Point(28, 114);
            txtLocation.Name = "txtLocation";
            txtLocation.Size = new Size(644, 25);
            txtLocation.TabIndex = 1;
            txtLocation.TextChanged += TxtLocation_TextChanged;
            txtLocation.KeyDown += TxtLocation_KeyDown;
            txtLocation.Leave += TxtLocation_Leave;

            lblLocationHint.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblLocationHint.Font = new Font("Segoe UI", 8.5F);
            lblLocationHint.ForeColor = muted;
            lblLocationHint.Location = new Point(28, 142);
            lblLocationHint.Name = "lblLocationHint";
            lblLocationHint.Size = new Size(644, 18);
            lblLocationHint.Text = "Type 3 or more letters — South African place suggestions will appear.";

            lstLocationSuggestions.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lstLocationSuggestions.BorderStyle = BorderStyle.FixedSingle;
            lstLocationSuggestions.Font = new Font("Segoe UI", 10F);
            lstLocationSuggestions.IntegralHeight = false;
            lstLocationSuggestions.ItemHeight = 22;
            lstLocationSuggestions.Location = new Point(28, 140);
            lstLocationSuggestions.Name = "lstLocationSuggestions";
            lstLocationSuggestions.Size = new Size(644, 132);
            lstLocationSuggestions.TabIndex = 7;
            lstLocationSuggestions.TabStop = false;
            lstLocationSuggestions.Visible = false;
            lstLocationSuggestions.Click += LstLocationSuggestions_Click;
            lstLocationSuggestions.KeyDown += LstLocationSuggestions_KeyDown;

            lblCategory.AutoSize = true;
            lblCategory.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblCategory.ForeColor = text;
            lblCategory.Location = new Point(28, 168);
            lblCategory.Name = "lblCategory";
            lblCategory.Text = "Category";

            pnlCategoryHost.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlCategoryHost.BackColor = Color.White;
            pnlCategoryHost.BorderStyle = BorderStyle.FixedSingle;
            pnlCategoryHost.Controls.Add(cboCategory);
            pnlCategoryHost.Location = new Point(28, 190);
            pnlCategoryHost.Name = "pnlCategoryHost";
            pnlCategoryHost.Padding = new Padding(4, 2, 4, 2);
            pnlCategoryHost.Size = new Size(644, 30);

            cboCategory.Dock = DockStyle.Fill;
            cboCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCategory.FlatStyle = FlatStyle.Flat;
            cboCategory.Font = new Font("Segoe UI", 10F);
            cboCategory.FormattingEnabled = true;
            cboCategory.IntegralHeight = false;
            cboCategory.ItemHeight = 20;
            cboCategory.Name = "cboCategory";
            cboCategory.TabIndex = 2;
            cboCategory.SelectedIndexChanged += CboCategory_SelectedIndexChanged;

            lblCategoryError.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblCategoryError.Font = new Font("Segoe UI", 8.5F);
            lblCategoryError.ForeColor = Color.FromArgb(176, 42, 55);
            lblCategoryError.Location = new Point(28, 218);
            lblCategoryError.Name = "lblCategoryError";
            lblCategoryError.Size = new Size(644, 18);
            lblCategoryError.Text = string.Empty;
            lblCategoryError.Visible = false;

            lblDescription.AutoSize = true;
            lblDescription.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblDescription.ForeColor = text;
            lblDescription.Location = new Point(28, 242);
            lblDescription.Name = "lblDescription";
            lblDescription.Text = "Description";

            rtbDescription.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            rtbDescription.BorderStyle = BorderStyle.FixedSingle;
            rtbDescription.Font = new Font("Segoe UI", 10F);
            rtbDescription.Location = new Point(28, 264);
            rtbDescription.Name = "rtbDescription";
            rtbDescription.Size = new Size(644, 88);
            rtbDescription.TabIndex = 3;
            rtbDescription.TextChanged += RtbDescription_TextChanged;

            lblDescriptionError.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblDescriptionError.Font = new Font("Segoe UI", 8.5F);
            lblDescriptionError.ForeColor = Color.FromArgb(176, 42, 55);
            lblDescriptionError.Location = new Point(28, 356);
            lblDescriptionError.Name = "lblDescriptionError";
            lblDescriptionError.Size = new Size(644, 18);
            lblDescriptionError.Text = string.Empty;
            lblDescriptionError.Visible = false;

            lblMedia.AutoSize = true;
            lblMedia.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblMedia.ForeColor = text;
            lblMedia.Location = new Point(28, 380);
            lblMedia.Name = "lblMedia";
            lblMedia.Text = "Attach image or document";

            btnAttachMedia.BackColor = Color.White;
            btnAttachMedia.Cursor = Cursors.Hand;
            btnAttachMedia.FlatAppearance.BorderColor = border;
            btnAttachMedia.FlatStyle = FlatStyle.Flat;
            btnAttachMedia.Font = new Font("Segoe UI", 10F);
            btnAttachMedia.ForeColor = text;
            btnAttachMedia.Location = new Point(28, 404);
            btnAttachMedia.Name = "btnAttachMedia";
            btnAttachMedia.Size = new Size(130, 32);
            btnAttachMedia.TabIndex = 4;
            btnAttachMedia.Text = "Attach file";
            btnAttachMedia.UseVisualStyleBackColor = false;
            btnAttachMedia.Click += BtnAttachMedia_Click;

            lblAttachment.Font = new Font("Segoe UI", 10F);
            lblAttachment.ForeColor = muted;
            lblAttachment.Location = new Point(170, 410);
            lblAttachment.Name = "lblAttachment";
            lblAttachment.Size = new Size(502, 22);
            lblAttachment.Text = "No file attached yet.";

            // Footer: Back + Submit only
            pnlFooter.BackColor = Color.White;
            pnlFooter.Controls.Add(btnSubmit);
            pnlFooter.Controls.Add(btnBackToMenu);
            pnlFooter.Controls.Add(pnlFooterLine);
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Height = 68;
            pnlFooter.Name = "pnlFooter";
            pnlFooter.Padding = new Padding(28, 0, 28, 0);

            pnlFooterLine.BackColor = border;
            pnlFooterLine.Dock = DockStyle.Top;
            pnlFooterLine.Height = 1;
            pnlFooterLine.Name = "pnlFooterLine";

            btnBackToMenu.BackColor = Color.White;
            btnBackToMenu.Cursor = Cursors.Hand;
            btnBackToMenu.FlatAppearance.BorderColor = border;
            btnBackToMenu.FlatStyle = FlatStyle.Flat;
            btnBackToMenu.Font = new Font("Segoe UI", 10F);
            btnBackToMenu.ForeColor = text;
            btnBackToMenu.Location = new Point(28, 18);
            btnBackToMenu.Name = "btnBackToMenu";
            btnBackToMenu.Size = new Size(160, 36);
            btnBackToMenu.TabIndex = 6;
            btnBackToMenu.Text = "Back to main menu";
            btnBackToMenu.UseVisualStyleBackColor = false;
            btnBackToMenu.Click += BtnBackToMenu_Click;

            btnSubmit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSubmit.BackColor = navy;
            btnSubmit.Cursor = Cursors.Hand;
            btnSubmit.Enabled = true;
            btnSubmit.FlatAppearance.BorderSize = 0;
            btnSubmit.FlatStyle = FlatStyle.Flat;
            btnSubmit.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnSubmit.ForeColor = Color.White;
            btnSubmit.Location = new Point(512, 18);
            btnSubmit.Name = "btnSubmit";
            btnSubmit.Size = new Size(160, 36);
            btnSubmit.TabIndex = 5;
            btnSubmit.Text = "Submit";
            btnSubmit.UseVisualStyleBackColor = false;
            btnSubmit.Click += BtnSubmit_Click;

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = page;
            ClientSize = new Size(820, 680);
            Controls.Add(pnlShell);
            Font = new Font("Segoe UI", 10F);
            MinimumSize = new Size(720, 620);
            Name = "ReportIssuesForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = MunicipalityBrand.WindowTitle("Report Issues");
            Load += (s, e) => LayoutCard();

            pnlProgressTrack.ResumeLayout(false);
            pnlCategoryHost.ResumeLayout(false);
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlBody.ResumeLayout(false);
            pnlBody.PerformLayout();
            pnlFooter.ResumeLayout(false);
            pnlCard.ResumeLayout(false);
            pnlShell.ResumeLayout(false);
            ResumeLayout(false);
        }

        private void LayoutCard()
        {
            if (pnlShell == null || pnlCard == null)
            {
                return;
            }

            int width = Math.Min(720, Math.Max(560, pnlShell.ClientSize.Width - 56));
            int height = Math.Min(600, Math.Max(520, pnlShell.ClientSize.Height - 48));
            pnlCard.Size = new Size(width, height);
            pnlCard.Location = new Point(
                Math.Max(16, (pnlShell.ClientSize.Width - pnlCard.Width) / 2),
                Math.Max(12, (pnlShell.ClientSize.Height - pnlCard.Height) / 2));

            int contentWidth = pnlCard.Width - 56;
            pnlProgressTrack.Width = contentWidth;
            lblEngagement.Width = contentWidth;
            txtLocation.Width = contentWidth;
            lblLocationHint.Width = contentWidth;
            lstLocationSuggestions.Width = contentWidth;
            cboCategory.Width = contentWidth;
            pnlCategoryHost.Width = contentWidth;
            lblCategoryError.Width = contentWidth;
            rtbDescription.Width = contentWidth;
            lblDescriptionError.Width = contentWidth;
            lblAttachment.Width = Math.Max(120, contentWidth - 150);
            btnSubmit.Left = pnlFooter.Width - btnSubmit.Width - 28;
            UpdateProgressFill(prgValue);
        }

        private int prgValue;

        private void UpdateProgressFill(int value)
        {
            prgValue = Math.Max(0, Math.Min(100, value));
            if (pnlProgressTrack == null || pnlProgressFill == null)
            {
                return;
            }

            pnlProgressFill.Width = (int)(pnlProgressTrack.Width * (prgValue / 100.0));
        }

        private void PnlCard_Paint(object sender, PaintEventArgs e)
        {
            var card = (Panel)sender;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (var path = RoundedRect(new Rectangle(0, 0, card.Width - 1, card.Height - 1), 10))
            using (var pen = new Pen(Color.FromArgb(210, 214, 220)))
            {
                e.Graphics.DrawPath(pen, path);
            }
        }

        private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int d = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
