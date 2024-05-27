using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPOI.HPSF;
using POEMgr.Application;
using POEMgr.Application.Interfaces;
using POEMgr.Application.TransferModels;
using POEMgr.Repository.CommonQuery;

namespace POEMgr.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/auditRequests")]
    public class AuditsController : ControllerBase
    {
        private readonly IPoeRequestService _poeRequestService;

        public AuditsController(IPoeRequestService poeRequestService)
        {
            this._poeRequestService = poeRequestService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> PoeRequest_audit_list_get([FromQuery] PoeRequest_audit_list_get_req model)
        {
            return Ok(await _poeRequestService.PoeRequest_audit_list_get(model));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> PoeRequest_audit_detail_get(string id)
        {
            return Ok(await _poeRequestService.PoeRequest_detail_get(id));
        }

        [HttpGet]
        [Route("{id}/downloadfiles")]
        public async Task<IActionResult> PoeRequest_downLoadZipFile(string id)
        {
            return File(await _poeRequestService.PoeRequest_downLoadZipFile(id), "application/x-zip-compressed", $@"PoeRequestFiles{Convert.ToInt64((DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds)}.zip");
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Approve(string id, AuditPoeRequestRequest model)
        {
            if(model.Status == Dictionaries.Approved || model.Status == Dictionaries.PartialApproved)
            {
                return Ok(await _poeRequestService.PoeRequest_approvePoeRequest(id, model));
            }
            else if (model.Status == Dictionaries.Rejected && !string.IsNullOrEmpty(model.Reason))
            {
                return Ok(await _poeRequestService.PoeRequest_rejectPoeRequest(id, new RejectPoeRequestRequest { RejectReason = model.Reason }));
            }
            else
            {
                return Ok(await _poeRequestService.PoeRequest_saveAuditPoeRequest(id, model));
            }
        }

        [HttpGet]
        [Route("exportExcel")]
        public async Task<IActionResult> PoeRequest_exportExcel([FromQuery] PoeRequest_exportExcel_req p)
        {
            return File(await _poeRequestService.PoeRequest_audit_exportExcel(p), "application/octet-stream", $@"PoeRequestExport{Convert.ToInt64((DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds)}.xlsx");
        }

        //[HttpPut]
        //[Route("auditListSubmit")]
        //public async Task<IActionResult> SubmitPoeRequests([FromBody] SubmitPoeRequestRequest model)
        //{
        //    return Ok(await _poeRequestService.SubmitPoeRequests(model));
        //}
    }
}
