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
    public partial class FrmGiderGuncelle : Form
    {
        public FrmGiderGuncelle()
        {
            InitializeComponent();
        }

        SqlBaglantim bgl= new SqlBaglantim();

        public string elektrik, su, dogalgaz, internet, gida, personel, diger, id;

        private void txtGiderId_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            try
            {
                SqlCommand komut = new SqlCommand("update Giderler set Elektrik=@p2, Su=@p3, Dogalgaz=@p4, internet=@p5, Gida=@p6, Personel=@p7, Diger=@p8 where Odemeid=@p1", bgl.baglanti());
                
                komut.Parameters.AddWithValue("@p1", txtGiderId.Text);
                komut.Parameters.AddWithValue("@p2", txtElektrik.Text);
                komut.Parameters.AddWithValue("@p3", txtSu.Text);
                komut.Parameters.AddWithValue("@p4", txtDglgaz.Text);
                komut.Parameters.AddWithValue("@p5", txtInt.Text);
                komut.Parameters.AddWithValue("@p6", txtGida.Text);
                komut.Parameters.AddWithValue("@p7", txtPersonel.Text);
                komut.Parameters.AddWithValue("@p8", txtDiger.Text);
                komut.ExecuteNonQuery();
                bgl.baglanti().Close();
                MessageBox.Show("Kayıt güncellendi");
            }
            catch (Exception)
            {

                MessageBox.Show("Hata,yeniden deneyin");
            }

        }

        private void FrmGiderGuncelle_Load(object sender, EventArgs e)
        {
            txtElektrik.Text = elektrik;
            txtSu.Text = su;
            txtDglgaz.Text = dogalgaz;
            txtInt.Text = internet;
            txtGida.Text = gida;
            txtPersonel.Text = personel;
            txtDiger.Text = diger;
            txtGiderId.Text = id; 
        }
    }
}
