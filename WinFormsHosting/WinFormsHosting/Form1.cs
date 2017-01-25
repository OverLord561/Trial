using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using WinFormsHosting.ServiceReference1;




namespace WinFormsHosting
{
    
    public partial class Form1 : Form
    {
        //Service1Client _service = new Service1Client();
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
            buttonDelete.Enabled = status;
        }

        public void SetUserName(string name)
        {
            SetUserId(name);
            this.userName = name;
            inputName.Text = "";
            inputPassword.Text = "";
           
        }

        public async void SetUserId(string name)
        {
           
                string serviceUrl = string.Format("http://localhost:53277/Service1.svc/UserIdByName/{0}", name);
                this.userId = Convert.ToInt32(GetValueFromGetResponse(serviceUrl));
            


            
              ShowUserFiles();


        }



        public Form1()
        {
            InitializeComponent();
            //hide upload button
            uploadButton.Enabled = false;
            //hide log out button
            logOutButton.Enabled = false;
            buttonDelete.Enabled = false;
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
        private   void logInButton_Click(object sender, EventArgs e)
        {

           
                string serviceUrl = string.Format("http://localhost:53277/Service1.svc/User/LogIn/{0}/{1}", inputName.Text, inputPassword.Text);
                bool response = false;
                response = Convert.ToBoolean(GetValueFromGetResponse(serviceUrl));
              

                ShowStatus("Checking...");
                SetStatus(response);
            
               
            
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
            else
            {
                return;
            }
           
            fileName = openFile.SafeFileName;
            ShowProgress(progressLabel,"Copying started...");

            string serviceUrl = "http://localhost:53277/Service1.svc/File/Copy";
            JObject json = new JObject();

            json.Add("sourceFile", path);
            json.Add("fileName", fileName);
            json.Add("hostingPath", this.hostingPath);
            json.Add("userName", this.userName);


            GetValueFromPostResponse(serviceUrl, json);

            
            ShowProgress(progressLabel,"Copied");
            newFilePath = hostingPath + "\\" + this.userName + "\\" + fileName;

            JObject json2 = new JObject();
            string serviceUrlToAddIntoDB = "http://localhost:53277/Service1.svc/File/Add";
            json2.Add("userId", this.userId);
            json2.Add("name", fileName);
            json2.Add("description", "");
            json2.Add("path", newFilePath);

            GetValueFromPostResponse(serviceUrlToAddIntoDB, json2);
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
            dataGridView1.DataSource = null;
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
            inputDelete.Text = "";
            
        }

        private async void ShowUserFiles()
        {
            string response;
            
                string serviceUrl = string.Format("http://localhost:53277/Service1.svc/UserFilesByUserId/{0}", this.userId);
      
                response = GetValueFromGetResponse(serviceUrl );
          
            
              

            dataGridView1.DataSource = JsonConvert.DeserializeObject<List<UserFilesDTO>>(response);
            dataGridView1.Columns["Id"].Visible = false;
            dataGridView1.AutoResizeColumns();

        }

        private async void buttonUpdate_Click(object sender, EventArgs e)
        {
            bool isUpdated = false;
           
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

                            string serviceUrl = "http://localhost:53277/Service1.svc/FileInfo/Update";

                        JObject json = new JObject();

                        json.Add("Id", fileInfo.Id);
                        json.Add("Name", fileInfo.Name);
                        json.Add("Description", fileInfo.Description);
                        json.Add("hostingPath", this.hostingPath);

                            JObject file = new JObject();
                            file.Add("Name", fileInfo.Name);
                            file.Add("Description", fileInfo.Description);
                            file.Add("Id", fileInfo.Id);
                            json.Add("info",file);

                            string response = GetValueFromPostResponse(serviceUrl, json);


                      

                        isUpdated = JsonConvert.DeserializeObject<bool>(response);

                        if (!isUpdated)
                                {
                                    ShowProgress(progressLabel, "Incorrect file name");
                                }
                                else
                                {
                                    ShowProgress(progressLabel, "Data was updated");
                                }

                    }
                            catch
                    {
                        ShowProgress(progressLabel, "Name of file must be declared!!!");
                    }

                }
                        
                    }
                    
                ShowUserFiles();
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

        private async void buttonDelete_Click(object sender, EventArgs e)
        {
            string fileName =  inputDelete.Text;


            string serviceUrl = string.Format("http://localhost:53277/Service1.svc/File/Delete");
            JObject json = new JObject();
            json.Add("fileName", fileName);
            json.Add("userName", this.userName);

            string response = GetValueFromPostResponse(serviceUrl,json);
            string message =  JsonConvert.DeserializeObject<string>(response);

            ShowProgress(progressLabel, message);
            ShowUserFiles();
            inputDelete.Text = "";

           
        }
        public string GetValueFromGetResponse(string serviceUrl)
        {
            
            using (WebClient proxy = new WebClient())
            {
                proxy.Encoding = System.Text.Encoding.UTF8;
                byte[] _data = proxy.DownloadData(serviceUrl);
                string vvv = proxy.DownloadString(serviceUrl);
                Stream _mem = new MemoryStream(_data);
                var reader = new StreamReader(_mem);
                var result = reader.ReadToEndAsync();
                return result.Result;
            }
             
        }

        public string GetValueFromPostResponse(string serviceUrl, JObject json)
        {
            using (WebClient proxy = new WebClient())
            {
               
                proxy.Headers[HttpRequestHeader.ContentType] = "application/json";
                //charset=utf-8"
                proxy.Encoding = System.Text.Encoding.UTF8;
                string response = proxy.UploadString(serviceUrl, json.ToString(Newtonsoft.Json.Formatting.None, null));
                return response;
            }
               
        }
    }
}
