# PowerShell脚本测试API连接
Write-Host "=== 家具管理系统API连接测试 ===" -ForegroundColor Green
Write-Host ""

$baseUrl = "http://localhost:5192"
$apiUrl = "$baseUrl/api"

Write-Host "测试基础URL: $baseUrl" -ForegroundColor Yellow
Write-Host "测试API URL: $apiUrl" -ForegroundColor Yellow
Write-Host ""

# 测试基础连接
Write-Host "1. 测试基础连接..." -ForegroundColor Cyan
try {
    $response = Invoke-WebRequest -Uri $baseUrl -Method GET -TimeoutSec 10
    Write-Host "✓ 基础连接成功 - 状态码: $($response.StatusCode)" -ForegroundColor Green
} catch {
    Write-Host "✗ 基础连接失败: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# 测试供应商API
Write-Host "2. 测试供应商API..." -ForegroundColor Cyan
try {
    $supplierUrl = "$apiUrl/Supplier"
    $response = Invoke-WebRequest -Uri $supplierUrl -Method GET -TimeoutSec 10
    Write-Host "✓ 供应商API连接成功 - 状态码: $($response.StatusCode)" -ForegroundColor Green
    
    # 尝试解析JSON响应
    $content = $response.Content | ConvertFrom-Json
    if ($content -is [array]) {
        Write-Host "✓ 返回了 $($content.Count) 个供应商记录" -ForegroundColor Green
        if ($content.Count -gt 0) {
            Write-Host "示例数据: $($content[0] | ConvertTo-Json -Compress)" -ForegroundColor Gray
        }
    } else {
        Write-Host "响应内容: $($response.Content)" -ForegroundColor Gray
    }
} catch {
    Write-Host "✗ 供应商API连接失败: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# 测试分类API
Write-Host "3. 测试分类API..." -ForegroundColor Cyan
try {
    $categoryUrl = "$apiUrl/Category"
    $response = Invoke-WebRequest -Uri $categoryUrl -Method GET -TimeoutSec 10
    Write-Host "✓ 分类API连接成功 - 状态码: $($response.StatusCode)" -ForegroundColor Green
} catch {
    Write-Host "✗ 分类API连接失败: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# 测试家具API
Write-Host "4. 测试家具API..." -ForegroundColor Cyan
try {
    $furnitureUrl = "$apiUrl/Furniture"
    $response = Invoke-WebRequest -Uri $furnitureUrl -Method GET -TimeoutSec 10
    Write-Host "✓ 家具API连接成功 - 状态码: $($response.StatusCode)" -ForegroundColor Green
} catch {
    Write-Host "✗ 家具API连接失败: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "=== 测试完成 ===" -ForegroundColor Green
Write-Host "如果所有测试都成功，请重新启动客户端应用程序" -ForegroundColor Yellow
Write-Host "按任意键退出..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
