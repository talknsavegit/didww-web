using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Services;

namespace AsmxWebService
{
    /// <summary>
    /// Summary description for DisabledCallForwarding
    /// </summary>
    [WebService(Namespace = "https://didww-api.talknsave.net")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class CallForwarding : System.Web.Services.WebService
    {
        private HttpClient _client = new HttpClient();

        public CallForwarding()
        {
            // Configure the HttpClient
            _client.BaseAddress = new Uri("https://didww-api.talknsave.net"); // Replace with actual API address if different
        }

        [WebMethod]
        public async Task<string> AddForwardRule(string phoneNumber, string did, string countryCode)
        {
            var response = await _client.GetAsync($"/api/services/addforwardRule?PhoneNumber={phoneNumber}&DID={did}&CountryCode={countryCode}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
            else
            {
                throw new Exception("API call failed: " + response.ReasonPhrase);
            }
        }
        [WebMethod]
        public async Task<string> RemoveForwardRule(string did)
        {
            var response = await _client.GetAsync($"/api/services/removeForwardRule?did={did}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
            else
            {
                throw new Exception("API call failed: " + response.ReasonPhrase);
            }
        } 
        [WebMethod]
        public async Task<string> GetLogs(int option)
        {
            var response = await _client.GetAsync($"/api/services/getLogs?option={option}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
            else
            {
                throw new Exception("API call failed: " + response.ReasonPhrase);
            }
        }
    }
}
