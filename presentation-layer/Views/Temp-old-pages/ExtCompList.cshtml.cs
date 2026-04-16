using dashboard.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace dashboard.Pages;
public class ExtCompList : PageModel
{
    private string connectionString = "server=192.168.133.6;Database=s2group;User Id=dashboard;Password=1234;";
    public class IpInfo
    {
        // public List<IpAdress> Ip {get; private set;} = new();
        public string Ip { get; set; }
        public int Amount { get; private set; }
        public List<int> CustomerIds { get; private set; }
        public List<string> CustomerNames { get; private set; } = new();
        public IpInfo(string ip, int amount, int customerIds)
        {
            Ip = ip;
            Amount = amount;
            CustomerIds = [customerIds];
        }
        public void AddToCount(int number, int? customerIds)
        {
            Amount = Amount + number;
            if (customerIds != null)
            {
                CustomerIds.Add((int)customerIds);
            }
        }
    }
    public List<CallsViewModel> Calls { get; private set; } = new();
    public List<IpInfo> UniqueIps { get; private set; } = new();
    public List<IpInfo> DuplicateIps {get; private set; } = new();
    public float duplicatePercent {get; private set;}
    public float uniquePercent {get; private set;}
    public void OnGet()
    {
        using var conn = new MySqlConnection(connectionString);
        conn.Open();
        using var cmd = new MySqlCommand(
            @"SELECT * FROM calls", conn
        );
        using var reader = cmd.ExecuteReader();
        if (reader.HasRows)
        {
            while (reader.Read())
            {
                Calls.Add(new CallsViewModel(reader.GetInt32(6), DateOnly.Parse(reader.GetString(1)), reader.GetString(2), reader.GetString(3), reader.GetInt32(4), reader.GetInt32(5)));
            }
        }
        conn.Close();

        //sorting ip's on if they are used by multiple customers
        foreach(var call in Calls)
        {
            IpInfo? duplicateIp = null;
            bool duplicateAlreadyFound = false;
            foreach (var ipInformation in DuplicateIps)
            {
                if (call.Ip == ipInformation.Ip && !ipInformation.CustomerIds.Contains(call.CustomerId))
                {
                    ipInformation.AddToCount(call.Amount, call.CustomerId);
                    duplicateAlreadyFound = true;
                }
                else if (call.Ip == ipInformation.Ip && ipInformation.CustomerIds.Contains(call.CustomerId))
                {
                    ipInformation.AddToCount(call.Amount, null);
                    duplicateAlreadyFound = true;
                }
            }
            if (!duplicateAlreadyFound)
            {
                foreach (var ipInformation in UniqueIps)
                {
                    if (call.Ip == ipInformation.Ip && !ipInformation.CustomerIds.Contains(call.CustomerId))
                    {
                        ipInformation.AddToCount(call.Amount, call.CustomerId);
                        duplicateIp = ipInformation;
                    }
                    else if (call.Ip == ipInformation.Ip && ipInformation.CustomerIds.Contains(call.CustomerId))
                    {
                        ipInformation.AddToCount(call.Amount, null);
                    }
                }
            }
            if (duplicateIp != null)
            {
                DuplicateIps.Add(duplicateIp);
                UniqueIps.Remove(duplicateIp);
            }
            else
            {
                UniqueIps.Add(new IpInfo(call.Ip, call.Amount, call.CustomerId));
            }
        }

        // replacing id and ip's with names
        foreach (var ipInfo in DuplicateIps)
        {
            foreach (var id in ipInfo.CustomerIds)
            {
                using var connGetName = new MySqlConnection(connectionString);
                connGetName.Open();
                using var cmdGetName = new MySqlCommand(
                    @"SELECT name FROM customers WHERE id = @id", connGetName
                );
                cmdGetName.Parameters.AddWithValue("@id", id);
                using var readerGetName = cmdGetName.ExecuteReader();
                if (readerGetName.Read())
                {
                    ipInfo.CustomerNames.Add(readerGetName.GetString(0));
                }
                connGetName.Close();
            }
            using var connGetIpName = new MySqlConnection(connectionString);
            connGetIpName.Open();
            using var cmdGetIpName = new MySqlCommand(
                @"SELECT partner_name FROM partners WHERE ip_adress = @ip_adress", connGetIpName
            );
            cmdGetIpName.Parameters.AddWithValue("@ip_adress", ipInfo.Ip);
            using var readerGetIpName = cmdGetIpName.ExecuteReader();
            if (readerGetIpName.Read())
            {
                if (!readerGetIpName.IsDBNull(0))
                {
                    ipInfo.Ip = readerGetIpName.GetString(0);
                }
                connGetIpName.Close();
            }
        }
    }
}
