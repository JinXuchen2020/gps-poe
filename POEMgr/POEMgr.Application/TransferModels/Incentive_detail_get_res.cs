

namespace POEMgr.Application.TransferModels
{
    public class Incentive_detail_get_res
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string WelcomeSpeech { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int? SubmitDeadlineDay { get; set; }
        public int? RemindEmailDay { get; set; }
        public int? ReSubmitDeadlineDay { get; set; }
        public int? RejectCount { get; set; }
        public List<Incentive_detail_get_res_checkpoint> CheckPoints { get; set; }
        public List<Incentive_detail_get_res_mailTemplate> MailTemplates { get; set; }
        public List<Incentive_detail_get_res_fileTemplate> Files { get; set; }
    }

    public class Incentive_detail_get_res_checkpoint
    {
        public string Id { get; set; }
        public string Content { get; set; }
    }

    public class Incentive_detail_get_res_mailTemplate
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
    }

    public class Incentive_detail_get_res_fileTemplate
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string Path { get; set; }
    }
}
