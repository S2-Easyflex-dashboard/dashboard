namespace logic_layer
{
    public class AlertModel
        //'1 melding per 1 klant
    {
        public int CustomerId { get; private set; }
        public string AlertType { get; private set; } // "Waarschuwing" of "Probleem"
        public int MaxCallsOnOneDay { get; private set; }
        public DateOnly MaxCallsDate { get; private set; }

        public AlertModel(int customerId, string alertType, Dictionary<DateOnly, int> callsPerDay)
        {
            CustomerId = customerId;
            AlertType = alertType;
            var max = callsPerDay.MaxBy(d => d.Value);
            MaxCallsOnOneDay = max.Value;
            MaxCallsDate = max.Key;
        }
    }
}