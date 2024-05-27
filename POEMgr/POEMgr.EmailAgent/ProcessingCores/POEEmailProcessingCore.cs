using MailService.Interfaces;
using Microsoft.EntityFrameworkCore;
using POEMgr.Repository.DBContext;
using POEMgr.Repository.DbModels;
using System.Text.RegularExpressions;

namespace POEMgr.EmailAgent.ProcessingCores
{
    public class POEEmailProcessingCore
    {
        private readonly POEContext _poeContext;
        private readonly IEmailService _emailService;
        private readonly CommonSetting _config;

        private List<Poe_MailTemplate> poe_MailTemplates;
        private List<Poe_POERequest> poe_POERequests;
        private List<Poe_Incentive> poe_Incentives;
        private List<Poe_Partner> poe_Partners;
        private List<Poe_PartnerIncentive> poe_PartnerIncentives;
        private List<Poe_MailSendRecord> poe_MailSendRecords;
        private List<Poe_Customer> poe_Customers;
        private List<Poe_SubscriptionStatus> poe_SubscriptionStatuss;
        private List<Poe_RequestLog> poe_RequestLogs;

        public POEEmailProcessingCore(CommonSetting config, POEContext poeContext, IEmailService emailService)
        {
            this._config = config;
            this._poeContext = poeContext;
            this._emailService = emailService;
        }

        public async Task StartCheck()
        {
            try
            {
                this._poeContext.Poe_SchedulerLog.Add(new Poe_SchedulerLog
                {
                    Id = Guid.NewGuid(),
                    Content = "SchedulerStart",
                    CreateTime = DateTime.Now
                });
                this._poeContext.SaveChanges();

                poe_POERequests = this._poeContext.Poe_POERequest.Where(x => x.CompletedDate == null && x.Status != "Submitted" && x.Status != "Approved").ToList();
                poe_MailTemplates = this._poeContext.Poe_MailTemplate.Where(x => poe_POERequests.Select(y => y.IncentiveId).Contains(x.IncentiveId)).ToList();
                poe_Incentives = this._poeContext.Poe_Incentive.Where(x => poe_POERequests.Select(y => y.IncentiveId).Contains(x.Id)).ToList();
                poe_Partners = this._poeContext.Poe_Partner.Where(x => poe_POERequests.Select(y => y.PartnerId).Contains(x.Id)).ToList();
                poe_PartnerIncentives = this._poeContext.Poe_PartnerIncentive
                    .Where(x => poe_POERequests.Select(y => y.PartnerId).Contains(x.PartnerId) || poe_POERequests.Select(y => y.IncentiveId).Contains(x.IncentiveId))
                    .ToList();
                poe_MailSendRecords = this._poeContext.Poe_MailSendRecord.Where(x => poe_POERequests.Select(y => y.Id).Contains(x.PoeRequestId)).ToList();
                poe_Customers = this._poeContext.Poe_Customer.Where(x => poe_POERequests.Select(y => y.CustomerId).Contains(x.Id)).ToList();
                poe_SubscriptionStatuss = this._poeContext.Poe_SubscriptionStatus.Where(x => poe_POERequests.Select(y => y.Id).Contains(x.PoeRequestId)).ToList();
                poe_RequestLogs = this._poeContext.Poe_RequestLog.Where(x => poe_POERequests.Select(y => y.Id).Contains(x.RequestId)).ToList();

                foreach (Poe_POERequest item in poe_POERequests)
                {
                    //Send Expired Mail
                    if (item.DeadLineDate != null && item.DeadLineDate.Value < DateTime.Now)
                    {
                        if (!poe_MailSendRecords.Any(x => x.PoeRequestId == item.Id && x.CreatedTime >= item.ModifiedTime && x.Type == "Notify-Expire"))
                        {
                            await SendMail(item.Id, "Notify-Expire");
                        }
                    }
                    //Send Sec Remind Mail
                    Poe_Incentive poe_Incentive = poe_Incentives.Where(x => x.Id == item.IncentiveId).FirstOrDefault();
                    if (poe_Incentive != null && poe_Incentive.RemindEmailDay != null)
                    {
                        if ((DateTime.Now - Convert.ToDateTime(item.StartDate)).TotalDays >= poe_Incentive.RemindEmailDay && item.Status == "EmailSent")
                        {
                            if (!poe_MailSendRecords.Any(x => x.PoeRequestId == item.Id && x.Type == "Notify-Sec" && !string.IsNullOrEmpty(x.ErrorMsg)))
                            {
                                await SendMail(item.Id, "Notify-Sec");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this._poeContext.Poe_SchedulerLog.Add(new Poe_SchedulerLog
                {
                    Id = Guid.NewGuid(),
                    Content = "Scheduler Error",
                    ErrorMsg = ex.ToString(),
                    CreateTime = DateTime.Now
                });
                this._poeContext.SaveChanges();
            }
        }

        private async Task SendMail(string poeRequestId, string type)
        {
            try
            {
                Poe_POERequest poe_POERequest = poe_POERequests.Where(x => x.Id == poeRequestId).First();
                Poe_Partner poe_Partner = poe_Partners.Where(x => x.Id == poe_POERequest.PartnerId).FirstOrDefault();
                Poe_PartnerIncentive poe_PartnerIncentive = poe_PartnerIncentives.Where(x => x.PartnerId == poe_POERequest.PartnerId && x.IncentiveId == poe_POERequest.IncentiveId).FirstOrDefault();
                var incentiveFile = await _poeContext.Poe_POEFile
                .Where(x => x.IncentiveId == poe_POERequest.IncentiveId)
                .FirstOrDefaultAsync();
                string email = poe_PartnerIncentive.Mail ?? poe_Partner.Mail;
                string emailCC = poe_PartnerIncentive.MailCC ?? poe_Partner.MailCC;
                if (poe_Partner != null && !string.IsNullOrEmpty(email))
                {
                    Poe_MailTemplate poe_MailTemplate = poe_MailTemplates.Where(x => x.IncentiveId == poe_POERequest.IncentiveId && x.Type == type).FirstOrDefault();
                    if (poe_MailTemplate != null)
                    {
                        //Replace content
                        Poe_Customer poe_Customer = poe_Customers.Where(x => x.Id == poe_POERequest.CustomerId).FirstOrDefault();
                        Poe_Incentive poe_Incentive = poe_Incentives.Where(x => x.Id == poe_POERequest.IncentiveId).FirstOrDefault();
                        List<Poe_SubscriptionStatus> poe_SubscriptionsStatuss1 = poe_SubscriptionStatuss.Where(x => x.PoeRequestId == poe_POERequest.Id).ToList();
                        List<Poe_RequestLog> poe_RequestLogs1 = poe_RequestLogs.Where(x => x.RequestId == poeRequestId).ToList();
                        string content = poe_MailTemplate.Content;
                        content = content.Replace("{A}", poe_Partner == null ? string.Empty : poe_Partner.PartnerName);
                        content = content.Replace("{B}", poe_POERequest.PartnerId);
                        content = content.Replace("{C}", poe_Customer == null ? string.Empty : poe_Customer.CustomerName);
                        content = content.Replace("{D}", poe_Incentive == null ? string.Empty : poe_Incentive.IncentiveName);
                        content = content.Replace("{E}", poe_SubscriptionsStatuss1 == null ? string.Empty : "<br>" + string.Join("<br>", poe_SubscriptionsStatuss1.Select(x => x.SubscriptionId).ToList<string>()));
                        content = content.Replace("{F}", poe_POERequest.StartDate == null ? string.Empty : Convert.ToDateTime(poe_POERequest.StartDate).ToString("yyyy-MM-dd"));
                        content = content.Replace("{G}", poe_POERequest.DeadLineDate == null ? string.Empty : Convert.ToDateTime(poe_POERequest.DeadLineDate).ToString("yyyy-MM-dd"));
                        content = content.Replace("{H}", poe_POERequest.StartDate == null ? string.Empty : FiscalQuarter(Convert.ToDateTime(poe_POERequest.StartDate)));
                        content = content.Replace("{I}", string.IsNullOrEmpty(_config.LoginUrl) ? string.Empty : $"<a href=\"{_config.LoginUrl}\">登录</a>");
                        content = content.Replace("{J}", poe_RequestLogs == null ? string.Empty : "<br>" + string.Join("<br>", poe_RequestLogs.Select(x => x.Reason).ToList<string>()));
                        content = content.Replace("{K}", poe_POERequest.DeadLineDate == null ? string.Empty : Convert.ToDateTime(poe_POERequest.DeadLineDate).ToString("yyyy-MM-dd"));

                        var messagePattern = "<p>邮件标题[:：]?(?<subject>[\\s\\S]+)</p><p>邮件正文：</p>(?<content>[\\s\\S]+)";
                        var matchResult = Regex.Match(content, messagePattern);
                        var subject = type;
                        var emailContent = content;
                        if (matchResult.Success)
                        {
                            subject = matchResult.Groups["subject"].Value;
                            emailContent = matchResult.Groups["content"].Value;
                        }

                        List<string> sendTo = email.Split(';').ToList();
                        List<string> cc = string.IsNullOrEmpty(emailCC) ? new List<string>() : emailCC.Split(';').ToList();
                        //send mail
                        string sendResult = await _emailService.SendAsync(sendTo, cc, subject, emailContent);

                        //update poe_POERequest status
                        if (type == "Notify-Expire" && poe_POERequest.Status != "Expired")
                        {
                            poe_POERequest.Status = "Expired";
                            poe_POERequest.CompletedDate = DateTime.Now;
                            poe_POERequest.ModifiedTime = DateTime.Now;
                            poe_POERequest.ModifiedBy = "SchduleJob";
                            this._poeContext.Update(poe_POERequest);
                        }
                        //else if (type == "Notify-Sec" && poe_POERequest.Status == "Draft" && string.IsNullOrEmpty(sendResult))
                        //{
                        //    poe_POERequest.Status = "EmailSent";
                        //    poe_POERequest.ModifiedTime = DateTime.Now;
                        //    poe_POERequest.ModifiedBy = "SchduleJob";
                        //    poe_POERequest.DeadLineDate = DateTime.Now.AddDays(poe_Incentive.SubmitDeadlineDay.Value);
                        //    this._poeContext.Update(poe_POERequest);
                        //}

                        //add record
                        Poe_MailSendRecord Poe_MailSendRecord = new Poe_MailSendRecord
                        {
                            Id = Guid.NewGuid().ToString(),
                            PoeRequestId = poeRequestId,
                            SendTo = email,
                            CC = emailCC,
                            Type = type,
                            Content = content,
                            ErrorMsg = sendResult,
                            CreatedTime = DateTime.Now,
                            ModifiedTime = DateTime.Now,
                            CreatedBy = "SchduleJob",
                            ModifiedBy = "SchduleJob"
                        };
                        this._poeContext.Add(Poe_MailSendRecord);
                        this._poeContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                this._poeContext.Poe_SchedulerLog.Add(new Poe_SchedulerLog
                {
                    Id = Guid.NewGuid(),
                    Content = "Scheduler Error",
                    ErrorMsg = ex.ToString(),
                    CreateTime = DateTime.Now
                });
                this._poeContext.SaveChanges();
            }
        }

        public async Task SendHttpRequest()
        {
            try
            {
                using (HttpClient hc = new HttpClient())
                {
                    var responseMessage = await hc.GetAsync(_config.KeepRequestUrl);
                }
            }
            catch (Exception ex)
            {
                this._poeContext.Poe_SchedulerLog.Add(new Poe_SchedulerLog
                {
                    Id = Guid.NewGuid(),
                    Content = "SendHttpRequest",
                    ErrorMsg = ex.ToString(),
                    CreateTime = DateTime.Now
                });
                this._poeContext.SaveChanges();
            }
        }

        public async Task SendMailTest()
        {
            string sendResult = await _emailService.SendAsync(_config.SendMailTestSendTo, "Schedule SendMailTest", "Schedule SendMailTest");
            this._poeContext.Poe_SchedulerLog.Add(new Poe_SchedulerLog
            {
                Id = Guid.NewGuid(),
                Content = "SendMailTest",
                ErrorMsg = sendResult,
                CreateTime = DateTime.Now
            });
            this._poeContext.SaveChanges();
        }

        public string FiscalQuarter(DateTime startDate)
        {
            string result = "FY";
            int year = Convert.ToDateTime(startDate).Year;
            int month = Convert.ToDateTime(startDate).Month;
            result += month >= 7 ? (year + 1).ToString().Substring(2) : year.ToString().Substring(2);

            if (month >= 7 && month <= 9) result += "Q1";
            else if (month >= 10 && month <= 12) result += "Q2";
            else if (month >= 1 && month <= 3) result += "Q3";
            else if (month >= 4 && month <= 6) result += "Q4";

            return result;
        }
    }

    public class CommonSetting
    {
        public string KeepRequestUrl { get; set; }
        public string LoginUrl { get; set; }
        public string SendMailTestSendTo { get; set; }
    }
}
