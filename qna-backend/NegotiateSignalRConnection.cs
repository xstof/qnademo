using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using System.Net.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using System.Net.Http.Headers;
using System.Linq;
using Microsoft.Azure.SignalR.Management;

namespace Qna.Backend
{
    public static class NegotiateSignalRConnection
    {
        private const string SessionToken = "qna-session-id";

        [FunctionName("negotiate")]
        public static SignalRConnectionInfo GetSignalRConnectionInfo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "sessions/{sessionId}/signalr/{userId}/negotiate")] HttpRequest req,
            string sessionId,
            string userId,
            [SignalRConnectionInfo(HubName = "%Region%", UserId = "{userId}")] SignalRConnectionInfo connectionInfo,
            ILogger log)
        {
            log.LogInformation("NegotiateSignalRConnection was called.");

            return connectionInfo;
        }

        [FunctionName("SubscribeForUpdates")]
         public static async Task<IActionResult> SubscribeForSessionUpdatesOnSignalR(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "sessions/{sessionId}/signalr/{userId}/subscribe")] HttpRequestMessage reqMsg,
            string sessionId,
            string userId,
            [SignalR(HubName = "%Region%")] IAsyncCollector<SignalRGroupAction> signalRGroupActions,
            ILogger log)
        {
            log.LogInformation("SubscribeForSessionUpdatesOnSignalR was called.");

            // TODO: check if session with given id exists or not

            // determine browser session id from cookie:
            // TODO: this fails, probably because of cross domain restrictions on cookies:
            var browserSessionIdCookie = reqMsg.Headers.GetCookies(SessionToken).FirstOrDefault();
            var browserSessionId = System.Guid.NewGuid().ToString();
            if(browserSessionIdCookie != null && browserSessionIdCookie.Cookies[0] != null){
                browserSessionId = browserSessionIdCookie.Cookies[0].Value;
                log.LogInformation($"got cookie with session id: {browserSessionId}");
            }

            // remove users from all signalr groups:
            try{
                log.LogInformation($"About to remove user with browser session {browserSessionId} from all SignalR groups.");
                var signalRMgmtBuilder = new ServiceManagerBuilder();
                var signalRService = signalRMgmtBuilder.WithOptions(config => {config.ConnectionString = System.Environment.GetEnvironmentVariable("AzureSignalRConnectionString", EnvironmentVariableTarget.Process);}).Build();
                var region = System.Environment.GetEnvironmentVariable("Region", EnvironmentVariableTarget.Process);
                var signalRHub = await signalRService.CreateHubContextAsync(region);
                await signalRHub.UserGroups.RemoveFromAllGroupsAsync(browserSessionId);
                log.LogInformation($"Removed user with browser session {browserSessionId} from all SignalR groups.");
            }
            catch(Exception e){
                log.LogError($"Failed to remove user/browsersession ({browserSessionId}) from all signalr groups: {e.Message}");
            }

            // add user to group for specified session:
            var groupname = $"listeners-for-session-{sessionId}";
            log.LogInformation($"About to add user to group {groupname} for session {sessionId}.");
            await signalRGroupActions.AddAsync(new SignalRGroupAction{
                UserId = userId,
                GroupName = groupname,
                Action = GroupAction.Add
            });
            log.LogInformation($"Added user with id {userId} and browser session {browserSessionId} to SignalR group for qna session {sessionId} with group name: {groupname}");
            
            var signalRNegotiateBaseUrl = System.Environment.GetEnvironmentVariable("SignalRNegotiateBaseUrl", EnvironmentVariableTarget.Process).Replace("{sessionId}", sessionId);
            return new OkWithSessionCookieObjectResult( new { SignalRNegotiateBaseUrl =  signalRNegotiateBaseUrl}, browserSessionId);
        }
    }

    public class OkWithSessionCookieObjectResult : OkObjectResult {
       private string _sessionId = System.Guid.NewGuid().ToString();

        public OkWithSessionCookieObjectResult(Object value) : base(value) {}

        public OkWithSessionCookieObjectResult(Object value, string sessionId) : base(value){
            this._sessionId = sessionId;
        }
 
       public async override Task ExecuteResultAsync(ActionContext context){
           await base.ExecuteResultAsync(context);
           context.HttpContext.Response.Cookies.Append("qna-session-id", this._sessionId);
       }
    }

    public class OkWithSessionCookieResult : OkResult {
       private string _sessionId = System.Guid.NewGuid().ToString();

        public OkWithSessionCookieResult() : base() {}

        public OkWithSessionCookieResult(string sessionId) : base(){
            this._sessionId = sessionId;
        }
       public override void ExecuteResult(ActionContext context){
           context.HttpContext.Response.Cookies.Append("qna-session-id", this._sessionId, new CookieOptions(){HttpOnly=true, IsEssential=true, Secure = false });
           base.ExecuteResult(context);
       }
    }
}
