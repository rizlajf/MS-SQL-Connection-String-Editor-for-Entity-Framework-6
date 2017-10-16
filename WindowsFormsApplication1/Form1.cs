using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        ConnectionStringClass connectionStringObject = null;
        //SqlConnectionStringBuilder connection = null;
        public Form1()
        {
            InitializeComponent();
            groupBox1.Visible = false;
            ReadFile();
            LoadDataInTheForm();
        }

        private void LoadDataInTheForm()
        {
            foreach (connectionString cs in connectionStringObject.connectionStrings)
            {
                comboBox1.Items.Add(cs.Name);
            }
        }

        public void ReadFile()
        {
            //Then access the command line args from your form ...
            string path = Program.CommandLineArgs[0];
            ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
            configMap.ExeConfigFilename = path;
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
            connectionStringObject = new ConnectionStringClass();
            connectionStringObject.connectionStrings = new List<connectionString>();
            ConnectionStringSettingsCollection connections = config.ConnectionStrings.ConnectionStrings;
            if (connections.Count != 0)
            {
                foreach (ConnectionStringSettings connection in connections)
                {
                    connectionString cs = new connectionString();
                    cs.Name = connection.Name;
                    cs.Provider = connection.ProviderName;
                    cs.ConnectionString = connection.ConnectionString;
                    connectionStringObject.connectionStrings.Add(cs);
                }
            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DBNameComboBox.Enabled = false;
            groupBox1.Visible = true;
            groupBox1.Text = this.comboBox1.GetItemText(this.comboBox1.SelectedItem);
            groupBox2.Visible = false;
            groupBox3.Visible = false;
            LoadListOfValues();
        }

        private void LoadListOfValues()
        {
            DataTable dt = SqlDataSourceEnumerator.Instance.GetDataSources();
            foreach (DataRow dr in dt.Rows)
            {
                ServerNameComboBox.Items.Add(string.Concat(dr["ServerName"], "\\", dr["InstanceName"]));
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            groupBox2.Visible = true;
            string loginName = LoginNamrTextBox.Text;
            LoginAndLoad(loginName, null);
            DBNameComboBox.Enabled = true;
        }

        private void LoginAndLoad(string userName, string password)
        {
            List<String> databases = new List<String>();

            SqlConnectionStringBuilder connection = new SqlConnectionStringBuilder();

            connection.DataSource = this.ServerNameComboBox.GetItemText(this.ServerNameComboBox.SelectedItem);
            // enter credentials if you want
            //connection.UserID = //get username;
            // connection.Password = //get password;
            connection.IntegratedSecurity = true;

            String strConn = connection.ToString();

            //create connection
            SqlConnection sqlConn = new SqlConnection(strConn);

            //open connection
            sqlConn.Open();

            //get databases
            DataTable tblDatabases = sqlConn.GetSchema("Databases");

            //close connection
            sqlConn.Close();

            //add to list
            foreach (DataRow row in tblDatabases.Rows)
            {
                String strDatabaseName = row["database_name"].ToString();

                databases.Add(strDatabaseName);

            }
            DBNameComboBox.DataSource = databases;
        }

        private void DBNameComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ServerNameComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            groupBox3.Visible = true;
            string userName = UserNametextBox.Text;
            string pw = PassWordTextBox.Text;
            LoginAndLoad(userName, pw);
        }

        private void TestButton_Click(object sender, EventArgs e)
        {

        }


        //public List<string> GetDatabaseList()
        //{
        //    List<string> list = new List<string>();

        //    // Open connection to the database
        //    string conString = "server=xeon;uid=sa;pwd=manager; database=northwind";

        //    using (SqlConnection con = new SqlConnection(conString))
        //    {
        //        con.Open();

        //        // Set up a command with the given query and associate
        //        // this with the current connection.
        //        using (SqlCommand cmd = new SqlCommand("SELECT name from sys.databases", con))
        //        {
        //            using (IDataReader dr = cmd.ExecuteReader())
        //            {
        //                while (dr.Read())
        //                {
        //                    list.Add(dr[0].ToString());
        //                }
        //            }
        //        }
        //    }
        //    return list;

        //}
    }
}
