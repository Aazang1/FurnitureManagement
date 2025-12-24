-- 检查和修复supplier表结构
USE furniture_management;

-- 1. 检查当前表结构
DESCRIBE supplier;

-- 2. 检查是否缺少字段，如果缺少则添加
-- 检查email字段
SELECT COUNT(*) as email_exists FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_SCHEMA = 'furniture_management' 
AND TABLE_NAME = 'supplier' 
AND COLUMN_NAME = 'email';

-- 如果email字段不存在，添加它
-- ALTER TABLE supplier ADD COLUMN email VARCHAR(255) NULL;

-- 检查updated_at字段
SELECT COUNT(*) as updated_at_exists FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_SCHEMA = 'furniture_management' 
AND TABLE_NAME = 'supplier' 
AND COLUMN_NAME = 'updated_at';

-- 如果updated_at字段不存在，添加它
-- ALTER TABLE supplier ADD COLUMN updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP;

-- 3. 显示完整的表结构
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

-- 4. 显示现有数据
SELECT * FROM supplier LIMIT 5;

-- 5. 如果需要，创建标准的supplier表结构
/*
DROP TABLE IF EXISTS supplier_backup;
CREATE TABLE supplier_backup AS SELECT * FROM supplier;

DROP TABLE IF EXISTS supplier;
CREATE TABLE supplier (
    supplier_id INT AUTO_INCREMENT PRIMARY KEY,
    supplier_name VARCHAR(255) NOT NULL,
    contact_person VARCHAR(255) NULL,
    phone VARCHAR(50) NULL,
    email VARCHAR(255) NULL,
    address TEXT NULL,
    created_by INT NULL,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- 迁移数据
INSERT INTO supplier (supplier_id, supplier_name, contact_person, phone, email, address, created_by, created_at, updated_at)
SELECT 
    supplier_id,
    supplier_name,
    contact_person,
    phone,
    COALESCE(email, NULL) as email,
    address,
    created_by,
    COALESCE(created_at, NOW()) as created_at,
    COALESCE(updated_at, NOW()) as updated_at
FROM supplier_backup;
*/
