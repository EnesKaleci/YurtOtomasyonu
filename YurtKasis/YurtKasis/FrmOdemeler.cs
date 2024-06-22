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
    public partial class FrmOdemeler : Form
    {
        public FrmOdemeler()
        {
            InitializeComponent();
        }

       SqlBaglantim bgl = new SqlBaglantim();
        private void FrmOdemeler_Load(object sender, EventArgs e)
        {
            // TODO: Bu kod satırı 'yurtOtomasyonDataSet5.Borclar' tablosuna veri yükler. Bunu gerektiği şekilde taşıyabilir, veya kaldırabilirsiniz.
            this.borclarTableAdapter3.Fill(this.yurtOtomasyonDataSet5.Borclar);

            // TODO: Bu kod satırı 'yurtOtomasyonDataSet3.Borclar' tablosuna veri yükler. Bunu gerektiği şekilde taşıyabilir, veya kaldırabilirsiniz.
            this.borclarTableAdapter1.Fill(this.yurtOtomasyonDataSet3.Borclar);


        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int secilen;
            string id, ad, soyad, kalan;
            secilen = dataGridView1.SelectedCells[0].RowIndex;
            id = dataGridView1.Rows[secilen].Cells[0].Value.ToString();
            ad = dataGridView1.Rows[secilen].Cells[1].Value.ToString();
            soyad = dataGridView1.Rows[secilen].Cells[2].Value.ToString();
            kalan = dataGridView1.Rows[secilen].Cells[3].Value.ToString();

            txtOgrAd.Text = ad; 
            txtOgrSoyad.Text = soyad;
            txtKalanBorc.Text = kalan;
            txtOgrId.Text = id;
        }

        private void btnOdemeAl_Click(object sender, EventArgs e)
        {
            // Ödenen tutarı alma
            int odenen, kalan, yeniborc;
            odenen = Convert.ToInt32(txtOdenen.Text);
            kalan = Convert.ToInt32(txtKalanBorc.Text);
            yeniborc = kalan - odenen;
            txtKalanBorc.Text = yeniborc.ToString();

            // Sql tablosunu güncelleme
         
                     
            SqlCommand komut = new SqlCommand("update Borclar set OgrKalanBorc=@p1 where Ogrid=@p2", bgl.baglanti());
            komut.Parameters.AddWithValue("@p2", txtOgrId.Text);
            komut.Parameters.AddWithValue("@p1",  txtKalanBorc.Text);
            komut.ExecuteNonQuery();   
            bgl.baglanti().Close();

            this.borclarTableAdapter3.Fill(this.yurtOtomasyonDataSet5.Borclar);

            // Kasa tablosuna ekleme ya

            SqlCommand komut3 = new SqlCommand("insert into Kasa (OdemeAy,OdemeMiktar) values (@k1,@k2)", bgl.baglanti());
            komut3.Parameters.AddWithValue("@k1", txtOdenenAy.Text);
            komut3.Parameters.AddWithValue("@k2", txtOdenen.Text);
            komut3.ExecuteNonQuery();
            bgl.baglanti().Close();


        }

        private void txtArama_TextChanged(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();

            string query;

            // Check if the txtArama.Text is null or empty
            if (string.IsNullOrEmpty(txtArama.Text))
            {
                // If txtArama.Text is null or empty, select all records
                query = "SELECT * FROM Borclar";
            }
            else
            {
                // If txtArama.Text is not null or empty, select records where OgrAd matches
                query = "SELECT * FROM Borclar WHERE OgrSoyad LIKE @OgrSoyad";
            }

            // Create a SqlDataAdapter with the query and connection
            SqlDataAdapter komut2 = new SqlDataAdapter(query, bgl.baglanti());

            if (!string.IsNullOrEmpty(txtArama.Text))
            {
                // If using a parameterized query, add the parameter to prevent SQL injection
                komut2.SelectCommand.Parameters.AddWithValue("@OgrSoyad", "%" + txtArama.Text + "%");
            }

            // Fill the DataTable with the results of the query
            komut2.Fill(dt);

            // Set the DataSource of the DataGridView to the DataTable
            dataGridView1.DataSource = dt;

            // Close the connection (assuming bgl.baglanti() opens a new connection)
            bgl.baglanti().Close();

            //DataTable dt = new DataTable();

            //SqlDataAdapter komut2 = new SqlDataAdapter("select * from Borclar where OgrAd like '"+txtArama.Text+"'", bgl.baglanti());

            //komut2.Fill(dt);
            //dataGridView1.DataSource = dt;
            //bgl.baglanti().Close();


            //if (txtArama.Text==null)
            //{
            //    this.borclarTableAdapter3.Fill(this.yurtOtomasyonDataSet5.Borclar);
            //}




        }
    }
}
