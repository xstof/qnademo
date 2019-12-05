using System;
using Newtonsoft.Json;

namespace Qna.Backend.WireModels
{
    public class Configuration{
        [JsonProperty("region")]
        public string Region { get; set; }
        public string UserName { get; set; } // not currently used
        [JsonProperty("signalRNegotiateUrl")]
        public string SignalRNegotiateUrl { get; set; }

        [JsonProperty("browserSessionId")]
        public string BrowserSessionId { get; set; }

        [JsonProperty("auth")]
        public Authentication Auth { get; set; }
    }

    public class Authentication{
        [JsonProperty("clientId")]
        public string ClientId { get; set; } = "d03fc97e-cc4e-4758-944a-43fe4cf3eecc";

        // TODO: CHANGE THIS TO REFLECT YOUR APP REGISTRATION AND AAD B2C TENANT:
        [JsonProperty("authority")]
        public string Authority { get; set; } = "https://xstofb2c.b2clogin.com/xstofb2c.onmicrosoft.com/b2c_1_susi";

        // TODO: CHANGE THIS TO REFLECT YOUR APP REGISTRATION AND AAD B2C TENANT:
        [JsonProperty("authoringScope")]
        public string AuthoringScope { get; set; } = "https://xstofb2c.onmicrosoft.com/qna/qna_author";
    }
}