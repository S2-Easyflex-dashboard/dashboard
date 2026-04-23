namespace logic_layer
{
    public class PartnerModel
    {
        public int PartnerId { get; private set; }
        public string IpAdress { get; private set; }
        public string Name { get; private set; }

        public PartnerModel(int partnerId, string ipAdress, string name)
        {
            PartnerId = partnerId;
            IpAdress = ipAdress;
            Name = name;
        }
    }
}
