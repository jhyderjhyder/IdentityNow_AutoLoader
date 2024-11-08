#Read-Host "Give me data" 
#c:\temp\fileUpload\commands\Badging.ps1 -ApplicationID "123456" -ApplicationName "Badging"
param ($ApplicationName, $ApplicationID, $rootFolder, $fileName)

Write-Output "ApplicationName = $ApplicationName"
Write-Output "ApplicationID = $ApplicationID"
Write-Output "rootFolder = $rootFolder"
Write-Output "fileName = $fileName"

$ApplicationName ="MyAppName"
$ApplicationID = "11223"
$rootFolder = "c::"
$fileName = "fname"
#limited testing might need to use Start-Process but seems to wait to return testing until this command is done
java -cp ./bsh-2.0b4.jar bsh.Interpreter ./preIterate.java $ApplicationName $ApplicationID $rootFolder $fileName
#Start-Process 'java -cp ./bsh-2.0b4.jar bsh.Interpreter ./preIterate.java' -Wait -NoNewWindow

return "testing $args";