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
using System.Security.Cryptography;
using System.Xml.Linq;
using System.Net;
using System.Text.Unicode;
using System;
using System.Reflection.Metadata;
using static System.Net.Mime.MediaTypeNames;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.Build.Framework;
using System.Security.Policy;
using Microsoft.AspNetCore.Http;
using static System.Net.WebRequestMethods;

namespace Didww.Controllers
{
    [ApiController]
    [Route("api/services")]
    public class ServicesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        readonly string apiUrl = "https://api.didww.com/v3/";
        //readonly string apiKey = "gtyksxjhwczv5dkk1ty1gar14pohd238";
        readonly string apiKey = "VCOG1CH1Y7CDK1LZF3O19ZFOT1A9F";
        private string? didid;
        private string? trunkinid;
        private string? predesttrunkNo;
        private string? voice_trunck_link_self;
        private readonly IPhoneNumbersService _phoneNumberService;
        private readonly IDIDWPortalService _didWPortalService;
        public ServicesController(IConfiguration configuration, IPhoneNumbersService phoneNumberService, IDIDWPortalService didWPortalService)
        {
            _configuration = configuration;
            _phoneNumberService = phoneNumberService;
            _didWPortalService = didWPortalService;
        }
        //[TokenValidationFilter]
        [HttpGet("countryinfo")]
        public async Task<IActionResult> getCountry(string code)
        {
            try
            {

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Api-Key", apiKey);

                    HttpResponseMessage response = await client.GetAsync(apiUrl + "countries?filter%5Bprefix=" + code);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();

                        CountryApiResponse? apiResponse = JsonConvert.DeserializeObject<CountryApiResponse>(responseData);

                        List<CountryAttributes> countryAttributes = apiResponse.Data.Select(d => d.Attributes).ToList();

                        return Ok(countryAttributes);
                    }
                    else
                    {
                        return Unauthorized(new { Error = "Invalid Country Code" });
                    }
                }

            }
            catch (HttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "Internal server error", ExceptionMessage = ex.Message });
            }
            catch (JsonException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "Internal server error", ExceptionMessage = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "Internal server error", ExceptionMessage = ex.Message });
            }
            //return Unauthorized(new { Error = "Invalid Country Code" });
        }

        //[TokenValidationFilter]
        [HttpGet("getdid")]

        public string ConvertUnauthorizedObjectResultToString(IActionResult actionResult)
        {
            // Check if the result is an UnauthorizedObjectResult
            if (actionResult is UnauthorizedObjectResult unauthorizedObjectResult)
            {
                // Extract content from the UnauthorizedObjectResult
                // Here, we assume the content is a string, but adjust this based on your specific scenario.
                string content = unauthorizedObjectResult.Value?.ToString();

                return content ?? string.Empty;
            }
            else
            {
                // Handle other types of IActionResult if needed.
                // You might return an empty string or throw an exception, depending on your requirements.
                return string.Empty;
            }
        }

        //[TokenValidationFilter]
        public static string GetCountryByDID(string DID)
        {
            // Dictionary of country codes and their respective countries
            Dictionary<string, string> countryCodes = new Dictionary<string, string>
        {

            { "100", "Canada" },
            { "1", "USA" },
            { "44", "UK" },
            { "33", "France" },
            { "27", "South Africa" },
            { "61", "Australia" },
            { "55", "Brazil" },
            { "65", "Singapore" },
            { "34", "Spain" },
            { "52", "Mexico" },
            { "54", "Argentina" },
            { "56", "Chile" }
        };

            // Check if DID starts with any of the country codes
            foreach (var code in countryCodes)
            {
                if (DID.StartsWith(code.Key))
                {
                    return code.Key;
                }
            }

            return string.Empty; // Return empty string if no match is found
        }
    
        [HttpGet("addforwardRule")]
        public async Task<IActionResult> AddForwardRule(string PhoneNumber, string DID, string CountryCode, int isPortal = 0)
        {
            string Params = "";
            var thirdParam = "DIDWWW";
            var fourthParam = "014";
            try
             {
                string ccode = GetCountryByDID(DID);

                if (ccode == CountryCode)
                    DID = DID;
                else if (CountryCode == "100")
                { 
                    if(ccode == "1") 
                        DID = DID;
                    else
                        DID = "1"+ DID;
                }                
                else
                    DID = CountryCode + DID;

                var PhoneWithCode = CountryCode + PhoneNumber;
                string PhonewithoutZero = "";

                if (!PhoneNumber.StartsWith("0"))
                {
                    PhonewithoutZero = PhoneNumber;
                    PhoneNumber = "0" + PhoneNumber;
                }
                else
                {
                    PhonewithoutZero = PhoneNumber.TrimStart('0');
                } 

                 Params = "Function: AddForwardRule, PhoneNumber: 972" + PhonewithoutZero + ", DID: "+ DID;
                

                voice_trunck_link_self = null;
                predesttrunkNo = "";
                bool isValidPhoneNumber = true;
                var message = "";

                // Validate input parameters    
                if (string.IsNullOrEmpty(PhoneNumber) || string.IsNullOrEmpty(DID) || string.IsNullOrEmpty(CountryCode))
                {
                    _didWPortalService.AddtblAPILog(isPortal, "PhoneNumber/DID/CountryCode is null", Convert.ToInt32(CountryCode), 1, DID, PhoneNumber, "ERROR", Params, null, null, null, null, "404", null);

                    if (isPortal == 1)
                        return Ok(0); //BadRequest("Invalid input parameters");
                    else
                        return Ok("\"ERROR\",\"PhoneNumber/DID/CountryCode is null\"");

                    //return Ok(new { statusCode = "ERROR", StatusDescription = "PhoneNumber/DID/CountryCode is null" });
                } 
                var result = await Getdid(DID);
                bool isValidDID = result != null;

                if (!isValidPhoneNumber || !isValidDID || string.IsNullOrEmpty(didid))
                { 
                    _didWPortalService.AddtblAPILog(isPortal, "DID is invalid", Convert.ToInt32(CountryCode), 1, DID, PhoneNumber, "ERROR", Params, result == null ? null : ConvertUnauthorizedObjectResultToString(result), null, null, null, "404", null);
                   
                    if (isPortal == 1)
                        return Ok(2); //BadRequest("Invalid phone number, DID, or country code");
                    else
                        return Ok("\"ERROR\",\"DID is invalid\"");
                }
                // Call the stored procedure with the provided parameters
                string trunkid = "";
                message = "NA";
                var configuration = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile("appsettings.json")
                                    .Build();

                var connectionString = configuration.GetConnectionString("DIDDbConnection");

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand("Remove_trunk_in_record", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@did_id", didid);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                trunkid = reader["result"].ToString();
                            }
                        }
                    }
                }
                //if (trunkid == "Not Found")
                {
                    HttpWebRequest request = CreateWEBSVCSWebRequest("http://websvcs-new.talknsave.net/IH_EXT_WS/BillingInfo.asmx?op=GetPhoneDetails");
                    XmlDocument soapEnvelopeXml = new XmlDocument();
                    soapEnvelopeXml.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?>
                    <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                      <soap:Body>
                        <GetPhoneDetails xmlns=""http://TNS-EXTERNAL.org/"">
                          <strPhoneNumber>" + PhoneNumber + @"</strPhoneNumber>
                        </GetPhoneDetails>
                      </soap:Body>
                    </soap:Envelope>");

                    using (Stream stream = request.GetRequestStream())
                    {
                        soapEnvelopeXml.Save(stream);
                    }

                    using (WebResponse response = request.GetResponse())
                    {
                        using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                        {
                            string soapResult = rd.ReadToEnd();

                            var rootElement = XElement.Parse(soapResult);

                            XmlDocument xdoc = new XmlDocument();                            
                            xdoc.LoadXml(soapResult);
                            var isHotMobile = false;
                            if (xdoc.GetElementsByTagName("bolFound")[0].InnerText == "true")
                            {
                                if (xdoc.GetElementsByTagName("intProviderCode")[0].InnerText == "77")
                                {
                                    isHotMobile = true;
                                }
                                else
                                {
                                    isHotMobile = false;
                                }                                
                            }
                            message = isHotMobile == true ? "Hot Mobile" : "014";
                            thirdParam = message == "Hot Mobile" ? "DIDWWW" : "BZKINT";
                            fourthParam = message ;
                            if (!isHotMobile)
                            {
                                HttpWebRequest requestKNT;
                                XmlDocument soapEnvelopeXmlKNT = new XmlDocument();

                                requestKNT = CreateWEBSVCSWebRequest("http://websvcs-new.talknsave.net/BezekIntlAPI/BezekIntlAPIWS.asmx?op=AddKNTForwarding");
                                soapEnvelopeXmlKNT = new XmlDocument();
                                soapEnvelopeXmlKNT.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Body>
    <AddKNTForwarding xmlns=""http://talknsave.net/"">
      <strDID>" + DID + @"</strDID>
      <strPhoneNumber>" + PhoneNumber + @"</strPhoneNumber>
      <strLoginID>57</strLoginID>
    </AddKNTForwarding>
  </soap:Body>
</soap:Envelope>");
                                using (Stream stream = requestKNT.GetRequestStream())
                                {
                                    soapEnvelopeXmlKNT.Save(stream);
                                }

                                using (WebResponse responseKNT = requestKNT.GetResponse())
                                {
                                    using (StreamReader rdKNT = new StreamReader(responseKNT.GetResponseStream()))
                                    {
                                        string soapResultKNT = rdKNT.ReadToEnd();

                                        var rootElementKNT = XElement.Parse(soapResultKNT);

                                        XmlDocument xdocKNT = new XmlDocument();
                                        xdocKNT.LoadXml(soapResultKNT);

                                        XmlNamespaceManager nsmgr = new XmlNamespaceManager(xdocKNT.NameTable);
                                        nsmgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
                                        nsmgr.AddNamespace("tns", "http://talknsave.net/");

                                        XmlNode resultNode = xdocKNT.SelectSingleNode("//soap:Body/tns:AddKNTForwardingResponse/tns:AddKNTForwardingResult", nsmgr);

                                        string res = resultNode.InnerText;
                                        if (resultNode == null || res != "0")  
                                        {
                                            _didWPortalService.AddtblAPILog(isPortal, "014 API returned -99", Convert.ToInt32(CountryCode), 1, DID, PhoneNumber, "ERROR", Params, null, null, null, message, "-99", null);
                                            if (isPortal == 1)
                                                return Ok(0);
                                            else
                                                return Ok("\"ERROR\",\"014 API returned -99\",\"" + thirdParam + "\",\"" + fourthParam + "\"");

                                            {
                                                //return Ok("\"ERROR\",\"014 API returned -99\"");
                                                //HttpContext.Response.Headers.Add("statusCode", "Error");
                                                //HttpContext.Response.Headers.Add("StatusDescription", "014 API returned -99");
                                                //return Ok(new { statusCode = "ERROR", StatusDescription = "014 API returned -99" });
                                                //HttpStatusCode.OK;//Ok(400);
                                                //return BadRequest(new { statusCode = "ERROR", message = "014 API returned -99" });
                                                //return StatusCode(  99, new { StatusCode = "Error", Message = "014 API returned -99." });
                                                //return BadRequest();//StatusCode(404, new { StatusCode = "Error", StatusDescription = "014 API returned -99" });
                                                // HTTP 200 OK
                                                //var apiResult = new
                                                //{
                                                //    statusCode = "Error",
                                                //    StatusDescription = "014 API returned -99"
                                                //};
                                                //return Ok(apiResult);
                                            };
                                        }
                                    }
                                }
                            }


                            trunkid = await _didWPortalService.CreateSIPTrunk(isHotMobile, DID, "972" + PhonewithoutZero);
                            
                        }
                    }
                    //if (response == 1 || response == 2 || response==4)
                    //{
                    //    PhoneNumber = CountryCode + PhoneNumber;
                    //     var isHotMobile = response == 1 ? true : false;
                    //     message = isHotMobile == true ? "Hot Mobile" : "014";
                    //    Params = "Function: AddForwardRule, PhoneNumber: " + PhoneNumber + ", DID: " + DID ;
                    //    trunkid = await _didWPortalService.CreateSIPTrunk(isHotMobile, DID, PhoneNumber);
                    //    if (string.IsNullOrEmpty(trunkid))
                    //    {  
                    //        return Ok(3);
                    //    }
                    //}
                    //else
                    //{
                    //    return Ok(4);
                    //}
                } 
                var providercode = message == "Hot Mobile" ? 2 : 1; 
                thirdParam = message == "Hot Mobile" ? "DIDWWW" : "BZKINT";
                fourthParam = message;


                if (!string.IsNullOrEmpty(trunkid))
                {
                    var response = await _didWPortalService.AssignTrunk(isPortal, Params, didid, trunkid, PhoneWithCode, predesttrunkNo,message, Convert.ToInt32(CountryCode), providercode, DID, PhoneNumber);

                    //Condition always false
                    if (providercode == 1 && response == "OK" && false)
                    {
                        HttpWebRequest request;
                        XmlDocument soapEnvelopeXml = new XmlDocument();
                        //if (CountryCode == "972")
                        //{
                            request = CreateWEBSVCSWebRequest("http://websvcs-new.talknsave.net/BezekIntlAPI/BezekIntlAPIWS.asmx?op=AddKNTForwarding");
                            soapEnvelopeXml = new XmlDocument();
                            soapEnvelopeXml.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Body>
    <AddKNTForwarding xmlns=""http://talknsave.net/"">
      <strDID>" + DID + @"</strDID>
      <strPhoneNumber>" + PhoneNumber + @"</strPhoneNumber>
      <strLoginID>57</strLoginID>
    </AddKNTForwarding>
  </soap:Body>
</soap:Envelope>");
                        //}
//                        else 
//                        {
//                            request = CreateWEBSVCSWebRequest("http://websvcs-new.talknsave.net/BezekIntlAPI/BezekIntlAPIWS.asmx?op=TollFreeServiceUpsert");
//                            soapEnvelopeXml = new XmlDocument();
//                            soapEnvelopeXml.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?>
//<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
//  <soap:Body>
//     <TollFreeServiceUpsert xmlns=""http://talknsave.net/"">
//      <strDestPhone>" + PhoneNumber + @"</strDestPhone>
//      <strGreenNumber>" + DID + @"</strGreenNumber>
//      <strLoginID>57</strLoginID>
//    </TollFreeServiceUpsert>
//  </soap:Body>
//</soap:Envelope>");
//                        }
 
                        using (Stream stream = request.GetRequestStream())
                        {
                            soapEnvelopeXml.Save(stream);
                        }

                        using (WebResponse response2 = request.GetResponse())
                        {
                            using (StreamReader rd = new StreamReader(response2.GetResponseStream()))
                            {
                                string soapResult = rd.ReadToEnd();

                                var rootElement = XElement.Parse(soapResult);

                                XmlDocument xdoc = new XmlDocument();
                                xdoc.LoadXml(soapResult);

                                XmlNamespaceManager nsmgr = new XmlNamespaceManager(xdoc.NameTable);
                                nsmgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
                                nsmgr.AddNamespace("tns", "http://talknsave.net/");

                                XmlNode resultNode = xdoc.SelectSingleNode("//soap:Body/tns:AddKNTForwardingResponse/tns:AddKNTForwardingResult", nsmgr);

                                if (resultNode != null)
                                {
                                    string res = resultNode.InnerText;
                                    if(res == "-99")
                                        _didWPortalService.AddtblAPILog(isPortal, "014 API returned -99", Convert.ToInt32(CountryCode), 1, DID, PhoneNumber, "014 API returned -99", Params, null, null, null, message, "-99", null);                                    
                                }
                                if (isPortal == 1)
                                    return Ok(true);
                                else
                                    return Ok("\"OK\",\"Success\",\""+ thirdParam + "\",\"" + fourthParam + "\"");
                            }
                        }
                    } 
                    
                    if (isPortal == 1)
                        return Ok(response == "OK" ? true : false);
                    else
                        return response == "OK"? Ok("\"OK\",\"Success\",\"" + thirdParam + "\",\"" + fourthParam + "\"") : Ok("\"ERROR\",\"404\",\"" + thirdParam + "\",\"" + fourthParam + "\"");
                }
                else
                {
                    _didWPortalService.AddtblAPILog(isPortal, Params, Convert.ToInt32(CountryCode), providercode, DID, PhoneNumber, "404", Params, null, null, null, null, "404");
                    //return Ok(4); // BadRequest("Failed to add forward rule"); 
                    if (isPortal == 1)
                        return Ok(4);
                    else
                        return Ok("\"ERROR\",\"404\",\"" + thirdParam + "\",\"" + fourthParam + "\"");
                }

            }
            catch (Exception ex)
            {
                // Handle exceptions here
                Console.WriteLine($"An exception occurred: {ex.Message}");
                var testdata = new
                {
                    id = 00,
                    Message = ":>" + ex.Message,
                    completed = true 
                };
                //return Ok(testdata);
                _didWPortalService.AddtblAPILog(isPortal, "An exception occurred:" + ex.Message , Convert.ToInt32(CountryCode), 1, DID, PhoneNumber, "ERROR", Params, null, null, null, null, "404", null);
                if (isPortal == 1)
                    return Ok(0); // StatusCode(500, "Internal server error"); // or handle the error as appropriate for your application
                else
                    return Ok("\"ERROR\",\"An exception occurred:" + ex.Message + "\"");                
            }       
        }
        [HttpGet("VerifyNumber")]
        public async Task<IActionResult> VerifyNumber(string phoneNumber)
        {
            var result = await _phoneNumberService.VerifyNumber(phoneNumber);
            return Ok(result);
        }

        //[TokenValidationFilter]
        [HttpGet("removeForwardRule")]
        public async Task<IActionResult> RemoveForwardRule(string did, string CountryCode = "", string PhoneNumber="", int isPortal = 0)
        {
            try
            {
                string Params = "Function: RemoveForwardRule, DID: " + did;


                string sPhoneNumber = "";
                string sCode = "";

                if ((string.IsNullOrEmpty(PhoneNumber) || string.IsNullOrEmpty(CountryCode)) && isPortal == 0)
                {
                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();

                    var connectionString = configuration.GetConnectionString("DIDDbConnection");

                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        using (var command = new SqlCommand("sp_GetPhoneDetails", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@did_id", did);

                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    sPhoneNumber = reader["PhoneNumber"].ToString();
                                    sCode = reader["CountryCode"].ToString();
                                }
                            }
                        }
                    }
                }
                else
                {
                    string ccode = GetCountryByDID(did);

                    if (ccode == CountryCode)
                        did = did;
                    else if (CountryCode == "100")
                    {
                        if (ccode == "1")
                            did = did;
                        else
                            did = "1" + did;
                    }
                    else
                        did = CountryCode + did;

                    if (!PhoneNumber.StartsWith("0"))
                    {
                        PhoneNumber = "0" + PhoneNumber;
                    }
                    

                    sPhoneNumber = PhoneNumber;
                    sCode = CountryCode;
                }


                // Validate DID
                var didresult = await Getdid(did);
                bool isValidDID = didresult != null;
                var jsonString = didresult.ToJson();
                var rootObject = JsonConvert.DeserializeObject<RootObject>(jsonString);

                if (rootObject?.Value?.DidInfoResponse?.Length > 0)
                {
                    var voiceInTrunkUrl = rootObject.Value.DidInfoResponse[0].VoiceInTrunk;
                    trunkinid = await _didWPortalService.getTrunkID(voiceInTrunkUrl);
                }
                else
                {
                    _didWPortalService.RemovetblAPILog(isPortal, Params, 1, did, jsonString, sCode, sPhoneNumber);
                    if (isPortal == 1)
                        return Ok(0);
                    else
                        return Ok("\"ERROR \" ,\"Invalid DID\"");
                }
                if (didresult == null)
                {
                    _didWPortalService.RemovetblAPILog(isPortal, Params, 1, did, jsonString, sCode, sPhoneNumber);

                    if (isPortal == 1)
                        return Ok(0);
                    else
                        return Ok("\"ERROR\",\"Invalid DID\"");
                }
                else
                {

                    var response = await _didWPortalService.RemoveForwardRule(didid, trunkinid, did);
                    //_didWPortalService.RemovetblAPILog(isPortal, Params, 1, did, response);

                    _didWPortalService.RemovetblAPILog(isPortal, Params, 1, did, response, sCode, sPhoneNumber);


                    if (true)
                    {
                        HttpWebRequest request;
                        XmlDocument soapEnvelopeXml = new XmlDocument();
                        //if (sCode == "972")
                        //{
                            request = CreateWEBSVCSWebRequest("http://websvcs-new.talknsave.net/BezekIntlAPI/BezekIntlAPIWS.asmx?op=RemoveKNTForwarding");
                            soapEnvelopeXml = new XmlDocument();
                            soapEnvelopeXml.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Body>
    <RemoveKNTForwarding xmlns=""http://talknsave.net/"">
      <strDID>" + did + @"</strDID>
      <strPhoneNumber>" + sPhoneNumber + @"</strPhoneNumber>
      <strLoginID>57</strLoginID>
    </RemoveKNTForwarding>
  </soap:Body>
</soap:Envelope>");

//                        return Ok(@"<?xml version=""1.0"" encoding=""utf-8""?>
//<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
//  <soap:Body>
//    <RemoveKNTForwarding xmlns=""http://talknsave.net/"">
//      <strDID>" + did + @"</strDID>
//      <strPhoneNumber>" + sPhoneNumber + @"</strPhoneNumber>
//      <strLoginID>57</strLoginID>
//    </RemoveKNTForwarding>
//  </soap:Body>
//</soap:Envelope>"); 
                        //}
                        //                        else
                        //                        {
                        //                            request = CreateWEBSVCSWebRequest("http://websvcs-new.talknsave.net/BezekIntlAPI/BezekIntlAPIWS.asmx?op=TollFreeServiceDelete");
                        //                            soapEnvelopeXml = new XmlDocument();
                        //                            soapEnvelopeXml.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?>
                        //<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                        //  <soap:Body>
                        //    <TollFreeServiceDelete xmlns=""http://talknsave.net/"">
                        //      <strDestPhone>" + sPhoneNumber + @"</strDestPhone>
                        //      <strGreenNumber>" + did + @"</strGreenNumber>
                        //      <strLoginID>57</strLoginID>
                        //    </TollFreeServiceDelete>
                        //  </soap:Body>
                        //</soap:Envelope>");

                        //                        }


                        using (Stream stream = request.GetRequestStream())
                        {
                            soapEnvelopeXml.Save(stream);
                        }

                        using (WebResponse response2 = request.GetResponse())
                        {
                            using (StreamReader rd = new StreamReader(response2.GetResponseStream()))
                            {
                                string soapResult = rd.ReadToEnd();

                                var rootElement = XElement.Parse(soapResult);

                                XmlDocument xdoc = new XmlDocument();
                                xdoc.LoadXml(soapResult);
                                var testdata = new
                                {
                                    //id = 58,
                                    //title = "RemoveResult:" + did + ":" + sPhoneNumber + ">" + xdoc.InnerXml.ToString(),
                                    completed = true
                                };

                                if (isPortal == 1)
                                    return Ok(true);
                                else
                                    return Ok("\"OK\",\"Success\"");
                            }
                        }
                    }

                    if (isPortal == 1)
                        return Ok(response == "OK" ? true : false);
                    else
                        return response == "OK"? Ok("\"OK\",\"Success\"") : Ok("\"ERROR\",\"404\"");

    }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"SQL error: {sqlEx.Message}");     
                if (isPortal == 1)
                    return Ok(0); // StatusCode(500, new { Error = $"SQL error: {sqlEx.Message}" });
                else
                    return Ok("\"ERROR\",\"SQL error:" + sqlEx.Message + "\"");
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP request error: {httpEx.Message}");

                if (isPortal == 1)
                    return Ok(0); //BadRequest(new { Error = $"HTTP request error: {httpEx.Message}" }); 
                else
                    return Ok("\"ERROR\",\"HTTP request error:" + httpEx.Message + "\"");
            }
            catch (TaskCanceledException canceledEx)
            {
                Console.WriteLine($"Request timed out: {canceledEx.Message}");
                if (isPortal == 1)
                    return Ok(0); // StatusCode(408, new { Error = $"Request timed out: {canceledEx.Message}" });
                else
                    return Ok("\"ERROR\",\"Request timed out:" + canceledEx.Message + "\"");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected exception occurred: {ex.Message}");
                if (isPortal == 1)
                    return Ok(0); // StatusCode(500, new { Error = $"An unexpected exception occurred: {ex.Message}" });
                else
                    return Ok("\"ERROR\",\"An unexpected exception occurred:" + ex.Message + "\"");
                //return Ok(new { statusCode = "ERROR", StatusDescription = "An unexpected excep  tion occurred:" + ex.Message });
            }
        }


        [HttpGet("getLogs")]
        public async Task<IActionResult> GetLogs(string type,int option = 1)
        {
            try
            {
                // StringBuilder to store HTML table markup
                 StringBuilder htmlTable = new StringBuilder();

                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
                var connectionString = configuration.GetConnectionString("DIDDbConnection");
  
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "";
                    string stype = "";
                    if (type == "ADD")
                        stype = " Command = 'ADD' ";
                    else if (type == "REMOVE")
                        stype = " Command = 'REMOVE' ";
                    else
                        stype = " (Command = 'ADD' OR Command = 'REMOVE') "; 


                    switch (option)
                    {
                        case 1:
                            query = "SELECT EnteredOn, Params,PreviousDestNo as PreviousNo,CurrentDestNo as CurrentNo,StatusCode as  ResultCode,UsedService as UsedAPI,IsGUI,PayLoad from tblAPILog where " + stype + " AND convert(date, EnteredOn) = CONVERT(DATE, GETDATE()) order by EnteredOn desc";
                            break;
                        case 2:
                            query = "SELECT EnteredOn, Params,PreviousDestNo as PreviousNo,CurrentDestNo as CurrentNo,StatusCode as  ResultCode,UsedService as UsedAPI,IsGUI,PayLoad from tblAPILog where  " + stype + " AND convert(date, EnteredOn) >= CONVERT(DATE, GETDATE()-7) order by EnteredOn desc";
                            break;
                        case 3:
                            query = "SELECT EnteredOn, Params,PreviousDestNo as PreviousNo,CurrentDestNo as CurrentNo,StatusCode as  ResultCode,UsedService as UsedAPI,IsGUI,PayLoad from tblAPILog where  " + stype + " AND convert(date, EnteredOn) >= CONVERT(DATE, GETDATE()-30) order by EnteredOn desc";
                            break;
                        default:
                            query = "SELECT EnteredOn, Params,PreviousDestNo as PreviousNo,CurrentDestNo as CurrentNo,StatusCode as  ResultCode,UsedService as UsedAPI,IsGUI,PayLoad from tblAPILog  where  " + stype + " order by EnteredOn desc";
                            break;
                    }

                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            // Check if there are rows in the result set
                            if (reader.HasRows)
                            {
                                // Start building the HTML table
                                htmlTable.Append("<table  id='example' class='display' border='1' width='100%'><thead><tr>");

                                // Add column headers
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    if(reader.GetName(i) =="PayLoad")
                                    {
                                        htmlTable.Append("<th style=\"display: none;\">" + reader.GetName(i) + "</th>");
                                    }
                                    else 
                                    {
                                        htmlTable.Append("<th>" + reader.GetName(i) + "</th>");
                                    }
                                }

                                htmlTable.Append("</tr></thead><tbody>");

                                // Iterate through the result set
                                while (reader.Read())
                                {
                                    htmlTable.Append("<tr>");

                                    // Add data for each column
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        if (i == (reader.FieldCount -1)) //PayLoad is last col
                                        {
                                            htmlTable.Append("<td  style=\"display: none;\">" + reader[i].ToString() + "</td>");
                                        }
                                        else
                                        {
                                             htmlTable.Append("<td>" + reader[i].ToString() + "</td>");
                                        }
                                    }

                                    htmlTable.Append("</tr>");
                                }

                                htmlTable.Append("</tbody></table>");
                            }
                            else
                            {
                                htmlTable.Append("No data found.");
                            }
                        }
                    }
                }
                return Ok(htmlTable.ToString());
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpGet("getWebReport")]
        public async Task<IActionResult> GetWebReport(string type, int option = 1)
        {
            try
            {
                // StringBuilder to store HTML table markup
                StringBuilder htmlTable = new StringBuilder();

                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
                var connectionString = configuration.GetConnectionString("DIDDbConnection");

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "";
                    string stype = "";
                    if (type == "ADD")
                        stype = " Command = 'ADD'  ";
                    else if (type == "REMOVE")
                        stype = " Command = 'REMOVE' ";
                    else
                        stype = " (Command = 'ADD' OR Command = 'REMOVE') ";

                    switch (option)
                    {
                        case 1:
                            query = "SELECT Counter, ProviderCode, Command, PhoneNumber, DID, CountryCode, ResponseCode, ResponseComment, EnteredOn, Params, UsedService as UsedAPI, IsGUI, UsedService as Provider from tblAPILog where " + stype + " AND convert(date, EnteredOn) = CONVERT(DATE, GETDATE()) order by EnteredOn desc";
                            break;
                        case 2:
                            query = "SELECT Counter, ProviderCode, Command, PhoneNumber, DID, CountryCode, ResponseCode, ResponseComment, EnteredOn, Params, UsedService as UsedAPI, IsGUI, UsedService as Provider from tblAPILog where " + stype + "  AND convert(date, EnteredOn) >= CONVERT(DATE, GETDATE()-7) order by EnteredOn desc";
                            break;
                        case 3:
                            query = "SELECT Counter, ProviderCode, Command, PhoneNumber, DID, CountryCode, ResponseCode, ResponseComment, EnteredOn, Params, UsedService as UsedAPI, IsGUI, UsedService as Provider from tblAPILog where " + stype + " AND convert(date, EnteredOn) >= CONVERT(DATE, GETDATE()-30) order by EnteredOn desc";
                            break;
                        default:
                            query = "SELECT Counter, ProviderCode, Command, PhoneNumber, DID, CountryCode, ResponseCode, ResponseComment, EnteredOn, Params, UsedService as UsedAPI, IsGUI, UsedService as Provider  from tblAPILog  where " + stype + " order by EnteredOn desc";
                            break;
                    }



                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            // Check if there are rows in the result set
                            if (reader.HasRows)
                            {
                                // Start building the HTML table
                                htmlTable.Append("<table id='example' border='1' width='100%' style='text-align:center'><tr>");

                                int providerRowNumber = 0;
                                // Add column headers
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    htmlTable.Append("<th>" + reader.GetName(i) + "</th>");
                                    if (reader.GetName(i) == "Provider")
                                        providerRowNumber = i;
                                }

                                htmlTable.Append("</tr>");

                                // Iterate through the result set
                                while (reader.Read())
                                {
                                    htmlTable.Append("<tr>");

                                    // Add data for each column
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        if (providerRowNumber == i)
                                        {
                                            if (reader[i].ToString() == "014")
                                                htmlTable.Append("<td>BZKINT</td>");
                                            else if (reader[i].ToString() == "Hot Mobile")
                                                htmlTable.Append("<td>DIDWWW</td>");
                                        }
                                        else
                                            htmlTable.Append("<td>" + reader[i].ToString() + "</td>");


                                    }

                                    htmlTable.Append("</tr>");
                                }

                                htmlTable.Append("</table>");
                            }
                            else
                            {
                                htmlTable.Append("No data found.");
                            }
                        }
                    }
                }
                return Ok(htmlTable.ToString());
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        //[TokenValidationFilter]
        [HttpGet("PhonenumberInfo")]
        async public Task<PhoneNumberParts> DividePhoneNumber(string phoneNumber)
        {
            try
            {
                string numericPhoneNumber = new string(phoneNumber.Where(char.IsDigit).ToArray());

                if (numericPhoneNumber.Length < 10 || numericPhoneNumber.Length > 12)
                {
                    return null;
                }

                string countrycode = numericPhoneNumber.Substring(0, numericPhoneNumber.Length - 11);
                string areacode = numericPhoneNumber.Substring(numericPhoneNumber.Length - 11, 3);
                string mobilenumber = numericPhoneNumber.Substring(numericPhoneNumber.Length - 7, 7);

                string countryName = await GetCountryName(countrycode);
                if (countryName == "Unknown" || string.IsNullOrEmpty(countryName))
                {
                    return null;
                }

                string areaCodeName = await GetAreaCodeName(areacode);

                PhoneNumberParts parts = new PhoneNumberParts
                {
                    CountryCode = "+" + countrycode,
                    AreaCode = areacode,
                    MobileNumber = mobilenumber,
                    CountryName = countryName,
                    areaCodeName = areaCodeName
                };

                return parts;
            }
            catch (Exception ex)
            {
                // Handle exceptions here
                Console.WriteLine($"An exception occurred: {ex.Message}");
                return null; // or handle the error as appropriate for your application
            }
        }

        async public Task<string> GetCountryName(string countryCode)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Api-Key", apiKey);

                    HttpResponseMessage response = await client.GetAsync(apiUrl + "countries?filter%5Bprefix=" + countryCode);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();

                        CountryApiResponse apiResponse = JsonConvert.DeserializeObject<CountryApiResponse>(responseData);
                        if (apiResponse.Data.Count > 0)
                        {
                            return apiResponse.Data[0].Attributes.Name;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return (response.StatusCode).ToString();
                    }
                    //return "Unknown";
                }
            }
            catch (HttpRequestException ex)
            {
                // Log the exception for troubleshooting
                Console.WriteLine($"HTTP request failed: {ex.Message}");
                return "Unknown";
            }
            catch (JsonException ex)
            {
                // Log the exception for troubleshooting
                Console.WriteLine($"JSON deserialization failed: {ex.Message}");
                return "Unknown";
            }
            catch (Exception ex)
            {
                // Log the exception for troubleshooting
                Console.WriteLine($"An exception occurred: {ex.Message}");
                return "Unknown"; // or handle the error as appropriate for your application
            }
        }

        async public Task<PhoneNumberInfo> GetDidGroup(string Link)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Api-Key", apiKey);

                    HttpResponseMessage response = await client.GetAsync(Link);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();

                        DidGroupLinkResponse apiResponse = JsonConvert.DeserializeObject<DidGroupLinkResponse>(responseData);
                        if (!string.IsNullOrEmpty(apiResponse.data.id))
                        {
                            return await GetGroupName(apiResponse.data.id);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                // Log the exception for troubleshooting
                Console.WriteLine($"HTTP request failed: {ex.Message}");
                return null;
            }
            catch (JsonException ex)
            {
                // Log the exception for troubleshooting
                Console.WriteLine($"JSON deserialization failed: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                // Log the exception for troubleshooting
                Console.WriteLine($"An exception occurred: {ex.Message}");
                return null; // or handle the error as appropriate for your application
            }
        }
        public static HttpWebRequest CreateWEBSVCSWebRequest(string url)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            //HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(@"http://websvcs-new.talknsave.net/IH_EXT_WS/BillingInfo.asmx?op=GetPhoneDetails");

            webRequest.Headers.Add(@"SOAP:Action");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }
        public async Task<IActionResult> Getdid(string number)
        {
            try
            {
                //PhoneNumberParts? parts = await DividePhoneNumber(number);
                if (!string.IsNullOrEmpty(number))
                { 
                    using (HttpClient client = new HttpClient())
                    {

                        client.DefaultRequestHeaders.Add("Api-Key", apiKey);

                        HttpResponseMessage response = await client.GetAsync(apiUrl + "dids?filter%5Bnumber=" + number);

                        if (response.IsSuccessStatusCode)
                        {
                            string responseData = await response.Content.ReadAsStringAsync();

                            DidApiResponse? apiResponse = JsonConvert.DeserializeObject<DidApiResponse>(responseData);
                            if (apiResponse?.data.Count != 0)
                            {
                                var didAttributes = apiResponse.data.First().attributes;

                                DateTime expiresAt = didAttributes.expires_at;
                                PhoneNumberInfo didGroupResult = await GetDidGroup(apiResponse.data.First().relationships.did_group.links.self);
                                didid = apiResponse.data.First().id;
                                var didInfoResponses = apiResponse.data.Select(d => new DidInfoResponse
                                {
                                    Active = d.attributes != null && d?.attributes.blocked != null ? !d.attributes.blocked : false,
                                    Number = d?.attributes?.number,
                                    Id = d.id,
                                    ExpiresAt = d?.attributes.expires_at,
                                    Status = d?.attributes != null && d.attributes.expires_at > DateTime.UtcNow ? "Working" : "Expired",
                                    phonenumberinfo = didGroupResult,
                                    VoiceInTrunk = d.relationships?.voice_in_trunk?.links?.self,
                                    Relationships = new List<string>
                                          {
                                              //d.relationships?.didGroup?.links?.self,
                                              d.relationships?.order?.links?.self,
                                              d.relationships?.capacity_pool?.links?.self,
                                              d.relationships?.shared_capacity_group?.links?.self,
                                              d.relationships?.address_verification?.links?.self,
                                              d.relationships?.voice_in_trunk?.links?.self,
                                              d.relationships?.voice_in_trunk_group?.links?.self
                                          }.Where(link => !string.IsNullOrEmpty(link)).ToList()
                                }).ToList();
                                //didInfoResponses.Id = apiResponse.data.Id;
                                getdidOutput result = new getdidOutput();
                                result.DidInfoResponse = didInfoResponses;
                                // result.PhoneNumberParts = parts;
                                voice_trunck_link_self = apiResponse.data.First().relationships.voice_in_trunk.links.self;
                                // var desttrunkin = await _didWPortalService.getTrunkID(apiResponse.data.First().relationships.voice_in_trunk.links.self);
                                predesttrunkNo = await _didWPortalService.getvoiceTrunkDest(didid);

                                return Ok(result);
                            }
                            else
                            {
                                return Unauthorized(new { Error = "Please Provide valid Number" });
                            }
                        }
                        else
                        {
                            return StatusCode((int)response.StatusCode, new { Error = "API request failed" });
                        }
                    }
                }
                else
                {
                    return Unauthorized(new { Error = "Country Code Unknown. Please Provide valid Number" });
                }

            }

            catch (HttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "Internal server error", ExceptionMessage = ex.Message });
            }
            catch (JsonException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "Internal server error", ExceptionMessage = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "Internal server error", ExceptionMessage = ex.Message });
            }

        }

        public async Task<PhoneNumberInfo> GetGroupName(string id)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Api-Key", apiKey);

                    HttpResponseMessage response = await client.GetAsync(apiUrl + "did_groups/" + id);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();
                        DidGroupResponse groupResponse = JsonConvert.DeserializeObject<DidGroupResponse>(responseData);

                        PhoneNumberInfo phoneNumberInfo = new PhoneNumberInfo
                        {
                            area_name = groupResponse.data.attributes.area_name,
                            prefix = groupResponse.data.attributes.prefix,
                            features = groupResponse.data.attributes.features,
                            is_metered = groupResponse.data.attributes.is_metered,
                            allow_additional_channels = groupResponse.data.attributes.allow_additional_channels,
                            available_dids_enabled = groupResponse.meta.available_dids_enabled,
                            needs_registration = groupResponse.meta.needs_registration,
                            is_available = groupResponse.meta.is_available,
                            total_count = groupResponse.meta.total_count
                        };

                        return phoneNumberInfo;
                    }
                    else
                    {
                        // Handle API request failure
                        return null;
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                // Log the exception for troubleshooting
                Console.WriteLine($"HTTP request failed: {ex.Message}");
                return null;
            }
            catch (JsonException ex)
            {
                // Log the exception for troubleshooting
                Console.WriteLine($"JSON deserialization failed: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                // Log the exception for troubleshooting
                Console.WriteLine($"An exception occurred: {ex.Message}");
                return null; // or handle the error as appropriate for your application
            }
        }
        async public Task<string> GetAreaCodeName(string areaCode)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Api-Key", apiKey);

                    HttpResponseMessage response = await client.GetAsync(apiUrl + "nanpa_prefixes?filter%5Bnxx=" + areaCode);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();

                        CountryApiResponse? apiResponse = JsonConvert.DeserializeObject<CountryApiResponse>(responseData);
                        if (apiResponse?.Data?.Count > 0)
                        {
                            return apiResponse.Data[0].Attributes.Name;
                        }
                    }

                    return "Unknown";
                }
            }
            catch (HttpRequestException ex)
            {
                // Log the exception for troubleshooting
                Console.WriteLine($"HTTP request failed: {ex.Message}");
                return "Unknown";
            }
            catch (JsonException ex)
            {
                // Log the exception for troubleshooting
                Console.WriteLine($"JSON deserialization failed: {ex.Message}");
                return "Unknown";
            }
            catch (Exception ex)
            {
                // Log the exception for troubleshooting
                Console.WriteLine($"An exception occurred: {ex.Message}");
                return "Unknown"; // or handle the error as appropriate for your application
            }
        }
        //async public Task<string> GetTrunk(string PhoneNumber)
        //{
        //    string voiceTrunk = $"{apiUrl}voice_in_trunks";
        //    string name =  Guid.NewGuid().ToString();
        //    string forwardRulePayload = $@"{{
        //                    ""data"": {{
        //                        ""type"": ""voice_in_trunks"",
        //                        ""attributes"": {{
        //                            ""name"": ""{name}"",
        //                            ""capacity_limit"": 5,
        //                            ""configuration"": {{
        //                                ""type"": ""pstn_configurations"",
        //                                ""attributes"": {{
        //                                    ""dst"": ""{PhoneNumber}""
        //                                }}
        //                            }}
        //                        }}
        //                    }}
        //                }}";

        //    using (HttpClient client = new HttpClient())
        //    {
        //        client.DefaultRequestHeaders.Add("Api-Key", apiKey);

        //        // Corrected: Create a StringContent instance for the JSON payload
        //        StringContent content = new StringContent(forwardRulePayload, Encoding.UTF8, "application/vnd.api+json");

        //        HttpResponseMessage response = await client.PostAsync(voiceTrunk, content);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            string responseData = await response.Content.ReadAsStringAsync();
        //            TrunkDTO apiResponse = JsonConvert.DeserializeObject<TrunkDTO>(responseData);
        //            var configuration = new ConfigurationBuilder()
        //             .SetBasePath(Directory.GetCurrentDirectory())
        //             .AddJsonFile("appsettings.json")
        //             .Build();

        //            var connectionString = configuration.GetConnectionString("DIDDbConnection");
        //            using (var connection = new SqlConnection(connectionString))
        //            {
        //                // Open the database connection
        //                connection.Open();

        //                using (var command = new SqlCommand("insert_trunk_in_record", connection))
        //                {
        //                    command.CommandType = CommandType.StoredProcedure;
        //                    command.Parameters.AddWithValue("@did_id", didid);
        //                    command.Parameters.AddWithValue("@voice_in_trunk", apiResponse.data.id);
        //                    command.ExecuteNonQuery();
        //                }
        //                return apiResponse.data.id;
        //            }
        //        }
        //        else
        //        {
        //            // Handle the error response
        //            return "";
        //        }
        //    }

        //}
    }
}
