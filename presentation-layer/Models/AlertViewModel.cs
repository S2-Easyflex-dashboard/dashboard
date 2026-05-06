namespace presentation_layer.Models
{
    public class AlertEntryViewModel
    {
        public int CustomerId { get; set; }
        public string AlertType { get; set; }
        public int MaxCallsOnOneDay { get; set; }
        public DateOnly MaxCallsDate { get; set; }

        public AlertEntryViewModel(int customerId, string alertType, int maxCalls, DateOnly maxDate)
        {
            CustomerId = customerId;
            AlertType = alertType;
            MaxCallsOnOneDay = maxCalls;
            MaxCallsDate = maxDate;
        }
    }

    public class AlertsViewModel
    {
        public List<AlertEntryViewModel> Alerts { get; set; }

        public AlertsViewModel(List<AlertEntryViewModel> alerts)
        {
            Alerts = alerts;
        }
    }
}