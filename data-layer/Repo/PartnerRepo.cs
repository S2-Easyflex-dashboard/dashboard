using MySql.Data.MySqlClient;
namespace data_layer
{
    public class PartnerRepo
    {
        private string connectionString = "server=192.168.133.6;Database=s2group;User Id=dashboard;Password=1234;";

        public List<PartnerDTO> GetAllPartnersByIp(List<string> ipAdress)
        {
            List<PartnerDTO> partnerDTOList = [];
            string commandParts = @"SELECT * FROM partners WHERE 1 = 1";
            foreach(string ip in ipAdress)
            {
                commandParts = commandParts + " OR ip_adress = \"" + ip + "\"";
            }
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            using var cmd = new MySqlCommand(
                commandParts, conn
            );
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                partnerDTOList.Add(new PartnerDTO(reader.GetInt32(0), reader.GetString(1), reader.GetString(2)));
            }
            conn.Close();
            return partnerDTOList;
        }
    }
}
