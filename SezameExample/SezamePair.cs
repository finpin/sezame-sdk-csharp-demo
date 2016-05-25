using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SezameExample
{
    public partial class LinkForm : Form
    {
        public LinkForm()
        {
            InitializeComponent();
        }

        public void ShowPicture(Image img)
        {
            pictureBox.Image = img;
            Show();
        }
    }
}
