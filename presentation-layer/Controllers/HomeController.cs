using Microsoft.AspNetCore.Mvc;
using presentation_layer.Models;
using System.Diagnostics;
using logic_layer;

namespace presentation_layer.Controllers
{
    public class HomeController : Controller
    {
        private readonly CallService CallService = new CallService();

        public IActionResult Index(string[]? rfFilter, string? serviceFilter, int? klantFilter)
        {
            List<CallModel> callModelList = CallService.GetAllCalls();
            return View(new IndexViewModel(CallService.GetInternVsExtern(klantFilter, serviceFilter, callModelList), CallService.GetAverageCallsPerDay(klantFilter, serviceFilter, callModelList), CallService.SplitCallsPerService(rfFilter.Contains("temphire"), rfFilter.Contains("relation"), callModelList), rfFilter.Contains("relation"), rfFilter.Contains("temphire"), klantFilter, serviceFilter));
        }

        public IActionResult ExtCompList()
        {
            List<CallModel> callModelList = CallService.GetAllCalls();
            List<IpInfoViewModel> ipInfoViewModel = [];
            foreach (var ipInfo in CallService.GetDuplicateIpCalls(callModelList))
            {
                ipInfoViewModel.Add(new(ipInfo.Ip, ipInfo.CompanyName, ipInfo.Amount, ipInfo.CustomerIds, ipInfo.CustomerNames));
            }
            return View(new ExtCompListViewModel(ipInfoViewModel));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Alerts()
        {
            List<CallModel> callModelList = CallService.GetAllCalls();
            List<AlertEntryViewModel> alertViewModels = [];

            foreach (var alert in CallService.GetAlerts(callModelList))
            {
                alertViewModels.Add(new(alert.CustomerId, alert.AlertType, alert.MaxCallsOnOneDay, alert.MaxCallsDate));
            }

            return View(new AlertsViewModel(alertViewModels));
        }
    }
}
