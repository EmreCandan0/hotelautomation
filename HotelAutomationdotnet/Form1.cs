using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Npgsql;

namespace HotelAutomationdotnet
{
    public partial class LoginPanel : Form
    {
        private string username, password;

        public LoginPanel()
        {
            InitializeComponent();
        }
        private void LoginPanel_Load(object sender, EventArgs e)
        {

        }

        NpgsqlConnection baglanti = new NpgsqlConnection("Host=localhost;Username=postgres;Password=password;Database=temp_proje");

        private void loginButton_Click(object sender, EventArgs e)
        {
            
            username = usernameTextBox.Text.Trim();
            password = passwordTextBox.Text.Trim();
          

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Kullanıcı adı ve Şifre Giriniz");
                return;
            }

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(baglanti.ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT şifre FROM admin WHERE admin = @username";
                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);

                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())

                            {
                                string storedHashedPassword = reader.GetString(0); // Database'den gelen hash

                                // Şifreyi hash'leyip karşılaştırma
                                string hashedInputPassword = HashingHelper.HashPassword(password);

                                if (hashedInputPassword == storedHashedPassword)
                                {
                                    MessageBox.Show("Giriş Başarılı");
                                    this.Hide();
                                    AdminPanel adminPanel = new AdminPanel();
                                    adminPanel.FormClosed += (s, args) => this.Show();
                                    adminPanel.Show();
                                }
                                else
                                {
                                    MessageBox.Show("Yanlış Kullanıcı Adı veya Şifre");
                                    usernameTextBox.Clear();
                                    passwordTextBox.Clear();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Kullanıcı bulunamadı");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void passwordTextBox_Click(object sender, EventArgs e)
        {
            passwordTextBox.Text = "";
        }
        private void usernameTextBox_Click(object sender, EventArgs e)
        {
            usernameTextBox.Text = "";
        }
        private void checkoutButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            HotelAutomationdotnet.CheckoutPanel checkoutPanel = new HotelAutomationdotnet.CheckoutPanel();
            checkoutPanel.FormClosed += (s, args) => this.Show();
            checkoutPanel.Show();
        }
        private void rezervasyonYapButton_Click(object sender, EventArgs e)
        {
            // Otel Odası Seçme Panelinin açılması
            this.Hide();
            HotelAutomationdotnet.HotelRoomSelectionPanel hotelRoomSelectionPanel = new HotelAutomationdotnet.HotelRoomSelectionPanel();
            hotelRoomSelectionPanel.FormClosed += (s, args) => this.Show();
            hotelRoomSelectionPanel.Show();
        }
        public void SaveHashedPassword(string username, string password)
        {
            string hashedPassword = HashingHelper.HashPassword(password);

            string query = "INSERT INTO admin (admin, şifre) VALUES (@username, @hashedPassword)";

            using (var connection = new NpgsqlConnection("Host=localhost;Username=postgres;Password=password;Database=temp_proje"))
            {
                connection.Open();

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@hashedPassword", hashedPassword);

                    command.ExecuteNonQuery();
                }
            }
        }

    }

    

    public static class HashingHelper
    {
        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
