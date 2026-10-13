using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MunicipalServicesApp.Data;
using MunicipalServicesApp.Models;
using MunicipalServicesApp.Services;

namespace MunicipalServicesApp.Forms
{
    public partial class LocalEventsForm : Form
    {
        private const string SearchPlaceholder = "Search by place, title, or word";
        private const int MaxRecommendations = 4;
        private const int SidePadding = 16;
        private const int RecommendationCardHeight = 78;
        private const int CompactRowHeight = 40;
        private const int CardGap = 12;

        private readonly EventRepository catalogue = EventRepository.Instance;
        private readonly EventRecommendationService recommendations;
        private readonly ToolTip tips = new ToolTip();

        private bool filtersReady;
        private int lastCardWidth;
        private IList<LocalEvent> currentResults = new List<LocalEvent>();

        public LocalEventsForm()
        {
            InitializeComponent();
            recommendations = new EventRecommendationService(catalogue);
            tips.SetToolTip(btnClear, "Clears the filters. Search history for recommendations is kept.");
            tips.SetToolTip(dtpFrom, "Optional start of the date search. Uncheck to ignore it.");
            tips.SetToolTip(dtpTo, "Optional end of the date search. Uncheck to ignore it.");
            SetPlaceholder(txtSearch, SearchPlaceholder);
            txtSearch.Enter += TxtSearch_Enter;
            txtSearch.Leave += TxtSearch_Leave;
            Load += LocalEventsForm_Load;
            LoadCategories();
            ResetDatePickers();
            BindPriorityNotices();
            BindAnnouncementQueue();
            ApplyFilters(recordKeyword: false, recordSelection: false);
            filtersReady = true;
            LayoutChrome();
        }

        private void LocalEventsForm_Load(object sender, EventArgs e)
        {
            lastCardWidth = -1;
            LayoutChrome();
            ShowRecommendations();
            ShowRecentlyViewed();
            BindAnnouncementQueue();
        }

        private void TxtSearch_Enter(object sender, EventArgs e)
        {
            if (!IsPlaceholder(txtSearch))
            {
                return;
            }

            txtSearch.TextChanged -= TxtSearch_TextChanged;
            txtSearch.Text = string.Empty;
            txtSearch.ForeColor = Color.Black;
            txtSearch.TextChanged += TxtSearch_TextChanged;
        }

        private void TxtSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                SetPlaceholder(txtSearch, SearchPlaceholder);
            }
        }

        private void LoadCategories()
        {
            cmbCategory.Items.Clear();
            cmbCategory.Items.Add("All categories");
            IList<string> categories = catalogue.GetCategories();
            for (int i = 0; i < categories.Count; i++)
            {
                cmbCategory.Items.Add(categories[i]);
            }

            cmbCategory.SelectedIndex = 0;
        }

        private void ResetDatePickers()
        {
            dtpFrom.Value = DateTime.Today;
            dtpFrom.Checked = true;
            dtpTo.Value = DateTime.Today.AddMonths(3);
            dtpTo.Checked = false;
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            ApplyFilters(recordKeyword: true, recordSelection: true);
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            filtersReady = false;
            cmbCategory.SelectedIndex = 0;
            ResetDatePickers();
            SetPlaceholder(txtSearch, SearchPlaceholder);
            filtersReady = true;
            ApplyFilters(recordKeyword: false, recordSelection: false);
        }

        private void CmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!filtersReady)
            {
                return;
            }

            ApplyFilters(recordKeyword: false, recordSelection: true);
        }

        private void DtpFrom_ValueChanged(object sender, EventArgs e)
        {
            if (!filtersReady)
            {
                return;
            }

            ApplyFilters(recordKeyword: false, recordSelection: true);
        }

        private void DtpTo_ValueChanged(object sender, EventArgs e)
        {
            if (!filtersReady)
            {
                return;
            }

            ApplyFilters(recordKeyword: false, recordSelection: true);
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            if (!filtersReady || IsPlaceholder(txtSearch))
            {
                return;
            }

            ApplyFilters(recordKeyword: false, recordSelection: false);
        }

        private void LocalEventsForm_Resize(object sender, EventArgs e)
        {
            LayoutChrome();
        }

        private void ApplyFilters(bool recordKeyword, bool recordSelection)
        {
            string keyword = CurrentKeyword();
            string category = CurrentCategory();
            DateTime? fromDate;
            DateTime? toDate;
            ReadDateFilter(out fromDate, out toDate);

            currentResults = catalogue.Search(keyword, category, fromDate, toDate);

            if (recordSelection)
            {
                catalogue.RecordSearch(
                    recordKeyword ? keyword : string.Empty,
                    category,
                    PreferenceFrom(),
                    PreferenceTo());
            }

            ShowEvents(fromDate, toDate);
            ShowRecommendations();
            ShowRecentlyViewed();
        }

        private void ShowEvents(DateTime? fromDate, DateTime? toDate)
        {
            bool upcomingOnly = fromDate.HasValue && fromDate.Value.Date >= DateTime.Today;
            string heading = upcomingOnly ? "Upcoming events" : "Events";
            lblUpcoming.Text = currentResults.Count == 1
                ? heading + " (1)"
                : heading + " (" + currentResults.Count + ")";

            bool singleEmptyDay = fromDate.HasValue &&
                                  toDate.HasValue &&
                                  fromDate.Value.Date == toDate.Value.Date &&
                                  !catalogue.ContainsDate(fromDate.Value.Date);

            lblNoEvents.Text = singleEmptyDay
                ? "No events start on this date. Pick another day, or clear the end date."
                : "No events match this search. Try another category or a wider date range.";
            lblNoEvents.Visible = currentResults.Count == 0;
            flpEvents.Visible = currentResults.Count > 0;

            int cardWidth = ComputeCardWidth();
            lastCardWidth = cardWidth;
            Refill(flpEvents, currentResults.Count, cardWidth, (index, width) =>
                EventCardFactory.CreateEventCard(currentResults[index], width, OpenEvent));
        }

        private void ShowRecommendations()
        {
            lblPattern.Text = recommendations.DescribeSearchPatterns();
            IList<RecommendedEvent> suggested = recommendations.Recommend(currentResults, MaxRecommendations);
            int width = SideContentWidth();

            Refill(flpRecommend, suggested.Count, width, (index, cardWidth) =>
                EventCardFactory.CreateRecommendationCard(suggested[index], cardWidth, OpenEvent));

            LayoutSide();
        }

        private void ShowRecentlyViewed()
        {
            IList<LocalEvent> recent = catalogue.GetRecentlyViewed();
            int width = SideContentWidth();
            Refill(flpRecent, recent.Count, width, (index, cardWidth) =>
                EventCardFactory.CreateCompactRow(recent[index], cardWidth, string.Empty, OpenEvent));
            LayoutSide();
        }

        private void BindPriorityNotices()
        {
            IList<LocalEvent> notices = catalogue.GetPriorityAnnouncements();
            lblPriority.Text = notices.Count == 0
                ? "Priority notices"
                : "Priority notices — most urgent first";

            ClearPanel(flpPriority);
            for (int i = 0; i < notices.Count; i++)
            {
                flpPriority.Controls.Add(EventCardFactory.CreatePriorityCard(notices[i], i + 1, OpenEvent));
            }
        }

        private void BindAnnouncementQueue()
        {
            IList<LocalEvent> issued = catalogue.GetAnnouncementQueue();
            int width = SideContentWidth();
            Refill(flpQueue, issued.Count, width, (index, cardWidth) =>
                EventCardFactory.CreateCompactRow(issued[index], cardWidth, (index + 1) + ".  ", OpenEvent));
            LayoutSide();
        }

        private void OpenEvent(LocalEvent item)
        {
            if (item == null)
            {
                return;
            }

            catalogue.RememberViewed(item.Id);
            ShowRecentlyViewed();

            using (var details = new EventDetailsForm(catalogue.FindById(item.Id) ?? item))
            {
                details.ShowDialog(this);
            }
        }

        private void LayoutChrome()
        {
            if (pnlFilters == null || btnBack == null)
            {
                return;
            }

            btnBack.Location = new Point(pnlHeader.ClientSize.Width - btnBack.Width - 24, 26);

            int y = 16;
            int right = pnlFilters.ClientSize.Width - 16;
            btnClear.SetBounds(right - 84, y - 2, 84, 32);
            btnSearch.SetBounds(btnClear.Left - 108, y - 2, 100, 32);
            dtpTo.SetBounds(btnSearch.Left - 140, y, 128, 27);
            lblTo.Location = new Point(dtpTo.Left - 24, y + 4);
            dtpFrom.SetBounds(lblTo.Left - 136, y, 128, 27);
            lblFrom.Location = new Point(dtpFrom.Left - 40, y + 4);
            cmbCategory.SetBounds(lblFrom.Left - 192, y, 180, 27);
            int searchWidth = cmbCategory.Left - 28;
            if (searchWidth < 140)
            {
                searchWidth = 140;
            }

            txtSearch.SetBounds(16, y, searchWidth, 27);

            int cardWidth = ComputeCardWidth();
            if (filtersReady && cardWidth != lastCardWidth && currentResults.Count > 0)
            {
                lastCardWidth = cardWidth;
                Refill(flpEvents, currentResults.Count, cardWidth, (index, width) =>
                    EventCardFactory.CreateEventCard(currentResults[index], width, OpenEvent));
            }

            LayoutSide();
        }

        private void LayoutSide()
        {
            if (pnlSide == null || lblRecommend == null)
            {
                return;
            }

            int width = SideContentWidth();
            int y = 12;

            lblRecommend.SetBounds(SidePadding, y, width, 22);
            y += 26;

            Size patternSize = TextRenderer.MeasureText(
                lblPattern.Text,
                lblPattern.Font,
                new Size(width, 120),
                TextFormatFlags.WordBreak);
            int patternHeight = Math.Max(32, patternSize.Height + 4);
            lblPattern.SetBounds(SidePadding, y, width, patternHeight);
            y += patternHeight + 8;

            y = PlaceFlow(flpRecommend, y, width, RecommendationCardHeight);
            y += 8;

            lblRecent.SetBounds(SidePadding, y, width, 22);
            y += 26;
            y = PlaceFlow(flpRecent, y, width, CompactRowHeight);
            y += 10;

            lblQueue.SetBounds(SidePadding, y, width, 36);
            y += 40;
            PlaceFlow(flpQueue, y, width, CompactRowHeight);
        }

        private int PlaceFlow(FlowLayoutPanel panel, int y, int width, int rowHeight)
        {
            int rows = panel.Controls.Count;
            int height = rows == 0 ? 0 : (rows * (rowHeight + CardGap));
            panel.SetBounds(SidePadding, y, width, height);
            for (int i = 0; i < panel.Controls.Count; i++)
            {
                int margin = panel.Controls[i].Margin.Horizontal;
                panel.Controls[i].Width = Math.Max(180, width - margin);
            }

            return y + height;
        }

        private int SideContentWidth()
        {
            int width = pnlSide.ClientSize.Width - (SidePadding * 2) - 8;
            return Math.Max(220, width);
        }

        private int ComputeCardWidth()
        {
            int available = flpEvents.ClientSize.Width - flpEvents.Padding.Horizontal - 24;
            if (available < 280)
            {
                available = 280;
            }

            if (available >= 680)
            {
                return (available / 2) - CardGap;
            }

            return available;
        }

        private void ReadDateFilter(out DateTime? fromDate, out DateTime? toDate)
        {
            fromDate = dtpFrom.Checked ? dtpFrom.Value.Date : (DateTime?)null;
            toDate = dtpTo.Checked ? dtpTo.Value.Date : (DateTime?)null;
            if (fromDate.HasValue && toDate.HasValue && toDate.Value < fromDate.Value)
            {
                DateTime? swap = fromDate;
                fromDate = toDate;
                toDate = swap;
            }
        }

        private DateTime? PreferenceFrom()
        {
            if (!dtpFrom.Checked)
            {
                return null;
            }

            if (dtpTo.Checked || dtpFrom.Value.Date != DateTime.Today)
            {
                return dtpFrom.Value.Date;
            }

            return null;
        }

        private DateTime? PreferenceTo()
        {
            if (!dtpTo.Checked)
            {
                return null;
            }

            return dtpTo.Value.Date;
        }

        private string CurrentKeyword()
        {
            if (IsPlaceholder(txtSearch))
            {
                return string.Empty;
            }

            return txtSearch.Text.Trim();
        }

        private string CurrentCategory()
        {
            if (cmbCategory.SelectedIndex <= 0 || cmbCategory.SelectedItem == null)
            {
                return string.Empty;
            }

            return cmbCategory.SelectedItem.ToString();
        }

        private void Refill(FlowLayoutPanel panel, int count, int cardWidth, Func<int, int, Control> create)
        {
            panel.SuspendLayout();
            ClearPanel(panel);
            for (int i = 0; i < count; i++)
            {
                panel.Controls.Add(create(i, cardWidth));
            }

            panel.ResumeLayout();
        }

        private static void ClearPanel(Control panel)
        {
            while (panel.Controls.Count > 0)
            {
                Control child = panel.Controls[0];
                panel.Controls.RemoveAt(0);
                child.Dispose();
            }
        }

        private void SetPlaceholder(TextBox box, string placeholder)
        {
            box.TextChanged -= TxtSearch_TextChanged;
            box.Text = placeholder;
            box.ForeColor = Color.Gray;
            box.Tag = placeholder;
            box.TextChanged += TxtSearch_TextChanged;
        }

        private static bool IsPlaceholder(TextBox box)
        {
            return box.Tag is string placeholder &&
                   box.Text == placeholder &&
                   box.ForeColor == Color.Gray;
        }

        private void PnlFilters_Paint(object sender, PaintEventArgs e)
        {
            using (var pen = new Pen(Color.FromArgb(229, 231, 235)))
            {
                e.Graphics.DrawLine(pen, 0, pnlFilters.Height - 1, pnlFilters.Width, pnlFilters.Height - 1);
            }
        }

        private void PnlSide_Paint(object sender, PaintEventArgs e)
        {
            using (var pen = new Pen(Color.FromArgb(229, 231, 235)))
            {
                e.Graphics.DrawLine(pen, 0, 0, 0, pnlSide.Height);
            }
        }
    }
}
