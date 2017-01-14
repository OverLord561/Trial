﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
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
        int count = 0;



        public void SetStatus(bool status)
        {
            
            isAuthorised = status;
            uploadButton.Enabled = status;
            logOutButton.Enabled = status;
            progressLabel.Enabled = status;
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
            // I am using the current working directory of the application and folder for users files\\Hosting
            hostingPath = Directory.GetCurrentDirectory() + "\\Hosting";
        }

       

        public void ShowStatus(string message)
        {
           statusLabel.Text = message;
        }

        public void ShowProgress(string message)
        {
            progressLabel.Text = message;
        }

        // Log In button
        private async  void logInButton_Click(object sender, EventArgs e)
        {
            
            ShowStatus("Checking...");
            SetStatus( await _service.LogInAsync(inputName.Text, inputPassword.Text));   
                    
            //label3.BackColor = Color.Red;
           

            if (isAuthorised)
            {
                SetUserName(inputName.Text);
                ShowStatus("Hello " + this.userName);
                

            }
            else
            {
                ShowStatus("Current user does not exist...");
            }

        }
        
        private async void LoadFile()
        {
            string path = null;
            string fileName = null;
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
            //////
            fileName = openFile.SafeFileName;
            ShowProgress("Copying started...");
            await _service.CopyFileToFolderAsync(path, fileName,this.hostingPath, this.userName);
           
            ShowProgress("Copied");
            await _service.AddFileAsync(this.userId, fileName, "", path);
            ShowUserFiles();


        }

        // Upload button
        private void uploadButton_Click(object sender, EventArgs e)
        {
            LoadFile();
            //label4.Text = LoadFile();
           // string filePath = LoadFile();
           

            //return LoadFile();
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

        }

        private async void buttonUpdate_Click(object sender, EventArgs e)
        {
            dataGridView1.EndEdit();

            bool cellWasChanged = false;

            
            UserFilesDTO fileInfo = new UserFilesDTO();

            // use reflection
            PropertyInfo propertyInfo; 

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Selected == true)
                    {
                        cellWasChanged = true;
                    }
                    var inedit =  cell.IsInEditMode;
                    string value = cell.Value.ToString();
                    string columnName = dataGridView1.Columns[cell.ColumnIndex].Name;

                    //mapper
                    propertyInfo = fileInfo.GetType().GetProperty(columnName);
                    propertyInfo.SetValue(fileInfo, value, null);
                }

                if (cellWasChanged)
                {
                    await _service.UpdateFileInfoAsync(fileInfo);
                    cellWasChanged = false;

                }
               

            }
            List<WinFormsHosting.ServiceReference1.UserFilesDTO> newInfo = new List<UserFilesDTO>();
            UserFilesDTO c = new UserFilesDTO();
            
           // foreach(UserFilesDTO file in dataGridView1.DataSource)

         //   await _service.UpdateFileInfoAsync((WinFormsHosting.ServiceReference1.UserFilesDTO)dataGridView1.DataSource);

        }


        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            count++;
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Reference")
            {
                //your code goes here
            }
        }

    }
}
