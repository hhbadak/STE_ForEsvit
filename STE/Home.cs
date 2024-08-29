using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace STE
{
    public partial class Home : Form
    {
        public static Employee LoginUser;
        DataModel dm = new DataModel();

        public Home()
        {
            InitializeComponent();
        }

        private void Home_Load(object sender, EventArgs e)
        {
            Login frm = new Login();
            frm.ShowDialog();
            Employee model = Helpers.GirisYapanKullanici;

            // GetFault() metodundan verileri alıyoruz
            var faultList = dm.GetFault();

            // Seçiniz öğesini ekliyoruz
            faultList.Insert(0, new Fault { FaultID = 0, FaultDescripton = "Seçiniz" });

            // ComboBox'a verileri ekliyoruz
            cb_fault.DataSource = faultList;
            cb_fault.ValueMember = "FaultID";
            cb_fault.DisplayMember = "FaultDescripton";

            // GetQuality() metodundan verileri alıyoruz
            var qualityList = dm.GetQuality();

            // Seçiniz öğesini ekliyoruz
            qualityList.Insert(0, new ListQuality { ID = 0, Definition = "Seçiniz" });

            // ComboBox'a verileri ekliyoruz
            cb_quality.DataSource = qualityList;
            cb_quality.ValueMember = "ID";
            cb_quality.DisplayMember = "Definition";

            loadGrid();
        }

        private void loadGrid()
        {
            var result = dm.logEntryListSte(new Ste
            {
                QualityID = cb_quality.SelectedIndex,
                FaultID = cb_fault.SelectedIndex,
                Barcode = tb_barcode.Text
            });

            if (result != null)
            {
                var rt = result.OrderByDescending(r => r.ID).ToList();
                DataTable dt = new DataTable();

                dt.Columns.Add("ID");
                dt.Columns.Add("Barkod No");
                dt.Columns.Add("Kod");
                dt.Columns.Add("Kontrol Tarihi");
                dt.Columns.Add("Kalite");
                dt.Columns.Add("STE Personeli");

                foreach (var item in rt)
                {
                    DataRow r = dt.NewRow();
                    r["ID"] = item.ID;
                    r["Barkod No"] = item.Barcode;
                    r["Kod"] = item.ProductCode;
                    r["Kontrol Tarihi"] = item.DateTime.ToShortDateString();
                    r["Kalite"] = item.Quality;
                    r["STE Personeli"] = item.QualityPersonal;
                    dt.Rows.Add(r);
                }

                dgv_product.DataSource = dt;
                lbl_number.Text = "Bakılan Ürün sayısı: " + dgv_product.RowCount;
            }
            else
            {
                MessageBox.Show("Veri yüklenirken bir hata oluştu.");
            }
            tb_barcode.Select();
        }

        private void tb_barcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Ste ste = new Ste();

                if (tb_barcode.Text.Length == 10)
                {
                    ste.Barcode = tb_barcode.Text;

                    // Eğer kalite veya hata kodu seçilmişse, güncellemeyi yap
                    bool isFaultSelected = cb_fault.SelectedIndex > 0;
                    bool isQualitySelected = cb_quality.SelectedIndex > 0;

                    // Ürün bilgilerini al
                    var product = dm.getProductDetails(ste.Barcode);

                    if (product != null)
                    {
                        // Eğer ürün varsa ve seçim yapılmamışsa, ürünün mevcut kalite ve hata bilgilerini kullan
                        if (!isFaultSelected)
                        {
                            ste.FaultID = product.FaultID; // Mevcut hata kodunu al
                        }
                        else
                        {
                            // Seçili hata kodunu kullan
                            int faultID;
                            if (int.TryParse(cb_fault.SelectedValue?.ToString(), out faultID))
                            {
                                ste.FaultID = faultID;
                            }
                        }

                        if (!isQualitySelected)
                        {
                            ste.QualityID = product.QualityID; // Mevcut kalite kodunu al
                        }
                        else
                        {
                            // Seçili kalite kodunu kullan
                            int qualityID;
                            if (int.TryParse(cb_quality.SelectedValue?.ToString(), out qualityID))
                            {
                                ste.QualityID = qualityID;
                            }
                        }

                        // Güncellemeleri yap
                        if (isFaultSelected)
                        {
                            dm.updateProductFault(ste);
                        }

                        if (isQualitySelected)
                        {
                            dm.updateProductQuality(ste);
                        }

                        ste.DateTime = DateTime.Now;
                        ste.QualityPersonalID = LoginUser?.ID ?? -1; // Kullanıcı ID'si

                        // Ste tablosuna veri ekle
                        if (dm.createSte(ste))
                        {
                            tb_barcode.Text = "";
                            cb_fault.SelectedIndex = cb_quality.SelectedIndex = 0;
                            loadGrid();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Barkod bulunamadı.");
                    }
                }
            }
        }




    }
}
