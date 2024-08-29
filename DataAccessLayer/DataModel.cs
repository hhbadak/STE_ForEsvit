using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class DataModel
    {
        SqlConnection con; SqlCommand cmd;

        public DataModel()
        {
            con = new SqlConnection(ConnectionStrings.ConStr);
            cmd = con.CreateCommand();
        }

        #region Personal Metot
        public Employee personalLogin(string username, string password)
        {
            Employee model = new Employee();
            try
            {
                cmd.CommandText = "SELECT Kimlik FROM kullanici_liste WHERE kullanici_adi = @uName AND sifre = @password";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@uName", username);
                cmd.Parameters.AddWithValue("@password", password);
                con.Open();
                int id = Convert.ToInt32(cmd.ExecuteScalar());
                if (id > 0)
                {
                    model = getPersonal(id);
                }
                return model;

            }
            catch
            {
                return null;
            }
            finally { con.Close(); }
        }

        public Employee getPersonal(int id)
        {
            try
            {
                Employee model = new Employee();
                cmd.CommandText = "SELECT Kimlik, kullanici_adi, sifre, ad_soyad, durum, pcAd, versiyon, KisaAd, Departman \r\nFROM kullanici_liste\r\nWHERE Kimlik = @id";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@id", id);
                if (con.State != System.Data.ConnectionState.Open)
                {
                    con.Open();
                }
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    model.ID = Convert.ToInt32(reader["Kimlik"]);
                    model.Username = reader.GetString(1);
                    model.Password = reader.GetString(2);
                    model.NameSurname = reader.GetString(3);
                    model.Status = reader.GetByte(4);
                    model.PcName = reader.GetString(5);
                    model.Version = reader.GetString(6);
                    model.ShortName = reader.GetString(7);
                    model.Department = reader.GetString(8);
                }
                return model;
            }
            catch
            {
                return null;
            }
            finally { con.Close(); }
        }
        #endregion

        #region Product Metot


        public bool updateProductFault(Ste ste)
        {
            try
            {
                cmd.CommandText = "UPDATE Products SET Fault = @fault WHERE Barcode=@barcode";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@fault", ste.FaultID);
                cmd.Parameters.AddWithValue("@barcode", ste.Barcode);
                con.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
            finally { con.Close(); }
        }

        public bool updateProductQuality(Ste ste)
        {
            try
            {
                cmd.CommandText = "UPDATE Products SET Quality = @quality, Fault = 46 WHERE Barcode=@barcode";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Quality", ste.QualityID);
                cmd.Parameters.AddWithValue("@barcode", ste.Barcode);
                con.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
            finally { con.Close(); }
        }

        //public Products products(string barcode)
        //{
        //    Products model = new Products();
        //    try
        //    {
        //        cmd.CommandText = "SELECT Fault FROM Products WHERE Barcode = @barcode";
        //        cmd.Parameters.Clear();

        //        cmd.Parameters.AddWithValue("@barcode", model.Barcode);

        //        con.Open();
        //        int id = Convert.ToInt32(cmd.ExecuteScalar());
        //        if (id > 0)
        //        {
        //            model = getProductFault();
        //        }
        //        return model;

        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //    finally { con.Close(); }
        //}

        public List<Ste> getProductFault(string filter)
        {
            List<Ste> rt = new List<Ste>();
            try
            {
                cmd.CommandText = @"SELECT Fault FROM Products WHERE Barcode = @barcode";

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@barcode", filter);

                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Ste model = new Ste
                        {
                            FaultID = reader.GetByte(0),
                        };
                        rt.Add(model);
                    }
                }
                return rt;
            }
            catch
            {
                return null;
            }
            finally
            {
                con.Close();
            }
        }

        public List<Products> getProductQuality(string filter)
        {
            List<Products> rt = new List<Products>();
            try
            {
                cmd.CommandText = @"SELECT Quality FROM Products WHERE Barcode = @barcode";

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@barcode", filter);

                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Products model = new Products
                        {
                            QualityID = reader.GetByte(0),
                        };
                        rt.Add(model);
                    }
                }
                return rt;
            }
            catch
            {
                return null;
            }
            finally
            {
                con.Close();
            }
        }

        public List<Products> logEntryListProduct(Products filter)
        {
            List<Products> rt = new List<Products>();
            try
            {
                cmd.CommandText = @"SELECT Id, Barcode, Quality, Fault FROM Products WHERE Barcode = @barcode";

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@barcode", filter.Barcode);

                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Products model = new Products
                        {
                            // Bu satırların veri tiplerini kontrol edin ve doğru dönüşümler yapın
                            ProductID = reader.GetInt32(0),
                            Barcode = reader.GetString(1),
                            QualityID = reader.GetByte(2),
                            FaultID = reader.GetByte(3),
                        };
                        rt.Add(model);
                    }
                }
                return rt;
            }
            catch
            {
                return null;
            }
            finally
            {
                con.Close();
            }
        }

        #endregion

        #region Fault Metot

        public List<Fault> GetFault()
        {
            List<Fault> fault = new List<Fault>();
            try
            {
                cmd.CommandText = "SELECT Kimlik, numara, tanim FROM hata_liste";
                cmd.Parameters.Clear();
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Fault f = new Fault() { FaultID = reader.GetByte(0), Number = reader.GetInt16(1), FaultDescripton = !reader.IsDBNull(2) ? reader.GetString(2) : "Açıklama Yok" };
                    fault.Add(f);
                }
                return fault;
            }
            catch
            {
                return null;
            }
            finally
            {
                con.Close();
            }
        }

        #endregion

        #region Quality Metot

        public List<ListQuality> GetQuality()
        {
            List<ListQuality> quality = new List<ListQuality>();
            try
            {
                cmd.CommandText = "SELECT Kimlik, numara, tanim FROM kalite_liste";
                cmd.Parameters.Clear();
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ListQuality q = new ListQuality() { ID = reader.GetByte(0), Number = reader.GetInt16(1), Definition = !reader.IsDBNull(2) ? reader.GetString(2) : "Açıklama Yok" };
                    quality.Add(q);
                }
                return quality;
            }
            catch
            {
                return null;
            }
            finally
            {
                con.Close();
            }
        }

        #endregion

        public bool updateProductAndSte(Ste ste)
        {
            try
            {
                // Önce Products tablosunu güncelle
                cmd.CommandText = "UPDATE Products SET Quality = @quality, Fault = @fault WHERE Barcode = @barcode";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@quality", ste.QualityID);
                cmd.Parameters.AddWithValue("@fault", ste.FaultID);
                cmd.Parameters.AddWithValue("@barcode", ste.Barcode);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                // Ardından Ste tablosuna kaydı ekle
                cmd.CommandText = "INSERT INTO kalite_STE(Barcode, QualityID, FaultID, DateTime, QualityPersonalID) VALUES(@barcode, @quality, @fault, FORMAT(@date, 'yyyy-MM-dd HH:mm:ss'), @qpid)";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@barcode", ste.Barcode);
                cmd.Parameters.AddWithValue("@quality", ste.QualityID);
                cmd.Parameters.AddWithValue("@fault", ste.FaultID);
                cmd.Parameters.AddWithValue("@date", ste.DateTime);
                cmd.Parameters.AddWithValue("@qpid", ste.QualityPersonalID);

                con.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                con.Close();
            }
        }


        #region Ste Metot

        public bool processBarcodeScan(Ste ste)
        {
            // Eğer hata ve kalite bilgisi girilmemişse, barkoddan gelen veriyi al
            if (ste.QualityID == 0 && ste.FaultID == 0)
            {
                Ste productDetails = getProductDetails(ste.Barcode);
                if (productDetails != null)
                {
                    ste.QualityID = productDetails.QualityID;
                    ste.FaultID = productDetails.FaultID;
                }
            }

            // Ürünü güncelle ve Ste tablosuna kaydı ekle
            return updateProductAndSte(ste);
        }


        public List<Ste> logEntryListSte(Ste filter)
        {
            List<Ste> rt = new List<Ste>();
            try
            {
                cmd.CommandText = @"SELECT ste.ID, ste.Barcode, kodl.tanim, ste.DateTime, klt.tanim, kl.kullanici_adi  
FROM kalite_STE AS ste
JOIN Products AS p ON p.Barcode = ste.Barcode
JOIN kod_liste AS kodl ON kodl.Kimlik = p.ProductCode
JOIN kalite_liste AS klt ON klt.Kimlik = ste.QualityID
JOIN kullanici_liste AS kl ON kl.Kimlik = ste.QualityPersonalID
WHERE CONVERT(date, ste.DateTime) = @datetime";

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@datetime", DateTime.Now.Date);

                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Ste model = new Ste
                        {
                            // Bu satırların veri tiplerini kontrol edin ve doğru dönüşümler yapın
                            ID = reader.GetInt32(0),
                            Barcode = reader.GetString(1),
                            ProductCode = reader.GetString(2),
                            DateTime = reader.GetDateTime(3),
                            Quality = reader.GetString(4),
                            QualityPersonal = reader.GetString(5),
                        };
                        rt.Add(model);
                    }
                }
                return rt;
            }
            catch
            {
                return null;
            }
            finally
            {
                con.Close();
            }
        }

        public Ste getProductDetails(string barcode)
        {
            try
            {
                cmd.CommandText = "SELECT Quality, Fault FROM Products WHERE Barcode = @barcode";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@barcode", barcode);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                Ste model = new Ste();
                if (reader.Read())
                {
                    model.QualityID = reader.GetByte(0);
                    model.FaultID = reader.GetByte(1);
                }
                return model;
            }
            catch
            {
                return null;
            }
            finally
            {
                con.Close();
            }
        }


        public bool createSte(Ste ste)
        {
            try
            {
                cmd.CommandText = "INSERT INTO kalite_STE(Barcode, QualityID, DateTime, QualityPersonalID) VALUES(@barcode, @qid, FORMAT(@date, 'yyyy-MM-dd HH:mm:ss'), @qpid)";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@barcode", ste.Barcode);
                cmd.Parameters.AddWithValue("@qid", ste.QualityID);
                cmd.Parameters.AddWithValue("@date", ste.DateTime);
                cmd.Parameters.AddWithValue("@qpid", ste.QualityPersonalID);
                con.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                con.Close();
            }
        }

        #endregion

    }
}
