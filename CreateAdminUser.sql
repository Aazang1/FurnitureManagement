-- 创建新的管理员用户
-- 用户名: admin, 密码: 123456
INSERT INTO user (username, password, real_name, email, phone, role, status, created_at, updated_at)
VALUES (
    'admin',
    'e10adc3949ba59abbe56e057f20f883e',  -- 123456的MD5哈希
    '系统管理员',
    'admin@furniture.com',
    '13800000000',
    'Admin',
    'active',
    NOW(),
    NOW()
);

-- 或者创建另一个管理员
-- 用户名: manager, 密码: admin123
INSERT INTO user (username, password, real_name, email, phone, role, status, created_at, updated_at)
VALUES (
    'manager',
    '0192023a7bbd73250516f069df18b500',  -- admin123的MD5哈希
    '系统经理',
    'manager@furniture.com',
    '13800000001',
    'Manager',
    'active',
    NOW(),
    NOW()
);
