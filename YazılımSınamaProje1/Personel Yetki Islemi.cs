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

namespace YazılımSınamaProje1
{
    public partial class Personel_Yetki_Islemi : Form
    {
        public Personel_Yetki_Islemi()
        {
            InitializeComponent();
        }
        SqlConnection baglanti = new SqlConnection("Data Source=DESKTOP-OPJP9FR\\ADMIN_PELIN;Initial Catalog=DemirbaşTakip;Integrated Security=True");
    
        private void btnPersonelUzerindeDemirbasArama_Click(object sender, EventArgs e)
        {

            if (baglanti.State == System.Data.ConnectionState.Closed)
            {
                if (txtPersonelAramaAd.Text == "" && txtPersonelAramaSoyad.Text == "")
                {
                    MessageBox.Show("Kullanici Adı veya şifre boş geçilemez!!!");
                }
                else
                {
                    baglanti.Open();
                    string PersonelAdı = txtPersonelAramaAd.Text;
                    string PersonelSoyad = txtPersonelAramaSoyad.Text;
                    SqlCommand komut = new SqlCommand("SELECT d.demirbasAd  , d.demirbasTuruID ,d.adet ,d.fiyat ,d.satınAlimTarihi  FROM  demirbaslar d " +
                    "INNER JOIN odaDemirbaslar od ON d.demirbasID = od.demirbasID INNER JOIN odalar o ON " +
                    "o.odaID=od.odaID INNER JOIN personeller p ON p.personelID=o.personelID WHERE personelAd='" + txtPersonelAramaAd.Text + "'AND personelSoyad='" + txtPersonelAramaSoyad.Text + "'", baglanti);
                    SqlDataAdapter adapter = new SqlDataAdapter(komut);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    //SqlDataReader reader = komut.ExecuteReader();
                    dtgPersonelarama.DataSource = ds.Tables[0];
                    baglanti.Close();
                }
            }
        }

        private void btnPersonelOdaArama_Click(object sender, EventArgs e)
        {

            if (baglanti.State == System.Data.ConnectionState.Closed)
            {
                if (txtPerOdaAdi.Text == "")
                {
                    MessageBox.Show("Boş geçilemez!!!");
                }
                else
                {
                    baglanti.Open();
                    SqlCommand komut = new SqlCommand("SELECT d.demirbasAd  , d.demirbasTuruID ,d.adet ,d.fiyat ,d.satınAlimTarihi  FROM  demirbaslar d " +
                    "INNER JOIN odaDemirbaslar od ON d.demirbasID = od.demirbasID INNER JOIN odalar o ON  o.odaID=od.odaID WHERE odaAd='" + txtPerOdaAdi.Text + "'", baglanti);
                    SqlDataAdapter adapter = new SqlDataAdapter(komut);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    //SqlDataReader reader = komut.ExecuteReader();
                    dtgPersonelOdaArama.DataSource = ds.Tables[0];
                    baglanti.Close();
                }
            }
        }
        private void btngoster_Click(object sender, EventArgs e)
        {
            baglanti.Open();
            SqlCommand commend = new SqlCommand("Select * From odalar", baglanti);
            SqlDataAdapter sda = new SqlDataAdapter(commend);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            dtgPersonelOdaArama.DataSource = dt;
            baglanti.Close();
        }
    }
}
