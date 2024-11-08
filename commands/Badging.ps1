#Read-Host "Give me data" 
#c:\temp\fileUpload\commands\Badging.ps1 -ApplicationID "123456" -ApplicationName "Badging"
param ($ApplicationName, $ApplicationID, $rootFolder, $fileName)

Write-Output "ApplicationName = $ApplicationName"
Write-Output "ApplicationID = $ApplicationID"
Write-Output "rootFolder = $rootFolder"
Write-Output "fileName = $fileName"

(Get-Content $fileName).Replace('sailpointdemo.com', "customer.com") | Set-Content $fileName

return "testing $args";