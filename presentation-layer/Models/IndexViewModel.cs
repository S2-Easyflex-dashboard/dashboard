namespace presentation_layer.Models
{
    public class IndexViewModel
    {
        public int ExternalCustomer { get; private set; }
        public int InternalCustomers { get; private set; }
        //user id is the first one, amount of calls the second one
        //public int[] HighestCallsTotal { get; private set; } = [0, 0];
        //callsperday will first contain all calls made on a specific weekday, but after all have been added it will have the average instead (the index for this on counts up through the days, starting at sunday)
        public int[] CallsPerDay { get; private set; } = [0, 0, 0, 0, 0, 0, 0,];
        public int ManagingLevel { get; private set; }
        public int RelationLevel { get; private set; }
        public int TempHireLevel { get; private set; }

        public bool RfFilterRelation { get; private set; }
        public bool RfFilterTempHire { get; private set; }
        public int? CustomerFilter { get; private set; }
        public string? ServiceFilter { get; private set; }

        public IndexViewModel(int[] externalInternalCust, int[] callsPerDay, int[] levelOfCall, bool rfFilterRelation, bool rfFilterTempHire, int? customerFilter, string? serviceFilter)
        {
            ExternalCustomer = externalInternalCust[1];
            InternalCustomers = externalInternalCust[0];
            CallsPerDay = callsPerDay;
            ManagingLevel = levelOfCall[2];
            RelationLevel = levelOfCall[1];
            TempHireLevel = levelOfCall[0];
            RfFilterRelation = rfFilterRelation;
            RfFilterTempHire = rfFilterTempHire;
            CustomerFilter = customerFilter;
            ServiceFilter = serviceFilter;
        }
    }
}
