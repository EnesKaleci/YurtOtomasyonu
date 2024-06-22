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
    public partial class FrmPersonel : Form
    {
        public FrmPersonel()
        {
            InitializeComponent();
        }
        SqlBaglantim bgl = new SqlBaglantim();
        private void FrmPersonel_Load(object sender, EventArgs e)
        {
            // TODO: Bu kod satırı 'yurtOtomasyonDataSet9.Personeller' tablosuna veri yükler. Bunu gerektiği şekilde taşıyabilir, veya kaldırabilirsiniz.
            this.personellerTableAdapter.Fill(this.yurtOtomasyonDataSet9.Personeller);

        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            SqlCommand komut = new SqlCommand("insert into Personeller(PersonelAdSoyad, PersonelDepartman) values (@p1, @p2)", bgl.baglanti());
            komut.Parameters.AddWithValue("@p1", txtPersonelAd.Text);
            komut.Parameters.AddWithValue("@p2", txtPersonelGorev.Text);
            komut.ExecuteNonQuery();
            bgl.baglanti().Close();

            MessageBox.Show("Personel Eklendi!");
            this.personellerTableAdapter.Fill(this.yurtOtomasyonDataSet9.Personeller);
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            SqlCommand cmd = new SqlCommand("delete from Personeller where Personelid=@p1", bgl.baglanti());
            cmd.Parameters.AddWithValue("@p1", txtPersonelId.Text);
            cmd.ExecuteNonQuery();
            bgl.baglanti().Close();

            MessageBox.Show("Silme işlemi başarılı!");
            this.personellerTableAdapter.Fill(this.yurtOtomasyonDataSet9.Personeller);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int secilen;

            secilen = dataGridView1.SelectedCells[0].RowIndex;
            string ad, gorev, id;
            id = dataGridView1.Rows[secilen].Cells[0].Value.ToString();
            ad = dataGridView1.Rows[secilen].Cells[1].Value.ToString();
            gorev = dataGridView1.Rows[secilen].Cells[2].Value.ToString();

            txtPersonelAd.Text = ad;
            txtPersonelGorev.Text = gorev;
            txtPersonelId.Text = id;
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            SqlCommand cmd = new SqlCommand("update Personeller set PersonelAdSoyad=@p1, PersonelDepartman=@p2 where Personelid=@p3", bgl.baglanti());
            cmd.Parameters.AddWithValue("@p1", txtPersonelAd.Text);
            cmd.Parameters.AddWithValue("@p2", txtPersonelGorev.Text);
            cmd.Parameters.AddWithValue("@p3", txtPersonelId.Text);
            cmd.ExecuteNonQuery();
            bgl.baglanti().Close();

            MessageBox.Show("Yönetici Güncellendi!");
            this.personellerTableAdapter.Fill(this.yurtOtomasyonDataSet9.Personeller);
        }
    }
}
