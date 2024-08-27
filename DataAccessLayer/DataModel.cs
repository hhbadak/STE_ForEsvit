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

    }
}
