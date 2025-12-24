-- 修复supplier表结构脚本
USE furniture_management;

-- 1. 备份现有数据
DROP TABLE IF EXISTS supplier_backup;
CREATE TABLE supplier_backup AS SELECT * FROM supplier;

-- 2. 检查并添加缺失的字段
-- 添加email字段（如果不存在）
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
     WHERE TABLE_SCHEMA = 'furniture_management' 
     AND TABLE_NAME = 'supplier' 
     AND COLUMN_NAME = 'email') = 0,
    'ALTER TABLE supplier ADD COLUMN email VARCHAR(255) NULL',
    'SELECT "email字段已存在" as message'
));
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- 添加updated_at字段（如果不存在）
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
     WHERE TABLE_SCHEMA = 'furniture_management' 
     AND TABLE_NAME = 'supplier' 
     AND COLUMN_NAME = 'updated_at') = 0,
    'ALTER TABLE supplier ADD COLUMN updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP',
    'SELECT "updated_at字段已存在" as message'
));
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- 3. 确保created_at字段有默认值
ALTER TABLE supplier MODIFY COLUMN created_at DATETIME DEFAULT CURRENT_TIMESTAMP;

-- 4. 更新现有记录的updated_at字段（如果为NULL）
UPDATE supplier SET updated_at = created_at WHERE updated_at IS NULL;
UPDATE supplier SET updated_at = NOW() WHERE updated_at IS NULL;

-- 5. 显示修复后的表结构
DESCRIBE supplier;

-- 6. 显示数据样本
SELECT * FROM supplier LIMIT 3;

-- 7. 验证字段完整性
SELECT 
    supplier_id,
    supplier_name,
    contact_person,
    phone,
    email,
    address,
    created_at,
    updated_at
FROM supplier 
WHERE supplier_id IN (1, 2, 3);

SELECT '修复完成！' as status;
