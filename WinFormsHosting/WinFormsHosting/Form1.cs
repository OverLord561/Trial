using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormsHosting.ServiceReference1;


namespace WinFormsHosting
{
    
    public partial class Form1 : Form
    {
        Service1Client _service = new Service1Client();
        bool isAuthorised = false;
        string hostingPath;
        string userName;
        int userId;
        List<int> indexesOfChangedRows;
        


        public void SetStatus(bool status)
        {
            
            isAuthorised = status;
            uploadButton.Enabled = status;
            logOutButton.Enabled = status;
            progressLabel.Enabled = status;
            buttonUpdate.Enabled = status;
        }

        public void SetUserName(string name)
        {
            SetUserId(name);
            this.userName = name;
        }

        public async void SetUserId(string name)
        {
          this.userId = await _service.GetUserIdByNameAsync(name);
            ShowUserFiles();
           

        }



        public Form1()
        {
            InitializeComponent();
            //hide upload button
            uploadButton.Enabled = false;
            //hide log out button
            logOutButton.Enabled = false;

            buttonUpdate.Enabled = false;
            // I am using the current working directory of the application and folder for users files\\Hosting
            hostingPath = Directory.GetCurrentDirectory() + "\\Hosting";

            indexesOfChangedRows = new List<int>();
            
        }

       

        public void ShowStatus(string message)
        {
           statusLabel.Text = message;
        }

        public void ShowProgress(Label label ,string message)
        {
            label.Text = message;
        }

        // Log In button
        private async  void logInButton_Click(object sender, EventArgs e)
        {
            
            ShowStatus("Checking...");
            SetStatus( await _service.LogInAsync(inputName.Text, inputPassword.Text));   
                    
           

            if (isAuthorised)
            {
                SetUserName(inputName.Text);
                ShowStatus("Hello " + this.userName);
              
            }
            else
            {
                ShowStatus("Current user does not exist...");
                dataGridView1.DataSource = null;
            }

        }
        
        private async void LoadFile()
        {
            string path = null;
            string fileName = null;
            string newFilePath = null;
            OpenFileDialog openFile = new OpenFileDialog();

          //  openFile.Filter = "Text documents (*.txt)|*.txt";
            openFile.ShowDialog();

            if (openFile.FileNames.Length > 0)
            {
                foreach (string filename in openFile.FileNames)
                {
                     path = filename;                   
                }
            }
           
            fileName = openFile.SafeFileName;
            ShowProgress(progressLabel,"Copying started...");
            await _service.CopyFileToFolderAsync(path, fileName,this.hostingPath, this.userName);
           
            ShowProgress(progressLabel,"Copied");
            newFilePath = hostingPath + "\\" + this.userName + "\\" + fileName;
            await _service.AddFileAsync(this.userId, fileName, "", newFilePath);
            ShowUserFiles();


        }

        // Upload button
        private void uploadButton_Click(object sender, EventArgs e)
        {
            LoadFile();
           
        }

        // Register label
        private void regiserLabel_Click(object sender, EventArgs e)
        {
            
            var registerForm = new FormRegistration(this);
            registerForm.Show();
        }

        // Log out button click
        private void logOutButton_Click(object sender, EventArgs e)
        {
           
            ShowStatus("Status");
            SetStatus(false);
            ResetInputs();
            ShowProgress(progressLabel , "Progress:");
            dataGridView1.DataSource = null;
        }

        private void ResetInputs()
        {
            inputName.Text = "";
            inputPassword.Text = "";
            
        }

        private async void ShowUserFiles()
        {

            dataGridView1.DataSource = await _service.GetUserFilesByUserIdAsync(this.userId);
            dataGridView1.Columns["Id"].Visible = false;
            dataGridView1.AutoResizeColumns();

        }

        private async void buttonUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.EndEdit();


                UserFilesDTO fileInfo = new UserFilesDTO();

                // use reflection
                PropertyInfo propertyInfo;

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {

                    foreach (int changedIndex in indexesOfChangedRows)
                    {
                        if (row.Index == changedIndex)
                        {
                            try
                            {



                                foreach (DataGridViewCell cell in row.Cells)
                                {



                                    string value = cell.Value.ToString();
                                    string columnName = dataGridView1.Columns[cell.ColumnIndex].Name;


                                    //auto mapper
                                    propertyInfo = fileInfo.GetType().GetProperty(columnName);
                                    propertyInfo.SetValue(fileInfo, value, null);
                                }


                                // ShowProgress(progressLabel, );
                                await _service.UpdateFileInfoAsync(fileInfo, this.hostingPath);

                            }
                            catch
                            {
                                ShowProgress(progressLabel, "Name of file must be declared!!!");
                            }

                            }



                    }
                    
                    

                }

                ShowUserFiles();
            }
            catch (FaultException ex)
            {
                ShowProgress(progressLabel, ex.Message);
            }
            
           
        }


        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            InsertChangedRow(e.RowIndex);
            
        }
        private void InsertChangedRow(int index)
        {
            this.indexesOfChangedRows.Add(index);
        }

    }
}
