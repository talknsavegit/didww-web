using DIDWW_Api.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using static Didww.Models.DidModel;
using static System.Net.Mime.MediaTypeNames;

namespace Didww.Services
{
    public class DIDWPortalService : IDIDWPortalService
    {
        private readonly IConfiguration _configuration;
        readonly string apiUrl = "https://api.didww.com/v3/";
        readonly string apiKey = "VCOG1CH1Y7CDK1LZF3O19ZFOT1A9F";
        public DIDWPortalService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<string> CreateSIPTrunk(bool isHotMobile, string didNo,string destNo)
        {
         
            var client = new HttpClient();
            string name = Guid.NewGuid().ToString();
            var request = new HttpRequestMessage(HttpMethod.Post, apiUrl+"voice_in_trunks");
            request.Headers.Add("Api-Key", apiKey);
            var username = isHotMobile == false ? "{DID}" : destNo;
            var host = isHotMobile == false ? "212.179.236.116" : "77.137.137.154";
            var prefix = isHotMobile == false ? "" : "+";
            string forwardRulePayload0 = $@"{{
            ""data"": {{
             ""type"": ""voice_in_trunks"",
             ""attributes"": {{
            ""priority"": ""1"",
            ""weight"": ""2"",
            
            ""ringing_timeout"": 30,
            ""name"": ""{name}"",
            ""cli_format"": ""e164"",
            ""cli_prefix"": ""{prefix}"",
            ""description"": ""custom description"",
            ""configuration"": {{
                ""type"": ""sip_configurations"",
                ""attributes"": {{
                    ""username"": ""{username}"",
                    ""host"": ""{host}"",
                    ""codec_ids"": [9,6,7],
                    ""rx_dtmf_format_id"": 1,
                    ""tx_dtmf_format_id"": 1,
                    ""resolve_ruri"": ""false"",
                    ""sst_enabled"": ""false"",
                    ""sst_min_timer"": 600,
                    ""sst_max_timer"": 900,
                    ""sst_refresh_method_id"": 1,
                    ""sst_accept_501"": ""true"",
                    ""sip_timer_b"": 8000,
                    ""dns_srv_failover_timer"": 2000,
                    ""rtp_ping"": ""false"",
                    ""rtp_timeout"": 30,
                    ""force_symmetric_rtp"": ""false"",
                    ""symmetric_rtp_ignore_rtcp"": ""false"",
                    ""rerouting_disconnect_code_ids"": [58,59],
                  
                    ""transport_protocol_id"": 1,
                    ""max_transfers"": 0,
                    ""max_30x_redirects"": 0,
                    ""media_encryption_mode"": ""disabled"",
                    ""stir_shaken_mode"": ""disabled"",
                    ""allowed_rtp_ips"": null
                }}
                        }}
                    }}
                }}
                }}";
            var content = new StringContent($@"{{
            ""data"": {{
             ""type"": ""voice_in_trunks"",
             ""attributes"": {{
            ""priority"": ""1"",
            ""weight"": ""2"",
            
            ""ringing_timeout"": 30,
            ""name"": ""{name}"",
            ""cli_format"": ""e164"",
            ""cli_prefix"": ""{prefix}"",
            ""description"": ""custom description"",
            ""configuration"": {{
                ""type"": ""sip_configurations"",
                ""attributes"": {{
                    ""username"": ""{username}"",
                    ""host"": ""{host}"",
                    ""codec_ids"": [9,6,7],
                    ""rx_dtmf_format_id"": 1,
                    ""tx_dtmf_format_id"": 1,
                    ""resolve_ruri"": ""false"",
                    ""sst_enabled"": ""false"",
                    ""sst_min_timer"": 600,
                    ""sst_max_timer"": 900,
                    ""sst_refresh_method_id"": 1,
                    ""sst_accept_501"": ""true"",
                    ""sip_timer_b"": 8000,
                    ""dns_srv_failover_timer"": 2000,
                    ""rtp_ping"": ""false"",
                    ""rtp_timeout"": 30,
                    ""force_symmetric_rtp"": ""false"",
                    ""symmetric_rtp_ignore_rtcp"": ""false"",
                    ""rerouting_disconnect_code_ids"": [58,59],
                  
                    ""transport_protocol_id"": 1,
                    ""max_transfers"": 0,
                    ""max_30x_redirects"": 0,
                    ""media_encryption_mode"": ""disabled"",
                    ""stir_shaken_mode"": ""disabled"",
                    ""allowed_rtp_ips"": null
                }}
                        }}
                    }}
                }}
                }}", Encoding.UTF8, "application/vnd.api+json");

            request.Content = content;
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                TrunkDTO apiResponse = JsonConvert.DeserializeObject<TrunkDTO>(responseData);
                var configuration = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json")
                 .Build();

                var connectionString = configuration.GetConnectionString("DIDDbConnection");
                using (var connection = new SqlConnection(connectionString))
                {
                    // Open the database connection
                    connection.Open();

                    using (var command = new SqlCommand("insert_trunk_in_record", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@did_id", didNo);
                        command.Parameters.AddWithValue("@voice_in_trunk", apiResponse.data?.id);
                        command.ExecuteNonQuery();
                    }
                    return apiResponse.data.id+"=="+ forwardRulePayload0;
                }
            }
            return "";


        }
        public async Task<string> AssignTrunk(int isPortal, string Params,string didid, string trunkid, string? PhoneNumber = null, string? predesttrunkNo= null, string? message = null,
            int? intCountryCode = null, int? intProviderCode = null, string? strDID = null, string? strPhoneNumber = null)
        {

            string payload2 = " ";
            if (trunkid.Split("==").Count() > 1)
            {
                payload2 = trunkid.Split("==")[1];
                trunkid = trunkid.Split("==")[0];
            }
            else
            {
                payload2 = " trunk id="+ trunkid;
            }

            using (HttpClient client = new HttpClient())
            {
                string forwardRuleUrl = $"{apiUrl}dids/{didid}";
                string forwardRulePayload = $@"{{
                            ""data"": {{
                                ""id"": ""{didid}"",
                                ""type"": ""dids"",
                                ""relationships"": {{
                                    ""voice_in_trunk"": {{
                                        ""data"": {{
                                            ""type"": ""voice_in_trunks"",
                                            ""id"": ""{trunkid}""
                                        }}
                                    }}
                                }}
                            }}
                        }}";
  
                var forwardRuleRequest = new HttpRequestMessage(new HttpMethod("PATCH"), forwardRuleUrl)
                {
                    Content = new StringContent(forwardRulePayload, Encoding.UTF8, "application/vnd.api+json")
                };
                  
                // Add headers
                forwardRuleRequest.Headers.Add("Accept", "application/vnd.api+json");
                forwardRuleRequest.Headers.Add("Api-Key", apiKey);

                // Send the forward rule removal request
                var forwardRuleResponse = await client.SendAsync(forwardRuleRequest);

                // Check the response
                if (forwardRuleResponse.IsSuccessStatusCode) 
                {
                    //var desttrunkinid = await getTrunkID(voice_trunck_link_self);

                    var post_dstno = strPhoneNumber;//await getvoiceTrunkDest(didid);
                    //post_dstno= post_dstno== "{DID}"?PhoneNumber: post_dstno;

                    predesttrunkNo = String.IsNullOrEmpty(predesttrunkNo) ? "{DID}" : intProviderCode ==2 ? await getvoiceTrunkDest(didid): "{DID}";
                    var result = await forwardRuleResponse.Content.ReadAsStringAsync();
                    //var description = JObject.Parse(result)["data"]["attributes"]["description"].ToString();
                    AddtblAPILog(isPortal, forwardRuleResponse.StatusCode.ToString(), Convert.ToInt32(intCountryCode), intProviderCode, strDID, strPhoneNumber, forwardRuleResponse.StatusCode.ToString(), Params, result, predesttrunkNo, post_dstno, message, "200", "Create Trunk = " + payload2 + " Assign Trunk = " + forwardRulePayload);
                    return forwardRuleResponse.StatusCode.ToString();
                }
                else
                {
                    var result = await forwardRuleResponse.Content.ReadAsStringAsync();
                    var detail = JObject.Parse(result)["errors"][0]["detail"].ToString();
                    var code = JObject.Parse(result)["errors"][0]["code"].ToString();
                    AddtblAPILog(isPortal, detail, Convert.ToInt32(intCountryCode), intProviderCode, strDID, strPhoneNumber, forwardRuleResponse.StatusCode.ToString(), Params, result, predesttrunkNo, "NA", message, code, "Create Trunk = " + payload2 + " Assign Trunk = " + forwardRulePayload);
                    return forwardRuleResponse.StatusCode.ToString(); // BadRequest(new { Error = $"Failed to Add forward rule. Status code: {forwardRuleResponse.StatusCode}" });
                }
            }
        


        }
        public async Task<string> getTrunkID(string didInfoidURL)
        {
            try
            {
                string voiceInTrunkUrl = didInfoidURL;

                // Make the API call to get voice_in_trunk information
                using (HttpClient client = new HttpClient())
                {
                    // Add headers, such as Api-Key
                    client.DefaultRequestHeaders.Add("Api-Key", apiKey);

                    // Send the request
                    var voiceInTrunkResponse = await client.GetStringAsync(voiceInTrunkUrl);
                    var Response = JsonConvert.DeserializeObject<VoiceInTrunkResponse>(voiceInTrunkResponse);
                    string voiceInTrunkId = Response?.Data?.Id;
                    return voiceInTrunkId;
                }
            }
            catch (HttpRequestException ex)
            {
                // Handle exceptions related to the HTTP request (e.g., network issues, server not reachable)
                Console.WriteLine($"HTTP request error: {ex.Message}");
                // Return an error message as a string
                return $"Error: {ex.Message}";
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"An error occurred: {ex.Message}");
                // Return an error message as a string
                return $"Error: {ex.Message}";
            }
        }
        public async Task<string> getvoiceTrunkDest(string id)
        {
            try
            {
                string voiceInTrunkUrl = "https://api.didww.com/v3/dids/" + id+ "?include=voice_in_trunk";

                // Make the API call to get voice_in_trunk information
                using (HttpClient client = new HttpClient())
                {
                    // Add headers, such as Api-Key
                    client.DefaultRequestHeaders.Add("Api-Key", apiKey);

                    // Send the request
                    var voiceInTrunkResponse = await client.GetStringAsync(voiceInTrunkUrl);
                    var Response = JsonConvert.DeserializeObject<DestNoResponse>(voiceInTrunkResponse);
                    // If you need to return a string, you can return the response as is or modify it
                    var destNo = Response?.included.FirstOrDefault()?.attributes.configuration?.attributes?.username;
                    return string.IsNullOrEmpty(destNo)? Response?.included.FirstOrDefault()?.attributes.configuration?.attributes?.dst:destNo;
                }
            }
            catch (HttpRequestException ex)
            {
                // Handle exceptions related to the HTTP request (e.g., network issues, server not reachable)
                Console.WriteLine($"HTTP request error: {ex.Message}");
                // Return an error message as a string
                return null;
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"An error occurred: {ex.Message}");
                // Return an error message as a string
                return null;
            }
        }
        public void AddtblAPILog(int isPortal, string? strResponseComment = null, int? intCountryCode = null, int? intProviderCode = 1, string? strDID = null, string? strPhoneNumber = "", string? strResponseCode = null,
             string? Params = null, string? Result = "", string? predest = null, string? postdest = null, string? UsedService = null, string? StatusCode = null, string? PayLoad = null)
        {
            var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

            var connectionString = configuration.GetConnectionString("DIDDbConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string procName = "";
                //if(isPortal==1)
                //{
                procName = "sp_DBFWD_AddCreateForwardingLog";
                //}
                //else
                //{
                //    procName = "sp_DBFWD_AddCreateAuditLog";
                //}


                using (var command = new SqlCommand(procName, connection))
                {
                    if (intCountryCode == 972)
                        strPhoneNumber = "0" + strPhoneNumber;

                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@intProviderCode", intProviderCode);
                    command.Parameters.AddWithValue("@strPhoneNumber", strPhoneNumber);
                    command.Parameters.AddWithValue("@strDID", strDID);
                    command.Parameters.AddWithValue("@intCountryCode", intCountryCode);
                    command.Parameters.AddWithValue("@strResponseCode", strResponseCode);
                    command.Parameters.AddWithValue("@strResponseComment", strResponseComment);

                    command.Parameters.AddWithValue("@Params", Params);
                    if(Result?.Length>2499)
                        command.Parameters.AddWithValue("@Result", Result.Substring(0, 2495));
                    else
                        command.Parameters.AddWithValue("@Result", Result);
                    command.Parameters.AddWithValue("@Option", 1);
                    command.Parameters.AddWithValue("@PreviousDestNo", predest);
                    command.Parameters.AddWithValue("@CurrentDestNo", postdest);
                    command.Parameters.AddWithValue("@UsedService", UsedService);

                    command.Parameters.AddWithValue("@StatusCode", StatusCode);
                    if (isPortal == 1)
                        command.Parameters.AddWithValue("@IsGUI", true);
                    else
                        command.Parameters.AddWithValue("@IsGUI", false);
                    command.Parameters.AddWithValue("@PayLoad", PayLoad);

                    var intNewCounterParam = new SqlParameter("@intNewCounter", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(intNewCounterParam);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        { 
                            //trunkid = reader["result"].ToString();
                        }
                    }
                }
            }
        }

        public void RemovetblAPILog(int isPortal, string strResponseComment,int intProviderCode, string strDID, string strResponseCode, string sCode, string sPhoneNumber)
        {
            var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

            var connectionString = configuration.GetConnectionString("DIDDbConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string procName = "";
                //if (isPortal == 1)
                //{ 
                    procName = "sp_DBFWD_AddRemoveForwardingLog";
                //}
                //else
                //{
                //   procName  = "sp_DBFWD_AddRemoveAuditLog";
                //}
                using (var command = new SqlCommand(procName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@intProviderCode", intProviderCode);
                    command.Parameters.AddWithValue("@strDID", strDID);
                    command.Parameters.AddWithValue("@strResponseCode", "200");
                    command.Parameters.AddWithValue("@strResponseComment", strResponseCode);
                    command.Parameters.AddWithValue("@Params", strResponseComment);
                    if (isPortal == 1)
                        command.Parameters.AddWithValue("@IsGUI", true);
                    else
                        command.Parameters.AddWithValue("@IsGUI", false);
                    command.Parameters.AddWithValue("@sCode", sCode);
                    command.Parameters.AddWithValue("@sPhoneNumber", sPhoneNumber);
                    var intNewCounterParam = new SqlParameter("@intNewCounter", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(intNewCounterParam);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            //trunkid = reader["result"].ToString();
                        }
                    }
                }
            }
        }
        private void SaveDestLogs(string Params, string? Result = null, string? predest = null, string? postdest = null, string? UsedService = null, string? StatusCode = null)
        {
            var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

            var connectionString = configuration.GetConnectionString("DIDDbConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("add_dest_logs", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Params", Params);
                    command.Parameters.AddWithValue("@Result", Result);
                    command.Parameters.AddWithValue("@Option", 1);
                    command.Parameters.AddWithValue("@PreviousDestNo", predest);
                    command.Parameters.AddWithValue("@CurrentDestNo", postdest);
                    command.Parameters.AddWithValue("@UsedService", UsedService);
                    command.Parameters.AddWithValue("@StatusCode", StatusCode);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            //trunkid = reader["result"].ToString();
                        }
                    }
                }
            }
        }
        public async Task<string> RemoveForwardRule(string didid,string trunkinid, string did)
        {
            string Params = "Function: RemoveForwardRule, DID: " + did;
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json")
               .Build();

            var connectionString = configuration.GetConnectionString("DIDDbConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                // Open the database connection
                connection.Open();

                using (var command = new SqlCommand("insert_trunk_in_record", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@did_id", didid);
                    command.Parameters.AddWithValue("@voice_in_trunk", trunkinid); // Replace with actual value
                    command.ExecuteNonQuery();
                }
            }

            using (HttpClient client = new HttpClient())
            {
                // Construct the URL for forward rule removal
                string forwardRuleUrl = $"{apiUrl}dids/{didid}";

                // Construct the JSON payload for forward rule removal
                // Assuming didid is a variable holding the dynamic id value
                string forwardRulePayload = $@"{{
                            ""data"": {{
                                ""id"": ""{didid}"",
                                ""type"": ""dids"",
                                ""relationships"": {{
                                    ""voice_in_trunk"": {{
                                        ""data"": {{
                                            ""type"": ""voice_in_trunks"",
                                            ""id"": null
                                        }}
                                    }}
                                }}
                            }}
                        }}";


                // Create the HttpRequestMessage
                var forwardRuleRequest = new HttpRequestMessage(new HttpMethod("PATCH"), forwardRuleUrl)
                {
                    Content = new StringContent(forwardRulePayload, Encoding.UTF8, "application/vnd.api+json")
                };

                // Add headers
                forwardRuleRequest.Headers.Add("Accept", "application/vnd.api+json");
                forwardRuleRequest.Headers.Add("Api-Key", apiKey);

                // Send the forward rule removal request
                var forwardRuleResponse = await client.SendAsync(forwardRuleRequest);

                // Check the response
                if (forwardRuleResponse.IsSuccessStatusCode)
                {
                   
                    SaveLogs(Params, await forwardRuleResponse.Content.ReadAsStringAsync(), forwardRuleResponse.StatusCode.ToString());
                    return forwardRuleResponse.StatusCode.ToString();  
                }
                else
                {
                  
                    SaveLogs(Params, await forwardRuleResponse.Content.ReadAsStringAsync(), forwardRuleResponse.StatusCode.ToString());
                    return forwardRuleResponse.StatusCode.ToString(); 
                }
            }
        }
        public void SaveLogs(string Params, string? Result = null, string? StatusCode = null)
        {
            var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

            var connectionString = configuration.GetConnectionString("DIDDbConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("add_display_logs", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Params", Params);
                    command.Parameters.AddWithValue("@Result", Result);
                    command.Parameters.AddWithValue("@Option", 1);
                    command.Parameters.AddWithValue("@StatusCode", StatusCode);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            //trunkid = reader["result"].ToString();
                        }
                    }
                }
            }
        }


    }

}
