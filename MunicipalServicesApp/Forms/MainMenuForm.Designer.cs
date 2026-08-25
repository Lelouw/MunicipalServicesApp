using System.Drawing;
using System.Windows.Forms;

namespace MunicipalServicesApp.Forms
{
    partial class MainMenuForm
    {
        private System.ComponentModel.IContainer components = null;
        private Panel pnlHeader;
        private Panel pnlContent;
        private Label lblTitle;
        private Label lblSubtitle;
        private Label lblInstruction;
        private Button btnReportIssues;
        private Button btnLocalEvents;
        private Button btnServiceStatus;
        private Button btnExit;
        private Label lblPartNote;
        private Label lblDisabledHint;

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
            lblInstruction = new Label();
            btnReportIssues = new Button();
            btnLocalEvents = new Button();
            btnServiceStatus = new Button();
            lblDisabledHint = new Label();
            btnExit = new Button();
            lblPartNote = new Label();
            pnlHeader.SuspendLayout();
            pnlContent.SuspendLayout();
            SuspendLayout();

            pnlHeader.BackColor = UiTheme.Navy;
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Padding = new Padding(28, 20, 28, 20);
            pnlHeader.Size = new Size(860, 130);

            lblTitle.AutoSize = false;
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Font = UiTheme.HeaderFont;
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(28, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(804, 46);
            lblTitle.Text = "Municipal Services";

            lblSubtitle.AutoSize = false;
            lblSubtitle.Dock = DockStyle.Top;
            lblSubtitle.Font = UiTheme.SubHeaderFont;
            lblSubtitle.ForeColor = UiTheme.Gold;
            lblSubtitle.Location = new Point(28, 66);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(804, 44);
            lblSubtitle.Text = "Report issues, follow local announcements, and track service requests.";

            pnlContent.BackColor = UiTheme.Background;
            pnlContent.Controls.Add(lblPartNote);
            pnlContent.Controls.Add(btnExit);
            pnlContent.Controls.Add(lblDisabledHint);
            pnlContent.Controls.Add(btnServiceStatus);
            pnlContent.Controls.Add(btnLocalEvents);
            pnlContent.Controls.Add(btnReportIssues);
            pnlContent.Controls.Add(lblInstruction);
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.Location = new Point(0, 130);
            pnlContent.Name = "pnlContent";
            pnlContent.Padding = new Padding(40, 24, 40, 24);

            lblInstruction.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblInstruction.Font = UiTheme.BodyFont;
            lblInstruction.ForeColor = UiTheme.Text;
            lblInstruction.Location = new Point(40, 24);
            lblInstruction.Name = "lblInstruction";
            lblInstruction.Size = new Size(780, 42);
            lblInstruction.Text = "Welcome. Choose a service below. For Part 1, only Report Issues is available.";

            btnReportIssues.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnReportIssues.BackColor = UiTheme.Navy;
            btnReportIssues.Cursor = Cursors.Hand;
            btnReportIssues.FlatAppearance.BorderSize = 0;
            btnReportIssues.FlatStyle = FlatStyle.Flat;
            btnReportIssues.Font = UiTheme.ButtonFont;
            btnReportIssues.ForeColor = Color.White;
            btnReportIssues.Location = new Point(40, 80);
            btnReportIssues.Name = "btnReportIssues";
            btnReportIssues.Size = new Size(780, 58);
            btnReportIssues.TabIndex = 0;
            btnReportIssues.Text = "Report Issues";
            btnReportIssues.UseVisualStyleBackColor = false;
            btnReportIssues.Click += BtnReportIssues_Click;

            btnLocalEvents.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnLocalEvents.BackColor = UiTheme.Disabled;
            btnLocalEvents.Enabled = false;
            btnLocalEvents.FlatAppearance.BorderSize = 0;
            btnLocalEvents.FlatStyle = FlatStyle.Flat;
            btnLocalEvents.Font = UiTheme.ButtonFont;
            btnLocalEvents.ForeColor = Color.White;
            btnLocalEvents.Location = new Point(40, 150);
            btnLocalEvents.Name = "btnLocalEvents";
            btnLocalEvents.Size = new Size(780, 58);
            btnLocalEvents.TabIndex = 1;
            btnLocalEvents.Text = "Local Events and Announcements";
            btnLocalEvents.UseVisualStyleBackColor = false;

            btnServiceStatus.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnServiceStatus.BackColor = UiTheme.Disabled;
            btnServiceStatus.Enabled = false;
            btnServiceStatus.FlatAppearance.BorderSize = 0;
            btnServiceStatus.FlatStyle = FlatStyle.Flat;
            btnServiceStatus.Font = UiTheme.ButtonFont;
            btnServiceStatus.ForeColor = Color.White;
            btnServiceStatus.Location = new Point(40, 220);
            btnServiceStatus.Name = "btnServiceStatus";
            btnServiceStatus.Size = new Size(780, 58);
            btnServiceStatus.TabIndex = 2;
            btnServiceStatus.Text = "Service Request Status";
            btnServiceStatus.UseVisualStyleBackColor = false;

            lblDisabledHint.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblDisabledHint.Font = UiTheme.BodyFont;
            lblDisabledHint.ForeColor = UiTheme.Muted;
            lblDisabledHint.Location = new Point(40, 288);
            lblDisabledHint.Name = "lblDisabledHint";
            lblDisabledHint.Size = new Size(780, 36);
            lblDisabledHint.Text = "Local Events and Announcements and Service Request Status will be enabled in Part 2.";

            btnExit.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnExit.BackColor = Color.White;
            btnExit.Cursor = Cursors.Hand;
            btnExit.FlatAppearance.BorderColor = UiTheme.Navy;
            btnExit.FlatStyle = FlatStyle.Flat;
            btnExit.Font = UiTheme.ButtonFont;
            btnExit.ForeColor = UiTheme.Navy;
            btnExit.Location = new Point(680, 430);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(140, 42);
            btnExit.TabIndex = 3;
            btnExit.Text = "Exit";
            btnExit.UseVisualStyleBackColor = false;
            btnExit.Click += BtnExit_Click;

            lblPartNote.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblPartNote.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            lblPartNote.ForeColor = UiTheme.Muted;
            lblPartNote.Location = new Point(40, 438);
            lblPartNote.Name = "lblPartNote";
            lblPartNote.Size = new Size(500, 28);
            lblPartNote.Text = "AAPD7112W Part 1 — Report Issues";

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = UiTheme.Background;
            ClientSize = new Size(860, 620);
            Controls.Add(pnlContent);
            Controls.Add(pnlHeader);
            Font = UiTheme.BodyFont;
            MinimumSize = new Size(780, 560);
            Name = "MainMenuForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Municipal Services — Main Menu";
            pnlHeader.ResumeLayout(false);
            pnlContent.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
