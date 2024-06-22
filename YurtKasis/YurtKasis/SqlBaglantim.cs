using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace YurtKasis
{
    /// <summary>
    /// Sql bağlantısını class oluşturuldu ve bir fonksiyona atandı.
    /// </summary>
    public class SqlBaglantim
    {
        public SqlConnection baglanti()
        {
            SqlConnection baglan = new SqlConnection("Data Source=GENERANB18\\SQLEXPRESS;Initial Catalog=YurtOtomasyon;Integrated Security=True");
            baglan.Open();
            return baglan;
        }
            


    }
}
