using System.Drawing;
using System.Windows.Forms;

namespace MunicipalServicesApp.Forms
{
    partial class IssueDetailsForm
    {
        private System.ComponentModel.IContainer components = null;
        private Panel pnlHeader;
        private Label lblTitle;
        private Label lblSubtitle;
        private Panel pnlBody;
        private Label lblStatus;
        private Label lblRef;
        private Label lblValueRef;
        private Label lblCategory;
        private Label lblValueCategory;
        private Label lblLocation;
        private Label lblValueLocation;
        private Label lblDate;
        private Label lblValueDate;
        private Label lblDescription;
        private RichTextBox rtbDescription;
        private Label lblAttachmentHeading;
        private Label lblAttachmentName;
        private Button btnOpenAttachment;
        private PictureBox picAttachment;
        private Button btnClose;
        private Panel pnlStatus;
        private Label lblStatusUpdate;
        private Button btnPending;
        private Button btnInProgress;
        private Button btnResolved;

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
            Color muted = Color.FromArgb(107, 114, 128);
            Color text = Color.FromArgb(17, 24, 39);
            Color border = Color.FromArgb(210, 214, 220);

            pnlHeader = new Panel();
            lblTitle = new Label();
            lblSubtitle = new Label();
            pnlBody = new Panel();
            lblStatus = new Label();
            lblRef = new Label();
            lblValueRef = new Label();
            lblCategory = new Label();
            lblValueCategory = new Label();
            lblLocation = new Label();
            lblValueLocation = new Label();
            lblDate = new Label();
            lblValueDate = new Label();
            lblDescription = new Label();
            rtbDescription = new RichTextBox();
            lblAttachmentHeading = new Label();
            lblAttachmentName = new Label();
            btnOpenAttachment = new Button();
            picAttachment = new PictureBox();
            btnClose = new Button();
            pnlStatus = new Panel();
            lblStatusUpdate = new Label();
            btnPending = new Button();
            btnInProgress = new Button();
            btnResolved = new Button();
            pnlHeader.SuspendLayout();
            pnlBody.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picAttachment).BeginInit();
            SuspendLayout();

            pnlHeader.BackColor = navy;
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 88;
            pnlHeader.Name = "pnlHeader";

            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(24, 14);
            lblTitle.Name = "lblTitle";
            lblTitle.Text = "Issue details";

            lblSubtitle.AutoSize = true;
            lblSubtitle.Font = new Font("Segoe UI", 10F);
            lblSubtitle.ForeColor = gold;
            lblSubtitle.Location = new Point(24, 52);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Text = "Reference";

            pnlBody.AutoScroll = true;
            pnlBody.BackColor = Color.White;
            pnlBody.Dock = DockStyle.Fill;
            pnlBody.Name = "pnlBody";
            pnlBody.Padding = new Padding(24, 16, 24, 16);
            pnlBody.Controls.Add(picAttachment);
            pnlBody.Controls.Add(btnOpenAttachment);
            pnlBody.Controls.Add(lblAttachmentName);
            pnlBody.Controls.Add(lblAttachmentHeading);
            pnlBody.Controls.Add(rtbDescription);
            pnlBody.Controls.Add(lblDescription);
            pnlBody.Controls.Add(lblValueDate);
            pnlBody.Controls.Add(lblDate);
            pnlBody.Controls.Add(lblValueLocation);
            pnlBody.Controls.Add(lblLocation);
            pnlBody.Controls.Add(lblValueCategory);
            pnlBody.Controls.Add(lblCategory);
            pnlBody.Controls.Add(lblValueRef);
            pnlBody.Controls.Add(lblRef);
            pnlBody.Controls.Add(lblStatus);

            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            lblStatus.Location = new Point(24, 16);
            lblStatus.MinimumSize = new Size(84, 24);
            lblStatus.Name = "lblStatus";
            lblStatus.Padding = new Padding(10, 4, 10, 4);
            lblStatus.Text = "Pending";
            lblStatus.TextAlign = ContentAlignment.MiddleCenter;

            lblRef.AutoSize = true;
            lblRef.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblRef.ForeColor = muted;
            lblRef.Location = new Point(24, 52);
            lblRef.Name = "lblRef";
            lblRef.Text = "Ref #";

            lblValueRef.AutoSize = true;
            lblValueRef.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblValueRef.ForeColor = text;
            lblValueRef.Location = new Point(24, 70);
            lblValueRef.Name = "lblValueRef";
            lblValueRef.Text = "#";

            lblCategory.AutoSize = true;
            lblCategory.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblCategory.ForeColor = muted;
            lblCategory.Location = new Point(140, 52);
            lblCategory.Name = "lblCategory";
            lblCategory.Text = "Category";

            lblValueCategory.AutoSize = true;
            lblValueCategory.Font = new Font("Segoe UI", 11F);
            lblValueCategory.ForeColor = text;
            lblValueCategory.Location = new Point(140, 70);
            lblValueCategory.Name = "lblValueCategory";
            lblValueCategory.Text = "—";

            lblLocation.AutoSize = true;
            lblLocation.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblLocation.ForeColor = muted;
            lblLocation.Location = new Point(24, 108);
            lblLocation.Name = "lblLocation";
            lblLocation.Text = "Location";

            lblValueLocation.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblValueLocation.Font = new Font("Segoe UI", 11F);
            lblValueLocation.ForeColor = text;
            lblValueLocation.Location = new Point(24, 126);
            lblValueLocation.Name = "lblValueLocation";
            lblValueLocation.Size = new Size(536, 40);
            lblValueLocation.Text = "—";

            lblDate.AutoSize = true;
            lblDate.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblDate.ForeColor = muted;
            lblDate.Location = new Point(24, 172);
            lblDate.Name = "lblDate";
            lblDate.Text = "Date reported";

            lblValueDate.AutoSize = true;
            lblValueDate.Font = new Font("Segoe UI", 11F);
            lblValueDate.ForeColor = text;
            lblValueDate.Location = new Point(24, 190);
            lblValueDate.Name = "lblValueDate";
            lblValueDate.Text = "—";

            lblDescription.AutoSize = true;
            lblDescription.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblDescription.ForeColor = muted;
            lblDescription.Location = new Point(24, 228);
            lblDescription.Name = "lblDescription";
            lblDescription.Text = "Description";

            rtbDescription.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            rtbDescription.BackColor = Color.FromArgb(248, 249, 250);
            rtbDescription.BorderStyle = BorderStyle.None;
            rtbDescription.Font = new Font("Segoe UI", 10.5F);
            rtbDescription.Location = new Point(24, 250);
            rtbDescription.Name = "rtbDescription";
            rtbDescription.ReadOnly = true;
            rtbDescription.Size = new Size(536, 90);
            rtbDescription.TabStop = false;

            lblAttachmentHeading.AutoSize = true;
            lblAttachmentHeading.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblAttachmentHeading.ForeColor = muted;
            lblAttachmentHeading.Location = new Point(24, 352);
            lblAttachmentHeading.Name = "lblAttachmentHeading";
            lblAttachmentHeading.Text = "Attachment";

            lblAttachmentName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblAttachmentName.Font = new Font("Segoe UI", 10.5F);
            lblAttachmentName.ForeColor = text;
            lblAttachmentName.Location = new Point(24, 372);
            lblAttachmentName.Name = "lblAttachmentName";
            lblAttachmentName.Size = new Size(360, 24);
            lblAttachmentName.Text = "No file attached.";

            btnOpenAttachment.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOpenAttachment.BackColor = Color.White;
            btnOpenAttachment.Cursor = Cursors.Hand;
            btnOpenAttachment.FlatAppearance.BorderColor = border;
            btnOpenAttachment.FlatStyle = FlatStyle.Flat;
            btnOpenAttachment.Font = new Font("Segoe UI", 9.5F);
            btnOpenAttachment.ForeColor = navy;
            btnOpenAttachment.Location = new Point(400, 368);
            btnOpenAttachment.Name = "btnOpenAttachment";
            btnOpenAttachment.Size = new Size(160, 30);
            btnOpenAttachment.Text = "Open attachment";
            btnOpenAttachment.UseVisualStyleBackColor = false;
            btnOpenAttachment.Click += BtnOpenAttachment_Click;

            picAttachment.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            picAttachment.BackColor = Color.FromArgb(245, 247, 250);
            picAttachment.BorderStyle = BorderStyle.FixedSingle;
            picAttachment.Location = new Point(24, 410);
            picAttachment.Name = "picAttachment";
            picAttachment.Size = new Size(536, 180);
            picAttachment.SizeMode = PictureBoxSizeMode.Zoom;
            picAttachment.Visible = false;

            StyleStatusButton(btnPending, "Pending", new Point(16, 36));
            StyleStatusButton(btnInProgress, "In Progress", new Point(130, 36));
            StyleStatusButton(btnResolved, "Resolved", new Point(244, 36));

            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.BackColor = navy;
            btnClose.Cursor = Cursors.Hand;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnClose.ForeColor = Color.White;
            btnClose.Location = new Point(424, 32);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(150, 34);
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = false;
            btnClose.Click += BtnClose_Click;

            lblStatusUpdate.AutoSize = true;
            lblStatusUpdate.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblStatusUpdate.ForeColor = muted;
            lblStatusUpdate.Location = new Point(24, 8);
            lblStatusUpdate.Text = "Municipal desk — set status";

            pnlStatus.BackColor = Color.FromArgb(248, 249, 250);
            pnlStatus.Controls.Add(lblStatusUpdate);
            pnlStatus.Controls.Add(btnPending);
            pnlStatus.Controls.Add(btnInProgress);
            pnlStatus.Controls.Add(btnResolved);
            pnlStatus.Controls.Add(btnClose);
            pnlStatus.Dock = DockStyle.Bottom;
            pnlStatus.Height = 80;
            pnlStatus.Name = "pnlStatus";

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(600, 660);
            Controls.Add(pnlBody);
            Controls.Add(pnlStatus);
            Controls.Add(pnlHeader);
            Font = new Font("Segoe UI", 10F);
            MinimizeBox = false;
            MinimumSize = new Size(540, 560);
            Name = "IssueDetailsForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = MunicipalityBrand.WindowTitle("Issue details");
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlBody.ResumeLayout(false);
            pnlBody.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picAttachment).EndInit();
            ResumeLayout(false);
        }

        private void StyleStatusButton(Button button, string status, Point location)
        {
            button.BackColor = Color.White;
            button.Cursor = Cursors.Hand;
            button.FlatAppearance.BorderColor = Color.FromArgb(26, 54, 93);
            button.FlatStyle = FlatStyle.Flat;
            button.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button.ForeColor = Color.FromArgb(26, 54, 93);
            button.Location = location;
            button.Size = new Size(108, 32);
            button.Tag = status;
            button.Text = status;
            button.UseVisualStyleBackColor = false;
            button.Click += BtnSetStatus_Click;
        }
    }
}
