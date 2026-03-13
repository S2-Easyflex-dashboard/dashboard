using dashboard.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace dashboard.Pages;
public class ExtCompList : PageModel
{ 
    public class DuplicateIpInfo
    {
        public string Ip {get; private set;}
        public int Amount {get; private set;}
        public string Service {get; private set;}
        public DuplicateIpInfo(string ip, string service)
        {
            Ip = ip;
            Amount = 2;
            Service = service;
        }
        public void AddToCount(int number)
        {
            Amount = Amount + number;
        }
    }
    public List<CallsViewModel> Calls { get; private set; } = new();
    public List<string> UniqueIps { get; private set; } = new();
    public List<DuplicateIpInfo> DuplicateIps {get; private set; } = new();
    public float duplicatePercent {get; private set;}
    public float uniquePercent {get; private set;}
    public void OnGet()
    { 
        var temp = new CallsViewModel(1, DateOnly.FromDateTime(DateTime.Now), "10.20.30", "no clue", 55, 9022);
        var temp2 = new CallsViewModel(1, DateOnly.FromDateTime(DateTime.Now), "10.20.30", "no clue", 25, 9022);
        var temp4 = new CallsViewModel(1, DateOnly.FromDateTime(DateTime.Now), "10.20.30", "no clue", 25, 9022);
        var temp5 = new CallsViewModel(1, DateOnly.FromDateTime(DateTime.Now), "10.20.30", "no clue", 25, 9022);
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
                if(ip == call.Ip)
                {
                    duplicate = true;
                    foreach(var duplicateIp in DuplicateIps)
                    {
                        if(duplicateIp.Ip == call.Ip && duplicateIp.Service == call.Service)
                        {
                            duplicateIp.AddToCount(call.Amount);
                            found = true;
                        }
                    }
                    if(!found)
                    {
                        DuplicateIps.Add(new DuplicateIpInfo(call.Ip, call.Service));
                    }
                }
            }
            if(!duplicate)
            {
                UniqueIps.Add(call.Ip);
            }
        }
        foreach(var ip in DuplicateIps)
        {
            foreach(var UniqueIp in UniqueIps)
            {
                if(UniqueIp == ip.Ip)
                {
                    isDouble = true;
                }
            }
            if(isDouble)
            {
                UniqueIps.Remove(ip.Ip);
            }
            duplicatePercent = (float.Parse(DuplicateIps.Count()) / (UniqueIps.Count() + DuplicateIps.Count())) * 100;
            uniquePercent = (UniqueIps.Count() / (DuplicateIps.Count() + UniqueIps.Count())) * 100;
        }
    }
}