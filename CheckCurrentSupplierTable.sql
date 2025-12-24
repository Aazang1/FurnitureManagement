-- 只检查当前supplier表结构，不做任何修改
USE furniture_management;

-- 1. 显示表结构
DESCRIBE supplier;

-- 2. 显示详细字段信息
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT,
    COLUMN_KEY
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_SCHEMA = 'furniture_management' 
AND TABLE_NAME = 'supplier'
ORDER BY ORDINAL_POSITION;

-- 3. 显示现有数据样本
SELECT * FROM supplier LIMIT 3;
