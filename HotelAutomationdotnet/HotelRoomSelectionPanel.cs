using System;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Windows.Forms;
using Npgsql;
using NpgsqlTypes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace HotelAutomationdotnet
{
    public partial class HotelRoomSelectionPanel : Form
    {
        string customerName;
        DateTime girisTarihi;
        DateTime cikisTarihi;
        private double ucret = 0;
        private double cucret = 0;
        private double bucret = 0;
        private double mucret = 0;
        private double rucret = 0;
        private int selectedRoomNumber = 0;
        private NpgsqlConnection baglanti = new NpgsqlConnection("Host=localhost;Username=postgres;Password=password;Database=temp_proje");

        public HotelRoomSelectionPanel()
        {
            InitializeComponent();
        }




        // Method to fetch fees from the database based on room number
        private double childrenFee = 0;
        private double breakfastFee = 0;
        private double minibarFee = 0;
        private double roomServiceFee = 0;

        // Method to fetch fees from the database based on room number
        private void FetchRoomFees(int roomNumber)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(baglanti.ConnectionString))
                {
                    connection.Open();

                    // Query to fetch expenses based on room number and expense type
                    string query = @"
                SELECT expense_type, expense_amount
                FROM room_expenses_normalized
                WHERE oda_numarasi = @roomNumber";

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@roomNumber", roomNumber);

                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            // Initialize all fees to 0
                            childrenFee = breakfastFee = minibarFee = roomServiceFee = 0;

                            // Read each row and assign fees based on the expense type
                            while (reader.Read())
                            {
                                string expenseType = reader.GetString(0);
                                double expenseAmount = reader.IsDBNull(1) ? 0 : reader.GetDouble(1);

                                switch (expenseType.ToLower())
                                {
                                    case "children_fee":
                                        childrenFee = expenseAmount;
                                        break;
                                    case "breakfast_fee":
                                        breakfastFee = expenseAmount;
                                        break;
                                    case "minibar_fee":
                                        minibarFee = expenseAmount;
                                        break;
                                    case "room_service_fee":
                                        roomServiceFee = expenseAmount;
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching fees: " + ex.Message);
            }
        }


        // Update checkbox event handlers based on fetched data
        private void SetupUI()
        {
            // Set the event handlers with the values fetched from the database
            cocukCheckBox.CheckedChanged += (sender, e) => cucret = cocukCheckBox.Checked ? childrenFee : 0;
            kahvaltidahilCheckBox.CheckedChanged += (sender, e) => bucret = kahvaltidahilCheckBox.Checked ? breakfastFee : 0;
            minibarCheckBox.CheckedChanged += (sender, e) => mucret = minibarCheckBox.Checked ? minibarFee : 0;
            odaServisiCheckBox.CheckedChanged += (sender, e) => rucret = odaServisiCheckBox.Checked ? roomServiceFee : 0;
        }


        private void CheckAvailability(int roomNumber, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(baglanti.ConnectionString))
                {
                    connection.Open();
                    string query = @"
                SELECT check_availability(@roomNumber,@startDate::DATE,@endDate::DATE)";
                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@roomNumber", roomNumber);
                        command.Parameters.AddWithValue("@startDate", startDate.Date);
                        command.Parameters.AddWithValue("@endDate", endDate.Date);

                        int count = Convert.ToInt32(command.ExecuteScalar());
                        if (count == 0)
                        {
                            MessageBox.Show("Oda Seçili Tarihlerde Müsait.");
                        }
                        else
                        {
                            MessageBox.Show("Oda Seçili Tarihlerde Dolu.");
                            ucretHesaplaButton.Enabled = false;
                            rezervasyonYaptirButton.Enabled = false;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }


        private void rezervasyonYaptirButton_Click(object sender, EventArgs e)
        {
            switch (selectedRoomNumber)
            {
                case 1:
                    girisTarihi = dateTimeGiris1.Value;
                    cikisTarihi = dateTimeCikis1.Value;
                    break;
                case 2:
                    girisTarihi = dateTimeGiris2.Value;
                    cikisTarihi = dateTimeCikis2.Value;
                    break;
                case 3:
                    girisTarihi = dateTimeGiris3.Value;
                    cikisTarihi = dateTimeCikis3.Value;
                    break;
                case 4:
                    girisTarihi = dateTimeGiris4.Value;
                    cikisTarihi = dateTimeCikis4.Value;
                    break;
                case 5:
                    girisTarihi = dateTimeGiris5.Value;
                    cikisTarihi = dateTimeCikis5.Value;
                    break;
                case 6:
                    girisTarihi = dateTimeGiris6.Value;
                    cikisTarihi = dateTimeCikis6.Value;
                    break;
                case 7:
                    girisTarihi = dateTimeGiris7.Value;
                    cikisTarihi = dateTimeCikis7.Value;
                    break;

            }
            string customerName = nameBox.Text.Trim();
            double totalFee = Convert.ToDouble(feeBox.Text);

            // Tarih kontrolü
            if (cikisTarihi <= girisTarihi)
            {
                MessageBox.Show("Çıkış tarihi, giriş tarihinden büyük olmalıdır.");
                return;
            }

            if (string.IsNullOrEmpty(customerName))
            {
                MessageBox.Show("Lütfen isim giriniz.");
                return;
            }

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(baglanti.ConnectionString))
                {
                    connection.Open();

                    int customerId;

                    // Check if customer already exists
                    string checkCustomerQuery = "SELECT customer_id FROM customers_normalized WHERE isim_soyisim = @name LIMIT 1";
                    using (NpgsqlCommand command = new NpgsqlCommand(checkCustomerQuery, connection))
                    {
                        command.Parameters.AddWithValue("@name", customerName);
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            // If the customer already exists, get the existing customer_id
                            customerId = (int)result;
                        }
                        else
                        {
                            // If the customer does not exist, insert a new record and get the generated customer_id
                            string insertCustomerQuery = "INSERT INTO customers_normalized (isim_soyisim) VALUES (@name) RETURNING customer_id";
                            using (NpgsqlCommand insertCommand = new NpgsqlCommand(insertCustomerQuery, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@name", customerName);
                                customerId = (int)insertCommand.ExecuteScalar();  // Get the generated customer_id
                            }
                        }
                    }

                    // Insert reservation into reservations_normalized table
                    string insertReservationQuery = @"SELECT insert_reservation(@roomNumber,@customerId,@girisTarihi::DATE,@cikisTarihi::DATE)";
                    int reservationId;

                    using (NpgsqlCommand command = new NpgsqlCommand(insertReservationQuery, connection))
                    {
                        command.Parameters.AddWithValue("@roomNumber", selectedRoomNumber);
                        command.Parameters.AddWithValue("@customerId", customerId);
                        command.Parameters.AddWithValue("@girisTarihi", girisTarihi.Date);
                        command.Parameters.AddWithValue("@cikisTarihi", cikisTarihi.Date);
                        reservationId = (int)command.ExecuteScalar();  // Get the generated reservation_id
                    }

                    // Insert reservation expense into reservation_expenses_normalized table
                    string insertExpenseQuery = "call insert_reservation_expense(@reservationId, @expenseAmount)";

                    using (NpgsqlCommand command = new NpgsqlCommand(insertExpenseQuery, connection))
                    {
                        command.Parameters.AddWithValue("@reservationId", reservationId);
                        command.Parameters.AddWithValue("@expenseAmount", totalFee);  // Assuming totalFee is calculated
                        command.ExecuteNonQuery();
                    }

                    MessageBox.Show("Rezervasyon başarılı! Sizi arayacağız.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }


        private void nameBox_Click(object sender, EventArgs e)
        {
            nameBox.Text = "";
        }

        private void res1But_Click(object sender, EventArgs e)
        {
            customerName = nameBox.Text.Trim();
            girisTarihi = dateTimeGiris1.Value;
            cikisTarihi = dateTimeCikis1.Value;

            // Tarih kontrolü
            if (cikisTarihi <= girisTarihi)
            {
                MessageBox.Show("Çıkış tarihi, giriş tarihinden büyük olmalıdır.");
                return;
            }
            else
            {
                selectedRoomNumber = 1;
                CheckAvailability(selectedRoomNumber, dateTimeGiris1.Value, dateTimeCikis1.Value);
                if (string.IsNullOrEmpty(customerName))
                {
                    MessageBox.Show("Lütfen isim giriniz.");
                    return;
                }
                if (!string.IsNullOrEmpty(nameBox.Text))
                {
                    FetchRoomFees(selectedRoomNumber);
                    ucretHesaplaButton.Enabled = true;
                }
            }
        }

        private void res2But_Click(object sender, EventArgs e)
        {
            customerName = nameBox.Text.Trim();
            girisTarihi = dateTimeGiris2.Value;
            cikisTarihi = dateTimeCikis2.Value;

            // Tarih kontrolü
            if (cikisTarihi <= girisTarihi)
            {
                MessageBox.Show("Çıkış tarihi, giriş tarihinden büyük olmalıdır.");
                return;
            }
            else
            {
                selectedRoomNumber = 2;
                CheckAvailability(selectedRoomNumber, dateTimeGiris2.Value, dateTimeCikis2.Value);
                if (string.IsNullOrEmpty(customerName))
                {
                    MessageBox.Show("Lütfen isim giriniz.");
                    return;
                }
                if (!string.IsNullOrEmpty(nameBox.Text))
                {
                    FetchRoomFees(selectedRoomNumber);
                    ucretHesaplaButton.Enabled = true;
                }
            }
        }


        private void res3But_Click(object sender, EventArgs e)
        {
            customerName = nameBox.Text.Trim();
            girisTarihi = dateTimeGiris3.Value;
            cikisTarihi = dateTimeCikis3.Value;

            // Tarih kontrolü
            if (cikisTarihi <= girisTarihi)
            {
                MessageBox.Show("Çıkış tarihi, giriş tarihinden büyük olmalıdır.");
                return;
            }
            else
            {
                selectedRoomNumber = 3;
                CheckAvailability(selectedRoomNumber, dateTimeGiris3.Value, dateTimeCikis3.Value);
                if (string.IsNullOrEmpty(customerName))
                {
                    MessageBox.Show("Lütfen isim giriniz.");
                    return;
                }
                if (!string.IsNullOrEmpty(nameBox.Text))
                {
                    FetchRoomFees(selectedRoomNumber);
                    ucretHesaplaButton.Enabled = true;
                }
            }
        }
        private void res4But_Click(object sender, EventArgs e)
        {
            customerName = nameBox.Text.Trim();
            girisTarihi = dateTimeGiris4.Value;
            cikisTarihi = dateTimeCikis4.Value;

            // Tarih kontrolü
            if (cikisTarihi <= girisTarihi)
            {
                MessageBox.Show("Çıkış tarihi, giriş tarihinden büyük olmalıdır.");
                return;
            }
            else
            {
                selectedRoomNumber = 4;
                CheckAvailability(selectedRoomNumber, dateTimeGiris4.Value, dateTimeCikis4.Value);
                if (string.IsNullOrEmpty(customerName))
                {
                    MessageBox.Show("Lütfen isim giriniz.");
                    return;
                }
                if (!string.IsNullOrEmpty(nameBox.Text))
                {
                    FetchRoomFees(selectedRoomNumber);
                    ucretHesaplaButton.Enabled = true;
                }
            }
        }



        private void res5But_Click(object sender, EventArgs e)
        {
            customerName = nameBox.Text.Trim();
            girisTarihi = dateTimeGiris5.Value;
            cikisTarihi = dateTimeCikis5.Value;

            // Tarih kontrolü
            if (cikisTarihi <= girisTarihi)
            {
                MessageBox.Show("Çıkış tarihi, giriş tarihinden büyük olmalıdır.");
                return;
            }
            else
            {
                selectedRoomNumber = 5;
                CheckAvailability(selectedRoomNumber, dateTimeGiris5.Value, dateTimeCikis5.Value);
                if (string.IsNullOrEmpty(customerName))
                {
                    MessageBox.Show("Lütfen isim giriniz.");
                    return;
                }
                if (!string.IsNullOrEmpty(nameBox.Text))
                {
                    FetchRoomFees(selectedRoomNumber);
                    ucretHesaplaButton.Enabled = true;
                }
            }
        }
        private void res6But_Click(object sender, EventArgs e)
        {
            customerName = nameBox.Text.Trim();
            girisTarihi = dateTimeGiris6.Value;
            cikisTarihi = dateTimeCikis6.Value;

            // Tarih kontrolü
            if (cikisTarihi <= girisTarihi)
            {
                MessageBox.Show("Çıkış tarihi, giriş tarihinden büyük olmalıdır.");
                return;
            }
            else
            {
                selectedRoomNumber = 6;
                CheckAvailability(selectedRoomNumber, dateTimeGiris6.Value, dateTimeCikis6.Value);
                if (string.IsNullOrEmpty(customerName))
                {
                    MessageBox.Show("Lütfen isim giriniz.");
                    return;
                }
                if (!string.IsNullOrEmpty(nameBox.Text))
                {
                    FetchRoomFees(selectedRoomNumber);
                    ucretHesaplaButton.Enabled = true;
                }
            }
        }
        private void res7But_Click(object sender, EventArgs e)
        {
            customerName = nameBox.Text.Trim();
            girisTarihi = dateTimeGiris7.Value;
            cikisTarihi = dateTimeCikis7.Value;

            // Tarih kontrolü
            if (cikisTarihi <= girisTarihi)
            {
                MessageBox.Show("Çıkış tarihi, giriş tarihinden büyük olmalıdır.");
                return;
            }
            else
            {
                selectedRoomNumber = 7;
                CheckAvailability(selectedRoomNumber, dateTimeGiris7.Value, dateTimeCikis7.Value);
                if (string.IsNullOrEmpty(customerName))
                {
                    MessageBox.Show("Lütfen isim giriniz.");
                    return;
                }
                if (!string.IsNullOrEmpty(nameBox.Text))
                {
                    FetchRoomFees(selectedRoomNumber);
                    ucretHesaplaButton.Enabled = true;
                }
            }
        }


        private void incrementofBasePrice(int selectedRoomNumber)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(baglanti.ConnectionString))
                {
                    connection.Open();

                    // Fetch the base price from the room_normalized table
                    string query = "SELECT base_price FROM room_normalized WHERE oda_numarasi = @selectedRoomNumber";

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@selectedRoomNumber", selectedRoomNumber);  // Room number
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            // Fetch base price and calculate the duration of the stay
                            double basePrice = Convert.ToDouble(result);
                            TimeSpan dateDifference = cikisTarihi - girisTarihi;  // Duration in days
                            double durationInDays = dateDifference.TotalDays;
                            durationInDays = Math.Round(durationInDays, 0);

                            Console.WriteLine($"Duration in days: {durationInDays}");

                            // Adjust price based on the duration
                            double adjustedPrice = basePrice * durationInDays;
                            adjustedPrice = Math.Round(adjustedPrice, 0);  // Round to nearest integer

                            ucret = adjustedPrice;  // Update the total price with the adjusted base price

                            feeBox.Text = ucret.ToString();  // Display the total fee in the feeBox
                        }
                        else
                        {
                            MessageBox.Show("Base price could not be fetched.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error calculating price: " + ex.Message);
            }
        }


        private void ucretHesaplaButton_Click(object sender, EventArgs e)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(baglanti.ConnectionString))
                {
                    connection.Open();

                    // Call the dynamic price calculation function (dynamic_price)
                    string dynamicPriceFuncQuery = "SELECT dynamic_price(@roomNumber, @girisTarihi::DATE, @cikisTarihi::DATE)";
                    double dynamicPrice = 0.0;

                    using (NpgsqlCommand command = new NpgsqlCommand(dynamicPriceFuncQuery, connection))
                    {
                        command.Parameters.AddWithValue("@roomNumber", selectedRoomNumber);
                        command.Parameters.AddWithValue("@girisTarihi", girisTarihi.Date);
                        command.Parameters.AddWithValue("@cikisTarihi", cikisTarihi.Date);

                        object result = command.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            dynamicPrice = Convert.ToDouble(result);
                        }
                        else
                        {
                            throw new Exception("Dynamic price hesaplanamadı.");
                        }
                    }

                    // Call the fee_calculator_normalized function to get the fee
                    string feeQuery = "SELECT fee_calculator_normalized(@roomNumber, @customerName)";
                    double feePrice = 0.0;

                    using (NpgsqlCommand feeCommand = new NpgsqlCommand(feeQuery, connection))
                    {
                        feeCommand.Parameters.AddWithValue("@roomNumber", selectedRoomNumber);
                        feeCommand.Parameters.AddWithValue("@customerName", customerName);

                        object feeResult = feeCommand.ExecuteScalar();

                        if (feeResult != null && feeResult != DBNull.Value)
                        {
                            feePrice = Convert.ToDouble(feeResult);
                        }
                        else
                        {
                            throw new Exception("Fee calculator fonksiyonu sonucunda değer döndürülmedi.");
                        }
                    }

                    // Apply additional charges for selected services
                    if (cocukCheckBox.Checked) feePrice += childrenFee;
                    if (kahvaltidahilCheckBox.Checked) feePrice += breakfastFee;
                    if (minibarCheckBox.Checked) feePrice += minibarFee;
                    if (odaServisiCheckBox.Checked) feePrice += roomServiceFee;

                    // Calculate the total price and display in the feeBox
                    double totalPrice = dynamicPrice + feePrice;
                    feeBox.Text = totalPrice.ToString("F2");

                    // Enable the reservation button
                    rezervasyonYaptirButton.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }













    }
}
    