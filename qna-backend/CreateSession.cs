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
using Qna.Backend.WireModels;
using System.Security.Claims;
using System.Linq;

namespace Qna.Backend
{
    public static class CreateSession
    {
        [FunctionName("CreateSession")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] SessionCreationDetails creationDetails, 
            [CosmosDB(
                databaseName: "%cosmosdbname%",
                collectionName: "%cosmoscollectionname%",
                ConnectionStringSetting = "cosmosconnectionstring",
                PartitionKey = "session-{id}",
                Id = "{id}")] CosmosModels.Session session, // {id} comes from creationDetails in POST body
            [CosmosDB(
                databaseName: "%cosmosdbname%",
                collectionName: "%cosmoscollectionname%",
                ConnectionStringSetting = "cosmosconnectionstring")] out CosmosModels.Session sessionToCreate,
                ClaimsPrincipal principal,
            ILogger log)
        {
            log.LogInformation($"CreateSession was called for session {creationDetails.name} with id {creationDetails.id}.");
            
            // write out incoming claims:
            var isLoggedIn = false;
            if(principal != null && principal.Claims != null && principal.Claims.Count() > 0) {
                foreach (Claim claim in principal.Claims)  
                {  
                    log.LogInformation("CLAIM TYPE: " + claim.Type + "; CLAIM VALUE: " + claim.Value);  
                } 
                isLoggedIn = true;
            }
            else{
                log.LogInformation("Not found any claims.");
            }

            if(String.IsNullOrWhiteSpace(creationDetails.id)){
                log.LogError("Invalid session id provided");
                sessionToCreate = null;
                return new BadRequestObjectResult("Please provide a valid session id");
            }
            else if(String.IsNullOrWhiteSpace(creationDetails.name)){
                log.LogError("Invalid session name provided");
                sessionToCreate = null;
                return new BadRequestObjectResult("Please provide a valid session name");
            }
            else if (session != null) {
                sessionToCreate = null;
                log.LogError("Session exists already and cannot be created twice: " + session.SessionId);
                    return new BadRequestObjectResult("Session exists already");
            }
            else{
                log.LogTrace("writing session to Cosmosdb");

                var authorSub = "";
                var authorFirstName = "";
                var authorLastName = "";
                var authorFullName = "";
                var authorEmail = "";

                if(isLoggedIn){
                    log.LogInformation("recording author on the cosmos object");
                    authorSub = principal.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ?? "";
                    authorFirstName = principal.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname")?.Value ?? "";
                    authorLastName = principal.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname")?.Value ?? "";
                    authorFullName = $"{authorFirstName} {authorLastName}";
                    log.LogInformation($"author: {authorFirstName} {authorLastName} with sub: {authorSub}");
                    authorEmail = principal.Claims.FirstOrDefault(c => c.Type == "emails")?.Value ?? "";
                }

                sessionToCreate = new CosmosModels.Session(){
                    Id = creationDetails.id,
                    PartitionId = $"session-{creationDetails.id}",
                    SessionId = creationDetails.id,
                    SessionName = creationDetails.name,
                    AuthorSub = authorSub,
                    AuthorName = authorFullName,
                    AuthorEmail = authorEmail
                };
                
                // return ok
                return new OkResult();       
            }
        }
    }
}
