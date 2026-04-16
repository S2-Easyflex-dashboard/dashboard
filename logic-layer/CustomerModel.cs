namespace logic_layer
{
    public class CustomerViewModel
    {
        public int CustomerId { get; private set; }
        public string Name { get; private set; }

        public CustomerViewModel(int customerId, string name)
        {
            CustomerId = customerId;
            Name = name;
        }
    }
}