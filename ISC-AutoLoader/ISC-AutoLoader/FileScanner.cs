using Microsoft.Extensions.Hosting;
using RestSharp;
using System.Collections;
using System.Management.Automation;

namespace ISC_AutoLoader
{
    public class FileScanner : BackgroundService
    {
        public UserInputs _userInputs = new UserInputs();
        private String commandsFolder = "/commands";
        private Dictionary<String, String> masterApps = new Dictionary<String, String>();
        private String[] resultFolders = { "success", "failure", "response" };
        private int runNumber = 0;
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            int scanPerSecound = _userInputs.scanPerSecond();

            while (!stoppingToken.IsCancellationRequested)
            {
                runNumber++;
                foreach (String appName in masterApps.Keys)
                {
                    String folder = _userInputs.getStartingFolder() + "\\" + appName;
                    Console.WriteLine(folder);
                    string[] files = Directory.GetFiles(folder);
                    foreach (String fileName in files)
                    {
                        Console.WriteLine("Proccess:" + fileName);
                        processOneFile(fileName, masterApps[appName], folder, appName);
                    }
                }

                //https://devrel-ga-13054.api.identitynow-demo.com/beta/sources/0e1fcbb123cb4424a16db0f1af9fb526/load-accounts
                int secoundsToDelay = ((1000) * scanPerSecound) ;
                Console.WriteLine("Processed run #" + runNumber);
                //Not sure the max int size but no reason to keep it going forever this is just from the console to review results
                //we troubleshooting ps scripts
                if (runNumber > 10000)
                {
                    runNumber = 0;
                }
                await Task.Delay(secoundsToDelay, stoppingToken);
            }
        }
        /// <summary>
        /// This is where the main functionality is found
        /// </summary>
        /// <param name="file">FULL filePath</param>
        /// <param name="appID">Id number of the application</param>
        /// <param name="root"> Folder to start containing the root + the applicationName </param>
        private void processOneFile(String file, String appID, String root,String appName)
        {
        
            
          
            Boolean skip = false;
            try
            {
                String fileName = Path.GetFileName(file);
                String extention = Path.GetExtension(file);
                Console.WriteLine("Extention:" + extention);
                if (extention == null)
                {
                    extention = "txt";
                }
                String test = (file.Substring(0, file.Length - extention.Length));
                Console.WriteLine("checking for file::::" + test);
                
                //Check to see this is lock file to let us know that it needs to be skiped
                if (extention.Equals(".lck") || extention.Equals(".lock"))
                {
                    skip = true;
                }
                else
                {
                    //Good file to upload lets make sure there is not a lock file in the folder so that we should skip
                    //as the owner is currently writting the file
                   
                    if (File.Exists(test + ".lck"))
                    {
                        skip = true;
                    }
                    if (File.Exists(test + ".lock"))
                    {
                        skip = true;
                    }
                }

                //TODO any way to check if the file is open?
                if (skip == false)
                {
                    
                    String psCommand = _userInputs.getStartingFolder() + commandsFolder + "\\" + appName + ".ps1";
                    Console.WriteLine("checking for preIterate rule");
                    Console.WriteLine(psCommand);
                    String now = DateTime.Now.ToString("yyyy-MM-dd--hh--mm");
                    if (File.Exists(psCommand))
                    {
                        PowerShell ps = PowerShell.Create();
                        Console.WriteLine("Processing Custom PowerShell Script");
                        IDictionary parm = new Dictionary<String, String>();

                        parm.Add("ApplicationName", appName);
                        parm.Add("ApplicationID", appID);
                        parm.Add("rootFolder", root);
                        parm.Add("fileName", file);
                        ps.AddScript(File.ReadAllText(psCommand)).AddParameters(parm).Invoke();
                        String psLog = root + "/response/ps_" + now + ".log";
                        StreamWriter logWriter = new StreamWriter(psLog, true);
                        foreach (PSObject psResults in ps.Invoke())
                        {  
                            logWriter.WriteLine(psResults.ToString());  
                        }
                        logWriter.Flush();
                        logWriter.Close();
                        ps.Dispose();
                    }

                    OAuth oAuth = new OAuth();
                    //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + oAuth.getNewToken());
                    String oper = "/load-accounts";
                    if (Path.GetFileNameWithoutExtension(file).ToLower().ToString().StartsWith("ent-"))
                    {
                        oper = "/load-entitlements";
                        Console.WriteLine("Loading Entitlements data");
                    }

                    var url = _userInputs.getUrl() + "/beta/sources/" + appID + oper;
                    Console.WriteLine(url);
                    RestClient client = new RestClient(new RestClientOptions(url));
                    
                    
                    RestRequest request = new RestRequest(url, Method.Post);
                    int max = _userInputs.timeout();
                    if (max != 0)
                    {
                        request.Timeout = TimeSpan.FromMinutes(max);
                    }
                    
                    request.AddHeader("Authorization", "Bearer " + oAuth.getNewToken());
                    request.AlwaysMultipartFormData = true;
                    request.AddFile("file", file);
                    RestResponse result = client.ExecuteAsync(request).Result;

                    Console.WriteLine(result.IsSuccessStatusCode);
                    now = DateTime.Now.ToString("yyyy-MM-dd--hh--mm");

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
                    
                    
                    client.Dispose();
                }
                else
                {
                    Console.WriteLine("File should be skiped for now");
                }
            }
            //Catch random issues like file being locked

            catch (Exception ex)
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
        public FileScanner()
        {
            validate();
            OAuth auth = new OAuth();
            String token = auth.getNewToken();
            Console.WriteLine($"{token}");
            Applications apps = new Applications(token);
            Directory.CreateDirectory(_userInputs.getStartingFolder() + commandsFolder);

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
