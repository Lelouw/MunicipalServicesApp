using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MunicipalServicesApp.Models;

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
            LoadAttachment();
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
            switch (status)
            {
                case "Resolved":
                    lblStatus.BackColor = Color.FromArgb(219, 234, 254);
                    lblStatus.ForeColor = Color.FromArgb(30, 64, 175);
                    break;
                case "Closed":
                    lblStatus.BackColor = Color.FromArgb(229, 231, 235);
                    lblStatus.ForeColor = Color.FromArgb(75, 85, 99);
                    break;
                default:
                    lblStatus.BackColor = Color.FromArgb(254, 243, 199);
                    lblStatus.ForeColor = Color.FromArgb(146, 64, 14);
                    break;
            }
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
