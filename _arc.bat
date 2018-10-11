if exist arc.exe del arc.exe
csc /r:System.ServiceModel.dll /r:System.ServiceModel.Web.dll interface.cs arc.cs
pause

rem arc.exe 4
rem pause
