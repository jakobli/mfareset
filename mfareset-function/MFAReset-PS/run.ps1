$FunctionName = 'MFAReset-PS'
$ModuleName = 'MSOnline'
$ModuleVersion = '1.1.166.0'
$username = $Env:User
#import PS module
$PSModulePath = "D:\home\site\wwwroot\$FunctionName\bin\$ModuleName\$ModuleVersion\$ModuleName.psd1"
Import-module $PSModulePath
$endpoint = $env:MSI_ENDPOINT
$secret= $env:MSI_SECRET
# Vault URI to get AuthN Token
$vaultTokenURI = 'https://vault.azure.net&api-version=2017-09-01'
# Our Key Vault Credential that we want to retreive URI
# NOTE: API Ver for this is 2015-06-01
$vaultSecretURI = '<YOUR KEYVAULT URI>'
# Create AuthN Header with our Function App Secret
$header = @{'Secret' = $secret}
# Get Key Vault AuthN Token
$authenticationResult = Invoke-RestMethod -Method Get -Headers $header -Uri ($endpoint +'?resource=' +$vaultTokenURI)
# Use Key Vault AuthN Token to create Request Header
$requestHeader = @{ Authorization = "Bearer $($authenticationResult.access_token)" }
# Call the Vault and Retrieve Creds
$creds = Invoke-RestMethod -Method GET -Uri $vaultSecretURI -ContentType 'application/json' -Headers $requestHeader

# Build Credentials
$secpassword = $creds.value | ConvertTo-SecureString -AsPlainText -Force
$credential = New-Object System.Management.Automation.PSCredential ($username, $secpassword)
# Connect to MSOnline
Connect-MsolService -Credential $credential
# POST method: $req
$requestBody = Get-Content $req -Raw | ConvertFrom-Json
$UserPrincipleName = $requestBody.UPN
# Start Script
#Disable MFA for a user 
$beforeMFA = Get-MsolUser -UserPrincipalName $UserPrincipleName | Select StrongAuthenticationRequirements
Set-MsolUser -UserPrincipalName $UserPrincipleName -StrongAuthenticationMethods @()
$afterMFA = Get-MsolUser -UserPrincipalName $UserPrincipleName | Select StrongAuthenticationRequirements
while($beforeMFA -eq $afterMFA)
{
    Start-Sleep -m 500
    $afterMFA = Get-MsolUser -UserPrincipalName $UserPrincipleName | Select StrongAuthenticationRequirements
} 

#Send notification Email
$user = Get-MSOLUser -UserPrincipalName $UserPrincipleName | Select DisplayName, @{Name="PrimaryEmailAddress";Expression={$_.ProxyAddresses | ?{$_ -cmatch '^SMTP\:.*'}}}
$email = $user.PrimaryEmailAddress.split(":")[1]
$MailMessage = @"
<font face="Calibri">Hi $($user.DisplayName),<br>
<br>
We have received a request to reset your MFA settings.<br>
Please <a href="https://aka.ms/mfasetup" target="_blank">click here</a> to update your settings.<br>
If you require any assistance in how to set up your MFA application please <a href="https://docs.microsoft.com/en-us/azure/multi-factor-authentication/end-user/microsoft-authenticator-app-how-to" target="_blank">click here</a><br>
Didn't request this change? Please contact <a href="mailto: <YOUR EMAIL>" target="_blank">Cybersecurity Operations</a> as soon as possible.<br><br>

Best wishes,<br>
IT
"@
Send-MailMessage -From "<YOUR EMAIL>" -To $email -BodyAsHtml -Body $MailMessage -Subject "MFA has been reset" -SmtpServer <YOUR SMTPSERVER> -Encoding Default 
Out-File -Encoding Ascii -FilePath $res -inputObject "User $UserPrincipleName has been reset"