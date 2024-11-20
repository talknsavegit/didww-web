using Microsoft.Data.SqlClient;
using System.Data;

namespace Didww.Services
{
    public interface IDIDWPortalService
    {
        Task<string> CreateSIPTrunk(bool isHotMobile, string didNo, string destNo);
        Task<string> AssignTrunk(int isPortal, string Params,string didid, string trunkid, string? PhoneNumber = null, string? predesttrunkNo = null, string? message = null,
            int? intCountryCode = null, int? intProviderCode = null, string? strDID = null, string? strPhoneNumber = null);
        Task<string> getTrunkID(string didInfoidURL);
        Task<string> getvoiceTrunkDest(string id);
        Task<string> RemoveForwardRule(string didid, string trunkinid, string did);
        void SaveLogs(string Params, string? Result = null, string? StatusCode = null);
        void AddtblAPILog(int isPortal, string? strResponseComment = null, int? intCountryCode = null, int? intProviderCode = null, string? strDID = null, string? strPhoneNumber = null, string? strResponseCode = null,
            string? Params = null, string? Result = null, string? predest = null, string? postdest = null, string? UsedService = null, string? StatusCode = null, string? PayLoad = null);
        void RemovetblAPILog(int isPortal, string strResponseComment, int intProviderCode, string strDID, string strResponseCode, string sCode, string sPhoneNumber);
    }

}
