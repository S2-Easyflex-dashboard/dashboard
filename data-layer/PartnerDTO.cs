namespace data_layer
{
    public class PartnerDTO
    {
        public int Id { get; private set; }
        public string IpAdress { get; private set; }
        public string Name { get; private set; }

        public PartnerDTO(int id, string ipAdress, string name)
        {
            Id = id;
            IpAdress = ipAdress;
            Name = name;
        }
    }
}
