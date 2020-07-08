using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Security.Cryptography;
using Domain;

namespace Session
{
    public class Broker
    {
        SqlConnection conn;
        SqlConnectionStringBuilder connStringBuilder;
        
        void ConnectTo()
        {
            // Data Source=LAPTOP-DIVPBC44;Initial Catalog=ProjectDB;Integrated Security=True
            
            connStringBuilder = new SqlConnectionStringBuilder();
            connStringBuilder.DataSource = "LAPTOP-DIVPBC44";
            connStringBuilder.InitialCatalog = "ProjectDB";
            connStringBuilder.IntegratedSecurity = true;

            conn = new SqlConnection(connStringBuilder.ToString());
        }

        public Broker()
        {
            ConnectTo();
        }

        public void InsertProject(Project project)
        {
            try
            {
                string cmdText = "INSERT INTO dbo.Projects(Name) VALUES(@Name)";
                SqlCommand cmd = new SqlCommand(cmdText, conn);
                cmd.Parameters.AddWithValue("@Name", project.Name);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch(Exception)
            {
                throw;
            }
            finally
            {
                if(conn != null)
                {
                    conn.Close();
                }
            }
        }

        public void InsertMember(Person person)
        {
            try
            {
                string cmdText = "INSERT INTO dbo.Members([First Name], [Middle Name], [Last Name], [Time Available]) VALUES(@Firstname, @MiddleName, @Lastname, @Time)";
                SqlCommand cmd = new SqlCommand(cmdText, conn);
                cmd.Parameters.AddWithValue("@FirstName", person.Firstname);
                cmd.Parameters.AddWithValue("@MiddleName", person.Middlename);
                cmd.Parameters.AddWithValue("@LastName", person.Lastname);
                cmd.Parameters.AddWithValue("@Time", person.Time);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if(conn != null)
                {
                    conn.Close(); 
                }
            }
        }

        public void Involve(Person person, Project project, int time)
        {
            try
            {
                string cmdText = "INSERT INTO dbo.MembersTOProjects(MemberID, ProjectID, [Time Involved]) VALUES(@MemberID, @ProjectID, @Time)";
                SqlCommand cmd = new SqlCommand(cmdText, conn);
                cmd.Parameters.AddWithValue("@MemberID", person.Id);
                cmd.Parameters.AddWithValue("@ProjectID", project.Id);
                cmd.Parameters.AddWithValue("@Time", time);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        public List<Person> GetPersonsList()
        {
            List<Person> personsList = new List<Person>();
            try
            {
                string cmdText = "SELECT * FROM dbo.Members";
                SqlCommand cmd = new SqlCommand(cmdText, conn);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Person person = new Person();
                    person.Id = Convert.ToInt32(reader["ID"].ToString());
                    person.Firstname = reader["First Name"].ToString();
                    person.Middlename = reader["Middle Name"].ToString();
                    person.Lastname = reader["Last Name"].ToString();
                    person.Time = Convert.ToInt32(reader["Time Available"].ToString());

                    personsList.Add(person);
                }
                return personsList;
            }
            catch(Exception)
            {
                throw;
            }
            finally
            {
                if(conn != null)
                {
                    conn.Close();
                }
            }
        }

        public List<Project> GetProjectsList()
        {
            List<Project> projectsList = new List<Project>();
            try
            {
                string cmdText = "SELECT * FROM dbo.Projects";
                SqlCommand cmd = new SqlCommand(cmdText, conn);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Project project = new Project();
                    project.Id = Convert.ToInt32(reader["ID"].ToString());
                    project.Name = reader["Name"].ToString();

                    projectsList.Add(project);
                }
                return projectsList;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }

        }

        public int GetFreeTime(Person person)
        {
            try
            {
                string cmdText = "SELECT SUM([Time Involved]) FROM dbo.MembersTOProjects WHERE MemberID=@MemberID";
                SqlCommand cmd = new SqlCommand(cmdText, conn);
                cmd.Parameters.AddWithValue("@MemberID", person.Id);
                conn.Open();
                int TimeInvolved = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                return (person.Time - TimeInvolved) * 100 / person.Time;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        public List<string> GetPercentagesList(Person person)
        {
            try
            {
                List<string> Percentages = new List<string>();
                string cmdText = "SELECT Name, [Time Involved] " +
                                 "FROM dbo.MembersTOProjects " +
                                 "JOIN dbo.Projects " +
                                 "  ON dbo.MembersTOProjects.ProjectID = dbo.Projects.ID " +
                                 "WHERE MemberID=@MemberID";

                SqlCommand cmd = new SqlCommand(cmdText, conn);
                cmd.Parameters.AddWithValue("@MemberID", person.Id);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string involvement;
                    involvement = reader["Name"].ToString() + ": " + (Convert.ToInt32(reader["Time Involved"]) * 100 / person.Time).ToString() + "%";
                    Percentages.Add(involvement);
                }

                return Percentages;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }

        }

        public List<string> GetBusiestProjects()
        {
            try
            {
                List<string> BusyProjectsList = new List<string>();
                string cmdText = "SELECT Name, Percs, Cnts " +
                                    "FROM(   SELECT ProjectID, SUM([Time Involved] * 100 / [Time Available]) Percs, COUNT([Time Involved]) Cnts " +
                                    "        FROM dbo.MembersTOProjects " +
                                    "        JOIN dbo.Members " +
                                    "            ON dbo.MembersTOProjects.MemberID = dbo.Members.ID " +
                                    "        GROUP BY dbo.MembersTOProjects.ProjectID) KK " +
                                    "JOIN dbo.Projects " +
                                    "    ON KK.ProjectID = dbo.Projects.ID " +
                                    "ORDER BY Percs DESC ";

                SqlCommand cmd = new SqlCommand(cmdText, conn);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                int count = 0;

                while (count < 5 && reader.Read())
                {
                    string BusyProject;
                    BusyProject = reader["Name"].ToString() + ": " + reader["Percs"].ToString() + "% By " + reader["Cnts"].ToString() + " Members";
                    BusyProjectsList.Add(BusyProject);
                    count++;
                }

                return BusyProjectsList;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }

        }
    }
}
