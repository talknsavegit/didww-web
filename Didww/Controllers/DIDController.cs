using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using static DIDWW_Api.Model.CountryModel;
using static Didww.Models.DidModel;
using Microsoft.Data.SqlClient;
using System.Data;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using NuGet.Protocol;
using System.Xml;
using DIDWW_Api.Model;
using System.IO;
using Didww.Services;
using ServiceReference1;
using System.Security.Cryptography;

namespace DIDWW_Api.Controllers
{
    [ApiController]
    [Route("api/did")]
    public class DIDController : ControllerBase
    {
        public DIDController()
        {

        }
        [HttpGet("addforwardRule")]
        public async Task<IActionResult> AddForwardRule(string PhoneNumber, string DID, string CountryCode)
        {
            var client = new DisabledCallForwardingSoapClient(0);
            var response = await client.AddForwardRuleAsync(PhoneNumber, DID, CountryCode);
            return Ok(response);
        }

        [HttpGet("removeForwardRule")]
        public async Task<IActionResult> RemoveForwardRule(string did)
        {
            var client = new DisabledCallForwardingSoapClient(0);
            var response = await client.RemoveForwardRuleAsync(did);
            return Ok(response);
        }
        [HttpGet("getLogs")]
        public async Task<IActionResult> GetLogs(int option = 1)
        {
            var client = new DisabledCallForwardingSoapClient(0);
            var response = await client.GetLogsAsync(option);
            return Ok(response);
        }
    }
}
