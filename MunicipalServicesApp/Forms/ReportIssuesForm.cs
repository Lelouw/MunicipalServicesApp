using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MunicipalServicesApp.Data;
using MunicipalServicesApp.Models;
using MunicipalServicesApp.Services;

namespace MunicipalServicesApp.Forms
{
    public partial class ReportIssuesForm : Form
    {
        private const int LocationProgress = 25;
        private const int CategoryProgress = 25;
        private const int DescriptionProgress = 25;
        private const int AttachmentProgress = 25;
        private const int MinimumDescriptionLength = 10;
        private const string AttachmentFilter =
            "Images and documents|*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.pdf;*.doc;*.docx|" +
            "Image files|*.jpg;*.jpeg;*.png;*.gif;*.bmp|" +
            "Documents|*.pdf;*.doc;*.docx|" +
            "All files|*.*";
        private const string LocationHintDefault =
            "Type 3 or more letters — South African place suggestions will appear.";

        private readonly LocationSearchSettings locationSearchSettings;
        private readonly ILocationSuggestionService locationSuggestionService;
        private readonly System.Windows.Forms.Timer locationSearchTimer;

        private string selectedAttachmentPath = string.Empty;
        private bool suppressLocationSearch;
        private CancellationTokenSource locationSearchCts;

        public ReportIssuesForm()
        {
            InitializeComponent();
            locationSearchSettings = LocationSearchSettings.Load();
            locationSuggestionService = new NominatimLocationSuggestionService(locationSearchSettings);
            locationSearchTimer = new System.Windows.Forms.Timer();
            locationSearchTimer.Interval = locationSearchSettings.DebounceMilliseconds;
            locationSearchTimer.Tick += LocationSearchTimer_Tick;
            LoadCategories();
            UpdateEngagement();
        }

        private void LoadCategories()
        {
            cboCategory.Items.Clear();
            cboCategory.Items.Add("Select a category");
            foreach (string category in IssueCategories.All)
            {
                cboCategory.Items.Add(category);
            }

            cboCategory.SelectedIndex = 0;
        }

        private void TxtLocation_TextChanged(object sender, EventArgs e)
        {
            if (lblLocationHint != null && lblLocationHint.ForeColor == Color.FromArgb(176, 42, 55))
            {
                txtLocation.BackColor = Color.White;
                SetLocationHint(LocationHintDefault, UITheme.Muted);
            }

            UpdateEngagement();
            QueueLocationSearch();
        }

        private void TxtLocation_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                HideLocationSuggestions();
                e.Handled = true;
                return;
            }

            if (e.KeyCode == Keys.Down && lstLocationSuggestions.Visible && lstLocationSuggestions.Items.Count > 0)
            {
                lstLocationSuggestions.Focus();
                if (lstLocationSuggestions.SelectedIndex < 0)
                {
                    lstLocationSuggestions.SelectedIndex = 0;
                }

                e.Handled = true;
            }
        }

        private async void TxtLocation_Leave(object sender, EventArgs e)
        {
            await Task.Delay(200).ConfigureAwait(true);
            if (IsDisposed)
            {
                return;
            }

            if (!txtLocation.Focused && !lstLocationSuggestions.Focused)
            {
                HideLocationSuggestions();
            }
        }

        private void LstLocationSuggestions_Click(object sender, EventArgs e)
        {
            ApplySelectedLocationSuggestion();
        }

        private void LstLocationSuggestions_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ApplySelectedLocationSuggestion();
                e.Handled = true;
                return;
            }

            if (e.KeyCode == Keys.Escape)
            {
                HideLocationSuggestions();
                txtLocation.Focus();
                e.Handled = true;
                return;
            }

            if (e.KeyCode == Keys.Up && lstLocationSuggestions.SelectedIndex <= 0)
            {
                txtLocation.Focus();
                txtLocation.SelectionStart = txtLocation.Text.Length;
                e.Handled = true;
            }
        }

        private void QueueLocationSearch()
        {
            if (suppressLocationSearch)
            {
                return;
            }

            locationSearchTimer.Stop();
            CancelLocationSearch();

            string query = txtLocation.Text.Trim();
            if (query.Length < locationSearchSettings.MinimumQueryLength)
            {
                HideLocationSuggestions();
                SetLocationHint(LocationHintDefault, UITheme.Muted);
                return;
            }

            SetLocationHint("Looking up places...", UITheme.Muted);
            locationSearchTimer.Start();
        }

        private async void LocationSearchTimer_Tick(object sender, EventArgs e)
        {
            locationSearchTimer.Stop();
            string query = txtLocation.Text.Trim();
            if (query.Length < locationSearchSettings.MinimumQueryLength)
            {
                HideLocationSuggestions();
                return;
            }

            await SearchLocationsAsync(query);
        }

        private async Task SearchLocationsAsync(string query)
        {
            CancelLocationSearch();
            locationSearchCts = new CancellationTokenSource();
            CancellationToken token = locationSearchCts.Token;

            try
            {
                IReadOnlyList<LocationSuggestion> suggestions =
                    await locationSuggestionService.SearchAsync(query, token).ConfigureAwait(true);

                if (IsDisposed || token.IsCancellationRequested)
                {
                    return;
                }

                ShowLocationSuggestions(suggestions);
            }
            catch (OperationCanceledException)
            {
                if (!IsDisposed && !token.IsCancellationRequested)
                {
                    HideLocationSuggestions();
                    SetLocationHint("Could not load suggestions. You can still type the location.", UITheme.Muted);
                }
            }
            catch (HttpRequestException)
            {
                if (!IsDisposed)
                {
                    HideLocationSuggestions();
                    SetLocationHint("Could not load suggestions. You can still type the location.", UITheme.Muted);
                }
            }
        }

        private void ShowLocationSuggestions(IReadOnlyList<LocationSuggestion> suggestions)
        {
            lstLocationSuggestions.BeginUpdate();
            try
            {
                lstLocationSuggestions.Items.Clear();
                if (suggestions != null)
                {
                    foreach (LocationSuggestion suggestion in suggestions)
                    {
                        lstLocationSuggestions.Items.Add(suggestion);
                    }
                }
            }
            finally
            {
                lstLocationSuggestions.EndUpdate();
            }

            if (lstLocationSuggestions.Items.Count == 0)
            {
                HideLocationSuggestions();
                SetLocationHint("No matching places found. You can still enter the address yourself.", UITheme.Muted);
                return;
            }

            lstLocationSuggestions.SelectedIndex = -1;
            lstLocationSuggestions.BringToFront();
            lstLocationSuggestions.Visible = true;
            SetLocationHint("Select a suggestion, or keep typing your own address.", UITheme.Navy);
        }

        private void ApplySelectedLocationSuggestion()
        {
            var suggestion = lstLocationSuggestions.SelectedItem as LocationSuggestion;
            if (suggestion == null || string.IsNullOrWhiteSpace(suggestion.DisplayName))
            {
                return;
            }

            suppressLocationSearch = true;
            txtLocation.Text = suggestion.DisplayName;
            txtLocation.SelectionStart = txtLocation.Text.Length;
            suppressLocationSearch = false;
            HideLocationSuggestions();
            SetLocationHint("Location selected from live place suggestions.", UITheme.Success);
            txtLocation.Focus();
            UpdateEngagement();
        }

        private void HideLocationSuggestions()
        {
            lstLocationSuggestions.Visible = false;
            lstLocationSuggestions.Items.Clear();
        }

        private void SetLocationHint(string text, Color color)
        {
            lblLocationHint.Text = text;
            lblLocationHint.ForeColor = color;
        }

        private void CancelLocationSearch()
        {
            if (locationSearchCts == null)
            {
                return;
            }

            locationSearchCts.Cancel();
            locationSearchCts.Dispose();
            locationSearchCts = null;
        }

        private void BtnAttachMedia_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "Attach an image or document";
                dialog.Filter = AttachmentFilter;
                dialog.Multiselect = false;
                dialog.CheckFileExists = true;

                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                selectedAttachmentPath = dialog.FileName;
                lblAttachment.Text = "✓  " + Path.GetFileName(selectedAttachmentPath);
                lblAttachment.ForeColor = Color.FromArgb(26, 120, 60);
                UpdateEngagement();
            }
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            if (!TryValidateReport())
            {
                return;
            }

            string location = txtLocation.Text.Trim();
            string category = cboCategory.SelectedItem.ToString();

            IssueReport duplicate = FindPossibleDuplicate(location, category);
            if (duplicate != null)
            {
                string status = string.IsNullOrWhiteSpace(duplicate.Status) ? "Pending" : duplicate.Status;
                if (string.Equals(status, "Submitted", StringComparison.OrdinalIgnoreCase))
                {
                    status = "Pending";
                }

                DialogResult choice = MessageBox.Show(
                    this,
                    "A similar issue was already reported at \"" + duplicate.Location + "\" (" +
                    duplicate.Category + ", status: " + status + ", ref " + FormatReference(duplicate.RequestId) + ")." +
                    Environment.NewLine + Environment.NewLine +
                    "Do you still want to submit this as a new report?",
                    "Possible duplicate report",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (choice != DialogResult.Yes)
                {
                    return;
                }
            }

            var report = new IssueReport
            {
                RequestId = CreateRequestId(),
                Location = location,
                Category = category,
                Description = rtbDescription.Text.Trim(),
                AttachmentPath = selectedAttachmentPath,
                ReportedAt = DateTime.Now,
                Status = "Pending"
            };

            IssueRepository.Instance.Add(report);
            MunicipalServicesApp.Services.NotificationCentre.Instance.FollowSubmittedReport(report);
            MessageBox.Show(
                this,
                "Thank you. Your issue has been submitted." + Environment.NewLine +
                Environment.NewLine +
                "Reference number: " + FormatReference(report.RequestId) + Environment.NewLine +
                "Status: " + report.Status + Environment.NewLine +
                "Location: " + report.Location + Environment.NewLine +
                "Category: " + report.Category + Environment.NewLine +
                Environment.NewLine +
                "Track this reference under Service Request Status. The bell on the main menu will tell you when it moves to In Progress or Resolved.",
                "Report submitted",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            DialogResult = DialogResult.OK;
            Close();
        }

        private static IssueReport FindPossibleDuplicate(string location, string category)
        {
            if (string.IsNullOrWhiteSpace(location) || string.IsNullOrWhiteSpace(category))
            {
                return null;
            }

            string locationLower = location.ToLowerInvariant();

            foreach (IssueReport issue in IssueRepository.Instance.Issues)
            {
                if (string.Equals(issue.Status, "Closed", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!string.Equals(issue.Category, category, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(issue.Location))
                {
                    continue;
                }

                string existingLower = issue.Location.ToLowerInvariant();
                if (existingLower.Contains(locationLower) || locationLower.Contains(existingLower))
                {
                    return issue;
                }
            }

            return null;
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

        private void BtnBackToMenu_Click(object sender, EventArgs e)
        {
            Close();
        }

        private bool TryValidateReport()
        {
            ClearFieldErrors();
            bool isValid = true;
            Control focusTarget = null;

            if (string.IsNullOrWhiteSpace(txtLocation.Text))
            {
                ShowLocationError("Please enter the location of the issue.");
                focusTarget = txtLocation;
                isValid = false;
            }

            if (!HasCategory())
            {
                ShowCategoryError("Please select a category for the issue.");
                if (focusTarget == null)
                {
                    focusTarget = cboCategory;
                }

                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(rtbDescription.Text) ||
                rtbDescription.Text.Trim().Length < MinimumDescriptionLength)
            {
                ShowDescriptionError(
                    "Please describe the issue in at least " + MinimumDescriptionLength + " characters.");
                if (focusTarget == null)
                {
                    focusTarget = rtbDescription;
                }

                isValid = false;
            }

            if (focusTarget != null)
            {
                focusTarget.Focus();
            }

            return isValid;
        }

        private void ClearFieldErrors()
        {
            Color normalBorder = Color.FromArgb(210, 214, 220);

            txtLocation.BackColor = Color.White;
            if (pnlCategoryHost != null)
            {
                pnlCategoryHost.BackColor = Color.White;
            }

            cboCategory.BackColor = Color.White;
            rtbDescription.BackColor = Color.White;

            if (lblCategoryError != null)
            {
                lblCategoryError.Visible = false;
                lblCategoryError.Text = string.Empty;
            }

            if (lblDescriptionError != null)
            {
                lblDescriptionError.Visible = false;
                lblDescriptionError.Text = string.Empty;
            }

            // Restore location hint only when it was showing a validation error.
            if (lblLocationHint != null &&
                lblLocationHint.ForeColor == Color.FromArgb(176, 42, 55))
            {
                SetLocationHint(LocationHintDefault, UITheme.Muted);
            }
        }

        private void ShowLocationError(string message)
        {
            txtLocation.BackColor = Color.FromArgb(255, 245, 245);
            SetLocationHint(message, Color.FromArgb(176, 42, 55));
        }

        private void ShowCategoryError(string message)
        {
            if (pnlCategoryHost != null)
            {
                pnlCategoryHost.BackColor = Color.FromArgb(255, 245, 245);
            }

            cboCategory.BackColor = Color.FromArgb(255, 245, 245);
            lblCategoryError.Text = message;
            lblCategoryError.Visible = true;
        }

        private void ShowDescriptionError(string message)
        {
            rtbDescription.BackColor = Color.FromArgb(255, 245, 245);
            lblDescriptionError.Text = message;
            lblDescriptionError.Visible = true;
        }

        private void UpdateEngagement()
        {
            int progress = 0;

            if (!string.IsNullOrWhiteSpace(txtLocation.Text))
            {
                progress += LocationProgress;
            }

            if (HasCategory())
            {
                progress += CategoryProgress;
            }

            if (!string.IsNullOrWhiteSpace(rtbDescription.Text))
            {
                progress += DescriptionProgress;
            }

            if (!string.IsNullOrWhiteSpace(selectedAttachmentPath))
            {
                progress += AttachmentProgress;
            }

            UpdateProgressFill(progress);
            lblEngagement.Text = GetEngagementMessage(progress);
            lblEngagement.ForeColor = progress == 100 ? UITheme.Success : UITheme.Navy;
            btnSubmit.Enabled = true;
            btnSubmit.BackColor = UITheme.Navy;
            btnSubmit.ForeColor = Color.White;
        }

        private static string GetEngagementMessage(int progress)
        {
            switch (progress)
            {
                case 0:
                    return "Start by entering the location of the issue.";
                case 25:
                    return "Great start, keep going, you're almost there.";
                case 50:
                    return "Great start, keep going, you're almost there.";
                case 75:
                    return "Great start, keep going, you're almost there.";
                case 100:
                    return "All set! You're ready to submit your report.";
                default:
                    return "Great start, keep going, you're almost there.";
            }
        }

        private bool HasCategory()
        {
            return cboCategory.SelectedIndex > 0;
        }

        private static string CreateRequestId()
        {
            int nextNumber = IssueRepository.Instance.Count + 1;
            return "MSR-" + DateTime.Now.Year + "-" + nextNumber.ToString("0000");
        }

        private void ResetForm()
        {
            suppressLocationSearch = true;
            txtLocation.Clear();
            suppressLocationSearch = false;
            HideLocationSuggestions();
            SetLocationHint(LocationHintDefault, UITheme.Muted);
            cboCategory.SelectedIndex = 0;
            rtbDescription.Clear();
            selectedAttachmentPath = string.Empty;
            lblAttachment.Text = "No file attached yet.";
            lblAttachment.ForeColor = UITheme.Muted;
            UpdateEngagement();
            txtLocation.Focus();
        }

        private void CboCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lblCategoryError != null && lblCategoryError.Visible)
            {
                if (pnlCategoryHost != null)
                {
                    pnlCategoryHost.BackColor = Color.White;
                }

                cboCategory.BackColor = Color.White;
                lblCategoryError.Visible = false;
                lblCategoryError.Text = string.Empty;
            }

            UpdateEngagement();
        }

        private void RtbDescription_TextChanged(object sender, EventArgs e)
        {
            if (lblDescriptionError != null && lblDescriptionError.Visible)
            {
                rtbDescription.BackColor = Color.White;
                lblDescriptionError.Visible = false;
                lblDescriptionError.Text = string.Empty;
            }

            UpdateEngagement();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            locationSearchTimer.Stop();
            locationSearchTimer.Tick -= LocationSearchTimer_Tick;
            locationSearchTimer.Dispose();
            CancelLocationSearch();
            base.OnFormClosed(e);
        }
    }
}
