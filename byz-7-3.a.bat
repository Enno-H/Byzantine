call _byz.bat
call _arc.bat

rem                          N  L  ID V  V0 F  resp
start "byz1" cmd /k byz.exe  7  3  1  1  0  0  byz1.txt
start "byz1" cmd /k byz.exe  7  3  2  0  0  0
start "byz1" cmd /k byz.exe  7  3  3  1  0  0
start "byz1" cmd /k byz.exe  7  3  4  0  0  0
start "byz1" cmd /k byz.exe  7  3  5  1  0  0
start "byz1" cmd /k byz.exe  7  3  6  0  0  0
start "byz1" cmd /k byz.exe  7  3  7  1  0  0

timeout  1
start "arc" cmd /k arc.exe 7
pause
