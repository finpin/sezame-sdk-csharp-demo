using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sezame;
using Sezame.Authentication;
using ZXing;
using ZXing.Common;
using Newtonsoft.Json;
using System.IO;
using System.Drawing.Imaging;
using System.Security.Cryptography.X509Certificates;


namespace SezameExample
{
    public partial class MainForm : Form
    {
        protected SezameManager _manager;
        protected LinkForm linkForm;

        public MainForm()
        {
            InitializeComponent();
            _manager = new SezameManager();

            email.Text = _manager.email;
            applicationName.Text = "Sezame Dotnet Example";
            status.Text = _manager.status;
            sharedsecret.Text = _manager.sharedsecret;
            clientcode.Text = _manager.clientcode;
            linkForm = new LinkForm();
        }

        private void btnRegisterClick(object sender, EventArgs e)
        {
            var myCallback = new SezameRegisterCallbackType(registerCallback);
            _manager.register(email.Text, applicationName.Text, myCallback);
        }

        protected void registerCallback(string clientcode, string sharedsecret)
        {
            this.clientcode.Text = clientcode;
            this.sharedsecret.Text = sharedsecret;
        }

        private void btnSign_Click(object sender, EventArgs e)
        {
            var csr = _manager.buildCsr();
            var myCallback = new SezameSignCallbackType(signCallback);
            MessageBox.Show(csr);
            _manager.sign(csr, myCallback);
        }

        protected void signCallback(X509Certificate2 certificate)
        {
            status.Text = _manager.status;
            MessageBox.Show(certificate.ToString());
        }

        private void btnLink_Click(object sender, EventArgs e)
        {
            if (username.Text.Length == 0)
            {
                MessageBox.Show("Please enter a username!");
                return;
            }
            var myCallback = new SezameLinkCallbackType(linkCallback);
            _manager.link(username.Text, myCallback);
        }

        protected void linkCallback(string id, string clientcode)
        {
            var data = JsonConvert.SerializeObject(new
            {
                id = id,
                username = username.Text,
                client = clientcode
            });

            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions { Width = 500, Height = 500, Margin = 10 }
            };

            using (var bitmap = writer.Write(data))
            {
                using (var stream = new MemoryStream())
                {
                    bitmap.Save(stream, ImageFormat.Png);
                    var img = Image.FromStream(stream);
                    linkForm.ShowPicture(img);
                }
            }

        }

        private void btnAuth_Click(object sender, EventArgs e)
        {
            if (authuser.Text.Length == 0)
            {
                MessageBox.Show("Please enter a username!");
                return;
            }

            var myCallback = new SezameAuthCallbackType(authCallback);
            _manager.auth(authuser.Text, "Dotnet example UI", myCallback);
        }

        protected void authCallback(SezameAuthenticationResultKey status)
        {
            switch (status)
            {
                case SezameAuthenticationResultKey.NotPaired:
                    MessageBox.Show("This user ist not paired!");
                    break;

                case SezameAuthenticationResultKey.Timedout:
                    MessageBox.Show("User not responded in time!");
                    break;

                case SezameAuthenticationResultKey.Denied:
                    MessageBox.Show("User has denied the request!");
                    break;

                case SezameAuthenticationResultKey.Authenticated:
                    MessageBox.Show("Authentication succeeded!");
                    break;

            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_manager.status != "ready")
            {
                MessageBox.Show("Please register and sign first!");
                return;
            }
            var myCallback = new SezameCancelCallbackType(cancelCallback);
            _manager.cancel(myCallback);
        }

        protected void cancelCallback()
        {
            email.Text = _manager.email;
            applicationName.Text = "Sezame Dotnet Example";
            status.Text = _manager.status;
            sharedsecret.Text = _manager.sharedsecret;
            clientcode.Text = _manager.clientcode;
        }
    }
}
