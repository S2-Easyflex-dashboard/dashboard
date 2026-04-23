namespace data_layer
{
    public record CallDTO
    {
        public int CustomerId { get; init; }
        public DateOnly Date { get; init; }
        public string Ip { get; init; }
        public string Service { get; init; }
        public int Amount { get; init; }
        public int LicentionNr { get; init; }

        public CallDTO(int customerId, DateOnly date, string ip, string service, int amount, int licentionNr)
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