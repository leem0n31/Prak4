using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Data.SqlClient;

namespace WpfApp1
{
    public partial class AddCategoryWindow : Window
    {
        private string connectionString;
        public AddCategoryWindow(string connectionString)
        {
            InitializeComponent();
            this.connectionString = connectionString;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            string name = txtName.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Введите название вотчлиста.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("INSERT INTO Watchlist (UserID) VALUES (@UserID)", connection))
                    {
                        command.Parameters.AddWithValue("@UserID", name);
                        command.ExecuteNonQuery();
                    }
                }
                DialogResult = true;
                Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Ошибка добавления вотчлиста: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}