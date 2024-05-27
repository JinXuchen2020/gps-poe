using Microsoft.AspNetCore.Http;
using POEMgr.Application.TransferModels;
using POEMgr.Repository;
using POEMgr.Repository.CommonQuery;
using System.Net;

namespace POEMgr.Application.Interfaces
{
    public interface IPoeRequestService
    {
        Task<ApiResult> PoeRequest_list_get(PoeRequest_list_get_req model);

        Task<DownloadFileModel> PoeRequest_downLoadTemplateFile(string id);

        Task<ApiResult> PoeRequest_uploadFile(IFormFile p);

        Task<ApiResult> PoeRequest_removeFile(string p);

        Task<ApiResult> PoeRequest_detail_get(string id);

        Task<ApiResult> PoeRequest_save(string id, PoeRequest_save_req model);

        Task<byte[]> PoeRequest_downLoadZipFile(string id);
        Task<ApiResult> PoeRequest_rejectPoeRequest(string id, RejectPoeRequestRequest model);
        Task<ApiResult> PoeRequest_approvePoeRequest(string id, AuditPoeRequestRequest model);
        Task<ApiResult> PoeRequest_saveAuditPoeRequest(string id, AuditPoeRequestRequest model);
        Task<ApiResult> PoeRequest_audit_list_get(PoeRequest_audit_list_get_req model);
        Task<byte[]> PoeRequest_exportExcel(PoeRequest_exportExcel_req p);
        Task<byte[]> PoeRequest_audit_exportExcel(PoeRequest_exportExcel_req p);
    }
}