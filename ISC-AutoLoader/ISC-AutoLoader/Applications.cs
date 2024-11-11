using Newtonsoft.Json;


namespace ISC_AutoLoader
{
    public class Applications
    {
        public Dictionary<String,String> apps = new Dictionary<String,String>();
        private UserInputs ui = new UserInputs();
        public Applications(String token) {
            
            var url = ui.getUrl() + "/v3/sources/";
            Console.WriteLine(url);
            var countUrl = url + "?limit=1&count=true";
            Console.WriteLine(countUrl);
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            var payload = client.GetAsync(countUrl).Result;
            int count = Int32.Parse(payload.Headers.GetValues("X-Total-Count").First());
            var json = payload.Content.ReadAsStringAsync().Result;
            //Console.WriteLine(json);
            Console.WriteLine(count);
            loadApps(token, count);



    }
        private void loadApps(String token, int max)
        {
            apps = new Dictionary<String,String>();
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            int start = 0;

            List<String> limit = ui.limitList();
            while (start< max)
            {
                var url = ui.getUrl() + "/v3/sources/" + "?limit=2&count=true&offset=" + start;
                Console.WriteLine(url);
                var appRaw = client.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
                List<SailPointApp> appList = (List<SailPointApp>)JsonConvert.DeserializeObject<List<SailPointApp>>(appRaw);
                foreach (SailPointApp app in appList){
                    Console.WriteLine(app.id + ":" + app.name + ":" + app.type);
                    //TODO should we have more than this or a filter?
                    if (ui.allSources()==true ||  app.type.Equals("DelimitedFile"))
                    {
                        if (limit.Count == 0) {
                            if (!apps.ContainsKey(app.name))
                            {
                                apps.Add(app.name, app.id);
                            }

                        }
                        else
                        {
                            if (!apps.ContainsKey(app.name) && limit.Contains(app.name))
                            {
                                apps.Add(app.name, app.id);
                            }
                        }
                        
                    }
                    start++;
                }

            }


        }
        

        private class SailPointApp
        {
            public String id { get; set; }
            public String name { get; set; }

            public String type { get; set; }
        }

        
        
        
    }
}
