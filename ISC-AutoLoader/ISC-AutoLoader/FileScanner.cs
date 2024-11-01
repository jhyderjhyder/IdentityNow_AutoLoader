using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ISC_AutoLoader
{
    public class FileScanner : BackgroundService
    {
        public UserInputs _userInputs = new UserInputs();

        private Dictionary<String, String> masterApps = new Dictionary<String, String>();
        private String[] resultFolders = { "success", "failure", "response" };
        private int runNumber = 0;
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            
            while (!stoppingToken.IsCancellationRequested)
            {
                runNumber++;
                foreach (String appName in masterApps.Keys)
                {
                    String folder = _userInputs.getStartingFolder() + "/" + appName;
                    Console.WriteLine(folder);
                    string[] files = Directory.GetFiles(folder);
                    foreach (String fileName in files)
                    {
                        Console.WriteLine("Proccess:" + fileName);
                        processOneFile(fileName, masterApps[appName], folder);
                    }
                }

                //https://devrel-ga-13054.api.identitynow-demo.com/beta/sources/0e1fcbb123cb4424a16db0f1af9fb526/load-accounts
                int secoundsToDelay = 1000 * 60;
                Console.WriteLine("Processed run #" + runNumber);
                await Task.Delay(secoundsToDelay, stoppingToken);
            }
        }

        private void processOneFile(String file, String appID, String root){
            try
            {
                OAuth oAuth = new OAuth();
                //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + oAuth.getNewToken());
                var url = _userInputs.getUrl() + "/beta/sources/" + appID + "/load-accounts";
                Console.WriteLine(url);
                RestClient client = new RestClient(new RestClientOptions(url));
                RestRequest request = new RestRequest(url, Method.Post);
                request.AddHeader("Authorization", "Bearer " + oAuth.getNewToken());
                request.AlwaysMultipartFormData = true;
                request.AddFile("file", file);
                RestResponse result = client.ExecuteAsync(request).Result;

                Console.WriteLine(result.IsSuccessStatusCode);
                String now = DateTime.Now.ToString("yyyy-MM-dd--hh--mm");
                String fileName = Path.GetFileName(file);

                if (_userInputs.shouldArchive() == true)
                {
                    String logName = root + "/response/" + now + ".log";
                    StreamWriter logWriter = new StreamWriter(logName, true);
                    logWriter.WriteLine(result.Content);
                    logWriter.Flush();
                    logWriter.Close();
                    if (result.IsSuccessStatusCode)
                    {

                        String locationNew = root + "/success/" + now + "_" + fileName;
                        File.Move(file, locationNew);
                    }
                    else
                    {
                        String locationNew = root + "/failure/" + now + "_" + fileName;
                        File.Move(file, locationNew);
                    }
                }
                else
                {
                    String logName = root + "/response/trace.log";
                    StreamWriter logWriter = new StreamWriter(logName, true);
                    logWriter.WriteLine(now + ":processed:" + file + ":status_code:" + result.StatusCode);
                    logWriter.Flush();
                    logWriter.Close();
                    File.Delete(file);
                }
            //Catch random issues like file being locked
            }catch(Exception ex)
            {
                String logName = _userInputs.getStartingFolder() + "/error.log";
                StreamWriter logWriter = new System.IO.StreamWriter(logName, true);
                logWriter.WriteLine(ex.ToString());
                logWriter.Flush();
                logWriter.Close();
            }


        }

        private void validate()
        {
            if (_userInputs.getUrl() == null)
            {
                Console.WriteLine("ISC_URL is required");
            }
            if (_userInputs.getClientId() == null)
            {
                Console.WriteLine("ISC_CLIENT_ID is required");
            }
            if (_userInputs.getStartingFolder() == null)
            {
                Console.WriteLine("ISC_FOLDER is required");
               
            }
            if (!Directory.Exists(_userInputs.getStartingFolder()))
            {
                throw new Exception("Folder not found:" + _userInputs.getStartingFolder());
            }
        }
        public FileScanner() {
            validate();
            OAuth auth = new OAuth();
            String token = auth.getNewToken();
            Console.WriteLine($"{token}");
            Applications apps = new Applications(token);
            
            foreach (String key in apps.apps.Keys)
            {
                masterApps.Add(key, apps.apps[key]);
                String childFolder = _userInputs.getStartingFolder() + "/" + key;
                if (!Directory.Exists(childFolder))
                {
                    Directory.CreateDirectory(childFolder);
                }
                foreach (String results in resultFolders)
                {
                    var r = childFolder + "/" + results;
                    if (!Directory.Exists(r))
                    {
                        Directory.CreateDirectory(r);
                    }
                }
            }

        
        }
    }
}
