@echo off
chcp 65001
setlocal enabledelayedexpansion

set /p port="[TTT] 输入端口号: "
echo.

echo [TTT] 正在查询端口 %port% 的占用情况...
netstat -ano | findstr :%port%
echo.

set "pid="

for /f "tokens=4" %%i in ('netstat -ano ^| findstr :%port%') do (
    set "pid=%%i"
    goto :result
)

:result
if defined pid (
    echo [TTT] 找到占用端口 %port% 的进程ID: !pid!
) else (
    echo [TTT] 没有找到占用端口 %port% 的进程。
    goto end
)

:: 询问用户是否结束这个进程
set /p kill="[TTT] 是否要结束这个进程? (y/n): "
if /i "!kill!"=="y" (
    echo [TTT] 正在结束进程 !pid!...
    taskkill /F /PID !pid!
    if !errorlevel! equ 0 (
        echo [TTT] 成功结束进程 !pid!。
    ) else (
        echo [TTT] 无法结束进程 !pid!。
    )
) else (
    echo [TTT] 操作已取消。
)

:end
echo [TTT] 操作已结束。

pause
endlocal