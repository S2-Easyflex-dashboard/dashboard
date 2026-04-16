using data_layer;

namespace logic_layer
{
    public class CallService
    {
        private CallRepo _callRepo = new CallRepo();
        
        public List<CallsViewModel> GetAllCalls()
        {
            _callRepo.GetAllCalls();
            return _callRepo.CallDTOList
                .Select(c => new CallsViewModel(c.CustomerId, c.Date, c.Ip, c.Service, c.Amount, c.LicentionNr))
                .ToList();
        }
    }
}

