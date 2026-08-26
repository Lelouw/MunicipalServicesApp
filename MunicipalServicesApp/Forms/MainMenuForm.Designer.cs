using System.Drawing;
using System.Windows.Forms;

namespace MunicipalServicesApp.Forms
{
    partial class MainMenuForm
    {
        private System.ComponentModel.IContainer components = null;
        private MenuStrip menuStrip;
        private ToolStripMenuItem servicesMenu;
        private ToolStripMenuItem menuReportIssues;
        private ToolStripMenuItem menuLocalEvents;
        private ToolStripMenuItem menuServiceStatus;
        private ToolStripMenuItem menuExit;
        private ToolStripMenuItem helpMenu;
        private ToolStripMenuItem menuAbout;
        private Panel pnlHeader;
        private Label lblTitle;
        private Label lblSubtitle;
        private Label lblWelcome;
        private Panel cardReportIssues;
        private Panel cardLocalEvents;
        private Panel cardServiceStatus;
        private ToolTip cardTooltips;
        private Label lblRequestsHeading;
        private Label lblRequestsSubheading;
        private ComboBox cmbStatusFilter;
        private TextBox txtSearch;
        private ListView lvRequests;
        private Label lblNoRequests;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }

                if (cardTooltips != null)
                {
                    cardTooltips.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            cardTooltips = new ToolTip(components);
            menuStrip = new MenuStrip();
            servicesMenu = new ToolStripMenuItem();
            menuReportIssues = new ToolStripMenuItem();
            menuLocalEvents = new ToolStripMenuItem();
            menuServiceStatus = new ToolStripMenuItem();
            menuExit = new ToolStripMenuItem();
            helpMenu = new ToolStripMenuItem();
            menuAbout = new ToolStripMenuItem();
            pnlHeader = new Panel();
            lblTitle = new Label();
            lblSubtitle = new Label();
            lblWelcome = new Label();
            lblRequestsHeading = new Label();
            lblRequestsSubheading = new Label();
            txtSearch = new TextBox();
            cmbStatusFilter = new ComboBox();
            lvRequests = new ListView();
            lblNoRequests = new Label();
            menuStrip.SuspendLayout();
            pnlHeader.SuspendLayout();
            SuspendLayout();

            menuStrip.Items.AddRange(new ToolStripItem[] { servicesMenu, helpMenu });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(820, 24);
            menuStrip.TabIndex = 0;

            servicesMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
                menuReportIssues,
                menuLocalEvents,
                menuServiceStatus,
                new ToolStripSeparator(),
                menuExit
            });
            servicesMenu.Name = "servicesMenu";
            servicesMenu.Text = "Services";

            menuReportIssues.Name = "menuReportIssues";
            menuReportIssues.Text = "Report Issues";
            menuReportIssues.Click += BtnReportIssues_Click;

            menuLocalEvents.Enabled = false;
            menuLocalEvents.Name = "menuLocalEvents";
            menuLocalEvents.Text = "Local Events and Announcements";

            menuServiceStatus.Enabled = false;
            menuServiceStatus.Name = "menuServiceStatus";
            menuServiceStatus.Text = "Service Request Status";

            menuExit.Name = "menuExit";
            menuExit.Text = "Exit";
            menuExit.Click += BtnExit_Click;

            helpMenu.DropDownItems.Add(menuAbout);
            helpMenu.Name = "helpMenu";
            helpMenu.Text = "Help";

            menuAbout.Name = "menuAbout";
            menuAbout.Text = "About";
            menuAbout.Click += MenuAbout_Click;

            pnlHeader.BackColor = UITheme.Navy;
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 24);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(820, 96);

            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 17F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(24, 12);
            lblTitle.Name = "lblTitle";
            lblTitle.Text = MunicipalityBrand.Name;

            lblSubtitle.AutoSize = false;
            lblSubtitle.Font = UITheme.SubHeaderFont;
            lblSubtitle.ForeColor = UITheme.Gold;
            lblSubtitle.Location = new Point(24, 48);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(772, 36);
            lblSubtitle.Text = MunicipalityBrand.Slogan;

            lblWelcome.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblWelcome.AutoSize = false;
            lblWelcome.Font = new Font("Segoe UI", 10F, FontStyle.Italic);
            lblWelcome.ForeColor = UITheme.Muted;
            lblWelcome.Location = new Point(24, 120);
            lblWelcome.Name = "lblWelcome";
            lblWelcome.Size = new Size(772, 22);
            lblWelcome.Text = MunicipalityBrand.WelcomeMessage;

            cardReportIssues = CreateServiceCard("Report Issues", '\uE7BA', true);
            cardLocalEvents = CreateServiceCard("Local Events", '\uE787', false);
            cardServiceStatus = CreateServiceCard("Service Status", '\uE9D5', false);

            cardTooltips.SetToolTip(cardReportIssues, "Report Issues");
            cardTooltips.SetToolTip(cardLocalEvents, "Local Events and Announcements — enabled in Part 2");
            cardTooltips.SetToolTip(cardServiceStatus, "Service Request Status — enabled in Part 2");

            cardReportIssues.Click += BtnReportIssues_Click;
            foreach (Control child in cardReportIssues.Controls)
            {
                child.Click += BtnReportIssues_Click;
            }

            lblRequestsHeading.AutoSize = true;
            lblRequestsHeading.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblRequestsHeading.ForeColor = UITheme.Text;
            lblRequestsHeading.Location = new Point(24, 264);
            lblRequestsHeading.Name = "lblRequestsHeading";
            lblRequestsHeading.Text = "Community reports";

            lblRequestsSubheading.AutoSize = true;
            lblRequestsSubheading.Font = new Font("Segoe UI", 8.5F);
            lblRequestsSubheading.ForeColor = UITheme.Muted;
            lblRequestsSubheading.Location = new Point(24, 289);
            lblRequestsSubheading.Name = "lblRequestsSubheading";
            lblRequestsSubheading.Text = MunicipalityBrand.CommunityReportsHint;

            txtSearch.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            txtSearch.Font = UITheme.BodyFont;
            txtSearch.Location = new Point(24, 314);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(280, 25);
            txtSearch.TabIndex = 1;
            txtSearch.TextChanged += TxtSearch_TextChanged;

            cmbStatusFilter.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            cmbStatusFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatusFilter.Font = UITheme.BodyFont;
            cmbStatusFilter.FormattingEnabled = true;
            cmbStatusFilter.Items.AddRange(new object[] { "All statuses", "Pending", "Resolved", "Closed" });
            cmbStatusFilter.Location = new Point(316, 314);
            cmbStatusFilter.Name = "cmbStatusFilter";
            cmbStatusFilter.Size = new Size(150, 25);
            cmbStatusFilter.TabIndex = 2;
            cmbStatusFilter.SelectedIndex = 0;
            cmbStatusFilter.SelectedIndexChanged += CmbStatusFilter_SelectedIndexChanged;

            lvRequests.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lvRequests.BorderStyle = BorderStyle.None;
            lvRequests.Columns.Add("Ref #", 70);
            lvRequests.Columns.Add("Category", 120);
            lvRequests.Columns.Add("Location", 200);
            lvRequests.Columns.Add("Date reported", 140);
            lvRequests.Columns.Add("Attachment", 150);
            lvRequests.Columns.Add("Status", 120);
            lvRequests.Columns.Add("", 56);
            lvRequests.Font = new Font("Segoe UI", 10F);
            lvRequests.FullRowSelect = true;
            lvRequests.GridLines = false;
            lvRequests.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvRequests.Location = new Point(24, 346);
            lvRequests.Name = "lvRequests";
            lvRequests.OwnerDraw = true;
            lvRequests.Size = new Size(772, 310);
            lvRequests.TabIndex = 3;
            lvRequests.UseCompatibleStateImageBehavior = false;
            lvRequests.View = View.Details;
            lvRequests.DrawColumnHeader += LvRequests_DrawColumnHeader;
            lvRequests.DrawItem += LvRequests_DrawItem;
            lvRequests.DrawSubItem += LvRequests_DrawSubItem;
            lvRequests.MouseClick += LvRequests_MouseClick;
            lvRequests.MouseMove += LvRequests_MouseMove;
            lvRequests.MouseDoubleClick += LvRequests_MouseDoubleClick;

            lblNoRequests.AutoSize = true;
            lblNoRequests.Font = new Font("Segoe UI", 9.5F, FontStyle.Italic);
            lblNoRequests.ForeColor = UITheme.Muted;
            lblNoRequests.Location = new Point(24, 354);
            lblNoRequests.Name = "lblNoRequests";
            lblNoRequests.Text = "No issues have been reported yet. Click \"Report Issues\" to get started.";

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = UITheme.Background;
            ClientSize = new Size(820, 720);
            Controls.Add(lblNoRequests);
            Controls.Add(lvRequests);
            Controls.Add(cmbStatusFilter);
            Controls.Add(txtSearch);
            Controls.Add(lblRequestsSubheading);
            Controls.Add(lblRequestsHeading);
            Controls.Add(cardServiceStatus);
            Controls.Add(cardLocalEvents);
            Controls.Add(cardReportIssues);
            Controls.Add(lblWelcome);
            Controls.Add(pnlHeader);
            Controls.Add(menuStrip);
            Font = UITheme.BodyFont;
            MainMenuStrip = menuStrip;
            MinimumSize = new Size(680, 580);
            Name = "MainMenuForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = MunicipalityBrand.AppShortName;
            Resize += MainMenuForm_Resize;
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
