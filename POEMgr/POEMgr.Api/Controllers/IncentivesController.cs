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
    [Route("api/incentives")]
    public class IncentivesController : ControllerBase
    {
        private readonly IIncentiveService _incentiveService;

        public IncentivesController(IIncentiveService incentiveService)
        {
            _incentiveService = incentiveService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Incentive_list_get()
        {
            return Ok(await _incentiveService.Incentive_list_get());
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Incentive_detail_get(string id)
        {
            return Ok(await _incentiveService.Incentive_detail_get(id));
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Incentive_delete(string id)
        {
            return Ok(await _incentiveService.Incentive_delete(id));
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Incentive_add([FromBody] Incentive_add_req p)
        {
            return Ok(await _incentiveService.Incentive_add(p));
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Incentive_update(string id, Incentive_update_req model)
        {
            return Ok(await _incentiveService.Incentive_update(id, model));
        }

        [HttpPost]
        [Route("uploadFile")]
        public async Task<IActionResult> Incentive_uploadFile(IFormFile file)
        {
            return Ok(await _incentiveService.Incentive_uploadFile(file));
        }

        [HttpDelete]
        [Route("removeFile")]
        public async Task<IActionResult> Incentive_removeFile(string id)
        {
            return Ok(await _incentiveService.Incentive_removeFile(id));
        }

        [HttpPost]
        [Route("excelImport")]
        public async Task<IActionResult> Incentive_excelImport(IFormFile file)
        {
            return Ok(await _incentiveService.Incentive_excelImport(file));
        }

        [HttpGet]
        [Route("IncentiveFilter")]
        public async Task<IActionResult> Incentive_filter_get()
        {
            return Ok(await _incentiveService.Incentive_filter_get());
        }

        [HttpGet]
        [Route("manage")]
        public async Task<IActionResult> Incentive_manage_list_get([FromQuery] GetIncentivesManageRequest model)
        {
            return Ok(await _incentiveService.Incentive_manage_list_get(model));
        }


        [HttpPost]
        [Route("sendEmail")]
        public async Task<IActionResult> Incentive_manage_sendMail([FromBody] List<string> p)
        {
            return Ok(await _incentiveService.Incentive_manage_sendMail(p));
        }
    }
}
