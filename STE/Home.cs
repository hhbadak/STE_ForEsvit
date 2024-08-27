using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STE
{
    public partial class Home : Form
    {
        public static Employee LoginUser;
        DataModel dm = new DataModel();
        public Home()
        {
            InitializeComponent();
        }

        private void Home_Load(object sender, EventArgs e)
        {
            Login frm = new Login();
            frm.ShowDialog();
            Employee model = Helpers.GirisYapanKullanici;
        }

    }
}
