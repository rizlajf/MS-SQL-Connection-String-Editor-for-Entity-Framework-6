﻿using System;
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
        string NewServer = string.Empty;
        string NewDb = string.Empty;
        IList<connectionString> connectionStringsList = null;
        Configuration config = null;
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
            config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
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
            connectionStringsList = connectionStringObject.connectionStrings;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {            
            DBNameComboBox.Enabled = false;
            groupBox1.Visible = true;
            groupBox1.Text = this.comboBox1.GetItemText(this.comboBox1.SelectedItem);
            groupBox2.Visible = false;
            groupBox3.Visible = false;
            NameTextBox.Text = this.comboBox1.GetItemText(this.comboBox1.SelectedItem);
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
            this.DBNameComboBox.DataSource = null;
            this.DBNameComboBox.Items.Clear();
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
            if(!string.IsNullOrEmpty(userName))
            {
                connection.UserID = userName;
            }
            if (!string.IsNullOrEmpty(password))
            {
                connection.Password = password;
            }
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
            NewDb = this.DBNameComboBox.GetItemText(this.DBNameComboBox.SelectedItem);
        }

        private void ServerNameComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.DBNameComboBox.DataSource = null;
            this.DBNameComboBox.Items.Clear();
            this.DBNameComboBox.Enabled = true;
            NewServer = this.ServerNameComboBox.GetItemText(this.ServerNameComboBox.SelectedItem);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            this.DBNameComboBox.DataSource = null;
            this.DBNameComboBox.Items.Clear();
            groupBox3.Visible = true;
            string userName = UserNametextBox.Text;
            string pw = PassWordTextBox.Text;
            LoginAndLoad(userName, pw);
        }

        private void TestButton_Click(object sender, EventArgs e)
        {

        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder(); 
            connectionString newCS = connectionStringsList.Single(p => p.Name == this.comboBox1.GetItemText(this.comboBox1.SelectedItem));
            //newCS.ConnectionString =

            // Set the properties for the data source.
            sqlBuilder.DataSource = NewServer;
            sqlBuilder.InitialCatalog = NewDb;
            sqlBuilder.IntegratedSecurity = true;

            // Build the SqlConnection connection string.
            string providerString = sqlBuilder.ToString();
            newCS.ConnectionString = providerString;
            newCS.Name = NameTextBox.Text;
            newCS.Provider = PNTextBox.Text;

            //// Initialize the EntityConnectionStringBuilder.
            //EntityConnectionStringBuilder entityBuilder =
            //new EntityConnectionStringBuilder();

            ////Set the provider name.
            //entityBuilder.Provider = providerName;

            //// Set the provider-specific connection string.
            //entityBuilder.ProviderConnectionString = providerString;

            //// Set the Metadata location.
            //entityBuilder.Metadata = @"res://*/AdventureWorksModel.csdl|
            //            res://*/AdventureWorksModel.ssdl|
            //            res://*/AdventureWorksModel.msl";
            //Console.WriteLine(entityBuilder.ToString());

            //using (EntityConnection conn =
            //new EntityConnection(entityBuilder.ToString()))
            //{
            //    conn.Open();
            //    Console.WriteLine("Just testing the connection.");
            //    conn.Close();
            //}

            Configuration Config1 = config;
            ConnectionStringsSection conSetting = (ConnectionStringsSection)Config1.GetSection("connectionStrings");
            ConnectionStringSettings StringSettings = new ConnectionStringSettings(newCS.Name, newCS.ConnectionString, newCS.Provider);
            conSetting.ConnectionStrings.Remove(StringSettings);
            conSetting.ConnectionStrings.Add(StringSettings);
            Config1.Save(ConfigurationSaveMode.Modified);
            ClearFormData();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            ClearFormData();
        }

        private void ClearFormData()
        {
            comboBox1.SelectedIndex = -1;
            ServerNameComboBox.SelectedIndex = -1;
            DBNameComboBox.SelectedIndex = -1;
            NameTextBox.Clear();
            PNTextBox.Clear();
            LoginNamrTextBox.Clear();
            UserNametextBox.Clear();
            PassWordTextBox.Clear();
            //radioButton1.Checked = false;
            //radioButton2.Checked = false;
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
