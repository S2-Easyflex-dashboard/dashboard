using dashboard.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace dashboard.Pages
{
    public class IndexModel : PageModel
    {
        public List<CallsViewModel> Calls { get; private set; } = new();
        public float ExternalCustomer { get; private set; } = 0;
        public float InternalCustomers { get; private set; } = 0;
        public float ExternalPercent { get; private set; }
        public float InternalPercent { get; private set; }
        public void OnGet()
        {
            var temp = new CallsViewModel(1, DateOnly.FromDateTime(DateTime.Now), "10.20.30", "no clue", 55, 9022);
            var temp2 = new CallsViewModel(1, DateOnly.FromDateTime(DateTime.Now), "13.20.30", "no clue", 25, 9022);
            var temp3 = new CallsViewModel(1, DateOnly.FromDateTime(DateTime.Now), "20.10.30", "no clue", 50, 9022);
            Calls.Add(temp);
            Calls.Add(temp2);
            Calls.Add(temp3);
            foreach (var call in Calls)
            {
                if (call.Ip.Split(".")[0] == "10")
                {
                    InternalCustomers = InternalCustomers + call.Amount;
                }
                else
                {
                    ExternalCustomer = ExternalCustomer + call.Amount;
                }
            }
            ExternalPercent = (ExternalCustomer / (ExternalCustomer + InternalCustomers)) * 100;
            InternalPercent = (InternalCustomers / (InternalCustomers + ExternalCustomer)) * 100;
        }
    }
}
