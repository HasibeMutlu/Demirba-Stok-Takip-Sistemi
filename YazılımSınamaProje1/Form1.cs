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
    public partial class frmDemirbasStokTakip : Form
    {
        SqlConnection baglanti = new SqlConnection("Data Source=DESKTOP-OPJP9FR\\ADMIN_PELIN;Initial Catalog=DemirbaşTakip;Integrated Security=True");
        SqlCommand kaydet;
        public static string adSoyad = "";
        public frmDemirbasStokTakip()
        {
            InitializeComponent();
        }

        //Kullanici Girişi Kontrol ediliyor
        private void btnKullaniciGiris_Click(object sender, EventArgs e)
        {
            //
            if (baglanti.State == System.Data.ConnectionState.Closed)
            {
                if (txtKullaniciAd.Text == "" && txtKullaniciSifre.Text == "")
                {
                    MessageBox.Show("Kullanici Adı veya şifre boş geçilemez!!!");
                }
                else
                {
                    baglanti.Open();
                    string kullaniciAd = txtKullaniciAd.Text;
                    string kullaniciSifre = txtKullaniciSifre.Text;
                    SqlCommand com = new SqlCommand("select * from kullanicilar where kullaniciAd=@kullaniciAd and kullaniciSifre=@kullaniciSifre", baglanti);

                    //Injections
                    com.Parameters.AddWithValue("kullaniciSifre", kullaniciSifre.ToString());
                    com.Parameters.AddWithValue("kullaniciAd", kullaniciAd.ToString());

                    adSoyad = kullaniciAd;
                    SqlDataReader reader = com.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            bool sec = (bool)reader[3];
                            if (sec == true)
                            {
                                MessageBox.Show(" Hoşgeldin Yetkili  " + kullaniciAd );
                                tbpStokIslemi.TabPages.Remove(tbpGiris);
                                tbpStokIslemi.TabPages.Add(tbpPersonelIslemleri);
                                tbpStokIslemi.TabPages.Add(tbpDemirbasIslemleri);
                                tbpStokIslemi.TabPages.Add(tbpStokIslemleri);
                                tbpStokIslemi.TabPages.Add(tbpOdaIslemleri);
                                tbpStokIslemi.TabPages.Add(tbpOdaUzerindeDemirbaslar);

                                tbpStokIslemi.SelectedIndex = 0;
                            }
                            else if (sec == false)
                            {
                                Personel_Yetki_Islemi pyi = new Personel_Yetki_Islemi();
                                pyi.Show();
                                MessageBox.Show(" Hoşgeldin Personel " + kullaniciAd);
                                this.Hide();
                            }
                            else { break; }
                        }

                    }
                    else
                    {
                        MessageBox.Show("Kullanıcı yok");
                    }
                    baglanti.Close();
                }

            }
        }
        //Kullanici giriş yapmadan önce sistemin tabpages görünürlüğünü gizlendi
        private void frmDemirbasStokTakip_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'demirbaşTakipDataSet.odaDemirbaslar' table. You can move, or remove it, as needed.
            this.odaDemirbaslarTableAdapter.Fill(this.demirbaşTakipDataSet.odaDemirbaslar);

            tbpStokIslemi.TabPages.Remove(tbpDemirbasIslemleri);
            tbpStokIslemi.TabPages.Remove(tbpOdaIslemleri);
            tbpStokIslemi.TabPages.Remove(tbpOdaUzerindeDemirbaslar);
            tbpStokIslemi.TabPages.Remove(tbpPersonelIslemleri);
            tbpStokIslemi.TabPages.Remove(tbpStokIslemleri);

            string query2 = "select * from demirbaslar";
            SqlCommand command2 = new SqlCommand(query2, baglanti);
            SqlDataAdapter adapter = new SqlDataAdapter(command2);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            dtgDemirbasEkle.DataSource = ds.Tables[0];
            baglanti.Close();

        }

        //Personellerin Ad ve Soyadlarını girerek Demirbaşlarını bulundu
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
        //Veritabanına form ekranından demirbaşEklendi
        private void btnDemirbasEkle_Click_1(object sender, EventArgs e)
        {
            int demirbasAdet = Convert.ToInt32(textAdet.Text);
            int demirbasFiyat = Convert.ToInt32(textFiyat.Text);
            string dt = Convert.ToDateTime(dtsatinAlmaTarihi.Value).ToString("yyyy-MM-dd");
            string demirbasTur = cmdDemirbasTuru.Text;
            string demirbasAd = txtdemirbasad.Text;
            string departman = cmbDepartman.Text;

            if (baglanti.State == System.Data.ConnectionState.Closed)
            {
                if (cmbDepartman.Text == "" || cmdDemirbasTuru.Text == "" || txtdemirbasad.Text == "" || dt == "" ||
                    demirbasAdet.ToString() == "" || demirbasFiyat.ToString() == "")
                {
                    MessageBox.Show("Alanlar boş geçilemez!!!");
                }
                else
                {
                    string query_demirbastur = "(SELECT demirbasTuruID from demirbasTurleri where demirbasTuruAd='" + demirbasTur + "')";
                    baglanti.Open();
                    string query = "INSERT INTO demirbaslar(demirbasAd,demirbasTuruID, adet, fiyat,satınAlimTarihi) " +
                       "Values('" + demirbasAd + "'," + query_demirbastur + ", '" + demirbasAdet + "','" + demirbasFiyat + "','" + dt + "')";
                    SqlCommand command = new SqlCommand(query, baglanti);
                    command.ExecuteNonQuery();
                    string query2 = "select * from demirbaslar";
                    SqlCommand command2 = new SqlCommand(query2, baglanti);
                    SqlDataAdapter adapter = new SqlDataAdapter(command2);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    dtgDemirbasEkle.DataSource = ds.Tables[0];
                    baglanti.Close();
                }
            }
        }
        //Oda işlemlerinde Veritabanına Oda ekleme işlemleri

        private void tbDemirbasStokTakipAraci_Click_1(object sender, EventArgs e)
        {

        }
        private void txtPersonelAd(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) //sayı girmeyi engelliyor
               && !char.IsSeparator(e.KeyChar);
        }
        private void txtPersonelSoyad(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) //sayı girmeyi engelliyor
               && !char.IsSeparator(e.KeyChar);
        }

        private void txtDemirbasad(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) //sayı girmeyi engelliyor
               && !char.IsSeparator(e.KeyChar);
        }

        private void txtAdet(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);  //karakter girmeyi engelliyor
        }

        private void txtFiyat(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);  //karakter girmeyi engelliyor
        }

        private void txtDEMIBASAD(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) //sayı girmeyi engelliyor
              && !char.IsSeparator(e.KeyChar);
            if (string.IsNullOrEmpty(txtdemirbasAdi.Text))
            {
                dtStokIslemleri.DataSource = txtdemirbasAdi.Text;
            }
        }

        private void txtFIYAT(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);  //karakter girmeyi engelliyor
        }

        private void txtADET(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);//karakter girmeyi engelliyor
        }

        private void txtodaAdi(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) //sayı girmeyi engelliyor
              && !char.IsSeparator(e.KeyChar);
        }

        private void txtODAADI(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) //sayı girmeyi engelliyor
              && !char.IsSeparator(e.KeyChar);
        }

        private void txtPERSONELSOYADI(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) //sayı girmeyi engelliyor
              && !char.IsSeparator(e.KeyChar);
        }


        int selectedRow;
        //Oda Ekleme işlemleri
        private void btnOdaEkle_Click(object sender, EventArgs e)
        {
            if (baglanti.State == System.Data.ConnectionState.Closed)
            {
                if (cmbOdaDepartmanAdi.Text == "" || cmbOdaSorumlusu.Text == "" || txtOdaAdi.Text == "")
                {
                    MessageBox.Show("Alanlar boş geçilemez!!!");
                }
                else
                {
                    string departmanAd = cmbOdaDepartmanAdi.Text;
                    string odaAd = txtOdaAdi.Text;
                    string personelAd = cmbOdaSorumlusu.Text;
                    string query_departman = "(SELECT departmanID from departmanlar where departmanAd='" + departmanAd + "')";
                    string query_personel = "(SELECT personelID from personeller where personelAd='" + personelAd + "')";
                    baglanti.Open();
                    string query = "INSERT INTO odalar(odaAd,departmanID,personelID) " +
                       "Values('" + odaAd + "'," + query_departman + ", " + query_personel + ")";
                    SqlCommand command = new SqlCommand(query, baglanti);
                    command.ExecuteNonQuery();
                    string query2 = "select * from odalar";
                    SqlCommand command2 = new SqlCommand(query2, baglanti);
                    SqlDataAdapter adapter = new SqlDataAdapter(command2);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    dtgvOdaBilgisi.DataSource = ds.Tables[0];
                    baglanti.Close();
                }
            }
        }
        //Oda uzerinde demirbas guncelle
        private void btnOdaGuncelle_Click(object sender, EventArgs e)
        {
            baglanti.Open();
            SqlCommand comOdaEkle = new SqlCommand("");
            SqlDataAdapter sda = new SqlDataAdapter();
            SqlCommandBuilder scb = new SqlCommandBuilder(sda);
            DataTable dt = new DataTable();
            try
            {
                dtgvOdaBilgisi.Update();
            }
            catch (Exception exUp)
            {
                MessageBox.Show(exUp.Message.ToString());
                throw;
            }

            sda.Update(dt);
        }
        //Oda Uzerinde  Demirbas silme
        private void btnOdaSil_Click(object sender, EventArgs e)
        {
            System.Collections.ArrayList slv = new System.Collections.ArrayList();
            baglanti.Open();
            int i = 0;
            for (i = 0; i < dtgvOdaBilgisi.Rows.Count; i++)
            {
                DataGridViewRow dgvr = dtgvOdaBilgisi.Rows[i];
                if (dtgvOdaBilgisi.SelectedRows.Count >= -1)
                {
                    if (dgvr.Selected == true)
                    {
                        try
                        {
                            SqlCommand silCom = new SqlCommand("Delete  From odalar Where odaID='" + i + "'", baglanti);
                            SqlDataAdapter adapSil = new SqlDataAdapter(silCom);
                            dtgvOdaBilgisi.Rows.RemoveAt(i);
                            int say = silCom.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                            throw;
                        }
                    }
                }
                else
                {
                    MessageBox.Show(selectedRow.ToString());
                }
            }
            baglanti.Close();
        }
        //Oda Uzerinde Demirbas Arama
        private void btnOdaArama_Click(object sender, EventArgs e)
        {
            if (baglanti.State == System.Data.ConnectionState.Closed)
            {
                if (txtOdaArama.Text == "")
                {
                    MessageBox.Show("Boş geçilemez!!!");
                }
                else
                {
                    baglanti.Open();
                    SqlCommand komut = new SqlCommand("SELECT d.demirbasAd  , d.demirbasTuruID ,d.adet ,d.fiyat ,d.satınAlimTarihi  FROM  demirbaslar d " +
                    "INNER JOIN odaDemirbaslar od ON d.demirbasID = od.demirbasID INNER JOIN odalar o ON  o.odaID=od.odaID WHERE odaAd='" + txtOdaArama.Text + "'", baglanti);
                    SqlDataAdapter adapter = new SqlDataAdapter(komut);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    //SqlDataReader reader = komut.ExecuteReader();
                    dgOdaUzerindaDemirbaslar.DataSource = ds.Tables[0];
                    baglanti.Close();
                }
            }
        }
        //Oda bilgileri gösteriliyor
        private void btngoster_Click(object sender, EventArgs e)
        {
            baglanti.Open();
            SqlCommand commend = new SqlCommand("Select * From odalar", baglanti);
            SqlDataAdapter sda = new SqlDataAdapter(commend);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            dtgvOdaBilgisi.DataSource = dt;
            baglanti.Close();
        }

        bool cikisSec = false;


        private void btnStokEkle_Click(object sender, EventArgs e)
        {
            int demirbasAdet = Convert.ToInt32(textAdet.Text);
            int demirbasFiyat = Convert.ToInt32(textFiyat.Text);
            string dt = Convert.ToDateTime(dtsatinAlmaTarihi.Value).ToString("yyyy-MM-dd");
            string demirbasTur = cmdDemirbasTuru.Text;
            string demirbasAd = txtdemirbasad.Text;
            string departman = cmbDepartman.Text;

            if (baglanti.State == System.Data.ConnectionState.Closed)
            {
                if (cmbDepartman.Text == "" || cmdDemirbasTuru.Text == "" || txtdemirbasad.Text == "" || dt == "" ||
                    demirbasAdet.ToString() == "" || demirbasFiyat.ToString() == "")
                {
                    MessageBox.Show("Alanlar boş geçilemez!!!");
                }
                else
                {
                    string query_demirbastur = "(SELECT demirbasTuruID from demirbasTurleri where demirbasTuruAd='" + demirbasTur + "')";
                    baglanti.Open();
                    string query = "INSERT INTO demirbaslar(demirbasAd,demirbasTuruID, adet, fiyat,satınAlimTarihi) " +
                       "Values('" + demirbasAd + "'," + query_demirbastur + ", '" + demirbasAdet + "','" + demirbasFiyat + "','" + dt + "')";
                    SqlCommand command = new SqlCommand(query, baglanti);
                    command.ExecuteNonQuery();
                    string query2 = "select * from demirbaslar";
                    SqlCommand command2 = new SqlCommand(query2, baglanti);
                    SqlDataAdapter adapter = new SqlDataAdapter(command2);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    dtgDemirbasEkle.DataSource = ds.Tables[0];
                    baglanti.Close();
                }
            }
        }

        private void btnAra_Click(object sender, EventArgs e)
        {
            int demirbasAdet = Convert.ToInt32(textAdet.Text);
            int demirbasFiyat = Convert.ToInt32(textFiyat.Text);
            string dt = Convert.ToDateTime(dateTimePicker1.Value).ToString("yyyy-MM-dd");
            string demirbasTur = cmdDemirbasTuru.Text;
            string demirbasAd = txtdemirbasAdi.Text;
            string departman = cmbDepartmanAd.Text;

            if (baglanti.State == System.Data.ConnectionState.Closed)
            {
                if (cmbDepartmanAd.Text == "" || txtdemirbasAdi.Text == "" || dt == "" ||
                    txtfiyat.ToString() == "" || txtadet.ToString() == "")
                {
                    MessageBox.Show("Alanlar boş geçilemez!!!");
                }
                else
                {
                    string query_demirbastur = "(SELECT demirbasTuruID from demirbasTurleri where demirbasTuruAd='" + demirbasTur + "')";
                    baglanti.Open();
                    string query = "SELECT d.demirbasAd  , d.demirbasTuruID ,d.adet ,d.fiyat ,d.satınAlimTarihi  FROM  demirbaslar d " +
                    "INNER JOIN odaDemirbaslar od ON d.demirbasID = od.demirbasID INNER JOIN odalar o ON " +
                    "o.odaID=od.odaID INNER JOIN departmanlar d ON d.departmanID=o.departmanID WHERE departmanAd='" + txtPersonelAramaAd.Text + "'AND personelSoyad='" + txtPersonelAramaSoyad.Text + "'";
                    SqlCommand command = new SqlCommand(query, baglanti);
                    command.ExecuteNonQuery();
                    string query2 = "select * from demirbaslar";
                    SqlCommand command2 = new SqlCommand(query2, baglanti);
                    SqlDataAdapter adapter = new SqlDataAdapter(command2);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    dtStokIslemleri.DataSource = ds.Tables[0];
                    baglanti.Close();
                }
            }

        }

        private void Demirbas_Click(object sender, EventArgs e)
        {
            string demirbasTur = cmdDemirbasTuru.Text;
            string departman = cmbDepartman.Text;// bu textler boş geliyo // boş geldiği için where şartında boş gelenleri seçmesini istiyosun.
            baglanti.Open();
            SqlCommand comDepartmanListesi = new SqlCommand("SELECT * FROM departmanlar", baglanti);
            // comDepartmanListesi.CommandType = CommandType.Text;
            SqlDataReader reader = comDepartmanListesi.ExecuteReader();
            cmbDepartman.Items.Add("");
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    cmbDepartman.Items.Add(reader["departmanAd"]);
                }
            }
            reader.Close();
            baglanti.Close();
            cmbDepartman.SelectedIndex = 0;

            baglanti.Open();
            SqlCommand comDemirbasTuru = new SqlCommand("SELECT * from demirbasTurleri", baglanti);
            SqlDataReader readTuru = comDemirbasTuru.ExecuteReader();
            cmdDemirbasTuru.Items.Add("");
            if (readTuru.HasRows)
            {
                while (readTuru.Read())
                {
                    cmdDemirbasTuru.Items.Add(readTuru["demirbasTuruAd"]);
                }
            }
            readTuru.Close();
            baglanti.Close();
            cmdDemirbasTuru.SelectedIndex = 0;

            baglanti.Open();
            SqlCommand comOdaDepartman = new SqlCommand("SELECT * FROM departmanlar", baglanti);
            SqlDataReader okuOdaDepartman = comOdaDepartman.ExecuteReader();
            cmbOdaDepartmanAdi.Items.Add("");
            if (okuOdaDepartman.HasRows)
            {
                while (okuOdaDepartman.Read())
                {
                    cmbOdaDepartmanAdi.Items.Add(okuOdaDepartman["departmanAd"]);
                }
            }
            okuOdaDepartman.Close();
            baglanti.Close();
            cmbOdaDepartmanAdi.SelectedIndex = 0;

            baglanti.Open();
            SqlCommand comOdaPersonel = new SqlCommand("SELECT * FROM personeller", baglanti);
            SqlDataReader okuOdaPersonel = comOdaPersonel.ExecuteReader();
            cmbOdaSorumlusu.Items.Add("");
            if (okuOdaPersonel.HasRows)
            {
                while (okuOdaPersonel.Read())
                {
                    cmbOdaSorumlusu.Items.Add(okuOdaPersonel["personelAd"]);
                }
            }
            okuOdaPersonel.Close();
            baglanti.Close();
            cmbOdaSorumlusu.SelectedIndex = 0;

            baglanti.Open();
            SqlCommand comDepartmanAra = new SqlCommand("SELECT * FROM departmanlar", baglanti);
            SqlDataReader okuDepartman = comDepartmanAra.ExecuteReader();
            cmbDepartmanAd.Items.Add("");
            if (okuDepartman.HasRows)
            {
                while (okuDepartman.Read())
                {
                    cmbDepartmanAd.Items.Add(okuDepartman["departmanAd"]);
                }
            }
            okuDepartman.Close();
            baglanti.Close();
            cmbDepartmanAd.SelectedIndex = 0;
        }

        public class ytesti
        {
            SqlConnection baglanti = new SqlConnection("Data Source=DESKTOP-OPJP9FR\\ADMIN_PELIN;Initial Catalog=DemirbaşTakip;Integrated Security=True");
            public int yetkiligirisikontrol(string ad, string sifre)
            {
                int sifree = 0;
                string sql = "select kullaniciAd, kullaniciSifre from kullanicilar";
                baglanti.Open();
                SqlCommand cmd = new SqlCommand(sql, baglanti);
                cmd.ExecuteNonQuery();
                SqlDataReader kullanicioku = cmd.ExecuteReader();
                while (kullanicioku.Read())
                {
                    if (kullanicioku["kullaniciAd"].ToString() == ad && kullanicioku["kullaniciSifre"].ToString() == sifre)
                    {
                        sifree = Convert.ToInt32(kullanicioku["kullaniciSifre"]);
                    }
                }
                return sifree;
            }
        }
        public class yazilimTesti
        {
        SqlConnection baglanti = new SqlConnection("Data Source=DESKTOP-OPJP9FR\\ADMIN_PELIN;Initial Catalog=DemirbaşTakip;Integrated Security=True");
            public int personelgiriskontrol(string ad, string sifre)
            {
                int sifree2 = 0;
                string sql = "select kullaniciAd, kullaniciSifre from kullanicilar";
                baglanti.Open();
                SqlCommand cmd = new SqlCommand(sql, baglanti);
                cmd.ExecuteNonQuery();
                SqlDataReader kullanicioku = cmd.ExecuteReader();
                while (kullanicioku.Read())
                {
                    if (kullanicioku["kullaniciAd"].ToString() == ad && kullanicioku["kullaniciSifre"].ToString() == sifre)
                    {
                        sifree2 = Convert.ToInt32(kullanicioku["kullaniciSifre"]);
                    }
                }
                return sifree2;
            }

        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {

        }
    }
}