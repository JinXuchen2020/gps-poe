using Microsoft.AspNetCore.Http;
using POEMgr.Application.TransferModels;
using POEMgr.Repository.DbModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace POEMgr.Application
{
    public static class Utility
    {
        private static string NAME = "name";
        private static string PREFERREDUSERNAME = "preferred_username";
        private static string OID = "oid";
        private static string ONJECTID = "http://schemas.microsoft.com/identity/claims/objectidentifier";

        public static string GetFiscalQuarter(DateTime? dateTime)
        {
            string result = string.Empty;
            if (dateTime.HasValue)
            {
                var year = dateTime.Value.Year;
                result += $"FY{year - 2000}";
                var month = dateTime.Value.Month;
                switch (month)
                {
                    case var o when o >= 1 && o <= 3:
                        result += "Q3";
                        break;
                    case var o when o >= 4 && o <= 6:
                        result += "Q4";
                        break;
                    case var o when o >= 7 && o <= 9:
                        result += "Q1";
                        break;
                    case var o when o >= 10 && o <= 12:
                        result += "Q2";
                        break;
                }
            }

            return result;
        }

        public static (DateTime, DateTime) GetDateTimeRange(string fiscalYear, string fiscalQuarter)
        {
            int year = int.Parse(Regex.Match(fiscalYear, "\\d+").Value) + 2000;
            int minMonth = 0;
            int maxMonth = 0;
            switch (fiscalQuarter)
            {
                case "Q3":
                    minMonth = 1;
                    maxMonth = 3;
                    break;
                case "Q4":
                    minMonth = 4;
                    maxMonth = 6;
                    break;
                case "Q1":
                    minMonth = 7;
                    maxMonth = 9;
                    break;
                case "Q2":
                    minMonth = 10;
                    maxMonth = 12;
                    break;
            }

            return (new DateTime(year, minMonth, 1), new DateTime(year, maxMonth, 31));
        }

        public static string UserId(this ClaimsPrincipal user)
        {
            return user?.Claims?
                .FirstOrDefault(x => x.Type == ONJECTID)?.Value;
        }

        public static string Email(this ClaimsPrincipal user)
        {
            return user?.Claims?.FirstOrDefault(x => x.Type == PREFERREDUSERNAME)?.Value ?? string.Empty;
        }

        public static string UserName(this ClaimsPrincipal user)
        {
            return user?.Claims?.FirstOrDefault(x => x.Type == NAME)?.Value ?? string.Empty;
        }

        public static string Description(this Enum item)
        {
            var attr = item.GetType().GetField(item.ToString())?.GetCustomAttribute<DescriptionAttribute>();
            return attr?.Description ?? string.Empty;
        }

        public static PoeRequest_detail_get_res_auditLog GetEmailRecordContent(Poe_MailSendRecord record)
        {
            string content = string.Empty;
            switch(record.Type)
            {
                case Dictionaries.NotifyFirst:
                    content += "首次通知邮件发送";
                    break;
                case Dictionaries.NotifySec:
                    content += "再次通知提醒邮件发送";
                    break;
                case Dictionaries.NotifyReSubmit:
                    content += "待重新提交提醒邮件发送";
                    break;
                case Dictionaries.NotifyExpire:
                    content += "失效通知邮件发送";
                    break;
                case Dictionaries.NotifyComplete:
                    content += "完成通知邮件发送";
                    break;
            }

            if (string.IsNullOrEmpty(record.ErrorMsg))
            {
                content += "成功";
            }
            else
            {
                content += "失败";
            }

            content += $"，时间：{record.CreatedTime?.ToString("yyyy-MM-dd hh:mm:ss")}";

            return new PoeRequest_detail_get_res_auditLog
            {
                Id = record.Id,
                Title = content,
                Content = record.Content,
                CreatedTime = record.CreatedTime
            };
        }
    }
}
