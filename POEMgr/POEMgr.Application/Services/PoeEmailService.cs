using Authentication;
using MailService.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using POEMgr.Application.Interfaces;
using POEMgr.Repository.DBContext;
using POEMgr.Repository.DbModels;
using System.Text.RegularExpressions;

namespace POEMgr.Application.Services
{
    public class PoeEmailService : IPoeEmailService
    {
        private readonly IEmailService _emailServiceCore;
        private readonly POEContext _poeContext;
        private readonly Appsettings _appSettings;
        private readonly IClaimsAccessor _claimsAccessor;
        private readonly IPoeFileService _iPoeFileService;

        public PoeEmailService(POEContext poeContext, Appsettings appSettings, IPoeFileService fileService, IClaimsAccessor claimsAccessor, IEmailService emailServiceCore)
        {
            _emailServiceCore = emailServiceCore;
            _poeContext = poeContext;
            _appSettings = appSettings;
            _claimsAccessor = claimsAccessor;
            _iPoeFileService = fileService;
        }

        public async Task<string> SendAsync(string to, string subject, string body)
        {
            return await _emailServiceCore.SendAsync(to, subject, body);
        }

        public async Task<string> SendAsync(List<string> to, string subject, string body)
        {
            return await _emailServiceCore.SendAsync(to, subject, body);
        }

        public async Task<string> SendAsync(List<string> to, List<string> cc, string subject, string body)
        {
            return await _emailServiceCore.SendAsync(to, cc, subject, body);
        }

        public async Task<string> SendNotifyEmailAsync(string requestId, string type)
        {
            var poe_POERequest = await _poeContext.Poe_POERequest
                .Where(x => x.Id == requestId)
                .FirstOrDefaultAsync();

            var templateType = type;

            var poe_Partner = await _poeContext.Poe_PartnerIncentive
                .Where(x => x.PartnerId == poe_POERequest.PartnerId && x.IncentiveId == poe_POERequest.IncentiveId)
                .FirstOrDefaultAsync();

            var incentiveFile = await _poeContext.Poe_POEFile
                .Where(x => x.IncentiveId == poe_POERequest.IncentiveId)
                .FirstOrDefaultAsync();

            var poe_MailTemplate = await _poeContext.Poe_MailTemplate
                .Where(x => x.IncentiveId == poe_POERequest.IncentiveId && x.Type == templateType)
                .FirstOrDefaultAsync();

            var result = string.Empty;

            if (poe_MailTemplate != null && poe_Partner != null && !string.IsNullOrEmpty(poe_Partner.Mail))
            {
                List<string> sendTo = poe_Partner.Mail.Split(';').ToList();
                List<string> cc = string.IsNullOrEmpty(poe_Partner.MailCC) ? new List<string>() : poe_Partner.MailCC.Split(';').ToList();
                string content = await MailContentConvert(poe_POERequest.Id, poe_MailTemplate.Content);
                var messagePattern = "<p>邮件标题[:：]?(?<subject>[\\s\\S]+)</p><p>邮件正文：</p>(?<content>[\\s\\S]+)";
                var matchResult = Regex.Match(content, messagePattern);
                var subject = templateType;
                var emailContent = content;
                if (matchResult.Success)
                {
                    subject = matchResult.Groups["subject"].Value;
                    emailContent = matchResult.Groups["content"].Value;
                }

                var imgPattern = "src=\\\"(?<imgSrc>https.*?[png|jpg]*)\\\"";
                var imgMatches = Regex.Matches(emailContent, imgPattern);
                var linkedResources = new List<(string, byte[])>();
                foreach (Match match in imgMatches)
                {
                    var imgSrc = match.Groups["imgSrc"].Value;
                    linkedResources.Add((Path.GetFileName(imgSrc), _iPoeFileService.GetFileFromBlob(imgSrc)));
                }

                var sentDate = DateTime.Now;

                result = await _emailServiceCore.SendAsync(
                    sendTo,
                    cc,
                    subject,
                    emailContent,
                    new List<(string, byte[])> { (incentiveFile.FileName, _iPoeFileService.GetFileFromBlob(incentiveFile.BlobUri)) },
                    linkedResources
                );

                if (string.IsNullOrEmpty(result))
                {
                    var sentResult = _emailServiceCore.GetUnReadMessage(sentDate);
                    if (sentResult.Any())
                    {
                        var parseResult = sentResult.Select(c =>
                        {
                            var errorPattern = "<a .*>(?<mailAddress>.*)<\\/a>[\\s\\S]*Remote Server returned '(?<errorMsg>.*)'|[\\s\\S]*<(?<mailAddress>.*)>[\\s\\S]*";
                            var errorMatch = Regex.Match(c.Content, errorPattern);
                            var mailAddress = string.Empty;
                            var errorMsg = string.Empty;
                            if (errorMatch.Success)
                            {
                                mailAddress = errorMatch.Groups["mailAddress"].Value;
                                errorMsg = errorMatch.Groups["errorMsg"].Success ? errorMatch.Groups["errorMsg"].Value : c.Content;
                            }
                            return new { EmailAddress = mailAddress, ErrorMessage = errorMsg };
                        }).ToList();
                        result += JsonConvert.SerializeObject(new
                        {
                            ErrorCode = "Undeliverable",
                            EmailAddress = string.Join(";", parseResult.Where(c => !string.IsNullOrEmpty(c.EmailAddress)).Select(c => c.EmailAddress)),
                            ErrorMessage = string.Join(";", parseResult.Where(c => !string.IsNullOrEmpty(c.ErrorMessage)).Select(c => c.ErrorMessage))
                        });
                    }
                }

                Poe_MailSendRecord Poe_MailSendRecord = new Poe_MailSendRecord
                {
                    Id = Guid.NewGuid().ToString(),
                    PoeRequestId = poe_POERequest.Id,
                    SendTo = poe_Partner.Mail,
                    CC = poe_Partner.MailCC,
                    Type = templateType,
                    Content = content,
                    ErrorMsg = result,
                    CreatedBy = _claimsAccessor.UserId,
                    ModifiedBy = _claimsAccessor.UserId
                };

                _poeContext.Add(Poe_MailSendRecord);
                _poeContext.SaveChanges();
            }

            return result;
        }

        public async Task<string> MailContentConvert(string poeRequestId, string content)
        {
            Poe_POERequest poe_POERequest = await _poeContext.Poe_POERequest.Where(x => x.Id == poeRequestId).FirstOrDefaultAsync();
            if (poe_POERequest != null)
            {
                Poe_Partner poe_Partner = await _poeContext.Poe_Partner.Where(x => x.Id == poe_POERequest.PartnerId).FirstOrDefaultAsync();
                Poe_Customer poe_Customer = await _poeContext.Poe_Customer.Where(x => x.Id == poe_POERequest.CustomerId).FirstOrDefaultAsync();
                Poe_Incentive poe_Incentive = await _poeContext.Poe_Incentive.Where(x => x.Id == poe_POERequest.IncentiveId).FirstOrDefaultAsync();
                List<Poe_SubscriptionStatus> poe_SubscriptionStatus = await _poeContext.Poe_SubscriptionStatus.Where(x => x.PoeRequestId == poe_POERequest.Id).ToListAsync();
                var subscriptions = poe_POERequest.Status == PoeRequestStatus.Approved.ToString() || poe_POERequest.Status == PoeRequestStatus.PartialApproved.ToString()
                    ? poe_SubscriptionStatus.Where(c => !string.IsNullOrEmpty(c.Status)).Select(c => c.SubscriptionId)
                    : poe_SubscriptionStatus.Select(c => c.SubscriptionId);

                List <Poe_RequestLog> poe_RequestLogs = await _poeContext.Poe_RequestLog.Where(x => x.RequestId == poe_POERequest.Id && !string.IsNullOrEmpty(x.Reason)).ToListAsync();
                content = content.Replace("{A}", poe_Partner?.PartnerName);
                content = content.Replace("{B}", poe_POERequest?.PartnerId);
                content = content.Replace("{C}", poe_Customer?.CustomerName);
                content = content.Replace("{D}", poe_Incentive?.IncentiveName);
                content = content.Replace("{E}", "<br>" + string.Join("<br>", subscriptions));
                content = content.Replace("{F}", poe_POERequest.StartDate?.ToString("yyyy-MM-dd") ?? string.Empty);
                content = content.Replace("{G}", poe_POERequest.DeadLineDate?.ToString("yyyy-MM-dd") ?? string.Empty);
                content = content.Replace("{H}", Utility.GetFiscalQuarter(poe_POERequest.StartDate));
                content = content.Replace("{I}", _appSettings.LoginUrl ?? string.Empty);
                content = content.Replace("{J}", "<br>" + string.Join("<br>", poe_RequestLogs.Select(x => x.Reason)));
                content = content.Replace("{K}", poe_POERequest.StartDate.HasValue ? poe_POERequest.StartDate.Value.AddDays(poe_Incentive.ReSubmitDeadlineDay.Value).ToString("yyyy-MM-dd") : string.Empty);
            }

            return content;
        }
    }
}
