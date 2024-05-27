using Microsoft.AspNetCore.Http;
using POEMgr.Application.TransferModels;
using POEMgr.Repository;
using POEMgr.Repository.CommonQuery;
using System.Net;

namespace POEMgr.Application.Interfaces
{
    public interface IIncentiveService
    {
        Task<ApiResult> Incentive_list_get();
        Task<ApiResult> Incentive_detail_get(string id);
        Task<ApiResult> Incentive_add(Incentive_add_req p);
        Task<ApiResult> Incentive_delete(string p);
        Task<ApiResult> Incentive_update(string id, Incentive_update_req p);
        Task<ApiResult> Incentive_uploadFile(IFormFile p);
        Task<ApiResult> Incentive_removeFile(string p);
        Task<ApiResult> Incentive_excelImport(IFormFile model);
        Task<ApiResult> Incentive_filter_get();
        Task<ApiResult> Incentive_manage_list_get(GetIncentivesManageRequest model);

        Task<ApiResult> Incentive_manage_sendMail(List<string> id);
    }
}