using MySql.Data.MySqlClient;
namespace data_layer
{
    public class CustomerRepo
    {
        private string connectionString = "server=192.168.133.6;Database=s2group;User Id=dashboard;Password=1234;";

        public List<CustomerDTO> GetAllCustomersByIds(List<int> customerIds)
        {
            List<CustomerDTO> customerDTOList = [];
            string commandParts = @"SELECT * FROM customers WHERE 1 = 1";
            foreach (int id in customerIds)
            {
                commandParts = commandParts + " OR id = " + id;
            }
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            using var cmd = new MySqlCommand(
                commandParts, conn
            );
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                customerDTOList.Add(new CustomerDTO(reader.GetInt32(0), reader.GetString(1)));
            }
            conn.Close();
            return customerDTOList;
        }
    }
}
