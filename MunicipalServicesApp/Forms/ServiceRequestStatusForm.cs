using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using MunicipalServicesApp.DataStructures;
using MunicipalServicesApp.Models;
using MunicipalServicesApp.Services;

namespace MunicipalServicesApp.Forms
{
    public partial class ServiceRequestStatusForm : Form
    {
        private const string TrackPlaceholder = "3 or MSR-2026-0003";
        private const string FilterPlaceholder = "Filter by place, category, or reference";

        private readonly ServiceRequestCatalogue catalogue;
        private readonly ToolTip tips = new ToolTip();
        private readonly bool serviceDesk;
        private readonly DeskScope deskScope;
        private bool screenReady;
        private string selectedId;
        private GeneralTreeNode<CatalogueItem> selectedCategory;

        private static readonly Color HeaderText = Color.FromArgb(107, 114, 128);
        private static readonly Color RowText = Color.FromArgb(17, 24, 39);
        private static readonly Color Divider = Color.FromArgb(229, 231, 235);
        private static readonly Font HeaderFont = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        private static readonly Font RowFont = new Font("Segoe UI", 9.5F);
        private static readonly Font BadgeFont = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold);

        public ServiceRequestStatusForm(bool serviceDesk)
            : this(serviceDesk, null)
        {
        }

        public ServiceRequestStatusForm(bool serviceDesk, string deskCode)
        {
            this.serviceDesk = serviceDesk;
            deskScope = serviceDesk ? MunicipalDesk.ScopeFor(deskCode) : null;
            InitializeComponent();
            try
            {
                catalogue = ServiceRequestCatalogue.Instance;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    "The service request index could not be prepared." + Environment.NewLine + Environment.NewLine + ex.Message,
                    "Service Request Status",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Load += delegate { Close(); };
                return;
            }

            btnAdvance.Visible = false;
            btnAdvance.Dock = DockStyle.None;
            lblSubtitle.Text = serviceDesk
                ? (deskScope == null
                    ? "Service desk. Use Action to move a request to the next status."
                    : deskScope.Label + " desk. This list is filtered to " + deskScope.Label + ". Use Action to move a request.")
                : "Resident view. You can track a request. Only the service desk can change a status.";
            tips.SetToolTip(btnTrack, "Looks the reference up in the binary search tree and shows the path.");
            tips.SetToolTip(cmbOrder, "Each choice reads a different index. The filter only hides rows.");
            lvRequests.MouseClick += LvRequests_MouseClick;
            lvRequests.MouseMove += LvRequests_MouseMove;
            var rowImages = new ImageList();
            rowImages.ImageSize = new Size(1, 34);
            lvRequests.SmallImageList = rowImages;
            SetPlaceholder(txtTrack, TrackPlaceholder);
            if (deskScope == null)
            {
                SetPlaceholder(txtFilter, FilterPlaceholder);
            }
            else
            {
                LockDeskFilter();
            }
            txtTrack.KeyDown += TxtTrack_KeyDown;
            txtFilter.TextChanged += TxtFilter_TextChanged;
            BuildCatalogue();
            BindUrgent();
            BindNetwork();
            BindStats();
            screenReady = true;
            BindList();
            LayoutChrome();
        }

        private void ServiceRequestStatusForm_Resize(object sender, EventArgs e)
        {
            LayoutChrome();
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnTrack_Click(object sender, EventArgs e)
        {
            if (catalogue == null)
            {
                return;
            }

            string input = IsPlaceholder(txtTrack) ? string.Empty : txtTrack.Text;
            TrackResult result = catalogue.Track(input);
            if (result.Found && deskScope != null && result.Request != null &&
                !deskScope.Includes(result.Request.Area, result.Request.Report.Location))
            {
                lblTrackResult.Text = RequestReference.Display(result.Request.Report.RequestId) +
                                      " is not on the " + deskScope.Label + " desk.";
                return;
            }

            lblTrackResult.Text = result.Message;
            if (!result.Found)
            {
                return;
            }

            screenReady = false;
            selectedCategory = catalogue.Catalogue.Root;
            SelectTreeNode(tvCategories.Nodes, catalogue.Catalogue.Root);
            if (deskScope == null)
            {
                SetPlaceholder(txtFilter, FilterPlaceholder);
            }
            screenReady = true;
            selectedId = result.Request.Report.RequestId;
            BindList();
            SelectListedRequest(selectedId);
            BindDetail(selectedId);
        }

        private void TxtTrack_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                BtnTrack_Click(sender, EventArgs.Empty);
            }
        }

        private void TxtFilter_TextChanged(object sender, EventArgs e)
        {
            if (screenReady)
            {
                BindList();
            }
        }

        private void CmbOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (screenReady)
            {
                BindList();
            }
        }

        private void TvCategories_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (!screenReady || e.Node == null)
            {
                return;
            }

            selectedCategory = e.Node.Tag as GeneralTreeNode<CatalogueItem>;
            BindList();
        }

        private void LvRequests_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!screenReady || lvRequests.SelectedItems.Count == 0)
            {
                return;
            }

            var view = lvRequests.SelectedItems[0].Tag as ServiceRequestView;
            if (view == null)
            {
                return;
            }

            selectedId = view.Report.RequestId;
            BindDetail(selectedId);
        }

        private void BtnAdvance_Click(object sender, EventArgs e)
        {
            if (catalogue == null || string.IsNullOrEmpty(selectedId))
            {
                MessageBox.Show(this, "Select a request first.", "Service Request Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            RequestDetail detail = catalogue.Detail(selectedId);
            if (detail == null)
            {
                return;
            }

            string current = RequestStatuses.Canonical(detail.Request.Report.Status);
            string next = RequestStatuses.Next(current);
            if (next == null)
            {
                MessageBox.Show(this, RequestReference.Display(selectedId) + " is already " + current + ".", "Service Request Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!serviceDesk)
            {
                return;
            }

            string blocked = RequestStatuses.Rank(next) >= RequestStatuses.Rank(RequestStatuses.InProgress) && HasOpenDependency(detail)
                ? detail.BlockMessage
                : null;

            using (var update = new StatusUpdateForm(
                RequestReference.Display(selectedId),
                detail.Request.Report.Category ?? "Request",
                current,
                next,
                blocked))
            {
                if (update.ShowDialog(this) != DialogResult.OK || blocked != null)
                {
                    return;
                }
            }

            AdvanceResult result = catalogue.TryAdvance(selectedId);
            lblTrackResult.Text = result.Message;
            if (!result.Updated)
            {
                return;
            }

            BindStats();
            BindUrgent();
            BindList();
            SelectListedRequest(selectedId);
            BindDetail(selectedId);
        }

        private void BuildCatalogue()
        {
            tvCategories.BeginUpdate();
            tvCategories.Nodes.Clear();
            AddCatalogueNode(null, catalogue.Catalogue.Root);
            tvCategories.ExpandAll();
            tvCategories.EndUpdate();
            SelectTreeNode(tvCategories.Nodes, catalogue.Catalogue.Root);
            if (tvCategories.SelectedNode != null)
            {
                selectedCategory = tvCategories.SelectedNode.Tag as GeneralTreeNode<CatalogueItem>;
            }
        }

        private void AddCatalogueNode(TreeNode parent, GeneralTreeNode<CatalogueItem> source)
        {
            if (source == null || (source.Value != null && source.Value.IsRequest))
            {
                return;
            }

            int count = CountOnThisDesk(source);
            if (deskScope != null && count == 0)
            {
                return;
            }
            var node = new TreeNode(source.Value.Name + " (" + count.ToString() + ")");
            node.Tag = source;
            if (parent == null)
            {
                tvCategories.Nodes.Add(node);
            }
            else
            {
                parent.Nodes.Add(node);
            }

            for (int i = 0; i < source.Children.Count; i++)
            {
                AddCatalogueNode(node, source.Children[i]);
            }
        }

        private void BindList()
        {
            if (catalogue == null)
            {
                return;
            }

            string keep = selectedId;
            screenReady = false;
            lvRequests.Items.Clear();
            string filter = DeskFilterText();
            IList<ServiceRequestView> rows = catalogue.List(CurrentOrder(), selectedCategory, filter, deskScope);
            for (int i = 0; i < rows.Count; i++)
            {
                ServiceRequestView view = rows[i];
                var item = new ListViewItem(new[]
                {
                    RequestReference.Display(view.Report.RequestId),
                    view.Report.Category ?? string.Empty,
                    view.Area ?? string.Empty,
                    view.Report.ReportedAt.ToString("dd MMM yyyy"),
                    RequestStatuses.Canonical(view.Report.Status),
                    view.IsOpen ? view.Priority.ToString() : "—",
                    ActionLabel(view)
                });
                item.Tag = view;
                lvRequests.Items.Add(item);
            }

            lblNoRequests.Visible = rows.Count == 0;
            lvRequests.Visible = rows.Count > 0;
            screenReady = true;
            if (!string.IsNullOrEmpty(keep))
            {
                SelectListedRequest(keep);
            }

            ResizeRequestColumns();
        }

        private void BindDetail(string requestId)
        {
            RequestDetail detail = catalogue.Detail(requestId);
            if (detail == null)
            {
                lblDetailRef.Text = "Select a request";
                lblDetailMeta.Text = string.Empty;
                lblDetailLocation.Text = string.Empty;
                lblDetailDescription.Text = "Select a row, or track a reference, to see progress, dependencies, and the depot route.";
                lblHistory.Text = string.Empty;
                lblDecision.Text = string.Empty;
                lblGraph.Text = string.Empty;
                lblCrew.Text = string.Empty;
                flpProgress.Controls.Clear();
                btnAdvance.Enabled = false;
                btnAdvance.Text = "Record next update";
                ArrangeDetail();
                return;
            }

            ServiceRequestView view = detail.Request;
            string reference = RequestReference.Display(view.Report.RequestId);
            string status = RequestStatuses.Canonical(view.Report.Status);
            lblDetailRef.Text = reference + "  ·  " + status;
            lblDetailMeta.Text = (view.Report.Category ?? "Request") + "  ·  " + view.Area;
            lblDetailLocation.Text = view.Report.Location ?? string.Empty;
            lblDetailDescription.Text = string.IsNullOrWhiteSpace(view.Report.Description)
                ? "No description was provided."
                : view.Report.Description;
            BindProgress(status);
            lblHistory.Text = FormatHistory(view.History);
            lblDecision.Text = detail.DecisionTitle + Environment.NewLine + detail.DecisionDetail +
                               Environment.NewLine + "Path: " + string.Join(" ", detail.DecisionPath);
            lblGraph.Text = FormatGraph(detail);
            lblCrew.Text = detail.DepotName + " depot. Shortest path from " + DepotNetwork.Central + ": " +
                           string.Join(" → ", detail.TravelPath) + " (" + detail.TravelKm.ToString() + " km).";

            ArrangeDetail();
        }

        private void BindProgress(string status)
        {
            flpProgress.Controls.Clear();
            int current = RequestStatuses.Rank(status);
            for (int i = 0; i < RequestStatuses.Workflow.Length; i++)
            {
                var chip = new Label
                {
                    AutoSize = true,
                    Text = RequestStatuses.Workflow[i],
                    Font = BadgeFont,
                    Margin = new Padding(0, 2, 6, 2),
                    Padding = new Padding(8, 3, 8, 3)
                };

                if (i < current)
                {
                    chip.BackColor = Color.FromArgb(254, 243, 199);
                    chip.ForeColor = Color.FromArgb(146, 64, 14);
                }
                else if (i == current)
                {
                    chip.BackColor = UITheme.Navy;
                    chip.ForeColor = Color.White;
                }
                else
                {
                    chip.BackColor = Color.FromArgb(243, 244, 246);
                    chip.ForeColor = UITheme.Muted;
                }

                flpProgress.Controls.Add(chip);
            }
        }

        private void BindUrgent()
        {
            lvUrgent.Items.Clear();
            IList<ServiceRequestView> queue = catalogue.UrgentQueue(deskScope);
            for (int i = 0; i < queue.Count; i++)
            {
                ServiceRequestView view = queue[i];
                var item = new ListViewItem(new[]
                {
                    RequestReference.Display(view.Report.RequestId),
                    view.Report.Category ?? string.Empty,
                    view.Area ?? string.Empty,
                    view.Priority.ToString(),
                    view.PriorityReason ?? string.Empty
                });
                item.Tag = view;
                lvUrgent.Items.Add(item);
            }
        }

        private void BindNetwork()
        {
            lvNetwork.Items.Clear();
            IList<CrewLink> links = catalogue.CrewLinks();
            for (int i = 0; i < links.Count; i++)
            {
                lvNetwork.Items.Add(new ListViewItem(new[]
                {
                    links[i].From + " — " + links[i].To,
                    links[i].Kilometres.ToString()
                }));
            }
        }

        private void BindStats()
        {
            IndexOverview overview = catalogue.Overview();
            lblStats.Text =
                overview.Count.ToString() + " requests. AVL date index height " + overview.AvlHeight.ToString() +
                ". A plain search tree of the same dates, inserted in date order, reaches height " + overview.PlainDateTreeHeight.ToString() +
                ". Binary search tree height " + overview.BinarySearchHeight.ToString() +
                ". Red-black status index height " + overview.RedBlackHeight.ToString() +
                ". " + overview.ReportedFromSeptember.ToString() + " requests were reported on or after 1 September (AVL range query).";

            lblTraversal.Text =
                "Kruskal kept " + overview.MstLinkCount.ToString() + " links totalling " + overview.MstTotalKm.ToString() +
                " km, joining all " + overview.DepotCount.ToString() + " depots. " +
                "Breadth-first from " + DepotNetwork.Central + ": " + string.Join(", ", overview.DepotBreadthFirst) + ". " +
                "Depth-first: " + string.Join(", ", overview.DepotDepthFirst) + ".";
        }

        private RequestListOrder CurrentOrder()
        {
            switch (cmbOrder.SelectedIndex)
            {
                case 1:
                    return RequestListOrder.Reference;
                case 2:
                    return RequestListOrder.Status;
                case 3:
                    return RequestListOrder.Urgency;
                default:
                    return RequestListOrder.Date;
            }
        }

        private void SelectListedRequest(string requestId)
        {
            screenReady = false;
            lvRequests.SelectedItems.Clear();
            for (int i = 0; i < lvRequests.Items.Count; i++)
            {
                var view = lvRequests.Items[i].Tag as ServiceRequestView;
                if (view != null && string.Equals(view.Report.RequestId, requestId, StringComparison.OrdinalIgnoreCase))
                {
                    lvRequests.Items[i].Selected = true;
                    lvRequests.Items[i].EnsureVisible();
                    break;
                }
            }

            screenReady = true;
        }

        private static bool SelectTreeNode(TreeNodeCollection nodes, GeneralTreeNode<CatalogueItem> target)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (ReferenceEquals(nodes[i].Tag, target))
                {
                    nodes[i].TreeView.SelectedNode = nodes[i];
                    return true;
                }

                if (SelectTreeNode(nodes[i].Nodes, target))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasOpenDependency(RequestDetail detail)
        {
            for (int i = 0; i < detail.WaitingOn.Count; i++)
            {
                if (!RequestStatuses.IsFinished(detail.WaitingOn[i].Report.Status))
                {
                    return true;
                }
            }

            return false;
        }

        private static string FormatHistory(IList<StatusChange> history)
        {
            if (history == null || history.Count == 0)
            {
                return string.Empty;
            }

            var lines = new List<string>();
            for (int i = 0; i < history.Count; i++)
            {
                lines.Add(history[i].At.ToString("dd MMM yyyy HH:mm") + " — " + history[i].Status + ". " + history[i].Note);
            }

            return string.Join(Environment.NewLine, lines);
        }

        private static string FormatGraph(RequestDetail detail)
        {
            var lines = new List<string>
            {
                "Waiting on: " + FormatViews(detail.WaitingOn, "nothing"),
                "Holding up: " + FormatViews(detail.HoldingUp, "nothing"),
                "Breadth-first from this request: " + FormatViews(detail.Related, "just this request"),
                "Depth-first chain of jobs that wait on this one: " + FormatViews(detail.Downstream, "none"),
                detail.CycleMessage
            };

            if (!string.IsNullOrWhiteSpace(detail.DependencyNote))
            {
                lines.Insert(0, detail.DependencyNote);
            }

            if (!string.IsNullOrWhiteSpace(detail.BlockMessage))
            {
                lines.Add(detail.BlockMessage);
            }

            return string.Join(Environment.NewLine, lines);
        }

        private static string FormatViews(IList<ServiceRequestView> views, string empty)
        {
            if (views == null || views.Count == 0)
            {
                return empty;
            }

            var parts = new List<string>();
            for (int i = 0; i < views.Count; i++)
            {
                parts.Add(RequestReference.Display(views[i].Report.RequestId) + " " + views[i].Report.Category);
            }

            return string.Join(" → ", parts);
        }

        private void LayoutChrome()
        {
            if (lblStats == null || pnlHeader == null)
            {
                return;
            }

            int left = 16;
            int width = Math.Max(400, ClientSize.Width - 32);
            int y = pnlHeader.Bottom + 12;

            lblTrackCaption.Location = new Point(left, y + 4);
            txtTrack.Location = new Point(left + 150, y);
            txtTrack.Size = new Size(220, 27);
            btnTrack.Location = new Point(txtTrack.Right + 8, y - 1);
            lblTrackResult.Location = new Point(btnTrack.Right + 12, y + 2);
            lblTrackResult.Size = new Size(Math.Max(180, width - (btnTrack.Right + 12 - left)), 32);

            y = txtTrack.Bottom + 8;
            lblStats.Location = new Point(left, y);
            lblStats.Size = new Size(width, 48);

            int bodyTop = lblStats.Bottom + 10;
            int bottomHeight = 176;
            int bodyHeight = Math.Max(240, ClientSize.Height - bodyTop - bottomHeight - 16);
            int treeWidth = 230;
            int detailWidth = 360;
            int listLeft = left + treeWidth + 10;
            int listWidth = Math.Max(280, width - treeWidth - detailWidth - 20);
            int detailLeft = listLeft + listWidth + 10;

            lblCatalogue.Location = new Point(left, bodyTop);
            tvCategories.Location = new Point(left, lblCatalogue.Bottom + 4);
            tvCategories.Size = new Size(treeWidth, bodyHeight - 24);

            lblList.Location = new Point(listLeft, bodyTop);
            cmbOrder.Location = new Point(detailLeft - 250, bodyTop - 2);
            cmbOrder.Size = new Size(240, 27);
            cmbOrder.Left = listLeft + listWidth - cmbOrder.Width;
            txtFilter.Location = new Point(listLeft, lblList.Bottom + 4);
            txtFilter.Size = new Size(Math.Max(120, listWidth - cmbOrder.Width - 8), 27);
            cmbOrder.Top = txtFilter.Top;
            lvRequests.Location = new Point(listLeft, txtFilter.Bottom + 6);
            lvRequests.Size = new Size(listWidth, bodyHeight - (lvRequests.Top - bodyTop));
            lblNoRequests.Location = new Point(listLeft + 8, lvRequests.Top + 12);
            lblNoRequests.Size = new Size(listWidth - 16, 40);
            ResizeRequestColumns();

            pnlDetail.Location = new Point(detailLeft, bodyTop);
            pnlDetail.Size = new Size(width - (detailLeft - left), bodyHeight);
            ArrangeDetail();

            int bottomTop = bodyTop + bodyHeight + 10;
            int half = (width - 10) / 2;
            lblUrgent.Location = new Point(left, bottomTop);
            lvUrgent.Location = new Point(left, lblUrgent.Bottom + 4);
            lvUrgent.Size = new Size(half, Math.Max(80, ClientSize.Height - lvUrgent.Top - 28));
            ResizeColumns(lvUrgent, new[] { 54, 110, 120, 70 });

            lblNetwork.Location = new Point(left + half + 10, bottomTop);
            lvNetwork.Location = new Point(lblNetwork.Left, lblNetwork.Bottom + 4);
            lvNetwork.Size = new Size(half, Math.Max(48, lvUrgent.Height - 48));
            lblTraversal.Location = new Point(lvNetwork.Left, lvNetwork.Bottom + 2);
            lblTraversal.Size = new Size(half, 44);
            ResizeColumns(lvNetwork, new[] { Math.Max(160, half - 70) });
        }

        private void ArrangeDetail()
        {
            if (pnlDetailBody == null)
            {
                return;
            }

            int inner = Math.Max(180, pnlDetailBody.ClientSize.Width - 28);
            SetWrap(lblDetailRef, inner);
            SetWrap(lblDetailMeta, inner);
            SetWrap(lblDetailLocation, inner);
            SetWrap(lblDetailDescription, inner);
            SetWrap(lblHistory, inner);
            SetWrap(lblDecision, inner);
            SetWrap(lblGraph, inner);
            SetWrap(lblCrew, inner);
            flpProgress.MaximumSize = new Size(inner, 0);

            int y = 8;
            y = Stack(lblDetailRef, 8, y);
            y = Stack(lblDetailMeta, 8, y);
            y = Stack(lblDetailLocation, 8, y);
            y = Stack(lblDetailDescription, 8, y);
            y = Stack(lblProgressHeading, 8, y + 8);
            flpProgress.Location = new Point(8, y);
            flpProgress.Width = inner;
            y = flpProgress.Bottom + 6;
            y = Stack(lblHistory, 8, y);
            y = Stack(lblDecisionHeading, 8, y + 8);
            y = Stack(lblDecision, 8, y);
            y = Stack(lblGraphHeading, 8, y + 8);
            y = Stack(lblGraph, 8, y);
            y = Stack(lblCrewHeading, 8, y + 8);
            Stack(lblCrew, 8, y);
        }

        private static int Stack(Label label, int x, int y)
        {
            label.Location = new Point(x, y);
            return label.Bottom + 4;
        }

        private static void SetWrap(Label label, int width)
        {
            label.MaximumSize = new Size(width, 0);
        }

        private void ResizeRequestColumns()
        {
            if (lvRequests.Columns.Count < 7 || lvRequests.ClientSize.Width < 100)
            {
                return;
            }

            int usable = lvRequests.ClientSize.Width;
            int reference = 54;
            int reported = 108;
            int status = 110;
            int urgency = 64;
            int action = serviceDesk ? 88 : 56;
            int category = Math.Max(90, usable * 18 / 100);
            int area = Math.Max(90, usable - reference - reported - status - urgency - action - category - 8);
            lvRequests.Columns[0].Width = reference;
            lvRequests.Columns[1].Width = category;
            lvRequests.Columns[2].Width = area;
            lvRequests.Columns[3].Width = reported;
            lvRequests.Columns[4].Width = status;
            lvRequests.Columns[5].Width = urgency;
            lvRequests.Columns[6].Width = action;
        }

        private static void ResizeColumns(ListView list, int[] fixedWidths)
        {
            if (list.Columns.Count == 0 || list.ClientSize.Width < 80)
            {
                return;
            }

            int used = 0;
            int flexIndex = list.Columns.Count - 1;
            for (int i = 0; i < list.Columns.Count - 1 && i < fixedWidths.Length; i++)
            {
                list.Columns[i].Width = fixedWidths[i];
                used += fixedWidths[i];
            }

            list.Columns[flexIndex].Width = Math.Max(80, list.ClientSize.Width - used - 4);
        }

        private void LvRequests_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = false;
            using (var brush = new SolidBrush(Color.White))
            {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }

            TextRenderer.DrawText(
                e.Graphics,
                e.Header.Text,
                HeaderFont,
                new Rectangle(e.Bounds.X + 6, e.Bounds.Y, e.Bounds.Width - 8, e.Bounds.Height),
                HeaderText,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);

            using (var pen = new Pen(Divider))
            {
                e.Graphics.DrawLine(pen, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
            }
        }

        private void LvRequests_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = false;
        }

        private void LvRequests_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = false;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Color back = e.ItemIndex % 2 == 0 ? Color.White : Color.FromArgb(252, 252, 253);
            if (e.Item.Selected)
            {
                back = Color.FromArgb(239, 246, 255);
            }

            using (var brush = new SolidBrush(back))
            {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }

            using (var pen = new Pen(Divider))
            {
                e.Graphics.DrawLine(pen, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
            }

            if (e.ColumnIndex == 4)
            {
                StatusBadge.Draw(e.Graphics, e.Bounds, e.SubItem.Text, BadgeFont);
                return;
            }

            if (e.ColumnIndex == 6)
            {
                DrawAction(e.Graphics, e.Bounds, e.SubItem.Text);
                return;
            }

            TextRenderer.DrawText(
                e.Graphics,
                e.SubItem.Text,
                RowFont,
                new Rectangle(e.Bounds.X + 6, e.Bounds.Y, e.Bounds.Width - 8, e.Bounds.Height),
                RowText,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
        }

        private string ActionLabel(ServiceRequestView view)
        {
            if (!serviceDesk || view == null || view.Report == null)
            {
                return "—";
            }

            return RequestStatuses.Next(RequestStatuses.Canonical(view.Report.Status)) == null ? "—" : "Update";
        }

        private void LvRequests_MouseMove(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo hit = lvRequests.HitTest(e.Location);
            bool canUpdate = serviceDesk && hit.SubItem != null && hit.SubItem.Text == "Update" && ColumnIndex(hit) == 6;
            lvRequests.Cursor = canUpdate ? Cursors.Hand : Cursors.Default;
        }

        private void LvRequests_MouseClick(object sender, MouseEventArgs e)
        {
            if (!serviceDesk)
            {
                return;
            }

            ListViewHitTestInfo hit = lvRequests.HitTest(e.Location);
            if (hit.Item == null || ColumnIndex(hit) != 6 || hit.SubItem == null || hit.SubItem.Text != "Update")
            {
                return;
            }

            var view = hit.Item.Tag as ServiceRequestView;
            if (view == null)
            {
                return;
            }

            selectedId = view.Report.RequestId;
            BtnAdvance_Click(sender, EventArgs.Empty);
        }

        private static int ColumnIndex(ListViewHitTestInfo hit)
        {
            if (hit.Item == null || hit.SubItem == null)
            {
                return -1;
            }

            return hit.Item.SubItems.IndexOf(hit.SubItem);
        }

        private static void DrawAction(Graphics graphics, Rectangle bounds, string label)
        {
            if (label != "Update")
            {
                TextRenderer.DrawText(
                    graphics,
                    "—",
                    RowFont,
                    bounds,
                    HeaderText,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                return;
            }

            var button = new Rectangle(bounds.X + 6, bounds.Y + 6, Math.Min(72, bounds.Width - 12), bounds.Height - 12);
            using (GraphicsPath path = RoundedAction(button, 8))
            using (var brush = new SolidBrush(UITheme.Navy))
            {
                graphics.FillPath(brush, path);
            }

            TextRenderer.DrawText(
                graphics,
                "Update",
                BadgeFont,
                button,
                Color.White,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        private static GraphicsPath RoundedAction(Rectangle bounds, int radius)
        {
            int diameter = Math.Max(2, radius * 2);
            var path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void LockDeskFilter()
        {
            txtFilter.TextChanged -= TxtFilter_TextChanged;
            txtFilter.ReadOnly = true;
            txtFilter.Text = deskScope.Label;
            txtFilter.ForeColor = UITheme.Text;
            txtFilter.Tag = null;
            txtFilter.TextChanged += TxtFilter_TextChanged;
            tips.SetToolTip(txtFilter, "This desk code only shows " + deskScope.Label + ".");
        }

        private string DeskFilterText()
        {
            if (deskScope != null)
            {
                return string.Empty;
            }

            return IsPlaceholder(txtFilter) ? string.Empty : txtFilter.Text;
        }

        private int CountOnThisDesk(GeneralTreeNode<CatalogueItem> source)
        {
            IList<string> ids = catalogue.Catalogue.RequestIds(source);
            if (deskScope == null)
            {
                return ids.Count;
            }

            int count = 0;
            for (int i = 0; i < ids.Count; i++)
            {
                if (catalogue.IsOnDesk(ids[i], deskScope))
                {
                    count++;
                }
            }

            return count;
        }

        private void SetPlaceholder(TextBox box, string placeholder)
        {
            box.TextChanged -= TxtFilter_TextChanged;
            box.Text = placeholder;
            box.ForeColor = Color.Gray;
            box.Tag = placeholder;
            box.TextChanged += TxtFilter_TextChanged;

            box.Enter -= Box_Enter;
            box.Leave -= Box_Leave;
            box.Enter += Box_Enter;
            box.Leave += Box_Leave;
        }

        private void Box_Enter(object sender, EventArgs e)
        {
            var box = (TextBox)sender;
            if (!IsPlaceholder(box))
            {
                return;
            }

            box.TextChanged -= TxtFilter_TextChanged;
            box.Text = string.Empty;
            box.ForeColor = Color.Black;
            box.TextChanged += TxtFilter_TextChanged;
        }

        private void Box_Leave(object sender, EventArgs e)
        {
            var box = (TextBox)sender;
            if (!string.IsNullOrWhiteSpace(box.Text))
            {
                return;
            }

            string placeholder = box.Tag as string;
            if (string.IsNullOrEmpty(placeholder))
            {
                return;
            }

            box.TextChanged -= TxtFilter_TextChanged;
            box.Text = placeholder;
            box.ForeColor = Color.Gray;
            box.TextChanged += TxtFilter_TextChanged;
        }

        private static bool IsPlaceholder(TextBox box)
        {
            return box.Tag is string placeholder &&
                   box.Text == placeholder &&
                   box.ForeColor == Color.Gray;
        }
    }
}
