namespace logic_layer
{
    public class CallModel
    {
        public int CustomerId { get ; private set; }
        public DateOnly Date { get; private set; }
        public string Ip { get; private set; }
        public string Service { get; private set; }
        public int Amount { get; private set; }
        public int LicentionNr { get; private set; }
        
        public CallModel(int customerId, DateOnly date, string ip, string service, int amount, int licentionNr)
        {
            CustomerId = customerId;
            Date = date;
            Ip = ip;
            Service = service;
            Amount = amount;
            LicentionNr = licentionNr;
        }
    }
}

