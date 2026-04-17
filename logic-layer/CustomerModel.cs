namespace logic_layer
{
    public class CustomerModel
    {
        public int CustomerId { get; private set; }
        public string Name { get; private set; }

        public CustomerModel(int customerId, string name)
        {
            CustomerId = customerId;
            Name = name;
        }
    }
}