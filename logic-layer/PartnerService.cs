using logic_layer;

namespace data_layer
{
    public class PartnerService
    {
        public PartnerRepo PartnerRepo;

        public List<PartnerModel> GetPartnersByIps(string[] IpAdresses)
        {
            PartnerRepo.GetAllPartnersByIp(IpAdresses);
            return PartnerRepo.PartnerDTOList
                .Select(dto => new PartnerViewModel(dto.PartnerId, dto.IpAdress, dto.Name))
                .ToList();
        }
    }
}
