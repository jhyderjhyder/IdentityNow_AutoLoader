# IdentityNow_AutoLoader
IdentityNow ISC AutoLoader for Delimited Files.  A simple windows service to send csv files to IdentityNow

# Reason
The non cloud version could pickup files from your local ftp system or windows box.  As a customer you could schedule the pickup and use file protections that you normally use to secure your data.  IdentityNow is on a Cloud hosted solution so we need to detect when a file is created and upload over https.

## Process
The code will connect to your instance of IdentityNow and create folders for each application that is a delimitted Application.  Inside the folder it will create three child folders. 
1) Success (Upload got a good response from the application)
2) Failure (Upload had some type of issue)
3) Response (http response code recieved from the upload command)

As an admin you will need to set the permissions correctly if you want to prevent one application owner from adding files to the wrong folder.  The Service account that runs the windows service will need read and write access.  When the application owner copies a file into root of the Folder drop box the system will scan and see the new file.  It will upload the file to IdentityNow and then move the file into one of the child folders with the date it was processed.  

# Setup
You just need to define a few user variables. And DotNot Core 8x.  Only works on Windows as its a Windows Service

1) ISC_URL = tenant.API.identitynow.com
2) ISC_CLIENT_ID = PAT_ID value
3) ISC_CLIENT_SECRET = pat password
4) ISC_FOLDER = c:\fileUpload  (This is where all the application folders will be created)

## Validate
```cmd 
./ISC-AutoLoader.exe
```

## Install as Windows Service
```cmd
sc create "IdentityNow AutoLoader" binPath c:\IIQ\ISC-AutoLoader.exe
```

## UnInstall Windows Service
```cmd
sc delete "IdentityNow AutoLoader
```

Hope this helps the community.  
Jed Hyder