using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormsHosting.ServiceReference1;

namespace WinFormsHosting
{
    public partial class FormRegistration : Form
    {
        Service1Client _service = new Service1Client();
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

                del("Checking...");
                string userName = await _service.RegisterUserAsync(textBox1.Text, textBox2.Text);
                if (userName != null)
                {
                    setName(userName);
                    changeStatus(true);
                    del("Hello " + userName);

                }
                else
                {
                    del("Current user exists");
                }

                this.Close();
            }
            catch (System.ServiceModel.FaultException ex)
            {
                
                del(ex.Message);
            }
        

    }
    }
}
