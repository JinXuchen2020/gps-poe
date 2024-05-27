using Authentication;
using AutoMapper;
using Azure.Storage.Blobs;
using LogService;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using POEMgr.Application.Interfaces;
using POEMgr.Application.TransferModels;
using POEMgr.Repository.DBContext;
using POEMgr.Repository.DbModels;

namespace POEMgr.Application.Services
{
    internal class IncentiveService: BaseService, IIncentiveService
    {
        private readonly IPoeFileService _iPoeFileService;

        public IncentiveService(IMapper mapper, POEContext poeContext, IClaimsAccessor claimsAccessor, IPoeFileService fileService, IPoeLogService poeLogService) 
            : base(mapper, poeContext, claimsAccessor, poeLogService)
        {
            _iPoeFileService = fileService;
        }

        public async Task<ApiResult> Incentive_uploadFile(IFormFile p)
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                Incentive_uploadFile_res res = new Incentive_uploadFile_res();
                //string dir = $@"E:\POEStorage\IncentiveFileModels";
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
                string blobUri = _iPoeFileService.SaveFileToBlob(st, p.FileName, p.ContentType, "IncentiveFileModels");
                Poe_POEFile.Path = blobUri;
                Poe_POEFile.BlobUri = blobUri;
                res.Id = Poe_POEFile.Id;
                res.Name = Poe_POEFile.FileName;
                res.Path = Poe_POEFile.Path;
                res.BlobUri= Poe_POEFile.BlobUri;
                _POEContext.Add(WriteEntityBase(Poe_POEFile));
                await _POEContext.SaveChangesAsync();
                apiResult.Data = res;
                WriteDbLog(DbLogType.Create, "Incentive_uploadFile", JsonConvert.SerializeObject(Poe_POEFile));
            }
            catch (Exception ex)
            {
                apiResult.Code = -1;
                apiResult.Msg = ex.ToString();
            }
            return apiResult;
        }

        public async Task<ApiResult> Incentive_removeFile(string p)
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                Poe_POEFile removePoe_POEFile = await _POEContext.Poe_POEFile.Where(x=>x.Id==p).FirstOrDefaultAsync();
                if (removePoe_POEFile!=null)
                {
                    _iPoeFileService.DeleteFileFromBlob(removePoe_POEFile.FileName, "IncentiveFileModels");
                    _POEContext.Poe_POEFile.Remove(removePoe_POEFile);
                    apiResult.Data = _POEContext.SaveChanges();
                    WriteDbLog(DbLogType.Delete, "Incentive_removeFile", JsonConvert.SerializeObject(removePoe_POEFile));
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

        public async Task<ApiResult> Incentive_add(Incentive_add_req p)
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                var idCheck = await _POEContext.Poe_Incentive.Where(x => x.IncentiveName == p.Name).FirstOrDefaultAsync();
                if (idCheck != null)
                {
                    apiResult.Code = 1;
                    apiResult.Msg = $@"IncentiveName: {p.Name} already exists.";
                    apiResult.Data = p;
                    return apiResult;
                }

                Poe_Incentive poe_Incentive = new Poe_Incentive();
                poe_Incentive.Id = Guid.NewGuid().ToString();
                poe_Incentive.IncentiveName = p.Name;
                poe_Incentive.WelcomeSpeech = p.WelcomeSpeech;
                poe_Incentive.StartDate = Convert.ToDateTime(p.StartDate).Date;
                poe_Incentive.EndDate = Convert.ToDateTime(p.EndDate).Date;
                poe_Incentive.SubmitDeadlineDay = p.SubmitDeadlineDay;
                poe_Incentive.RemindEmailDay = p.RemindEmailDay;
                poe_Incentive.ReSubmitDeadlineDay = p.ReSubmitDeadlineDay;
                poe_Incentive.RejectCount = p.RejectCount;

                List<Poe_CheckPoint> poe_CheckPoints = new List<Poe_CheckPoint>();
                if (p.CheckPoints != null)
                {
                    poe_CheckPoints = p.CheckPoints
                        .Select(x => new Poe_CheckPoint
                        {
                            Id = Guid.NewGuid().ToString(),
                            IncentiveId = poe_Incentive.Id,
                            Content = x
                        }).ToList();
                }

                List<Poe_MailTemplate> poe_MailTemplates = new List<Poe_MailTemplate>();
                if (p.MailTemplates!=null)
                {
                    poe_MailTemplates = p.MailTemplates
                    .Select(x => new Poe_MailTemplate
                    {
                        Id = Guid.NewGuid().ToString(),
                        IncentiveId = poe_Incentive.Id,
                        Type = x.Type,
                        Content = x.Content,
                    }).ToList();
                }

                List<Poe_POEFile> updatePoe_POEFiles = _POEContext.Poe_POEFile.Where(x => p.FileIds.Contains(x.Id)).ToList();
                foreach (Poe_POEFile poe_POEFileItem in updatePoe_POEFiles)
                {
                    if (!string.IsNullOrEmpty(poe_POEFileItem.IncentiveId))
                    {
                        Poe_POEFile Poe_POEFile = new Poe_POEFile();
                        Poe_POEFile.Id = Guid.NewGuid().ToString();
                        Poe_POEFile.FileName = poe_POEFileItem.FileName;
                        var fileContent = _iPoeFileService.GetFileFromBlob(poe_POEFileItem.BlobUri);
                        var blob = new BlobClient(new Uri(poe_POEFileItem.BlobUri));
                        var contentType = blob.GetProperties().Value.ContentType;
                        var st = new MemoryStream(fileContent);
                        string blobUri = _iPoeFileService.SaveFileToBlob(st, poe_POEFileItem.FileName, contentType, "IncentiveFileModels");
                        Poe_POEFile.Path = blobUri;
                        Poe_POEFile.BlobUri = blobUri;
                        Poe_POEFile.IncentiveId = poe_Incentive.Id;
                        await _POEContext.AddAsync(WriteEntityBase(Poe_POEFile));
                    }
                    else
                    {
                        poe_POEFileItem.IncentiveId = poe_Incentive.Id;
                    }
                }

                await _POEContext.AddRangeAsync(WriteEntityBase(poe_CheckPoints));
                await _POEContext.AddRangeAsync(WriteEntityBase(poe_MailTemplates));
                await _POEContext.AddAsync(WriteEntityBase(poe_Incentive));
                 _POEContext.UpdateRange(WriteEntityBase(updatePoe_POEFiles));
                _POEContext.SaveChanges();
                apiResult.Data = poe_Incentive.Id;
                WriteDbLog(DbLogType.Create, "Incentive_add", poe_Incentive.Id);
            }
            catch (Exception ex)
            {
                apiResult.Code = -1;
                apiResult.Msg = ex.ToString();
            }
            return apiResult;
        }

        public async Task<ApiResult> Incentive_update(string id,Incentive_update_req p)
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                Poe_Incentive poe_Incentive = await _POEContext.Poe_Incentive.Where(x => x.Id == id).FirstOrDefaultAsync();
                if (poe_Incentive == null)
                {
                    apiResult.Code = 1;
                    apiResult.Msg = $@"IncentiveId: {id} not found.";
                    apiResult.Data = p;
                    return apiResult;
                }

                poe_Incentive.IncentiveName = string.IsNullOrWhiteSpace(p.Name)? poe_Incentive.IncentiveName : p.Name;
                poe_Incentive.WelcomeSpeech = string.IsNullOrWhiteSpace(p.WelcomeSpeech) ? poe_Incentive.WelcomeSpeech : p.WelcomeSpeech;
                poe_Incentive.StartDate = string.IsNullOrWhiteSpace(p.StartDate) ? poe_Incentive.StartDate : Convert.ToDateTime(p.StartDate).Date;
                poe_Incentive.EndDate = string.IsNullOrWhiteSpace(p.EndDate) ? poe_Incentive.EndDate : Convert.ToDateTime(p.EndDate).Date;
                poe_Incentive.SubmitDeadlineDay = p.SubmitDeadlineDay==null ? poe_Incentive.SubmitDeadlineDay : p.SubmitDeadlineDay;
                poe_Incentive.RemindEmailDay = p.RemindEmailDay == null ? poe_Incentive.RemindEmailDay : p.RemindEmailDay;
                poe_Incentive.ReSubmitDeadlineDay = p.ReSubmitDeadlineDay==null ? poe_Incentive.ReSubmitDeadlineDay : p.ReSubmitDeadlineDay;
                poe_Incentive.RejectCount = p.RejectCount == null ? poe_Incentive.RejectCount : p.RejectCount;

                List<Poe_CheckPoint> newPoe_CheckPoints = p.CheckPoints
                    .Select(x => new Poe_CheckPoint
                    {
                        Id = Guid.NewGuid().ToString(),
                        IncentiveId = poe_Incentive.Id,
                        Content = x
                    }).ToList();
                List<Poe_CheckPoint> removePoe_CheckPoints = _POEContext.Poe_CheckPoint.Where(x => x.IncentiveId == id).ToList();

                List<Poe_POEFile> updatePoe_POEFiles = _POEContext.Poe_POEFile.Where(x => x.IncentiveId==id).ToList();
                updatePoe_POEFiles.ForEach(x => x.IncentiveId = null);
                List<Poe_POEFile> updatePoe_POEFiles1 = _POEContext.Poe_POEFile.Where(x => p.FileIds.Contains(x.Id)).ToList();
                updatePoe_POEFiles1.ForEach(x => x.IncentiveId = poe_Incentive.Id);

                List<Poe_MailTemplate> newPoe_MailTemplates = p.MailTemplates
                    .Select(x => new Poe_MailTemplate
                    {
                        Id = Guid.NewGuid().ToString(),
                        IncentiveId = poe_Incentive.Id,
                        Type = x.Type,
                        Content = x.Content,
                    }).ToList();
                List<Poe_MailTemplate> removePoe_MailTemplates = _POEContext.Poe_MailTemplate.Where(x => x.IncentiveId == id).ToList();

                _POEContext.RemoveRange(removePoe_CheckPoints);
                await _POEContext.AddRangeAsync(WriteEntityBase(newPoe_CheckPoints));
                _POEContext.RemoveRange(removePoe_MailTemplates);
                await _POEContext.AddRangeAsync(WriteEntityBase(newPoe_MailTemplates));
                _POEContext.Update(WriteEntityBase(poe_Incentive));
                _POEContext.UpdateRange(WriteEntityBase(updatePoe_POEFiles));
                _POEContext.UpdateRange(WriteEntityBase(updatePoe_POEFiles1));
                _POEContext.SaveChanges();
                WriteDbLog(DbLogType.Update, "Incentive_update", id);
            }
            catch (Exception ex)
            {
                apiResult.Code = -1;
                apiResult.Msg = ex.ToString();
            }
            return apiResult;
        }

        public async Task<ApiResult> Incentive_detail_get(string id)
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                Poe_Incentive poe_Incentive = await _POEContext.Poe_Incentive.Where(x => x.Id == id).FirstOrDefaultAsync();
                if (poe_Incentive == null)
                {
                    apiResult.Code = 1;
                    apiResult.Msg = $@"Can not find IncentiveId: {id}.";
                    apiResult.Data = id;
                    return apiResult;
                }

                List<Poe_CheckPoint> poe_CheckPoints = _POEContext.Poe_CheckPoint.Where(x => x.IncentiveId == id).ToList();
                List<Poe_MailTemplate> poe_MailTemplates = _POEContext.Poe_MailTemplate.Where(x => x.IncentiveId == id).ToList();
                List<Poe_POEFile> poe_POEFiles = _POEContext.Poe_POEFile.Where(x => x.IncentiveId == id).ToList();
                Incentive_detail_get_res Incentive_detail_get_res = new Incentive_detail_get_res();
                Incentive_detail_get_res.Id = id;
                Incentive_detail_get_res.Name = poe_Incentive.IncentiveName;
                Incentive_detail_get_res.WelcomeSpeech = poe_Incentive.WelcomeSpeech;
                Incentive_detail_get_res.StartDate = Convert.ToDateTime(poe_Incentive.StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                Incentive_detail_get_res.EndDate = Convert.ToDateTime(poe_Incentive.EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                Incentive_detail_get_res.SubmitDeadlineDay = poe_Incentive.SubmitDeadlineDay;
                Incentive_detail_get_res.RemindEmailDay = poe_Incentive.RemindEmailDay;
                Incentive_detail_get_res.ReSubmitDeadlineDay = poe_Incentive.ReSubmitDeadlineDay;
                Incentive_detail_get_res.RejectCount = poe_Incentive.RejectCount;
                Incentive_detail_get_res.CheckPoints = poe_CheckPoints.Select(x => new Incentive_detail_get_res_checkpoint
                {
                    Id = x.Id,
                    Content = x.Content
                }).ToList();
                Incentive_detail_get_res.MailTemplates = poe_MailTemplates.Select(x => new Incentive_detail_get_res_mailTemplate 
                {
                    Id = x.Id,
                    Type = x.Type,
                    Content = x.Content
                }).ToList();
                Incentive_detail_get_res.Files = poe_POEFiles.Select(x => new Incentive_detail_get_res_fileTemplate
                {
                    Id = x.Id,
                    Name = x.FileName,
                    Path = x.BlobUri
                }).ToList();
                apiResult.Data = Incentive_detail_get_res;
            }
            catch (Exception ex)
            {
                apiResult.Code = -1;
                apiResult.Msg = ex.ToString();
            }
            return apiResult;
        }

        public async Task<ApiResult> Incentive_list_get()
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                var query = from incentive in _POEContext.Poe_Incentive
                            join user in _POEContext.Sys_User on incentive.CreatedBy equals user.Id into users
                            from ue in users.DefaultIfEmpty()
                            select new Incentive_list_get_res
                            {
                                Id = incentive.Id,
                                Name = incentive.IncentiveName,
                                StartDate = Convert.ToDateTime(incentive.StartDate).ToString("yyyy-MM-dd"),
                                EndDate = Convert.ToDateTime(incentive.EndDate).ToString("yyyy-MM-dd"),
                                CreatedName = ue == null ? string.Empty : ue.UserName,
                            };

                var res = await query.ToListAsync();
                if (_partnerIds.Any())
                {
                    var currentIncentives = await _POEContext.Poe_PartnerIncentive.Where(c => _partnerIds.Contains(c.PartnerId)).Select(c => c.IncentiveId).Distinct().ToListAsync();
                    res = res.Where(c => currentIncentives.Contains(c.Id)).ToList();
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

        public async Task<ApiResult> Incentive_delete(string p)
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                Poe_Incentive poe_Incentives = await _POEContext.Poe_Incentive.Where(x => x.Id == p).FirstOrDefaultAsync();
                List<Poe_CheckPoint> poe_CheckPoints = await _POEContext.Poe_CheckPoint.Where(x => x.IncentiveId == p).ToListAsync();
                List<Poe_MailTemplate> poe_MailTemplates = await _POEContext.Poe_MailTemplate.Where(x => x.IncentiveId == p).ToListAsync();
                List<Poe_POEFile> poe_POEFiles = await _POEContext.Poe_POEFile.Where(x => x.IncentiveId == p).ToListAsync();
                _POEContext.Poe_Incentive.Remove(poe_Incentives);
                _POEContext.Poe_CheckPoint.RemoveRange(poe_CheckPoints);
                _POEContext.Poe_MailTemplate.RemoveRange(poe_MailTemplates);
                _POEContext.Poe_POEFile.RemoveRange(poe_POEFiles);
                _POEContext.SaveChanges();
                WriteDbLog(DbLogType.Delete, "Incentive_delete", p);
            }
            catch (Exception ex)
            {
                apiResult.Code = -1;
                apiResult.Msg = ex.ToString();
            }
            return apiResult;
        }

        public async Task<ApiResult> Incentive_excelImport(IFormFile p)
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                string sourceString = await Task.Run(() => { return _iPoeFileService.ReadExcel(p, 0); });
                List<Incentive_excelImport_req> excelData = JsonConvert.DeserializeObject<List<Incentive_excelImport_req>>(sourceString);
                List<Poe_Incentive> poe_Incentive = _POEContext.Poe_Incentive.ToList();
                List<Poe_Customer> poe_Customer = _POEContext.Poe_Customer.ToList();
                List<Poe_Partner> poe_Partner = _POEContext.Poe_Partner.ToList();
                List<Sys_User> sys_User = _POEContext.Sys_User.ToList();
                List<Poe_Subscription> poe_Subscription = _POEContext.Poe_Subscription.ToList();
                List<Poe_SubscriptionStatus> poe_SubscriptionStatus = _POEContext.Poe_SubscriptionStatus.ToList();
                List<Poe_RequestPhase> poe_RequestPhase = _POEContext.Poe_RequestPhase.ToList();
                List<Poe_POERequest> poe_POERequests = _POEContext.Poe_POERequest.ToList();

                List<Poe_Partner> newPartners = new List<Poe_Partner>();
                List<Poe_Partner> updatePartners = new List<Poe_Partner>();
                List<Poe_PartnerIncentive> newPartnerIncentives = new List<Poe_PartnerIncentive>();
                List<Poe_PartnerIncentive> updatePartnerIncentives = new List<Poe_PartnerIncentive>();
                List<Sys_User> newUsers = new List<Sys_User>();
                List<Poe_Customer> newCustomers = new List<Poe_Customer>();
                List<Poe_Incentive> newIncentives = new List<Poe_Incentive>();
                List<Poe_Subscription> newSubscriptions = new List<Poe_Subscription>();
                List<Poe_SubscriptionStatus> newSubscriptionStatuss = new List<Poe_SubscriptionStatus>();
                List<Poe_RequestPhase> newRequestPhases = new List<Poe_RequestPhase>();
                List<Sys_UserRole> newUserRoles = new List<Sys_UserRole>();
                List<Poe_POERequest> newPoe_POERequests = new List<Poe_POERequest>();

                List<string> notCreatedIncentives = new List<string>();
                List<Incentive_excelImport_Res> res = new List<Incentive_excelImport_Res>();
                foreach (Incentive_excelImport_req item in excelData)
                {
                    //Poe_Incentive
                    Poe_Incentive queryPoe_Incentive = poe_Incentive.Where(x => x.IncentiveName.ToLower() == item.IncentiveName.TrimEnd().TrimStart().ToLower()).FirstOrDefault();
                    if (queryPoe_Incentive==null)
                    {
                        if (!notCreatedIncentives.Any(x => x == item.IncentiveName.TrimEnd().TrimStart()))
                        {
                            notCreatedIncentives.Add(item.IncentiveName.TrimEnd().TrimStart());
                        }
                        continue;
                    }
                    item.IncentiveId = queryPoe_Incentive.Id;
                    //poe_RequestPhase
                    Poe_RequestPhase queryPoe_RequestPhase = poe_RequestPhase.Where(x => x.IncentiveId == item.IncentiveId).FirstOrDefault();
                    Poe_RequestPhase queryPoe_RequestPhase1 = newRequestPhases.Where(x => x.IncentiveId == item.IncentiveId).FirstOrDefault();
                    string requestPhaseID = Guid.NewGuid().ToString();
                    if (queryPoe_RequestPhase == null && queryPoe_RequestPhase1 == null)
                    {
                        newRequestPhases.Add(new Poe_RequestPhase
                        {
                            Id = requestPhaseID,
                            IncentiveId = item.IncentiveId,
                        });
                    }
                    else
                    {
                        requestPhaseID = queryPoe_RequestPhase != null ? queryPoe_RequestPhase.Id : queryPoe_RequestPhase1.Id;
                    }

                    string partnerMailResponse = string.Empty;
                    string partnerManagerMailResponse = string.Empty;

                    //Poe_Partner
                    if (!string.IsNullOrWhiteSpace(item.PartnerId))
                    {
                        Poe_Partner partnerDbFilter = poe_Partner.Where(x => x.Id == item.PartnerId).FirstOrDefault();
                        Poe_Partner partnerNewFilter = newPartners.Where(x => x.Id == item.PartnerId).FirstOrDefault();
                        if (partnerDbFilter == null && partnerNewFilter == null)
                        {
                            Poe_Partner newPartner = new Poe_Partner
                            {
                                Id = item.PartnerId,
                                PartnerOneId = item.PartnerId,
                                PartnerName = item.PartnerName,
                                Mail = item.PartnerMail,
                                PhaseId = requestPhaseID,
                                IsDisabled = Dictionaries.No,
                                MailCC = item.PartnerManagerMail,
                            };
                            newPartners.Add(newPartner);
                        }
                        else if (partnerDbFilter != null)
                        {
                            if (!updatePartners.Any(x => x.Id == item.PartnerId))
                            {
                                updatePartners.Add(partnerDbFilter);
                            }
                            foreach (Poe_Partner x in updatePartners)
                            {
                                if (x.Id == item.PartnerId)
                                {
                                    if (!string.IsNullOrEmpty(item.PartnerManagerMail) && (string.IsNullOrEmpty(x.MailCC) || !x.MailCC.Contains(item.PartnerManagerMail)))
                                    {
                                        x.MailCC += string.IsNullOrEmpty(x.MailCC) ? string.Empty : ';' + $"{item.PartnerManagerMail}";
                                    }
                                    if (!string.IsNullOrEmpty(item.PartnerMail) && (string.IsNullOrEmpty(x.Mail) || !x.Mail.Contains(item.PartnerMail)))
                                    {
                                        x.Mail += string.IsNullOrEmpty(x.Mail) ? string.Empty : ';' + $"{item.PartnerMail}";
                                    }
                                    break;
                                }
                            }
                        }
                        else if (partnerNewFilter != null)
                        {
                            foreach (Poe_Partner x in newPartners)
                            {
                                if (x.Id == item.PartnerId)
                                {
                                    if (!string.IsNullOrEmpty(item.PartnerManagerMail) && (string.IsNullOrEmpty(x.MailCC) || !x.MailCC.Contains(item.PartnerManagerMail)))
                                    {
                                        x.MailCC += string.IsNullOrEmpty(x.MailCC) ? string.Empty : ';' + $"{item.PartnerManagerMail}";
                                    }
                                    if (!string.IsNullOrEmpty(item.PartnerMail) && (string.IsNullOrEmpty(x.Mail) || !x.Mail.Contains(item.PartnerMail)))
                                    {
                                        x.Mail += string.IsNullOrEmpty(x.Mail) ? string.Empty : ';' + $"{item.PartnerMail}";
                                    }
                                    break;
                                }
                            }
                        }

                        Poe_PartnerIncentive partnerIncentiveDbFilter = _POEContext.Poe_PartnerIncentive.Where(x => x.PartnerId == item.PartnerId && x.IncentiveId == item.IncentiveId).FirstOrDefault();
                        Poe_PartnerIncentive partnerIncentiveNewFilter = newPartnerIncentives.Where(x => x.PartnerId == item.PartnerId && x.IncentiveId == item.IncentiveId).FirstOrDefault();

                        if (partnerIncentiveDbFilter == null && partnerIncentiveNewFilter == null)
                        {
                            newPartnerIncentives.Add(new Poe_PartnerIncentive
                            {
                                Id = Guid.NewGuid().ToString(),
                                PartnerId = item.PartnerId,
                                IncentiveId = item.IncentiveId,
                                Mail = item.PartnerMail,
                                MailCC = item.PartnerManagerMail,
                            });
                            partnerMailResponse = item.PartnerMail;
                            partnerManagerMailResponse = item.PartnerManagerMail;
                        }
                        else if (partnerIncentiveDbFilter != null)
                        {
                            if (!updatePartnerIncentives.Any(x => x.Id == item.PartnerId))
                            {
                                updatePartnerIncentives.Add(partnerIncentiveDbFilter);
                            }
                            foreach (Poe_PartnerIncentive x in updatePartnerIncentives)
                            {
                                if (x.PartnerId == item.PartnerId && x.IncentiveId == item.IncentiveId)
                                {
                                    if (!string.IsNullOrEmpty(item.PartnerManagerMail) && (string.IsNullOrEmpty(x.MailCC) || !x.MailCC.Contains(item.PartnerManagerMail)))
                                    {
                                        x.MailCC += string.IsNullOrEmpty(x.MailCC) ? string.Empty : ';' + $"{item.PartnerManagerMail}";
                                    }
                                    if (!string.IsNullOrEmpty(item.PartnerMail) && (string.IsNullOrEmpty(x.Mail) || !x.Mail.Contains(item.PartnerMail)))
                                    {
                                        x.Mail += string.IsNullOrEmpty(x.Mail) ? string.Empty : ';' + $"{item.PartnerMail}";
                                    }
                                    partnerMailResponse = x.Mail;
                                    partnerManagerMailResponse = x.MailCC;
                                    break;
                                }
                            }
                        }
                        else if (partnerIncentiveNewFilter != null)
                        {
                            foreach (Poe_PartnerIncentive x in newPartnerIncentives)
                            {
                                if (x.PartnerId == item.PartnerId && x.IncentiveId == item.IncentiveId)
                                {
                                    if (!string.IsNullOrEmpty(item.PartnerManagerMail) && (string.IsNullOrEmpty(x.MailCC) || !x.MailCC.Contains(item.PartnerManagerMail)))
                                    {
                                        x.MailCC += string.IsNullOrEmpty(x.MailCC) ? string.Empty : ';' + $"{item.PartnerManagerMail}";
                                    }
                                    if (!string.IsNullOrEmpty(item.PartnerMail) && (string.IsNullOrEmpty(x.Mail) || !x.Mail.Contains(item.PartnerMail)))
                                    {
                                        x.Mail += string.IsNullOrEmpty(x.Mail) ? string.Empty : ';' + $"{item.PartnerMail}";
                                    }
                                    partnerMailResponse = x.Mail;
                                    partnerManagerMailResponse = x.MailCC;
                                    break;
                                }
                            }
                        }
                    }

                    //Poe_Customer
                    string customerId = string.Empty;
                    Poe_Customer queryPoe_Customer = poe_Customer.Where(x => x.CustomerName.ToLower() == item.TopParent.TrimEnd().TrimStart().ToLower()).FirstOrDefault();
                    Poe_Customer queryPoe_Customer1 = newCustomers.Where(x => x.CustomerName.ToLower() == item.TopParent.TrimEnd().TrimStart().ToLower()).FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(item.TopParent) && queryPoe_Customer == null && queryPoe_Customer1 == null)
                    {
                        Poe_Customer newCustomer = new Poe_Customer
                        {
                            Id = Guid.NewGuid().ToString(),
                            CustomerName = item.TopParent.TrimEnd().TrimStart(),
                            PartnerId = item.PartnerId
                        };
                        newCustomers.Add(newCustomer);
                        customerId = newCustomer.Id;
                    }
                    else
                    {
                        customerId = queryPoe_Customer == null ? queryPoe_Customer1.Id : queryPoe_Customer.Id;
                    }

                    //Poe_Subscription
                    if (!string.IsNullOrWhiteSpace(item.SubscriptionId) && !poe_Subscription.Exists(x => x.Id.ToLower() == item.SubscriptionId.TrimEnd().TrimStart().ToLower()) && !newSubscriptions.Exists(x => x.Id.ToLower() == item.SubscriptionId.TrimEnd().TrimStart().ToLower()))
                    {
                        Poe_Subscription newSubscription = new Poe_Subscription
                        {
                            Id = item.SubscriptionId.TrimEnd().TrimStart(),
                            CustomerId = customerId
                        };
                        newSubscriptions.Add(newSubscription);
                    }

                    //Poe_POERequest
                    string poeRequestId = string.Empty;
                    if (!string.IsNullOrWhiteSpace(item.PartnerId) && !string.IsNullOrWhiteSpace(item.IncentiveId) && !string.IsNullOrWhiteSpace(customerId))
                    {
                        Poe_POERequest poe_POERequest = poe_POERequests.Where(x => x.PartnerId == item.PartnerId && x.IncentiveId == item.IncentiveId && x.CustomerId == customerId && x.Status != Dictionaries.Approved && x.Status != Dictionaries.Expired).FirstOrDefault();
                        Poe_POERequest poe_POERequest1 = newPoe_POERequests.Where(x => x.PartnerId == item.PartnerId && x.IncentiveId == item.IncentiveId && x.CustomerId == customerId).FirstOrDefault();
                        poeRequestId = poe_POERequest != null ? poe_POERequest.Id : poe_POERequest1 != null ? poe_POERequest1.Id : string.Empty;
                        item.EmailSendStatus = poe_POERequest != null ? poe_POERequest.Status : poe_POERequest1 != null ? poe_POERequest1.Status : Dictionaries.Draft;
                        if (string.IsNullOrEmpty(poeRequestId))
                        {
                            Poe_POERequest newPoe_POERequest = new Poe_POERequest
                            {
                                Id = Guid.NewGuid().ToString(),
                                PartnerId = item.PartnerId,
                                IncentiveId = item.IncentiveId,
                                PhaseId = requestPhaseID,
                                CustomerId = customerId,
                                Status = Dictionaries.Draft,
                                //StartDate = DateTime.Now,
                            };
                            newPoe_POERequests.Add(newPoe_POERequest);
                            poeRequestId = newPoe_POERequest.Id;
                        }
                    }

                    //Poe_SubscriptionStatus
                    if (!string.IsNullOrWhiteSpace(item.SubscriptionId) && !poe_SubscriptionStatus.Exists(x => x.SubscriptionId.ToLower() == item.SubscriptionId.TrimEnd().TrimStart().ToLower() && x.PoeRequestId == poeRequestId) && !newSubscriptionStatuss.Exists(x => x.Id.ToLower() == item.SubscriptionId.TrimEnd().TrimStart().ToLower() && x.PoeRequestId == poeRequestId))
                    {
                        Poe_SubscriptionStatus newSubscriptionStatus = new Poe_SubscriptionStatus
                        {
                            Id= Guid.NewGuid().ToString(),
                            SubscriptionId = item.SubscriptionId.TrimEnd().TrimStart(),
                            PoeRequestId = poeRequestId
                        };
                        newSubscriptionStatuss.Add(newSubscriptionStatus);
                    }

                    Incentive_excelImport_Res resFilter = res.Where(x => x.Id == poeRequestId).FirstOrDefault();
                    res.Remove(res.Where(x => x.Id == poeRequestId).FirstOrDefault());                
                    res.Add(new Incentive_excelImport_Res
                    {
                        Id = poeRequestId,
                        IncentiveId = item.IncentiveId,
                        IncentiveName = item.IncentiveName,
                        PartnerId = item.PartnerId,
                        PartnerName = item.PartnerName,
                        PartnerEmail = partnerMailResponse,
                        PartnerManagerMail = partnerManagerMailResponse,
                        CustomerName = item.TopParent.TrimEnd().TrimStart(),
                        TotalCustomerName = item.CustomerName.TrimEnd().TrimStart(),
                        Status = item.EmailSendStatus,
                        TPID = item.TPID
                    });              
                }
                await _POEContext.AddRangeAsync(WriteEntityBase(newIncentives));
                await _POEContext.AddRangeAsync(WriteEntityBase(newCustomers));
                await _POEContext.AddRangeAsync(WriteEntityBase(newPartners));
                _POEContext.UpdateRange(WriteEntityBase(updatePartners));
                await _POEContext.AddRangeAsync(WriteEntityBase(newPartnerIncentives));
                _POEContext.UpdateRange(WriteEntityBase(updatePartnerIncentives));
                await _POEContext.AddRangeAsync(WriteEntityBase(newUsers));
                await _POEContext.AddRangeAsync(WriteEntityBase(newUserRoles));
                await _POEContext.AddRangeAsync(WriteEntityBase(newSubscriptions));
                await _POEContext.AddRangeAsync(WriteEntityBase(newSubscriptionStatuss));
                await _POEContext.AddRangeAsync(WriteEntityBase(newRequestPhases));
                await _POEContext.AddRangeAsync(WriteEntityBase(newPoe_POERequests));
                _POEContext.SaveChanges();
                apiResult.Data = res;
                WriteDbLog(DbLogType.Create, "Incentive_excelImport", string.Join(',', newPoe_POERequests.Select(x=>x.Id).ToList()));
            }
            catch (Exception ex)
            {
                apiResult.Code = -1;
                apiResult.Msg = ex.ToString();
            }
            return apiResult;
        }

        public async Task<ApiResult> Incentive_filter_get()
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                List<Poe_Incentive> poe_Incentives = await _POEContext.Poe_Incentive.ToListAsync();
                List<Incentive_filter_get_res> res = poe_Incentives.Select(x => new Incentive_filter_get_res { 
                    Id=x.Id,
                    Name=x.IncentiveName
                }).ToList();
                apiResult.Data = res;
            }
            catch (Exception ex)
            {
                apiResult.Code = -1;
                apiResult.Msg = ex.ToString();
            }
            return apiResult;
        }

        public async Task<ApiResult> Incentive_manage_list_get(GetIncentivesManageRequest p)
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                List<Incentive_excelImport_Res> incentivesResponse = await (
                    from a1 in _POEContext.Poe_POERequest
                    join a in _POEContext.Poe_Incentive on a1.IncentiveId equals a.Id
                    join b in _POEContext.Poe_RequestPhase on a.Id equals b.IncentiveId
                    join c in _POEContext.Poe_Partner on b.Id equals c.PhaseId
                    join pi in _POEContext.Poe_PartnerIncentive on new { IId = a.Id, PID = c.Id } equals new { IId = pi.IncentiveId, PID = pi.PartnerId }
                    join d in _POEContext.Poe_Customer on c.Id equals d.PartnerId
                    where (string.IsNullOrEmpty(p.IncentiveId) || a.Id == p.IncentiveId)
                    where (string.IsNullOrEmpty(p.IncentiveName) || a.IncentiveName.ToLower().Contains(p.IncentiveName.ToLower()))
                    select new Incentive_excelImport_Res
                    {
                        Id=a1.Id,
                        IncentiveId=a.Id,
                        IncentiveName=a.IncentiveName,
                        PartnerId = c.Id,
                        PartnerName = c.PartnerName,
                        PartnerEmail = pi.Mail,
                        CustomerName = d.CustomerName,
                        Status = a1.Status,
                        PartnerManagerMail = pi.MailCC
                    }
                ).OrderBy(x => x.PartnerId).ToListAsync();
                apiResult.Data = incentivesResponse;
            }
            catch (Exception ex)
            {
                apiResult.Code = -1;
                apiResult.Msg = ex.ToString();
            }
            return apiResult;
        }

        public async Task<ApiResult> Incentive_manage_sendMail(List<string> ids)
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                foreach (string id in ids)
                {
                    Poe_POERequest poe_POERequest = await _POEContext.Poe_POERequest.Where(x => x.Id == id).FirstOrDefaultAsync();
                    if (poe_POERequest != null)
                    {
                        var incentive = await _POEContext.Poe_Incentive.Where(c => c.Id == poe_POERequest.IncentiveId).FirstOrDefaultAsync();
                        if (poe_POERequest.Status == Dictionaries.Draft || string.IsNullOrEmpty(poe_POERequest.Status))
                        {
                            poe_POERequest.Status = Dictionaries.EmailSent;
                            poe_POERequest.StartDate = DateTime.Now;
                            poe_POERequest.DeadLineDate = WorkEndDayAdd(DateTime.Now, incentive.SubmitDeadlineDay.Value);
                        }

                        Poe_MailSendRecord poe_MailSendRecord = await _POEContext.Poe_MailSendRecord.Where(x => x.PoeRequestId == id && x.Type == Dictionaries.NotifyFirst).OrderByDescending(c => c.CreatedTime).FirstOrDefaultAsync();
                        if (poe_MailSendRecord != null && !string.IsNullOrEmpty(poe_MailSendRecord.ErrorMsg))
                        {
                            var sentList = poe_MailSendRecord.SendTo.Split(";");
                            var errorObj = JsonConvert.DeserializeObject<JObject>(poe_MailSendRecord.ErrorMsg);
                            if (errorObj.TryGetValue("ErrorCode", out var codeToken))
                            {
                                var errorCodePro = codeToken.Value<string>();
                                if (errorCodePro != null && errorCodePro.Equals("Undeliverable"))
                                {
                                    var errorAddress = errorObj.GetValue("EmailAddress").Value<string>();
                                    if (errorAddress != null && sentList.Length == errorAddress.Split(";").Length)
                                    {
                                        poe_POERequest.DeadLineDate = WorkEndDayAdd(DateTime.Now, incentive.SubmitDeadlineDay.Value);
                                    }
                                }
                            }                            
                        }
                        
                        _POEContext.Update(poe_POERequest);
                        _POEContext.SaveChanges();
                    }
                    WriteDbLog(DbLogType.Create, "Incentive_manage_sendMail", string.Join(',', ids));
                }
            }
            catch (Exception ex)
            {
                apiResult.Code = -1;
                apiResult.Msg = ex.ToString();
            }
            return apiResult;
        }
    }
}
