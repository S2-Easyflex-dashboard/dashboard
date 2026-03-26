using dashboard.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Linq;


namespace dashboard.Pages
{
    public class IndexModel : PageModel
    {
        private string connectionString = "Server= 192.168.133.6;Database=s2group;User Id=dashboard;Password=1234;";
        public List<CallsViewModel> Calls { get; private set; } = new();
        public float ExternalCustomer { get; private set; } = 0;
        public float InternalCustomers { get; private set; } = 0;
        public float ExternalPercent { get; private set; }
        public float InternalPercent { get; private set; }
        //each user will have the index of their id minus one (for example user 1 will have the index 0)
        public float[] CallsPerCustomer { get; private set; } = [0, 0, 0, 0, 0];
        //callsperday will first contain all calls made on a specific weekday, but after all have been added it will have the average instead (the index for this on counts up through the days, starting at sunday)
        public float[] CallsPerDay { get; private set; } = [0, 0, 0, 0, 0, 0, 0,];
        public List<DateOnly>[] UniqueDatesByDay { get; private set; } = [new(), new(), new(), new(), new(), new(), new()];
        public float ManagingLevel { get; private set; }
        public float RelationLevel { get; private set; }
        public float TempHireLevel { get; private set; }
        public float ManagingLevelPercent { get; private set; }
        public float RelationLevelPercent { get; private set; }
        public float TempHireLevelPercent { get; private set; }
        public bool RfFilterRelation { get; private set; }
        public bool RfFilterTempHire { get; private set; }
        public int? CustomerFilter {  get; private set; }
        public string? ServiceFilter { get; private set; }


        public async Task<IActionResult> OnGet(string[]? rfFilter, int? customerFilter, string? serviceFilter)
        {
            if (rfFilter.Contains("temphire"))
            {
                RfFilterTempHire = true;
            }

            if (rfFilter.Contains("relation"))
            {
                RfFilterRelation = true;
            }
            if (customerFilter != null)
            {
                CustomerFilter = customerFilter;
            }
            if (serviceFilter != null) 
            {
                ServiceFilter = serviceFilter;
            }
            await using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();
            await using var cmd = new MySqlCommand(
                @"SELECT * FROM calls", conn
            );
            await using var reader = await cmd.ExecuteReaderAsync(); // here
            while (await reader.ReadAsync())
            {
                Calls.Add(new CallsViewModel(reader.GetInt32(6), DateOnly.Parse(reader.GetString(1)), reader.GetString(2), reader.GetString(3), reader.GetInt32(4), reader.GetInt32(5)));
            }
            conn.Close();


            foreach (var call in Calls)
            {
                if ((CustomerFilter == call.CustomerId || CustomerFilter == null ) && (ServiceFilter == call.Service || ServiceFilter == null))
                {
                    CallsPerCustomer[call.CustomerId - 1] += call.Amount;
                    CallsPerDay[(int)call.Date.DayOfWeek] += call.Amount;
                    if (!UniqueDatesByDay[(int)call.Date.DayOfWeek].Contains(call.Date))
                    {
                        UniqueDatesByDay[(int)call.Date.DayOfWeek].Add(call.Date);
                    }
                }

                if (call.Ip.Split(".")[0] == "10")
                {
                    InternalCustomers = InternalCustomers + call.Amount;
                }
                else
                {
                    ExternalCustomer = ExternalCustomer + call.Amount;
                }

                if (call.Service.Contains("_fw_") || (RfFilterTempHire && call.Service.Contains("_rf_")))
                {
                    TempHireLevel += call.Amount;
                    if(RfFilterRelation && call.Service.Contains("_rf_"))
                    {
                        RelationLevel += call.Amount;

                    }
                }
                else if (call.Service.Contains("_rl_") || (RfFilterRelation && call.Service.Contains("_rf_")))
                {
                    RelationLevel += call.Amount;
                }
                else if (call.Service.Contains("_wm_") || call.Service.Contains("_bi_"))
                {
                    ManagingLevel += call.Amount;
                }
            }
            ManagingLevelPercent = (ManagingLevel / (ManagingLevel + RelationLevel + TempHireLevel)) * 100;
            RelationLevelPercent = (RelationLevel / (ManagingLevel + RelationLevel + TempHireLevel)) * 100;
            TempHireLevelPercent = (TempHireLevel / (ManagingLevel + RelationLevel + TempHireLevel)) * 100;
            ExternalPercent = (ExternalCustomer / (ExternalCustomer + InternalCustomers)) * 100;
            InternalPercent = (InternalCustomers / (InternalCustomers + ExternalCustomer)) * 100;
            CallsPerDay = [CallsPerDay[0] / UniqueDatesByDay[0].Count(), CallsPerDay[1] / UniqueDatesByDay[1].Count(), CallsPerDay[2] / UniqueDatesByDay[2].Count(), CallsPerDay[3] / UniqueDatesByDay[3].Count(), CallsPerDay[4] / UniqueDatesByDay[4].Count(), CallsPerDay[5] / UniqueDatesByDay[5].Count(), CallsPerDay[6] / UniqueDatesByDay[6].Count()];
            return Page();
        }
    }
}
