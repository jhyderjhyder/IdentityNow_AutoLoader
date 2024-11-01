using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISC_AutoLoader
{
    public class UserInputs
    {
        public UserInputs() { }

        private String url = Environment.GetEnvironmentVariable("ISC_URL");
        private String client_id = Environment.GetEnvironmentVariable("ISC_CLIENT_ID");
        private String client_secret = Environment.GetEnvironmentVariable("ISC_CLIENT_SECRET");
        private String startingFolder = Environment.GetEnvironmentVariable("ISC_FOLDER");

        //TODO extra validation
        public String getUrl(){return "https://" + url;}
        public String getClientId() { return client_id;}
        public String getClientSecret() { return client_secret;}
        public String getStartingFolder() {  return startingFolder;}

        public Boolean shouldArchive()
        {
            if (File.Exists(getStartingFolder()+ "/noArchive.txt"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
