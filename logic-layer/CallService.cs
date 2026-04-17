using data_layer;
using System.Runtime;

namespace logic_layer
{
    public class CallService
    {
        private CallRepo CallRepo = new CallRepo();
        public List<CallModel> CallModelList = [];

        public CallService()
        {
            CallRepo.GetAllCalls();
            foreach (CallDTO call in CallRepo.CallDTOList)
            {
                CallModelList.Add(new CallModel(call.CustomerId, call.Date, call.Ip, call.Service, call.Amount, call.LicentionNr));
            }
        }
        
        public List<IpInfoModel> GetDuplicateIpCalls()
        {
            List<IpInfoModel> UniqueIps = [];
            List<IpInfoModel> DuplicateIps = [];
            List<string> IpList = [];
            List<int> CustomerIdList = [];
            CustomerService customerService;
            PartnerService partnerService;
            
            foreach(CallModel call in CallModelList)
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
                        UniqueIps.Add(new IpInfoModel(call.Ip, call.Amount, call.CustomerId));
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

            customerService = new CustomerService(CustomerIdList);

            partnerService = new PartnerService(IpList);

            foreach(IpInfoModel ip in DuplicateIps)
            {
                foreach(CustomerModel customer in customerService.CustomerModelList)
                {
                    if (ip.CustomerIds.Contains(customer.CustomerId))
                    {
                        ip.AddCustomerName(customer.Name);
                    }
                }
                foreach(PartnerModel partner in partnerService.PartnerModelList)
                {
                    if(ip.Ip == partner.IpAdress)
                    {
                        ip.SetCompanyName(partner.Name);
                    }
                }
            }


            return DuplicateIps;
        }

        public int[] SplitCallsPerService(bool RfTempHireFilter, bool RfRelationFilter)
        {
            int[] LevelAmounts = [0, 0, 0];
            //0 is flexlevel, 1 is relationlevel, 2 is managinglevel
            foreach (CallModel call in CallModelList)
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

        public int[] GetAverageCallsPerDay(int? CustomerFilter, string? ServiceFilter)
        {
            int[] CallsPerDay = [0, 0, 0, 0, 0, 0, 0];
            //each index is a day, starting at sunday and counting up, aka sunday is 0, monday is 1.. etc
            List<DateOnly>[] UniqueDatesByDay = [new(), new(), new(), new(), new(), new(), new()];
            foreach (CallModel call in CallModelList)
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
            CallsPerDay = [CallsPerDay[0] / UniqueDatesByDay[0].Count(), CallsPerDay[1] / UniqueDatesByDay[1].Count(), CallsPerDay[2] / UniqueDatesByDay[2].Count(), CallsPerDay[3] / UniqueDatesByDay[3].Count(), CallsPerDay[4] / UniqueDatesByDay[4].Count(), CallsPerDay[5] / UniqueDatesByDay[5].Count(), CallsPerDay[6] / UniqueDatesByDay[6].Count()];
            return CallsPerDay;
        }

        public int[] GetInternVsExtern(int? CustomerFilter, string? ServiceFilter)
        {
            int[] InternExternComparison = [0, 0];
            //internal is 0, external is 1
            foreach (CallModel call in CallModelList)
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
    }
}