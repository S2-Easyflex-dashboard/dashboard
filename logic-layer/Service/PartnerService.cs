using data_layer;

namespace logic_layer
{
    public class PartnerService
    {
        private readonly PartnerRepo PartnerRepo = new PartnerRepo();

        public List<PartnerModel> GetALlPartnersByIp(List<string> IpAdresses)
        {
            List<PartnerDTO> partnerDTOList = PartnerRepo.GetAllPartnersByIp(IpAdresses);
            List<PartnerModel> partnerModelList = [];
            foreach (PartnerDTO partner in partnerDTOList)
            {
                partnerModelList.Add(new(partner.PartnerId, partner.IpAdress, partner.Name));
            }
            return partnerModelList;
        }
    }
}
