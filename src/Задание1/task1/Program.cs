using System;
using System.Data.SqlClient;

class Program
{
    static string connectionString = "Server=sql.bsite.net\\MSSQL2016;Database=leemon_;User Id=leemon_;Password=11111;TrustServerCertificate=true;";

    static void Main(string[] args)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Работа с таблицей Users.");
            Console.WriteLine("Выберите действие:");
            Console.WriteLine("1) Посмотреть все записи");
            Console.WriteLine("2) Добавить нового пользователя");
            Console.WriteLine("3) Обновить существующего пользователя");
            Console.WriteLine("4) Удалить существующего пользователя");
            Console.WriteLine("0) Выход");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ViewAllUsers();
                    break;
                case "2":
                    AddUser();
                    break;
                case "3":
                    UpdateUser();
                    break;
                case "4":
                    DeleteUser();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Неверный выбор. Попробуйте снова.");
                    break;
            }
        }
    }

    static void ViewAllUsers()
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            SqlCommand command = new SqlCommand("SELECT UserID, Username, Email, CreatedAt FROM Users WHERE DeletedAt IS NULL", connection);
            SqlDataReader reader = command.ExecuteReader();

            Console.Clear();
            Console.WriteLine("Список пользователей:");
            Console.WriteLine("ID\tUsername\tEmail\t\tCreatedAt");

            while (reader.Read())
            {
                Console.WriteLine($"{reader["UserID"]}\t{reader["Username"]}\t{reader["Email"]}\t{reader["CreatedAt"]}");
            }

            reader.Close();
        }

        Console.WriteLine("\nНажмите на любую клавишу чтобы вернуться.");
        Console.ReadKey();
    }

    static void AddUser()
    {
        Console.Clear();
        Console.WriteLine("Добавление пользователя:");
        Console.WriteLine("Введите следующие данные через запятую: Username,Email,Password");

        string input = Console.ReadLine();
        var data = input.Split(',');

        if (data.Length != 3)
        {
            Console.WriteLine("Неверный формат, повторите еще раз.");
            Console.ReadKey();
            return;
        }

        string username = data[0].Trim();
        string email = data[1].Trim();
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(data[2].Trim());

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            SqlCommand command = new SqlCommand("INSERT INTO Users (Username, Email, PasswordHash) OUTPUT INSERTED.UserID VALUES (@Username, @Email, @PasswordHash)", connection);
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@Email", email);
            command.Parameters.AddWithValue("@PasswordHash", passwordHash);

            int newUserId = (int)command.ExecuteScalar();
            Console.WriteLine($"Добавление успешно! Его Id - {newUserId}.");
        }
        Console.WriteLine("\nНажмите на любую клавишу чтобы вернуться.");
        Console.ReadKey();
    }
   
    
    static void UpdateUser()
    {
        Console.Clear();
        Console.WriteLine("Обновление пользователя:");
        Console.WriteLine("Введите ID пользователя, которого хотите обновить:");

        if (!int.TryParse(Console.ReadLine(), out int userId))
        {
            Console.WriteLine("Неверный ID. Попробуйте еще раз.");
            return;
        }

        Console.WriteLine("Введите новые данные через запятую (Username, Email, Password):");
        string input = Console.ReadLine();
        string[] data = input.Split(',');

        if (data.Length != 3)
        {
            Console.WriteLine("Неверный формат, повторите еще раз.");
            return;
        }

        string username = data[0].Trim();
        string email = data[1].Trim();
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(data[2].Trim());

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string query = "UPDATE Users SET Username = @Username, Email = @Email, PasswordHash = @PasswordHash WHERE UserID = @UserID AND DeletedAt IS NULL";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserID", userId);
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@Email", email);
            command.Parameters.AddWithValue("@PasswordHash", passwordHash);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                Console.WriteLine("Данные пользователя успешно обновлены.");
            }
            else
            {
                Console.WriteLine("Пользователь не найден или удален.");
            }
        }

        Console.WriteLine("\nНажмите любую клавишу, чтобы вернуться.");
        Console.ReadKey();
    }


    static void DeleteUser()
    {
        Console.Clear();
        Console.WriteLine("Удаление пользователя:");
        Console.WriteLine("Введите ID пользователя:");

        if (!int.TryParse(Console.ReadLine(), out int userId))
        {
            Console.WriteLine("Неверный формат ID.");
            Console.ReadKey();
            return;
        }

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            SqlCommand command = new SqlCommand("UPDATE Users SET DeletedAt = GETDATE() WHERE UserID = @UserID AND DeletedAt IS NULL", connection);
            command.Parameters.AddWithValue("@UserID", userId);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
                Console.WriteLine("Пользователь успешно удален!");
            else
                Console.WriteLine("Пользователь не найден или уже удален.");
        }

        Console.WriteLine("Нажмите любую клавишу чтобы вернуться.");
        Console.ReadKey();
    }

  
}
