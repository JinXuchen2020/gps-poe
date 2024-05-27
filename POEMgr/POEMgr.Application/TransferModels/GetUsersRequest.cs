

namespace POEMgr.Application.TransferModels
{
    public class GetUsersRequest: PageCommon
    {
        public string PartnerId { get; set; }
        public string PartnerName { get; set; }
        public string PartnerEmail { get; set; }
        public string RoleName { get; set; }
        public string IsDisabled { get; set; }
    }
}
