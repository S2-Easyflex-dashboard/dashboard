using dashboard.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Runtime.CompilerServices;

namespace dashboard.Pages;
public class ExtCompList : PageModel
{
    private string connectionString = "server=192.168.133.6;Database=s2group;User Id=dashboard;Password=1234;";
    public class IpInfo
    {
        // public List<IpAdress> Ip {get; private set;} = new();
        public string Ip { get; private set; }
        public int Amount { get; private set; }
        public List<int> Users { get; private set; }
        public IpInfo(string ip, int amount, int user)
        {
            Ip = ip;
            Amount = amount;
            Users = [user];
        }
        public void AddToCount(int number, int? user)
        {
            Amount = Amount + number;
            if (user != null)
            {
                Users.Add((int)user);
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
        if (!reader.HasRows)
        {
            conn.Close();
            return;
        }
        while (reader.Read())
        {
            Calls.Add(new CallsViewModel(reader.GetInt32(6), DateOnly.Parse(reader.GetString(1)), reader.GetString(2), reader.GetString(3), reader.GetInt32(4), reader.GetInt32(5)));
        }
        conn.Close();
        foreach(var call in Calls)
        {
            IpInfo? duplicateIp = null;
            bool duplicateAlreadyFound = false;
            foreach (var ipInformation in DuplicateIps)
            {
                if (call.Ip == ipInformation.Ip && !ipInformation.Users.Contains(call.CustomerId))
                {
                    ipInformation.AddToCount(call.Amount, call.CustomerId);
                    duplicateAlreadyFound = true;
                }
                else if (call.Ip == ipInformation.Ip && ipInformation.Users.Contains(call.CustomerId))
                {
                    ipInformation.AddToCount(call.Amount, null);
                    duplicateAlreadyFound = true;
                }
            }
            if (!duplicateAlreadyFound)
            {
                foreach (var ipInformation in UniqueIps)
                {
                    if (call.Ip == ipInformation.Ip && !ipInformation.Users.Contains(call.CustomerId))
                    {
                        ipInformation.AddToCount(call.Amount, call.CustomerId);
                        duplicateIp = ipInformation;
                    }
                    else if (call.Ip == ipInformation.Ip && ipInformation.Users.Contains(call.CustomerId))
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
    }
}
