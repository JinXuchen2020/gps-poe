set localpath="C:\Program Files (x86)\Beyondsoft\POE Email Service"
set serviceName="POEEmailService"

sc.exe stop %serviceName%
sc.exe delete %serviceName%

rmdir %localpath% /s /Q

timeout /T 2 /NOBREAK
rmdir %localpath% /s /Q