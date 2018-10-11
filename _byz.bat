if exist arc.exe del arc.exe
csc /r:System.ServiceModel.dll /r:System.ServiceModel.Web.dll interface.cs byz.cs
pause

rem byz.exe ...
rem pause
