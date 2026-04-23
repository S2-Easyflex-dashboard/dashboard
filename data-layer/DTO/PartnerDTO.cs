namespace data_layer
{
    public record PartnerDTO
    {
        public int PartnerId { get; init; }
        public string IpAdress { get; init; }
        public string Name { get; init; }

        public PartnerDTO(int partnerId, string ipAdress, string name)
        {
            PartnerId = partnerId;
            IpAdress = ipAdress;
            Name = name;
        }
    }
}
