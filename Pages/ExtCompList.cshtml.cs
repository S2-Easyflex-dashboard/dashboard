using dashboard.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace dashboard.Pages;
public class ExtCompList : PageModel
{ 
    private string connectionString = "server=192.168.133.6;Database=s2group;User Id=dashboard;Password=1234;";
    public class IpInfo
    {
        // public List<IpAdress> Ip {get; private set;} = new();
        public string Ip {get; private set;}
        public int Amount {get; private set;}
        public string Service {get; private set;}
        public List<int> Users {get; private set;}
        public IpInfo(string ip, int amount, string service, int user)
        {
            Ip = ip;
            Amount = amount;
            Service = service;
            Users = [user];
        }
        public void AddToCount(int number, int user)
        {
            Amount = Amount + number;
            Users.Add(user);
        }
    }
    public List<CallsViewModel> Calls { get; private set; } = new();
    public List<IpInfo> UniqueIps { get; private set; } = new();
    public List<IpInfo> DuplicateIps {get; private set; } = new();
    public float duplicatePercent {get; private set;}
    public float uniquePercent {get; private set;}
    public Task<IActionResult> OnGet()
    { 
        using var conn = new MySqlConnection(connectionString);
        conn.OpenAsync();
        using var cmd = new MySqlCommand(
            @"SELECT * FROM calls", conn
        );
        using var reader = cmd.ExecuteReaderAsync();
        if(!reader.HasRows)
        {
            conn.Close();
            return Page();
        }
        while (reader.Read())
        {
            Calls.Add(new CallsViewModel(reader.GetInt32(6), DateOnly.Parse(reader.GetString(1)), reader.GetString(2), reader.GetString(3), reader.GetInt32(4), reader.GetInt32(5)));
        }
        conn.Close();

        
        
        
        bool found = false;
        bool duplicate = false;
        bool isDouble = false;
        foreach (var call in Calls)
        {
            duplicate = false;
            foreach (var ip in UniqueIps)
            {
                found = false;
                if(ip.Ip == call.Ip)
                {
                    duplicate = true;
                    foreach(var duplicateIp in DuplicateIps)
                    {
                        if(duplicateIp.Ip == call.Ip && duplicateIp.Service == call.Service)
                        {
                            duplicateIp.AddToCount(call.Amount, call.CustomerId);
                            found = true;
                        }
                    }
                    if(!found)
                    {
                        DuplicateIps.Add(new IpInfo(call.Ip, (call.Amount + ip.Amount), call.Service, call.CustomerId));
                    }
                }
            }
            if(!duplicate)
            {
                UniqueIps.Add(new IpInfo(call.Ip, call.Amount, call.Service, call.CustomerId));
            }
        }
        foreach(var ip in DuplicateIps)
        {
            foreach(var UniqueIp in UniqueIps)
            {
                if(UniqueIp.Ip == ip.Ip)
                {
                    isDouble = true;
                }
            }
            if(isDouble)
            {
                UniqueIps.Remove(ip);
            }
            duplicatePercent = ((float)DuplicateIps.Count() / ((float)UniqueIps.Count() + (float)DuplicateIps.Count())) * 100;
            uniquePercent = ((float)UniqueIps.Count() / ((float)DuplicateIps.Count() + (float)UniqueIps.Count())) * 100;
        }
        return Page();
    }
}
