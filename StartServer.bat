@echo off
echo ===================================
echo 启动家具管理系统服务端
echo ===================================
echo.

cd /d "d:\15\guanli\FurnitureManagement.Server"

echo 正在启动服务端...
echo 服务端将运行在: http://localhost:5192
echo 按 Ctrl+C 可以停止服务端
echo.

dotnet run

pause
