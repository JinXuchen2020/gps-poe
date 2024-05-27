using POEMgr.Application.Interfaces;
using AutoMapper;
using POEMgr.Repository.DBContext;
using POEMgr.Application.TransferModels;
using FileService;
using Newtonsoft.Json;
using POEMgr.Repository.DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;
using NPOI.SS.Formula.Functions;
using MailService;
using Authentication;
using Microsoft.IdentityModel.Tokens;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;
using LogService;
using System.Drawing;
using FileService.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using NPOI.SS.UserModel;
using NPOI.XSSF.Streaming.Values;
using System.Text.RegularExpressions;
using NETCore.MailKit.Core;

namespace POEMgr.Application.Services
{
    internal class PoeRequestService : BaseService, IPoeRequestService
    {
        private readonly IPoeFileService _iPoeFileService;

        public PoeRequestService(IMapper mapper, POEContext poeContext, IClaimsAccessor claimsAccessor, IPoeFileService fileService, IPoeLogService poeLogService) 
            : base(mapper, poeContext, claimsAccessor, poeLogService)
        {
            _iPoeFileService = fileService;
        }

        public async Task<ApiResult> PoeRequest_list_get(PoeRequest_list_get_req p)
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                List<PoeRequest_list_get_res> res =
                    await (
                    from a in _POEContext.Poe_POERequest
                    join b in _POEContext.Poe_Partner on a.PartnerId equals b.Id
                    join c in _POEContext.Poe_Customer on a.CustomerId equals c.Id
                    join d in _POEContext.Poe_Incentive on a.IncentiveId equals d.Id
                    join pi in _POEContext.Poe_PartnerIncentive on new { IId = d.Id, PId = b.Id } equals new { IId = pi.IncentiveId, PId = pi.PartnerId }
                    where (string.IsNullOrEmpty(p.IncentiveId) || a.IncentiveId == p.IncentiveId)
                    where (string.IsNullOrEmpty(p.PartnerName) || b.PartnerName.Contains(p.PartnerName))
                    where (string.IsNullOrEmpty(p.Status) || a.Status == p.Status)
                    where !_partnerIds.Any() || _partnerIds.Contains(pi.PartnerId)
                    select new PoeRequest_list_get_res
                    {
                        Id = a.Id,
                        IncentiveId = d.Id,
                        IncentiveName = d.IncentiveName,
                        IncentiveWelcomeSpeech = d.WelcomeSpeech,
                        FiscalQuarter = a.StartDate == null ? string.Empty : Convert.ToDateTime(a.StartDate).ToString("yyyy-MM-dd"),
                        PartnerId = b.Id,
                        PartnerName = b.PartnerName,
                        PartnerEmail = pi.Mail,
                        CustomerName = c.CustomerName,
                        DeadlineDate = a.DeadLineDate.HasValue ? a.DeadLineDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty,
                        CompletedDate = a.CompletedDate.HasValue ? a.CompletedDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty,
                        Status = a.Status,
                        PoeTemplatePath = string.Empty
                    }
                    ).ToListAsync();

                List<Poe_POEFile> poe_POEFiles = _POEContext.Poe_POEFile.ToList();
                foreach (PoeRequest_list_get_res item in res)
                {
                    item.FiscalQuarter = string.IsNullOrEmpty(item.FiscalQuarter) ? string.Empty : FiscalQuarter(Convert.ToDateTime(item.FiscalQuarter));
                    Poe_POEFile poe_POEFile = poe_POEFiles.Where(x => x.IncentiveId == item.IncentiveId).FirstOrDefault();
                    if (poe_POEFile!=null)
                    {
                        item.PoeTemplatePath = poe_POEFile.BlobUri ?? string.Empty;
                        item.FileId = poe_POEFile.Id;
                        item.FileName = poe_POEFile.FileName;
                    }
                    item.CountDown = !string.IsNullOrEmpty(item.DeadlineDate) ? WorkDayMinus(DateTime.Now, Convert.ToDateTime(item.DeadlineDate)).ToString() : string.Empty;
                }
                apiResult.Data = res;
            }
            catch (Exception ex)
            {
                apiResult.Code = -1;
                apiResult.Msg = ex.ToString();
            }
            return apiResult;
        }

        public async Task<DownloadFileModel> PoeRequest_downLoadTemplateFile(string id)
        {
            DownloadFileModel res = new DownloadFileModel();
            var Poe_POEFile = await _POEContext.Poe_POEFile.Where(x => x.Id == id).FirstOrDefaultAsync();
            string path = Poe_POEFile.Path;
            //res.Stream = await _zipService.DownFilesReturnByte(path);
            res.Stream = _iPoeFileService.GetFileFromBlob(path);
            res.FileName = Poe_POEFile.FileName;
            return res;
        }

        public async Task<byte[]> PoeRequest_exportExcel(PoeRequest_exportExcel_req p)
        {
            (DateTime, DateTime)? dateRange = null;

            if (!string.IsNullOrEmpty(p.FiscalYear) && !string.IsNullOrEmpty(p.FiscalQuarter))
            {
                dateRange = Utility.GetDateTimeRange(p.FiscalYear, p.FiscalQuarter);
            }

            var res =
                    await (
                    from a in _POEContext.Poe_POERequest
                    join b in _POEContext.Poe_Partner on a.PartnerId equals b.Id
                    join c in _POEContext.Poe_Customer on a.CustomerId equals c.Id
                    join d in _POEContext.Poe_Incentive on a.IncentiveId equals d.Id
                    join pi in _POEContext.Poe_PartnerIncentive on new { IId = d.Id, PId = b.Id } equals new { IId = pi.IncentiveId, PId = pi.PartnerId }
                    join ss in _POEContext.Poe_SubscriptionStatus on a.Id equals ss.PoeRequestId
                    where (string.IsNullOrEmpty(p.IncentiveId) || a.IncentiveId == p.IncentiveId)
                    where (string.IsNullOrEmpty(p.PartnerName) || b.PartnerName.Contains(p.PartnerName))
                    where (string.IsNullOrEmpty(p.Status) || a.Status == p.Status)
                    where (dateRange == null || (a.StartDate >= dateRange.Value.Item1 && a.StartDate < dateRange.Value.Item2))
                    where !_partnerIds.Any() || _partnerIds.Contains(pi.PartnerId)
                    where a.Status != PoeRequestStatus.Draft.ToString()
                    select new PoeRequest_exportExcel
                    {
                        FiscalQuarter = Utility.GetFiscalQuarter(a.StartDate),
                        Status = Enum.Parse<PoeRequestStatus>(a.Status).Description(),
                        IncentiveName = d.IncentiveName,
                        PartnerName = b.PartnerName,
                        CustomerName = c.CustomerName,
                        DeadlineDate = a.DeadLineDate.HasValue ? a.DeadLineDate.Value.ToString("yyyy-MM-dd") : string.Empty,
                        CompletedDate = a.CompletedDate.HasValue ? a.CompletedDate.Value.ToString("yyyy-MM-dd") : string.Empty,
                        SubscriptionId = ss.SubscriptionId,
                        SubscriptionStatus = string.IsNullOrEmpty(ss.Status) ? "未检查" : "已检查",
                    }
                    ).ToListAsync();
            return _iPoeFileService.ExcelListToStream(res);
        }

        public async Task<byte[]> PoeRequest_audit_exportExcel(PoeRequest_exportExcel_req p)
        {
            (DateTime, DateTime)? dateRange = null;

            if (!string.IsNullOrEmpty(p.FiscalYear) && !string.IsNullOrEmpty(p.FiscalQuarter))
            {
                dateRange = Utility.GetDateTimeRange(p.FiscalYear, p.FiscalQuarter);
            }

            var res =
                    await (
                    from a in _POEContext.Poe_POERequest
                    join b in _POEContext.Poe_Partner on a.PartnerId equals b.Id
                    join c in _POEContext.Poe_Customer on a.CustomerId equals c.Id
                    join d in _POEContext.Poe_Incentive on a.IncentiveId equals d.Id
                    join pi in _POEContext.Poe_PartnerIncentive on new { IId = d.Id, PId = b.Id } equals new { IId = pi.IncentiveId, PId = pi.PartnerId }
                    join ss in _POEContext.Poe_SubscriptionStatus on a.Id equals ss.PoeRequestId
                    where (string.IsNullOrEmpty(p.IncentiveId) || a.IncentiveId == p.IncentiveId)
                    where (string.IsNullOrEmpty(p.PartnerName) || b.PartnerName.Contains(p.PartnerName))
                    where (string.IsNullOrEmpty(p.Status) || a.Status == p.Status)
                    where (dateRange == null || (a.StartDate >= dateRange.Value.Item1 && a.StartDate < dateRange.Value.Item2))
                    where a.Status != PoeRequestStatus.Draft.ToString()
                    select new PoeRequest_exportExcel
                    {
                        FiscalQuarter = Utility.GetFiscalQuarter(a.StartDate),
                        Status = Enum.Parse<PoeRequestStatus>(a.Status).Description(),
                        IncentiveName = d.IncentiveName,
                        PartnerName = b.PartnerName,
                        CustomerName = c.CustomerName,
                        DeadlineDate = a.DeadLineDate.HasValue ? a.DeadLineDate.Value.ToString("yyyy-MM-dd") : string.Empty,
                        CompletedDate = a.CompletedDate.HasValue ? a.CompletedDate.Value.ToString("yyyy-MM-dd") : string.Empty,
                        SubscriptionId = ss.SubscriptionId,
                        SubscriptionStatus = string.IsNullOrEmpty(ss.Status) ? "未检查" : "已检查",
                    }
                    ).ToListAsync();
            return _iPoeFileService.ExcelListToStream(res);
        }

        public async Task<ApiResult> PoeRequest_uploadFile(IFormFile p)
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                PoeRequest_uploadFile_res res = new PoeRequest_uploadFile_res();
                //string dir = $@"E:\POEStorage\PoeRequestUploadFiles";
                //if (!Directory.Exists(dir))
                //{
                //    _ = Directory.CreateDirectory(dir);
                //}
                //Poe_POEFile Poe_POEFile = new Poe_POEFile();
                //Poe_POEFile.Id = Guid.NewGuid().ToString();
                //Poe_POEFile.FileName = p.FileName;
                //Poe_POEFile.Path = $@"{dir}\{Poe_POEFile.Id}_{p.FileName}";
                //using FileStream fileStream = new($@"{dir}\{Poe_POEFile.Id + "_" + p.FileName}", FileMode.Create, FileAccess.Write, FileShare.Write);
                //await p.CopyToAsync(fileStream);
                //fileStream.Close();
                //DefaultEntityBase(ref Poe_POEFile);
                MemoryStream st = new MemoryStream();
                p.CopyTo(st);
                Poe_POEFile Poe_POEFile = new Poe_POEFile();
                Poe_POEFile.Id = Guid.NewGuid().ToString();
                Poe_POEFile.FileName = p.FileName;
                string blobUri = _iPoeFileService.SaveFileToBlob(st, p.FileName, p.ContentType, "PoeRequestUploadFiles");
                Poe_POEFile.Path = blobUri;
                Poe_POEFile.BlobUri = blobUri;
                res.Id = Poe_POEFile.Id;
                res.Name = Poe_POEFile.FileName;
                res.Path = Poe_POEFile.Path;
                res.BlobUri = Poe_POEFile.BlobUri;
                _POEContext.Add(WriteEntityBase(Poe_POEFile));
                await _POEContext.SaveChangesAsync();
                apiResult.Data = res;
                WriteDbLog(DbLogType.Create, "PoeRequest_uploadFile", JsonConvert.SerializeObject(Poe_POEFile));
            }
            catch (Exception ex)
            {
                apiResult.Code = -1;
                apiResult.Msg = ex.ToString();
            }
            return apiResult;
        }

        public async Task<ApiResult> PoeRequest_removeFile(string p)
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                Poe_POEFile removePoe_POEFile = await _POEContext.Poe_POEFile.Where(x => x.Id == p).FirstOrDefaultAsync();
                if (removePoe_POEFile!=null)
                {
                    _iPoeFileService.DeleteFileFromBlob(removePoe_POEFile.FileName, "PoeRequestUploadFiles");
                    _POEContext.Poe_POEFile.Remove(removePoe_POEFile);
                    apiResult.Data = _POEContext.SaveChanges();
                    WriteDbLog(DbLogType.Delete, "PoeRequest_removeFile", JsonConvert.SerializeObject(removePoe_POEFile));
                    return apiResult;
                }
            }
            catch (Exception ex)
            {
                apiResult.Code = -1;
                apiResult.Msg = ex.ToString();
            }
            return apiResult;
        }

        public async Task<byte[]> PoeRequest_downLoadZipFile(string id)
        {
            List<Poe_POEFile> poe_POEFiles = await _POEContext.Poe_POEFile.Where(x => x.PoeRequestId == id).ToListAsync();
            var res = await _iPoeFileService.CompressionFilesReturnByte(poe_POEFiles);
            return res;
        }

        public async Task<ApiResult> PoeRequest_save(string id, PoeRequest_save_req p)
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                Poe_POERequest poe_POERequest = _POEContext.Poe_POERequest.Where(x => x.Id == id).FirstOrDefault();
                //if (p.RequestStatus.Any(x => string.IsNullOrWhiteSpace(x.Status)))
                //{
                //    apiResult.Code = 1;
                //    apiResult.Msg = "FileIntegrity not all checked.";
                //    apiResult.Data = p;
                //    return apiResult;
                //}
                if (poe_POERequest == null)
                {
                    apiResult.Code = 1;
                    apiResult.Msg = $"PoeRequestId {id} not found.";
                    apiResult.Data = p;
                    return apiResult;
                }

                if(p.RequestStatus.Any())
                {
                    List<Poe_CheckPointStatus> oldPoe_CheckPointStatus = await _POEContext.Poe_CheckPointStatus.Where(x => x.PoeRequestId == id && x.Status == Dictionaries.Request && x.IsDeleted == false).ToListAsync();
                    oldPoe_CheckPointStatus?.ForEach(x => x.IsDeleted = true);
                    List<Poe_CheckPointStatus> newPoe_CheckPointStatus = p.RequestStatus.Select(x => new Poe_CheckPointStatus
                    {
                        Id = Guid.NewGuid().ToString(),
                        Type = Dictionaries.Request,
                        PoeRequestId = id,
                        CheckPointId = x.Id,
                        Status = x.Status
                    }).ToList();

                    _POEContext.UpdateRange(WriteEntityBase(oldPoe_CheckPointStatus));
                    _POEContext.AddRange(WriteEntityBase(newPoe_CheckPointStatus));
                }

                if (p.SubscriptionStatus.Any())
                {
                    var updatePoe_SubscriptionStatus = await _POEContext.Poe_SubscriptionStatus.Where(x => x.PoeRequestId == id).ToListAsync();
                    foreach (var item in updatePoe_SubscriptionStatus)
                    {
                        var filter = p.SubscriptionStatus.Where(x => x.SubscriptionId == item.SubscriptionId).FirstOrDefault();
                        if (filter != null)
                        {
                            item.Status = filter.Status;
                        }
                    }

                    _POEContext.UpdateRange(WriteEntityBase(updatePoe_SubscriptionStatus));
                }

                if (p.RequestFiles.Any())
                {
                    List<Poe_POEFile> updatePoe_POEFiles = _POEContext.Poe_POEFile.Where(x => x.PoeRequestId == id).ToList();
                    updatePoe_POEFiles.ForEach(x => x.PoeRequestId = null);
                    List<Poe_POEFile> updatePoe_POEFiles1 = _POEContext.Poe_POEFile.Where(x => p.RequestFiles.Contains(x.Id)).ToList();
                    updatePoe_POEFiles1.ForEach(x => x.PoeRequestId = id);
                    _POEContext.UpdateRange(WriteEntityBase(updatePoe_POEFiles));
                    _POEContext.UpdateRange(WriteEntityBase(updatePoe_POEFiles1));
                }

                Poe_RequestLog poe_RequestLog = new Poe_RequestLog
                {
                    Id = Guid.NewGuid().ToString(),
                    RequestId = id,
                    Content = $"PoeRequest {(p.Status == Dictionaries.Submitted ? "提交" : "保存")} 成功，时间：{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}"
                };

                poe_POERequest.Status = p.Status;
                _POEContext.Update(WriteEntityBase(poe_POERequest));
                _POEContext.Add(WriteEntityBase(poe_RequestLog));
                _POEContext.SaveChanges();
                apiResult.Data = p;
                WriteDbLog(DbLogType.Update, "PoeRequest_save", id+ ": " +JsonConvert.SerializeObject(p));
            }
            catch (Exception ex)
            {
                apiResult.Code = -1;
                apiResult.Msg = ex.ToString();
            }
            return apiResult;
        }

        public async Task<ApiResult> PoeRequest_rejectPoeRequest(string id, RejectPoeRequestRequest p)
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                Poe_POERequest poe_POERequest = await _POEContext.Poe_POERequest.Where(x => x.Id == id).FirstOrDefaultAsync();
                var incentive = await _POEContext.Poe_Incentive.Where(c => c.Id == poe_POERequest.IncentiveId).FirstOrDefaultAsync();
                Poe_RequestLog poe_RequestLog = new Poe_RequestLog
                {
                    Id = Guid.NewGuid().ToString(),
                    RequestId = id,
                };

                int poe_RequestLogCount = await _POEContext.Poe_RequestLog.Where(x => x.RequestId == id && !string.IsNullOrEmpty(x.Reason)).CountAsync();
                if (poe_RequestLogCount >= incentive.RejectCount)
                {
                    poe_POERequest.Status = Dictionaries.Expired;
                    poe_RequestLog.Content = $"PoeRequest 已失效，时间：{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}";
                }
                else
                {
                    poe_POERequest.Status = Dictionaries.Rejected;
                    poe_POERequest.DeadLineDate = WorkEndDayAdd(DateTime.Now, incentive.ReSubmitDeadlineDay.Value);
                    poe_RequestLog.Content = $"PoeRequest 被拒绝，时间：{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}";
                    poe_RequestLog.Reason = p.RejectReason;
                }

                _POEContext.Update(WriteEntityBase(poe_POERequest));
                _POEContext.Add(WriteEntityBase(poe_RequestLog));
                _POEContext.SaveChanges();
                apiResult.Data = p;
                WriteDbLog(DbLogType.Update, "PoeRequest_rejectPoeRequest", id+": "+JsonConvert.SerializeObject(p));
            }
            catch (Exception ex)
            {
                apiResult.Code = -1;
                apiResult.Msg = ex.ToString();
            }
            return apiResult;
        }

        public async Task<ApiResult> PoeRequest_approvePoeRequest(string id, AuditPoeRequestRequest p)
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                Poe_POERequest poe_POERequest = _POEContext.Poe_POERequest.Where(x => x.Id == id).FirstOrDefault();
                //if (p.AuditStatus.Any(x => string.IsNullOrWhiteSpace(x.Status)))
                //{
                //    apiResult.Code = 1;
                //    apiResult.Msg = "FileIntegrity not all checked.";
                //    apiResult.Data = p;
                //    return apiResult;
                //}
                if (poe_POERequest == null)
                {
                    apiResult.Code = 1;
                    apiResult.Msg = $"PoeRequestId {id} not found.";
                    apiResult.Data = p;
                    return apiResult;
                }

                if (p.AuditStatus.Any())
                {
                    List<Poe_CheckPointStatus> oldPoe_AuditStatus = await _POEContext.Poe_CheckPointStatus.Where(x => x.PoeRequestId == id && x.Type == Dictionaries.Audit && x.IsDeleted == false).ToListAsync();
                    oldPoe_AuditStatus.ForEach(c => c.IsDeleted = true);
                    List<Poe_CheckPointStatus> newAuditStatus = p.AuditStatus.Select(x => new Poe_CheckPointStatus
                    {
                        Id = Guid.NewGuid().ToString(),
                        Type = Dictionaries.Audit,
                        PoeRequestId = id,
                        CheckPointId = x.Id,
                        Status = x.Status
                    }).ToList();
                    _POEContext.UpdateRange(WriteEntityBase(oldPoe_AuditStatus));
                    _POEContext.AddRange(WriteEntityBase(newAuditStatus));
                }

                if(p.SubscriptionStatus.Any())
                {
                    var updatePoe_SubscriptionStatus = await _POEContext.Poe_SubscriptionStatus.Where(x => x.PoeRequestId == id).ToListAsync();
                    foreach (var item in updatePoe_SubscriptionStatus)
                    {
                        var filter = p.SubscriptionStatus.Where(x => x.SubscriptionId == item.SubscriptionId).FirstOrDefault();
                        if (filter != null)
                        {
                            item.Status = filter.Status;
                        }
                    }

                    _POEContext.UpdateRange(WriteEntityBase(updatePoe_SubscriptionStatus));
                }
                Poe_RequestLog poe_RequestLog = new Poe_RequestLog
                {
                    Id = Guid.NewGuid().ToString(),
                    RequestId = id,
                    Content = $"PoeRequest 通过审核，时间：{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}"
                };
                poe_POERequest.Status = p.Status;
                poe_POERequest.CompletedDate = DateTime.Now;
                _POEContext.Update(WriteEntityBase(poe_POERequest));
                _POEContext.Add(WriteEntityBase(poe_RequestLog));
                _POEContext.SaveChanges();

                apiResult.Data = p;
                WriteDbLog(DbLogType.Update, "PoeRequest_approvePoeRequest", id+": "+JsonConvert.SerializeObject(p));
            }
            catch (Exception ex)
            {
                apiResult.Code = -1;
                apiResult.Msg = ex.ToString();
            }
            return apiResult;
        }

        public async Task<ApiResult> PoeRequest_saveAuditPoeRequest(string id, AuditPoeRequestRequest p)
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                Poe_POERequest poe_POERequest = _POEContext.Poe_POERequest.Where(x => x.Id == id).FirstOrDefault();
                if (p.AuditStatus.Any(x => string.IsNullOrWhiteSpace(x.Status)))
                {
                    apiResult.Code = 1;
                    apiResult.Msg = "FileIntegrity not all checked.";
                    apiResult.Data = p;
                    return apiResult;
                }
                if (poe_POERequest == null)
                {
                    apiResult.Code = 1;
                    apiResult.Msg = $"PoeRequestId {id} not found.";
                    apiResult.Data = p;
                    return apiResult;
                }

                if (p.AuditStatus.Any())
                {
                    List<Poe_CheckPointStatus> oldPoe_AuditStatus = await _POEContext.Poe_CheckPointStatus.Where(x => x.PoeRequestId == id && x.Type == Dictionaries.Audit && x.IsDeleted == false).ToListAsync();
                    oldPoe_AuditStatus.ForEach(c => c.IsDeleted = true);
                    List<Poe_CheckPointStatus> newAuditStatus = p.AuditStatus.Select(x => new Poe_CheckPointStatus
                    {
                        Id = Guid.NewGuid().ToString(),
                        Type = Dictionaries.Audit,
                        PoeRequestId = id,
                        CheckPointId = x.Id,
                        Status = x.Status
                    }).ToList();
                    _POEContext.UpdateRange(WriteEntityBase(oldPoe_AuditStatus));
                    _POEContext.AddRange(WriteEntityBase(newAuditStatus));
                }
                   
                if(p.SubscriptionStatus.Any())
                {
                    var updatePoe_SubscriptionStatus = await _POEContext.Poe_SubscriptionStatus.Where(x => x.PoeRequestId == id).ToListAsync();
                    foreach (var item in updatePoe_SubscriptionStatus)
                    {
                        var filter = p.SubscriptionStatus.Where(x => x.SubscriptionId == item.SubscriptionId).FirstOrDefault();
                        if (filter != null)
                        {
                            item.Status = filter.Status;
                        }
                    }

                    _POEContext.UpdateRange(WriteEntityBase(updatePoe_SubscriptionStatus));
                }

                Poe_RequestLog poe_RequestLog = new Poe_RequestLog
                {
                    Id = Guid.NewGuid().ToString(),
                    RequestId = id,
                    Content = $"PoeRequest 保存成功，时间：{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}"
                };

                if(!string.IsNullOrEmpty(p.DeadLineDate))
                {
                    poe_POERequest.DeadLineDate = Convert.ToDateTime(p.DeadLineDate);
                }
                _POEContext.Update(WriteEntityBase(poe_POERequest));
                _POEContext.Add(WriteEntityBase(poe_RequestLog));
                _POEContext.SaveChanges();
                apiResult.Data = p;
                WriteDbLog(DbLogType.Update, "PoeRequest_saveAuditPoeRequest", id + ": "+JsonConvert.SerializeObject(p));
            }
            catch (Exception ex)
            {
                apiResult.Code = -1;
                apiResult.Msg = ex.ToString();
            }
            return apiResult;
        }

        public async Task<ApiResult> PoeRequest_detail_get(string id)
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                PoeRequest_detail_get_res poeRequest_detail_get_res = new PoeRequest_detail_get_res();
                Poe_POERequest poe_POERequest = await _POEContext.Poe_POERequest.Where(x => x.Id == id).FirstOrDefaultAsync();
                if (poe_POERequest == null)
                {
                    apiResult.Code = 1;
                    apiResult.Msg = $@"can not find id:{id}";
                }

                Poe_Customer poe_Customer = await _POEContext.Poe_Customer.Where(x => x.Id == poe_POERequest.CustomerId).FirstOrDefaultAsync();
                
                Poe_Partner poe_Partner = await _POEContext.Poe_Partner.Where(x => x.Id == poe_POERequest.PartnerId).FirstOrDefaultAsync();
                Poe_PartnerIncentive poe_PartnerIncentive = await _POEContext.Poe_PartnerIncentive.Where(x => x.PartnerId == poe_POERequest.PartnerId && x.IncentiveId == poe_POERequest.IncentiveId).FirstOrDefaultAsync();
                
                Poe_Incentive poe_Incentive = await _POEContext.Poe_Incentive.Where(x => x.Id == poe_POERequest.IncentiveId).FirstOrDefaultAsync();
               
                List<Poe_POEFile> poe_POEFile = await _POEContext.Poe_POEFile.Where(x => x.PoeRequestId == id).ToListAsync();
                List<Poe_CheckPoint> poe_CheckPoint = await _POEContext.Poe_CheckPoint.Where(x => x.IncentiveId == poe_POERequest.IncentiveId).ToListAsync();
                List<Poe_CheckPointStatus> poe_CheckPointStatus = await _POEContext.Poe_CheckPointStatus.Where(x => x.PoeRequestId == id && x.IsDeleted == false).ToListAsync();
                List<Poe_RequestLog> poe_RequestLog = await _POEContext.Poe_RequestLog.Where(x => x.RequestId == id).ToListAsync();
                List<Poe_MailSendRecord> poe_MailSendRecord = await _POEContext.Poe_MailSendRecord.Where(x => x.PoeRequestId == id).ToListAsync();
                List<Poe_SubscriptionStatus> poe_SubscriptionStatuss = await _POEContext.Poe_SubscriptionStatus.Where(x => x.PoeRequestId == id).ToListAsync();
                poeRequest_detail_get_res.Id = id;
                poeRequest_detail_get_res.Incentive = new Incentive_detail_get_res 
                {
                    Id = poe_Incentive.Id,
                    Name = poe_Incentive.IncentiveName,
                    CheckPoints = poe_CheckPoint.Select(c=> new Incentive_detail_get_res_checkpoint
                    {
                        Id=c.Id,
                        Content=c.Content
                    }).ToList()
                };
                poeRequest_detail_get_res.Partner = new Partner_detail_get_res
                {
                    Id = poe_Partner.Id,
                    Email = poe_PartnerIncentive.Mail ?? poe_Partner.Mail,
                    Name = poe_Partner.PartnerName
                };
                poeRequest_detail_get_res.Customer = new Customer_detail_get_res
                {
                    Id = poe_Customer.Id,
                    Name = poe_Customer.CustomerName,
                    Subscriptions = poe_SubscriptionStatuss.Select(x => x.SubscriptionId).ToList()
                };
                poeRequest_detail_get_res.Status = poe_POERequest.Status;
                poeRequest_detail_get_res.DeadLineDate = Convert.ToDateTime(poe_POERequest.DeadLineDate).ToString("yyyy-MM-dd HH:mm:ss");
                poeRequest_detail_get_res.RequestFiles = poe_POEFile.Select(x => new PoeRequest_detail_get_res_requestFile
                {
                    Id = x.Id,
                    Name = x.FileName,
                    Path = x.Path
                }).ToList();

                poeRequest_detail_get_res.RequestCheckPoints = poe_CheckPoint.Select(x => new PoeRequest_detail_get_res_checkPoints
                {
                    Id = x.Id,
                    Status = poe_CheckPointStatus.Where(y => y.CheckPointId == x.Id && y.Type == Dictionaries.Request).FirstOrDefault()?.Status ?? string.Empty
                }).ToList();

                poeRequest_detail_get_res.AuditCheckPoints = poe_CheckPoint.Select(x => new PoeRequest_detail_get_res_checkPoints
                {
                    Id = x.Id,
                    Status = poe_CheckPointStatus.Where(y => y.CheckPointId == x.Id && y.Type == Dictionaries.Audit).FirstOrDefault()?.Status ?? string.Empty
                }).ToList();

                poeRequest_detail_get_res.Subscriptions = poe_SubscriptionStatuss.Select(x => new PoeRequest_detail_get_res_subscriptionCheck
                {
                    SubscriptionId = x.SubscriptionId,
                    Status = x.Status
                }).ToList();

                var commonLogs = poe_RequestLog.Select(x => new PoeRequest_detail_get_res_auditLog
                {
                    Id = x.Id,
                    Title= x.Content,
                    Content = string.Empty,
                    Reason = x.Reason,
                    CreatedTime = x.CreatedTime
                }).ToList();

                var emailLogs = poe_MailSendRecord.Select(x => Utility.GetEmailRecordContent(x)).ToList();

                poeRequest_detail_get_res.Logs = commonLogs.Concat(emailLogs).OrderBy(c => c.CreatedTime).ToList();

                apiResult.Data = poeRequest_detail_get_res;
            }
            catch (Exception ex)
            {
                apiResult.Code = -1;
                apiResult.Msg = ex.ToString();
            }
            return apiResult;
        }

        public async Task<ApiResult> PoeRequest_audit_list_get(PoeRequest_audit_list_get_req p)
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                List<PoeRequest_audit_list_get_res> res =
                    await (
                    from a in _POEContext.Poe_POERequest
                    join b in _POEContext.Poe_Partner on a.PartnerId equals b.Id
                    join c in _POEContext.Poe_Customer on a.CustomerId equals c.Id
                    join d in _POEContext.Poe_Incentive on a.IncentiveId equals d.Id
                    join pi in _POEContext.Poe_PartnerIncentive on new { IId = d.Id, PId = b.Id } equals new { IId = pi.IncentiveId, PId = pi.PartnerId }
                    where !string.IsNullOrEmpty(a.Status)
                    where (string.IsNullOrEmpty(p.IncentiveId) || a.IncentiveId == p.IncentiveId)
                    where (string.IsNullOrEmpty(p.Status) || a.Status==p.Status)
                    where (string.IsNullOrEmpty(p.PartnerName) || b.PartnerName.Contains(p.PartnerName))
                    select new PoeRequest_audit_list_get_res
                    {
                        Id = a.Id,
                        IncentiveId = d.Id,
                        IncentiveName = d.IncentiveName,
                        PartnerName = b.PartnerName,
                        PartnerMail = pi.Mail,
                        CustomerName = c.CustomerName,
                        Status = a.Status,
                        FiscalQuarter = a.StartDate == null ? string.Empty : Convert.ToDateTime(a.StartDate).ToString("yyyy-MM-dd"),
                        PartnerId = b.Id,
                        PartnerEmail = pi.Mail,
                        DeadlineDate = a.DeadLineDate.HasValue ? a.DeadLineDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty,
                        CompletedDate = a.CompletedDate.HasValue ? a.CompletedDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty,
                        PoeTemplatePath = string.Empty
                    }
                    ).ToListAsync();
                List<Poe_POEFile> poe_POEFiles = _POEContext.Poe_POEFile.ToList();
                foreach (PoeRequest_audit_list_get_res item in res)
                {
                    item.FiscalQuarter = string.IsNullOrEmpty(item.FiscalQuarter) ? string.Empty : FiscalQuarter(Convert.ToDateTime(item.FiscalQuarter));
                    Poe_POEFile poe_POEFile = poe_POEFiles.Where(x => x.IncentiveId == item.IncentiveId).OrderBy(x => x.CreatedTime).FirstOrDefault();
                    if (poe_POEFile != null)
                    {
                        item.PoeTemplatePath = poe_POEFile.BlobUri ?? string.Empty;
                        item.FileId = poe_POEFile.Id;
                        item.FileName = poe_POEFile.FileName;
                    }
                    item.CountDown = !string.IsNullOrEmpty(item.DeadlineDate) ? WorkDayMinus(Convert.ToDateTime(item.DeadlineDate), DateTime.Now).ToString() : string.Empty;
                }
                apiResult.Data = res;
            }
            catch (Exception ex)
            {
                apiResult.Code = -1;
                apiResult.Msg = ex.ToString();
            }
            return apiResult;
        }

        public string[] GetUnCheckedSubscriptions(string poeRequestId)
        {
            var poeRequest = _POEContext.Poe_POERequest.First(c => c.Id == poeRequestId);
            var poe_SubscriptionStatuss = _POEContext.Poe_SubscriptionStatus.Where(c => c.PoeRequestId == poeRequestId).ToList();
            var allSubscriptions = _POEContext.Poe_Subscription.Where(c => c.CustomerId == poeRequest.CustomerId).ToList();

            var checkedSubscriptions = poe_SubscriptionStatuss.Where(c => !string.IsNullOrEmpty(c.Status)).Select(c => c.SubscriptionId);
            return allSubscriptions.Where(c => !checkedSubscriptions.Contains(c.Id)).Select(c => c.Id.ToString()).ToArray();
        }

        //public async Task<ApiResult> SubmitPoeRequests(SubmitPoeRequestRequest model)
        //{
        //    ApiResult apiResult = new ApiResult();
        //    try
        //    {
        //        List<Poe_Customer> poe_Customers = await _POEContext.Poe_Customer.ToListAsync();
        //        List<string> filter = model.CustomerIds.Where(x => !poe_Customers.Any(y => y.Id == x)).ToList();
        //        if (filter.Count > 0)
        //        {
        //            apiResult.Code = 1;
        //            apiResult.Msg = $"Not any customer matched:{string.Join(",", filter)}";
        //            apiResult.Data = model;
        //            return apiResult;
        //        }
        //        List<Poe_POERequest> poe_POERequests = await _POEContext.Poe_POERequest.Where(x => model.CustomerIds.Contains(x.CustomerId)).ToListAsync();
        //        List<Poe_POERequest> newPOERequests = new List<Poe_POERequest>();
        //        List<Poe_POERequest> updatePOERequests = new List<Poe_POERequest>();
        //        List<SubmitPoeRequestResponse> SubmitPoeRequestResponse = new List<SubmitPoeRequestResponse>();
        //        foreach (string customerId in model.CustomerIds)
        //        {
        //            if (poe_POERequests.Any(x => x.CustomerId == customerId))
        //            {
        //                Poe_POERequest poe_POERequestUpdate = poe_POERequests.Where(x => x.CustomerId == customerId).First();
        //                if (poe_POERequestUpdate.Status == Dictionaries.Rejected)
        //                {
        //                    poe_POERequestUpdate.Status = Dictionaries.Submitted;
        //                    DefaultEntityBase(ref poe_POERequestUpdate);
        //                    updatePOERequests.Add(poe_POERequestUpdate);
        //                }
        //                continue;
        //            }
        //            string poeRequestId = Guid.NewGuid().ToString();
        //            Poe_POERequest newPoeRequest = new Poe_POERequest
        //            {
        //                Id = poeRequestId,
        //                CustomerId = customerId,
        //                Status = Dictionaries.Submitted,
        //                StartDate = DateTime.Now,
        //            };
        //            SubmitPoeRequestResponse.Add(new SubmitPoeRequestResponse
        //            {
        //                CustomerId = customerId,
        //                PoeRequestId = poeRequestId
        //            });
        //            DefaultEntityBase(ref newPoeRequest);
        //            newPOERequests.Add(newPoeRequest);
        //        }
        //        await _POEContext.AddRangeAsync(newPOERequests);
        //        _POEContext.UpdateRange(updatePOERequests);
        //        int count = _POEContext.SaveChanges();
        //        apiResult.Data = SubmitPoeRequestResponse;
        //    }
        //    catch (Exception ex)
        //    {
        //        apiResult.Code = -1;
        //        apiResult.Msg = ex.ToString();
        //    }
        //    return apiResult;
        //}

        //public byte[] DownLoadTemplate(string templateName)
        //{
        //    FileStream fs = new FileStream($@"E:\POE Storage\PoeRequestTemplates\{templateName}", FileMode.Open, FileAccess.Read);
        //    byte[] infbytes = new byte[(int)fs.Length];
        //    fs.Read(infbytes, 0, infbytes.Length);
        //    fs.Close();
        //    return infbytes;
        //}

        //public ApiResult PoeRequestFileDelete(string id, List<string> fileNames)
        //{
        //    ApiResult apiResult = new ApiResult();
        //    try
        //    {
        //        string dir = $@"E:\POE Storage\PoeRequestUploadFiles\{id}";
        //        if (Directory.Exists(dir))
        //        {
        //            foreach (string fileName in fileNames)
        //            {
        //                File.Delete($@"{dir}\{fileName}");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        apiResult.Code = -1;
        //        apiResult.Msg = ex.ToString();
        //    }
        //    return apiResult;
        //}
    }
}
