using MySql.Data.MySqlClient;

namespace data_layer
{
    public class CallRepo
    {
        private string connectionString = "server=192.168.133.6;Database=s2group;User Id=dashboard;Password=1234;";
        
        public List<CallDTO> GetAllCalls()
        {
            List<CallDTO> callDTOList = [];
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            using var cmd = new MySqlCommand(
                @"SELECT * FROM calls", conn
            );
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                callDTOList.Add(new CallDTO(reader.GetInt32(6), DateOnly.Parse(reader.GetString(1)), reader.GetString(2), reader.GetString(3), reader.GetInt32(4), reader.GetInt32(5)));
            }
            conn.Close();
            return callDTOList;
        }
    }
}