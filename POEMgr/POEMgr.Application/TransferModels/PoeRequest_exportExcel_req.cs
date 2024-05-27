namespace POEMgr.Application.TransferModels
{

    public class PoeRequest_exportExcel_req
    {
        public string FiscalYear { get; set; }
        public string FiscalQuarter { get; set; }
        public string IncentiveId { get; set; }
        public string PartnerName { get; set; }
        public string Status { get; set; }
    }
}
