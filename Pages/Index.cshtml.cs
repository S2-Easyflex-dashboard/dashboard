using dashboard.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;


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
        public async Task<IActionResult> OnGet()
        {
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

            return Page();
        }
    }
}
