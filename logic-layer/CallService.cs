using data_layer;

namespace logic_layer
{
    public class CallService
    {
        private CallRepo _callRepo = new CallRepo();

        public List<CallModel> GetAllCalls()
        {
            _callRepo.GetAllCalls();
            return _callRepo.CallDTOList
                .Select(c => new CallModel(c.CustomerId, c.Date, c.Ip, c.Service, c.Amount, c.LicentionNr))
                .ToList();
        }

        public List<CallModel> GetDuplicateIpCalls()
        {
            var calls = GetAllCalls();
            var ipCustomerMap = new Dictionary<string, List<int>>();

            foreach (var call in calls)
            {
                if (!ipCustomerMap.ContainsKey(call.Ip))
                {
                    ipCustomerMap[call.Ip] = new List<int>();
                }
                if (!ipCustomerMap[call.Ip].Contains(call.CustomerId))
                {
                    ipCustomerMap[call.Ip].Add(call.CustomerId);
                }
            }

            var duplicateIps = ipCustomerMap
                .Where(kvp => kvp.Value.Count > 1)
                .Select(kvp => kvp.Key)
                .ToList();

            return calls.Where(c => duplicateIps.Contains(c.Ip)).ToList();
        }

        public List<CallModel> GetUniqueIpCalls()
        {
            var calls = GetAllCalls();
            var ipCustomerMap = new Dictionary<string, List<int>>();

            foreach (var call in calls)
            {
                if (!ipCustomerMap.ContainsKey(call.Ip))
                {
                    ipCustomerMap[call.Ip] = new List<int>();
                }
                if (!ipCustomerMap[call.Ip].Contains(call.CustomerId))
                {
                    ipCustomerMap[call.Ip].Add(call.CustomerId);
                }
            }

            var uniqueIps = ipCustomerMap
                .Where(kvp => kvp.Value.Count == 1)
                .Select(kvp => kvp.Key)
                .ToList();

            return calls.Where(c => uniqueIps.Contains(c.Ip)).ToList();
        }
    }
}