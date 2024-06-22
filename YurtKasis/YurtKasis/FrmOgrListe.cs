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
    public partial class FrmOgrListe : Form
    {
        public FrmOgrListe()
        {
            InitializeComponent();
        }
        SqlBaglantim bgl = new SqlBaglantim();

        private void FrmOgrListe_Load(object sender, EventArgs e)
        {
            // TODO: Bu kod satırı 'yurtOtomasyonDataSet6.Ogrenci' tablosuna veri yükler. Bunu gerektiği şekilde taşıyabilir, veya kaldırabilirsiniz.
            this.ogrenciTableAdapter.Fill(this.yurtOtomasyonDataSet6.Ogrenci);

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        int secilen;
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            secilen = dataGridView1.SelectedCells[0].RowIndex;
            FrmOgrDuzenle fr = new FrmOgrDuzenle(); 
            fr.id = dataGridView1.Rows[secilen].Cells[0].Value.ToString();
            fr.ad = dataGridView1.Rows[secilen].Cells[1].Value.ToString();
            fr.soyad = dataGridView1.Rows[secilen].Cells[2].Value.ToString();
            fr.tc = dataGridView1.Rows[secilen].Cells[3].Value.ToString();
            fr.telefon = dataGridView1.Rows[secilen].Cells[4].Value.ToString();
            fr.dogum = dataGridView1.Rows[secilen].Cells[5].Value.ToString();
            fr.bolum = dataGridView1.Rows[secilen].Cells[6].Value.ToString();
            fr.mail = dataGridView1.Rows[secilen].Cells[7].Value.ToString();
            fr.odano = dataGridView1.Rows[secilen].Cells[8].Value.ToString();
            fr.veliad = dataGridView1.Rows[secilen].Cells[9].Value.ToString();
            fr.velitel = dataGridView1.Rows[secilen].Cells[10].Value.ToString();
            fr.veliadres = dataGridView1.Rows[secilen].Cells[11].Value.ToString();



            fr.Show();
        }

        private void txtArama_TextChanged(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();

            string query;

            // Check if the txtArama.Text is null or empty
            if (string.IsNullOrEmpty(txtArama.Text))
            {
                // If txtArama.Text is null or empty, select all records
                query = "SELECT * FROM Ogrenci";
            }
            else
            {
                // If txtArama.Text is not null or empty, select records where OgrAd matches
                query = "SELECT * FROM Ogrenci WHERE OgrTC LIKE @OgrTC";
            }

            // Create a SqlDataAdapter with the query and connection
            SqlDataAdapter komut2 = new SqlDataAdapter(query, bgl.baglanti());

            if (!string.IsNullOrEmpty(txtArama.Text))
            {
                // If using a parameterized query, add the parameter to prevent SQL injection
                komut2.SelectCommand.Parameters.AddWithValue("@OgrTC", "%" + txtArama.Text + "%");
            }

            // Fill the DataTable with the results of the query
            komut2.Fill(dt);

            // Set the DataSource of the DataGridView to the DataTable
            dataGridView1.DataSource = dt;

            // Close the connection (assuming bgl.baglanti() opens a new connection)
            bgl.baglanti().Close();
        }
    }
}
