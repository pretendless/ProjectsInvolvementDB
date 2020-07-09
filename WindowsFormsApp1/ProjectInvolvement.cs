using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Domain;
using Session;

namespace DBApp
{
    public partial class ProjectInvolvement : Form
    {
        Broker broker;
        Person person;
        Project project;
        public ProjectInvolvement()
        {
            InitializeComponent();
            broker = new Broker();
        }

        private void btnInsertProject_Click(object sender, EventArgs e)
        {
            project = new Project();
            project.Name = txtProjectName.Text;

            broker.InsertProject(project);

        }

        private void btnInsertMember_Click(object sender, EventArgs e)
        {
            person = new Person();
            person.Firstname = txtFirstName.Text;
            person.Middlename = txtMiddleName.Text;
            person.Lastname = txtLastName.Text;

            try
            {
                person.Time = Convert.ToInt32(txtTimeAvailable.Text);
            }
            catch(Exception)
            {
                throw;
            }

            broker.InsertMember(person);
        }

        private void btnInvolve_Click(object sender, EventArgs e)
        {
            int time;
            person = cmbMembers.SelectedItem as Person;
            project = cmbProjects.SelectedItem as Project;

            try
            {
                time = Convert.ToInt32(txtTimeInvolved.Text);
            }
            catch(Exception)
            {
                throw;
            }

            broker.Involve(person, project, time);
        }

        private void btnRefreshMembers_Click(object sender, EventArgs e)
        {
            cmbMembers.DataSource = broker.GetPersonsList();
        }

        private void btnRefreshMembers2_Click(object sender, EventArgs e)
        {
            cmbPersons.DataSource = broker.GetPersonsList();
        }

        private void btnRefreshProjects_Click(object sender, EventArgs e)
        {
            cmbProjects.DataSource = broker.GetProjectsList();
        }

        private void btnFreeTime_Click(object sender, EventArgs e)
        {
            person = cmbPersons.SelectedItem as Person;
            txtFreeTime.Text = broker.GetFreeTime(person).ToString() + "%";
        }

        private void btnGetPercentage_Click(object sender, EventArgs e)
        {
            person = cmbPersons.SelectedItem as Person;
            PercentagesList.DataSource = broker.GetPercentagesList(person);
        }

        private void btnGetBusiestProjects_Click(object sender, EventArgs e)
        {
            BusiestProjectsList.DataSource = broker.GetBusiestProjects();
        }
    }
}
