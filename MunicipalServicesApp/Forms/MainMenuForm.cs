using System;
using System.Drawing;
using System.Windows.Forms;

namespace MunicipalServicesApp.Forms
{
    public partial class MainMenuForm : Form
    {
        public MainMenuForm()
        {
            InitializeComponent();
        }

        private void BtnReportIssues_Click(object sender, EventArgs e)
        {
            Hide();
            using (var reportForm = new ReportIssuesForm())
            {
                reportForm.ShowDialog(this);
            }

            Show();
            Activate();
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
