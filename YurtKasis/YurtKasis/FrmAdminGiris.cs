using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace YurtKasis
{
    public partial class FrmAdminGiris : Form
    {
        public FrmAdminGiris()
        {
            InitializeComponent();
        }
        SqlBaglantim bgl  = new SqlBaglantim();
        private void btnGiris_Click(object sender, EventArgs e)

        {

            SqlCommand cmd = new SqlCommand("Select * from Admin where YoneticiAd=@p1 and YoneticiSifre=@p2", bgl.baglanti());

            cmd.Parameters.AddWithValue("@p1", txtKullaniciAd.Text);
            cmd.Parameters.AddWithValue("@p2", txtSifre.Text);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                FrmAna frm = new FrmAna();
                frm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Hatalı kullanıcı adı veya şifre!");
            }
           
            
            //if (txtKullaniciAd.Text == "admin34" && txtSifre.Text=="1234")
            //{
            //    FrmAna fr = new FrmAna();
            //    fr.Show();
            //}
            //else
            //{
            //    MessageBox.Show("Hatalı Giriş Yaptınız!");
            //}
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
