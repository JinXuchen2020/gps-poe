namespace POEMgr.Application.TransferModels
{

    public class PoeRequest_exportExcel
    {
        public string FiscalQuarter { get; set; }
        public string IncentiveName { get; set; }
        public string PartnerName { get; set; }
        public string CustomerName { get; set; }
        public string DeadlineDate { get; set; }
        public string CompletedDate { get; set; }
        public string Status { get; set; }
        public string SubscriptionId { get; set; }
        public string SubscriptionStatus { get; set; }
    }
}
