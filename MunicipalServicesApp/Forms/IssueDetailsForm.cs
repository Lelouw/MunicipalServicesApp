using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MunicipalServicesApp.Models;
using MunicipalServicesApp.Services;

namespace MunicipalServicesApp.Forms
{
    public partial class IssueDetailsForm : Form
    {
        private readonly IssueReport report;
        private Image attachmentPreview;

        public IssueDetailsForm(IssueReport report)
        {
            this.report = report ?? throw new ArgumentNullException(nameof(report));
            InitializeComponent();
            BindReport();
        }

        private void BindReport()
        {
            string reference = FormatReference(report.RequestId);
            string status = NormalizeStatus(report.Status);

            lblTitle.Text = "Issue details";
            lblSubtitle.Text = "Reference " + reference;
            lblValueRef.Text = reference;
            lblValueCategory.Text = report.Category ?? "—";
            lblValueLocation.Text = string.IsNullOrWhiteSpace(report.Location) ? "—" : report.Location;
            lblValueDate.Text = report.ReportedAt.ToString("dd MMM yyyy HH:mm");
            rtbDescription.Text = string.IsNullOrWhiteSpace(report.Description) ? "No description provided." : report.Description;
            lblStatus.Text = status;
            ApplyStatusColors(status);
            HighlightStatusChoice(status);
            LoadAttachment();
        }

        private void BtnSetStatus_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button == null || button.Tag == null)
            {
                return;
            }

            if (!MunicipalDesk.TryOpen(this))
            {
                return;
            }

            string next = button.Tag.ToString();
            string current = NormalizeStatus(report.Status);
            if (string.Equals(current, next, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            DialogResult choice = MessageBox.Show(
                this,
                "Set " + FormatReference(report.RequestId) + " to " + next + "?",
                "Update status",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (choice != DialogResult.Yes)
            {
                return;
            }

            string message;
            bool updated = IssueProgress.TryChange(report, next, out message);
            MessageBox.Show(
                this,
                message,
                "Update status",
                MessageBoxButtons.OK,
                updated ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (updated)
            {
                BindReport();
            }
        }

        private void HighlightStatusChoice(string status)
        {
            Highlight(btnPending, status);
            Highlight(btnInProgress, status);
            Highlight(btnResolved, status);
        }

        private static void Highlight(Button button, string status)
        {
            if (button == null)
            {
                return;
            }

            bool selected = string.Equals(button.Tag as string, status, StringComparison.OrdinalIgnoreCase);
            button.BackColor = selected ? UITheme.Navy : Color.White;
            button.ForeColor = selected ? Color.White : UITheme.Navy;
        }

        private void LoadAttachment()
        {
            string path = report.AttachmentPath;
            if (string.IsNullOrWhiteSpace(path))
            {
                lblAttachmentName.Text = "No file attached.";
                lblAttachmentName.ForeColor = UITheme.Muted;
                btnOpenAttachment.Visible = false;
                picAttachment.Visible = false;
                return;
            }

            string fileName = Path.GetFileName(path);
            bool exists = File.Exists(path);
            lblAttachmentName.Text = exists ? fileName : fileName + " (file not found on this PC)";
            lblAttachmentName.ForeColor = exists ? UITheme.Success : UITheme.Danger;
            btnOpenAttachment.Visible = exists;
            btnOpenAttachment.Enabled = exists;

            if (exists && IsImageFile(path))
            {
                try
                {
                    using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var image = Image.FromStream(stream))
                    {
                        attachmentPreview = new Bitmap(image);
                    }

                    picAttachment.Image = attachmentPreview;
                    picAttachment.Visible = true;
                }
                catch
                {
                    picAttachment.Visible = false;
                }
            }
            else
            {
                picAttachment.Visible = false;
            }
        }

        private void ApplyStatusColors(string status)
        {
            Color backColor;
            Color foreColor;
            StatusBadge.Colors(status, out backColor, out foreColor);
            lblStatus.BackColor = backColor;
            lblStatus.ForeColor = foreColor;
        }

        private void BtnOpenAttachment_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(report.AttachmentPath) || !File.Exists(report.AttachmentPath))
            {
                MessageBox.Show(
                    this,
                    "The attached file could not be found on this computer.",
                    "Attachment unavailable",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = report.AttachmentPath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    "The attachment could not be opened." + Environment.NewLine + ex.Message,
                    "Attachment",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private static bool IsImageFile(string path)
        {
            string extension = Path.GetExtension(path);
            if (string.IsNullOrWhiteSpace(extension))
            {
                return false;
            }

            switch (extension.ToLowerInvariant())
            {
                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".gif":
                case ".bmp":
                    return true;
                default:
                    return false;
            }
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

        private static string NormalizeStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status) ||
                string.Equals(status, "Submitted", StringComparison.OrdinalIgnoreCase))
            {
                return "Pending";
            }

            return status;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (picAttachment.Image != null)
            {
                picAttachment.Image = null;
            }

            if (attachmentPreview != null)
            {
                attachmentPreview.Dispose();
                attachmentPreview = null;
            }

            base.OnFormClosed(e);
        }
    }
}
