namespace presentation_layer.Models
{
    public class IpInfoViewModel
    {
        // public List<IpAdress> Ip {get; private set;} = new();
        public string Ip { get; set; }
        public int Amount { get; private set; }
        public List<int> CustomerIds { get; private set; }
        public List<string> CustomerNames { get; private set; } = new();

        public IpInfoViewModel(string ip, int amount, int customerIds)
        {
            Ip = ip;
            Amount = amount;
            CustomerIds = [customerIds];
        }
    }
}
