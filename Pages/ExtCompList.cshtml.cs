using dashboard.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace dashboard.Pages;
public class ExtCompList : PageModel
{ 
    public class IpInfo
    {
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
    public void OnGet()
    { 
        var temp = new CallsViewModel(1, DateOnly.FromDateTime(DateTime.Now), "10.20.30", "no clue", 55, 9022);
        var temp2 = new CallsViewModel(1, DateOnly.FromDateTime(DateTime.Now), "10.20.30", "no clue", 25, 9022);
        var temp4 = new CallsViewModel(1, DateOnly.FromDateTime(DateTime.Now), "10.20.30", "no clue", 25, 9022);
        var temp5 = new CallsViewModel(1, DateOnly.FromDateTime(DateTime.Now), "11.20.30", "no clue", 25, 9022);
        var temp3 = new CallsViewModel(1, DateOnly.FromDateTime(DateTime.Now), "11.20.30", "no clue", 50, 9022);
        Calls.Add(temp);
        Calls.Add(temp2);
        Calls.Add(temp3);
        Calls.Add(temp4);
        Calls.Add(temp5);
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
    }
}