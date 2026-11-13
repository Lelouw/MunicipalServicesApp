using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using MunicipalServicesApp.Data;
using MunicipalServicesApp.Models;
using MunicipalServicesApp.Services;

namespace MunicipalServicesApp.Forms
{
    public partial class MainMenuForm : Form
    {
        private const string SearchPlaceholder = "Search by location or category...";

        public MainMenuForm()
        {
            InitializeComponent();
            ConfigureRequestsTable();
            SetPlaceholder(txtSearch, SearchPlaceholder);
            LayoutCards();
            RefreshRequestsList();
            cardTooltips.SetToolTip(btnNotifications, "Updates for reports you submitted or are watching.");
            NotificationCentre.Instance.Changed += NotificationCentre_Changed;
            UpdateNotificationBell();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            NotificationCentre.Instance.Changed -= NotificationCentre_Changed;
            base.OnFormClosed(e);
        }

        private void ConfigureRequestsTable()
        {
            var rowImages = new ImageList();
            rowImages.ImageSize = new Size(1, 36);
            lvRequests.SmallImageList = rowImages;
            lvRequests.BackColor = Color.White;
        }

        private void MainMenuForm_Resize(object sender, EventArgs e)
        {
            LayoutCards();
        }

        private void BtnReportIssues_Click(object sender, EventArgs e)
        {
            using (var reportForm = new ReportIssuesForm())
            {
                reportForm.ShowDialog(this);
            }

            // Always return to the landing page list with filters cleared
            // so the newly submitted report is visible.
            ResetListFilters();
            RefreshRequestsList();
            Activate();
        }

        private void ResetListFilters()
        {
            if (cmbStatusFilter != null && cmbStatusFilter.Items.Count > 0)
            {
                cmbStatusFilter.SelectedIndex = 0;
            }

            if (txtSearch != null)
            {
                txtSearch.TextChanged -= TxtSearch_TextChanged;
                txtSearch.Text = SearchPlaceholder;
                txtSearch.ForeColor = Color.Gray;
                txtSearch.Tag = SearchPlaceholder;
                txtSearch.TextChanged += TxtSearch_TextChanged;
            }
        }

        private void BtnLocalEvents_Click(object sender, EventArgs e)
        {
            using (var eventsForm = new LocalEventsForm())
            {
                eventsForm.ShowDialog(this);
            }

            Activate();
        }

        private void BtnServiceStatus_Click(object sender, EventArgs e)
        {
            using (var access = new ServiceAccessForm())
            {
                if (access.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                using (var statusForm = new ServiceRequestStatusForm(access.AsServiceDesk))
                {
                    statusForm.ShowDialog(this);
                }
            }

            RefreshRequestsList();
            Activate();
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MenuAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                this,
                MunicipalityBrand.Name + Environment.NewLine +
                MunicipalityBrand.Slogan + Environment.NewLine + Environment.NewLine +
                "Municipal Services Application" + Environment.NewLine +
                "Report Issues, Local Events and Announcements, and Service Request Status." + Environment.NewLine + Environment.NewLine +
                "Developed for the AAPD7112W PoE assignment.",
                "About " + MunicipalityBrand.Name,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            RefreshRequestsList();
        }

        private void CmbStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshRequestsList();
        }

        private Panel CreateServiceCard(string label, char iconGlyph, bool enabled)
        {
            Color backColor = enabled ? UITheme.Navy : Color.FromArgb(210, 215, 222);
            Color foreColor = enabled ? Color.White : Color.FromArgb(90, 98, 110);
            Color borderColor = enabled ? UITheme.Navy : Color.FromArgb(160, 168, 180);

            var card = new Panel
            {
                Size = new Size(230, 84),
                BackColor = backColor,
                Cursor = enabled ? Cursors.Hand : Cursors.Default,
                Tag = enabled
            };

            card.Paint += (s, e) =>
            {
                using (var pen = new Pen(borderColor))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
                }
            };

            var lblIcon = new Label
            {
                Text = iconGlyph.ToString(),
                Font = new Font("Segoe MDL2 Assets", 18F),
                ForeColor = foreColor,
                AutoSize = true,
                Location = new Point(16, 12),
                BackColor = Color.Transparent
            };

            var lblText = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = foreColor,
                AutoSize = true,
                Location = new Point(16, 52),
                BackColor = Color.Transparent
            };

            card.Controls.Add(lblIcon);
            card.Controls.Add(lblText);
            return card;
        }

        private void LayoutCards()
        {
            if (cardReportIssues == null || cardLocalEvents == null || cardServiceStatus == null)
            {
                return;
            }

            const int top = 160;
            const int left = 24;
            const int gap = 12;
            const int cardHeight = 84;
            int available = Math.Max(200, ClientSize.Width - 48);

            if (available >= (220 * 3 + gap * 2))
            {
                int cardWidth = (available - (gap * 2)) / 3;
                cardReportIssues.Size = new Size(cardWidth, cardHeight);
                cardLocalEvents.Size = new Size(cardWidth, cardHeight);
                cardServiceStatus.Size = new Size(cardWidth, cardHeight);
                cardReportIssues.Location = new Point(left, top);
                cardLocalEvents.Location = new Point(left + cardWidth + gap, top);
                cardServiceStatus.Location = new Point(left + (cardWidth + gap) * 2, top);
            }
            else
            {
                cardReportIssues.Size = new Size(available, cardHeight);
                cardLocalEvents.Size = new Size(available, cardHeight);
                cardServiceStatus.Size = new Size(available, cardHeight);
                cardReportIssues.Location = new Point(left, top);
                cardLocalEvents.Location = new Point(left, top + cardHeight + 10);
                cardServiceStatus.Location = new Point(left, top + (cardHeight + 10) * 2);
            }

            if (btnNotifications != null && pnlHeader != null)
            {
                btnNotifications.Location = new Point(pnlHeader.ClientSize.Width - 68, 28);
                if (lblNotificationBadge != null)
                {
                    lblNotificationBadge.Location = new Point(btnNotifications.Left + 24, btnNotifications.Top - 2);
                    lblNotificationBadge.BringToFront();
                }
            }

            int listTop = Math.Max(264, cardServiceStatus.Bottom + 24);
            int contentWidth = available;

            lblRequestsHeading.Location = new Point(left, listTop);
            lblRequestsSubheading.Location = new Point(left, listTop + 25);
            lblRequestsSubheading.Width = contentWidth;

            cmbStatusFilter.Width = 160;
            cmbStatusFilter.Location = new Point(left + contentWidth - cmbStatusFilter.Width, listTop + 50);
            txtSearch.Location = new Point(left, listTop + 50);
            txtSearch.Width = Math.Max(180, cmbStatusFilter.Left - left - 12);

            lvRequests.Location = new Point(left, listTop + 82);
            lvRequests.Size = new Size(contentWidth, Math.Max(120, ClientSize.Height - listTop - 110));
            lblNoRequests.Location = new Point(left, listTop + 90);
            lblNoRequests.Width = contentWidth;

            ResizeRequestColumns();
        }

        private void ResizeRequestColumns()
        {
            if (lvRequests == null || lvRequests.Columns.Count < 7)
            {
                return;
            }

            const int viewWidth = 56;
            int usable = lvRequests.ClientSize.Width - viewWidth;
            if (usable < 500)
            {
                usable = Math.Max(500, lvRequests.Width - viewWidth - 4);
            }

            int refWidth = Math.Max(60, usable * 8 / 100);
            int categoryWidth = Math.Max(110, usable * 14 / 100);
            int dateWidth = Math.Max(140, usable * 15 / 100);
            int attachmentWidth = Math.Max(120, usable * 16 / 100);
            int statusWidth = Math.Max(120, usable * 14 / 100);
            int locationWidth = Math.Max(140, usable - refWidth - categoryWidth - dateWidth - attachmentWidth - statusWidth);

            lvRequests.Columns[0].Width = refWidth;
            lvRequests.Columns[1].Width = categoryWidth;
            lvRequests.Columns[2].Width = locationWidth;
            lvRequests.Columns[3].Width = dateWidth;
            lvRequests.Columns[4].Width = attachmentWidth;
            lvRequests.Columns[5].Width = statusWidth;
            lvRequests.Columns[6].Width = viewWidth;
        }

        private void RefreshRequestsList()
        {
            if (lvRequests == null || cmbStatusFilter == null || txtSearch == null || lblNoRequests == null)
            {
                return;
            }

            lvRequests.Items.Clear();

            string statusChoice = cmbStatusFilter.SelectedItem != null
                ? cmbStatusFilter.SelectedItem.ToString()
                : "All statuses";
            string search = IsPlaceholder(txtSearch) ? string.Empty : txtSearch.Text.Trim().ToLowerInvariant();

            var issues = IssueRepository.Instance.Issues;
            int matchCount = 0;

            for (int i = issues.Count - 1; i >= 0; i--)
            {
                IssueReport issue = issues[i];
                string displayStatus = NormalizeStatus(issue.Status);

                if (statusChoice != "All statuses" &&
                    !string.Equals(displayStatus, statusChoice, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(search) &&
                    (issue.Location == null || !issue.Location.ToLowerInvariant().Contains(search)) &&
                    (issue.Category == null || !issue.Category.ToLowerInvariant().Contains(search)))
                {
                    continue;
                }

                var item = new ListViewItem(new[]
                {
                    FormatReference(issue.RequestId),
                    issue.Category ?? string.Empty,
                    issue.Location ?? string.Empty,
                    issue.ReportedAt.ToString("dd MMM yyyy HH:mm"),
                    FormatAttachment(issue.AttachmentPath),
                    displayStatus,
                    "\uE890"
                });
                item.Tag = issue;
                lvRequests.Items.Add(item);
                matchCount++;
            }

            bool noIssuesAtAll = issues.Count == 0;
            lblNoRequests.Visible = matchCount == 0;
            lvRequests.Visible = matchCount > 0;
            lblNoRequests.Text = noIssuesAtAll
                ? "No issues have been reported yet. Click \"Report Issues\" to get started."
                : "No reports match your search or filter.";

            lvRequests.Invalidate();
            lvRequests.Update();
            ResizeRequestColumns();
        }

        private static string FormatReference(string requestId)
        {
            if (string.IsNullOrWhiteSpace(requestId))
            {
                return "#";
            }

            int dash = requestId.LastIndexOf('-');
            if (dash >= 0 && dash < requestId.Length - 1)
            {
                string tail = requestId.Substring(dash + 1).TrimStart('0');
                return "#" + (string.IsNullOrEmpty(tail) ? "0" : tail);
            }

            return "#" + requestId;
        }

        private static string FormatAttachment(string attachmentPath)
        {
            if (string.IsNullOrWhiteSpace(attachmentPath))
            {
                return "—";
            }

            try
            {
                return Path.GetFileName(attachmentPath);
            }
            catch
            {
                return attachmentPath;
            }
        }

        private static string NormalizeStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status) ||
                string.Equals(status, "Submitted", StringComparison.OrdinalIgnoreCase))
            {
                return "Pending";
            }

            return status;
        }

        private void SetPlaceholder(TextBox box, string placeholder)
        {
            box.Text = placeholder;
            box.ForeColor = Color.Gray;
            box.Tag = placeholder;

            box.Enter += (s, e) =>
            {
                if (IsPlaceholder(box))
                {
                    box.Text = string.Empty;
                    box.ForeColor = Color.Black;
                }
            };

            box.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(box.Text))
                {
                    box.Text = placeholder;
                    box.ForeColor = Color.Gray;
                }
            };
        }

        private static bool IsPlaceholder(TextBox box)
        {
            return box.Tag is string placeholder &&
                   box.Text == placeholder &&
                   box.ForeColor == Color.Gray;
        }

        private static readonly Color TableHeaderText = Color.FromArgb(107, 114, 128);
        private static readonly Color TableRowText = Color.FromArgb(17, 24, 39);
        private static readonly Color TableDivider = Color.FromArgb(229, 231, 235);
        private static readonly Color TableHeaderLine = Color.FromArgb(229, 231, 235);
        private static readonly Font TableHeaderFont = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
        private static readonly Font TableRowFont = new Font("Segoe UI", 10F);
        private static readonly Font TableBadgeFont = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        private static readonly Font TableEyeFont = new Font("Segoe MDL2 Assets", 12F);

        private void LvRequests_MouseClick(object sender, MouseEventArgs e)
        {
            if (GetHitColumnIndex(e.Location) == 6)
            {
                OpenSelectedIssueDetails(GetHitItem(e.Location));
            }
        }

        private void LvRequests_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OpenSelectedIssueDetails(GetHitItem(e.Location));
        }

        private void LvRequests_MouseMove(object sender, MouseEventArgs e)
        {
            lvRequests.Cursor = GetHitColumnIndex(e.Location) == 6 ? Cursors.Hand : Cursors.Default;
        }

        private ListViewItem GetHitItem(Point location)
        {
            ListViewHitTestInfo hit = lvRequests.HitTest(location);
            return hit.Item;
        }

        private int GetHitColumnIndex(Point location)
        {
            ListViewHitTestInfo hit = lvRequests.HitTest(location);
            if (hit.Item == null || hit.SubItem == null)
            {
                return -1;
            }

            for (int i = 0; i < hit.Item.SubItems.Count; i++)
            {
                if (hit.Item.SubItems[i] == hit.SubItem)
                {
                    return i;
                }
            }

            return -1;
        }

        private void OpenSelectedIssueDetails(ListViewItem item)
        {
            var issue = item == null ? null : item.Tag as IssueReport;
            if (issue == null)
            {
                return;
            }

            using (var detailsForm = new IssueDetailsForm(issue))
            {
                detailsForm.ShowDialog(this);
            }

            RefreshRequestsList();
        }

        private void BtnNotifications_Click(object sender, EventArgs e)
        {
            using (var notifications = new NotificationForm())
            {
                notifications.ShowDialog(this);
            }

            UpdateNotificationBell();
        }

        private void MenuMunicipalDesk_Click(object sender, EventArgs e)
        {
            if (MunicipalDesk.IsOpen)
            {
                MessageBox.Show(
                    this,
                    "The municipal desk is already open for this session. You can publish an announcement from Local Events, and update a report from its details.",
                    "Municipal desk",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            if (MunicipalDesk.TryOpen(this))
            {
                MessageBox.Show(
                    this,
                    "The desk is open. Publish a notice from Local Events, or open a community report and set it to Pending, In Progress, or Resolved.",
                    "Municipal desk",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void NotificationCentre_Changed()
        {
            if (IsHandleCreated && InvokeRequired)
            {
                BeginInvoke(new Action(UpdateNotificationBell));
                return;
            }

            UpdateNotificationBell();
        }

        private void UpdateNotificationBell()
        {
            if (lblNotificationBadge == null)
            {
                return;
            }

            int unread = NotificationCentre.Instance.UnreadCount;
            lblNotificationBadge.Visible = unread > 0;
            lblNotificationBadge.Text = unread > 9 ? "9+" : unread.ToString();
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
                TableHeaderFont,
                new Rectangle(e.Bounds.X + 8, e.Bounds.Y, e.Bounds.Width - 8, e.Bounds.Height),
                TableHeaderText,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);

            using (var pen = new Pen(TableHeaderLine))
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

            Color rowBack = e.ItemIndex % 2 == 0 ? Color.White : Color.FromArgb(252, 252, 253);
            if (e.Item.Selected)
            {
                rowBack = Color.FromArgb(245, 247, 250);
            }

            using (var brush = new SolidBrush(rowBack))
            {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }

            using (var pen = new Pen(TableDivider))
            {
                e.Graphics.DrawLine(pen, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
            }

            if (e.ColumnIndex == 6)
            {
                TextRenderer.DrawText(
                    e.Graphics,
                    "\uE890",
                    TableEyeFont,
                    e.Bounds,
                    UITheme.Navy,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                return;
            }

            if (e.ColumnIndex != 5)
            {
                Color textColor = e.ColumnIndex == 4 && e.SubItem.Text == "—"
                    ? TableHeaderText
                    : TableRowText;

                TextRenderer.DrawText(
                    e.Graphics,
                    e.SubItem.Text,
                    TableRowFont,
                    new Rectangle(e.Bounds.X + 8, e.Bounds.Y, e.Bounds.Width - 12, e.Bounds.Height),
                    textColor,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
                return;
            }

            DrawStatusBadge(e.Graphics, e.Bounds, e.SubItem.Text);
        }

        private static void DrawStatusBadge(Graphics graphics, Rectangle cellBounds, string status)
        {
            StatusBadge.Draw(graphics, cellBounds, status, TableBadgeFont);
        }
    }
}
