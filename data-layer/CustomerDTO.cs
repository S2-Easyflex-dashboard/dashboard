namespace data_layer
{
    public class CustomerDTO
    {
        public int CustomerId { get; private set; }
        public string Name { get; private set; }

        public CustomerDTO(int customerId, string name)
        {
            CustomerId = customerId;
            Name = name;
        }
    }
}
