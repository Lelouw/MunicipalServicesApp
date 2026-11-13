using System.Drawing;
using System.Windows.Forms;

namespace MunicipalServicesApp.Forms
{
    partial class ServiceRequestStatusForm
    {
        private System.ComponentModel.IContainer components = null;
        private Panel pnlHeader;
        private Label lblTitle;
        private Label lblSubtitle;
        private Button btnBack;
        private Label lblTrackCaption;
        private TextBox txtTrack;
        private Button btnTrack;
        private Label lblTrackResult;
        private Label lblStats;
        private Label lblCatalogue;
        private TreeView tvCategories;
        private Label lblList;
        private ComboBox cmbOrder;
        private TextBox txtFilter;
        private ListView lvRequests;
        private Label lblNoRequests;
        private Panel pnlDetail;
        private Panel pnlDetailBody;
        private Label lblDetailRef;
        private Label lblDetailMeta;
        private Label lblDetailLocation;
        private Label lblDetailDescription;
        private Label lblProgressHeading;
        private FlowLayoutPanel flpProgress;
        private Label lblHistory;
        private Label lblDecisionHeading;
        private Label lblDecision;
        private Label lblGraphHeading;
        private Label lblGraph;
        private Label lblCrewHeading;
        private Label lblCrew;
        private Button btnAdvance;
        private Label lblUrgent;
        private ListView lvUrgent;
        private Label lblNetwork;
        private ListView lvNetwork;
        private Label lblTraversal;

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
            lblTrackCaption = new Label();
            txtTrack = new TextBox();
            btnTrack = new Button();
            lblTrackResult = new Label();
            lblStats = new Label();
            lblCatalogue = new Label();
            tvCategories = new TreeView();
            lblList = new Label();
            cmbOrder = new ComboBox();
            txtFilter = new TextBox();
            lvRequests = new ListView();
            lblNoRequests = new Label();
            pnlDetail = new Panel();
            pnlDetailBody = new Panel();
            lblDetailRef = new Label();
            lblDetailMeta = new Label();
            lblDetailLocation = new Label();
            lblDetailDescription = new Label();
            lblProgressHeading = new Label();
            flpProgress = new FlowLayoutPanel();
            lblHistory = new Label();
            lblDecisionHeading = new Label();
            lblDecision = new Label();
            lblGraphHeading = new Label();
            lblGraph = new Label();
            lblCrewHeading = new Label();
            lblCrew = new Label();
            btnAdvance = new Button();
            lblUrgent = new Label();
            lvUrgent = new ListView();
            lblNetwork = new Label();
            lvNetwork = new ListView();
            lblTraversal = new Label();
            pnlHeader.SuspendLayout();
            pnlDetail.SuspendLayout();
            pnlDetailBody.SuspendLayout();
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
            lblTitle.Location = new Point(20, 14);
            lblTitle.Text = "Service Request Status";

            lblSubtitle.Font = UITheme.SubHeaderFont;
            lblSubtitle.ForeColor = UITheme.Gold;
            lblSubtitle.Location = new Point(20, 48);
            lblSubtitle.Size = new Size(860, 28);
            lblSubtitle.Text = MunicipalityBrand.StatusSubtitle;

            btnBack.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBack.BackColor = UITheme.Gold;
            btnBack.Cursor = Cursors.Hand;
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.FlatStyle = FlatStyle.Flat;
            btnBack.Font = UITheme.ButtonFont;
            btnBack.ForeColor = UITheme.Navy;
            btnBack.Location = new Point(1060, 26);
            btnBack.Size = new Size(110, 36);
            btnBack.Text = "Back";
            btnBack.UseVisualStyleBackColor = false;
            btnBack.Click += BtnBack_Click;

            lblTrackCaption.AutoSize = true;
            lblTrackCaption.Font = UITheme.LabelFont;
            lblTrackCaption.ForeColor = UITheme.Text;
            lblTrackCaption.Text = "Track by reference";

            txtTrack.Font = UITheme.BodyFont;
            txtTrack.Name = "txtTrack";

            btnTrack.BackColor = UITheme.Navy;
            btnTrack.Cursor = Cursors.Hand;
            btnTrack.FlatAppearance.BorderSize = 0;
            btnTrack.FlatStyle = FlatStyle.Flat;
            btnTrack.Font = UITheme.ButtonFont;
            btnTrack.ForeColor = Color.White;
            btnTrack.Size = new Size(110, 28);
            btnTrack.Text = "Track";
            btnTrack.UseVisualStyleBackColor = false;
            btnTrack.Click += BtnTrack_Click;

            lblTrackResult.Font = UITheme.BodyFont;
            lblTrackResult.ForeColor = UITheme.Navy;
            lblTrackResult.Text = "Type a reference, for example 3 or MSR-2026-0003.";

            lblStats.Font = new Font("Segoe UI", 8.5F);
            lblStats.ForeColor = UITheme.Muted;
            lblStats.Text = string.Empty;

            lblCatalogue.AutoSize = true;
            lblCatalogue.Font = UITheme.LabelFont;
            lblCatalogue.ForeColor = UITheme.Text;
            lblCatalogue.Text = "Service catalogue — general tree";

            tvCategories.BorderStyle = BorderStyle.None;
            tvCategories.Font = new Font("Segoe UI", 9.5F);
            tvCategories.HideSelection = false;
            tvCategories.Name = "tvCategories";
            tvCategories.AfterSelect += TvCategories_AfterSelect;

            lblList.AutoSize = true;
            lblList.Font = UITheme.LabelFont;
            lblList.ForeColor = UITheme.Text;
            lblList.Text = "Submitted requests";

            cmbOrder.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbOrder.Font = UITheme.BodyFont;
            cmbOrder.Items.AddRange(new object[]
            {
                "Date reported (AVL tree)",
                "Reference (binary search tree)",
                "Status (red-black tree)",
                "Urgency (binary heap)"
            });
            cmbOrder.SelectedIndex = 0;
            cmbOrder.SelectedIndexChanged += CmbOrder_SelectedIndexChanged;

            txtFilter.Font = UITheme.BodyFont;
            txtFilter.Name = "txtFilter";

            lvRequests.BorderStyle = BorderStyle.None;
            lvRequests.FullRowSelect = true;
            lvRequests.GridLines = false;
            lvRequests.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvRequests.HideSelection = false;
            lvRequests.MultiSelect = false;
            lvRequests.OwnerDraw = true;
            lvRequests.View = View.Details;
            lvRequests.Font = new Font("Segoe UI", 9.5F);
            lvRequests.Columns.Add("Ref", 54);
            lvRequests.Columns.Add("Category", 120);
            lvRequests.Columns.Add("Area", 120);
            lvRequests.Columns.Add("Reported", 120);
            lvRequests.Columns.Add("Status", 120);
            lvRequests.Columns.Add("Urgency", 70);
            lvRequests.Columns.Add("Action", 88);
            lvRequests.DrawColumnHeader += LvRequests_DrawColumnHeader;
            lvRequests.DrawItem += LvRequests_DrawItem;
            lvRequests.DrawSubItem += LvRequests_DrawSubItem;
            lvRequests.SelectedIndexChanged += LvRequests_SelectedIndexChanged;

            lblNoRequests.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            lblNoRequests.ForeColor = UITheme.Muted;
            lblNoRequests.Text = "No requests match this catalogue branch or filter.";
            lblNoRequests.Visible = false;

            pnlDetail.BackColor = Color.White;
            pnlDetail.BorderStyle = BorderStyle.FixedSingle;
            pnlDetail.Controls.Add(pnlDetailBody);
            pnlDetail.Controls.Add(btnAdvance);

            pnlDetailBody.AutoScroll = true;
            pnlDetailBody.Dock = DockStyle.Fill;
            pnlDetailBody.Padding = new Padding(12);
            pnlDetailBody.Controls.Add(lblDetailRef);
            pnlDetailBody.Controls.Add(lblDetailMeta);
            pnlDetailBody.Controls.Add(lblDetailLocation);
            pnlDetailBody.Controls.Add(lblDetailDescription);
            pnlDetailBody.Controls.Add(lblProgressHeading);
            pnlDetailBody.Controls.Add(flpProgress);
            pnlDetailBody.Controls.Add(lblHistory);
            pnlDetailBody.Controls.Add(lblDecisionHeading);
            pnlDetailBody.Controls.Add(lblDecision);
            pnlDetailBody.Controls.Add(lblGraphHeading);
            pnlDetailBody.Controls.Add(lblGraph);
            pnlDetailBody.Controls.Add(lblCrewHeading);
            pnlDetailBody.Controls.Add(lblCrew);

            lblDetailRef.AutoSize = true;
            lblDetailRef.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblDetailRef.ForeColor = UITheme.Navy;
            lblDetailRef.Text = "Select a request";

            lblDetailMeta.AutoSize = true;
            lblDetailMeta.Font = UITheme.BodyFont;
            lblDetailMeta.ForeColor = UITheme.Text;
            lblDetailMeta.MaximumSize = new Size(320, 0);

            lblDetailLocation.AutoSize = true;
            lblDetailLocation.Font = UITheme.BodyFont;
            lblDetailLocation.ForeColor = UITheme.Muted;
            lblDetailLocation.MaximumSize = new Size(320, 0);

            lblDetailDescription.AutoSize = true;
            lblDetailDescription.Font = UITheme.BodyFont;
            lblDetailDescription.ForeColor = UITheme.Text;
            lblDetailDescription.MaximumSize = new Size(320, 0);
            lblDetailDescription.Text = "The list shows every submitted request and its current status. Track one by its reference number, or select a row.";

            lblProgressHeading.AutoSize = true;
            lblProgressHeading.Font = UITheme.LabelFont;
            lblProgressHeading.Text = "Progress";

            flpProgress.WrapContents = true;
            flpProgress.AutoSize = true;
            flpProgress.MaximumSize = new Size(320, 0);

            lblHistory.AutoSize = true;
            lblHistory.Font = new Font("Segoe UI", 8.5F);
            lblHistory.ForeColor = UITheme.Muted;
            lblHistory.MaximumSize = new Size(320, 0);

            lblDecisionHeading.AutoSize = true;
            lblDecisionHeading.Font = UITheme.LabelFont;
            lblDecisionHeading.Text = "Next step — binary decision tree";

            lblDecision.AutoSize = true;
            lblDecision.Font = UITheme.BodyFont;
            lblDecision.MaximumSize = new Size(320, 0);

            lblGraphHeading.AutoSize = true;
            lblGraphHeading.Font = UITheme.LabelFont;
            lblGraphHeading.Text = "Related jobs — graph";

            lblGraph.AutoSize = true;
            lblGraph.Font = UITheme.BodyFont;
            lblGraph.MaximumSize = new Size(320, 0);

            lblCrewHeading.AutoSize = true;
            lblCrewHeading.Font = UITheme.LabelFont;
            lblCrewHeading.Text = "Depot route — shortest path";

            lblCrew.AutoSize = true;
            lblCrew.Font = UITheme.BodyFont;
            lblCrew.MaximumSize = new Size(320, 0);

            btnAdvance.BackColor = UITheme.Navy;
            btnAdvance.Cursor = Cursors.Hand;
            btnAdvance.Dock = DockStyle.Bottom;
            btnAdvance.FlatAppearance.BorderSize = 0;
            btnAdvance.FlatStyle = FlatStyle.Flat;
            btnAdvance.Font = UITheme.ButtonFont;
            btnAdvance.ForeColor = Color.White;
            btnAdvance.Height = 40;
            btnAdvance.Text = "Record next update";
            btnAdvance.UseVisualStyleBackColor = false;
            btnAdvance.Click += BtnAdvance_Click;

            lblUrgent.AutoSize = true;
            lblUrgent.Font = UITheme.LabelFont;
            lblUrgent.Text = "Attend to these first — binary heap";

            ConfigurePlainList(lvUrgent);
            lvUrgent.Columns.Add("Ref", 54);
            lvUrgent.Columns.Add("Category", 110);
            lvUrgent.Columns.Add("Area", 120);
            lvUrgent.Columns.Add("Priority", 70);
            lvUrgent.Columns.Add("Why it is first", 220);

            lblNetwork.AutoSize = true;
            lblNetwork.Font = UITheme.LabelFont;
            lblNetwork.Text = "Crew network — minimum spanning tree";

            ConfigurePlainList(lvNetwork);
            lvNetwork.Columns.Add("Link kept by Kruskal", 280);
            lvNetwork.Columns.Add("Km", 50);

            lblTraversal.Font = new Font("Segoe UI", 8.5F);
            lblTraversal.ForeColor = UITheme.Muted;

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = UITheme.Background;
            CancelButton = btnBack;
            ClientSize = new Size(1200, 860);
            Controls.Add(lblTraversal);
            Controls.Add(lvNetwork);
            Controls.Add(lblNetwork);
            Controls.Add(lvUrgent);
            Controls.Add(lblUrgent);
            Controls.Add(pnlDetail);
            Controls.Add(lblNoRequests);
            Controls.Add(lvRequests);
            Controls.Add(txtFilter);
            Controls.Add(cmbOrder);
            Controls.Add(lblList);
            Controls.Add(tvCategories);
            Controls.Add(lblCatalogue);
            Controls.Add(lblStats);
            Controls.Add(lblTrackResult);
            Controls.Add(btnTrack);
            Controls.Add(txtTrack);
            Controls.Add(lblTrackCaption);
            Controls.Add(pnlHeader);
            Font = UITheme.BodyFont;
            MinimumSize = new Size(1100, 760);
            Name = "ServiceRequestStatusForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = MunicipalityBrand.WindowTitle("Service Request Status");
            Resize += ServiceRequestStatusForm_Resize;
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlDetailBody.ResumeLayout(false);
            pnlDetailBody.PerformLayout();
            pnlDetail.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        private static void ConfigurePlainList(ListView list)
        {
            list.BorderStyle = BorderStyle.None;
            list.Font = new Font("Segoe UI", 9F);
            list.FullRowSelect = true;
            list.GridLines = false;
            list.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            list.MultiSelect = false;
            list.View = View.Details;
            list.BackColor = Color.White;
        }
    }
}
