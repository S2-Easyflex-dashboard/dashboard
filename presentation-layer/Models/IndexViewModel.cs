namespace presentation_layer.Models
{
    public class IndexViewModel
    {
        public int ExternalCustomer { get; private set; }
        public int InternalCustomers { get; private set; }
        //user id is the first one, amount of calls the second one
        public int[] HighestCallsTotal { get; private set; } = [0, 0];
        //callsperday will first contain all calls made on a specific weekday, but after all have been added it will have the average instead (the index for this on counts up through the days, starting at sunday)
        public int[] CallsPerDay { get; private set; } = [0, 0, 0, 0, 0, 0, 0,];
        public int ManagingLevel { get; private set; }
        public int RelationLevel { get; private set; }
        public int TempHireLevel { get; private set; }

        public bool RfFilterRelation { get; set; }
        public bool RfFilterTempHire { get; set; }
        public int? CustomerFilter { get; set; }
        public string? ServiceFilter { get; set; }

        public IndexViewModel(int externalCustomer, int internalCustomers, int[] highestCallTotal, int[] callsPerDay, int managingLevel, int relationLevel, int tempHireLevel)
        {
            ExternalCustomer = externalCustomer;
            InternalCustomers = internalCustomers;
            HighestCallsTotal = highestCallTotal;
            CallsPerDay = callsPerDay;
            ManagingLevel = managingLevel;
            RelationLevel = relationLevel;
            TempHireLevel = tempHireLevel;
            RfFilterRelation = false;
            RfFilterTempHire = false;
        }
    }
}
