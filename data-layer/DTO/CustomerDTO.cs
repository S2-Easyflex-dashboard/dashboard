namespace data_layer
{
    public record CustomerDTO
    {
        public int CustomerId { get; init; }
        public string Name { get; init; }

        public CustomerDTO(int customerId, string name)
        {
            CustomerId = customerId;
            Name = name;
        }
    }
}
