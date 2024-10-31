using Newtonsoft.Json;

namespace ISC_AutoLoader
{
    public class OAuth
    {
        private String grant_type = "client_credentials";

        public String currentToken = null;
        public OAuth()
        {
            currentToken = getNewToken();
        }
        public String getNewToken()
        {
            UserInputs ui = new UserInputs();
            String singlePost = ui.getUrl() + "/oauth/token?grant_type=" + grant_type + "&client_id=" + ui.getClientId() + "&client_secret=" + ui.getClientSecret();
            HttpClient client = new HttpClient();
            var response = client.PostAsync(singlePost, null).Result;
            String json = response.Content.ReadAsStringAsync().Result;
            TokenObject token = JsonConvert.DeserializeObject<TokenObject>(json);  
            return token.access_token;
        }

        private class TokenObject
        {
            public String access_token { get; set; }
            public String token_type { get; set; }
            public String tenant_id { get; set; }

        }
    }
}
