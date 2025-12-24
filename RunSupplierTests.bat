@echo off
echo ===================================
echo 供应商问题诊断和修复工具
echo ===================================
echo.

cd /d "d:\c++\FurnitureManagement"

echo 请选择操作：
echo 1. 检查数据库表结构和数据
echo 2. 修复数据库表结构
echo 3. 测试供应商API
echo 4. 运行所有测试
echo.

set /p choice=请输入选择 (1-4): 

if "%choice%"=="1" (
    echo.
    echo 正在检查数据库...
    dotnet run --project TestSupplierDatabase.cs
) else if "%choice%"=="2" (
    echo.
    echo 正在修复数据库表结构...
    echo 请在MySQL中运行 FixSupplierTable.sql 脚本
    echo 脚本位置: d:\c++\FurnitureManagement\FixSupplierTable.sql
    echo.
    echo 您可以使用以下命令：
    echo mysql -u root -p furniture_management ^< FixSupplierTable.sql
    echo.
    pause
) else if "%choice%"=="3" (
    echo.
    echo 正在测试供应商API...
    dotnet run --project TestSupplierAPI.cs
) else if "%choice%"=="4" (
    echo.
    echo 运行完整测试...
    echo.
    echo 1. 数据库测试:
    dotnet run --project TestSupplierDatabase.cs
    echo.
    echo 2. API测试:
    dotnet run --project TestSupplierAPI.cs
) else (
    echo 无效选择
)

echo.
pause
