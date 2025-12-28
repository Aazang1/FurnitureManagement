-- 向资金流水表插入测试数据
USE furniture_management;

INSERT INTO capital_flow (flow_date, flow_type, amount, description, reference_type, reference_id, created_by, created_at)
VALUES
    -- 收入记录
    ('2025-12-20', 'income', 3800.00, '销售沙发', 'sale', 1, 1, NOW()),
    ('2025-12-21', 'income', 7400.00, '销售沙发和餐桌', 'sale', 2, 1, NOW()),
    ('2025-12-22', 'income', 3800.00, '销售沙发', 'sale', 3, 1, NOW()),
    ('2025-12-23', 'income', 5800.00, '销售沙发和餐桌', 'sale', 4, 1, NOW()),
    ('2025-12-24', 'income', 14200.00, '批量销售沙发', 'sale', 5, 1, NOW()),
    
    -- 支出记录
    ('2025-12-15', 'expense', 2500.00, '采购沙发', 'purchase', 1, 1, NOW()),
    ('2025-12-16', 'expense', 1200.00, '采购餐桌', 'purchase', 2, 1, NOW()),
    ('2025-12-17', 'expense', 500.00, '采购配件', 'purchase', 3, 1, NOW()),
    ('2025-12-18', 'expense', 800.00, '仓库租金', 'other', NULL, 1, NOW()),
    ('2025-12-19', 'expense', 300.00, '水电费', 'other', NULL, 1, NOW());

-- 查看插入的数据
SELECT * FROM capital_flow;
