using logic_layer;

namespace data_layer
{
    public class PartnerService
    {
        private PartnerRepo PartnerRepo = new PartnerRepo();
        public List<PartnerModel> PartnerModelList = [];
        
        public PartnerService(List<string> IpAdresses)
        {
            PartnerRepo.GetAllPartnersByIp(IpAdresses);
            foreach(PartnerDTO partner in PartnerRepo.PartnerDTOList)
            {
                PartnerModelList.Add(new PartnerModel(partner.PartnerId, partner.IpAdress, partner.Name));
            }
        }
    }
}
