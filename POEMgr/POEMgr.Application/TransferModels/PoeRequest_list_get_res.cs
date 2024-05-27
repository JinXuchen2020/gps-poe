namespace POEMgr.Application.TransferModels
{

    public class PoeRequest_list_get_res
    {
        public string Id { get; set; }
        public string IncentiveId { get; set; }
        public string IncentiveName { get; set; }
        public string IncentiveWelcomeSpeech { get; set; }
        public string PoeTemplatePath { get; set; }
        public string FiscalQuarter { get; set; }
        public string PartnerId { get; set; }
        public string FileId { get; set; }
        public string FileName { get; set; }
        public string PartnerName { get; set; }
        public string PartnerEmail { get; set; }
        public string CustomerName { get; set; }
        public string CountDown { get; set; }
        public string DeadlineDate { get; set; }
        public string CompletedDate { get; set; }
        public string Status { get; set; }
    }
}
