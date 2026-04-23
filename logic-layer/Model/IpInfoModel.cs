namespace logic_layer
{
    public class IpInfoModel
    {
        // public List<IpAdress> Ip {get; private set;} = new();
        public string? CompanyName { get; set; }
        public string Ip { get; private set; }
        public int Amount { get; private set; }
        public List<int> CustomerIds { get; private set; }
        public List<string> CustomerNames { get; private set; } = [];

        public IpInfoModel(string ip, int amount, int customerId)
        {
            Ip = ip;
            Amount = amount;
            CustomerIds = [customerId];
        }

        public void AddToCount(int amount, int customerId)
        {
            Amount += amount;
            CustomerIds.Add(customerId);
        }

        public void AddCustomerName(string customerNames)
        {
            CustomerNames.Add(customerNames);
        }
    }
}
