using Microsoft.AspNetCore.Mvc;
using presentation_layer.Models;
using System.Diagnostics;

namespace presentation_layer.Controllers
{
    public class HomeController : Controller
    {
        CallService CallService = new CallService();

        public IActionResult Index(string[]? rfFilter, int? customerFilter, string? serviceFilter)
        {
            var viewModel = new IndexViewModel(CallService.ExternalCustomer, CallService.InternalCustomers, CallService.HighestCallTotal, CallService.CallsPerDay, CallService.ManagingLevel, CallService.RelationLevel, CallService.TempHireLevel);
            viewModel.RfFilterTempHire = rfFilter.Contains("temphire");
            viewModel.RfFilterRelation = rfFilter.Contains("relation");
            if (customerFilter != null)
            {
                viewModel.CustomerFilter = (int)customerFilter;
            }
            if (serviceFilter != null)
            {
                viewModel.ServiceFilter = serviceFilter;
            }
            return View(viewModel);
        }

        public IActionResult ExtCompList()
        {
            var viewModel = new ExtCompListViewModel(CallService.DuplicateIps);
            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
