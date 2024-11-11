#Read-Host "Give me data" 
#c:\temp\fileUpload\commands\Badging.ps1 -ApplicationID "123456" -ApplicationName "Badging"
param ($ApplicationName, $ApplicationID, $rootFolder, $fileName)

Write-Output "ApplicationName = $ApplicationName"
Write-Output "ApplicationID = $ApplicationID"
Write-Output "rootFolder = $rootFolder"
Write-Output "fileName = $fileName"

#Convert your windows formated file to UTF-8
Get-Content -Encoding "iso-8859-1" $fileName | Set-Content -Encoding "UTF8" $fileName


return "testing $args";