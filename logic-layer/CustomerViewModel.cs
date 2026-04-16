namespace logic_layer
{
    public class CustomerViewModel
    {
        public int Id { get; private set; }
        public string Name { get; private set; }

        public CustomerViewModel(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}