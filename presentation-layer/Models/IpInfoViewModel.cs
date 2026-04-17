namespace presentation_layer.Models
{
    public class IpInfoViewModel
    {
        // public List<IpAdress> Ip {get; private set;} = new();
        public string? CompanyName { get; private set; }
        public string Ip { get; set; }
        public int Amount { get; private set; }
        public List<int> CustomerIds { get; private set; }
        public List<string> CustomerNames { get; private set; } = new();

        public IpInfoViewModel(string ip, string? companyName, int amount, List<int> customerIds, List<string> customerNames)
        {
            Ip = ip;
            CompanyName = companyName;
            Amount = amount;
            CustomerIds = customerIds;
            CustomerNames = customerNames;
        }
    }
}
