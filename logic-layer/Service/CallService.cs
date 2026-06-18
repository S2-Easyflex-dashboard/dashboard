using data_layer;
using System.Runtime;

namespace logic_layer
{
    public class CallService
    {
        private readonly CallRepo CallRepo = new();

        private static int TryDivideByZero(int firstValue, int secondValue)
        {
            try
            {
                return firstValue / secondValue;
            }
            catch (DivideByZeroException)
            {
                return 0;
            }
        }

        public List<CallModel> GetAllCalls()
        {
            List<CallDTO> callDTOList = CallRepo.GetAllCalls();
            List<CallModel> callModelList = [];
            foreach (CallDTO call in callDTOList)
            {
                callModelList.Add(new(call.CustomerId, call.Date, call.Ip, call.Service, call.Amount, call.LicentionNr));
            }
            return callModelList;
        }
        
        public List<IpInfoModel> GetDuplicateIpCalls(List<CallModel> callModelList)
        {
            List<IpInfoModel> UniqueIps = [];
            List<IpInfoModel> DuplicateIps = [];
            List<string> IpList = [];
            List<int> CustomerIdList = [];
            CustomerService customerService = new();
            PartnerService partnerService = new();
            
            foreach(CallModel call in callModelList)
            {
                IpInfoModel? IpToMove = null;

                foreach (IpInfoModel ip in UniqueIps)
                {
                    if(IpToMove == null && call.Ip == ip.Ip)
                    {
                        IpToMove = ip;
                    }
                }

                if (IpToMove == null)
                {
                    bool FoundDuplicate = false;

                    foreach(IpInfoModel ip in DuplicateIps)
                    {
                        if(call.Ip == ip.Ip)
                        {
                            ip.AddToCount(call.Amount, call.CustomerId);
                            FoundDuplicate = true;
                        }
                    }

                    if (!FoundDuplicate)
                    {
                        UniqueIps.Add(new(call.Ip, call.Amount, call.CustomerId));
                    }
                }
                else
                {
                    UniqueIps.Remove(IpToMove);
                    IpToMove.AddToCount(call.Amount, call.CustomerId);
                    DuplicateIps.Add(IpToMove);
                    IpList.Add(IpToMove.Ip);
                    if (!CustomerIdList.Contains(IpToMove.CustomerIds[0]))
                    {
                        CustomerIdList.Add(IpToMove.CustomerIds[0]);
                    }
                }
            }

            List<CustomerModel> customerModelList = customerService.GetAllCustomersById(CustomerIdList);
            List<PartnerModel> partnerModelList = partnerService.GetALlPartnersByIp(IpList);

            foreach(IpInfoModel ip in DuplicateIps)
            {
                foreach(CustomerModel customer in customerModelList)
                {
                    if (ip.CustomerIds.Contains(customer.CustomerId))
                    {
                        ip.AddCustomerName(customer.Name);
                    }
                }
                foreach(PartnerModel partner in partnerModelList)
                {
                    if(ip.Ip == partner.IpAdress)
                    {
                        ip.CompanyName = partner.Name;
                    }
                }
            }

            return DuplicateIps;
        }

        public static int[] SplitCallsPerService(bool RfTempHireFilter, bool RfRelationFilter, List<CallModel> callModelList)
        {
            int[] LevelAmounts = [0, 0, 0];
            //0 is flexlevel, 1 is relationlevel, 2 is managinglevel
            foreach (CallModel call in callModelList)
            {
                if (call.Service.Contains("_fw_") || (RfTempHireFilter && call.Service.Contains("_rf_")))
                {
                    LevelAmounts[0] += call.Amount;

                    if (RfRelationFilter && call.Service.Contains("_rf_"))
                    {
                        LevelAmounts[1] += call.Amount;
                    }
                }
                else if (call.Service.Contains("_rl_") || (RfRelationFilter && call.Service.Contains("_rf_")))
                {
                    LevelAmounts[1] += call.Amount;
                }
                else if (call.Service.Contains("_wm_") || call.Service.Contains("_bi_"))
                {
                    LevelAmounts[2] += call.Amount;
                }
            }
            return LevelAmounts;
        }

        public static int[] GetAverageCallsPerDay(int? CustomerFilter, string? ServiceFilter, List<CallModel> callModelList)
        {
            int[] CallsPerDay = [0, 0, 0, 0, 0, 0, 0];
            //each index is a day, starting at sunday and counting up, aka sunday is 0, monday is 1.. etc
            List<List<DateOnly>> UniqueDatesByDay = [new(), new(), new(), new(), new(), new(), new()];
            foreach (CallModel call in callModelList)
            {
                if ((CustomerFilter == call.CustomerId || CustomerFilter == null) && (ServiceFilter == call.Service || ServiceFilter == null))
                {
                    CallsPerDay[(int)call.Date.DayOfWeek] += call.Amount;
                    if (!UniqueDatesByDay[(int)call.Date.DayOfWeek].Contains(call.Date))
                    {
                        UniqueDatesByDay[(int)call.Date.DayOfWeek].Add(call.Date);
                    }
                }
            }
            CallsPerDay = [TryDivideByZero(CallsPerDay[0], UniqueDatesByDay[0].Count()), TryDivideByZero(CallsPerDay[1], UniqueDatesByDay[1].Count()), TryDivideByZero(CallsPerDay[2], UniqueDatesByDay[2].Count()), TryDivideByZero(CallsPerDay[3], UniqueDatesByDay[3].Count()), TryDivideByZero(CallsPerDay[4], UniqueDatesByDay[4].Count()), TryDivideByZero(CallsPerDay[5], UniqueDatesByDay[5].Count()), TryDivideByZero(CallsPerDay[6], UniqueDatesByDay[6].Count())];
            return CallsPerDay;
        }

        public static int[] GetInternVsExtern(int? CustomerFilter, string? ServiceFilter, List<CallModel> callModelList)
        {
            int[] InternExternComparison = [0, 0];
            //internal is 0, external is 1
            foreach (CallModel call in callModelList)
            {
                if ((CustomerFilter == call.CustomerId || CustomerFilter == null) && (ServiceFilter == call.Service || ServiceFilter == null))
                {
                    if (call.Ip.Split(".")[0] == "10")
                    {
                        InternExternComparison[0] += call.Amount;
                    }
                    else
                    {
                        InternExternComparison[1] += call.Amount;
                    }
                }
            }
            return InternExternComparison;
        }

        public static List<AlertModel> GetAlerts(List<CallModel> callModelList)
        {
            // Groepeer calls per klant per dag, tel Amount op
            Dictionary<int, Dictionary<DateOnly, int>> callsPerCustomerPerDay = new();

            foreach (CallModel call in callModelList)
            {
                if (!callsPerCustomerPerDay.ContainsKey(call.CustomerId))
                    callsPerCustomerPerDay[call.CustomerId] = new();

                if (!callsPerCustomerPerDay[call.CustomerId].ContainsKey(call.Date))
                    callsPerCustomerPerDay[call.CustomerId][call.Date] = 0;

                callsPerCustomerPerDay[call.CustomerId][call.Date] += call.Amount;
            }

            List<AlertModel> alerts = [];

            foreach (var customer in callsPerCustomerPerDay)
            {
                string type = "";
                List<DateOnly> sortedDates = customer.Value.Keys.OrderBy(d => d).ToList();

                // Check >1440 op 1 dag - direct Probleem
                foreach (var day in customer.Value)
                {
                    if (day.Value > 1440)
                    {
                        type = "Probleem";
                    }
                }

                if (type != "Probleem")
                {
                    // Tel opsomming dagen boven 120
                    int consecutive = 0;
                    int maxConsecutive = 0;

                    for (int i = 0; i < sortedDates.Count; i++)
                    {
                        if (customer.Value[sortedDates[i]] > 120)
                        {
                            consecutive++;
                            if (i > 0 && sortedDates[i] != sortedDates[i - 1].AddDays(1))
                                consecutive = 1;
                        }
                        else
                        {
                            consecutive = 0;
                        }
                        if (consecutive > maxConsecutive) maxConsecutive = consecutive;
                    } 

                    if (maxConsecutive >= 10)
                        type = "Probleem";
                    else if (maxConsecutive >= 5)
                        type = "Waarschuwing";
                    else if (customer.Value.Any(d => d.Value > 120))
                        type = "Waarschuwing";
                }

                if (type != "")
                    alerts.Add(new AlertModel(customer.Key, type, customer.Value));
            }

            return alerts;
        }
    }
}