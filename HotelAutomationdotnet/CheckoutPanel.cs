using System;
using System.Windows.Forms;
using Npgsql;

namespace HotelAutomationdotnet
{

    public partial class CheckoutPanel : Form
    {

        DateTime girisTarihi;
        DateTime cikisTarihi;
        public CheckoutPanel()
        {
            InitializeComponent();
        }
        
         private void odaKontrolButton_Click(object sender, EventArgs e)
        {
            // TextBox'lardan verilerin alınması
            string customerName = nameBox2.Text.Trim();
            string roomNumberText = roomNumberBox2.Text.Trim();

            // Girişleri doğrulama
            if (string.IsNullOrEmpty(customerName) || string.IsNullOrEmpty(roomNumberText))
            {
                MessageBox.Show("Lütfen müşteri bilgilerini doğru giriniz.");
                return;
            }

            // Oda numarasını integer'a çevirme
            if (!int.TryParse(roomNumberText, out int roomNumber))
            {
                MessageBox.Show("Oda numarası sayı olmalıdır.");
                return;
            }

            // Check the reservation status with the customer name, room number, and week number
            CheckReservationMatch(customerName, roomNumber, dateTimePicker1.Value, dateTimePicker2.Value);
        }

        private void rezervasyonuİptalEtButton_Click(object sender, EventArgs e)
        {
            string customerName = nameBox2.Text.Trim();
            string roomNumberText = roomNumberBox2.Text.Trim();

            if (string.IsNullOrEmpty(customerName) || !int.TryParse(roomNumberText, out int roomNumber))
            {
                MessageBox.Show("Müşteri bilgilerini ve oda numarasını kontrol ediniz.");
                return;
            }

            DeleteReservation(customerName, roomNumber);
        }
        private void CheckReservationMatch(string customerName, int roomNumber, DateTime start_date, DateTime checkout_date)
        {
            string connectionString = "Host=localhost;Username=postgres;Password=password;Database=temp_proje";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT res.*
                        FROM reservations_normalized res
                        JOIN customers_normalized cust ON res.customer_id = cust.customer_id
                        WHERE cust.isim_soyisim = @customerName
                        AND res.oda_numarasi = @roomNumber
                        AND res.giris_tarihi = @start_date
                        AND res.cikis_tarihi = @checkout_date";

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@customerName", customerName);
                        command.Parameters.AddWithValue("@roomNumber", roomNumber);
                        command.Parameters.AddWithValue("@start_date", start_date.Date);
                        command.Parameters.AddWithValue("@checkout_date", checkout_date.Date);

                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                MessageBox.Show("Rezervasyon bulundu.");
                                rezervasyonuİptalEtButton.Enabled = true;
                            }
                            else
                            {
                                MessageBox.Show("Rezervasyon bulunamadı.");
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }


        private void DeleteReservation(string customerName, int roomNumber)
        {
            string connectionString = "Host=localhost;Username=postgres;Password=password;Database=temp_proje";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    // Rezervasyon ID'sini al
                    string getReservationAndCustomerIdQuery = @"SELECT * FROM get_reservation_and_customer_id(@roomNumber, @customerName)";

                    int reservationId = 0;
                    int customerId = 0;

                    using (NpgsqlCommand getReservationAndCustomerIdCommand = new NpgsqlCommand(getReservationAndCustomerIdQuery, connection))
                    {
                        getReservationAndCustomerIdCommand.Parameters.AddWithValue("@customerName", customerName);
                        getReservationAndCustomerIdCommand.Parameters.AddWithValue("@roomNumber", roomNumber);

                        using (var reader = getReservationAndCustomerIdCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                reservationId = reader.GetInt32(0); // p_reservation_id
                                customerId = reader.GetInt32(1); // p_customer_id
                            }
                            else
                            {
                                MessageBox.Show("Rezervasyon bulunamadı.");
                                return;
                            }
                        }
                    }


                    // Müşteri ID'sini al
                    string getCustomerIdQuery = @"
            SELECT customer_id 
            FROM customers_normalized 
            WHERE isim_soyisim = @customerName
            LIMIT 1";

                    using (NpgsqlCommand getCustomerIdCommand = new NpgsqlCommand(getCustomerIdQuery, connection))
                    {
                        getCustomerIdCommand.Parameters.AddWithValue("@customerName", customerName);

                        object result = getCustomerIdCommand.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            customerId = Convert.ToInt32(result);
                        }
                    }

                    
                        string deleteaccQuery = @"CALL delete_accountant_record(@customerId, @reservationId)";

                        using (NpgsqlCommand deleteCustomerCommand = new NpgsqlCommand(deleteaccQuery, connection))
                        {
                        deleteCustomerCommand.Parameters.AddWithValue("@customerId", customerId);
                        deleteCustomerCommand.Parameters.AddWithValue("@reservationId", reservationId);

                        // Prosedürü çalıştır
                        deleteCustomerCommand.ExecuteNonQuery();
                    }
                    

                    // Önce harcama kayıtlarını sil
                    string deleteExpensesQuery = @"
            DELETE FROM reservation_expenses_normalized 
            WHERE reservation_id = @reservationId";

                    using (NpgsqlCommand deleteExpensesCommand = new NpgsqlCommand(deleteExpensesQuery, connection))
                    {
                        deleteExpensesCommand.Parameters.AddWithValue("@reservationId", reservationId);
                        deleteExpensesCommand.ExecuteNonQuery();
                    }

                    // Sonra rezervasyonu sil
                    string deleteReservationQuery = @"
            DELETE FROM reservations_normalized 
            WHERE reservation_id = @reservationId";

                    using (NpgsqlCommand deleteReservationCommand = new NpgsqlCommand(deleteReservationQuery, connection))
                    {
                        deleteReservationCommand.Parameters.AddWithValue("@reservationId", reservationId);
                        deleteReservationCommand.ExecuteNonQuery();
                    }

                    // Müşterinin başka rezervasyonu var mı?
                    string checkOtherReservationsQuery = @"
            SELECT COUNT(*) 
            FROM reservations_normalized 
            WHERE customer_id = @customerId";

                    int otherReservationsCount = 0;

                    using (NpgsqlCommand checkOtherReservationsCommand = new NpgsqlCommand(checkOtherReservationsQuery, connection))
                    {
                        checkOtherReservationsCommand.Parameters.AddWithValue("@customerId", customerId);

                        object result = checkOtherReservationsCommand.ExecuteScalar();
                        otherReservationsCount = result != null ? Convert.ToInt32(result) : 0;
                    }

                    // Eğer başka rezervasyon yoksa müşteriyi sil
                    if (otherReservationsCount == 0)
                    {
                        string deleteCustomerQuery = @"
                DELETE FROM customers_normalized 
                WHERE customer_id = @customerId";

                        using (NpgsqlCommand deleteCustomerCommand = new NpgsqlCommand(deleteCustomerQuery, connection))
                        {
                            deleteCustomerCommand.Parameters.AddWithValue("@customerId", customerId);
                            deleteCustomerCommand.ExecuteNonQuery();
                        }
                    }

                    // Eğer başka rezervasyon yoksa müşteriyi sil
                   

                    MessageBox.Show("Rezervasyon ve ilgili müşteri bilgileri başarıyla silindi.");
                }
            }

            catch (NpgsqlException ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }




        private void nameBox2_Click(object sender, EventArgs e)
        {
            nameBox2.Text = "";
        }

        private void roomNumberBox2_Click(object sender, EventArgs e)
        {
            roomNumberBox2.Text = "";
        }

       

   
    }
}
