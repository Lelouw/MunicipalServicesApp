using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MunicipalServicesApp.Data;
using MunicipalServicesApp.Models;

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

        private string selectedAttachmentPath = string.Empty;

        public ReportIssuesForm()
        {
            InitializeComponent();
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
            UpdateEngagement();
        }

        private void CboCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateEngagement();
        }

        private void RtbDescription_TextChanged(object sender, EventArgs e)
        {
            UpdateEngagement();
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
                lblAttachment.Text = Path.GetFileName(selectedAttachmentPath);
                lblAttachment.ForeColor = UiTheme.Text;
                UpdateEngagement();
            }
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            string validationMessage;
            if (!TryValidateReport(out validationMessage))
            {
                MessageBox.Show(
                    this,
                    validationMessage,
                    "Incomplete report",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var report = new IssueReport
            {
                RequestId = CreateRequestId(),
                Location = txtLocation.Text.Trim(),
                Category = cboCategory.SelectedItem.ToString(),
                Description = rtbDescription.Text.Trim(),
                AttachmentPath = selectedAttachmentPath,
                ReportedAt = DateTime.Now,
                Status = "Submitted"
            };

            IssueRepository.Instance.Add(report);
            MessageBox.Show(
                this,
                "Thank you. Your issue has been submitted." + Environment.NewLine +
                Environment.NewLine +
                "Request ID: " + report.RequestId + Environment.NewLine +
                "Location: " + report.Location + Environment.NewLine +
                "Category: " + report.Category + Environment.NewLine +
                Environment.NewLine +
                "The municipality will use these details to follow up. Status tracking will be available in Part 2.",
                "Report submitted",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            ResetForm();
        }

        private void BtnBackToMenu_Click(object sender, EventArgs e)
        {
            Close();
        }

        private bool TryValidateReport(out string message)
        {
            if (string.IsNullOrWhiteSpace(txtLocation.Text))
            {
                message = "Please enter the location of the issue.";
                txtLocation.Focus();
                return false;
            }

            if (!HasCategory())
            {
                message = "Please select a category for the issue.";
                cboCategory.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(rtbDescription.Text) ||
                rtbDescription.Text.Trim().Length < MinimumDescriptionLength)
            {
                message = "Please describe the issue in at least " + MinimumDescriptionLength + " characters.";
                rtbDescription.Focus();
                return false;
            }

            message = string.Empty;
            return true;
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

            prgEngagement.Value = progress;
            lblEngagement.Text = GetEngagementMessage(progress);
            lblEngagement.ForeColor = progress == 100 ? UiTheme.Success : UiTheme.Navy;
            btnSubmit.Enabled = progress >= 75;
        }

        private static string GetEngagementMessage(int progress)
        {
            switch (progress)
            {
                case 0:
                    return "Start by entering the location of the issue.";
                case 25:
                    return "Good start. Please select a category.";
                case 50:
                    return "Thank you. Describe what you observed.";
                case 75:
                    return "Almost there — attach a photo or document if you can, then submit.";
                case 100:
                    return "Excellent. Your report is ready to submit. Thank you for helping your community.";
                default:
                    return "Keep going — complete each step so the municipality can act.";
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
            txtLocation.Clear();
            cboCategory.SelectedIndex = 0;
            rtbDescription.Clear();
            selectedAttachmentPath = string.Empty;
            lblAttachment.Text = "No file attached yet.";
            lblAttachment.ForeColor = UiTheme.Muted;
            UpdateEngagement();
            txtLocation.Focus();
        }
    }
}
