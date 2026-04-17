using Microsoft.AspNetCore.Mvc;
using presentation_layer.Models;
using System.Diagnostics;
using logic_layer;

namespace presentation_layer.Controllers
{
    public class HomeController : Controller
    {
        CallService CallService = new CallService();

        public IActionResult Index(string[]? rfFilter, int? customerFilter, string? serviceFilter)
        {
            return View(new IndexViewModel(CallService.GetInternVsExtern(customerFilter, serviceFilter), CallService.GetAverageCallsPerDay(customerFilter, serviceFilter), CallService.SplitCallsPerService(rfFilter.Contains("temphire"), rfFilter.Contains("relation")), rfFilter.Contains("temphire"), rfFilter.Contains("relation"), customerFilter, serviceFilter));
        }

        public IActionResult ExtCompList()
        {
            List<IpInfoViewModel> ipInfoViewModel = [];
            foreach (var ipInfo in CallService.GetDuplicateIpCalls())
            {
                ipInfoViewModel.Add(new IpInfoViewModel(ipInfo.Ip, ipInfo.CompanyName, ipInfo.Amount, ipInfo.CustomerIds, ipInfo.CustomerNames));
            }
            return View(new ExtCompListViewModel(ipInfoViewModel));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
