using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace HotelAutomationdotnet
{
    public partial class AdminPanel : Form
    {
        private NpgsqlConnection connection = new NpgsqlConnection("Host=localhost;Username=postgres;Password=password;Database=temp_proje");

        public AdminPanel()
        {
            InitializeComponent();
            comboBox2.Visible = false;
            listView1.Visible = false;
            listView2.Visible = false;
            listView3.Visible = false;
        }

        private void verileriGoruntule()
        {
            listView1.Items.Clear();

            try
            {
                connection.Open();
                string query = "SELECT * FROM get_admin_info();";  // Fonksiyon çağrısı

                using (var command = new NpgsqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new ListViewItem(reader["reservation_id"].ToString());
                        item.SubItems.Add(reader["customer_id"].ToString());
                        item.SubItems.Add(reader["isim_soyisim"].ToString());
                        item.SubItems.Add(reader["oda_numarasi"].ToString());
                        item.SubItems.Add(reader["expense_amount"].ToString());
                        item.SubItems.Add(reader["giris_tarihi"].ToString());
                        item.SubItems.Add(reader["cikis_tarihi"].ToString());
                        listView1.Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private void islemleriGoruntule()
        {
            listView2.Items.Clear();

            try
            {
                connection.Open();
                string query = "SELECT * FROM get_accountant_info();";

                using (var command = new NpgsqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // ListView öğesi oluştur
                        var item = new ListViewItem(reader["transaction_id"].ToString());

                        // Alt öğeler ekle
                        item.SubItems.Add(reader["reservation_id"].ToString());
                        item.SubItems.Add(reader["customer_id"].ToString());
                        item.SubItems.Add(reader["fee"].ToString());
                        item.SubItems.Add(reader["transaction_date"].ToString());

                        // ListView'e öğeyi ekle
                        listView2.Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private void odalariGoruntule()
        {
            listView3.Items.Clear();

            try
            {
                connection.Open();
                string query = "SELECT * FROM get_room_expenses_info();";  // Calling the function

                using (var command = new NpgsqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Create a new ListView item
                        var item = new ListViewItem(reader["oda_numarasi"].ToString());

                        // Add subitems (columns) to the ListView item
                        item.SubItems.Add(reader["base_price"].ToString());
                        item.SubItems.Add(reader["expense_type"].ToString());
                        item.SubItems.Add(reader["expense_amount"].ToString());

                        // Add the item to the ListView
                        listView3.Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }




        private void ciroyuGoruntule()
        {
            try
            {
                connection.Open();
                string query = "SELECT SUM(expense_amount) AS toplam_ciro FROM reservation_expenses_normalized";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    var totalRevenue = command.ExecuteScalar();
                    ciroBox.Text = Convert.ToDecimal(totalRevenue).ToString("C2");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

       


        private void AdminPanel_Load(object sender, EventArgs e)
        {
            // Form yüklendiğinde yapılacak işlemler buraya eklenebilir.
        }

        private void odalariGoruntuleButton_Click(object sender, EventArgs e)
        {
            listView1.Visible = true;
            listView2.Visible = false;
            verileriGoruntule();
        }

        private void ciroyuGoruntuleButton_Click(object sender, EventArgs e)
        {
            ciroyuGoruntule();
        }

        private void gunlukCiro_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if the connection is already open
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    connection.Open();
                }

                // Log selected date
                DateTime selectedDate = dateTimePicker1.Value.Date;

                // Define the query to get the total earnings for all rooms on the selected date
                string query = @"
            SELECT COALESCE(SUM(toplamkazanc), 0) AS toplamgelir
            FROM gunlukkazanc_view
            WHERE @selected_date BETWEEN date1 AND date2;
        ";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@selected_date", selectedDate);

                    // Execute the query and retrieve the total earnings
                    var result = command.ExecuteScalar();

                    // Log the result
                    if (result != null && result != DBNull.Value)
                    {
                        decimal totalEarnings = Convert.ToDecimal(result);
                        gunlukTextBox.Text = totalEarnings.ToString("C2"); // Display total earnings in currency format
                    }
                    else
                    {
                        gunlukTextBox.Text = "₺0,00"; // If no earnings found, display 0
                    }
                }
            }
            catch (Exception ex)
            {
                // Display error message
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
           
                comboBox2.Visible = true;
            

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            comboBox2.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listView2.Visible = true;
            listView1.Visible = false;
            islemleriGoruntule();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listView3.Visible = true;
            odalariGoruntule();
        }
    }
}


