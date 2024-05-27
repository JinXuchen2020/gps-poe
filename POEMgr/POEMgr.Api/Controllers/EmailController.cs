using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using POEMgr.Application;
using POEMgr.Application.Interfaces;
using POEMgr.Application.TransferModels;

namespace POEMgr.Api.Controllers
{
    [Route("api/emails")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IPoeEmailService _poeEmailService;

        public EmailController(IPoeEmailService poeEmailService)
        {
            _poeEmailService = poeEmailService;
        }

        [HttpPost]
        [Route("send")]
        public async Task<IActionResult> PoeRequest_sendMail([FromBody] PoeRequest_email_send_req request)
        {
            var result = new List<string>();
            foreach(var id in request.RequestIds)
            {
                var sendResult = await _poeEmailService.SendNotifyEmailAsync(id, request.Type);
                if(!string.IsNullOrEmpty(sendResult)) result.Add(sendResult);
            }

            ApiResult apiResult = new ApiResult()
            {
                Code = result.Count == 0 ? 0 : -1,
                Data = null,
                Msg = string.Join(";", result)
            };

            return Ok(apiResult);
        }
    }
}
