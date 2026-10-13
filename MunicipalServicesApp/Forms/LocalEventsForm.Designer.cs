using System.Drawing;
using System.Windows.Forms;

namespace MunicipalServicesApp.Forms
{
    partial class LocalEventsForm
    {
        private System.ComponentModel.IContainer components = null;
        private Panel pnlHeader;
        private Label lblTitle;
        private Label lblSubtitle;
        private Button btnBack;
        private Panel pnlFilters;
        private TextBox txtSearch;
        private ComboBox cmbCategory;
        private Label lblFrom;
        private DateTimePicker dtpFrom;
        private Label lblTo;
        private DateTimePicker dtpTo;
        private Button btnSearch;
        private Button btnClear;
        private Panel pnlPriority;
        private Label lblPriority;
        private FlowLayoutPanel flpPriority;
        private Panel pnlMain;
        private Label lblUpcoming;
        private FlowLayoutPanel flpEvents;
        private Label lblNoEvents;
        private Panel pnlSide;
        private Label lblRecommend;
        private Label lblPattern;
        private FlowLayoutPanel flpRecommend;
        private Label lblRecent;
        private FlowLayoutPanel flpRecent;
        private Label lblQueue;
        private FlowLayoutPanel flpQueue;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }

                if (tips != null)
                {
                    tips.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            pnlHeader = new Panel();
            lblTitle = new Label();
            lblSubtitle = new Label();
            btnBack = new Button();
            pnlFilters = new Panel();
            txtSearch = new TextBox();
            cmbCategory = new ComboBox();
            lblFrom = new Label();
            dtpFrom = new DateTimePicker();
            lblTo = new Label();
            dtpTo = new DateTimePicker();
            btnSearch = new Button();
            btnClear = new Button();
            pnlPriority = new Panel();
            lblPriority = new Label();
            flpPriority = new FlowLayoutPanel();
            pnlMain = new Panel();
            lblUpcoming = new Label();
            flpEvents = new FlowLayoutPanel();
            lblNoEvents = new Label();
            pnlSide = new Panel();
            lblRecommend = new Label();
            lblPattern = new Label();
            flpRecommend = new FlowLayoutPanel();
            lblRecent = new Label();
            flpRecent = new FlowLayoutPanel();
            lblQueue = new Label();
            flpQueue = new FlowLayoutPanel();
            pnlHeader.SuspendLayout();
            pnlFilters.SuspendLayout();
            pnlPriority.SuspendLayout();
            pnlMain.SuspendLayout();
            pnlSide.SuspendLayout();
            SuspendLayout();

            pnlHeader.BackColor = UITheme.Navy;
            pnlHeader.Controls.Add(btnBack);
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
            lblTitle.Text = "Local Events and Announcements";

            lblSubtitle.AutoSize = false;
            lblSubtitle.Font = UITheme.SubHeaderFont;
            lblSubtitle.ForeColor = UITheme.Gold;
            lblSubtitle.Location = new Point(24, 48);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(760, 28);
            lblSubtitle.Text = MunicipalityBrand.EventsSubtitle;

            btnBack.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBack.BackColor = UITheme.Gold;
            btnBack.Cursor = Cursors.Hand;
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.FlatStyle = FlatStyle.Flat;
            btnBack.Font = UITheme.ButtonFont;
            btnBack.ForeColor = UITheme.Navy;
            btnBack.Location = new Point(960, 26);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(110, 36);
            btnBack.Text = "Back";
            btnBack.UseVisualStyleBackColor = false;
            btnBack.Click += BtnBack_Click;

            pnlFilters.BackColor = Color.White;
            pnlFilters.Controls.Add(txtSearch);
            pnlFilters.Controls.Add(cmbCategory);
            pnlFilters.Controls.Add(lblFrom);
            pnlFilters.Controls.Add(dtpFrom);
            pnlFilters.Controls.Add(lblTo);
            pnlFilters.Controls.Add(dtpTo);
            pnlFilters.Controls.Add(btnSearch);
            pnlFilters.Controls.Add(btnClear);
            pnlFilters.Dock = DockStyle.Top;
            pnlFilters.Height = 64;
            pnlFilters.Name = "pnlFilters";
            pnlFilters.Paint += PnlFilters_Paint;

            txtSearch.Font = UITheme.BodyFont;
            txtSearch.Location = new Point(16, 16);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(280, 27);
            txtSearch.TabIndex = 0;
            txtSearch.TextChanged += TxtSearch_TextChanged;

            cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCategory.Font = UITheme.BodyFont;
            cmbCategory.Location = new Point(308, 16);
            cmbCategory.Name = "cmbCategory";
            cmbCategory.Size = new Size(180, 27);
            cmbCategory.TabIndex = 1;
            cmbCategory.SelectedIndexChanged += CmbCategory_SelectedIndexChanged;

            lblFrom.AutoSize = true;
            lblFrom.Font = UITheme.BodyFont;
            lblFrom.ForeColor = UITheme.Muted;
            lblFrom.Location = new Point(500, 20);
            lblFrom.Name = "lblFrom";
            lblFrom.Text = "From";

            dtpFrom.Font = UITheme.BodyFont;
            dtpFrom.Format = DateTimePickerFormat.Custom;
            dtpFrom.CustomFormat = "dd MMM yyyy";
            dtpFrom.Location = new Point(544, 16);
            dtpFrom.Name = "dtpFrom";
            dtpFrom.ShowCheckBox = true;
            dtpFrom.Size = new Size(130, 27);
            dtpFrom.TabIndex = 2;
            dtpFrom.ValueChanged += DtpFrom_ValueChanged;

            lblTo.AutoSize = true;
            lblTo.Font = UITheme.BodyFont;
            lblTo.ForeColor = UITheme.Muted;
            lblTo.Location = new Point(684, 20);
            lblTo.Name = "lblTo";
            lblTo.Text = "To";

            dtpTo.Font = UITheme.BodyFont;
            dtpTo.Format = DateTimePickerFormat.Custom;
            dtpTo.CustomFormat = "dd MMM yyyy";
            dtpTo.Location = new Point(710, 16);
            dtpTo.Name = "dtpTo";
            dtpTo.ShowCheckBox = true;
            dtpTo.Size = new Size(130, 27);
            dtpTo.TabIndex = 3;
            dtpTo.ValueChanged += DtpTo_ValueChanged;

            btnSearch.BackColor = UITheme.Navy;
            btnSearch.Cursor = Cursors.Hand;
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.FlatStyle = FlatStyle.Flat;
            btnSearch.Font = UITheme.ButtonFont;
            btnSearch.ForeColor = Color.White;
            btnSearch.Location = new Point(852, 14);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(100, 32);
            btnSearch.TabIndex = 4;
            btnSearch.Text = "Search";
            btnSearch.UseVisualStyleBackColor = false;
            btnSearch.Click += BtnSearch_Click;

            btnClear.BackColor = Color.White;
            btnClear.Cursor = Cursors.Hand;
            btnClear.FlatAppearance.BorderColor = UITheme.Navy;
            btnClear.FlatStyle = FlatStyle.Flat;
            btnClear.Font = UITheme.ButtonFont;
            btnClear.ForeColor = UITheme.Navy;
            btnClear.Location = new Point(960, 14);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(84, 32);
            btnClear.TabIndex = 5;
            btnClear.Text = "Clear";
            btnClear.UseVisualStyleBackColor = false;
            btnClear.Click += BtnClear_Click;

            pnlPriority.BackColor = UITheme.Background;
            pnlPriority.Controls.Add(flpPriority);
            pnlPriority.Controls.Add(lblPriority);
            pnlPriority.Dock = DockStyle.Top;
            pnlPriority.Height = 164;
            pnlPriority.Name = "pnlPriority";

            lblPriority.Dock = DockStyle.Top;
            lblPriority.Font = UITheme.LabelFont;
            lblPriority.ForeColor = UITheme.Text;
            lblPriority.Height = 32;
            lblPriority.Name = "lblPriority";
            lblPriority.Padding = new Padding(20, 8, 0, 0);
            lblPriority.Text = "Priority notices — most urgent first";

            flpPriority.AutoScroll = true;
            flpPriority.BackColor = UITheme.Background;
            flpPriority.Dock = DockStyle.Fill;
            flpPriority.FlowDirection = FlowDirection.LeftToRight;
            flpPriority.Name = "flpPriority";
            flpPriority.Padding = new Padding(16, 4, 8, 4);
            flpPriority.WrapContents = false;

            pnlMain.BackColor = UITheme.Background;
            pnlMain.Controls.Add(flpEvents);
            pnlMain.Controls.Add(lblNoEvents);
            pnlMain.Controls.Add(lblUpcoming);
            pnlMain.Dock = DockStyle.Fill;
            pnlMain.Name = "pnlMain";

            lblUpcoming.Dock = DockStyle.Top;
            lblUpcoming.Font = UITheme.LabelFont;
            lblUpcoming.ForeColor = UITheme.Text;
            lblUpcoming.Height = 36;
            lblUpcoming.Name = "lblUpcoming";
            lblUpcoming.Padding = new Padding(20, 8, 0, 0);
            lblUpcoming.Text = "Upcoming events";

            lblNoEvents.Dock = DockStyle.Fill;
            lblNoEvents.Font = new Font("Segoe UI", 10F, FontStyle.Italic);
            lblNoEvents.ForeColor = UITheme.Muted;
            lblNoEvents.Name = "lblNoEvents";
            lblNoEvents.Padding = new Padding(24, 12, 24, 12);
            lblNoEvents.Text = "No events match this search.";
            lblNoEvents.Visible = false;

            flpEvents.AutoScroll = true;
            flpEvents.BackColor = UITheme.Background;
            flpEvents.Dock = DockStyle.Fill;
            flpEvents.Name = "flpEvents";
            flpEvents.Padding = new Padding(16, 4, 8, 16);
            flpEvents.WrapContents = true;

            pnlSide.AutoScroll = true;
            pnlSide.BackColor = Color.White;
            pnlSide.Controls.Add(flpQueue);
            pnlSide.Controls.Add(lblQueue);
            pnlSide.Controls.Add(flpRecent);
            pnlSide.Controls.Add(lblRecent);
            pnlSide.Controls.Add(flpRecommend);
            pnlSide.Controls.Add(lblPattern);
            pnlSide.Controls.Add(lblRecommend);
            pnlSide.Dock = DockStyle.Right;
            pnlSide.Name = "pnlSide";
            pnlSide.Width = 312;
            pnlSide.Paint += PnlSide_Paint;

            lblRecommend.Font = UITheme.LabelFont;
            lblRecommend.ForeColor = UITheme.Text;
            lblRecommend.Location = new Point(16, 12);
            lblRecommend.Name = "lblRecommend";
            lblRecommend.Size = new Size(280, 22);
            lblRecommend.Text = "Recommended for you";

            lblPattern.Font = new Font("Segoe UI", 8.5F);
            lblPattern.ForeColor = UITheme.Muted;
            lblPattern.Location = new Point(16, 36);
            lblPattern.Name = "lblPattern";
            lblPattern.Size = new Size(280, 48);
            lblPattern.Text = "Search to see recommendations.";

            flpRecommend.BackColor = Color.White;
            flpRecommend.FlowDirection = FlowDirection.TopDown;
            flpRecommend.Location = new Point(16, 88);
            flpRecommend.Name = "flpRecommend";
            flpRecommend.Size = new Size(280, 40);
            flpRecommend.WrapContents = false;

            lblRecent.Font = UITheme.LabelFont;
            lblRecent.ForeColor = UITheme.Text;
            lblRecent.Location = new Point(16, 140);
            lblRecent.Name = "lblRecent";
            lblRecent.Size = new Size(280, 22);
            lblRecent.Text = "Recently viewed";

            flpRecent.BackColor = Color.White;
            flpRecent.FlowDirection = FlowDirection.TopDown;
            flpRecent.Location = new Point(16, 166);
            flpRecent.Name = "flpRecent";
            flpRecent.Size = new Size(280, 36);
            flpRecent.WrapContents = false;

            lblQueue.Font = UITheme.LabelFont;
            lblQueue.ForeColor = UITheme.Text;
            lblQueue.Location = new Point(16, 214);
            lblQueue.Name = "lblQueue";
            lblQueue.Size = new Size(280, 36);
            lblQueue.Text = "Issued order — first published, first listed";

            flpQueue.BackColor = Color.White;
            flpQueue.FlowDirection = FlowDirection.TopDown;
            flpQueue.Location = new Point(16, 254);
            flpQueue.Name = "flpQueue";
            flpQueue.Size = new Size(280, 80);
            flpQueue.WrapContents = false;

            AcceptButton = btnSearch;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = UITheme.Background;
            CancelButton = btnBack;
            ClientSize = new Size(1100, 760);
            Controls.Add(pnlMain);
            Controls.Add(pnlSide);
            Controls.Add(pnlPriority);
            Controls.Add(pnlFilters);
            Controls.Add(pnlHeader);
            Font = UITheme.BodyFont;
            MinimumSize = new Size(1040, 680);
            Name = "LocalEventsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = MunicipalityBrand.WindowTitle("Local Events");
            Resize += LocalEventsForm_Resize;
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlFilters.ResumeLayout(false);
            pnlFilters.PerformLayout();
            pnlPriority.ResumeLayout(false);
            pnlMain.ResumeLayout(false);
            pnlSide.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
