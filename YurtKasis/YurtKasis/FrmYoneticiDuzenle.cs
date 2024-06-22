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
    public partial class FrmYoneticiDuzenle : Form
    {
        public FrmYoneticiDuzenle()
        {
            InitializeComponent();
        }

        SqlBaglantim bgl = new SqlBaglantim();
        private void FrmYoneticiDuzenle_Load(object sender, EventArgs e)
        {
            // TODO: Bu kod satırı 'yurtOtomasyonDataSet8.Admin' tablosuna veri yükler. Bunu gerektiği şekilde taşıyabilir, veya kaldırabilirsiniz.
            this.adminTableAdapter.Fill(this.yurtOtomasyonDataSet8.Admin);

        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            SqlCommand komut = new SqlCommand("insert into Admin(YoneticiAd, YoneticiSifre) values (@p1, @p2)", bgl.baglanti());
            komut.Parameters.AddWithValue("@p1", txtKullaniciAd.Text);
            komut.Parameters.AddWithValue("@p2", txtSifre.Text);
            komut.ExecuteNonQuery();
            bgl.baglanti().Close();

            MessageBox.Show("Yönetici Eklendi!");
            this.adminTableAdapter.Fill(this.yurtOtomasyonDataSet8.Admin);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int secilen;

            secilen = dataGridView1.SelectedCells[0].RowIndex;
            string ad, sifre, id;
            id= dataGridView1.Rows[secilen].Cells[0].Value.ToString();
            ad = dataGridView1.Rows[secilen].Cells[1].Value.ToString();
            sifre = dataGridView1.Rows[secilen].Cells[2].Value.ToString();  

            txtKullaniciAd.Text = ad;   
            txtSifre.Text = sifre; 
            txtYoneticiId.Text = id;
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            SqlCommand cmd = new SqlCommand("delete from Admin where Yoneticiid=@p1", bgl.baglanti());
            cmd.Parameters.AddWithValue("@p1", txtYoneticiId.Text);
            cmd.ExecuteNonQuery();
            bgl.baglanti().Close();

            MessageBox.Show("Silme işlemi başarılı!");
            this.adminTableAdapter.Fill(this.yurtOtomasyonDataSet8.Admin);
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            SqlCommand cmd = new SqlCommand("update Admin set YoneticiAd=@p1, YoneticiSifre=@p2 where Yoneticiid=@p3", bgl.baglanti());
            cmd.Parameters.AddWithValue("@p1", txtKullaniciAd.Text);
            cmd.Parameters.AddWithValue("@p2", txtSifre.Text);
            cmd.Parameters.AddWithValue("@p3", txtYoneticiId.Text);
            cmd.ExecuteNonQuery();
            bgl.baglanti().Close();

            MessageBox.Show("Yönetici Güncellendi!");
            this.adminTableAdapter.Fill(this.yurtOtomasyonDataSet8.Admin);
        }
    }
}
