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
using System.Data.SqlClient;

namespace WpfApp1
{
    public partial class UpdateCategoryWindow : Window
    {
        private string connectionString;
        private int categoryId;

        public UpdateCategoryWindow(string connectionString, int categoryId, string currentName)
        {
            InitializeComponent();
            this.connectionString = connectionString;
            this.categoryId = categoryId;
            txtCategoryID.Text = categoryId.ToString();
            txtName.Text = currentName;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
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
                    using (SqlCommand command = new SqlCommand("UPDATE Watchlist SET UserID = @UserID WHERE WatchlistID = @WatchlistID", connection))
                    {
                        command.Parameters.AddWithValue("@UserID", name);
                        command.Parameters.AddWithValue("@WatchlistID", categoryId);
                        command.ExecuteNonQuery();
                    }
                }
                DialogResult = true;
                Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Ошибка обновления вотчлиста: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}