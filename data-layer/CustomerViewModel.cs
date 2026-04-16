using MySql.Data.MySqlClient;

namespace dashboard.ViewModels
{
    public class CustomerViewModel
    {
        private string connectionString = "server=192.168.133.6;Database=s2group;User Id=dashboard;Password=1234;";
        public int Id { get; private set; }
        public string Name { get; private set; }

        public CustomerViewModel(int CustomerId)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            using var cmd = new MySqlCommand(
                @"SELECT name FROM customers WHERE id = @id", conn
            );
            cmd.Parameters.AddWithValue("@id", CustomerId);
            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return;
            }
            Id = CustomerId;
            Name = reader.GetString(0);
            conn.Close();
        }
    }
}
