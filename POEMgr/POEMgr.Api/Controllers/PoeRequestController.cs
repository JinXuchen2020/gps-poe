using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPOI.HPSF;
using POEMgr.Application.Interfaces;
using POEMgr.Application.TransferModels;
using POEMgr.Repository.CommonQuery;
using System.Net;

namespace POEMgr.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/poeRequests")]
    public class PoeRequestController : ControllerBase
    {
        private readonly IPoeRequestService _poeRequestService;

        public PoeRequestController(IPoeRequestService poeRequestService)
        {
            this._poeRequestService = poeRequestService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> PoeRequest_list_get([FromQuery] PoeRequest_list_get_req model)
        {
            return Ok(await _poeRequestService.PoeRequest_list_get(model));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> PoeRequest_detail_get(string id)
        {
            return Ok(await _poeRequestService.PoeRequest_detail_get(id));
        }

        [HttpGet]
        [Route("downLoadTemplate")]
        public async Task<IActionResult> PoeRequest_downLoadTemplateFile(string id)
        {
            var res = await _poeRequestService.PoeRequest_downLoadTemplateFile(id);
            return File(res.Stream, "application/octet-stream", $@"{Convert.ToInt64((DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds)}{res.FileName}");
        }

        [HttpPost]
        [Route("uploadFile")]
        public async Task<IActionResult> PoeRequest_uploadFile(IFormFile file)
        {
            return Ok(await _poeRequestService.PoeRequest_uploadFile(file));
        }

        [HttpDelete]
        [Route("removeFile")]
        public async Task<IActionResult> PoeRequest_removeFile(string id)
        {
            return Ok(await _poeRequestService.PoeRequest_removeFile(id));
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> PoeRequestSave(string id, PoeRequest_save_req p)
        {
            return Ok(await _poeRequestService.PoeRequest_save(id, p));
        }

        [HttpGet]
        [Route("exportExcel")]
        public async Task<IActionResult> PoeRequest_exportExcel([FromQuery] PoeRequest_exportExcel_req p)
        {
            return File(await _poeRequestService.PoeRequest_exportExcel(p), "application/octet-stream", $@"PoeRequestExport{Convert.ToInt64((DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds)}.xlsx");
        }
    }
}
