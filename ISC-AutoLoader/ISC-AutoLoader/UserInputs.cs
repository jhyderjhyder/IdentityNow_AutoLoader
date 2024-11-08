

namespace ISC_AutoLoader
{
    public class UserInputs
    {
        public UserInputs() {
            String configFile = this.startingFolder + "\\commands\\config.ini";
            if (File.Exists(configFile))
            {
                var data = File.ReadLines(configFile);
                foreach (String line in data)
                {
                    if (!line.StartsWith("#") && line.Contains("="))
                    {
                        var lineData = line.Split('=');
                        if (lineData.Length <= 2){
                            String key = lineData[0].Trim();
                            String value = lineData[1];
                            if (!config.ContainsKey(key))
                            {
                                config.Add(key, value);
                            }
                        }
                    }
                        
                        
                }
            }
            else
            {
                StreamWriter logWriter = new StreamWriter(configFile, true);
                logWriter.WriteLine("# Optional Configuration");
                logWriter.WriteLine("archive=true");
                logWriter.Flush();
                logWriter.Close();
            }
        
        }
        private String decryptedSecret = null;
        private String url = Environment.GetEnvironmentVariable("ISC_URL");
        private String client_id = Environment.GetEnvironmentVariable("ISC_CLIENT_ID");
        private String client_secret = Environment.GetEnvironmentVariable("ISC_CLIENT_SECRET");
        private String startingFolder = Environment.GetEnvironmentVariable("ISC_FOLDER");
        public Dictionary<String,String> config = new Dictionary<String,String>();

        //TODO extra validation
        public String getUrl(){return "https://" + url;}
        public String getClientId() { return client_id;}
        public String getClientSecret() { 
            if  (config.ContainsKey("EncryptKey")){
                if (decryptedSecret == null)
                {
                    Encrypt e = new Encrypt(config.ContainsKey("EncryptKey").ToString()) ;
                    decryptedSecret = e.decrypt(client_secret);
                }
                return decryptedSecret;

            }
            return client_secret;
        
        }
        public String getStartingFolder() {  return startingFolder;}

        /// <summary>
        /// This controls if the system should keep the uploaded
        /// file on the server
        /// </summary>
        /// <returns></returns>
        public Boolean shouldArchive()
        {
            if (config.ContainsKey("archive"))
            {
                String details = config["archive"];
                if (details.ToLower().Equals("false"))
                {
                    return false;
                }
            }
                return true;
            
        }
        /// <summary>
        /// As you want to give your filesystem a break depending on what
        /// you are trying to do with it.  This should 
        /// </summary>
        /// <returns></returns>
        public int scanPerSecond()
        {
            if (config.ContainsKey("delayScanPerSecond"))
            {
                try
                {
                    String details = config["delayScanPerSecond"];
                    int result = Int32.Parse(details);
                    return result;
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
               
            }
            
             
            return 60;
        }

        public List<String> limitList()
        {
            List<String> list = new List<String>();
            if (config.ContainsKey("limitList"))
            {
                try
                {
                    String details = config["limitList"];
                    var apps = details.Split(",");
                    foreach (String app in apps)
                    {
                        var test  = app.Trim();
                        if (!list.Contains(test))
                        {
                            list.Add(test);
                        }
                    }
                    return list;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

            }
            return list;
        }

        public int timeout()
        {
            if (config.ContainsKey("restTimeOut"))
            {
                try
                {
                    String details = config["restTimeOut"];
                    int result = Int32.Parse(details);
                    return result;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

            }
            return 0;
        }
    }
}
