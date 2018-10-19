call _byz.bat
call _arc.bat

rem                          N  L  ID V  V0 F  resp
start "byz1" cmd /k byz.exe  8  3  1  0  0  0  byz1.txt
start "byz1" cmd /k byz.exe  8  3  2  1  0  0
start "byz1" cmd /k byz.exe  8  3  3  0  0  0
start "byz1" cmd /k byz.exe  8  3  4  1  0  0
start "byz1" cmd /k byz.exe  8  3  5  0  0  0
start "byz1" cmd /k byz.exe  8  3  6  1  0  0
start "byz1" cmd /k byz.exe  8  3  7  1  0  0
start "byz1" cmd /k byz.exe  8  3  8  0  0  0

timeout  1
start "arc" cmd /k arc.exe 8
pause
