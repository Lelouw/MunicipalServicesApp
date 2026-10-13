using System;
using System.Drawing;
using System.Windows.Forms;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Forms
{
    public partial class EventDetailsForm : Form
    {
        private readonly LocalEvent item;

        public EventDetailsForm(LocalEvent item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            this.item = item;
            InitializeComponent();
            BindEvent();
        }

        private void BindEvent()
        {
            lblTitle.Text = item.Title;
            lblSubtitle.Text = item.KindLabel + "  ·  " + item.Category;
            lblValueWhen.Text = item.StartDate.ToString("dddd, dd MMMM yyyy");
            lblValueWhere.Text = string.IsNullOrWhiteSpace(item.Location) ? "—" : item.Location;
            lblValueCategory.Text = item.Category;
            rtbDescription.Text = string.IsNullOrWhiteSpace(item.Description)
                ? "No further details were published."
                : item.Description;

            if (item.IsAnnouncement)
            {
                lblPriority.Visible = true;
                lblPriority.Text = item.PriorityLabel + " notice";
                lblPriority.ForeColor = item.Priority >= LocalEvent.PriorityUrgent ? UITheme.Danger : UITheme.Navy;
            }
            else
            {
                lblPriority.Visible = false;
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
