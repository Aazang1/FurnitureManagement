@echo off
echo ===================================
echo 家具管理系统诊断工具
echo ===================================
echo.

cd /d "d:\c++\FurnitureManagement"

echo 1. 运行系统诊断...
echo.
dotnet run --project DiagnosticTool.cs

echo.
echo ===================================
echo.
echo 2. 是否要启动简单测试服务器？ (Y/N)
set /p choice=请选择: 

if /i "%choice%"=="Y" (
    echo.
    echo 启动简单测试服务器...
    echo 注意：可能需要管理员权限
    echo.
    dotnet run --project SimpleTestServer.cs
) else (
    echo 跳过测试服务器启动
)

echo.
pause
