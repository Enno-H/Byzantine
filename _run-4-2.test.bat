call _byz.bat
call _arc.bat

rem                          N  L  ID V  V0 F  resp
start "byz1" cmd /k byz.exe  4  2  1  0  0  1  byz1.txt
start "byz1" cmd /k byz.exe  4  2  2  0  0  0
start "byz1" cmd /k byz.exe  4  2  3  1  0  0
start "byz1" cmd /k byz.exe  4  2  4  1  0  0

timeout  1
start "arc" cmd /k arc.exe 4
pause
