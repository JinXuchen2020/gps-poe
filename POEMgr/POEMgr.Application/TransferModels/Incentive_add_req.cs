

using Microsoft.AspNetCore.Http;

namespace POEMgr.Application.TransferModels
{
    public class Incentive_add_req
    {
        public string Name { get; set; }
        public string WelcomeSpeech { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int? SubmitDeadlineDay { get; set; }
        public int? RemindEmailDay { get; set; }
        public int? ReSubmitDeadlineDay { get; set; }
        public int? RejectCount { get; set; }
        public List<string> CheckPoints { get; set; }
        public List<string> FileIds { get; set; }
        public List<Incentive_add_req_mailTemplate> MailTemplates { get; set; }
    }

    public class Incentive_add_req_mailTemplate
    {
        public string Type { get; set; }
        public string Content { get; set; }
    }

}
