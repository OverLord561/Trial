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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormsHosting.ServiceReference1;

namespace WinFormsHosting
{
    public partial class FormRegistration : Form
    {
       
        Form1 baseForm;

        // baseForm - object of Basic(main) form
        public FormRegistration( Form1 baseForm)
        {
            this.baseForm = baseForm;
            InitializeComponent();
           
        }
        public delegate void delAuthorise(bool status);
        public delegate void delPassData(string text);
        public delegate void delSetName(string name);

        private async void button2_Click(object sender, EventArgs e)
        {
            delPassData del = new delPassData(baseForm.ShowStatus);
            delAuthorise changeStatus = new delAuthorise(baseForm.SetStatus);
            delSetName setName = new delSetName(baseForm.SetUserName);
            try
            {
                string userName;
                using (WebClient proxy = new WebClient())
                {
                    
                    string serviceUrl = "http://localhost:53277/Service1.svc/User/Register";                    
                    JObject json = new JObject();
                    json.Add("name", textBox1.Text);
                    json.Add("password", textBox2.Text);
                    proxy.Encoding = System.Text.Encoding.UTF8;
                    proxy.Headers[HttpRequestHeader.ContentType] = "application/json";
                    string response = proxy.UploadString(serviceUrl, json.ToString(Newtonsoft.Json.Formatting.None, null));

                    userName = JsonConvert.DeserializeObject<string>(response);

                }
            
                del("Checking...");
                 
              
                if (userName != null)
                {
                    setName(userName);
                    changeStatus(true);
                    del("Hello " + userName);

                }
                else
                {
                    changeStatus(false);
                    del("Current user exists");
                    
                }

                this.Close();
            }
            catch (System.ServiceModel.FaultException<CurrentUser> ex)
            {                
                del(ex.Message);
            }
        

    }
    }
}
