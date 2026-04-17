namespace data_layer
{
    public class PartnerDTO
    {
        public int PartnerId { get; private set; }
        public string IpAdress { get; private set; }
        public string Name { get; private set; }

        public PartnerDTO(int partnerId, string ipAdress, string name)
        {
            PartnerId = partnerId;
            IpAdress = ipAdress;
            Name = name;
        }
    }
}
