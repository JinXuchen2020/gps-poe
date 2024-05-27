set localpath="C:\Program Files (x86)\Beyondsoft\POE Email Service"
set serviceName="POEEmailService"
set diaplayName="POE Email Service"
set serviceDescription="This Service is host the POE email Service"

xcopy /s /i "%~dp0" %localpath%
sc.exe create %serviceName% binpath=%localpath%\GSMO_OMS_DisableUSBStorage_WinServiceVersion.exe start=auto displayname=%diaplayName%
sc description %serviceName% %serviceDescription%

sc.exe start %serviceName%

@echo ###################################################################
@echo 服务已经成功安装，按任意建重启电脑应用该服务，请保存电脑上文件都已经保存好！！！ 
@echo 点击任意按键继续!
@echo The Service has been installed, the final step is to reboot the PC. 
@echo please ensure all your files have been saved before it reboot.
@echo Press any key to continue!
@echo ###################################################################
Pause

shutdown /r /t 3