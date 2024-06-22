using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YurtKasis
{
    public partial class FrmOgrKayit : Form
    {
        public FrmOgrKayit()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        SqlBaglantim bgl = new SqlBaglantim();
        private void Form1_Load (object sender, EventArgs e)
        {
            
            //Bolumleri Listeleme Komutu
            
           
            
            SqlCommand Komut = new SqlCommand("Select BolumAd From Bolumler", bgl.baglanti());

            SqlDataReader oku = Komut.ExecuteReader();

            while (oku.Read())
            {
                cmbxBolum.Items.Add(oku[0].ToString());
            }
            bgl.baglanti().Close();

            //Boş odaları listeleme komutu

           
           
            SqlCommand Komut2 = new SqlCommand("Select OdaNo From Odalar where OdaKapasite != OdaAktif", bgl.baglanti());
            
            SqlDataReader oku2 = Komut2.ExecuteReader();
            
            while(oku2.Read())
            {
                cmbOdaNo.Items.Add(oku2[0].ToString());
            }
            bgl.baglanti().Close();




        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            
            SqlCommand KomutKaydet = new SqlCommand("insert into Ogrenci(OgrAd, OgrSoyad, OgrTC, OgrTelefon, OgrDogum, OgrBolum, OgrMail, OgrOdaNo, OgrVeliAdSoyad, OgrVeliTelefon, OgrVeliAdres) VALUES (@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11)", bgl.baglanti());
           
            KomutKaydet.Parameters.AddWithValue("@p1", txtOgrnciAd.Text);
            KomutKaydet.Parameters.AddWithValue("@p2", txtOgrnciSoyad.Text);
            KomutKaydet.Parameters.AddWithValue("@p3", msktxtTC.Text);
            KomutKaydet.Parameters.AddWithValue("@p4", mskOgrTlf.Text);
            KomutKaydet.Parameters.AddWithValue("@p5", mskDgmTrh.Text);
            KomutKaydet.Parameters.AddWithValue("@p6", cmbxBolum.Text);
            KomutKaydet.Parameters.AddWithValue("@p7", txtMail.Text);
            KomutKaydet.Parameters.AddWithValue("@p8", cmbOdaNo.Text);
            KomutKaydet.Parameters.AddWithValue("@p9", txtVeliAdSoyad.Text);
            KomutKaydet.Parameters.AddWithValue("@p10", mskVeliTlf.Text);
            KomutKaydet.Parameters.AddWithValue("@p11", rchtxtAdres.Text);
            KomutKaydet.ExecuteNonQuery();


            SqlCommand komut3 = new SqlCommand("Select Ogrid from Ogrenci", bgl.baglanti());
            SqlDataReader oku3 = komut3.ExecuteReader();
            while (oku3.Read())
            {
                label12.Text = oku3[0].ToString();
            }
            bgl.baglanti().Close();

            MessageBox.Show("Öğrenci Eklendi");

            // Öğrenci borç alanı oluşturma

            SqlCommand KomutKaydet4 = new SqlCommand("insert into Borclar(Ogrid, OgrAd, OgrSoyad) VALUES (@b1, @b2, @b3)", bgl.baglanti());
            KomutKaydet4.Parameters.AddWithValue("@b1", label12.Text);
            KomutKaydet4.Parameters.AddWithValue("@b2", txtOgrnciAd.Text);
            KomutKaydet4.Parameters.AddWithValue("@b3", txtOgrnciSoyad.Text);
            KomutKaydet4.ExecuteNonQuery();
            bgl.baglanti().Close();

            // Öğrenci oda kontenjanı arttırma

            SqlCommand komutoda = new SqlCommand("update Odalar set OdaAktif=OdaAktif+1 where OdaNo=@o1", bgl.baglanti());
            komutoda.Parameters.AddWithValue("@o1", cmbOdaNo.Text);
            komutoda.ExecuteNonQuery();
            bgl.baglanti().Close();

        }
    }
}



//Data Source=GENERANB18\SQLEXPRESS;Initial Catalog=YurtOtomasyon;Integrated Security=True