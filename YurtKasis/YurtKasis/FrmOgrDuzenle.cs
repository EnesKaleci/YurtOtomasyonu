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
    public partial class FrmOgrDuzenle : Form
    {
        public FrmOgrDuzenle()
        {
            InitializeComponent();
        }

        SqlBaglantim bgl = new SqlBaglantim();

        private void rchtxtAdres_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            try
            {
                SqlCommand komut = new SqlCommand("update Ogrenci set OgrAd= @p2, OgrSoyad= @p3, OgrTC= @p4, OgrTelefon= @p5, OgrDogum= @p6, OgrBolum= @p7, OgrMail= @p8, OgrOdaNo= @p9, OgrVeliAdSoyad= @p10, OgrVeliTelefon= @p11, OgrVeliAdres= @p12 where Ogrid= @p1", bgl.baglanti());


                komut.Parameters.AddWithValue("@p1", txtOgrId.Text);
                komut.Parameters.AddWithValue("@p2", txtOgrnciAd.Text);
                komut.Parameters.AddWithValue("@p3", txtOgrnciSoyad.Text);
                komut.Parameters.AddWithValue("@p4", msktxtTC.Text);
                komut.Parameters.AddWithValue("@p5", mskOgrTlf.Text);
                komut.Parameters.AddWithValue("@p6", mskDgmTrh.Text);
                komut.Parameters.AddWithValue("@p7", cmbxBolum.Text);
                komut.Parameters.AddWithValue("@p8", txtMail.Text);
                komut.Parameters.AddWithValue("@p9", cmbOdaNo.Text);
                komut.Parameters.AddWithValue("@p10", txtVeliAdSoyad.Text);
                komut.Parameters.AddWithValue("@p11", mskVeliTlf.Text);
                komut.Parameters.AddWithValue("@p12", rchtxtAdres.Text);
                komut.ExecuteNonQuery();
                bgl.baglanti().Close();
                MessageBox.Show("Kayıt güncellendi");
            }
            catch (Exception)
            {

                MessageBox.Show("Hata, yeniden deneyin");
            }










        }
        

        private void btnSil_Click(object sender, EventArgs e)
        {
            SqlCommand komutsil = new SqlCommand("delete from Ogrenci where Ogrid=@id", bgl.baglanti());
            komutsil.Parameters.AddWithValue("id", txtOgrId.Text);
            komutsil.ExecuteNonQuery();
            bgl.baglanti().Close();
            MessageBox.Show("Kayıt silindi");

            //Oda aktifi arttırma

            SqlCommand komutarttır =new SqlCommand("update Odalar set OdaAktif=OdaAktif-1 where OdaNo=@o1", bgl.baglanti());
            komutarttır.Parameters.AddWithValue("@o1", cmbOdaNo.Text);
            komutarttır.ExecuteNonQuery();
            bgl.baglanti().Close();

        }

        public string id, ad, soyad, tc, telefon, dogum, bolum;
        public string mail, odano, veliad, velitel, veliadres;

        private void FrmOgrDuzenle_Load(object sender, EventArgs e)
        {
            // TODO: Bu kod satırı 'yurtOtomasyonDataSet10.Ogrenci' tablosuna veri yükler. Bunu gerektiği şekilde taşıyabilir, veya kaldırabilirsiniz.
            this.ogrenciTableAdapter.Fill(this.yurtOtomasyonDataSet10.Ogrenci);
            txtOgrId.Text = id;
            txtOgrnciAd.Text = ad;
            txtOgrnciSoyad.Text = soyad;
            msktxtTC.Text = tc;
            mskOgrTlf.Text = telefon;
            mskDgmTrh.Text = dogum;
            cmbxBolum.Text = bolum;
            txtMail.Text = mail;
            cmbOdaNo.Text = odano;
            txtVeliAdSoyad.Text = veliad;
            mskVeliTlf.Text = velitel;
            rchtxtAdres.Text = veliadres;


        }

        private void label13_Click(object sender, EventArgs e)
        {

        }
    }
}


