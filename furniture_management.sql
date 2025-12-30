/*
 Navicat Premium Dump SQL

 Source Server         : localhost_3306_1
 Source Server Type    : MySQL
 Source Server Version : 80037 (8.0.37)
 Source Host           : localhost:3306
 Source Schema         : furniture_management

 Target Server Type    : MySQL
 Target Server Version : 80037 (8.0.37)
 File Encoding         : 65001

 Date: 26/12/2025 16:25:18
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for capital_flow
-- ----------------------------
DROP TABLE IF EXISTS `capital_flow`;
CREATE TABLE `capital_flow`  (
  `flow_id` int NOT NULL AUTO_INCREMENT COMMENT '流水ID，主键，自增长',
  `flow_date` date NOT NULL COMMENT '流水日期',
  `flow_type` enum('income','expense') CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '资金流向：收入/支出',
  `amount` decimal(12, 2) NOT NULL COMMENT '金额',
  `description` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '流水描述',
  `reference_type` enum('purchase','sale','other') CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '关联类型：进货/销售/其他',
  `reference_id` int NULL DEFAULT NULL COMMENT '关联的业务ID（如进货单ID、销售单ID）',
  `created_by` int NULL DEFAULT NULL COMMENT '创建人ID，外键关联user表',
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  PRIMARY KEY (`flow_id`) USING BTREE,
  INDEX `created_by`(`created_by` ASC) USING BTREE,
  INDEX `idx_flow_date`(`flow_date` ASC) USING BTREE COMMENT '流水日期索引',
  CONSTRAINT `capital_flow_ibfk_1` FOREIGN KEY (`created_by`) REFERENCES `user` (`user_id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '资金流水记录表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of capital_flow
-- ----------------------------

-- ----------------------------
-- Table structure for category
-- ----------------------------
DROP TABLE IF EXISTS `category`;
CREATE TABLE `category`  (
  `category_id` int NOT NULL AUTO_INCREMENT COMMENT '分类ID，主键，自增长',
  `category_name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '分类名称',
  `description` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL COMMENT '分类描述',
  `created_by` int NULL DEFAULT NULL COMMENT '创建人ID，外键关联user表',
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  PRIMARY KEY (`category_id`) USING BTREE,
  INDEX `created_by`(`created_by` ASC) USING BTREE,
  CONSTRAINT `category_ibfk_1` FOREIGN KEY (`created_by`) REFERENCES `user` (`user_id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 5 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '家具分类表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of category
-- ----------------------------
INSERT INTO `category` VALUES (1, '沙发', '各种款式沙发', 1, '2025-10-22 08:27:40');
INSERT INTO `category` VALUES (2, '床', '床具及床垫', 1, '2025-10-22 08:27:40');
INSERT INTO `category` VALUES (3, '餐桌', '餐桌椅系列', 1, '2025-10-22 08:27:40');
INSERT INTO `category` VALUES (4, '柜子', '衣柜、书柜、储物柜', 1, '2025-10-22 08:27:40');

-- ----------------------------
-- Table structure for furniture
-- ----------------------------
DROP TABLE IF EXISTS `furniture`;
CREATE TABLE `furniture`  (
  `furniture_id` int NOT NULL AUTO_INCREMENT COMMENT '家具ID，主键，自增长',
  `furniture_name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '家具名称',
  `category_id` int NULL DEFAULT NULL COMMENT '分类ID，外键关联category表',
  `model` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '型号',
  `material` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '材质',
  `color` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '颜色',
  `purchase_price` decimal(10, 2) NOT NULL COMMENT '进货价格',
  `sale_price` decimal(10, 2) NOT NULL COMMENT '销售价格',
  `description` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL COMMENT '家具描述',
  `created_by` int NULL DEFAULT NULL COMMENT '创建人ID，外键关联user表',
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  PRIMARY KEY (`furniture_id`) USING BTREE,
  INDEX `category_id`(`category_id` ASC) USING BTREE,
  INDEX `created_by`(`created_by` ASC) USING BTREE,
  CONSTRAINT `furniture_ibfk_1` FOREIGN KEY (`category_id`) REFERENCES `category` (`category_id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `furniture_ibfk_2` FOREIGN KEY (`created_by`) REFERENCES `user` (`user_id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 5 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '家具基本信息表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of furniture
-- ----------------------------
INSERT INTO `furniture` VALUES (1, '真皮沙发', 1, 'SF-001', '真皮', '棕色', 2500.00, 3800.00, '111111', 1, '2025-10-22 08:27:40');
INSERT INTO `furniture` VALUES (2, '实木双人床', 2, NULL, NULL, NULL, 0.00, 0.00, '', NULL, '2025-10-22 08:27:40');
INSERT INTO `furniture` VALUES (3, '大理石餐桌', 3, 'TB-001', '大理石+实木', '白色', 1200.00, 2000.00, '111111111', 1, '2025-10-22 08:27:40');
INSERT INTO `furniture` VALUES (4, '三门衣柜', 4, NULL, NULL, NULL, 0.00, 0.00, '', NULL, '2025-10-22 08:27:40');

-- ----------------------------
-- Table structure for inventory
-- ----------------------------
DROP TABLE IF EXISTS `inventory`;
CREATE TABLE `inventory`  (
  `inventory_id` int NOT NULL AUTO_INCREMENT COMMENT '库存记录ID，主键，自增长',
  `furniture_id` int NULL DEFAULT NULL COMMENT '家具ID，外键关联furniture表',
  `warehouse_id` int NULL DEFAULT NULL COMMENT '仓库ID，外键关联warehouse表',
  `quantity` int NOT NULL DEFAULT 0 COMMENT '库存数量',
  `min_stock_level` int NULL DEFAULT 10 COMMENT '最低库存预警线',
  `last_updated` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '最后更新时间',
  PRIMARY KEY (`inventory_id`) USING BTREE,
  UNIQUE INDEX `unique_furniture_warehouse`(`furniture_id` ASC, `warehouse_id` ASC) USING BTREE,
  INDEX `idx_inventory_furniture`(`furniture_id` ASC) USING BTREE COMMENT '库存家具ID索引',
  INDEX `idx_inventory_warehouse`(`warehouse_id` ASC) USING BTREE COMMENT '库存仓库ID索引',
  CONSTRAINT `inventory_ibfk_1` FOREIGN KEY (`furniture_id`) REFERENCES `furniture` (`furniture_id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `inventory_ibfk_2` FOREIGN KEY (`warehouse_id`) REFERENCES `warehouse` (`warehouse_id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 7 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '库存信息表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of inventory
-- ----------------------------
INSERT INTO `inventory` VALUES (1, 1, 1, 20, 10, '2025-12-24 08:59:01');
INSERT INTO `inventory` VALUES (2, 1, 2, 4, 10, '2025-12-24 08:37:05');
INSERT INTO `inventory` VALUES (3, 2, 1, 8, 5, '2025-12-22 14:14:55');
INSERT INTO `inventory` VALUES (4, 3, 1, 12, 10, '2025-12-22 14:14:55');
INSERT INTO `inventory` VALUES (5, 3, 2, 3, 10, '2025-12-22 14:14:55');
INSERT INTO `inventory` VALUES (6, 4, 1, 6, 5, '2025-12-22 14:14:55');

-- ----------------------------
-- Table structure for purchase_detail
-- ----------------------------
DROP TABLE IF EXISTS `purchase_detail`;
CREATE TABLE `purchase_detail`  (
  `detail_id` int NOT NULL AUTO_INCREMENT COMMENT '进货明细ID，主键，自增长',
  `purchase_id` int NULL DEFAULT NULL COMMENT '进货单ID，外键关联purchase_order表',
  `furniture_id` int NULL DEFAULT NULL COMMENT '家具ID，外键关联furniture表',
  `quantity` int NOT NULL COMMENT '进货数量',
  `unit_price` decimal(10, 2) NOT NULL COMMENT '进货单价',
  `total_price` decimal(10, 2) NOT NULL COMMENT '进货总价',
  `warehouse_id` int NULL DEFAULT NULL COMMENT '入库仓库ID，外键关联warehouse表',
  PRIMARY KEY (`detail_id`) USING BTREE,
  INDEX `purchase_id`(`purchase_id` ASC) USING BTREE,
  INDEX `furniture_id`(`furniture_id` ASC) USING BTREE,
  INDEX `warehouse_id`(`warehouse_id` ASC) USING BTREE,
  CONSTRAINT `purchase_detail_ibfk_1` FOREIGN KEY (`purchase_id`) REFERENCES `purchase_order` (`purchase_id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `purchase_detail_ibfk_2` FOREIGN KEY (`furniture_id`) REFERENCES `furniture` (`furniture_id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `purchase_detail_ibfk_3` FOREIGN KEY (`warehouse_id`) REFERENCES `warehouse` (`warehouse_id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '进货明细表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of purchase_detail
-- ----------------------------
INSERT INTO `purchase_detail` VALUES (1, 1, 1, 5, 100.00, 500.00, 1);

-- ----------------------------
-- Table structure for purchase_order
-- ----------------------------
DROP TABLE IF EXISTS `purchase_order`;
CREATE TABLE `purchase_order`  (
  `purchase_id` int NOT NULL AUTO_INCREMENT COMMENT '进货单ID，主键，自增长',
  `supplier_id` int NULL DEFAULT NULL COMMENT '供应商ID，外键关联supplier表',
  `purchase_date` date NOT NULL COMMENT '进货日期',
  `total_amount` decimal(12, 2) NULL DEFAULT NULL COMMENT '进货总金额',
  `status` enum('pending','completed','cancelled') CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT 'pending' COMMENT '进货状态：待处理/已完成/已取消',
  `created_by` int NULL DEFAULT NULL COMMENT '创建人ID，外键关联user表',
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  PRIMARY KEY (`purchase_id`) USING BTREE,
  INDEX `supplier_id`(`supplier_id` ASC) USING BTREE,
  INDEX `created_by`(`created_by` ASC) USING BTREE,
  INDEX `idx_purchase_date`(`purchase_date` ASC) USING BTREE COMMENT '进货日期索引',
  CONSTRAINT `purchase_order_ibfk_1` FOREIGN KEY (`supplier_id`) REFERENCES `supplier` (`supplier_id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `purchase_order_ibfk_2` FOREIGN KEY (`created_by`) REFERENCES `user` (`user_id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '进货单主表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of purchase_order
-- ----------------------------
INSERT INTO `purchase_order` VALUES (1, 1, '2025-12-24', 500.00, 'completed', 1, '2025-12-24 08:58:36');

-- ----------------------------
-- Table structure for sale_detail
-- ----------------------------
DROP TABLE IF EXISTS `sale_detail`;
CREATE TABLE `sale_detail`  (
  `detail_id` int NOT NULL AUTO_INCREMENT COMMENT '销售明细ID，主键，自增长',
  `sale_id` int NULL DEFAULT NULL COMMENT '销售单ID，外键关联sale_order表',
  `furniture_id` int NULL DEFAULT NULL COMMENT '家具ID，外键关联furniture表',
  `quantity` int NOT NULL COMMENT '销售数量',
  `unit_price` decimal(10, 2) NOT NULL COMMENT '销售单价',
  `total_price` decimal(10, 2) NOT NULL COMMENT '销售总价',
  `warehouse_id` int NULL DEFAULT NULL COMMENT '出库仓库ID，外键关联warehouse表',
  PRIMARY KEY (`detail_id`) USING BTREE,
  INDEX `sale_id`(`sale_id` ASC) USING BTREE,
  INDEX `furniture_id`(`furniture_id` ASC) USING BTREE,
  INDEX `warehouse_id`(`warehouse_id` ASC) USING BTREE,
  CONSTRAINT `sale_detail_ibfk_1` FOREIGN KEY (`sale_id`) REFERENCES `sale_order` (`sale_id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `sale_detail_ibfk_2` FOREIGN KEY (`furniture_id`) REFERENCES `furniture` (`furniture_id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `sale_detail_ibfk_3` FOREIGN KEY (`warehouse_id`) REFERENCES `warehouse` (`warehouse_id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 9 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '销售明细表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sale_detail
-- ----------------------------
INSERT INTO `sale_detail` VALUES (1, 1, 1, 1, 3800.00, 3800.00, 1);
INSERT INTO `sale_detail` VALUES (2, 1, 3, 2, 2000.00, 4000.00, 1);
INSERT INTO `sale_detail` VALUES (3, 2, 1, 1, 3800.00, 3800.00, 1);
INSERT INTO `sale_detail` VALUES (5, 4, 1, 1, 3800.00, 3800.00, 1);
INSERT INTO `sale_detail` VALUES (6, 4, 3, 1, 2000.00, 2000.00, 2);
INSERT INTO `sale_detail` VALUES (7, 5, 1, 4, 3800.00, 15200.00, 1);
INSERT INTO `sale_detail` VALUES (8, 6, 1, 1, 3800.00, 3800.00, 2);

-- ----------------------------
-- Table structure for sale_order
-- ----------------------------
DROP TABLE IF EXISTS `sale_order`;
CREATE TABLE `sale_order`  (
  `sale_id` int NOT NULL AUTO_INCREMENT COMMENT '销售单ID，主键，自增长',
  `customer_name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '客户姓名',
  `customer_phone` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '客户电话',
  `sale_date` date NOT NULL COMMENT '销售日期',
  `total_amount` decimal(12, 2) NULL DEFAULT NULL COMMENT '销售总金额',
  `discount` decimal(10, 2) NULL DEFAULT 0.00 COMMENT '折扣金额',
  `final_amount` decimal(12, 2) NULL DEFAULT NULL COMMENT '最终金额（总金额-折扣）',
  `status` enum('pending','completed','cancelled') CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT 'pending' COMMENT '销售状态：待处理/已完成/已取消',
  `created_by` int NULL DEFAULT NULL COMMENT '创建人ID，外键关联user表',
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  PRIMARY KEY (`sale_id`) USING BTREE,
  INDEX `created_by`(`created_by` ASC) USING BTREE,
  INDEX `idx_sale_date`(`sale_date` ASC) USING BTREE COMMENT '销售日期索引',
  CONSTRAINT `sale_order_ibfk_1` FOREIGN KEY (`created_by`) REFERENCES `user` (`user_id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 7 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '销售单主表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sale_order
-- ----------------------------
INSERT INTO `sale_order` VALUES (1, '张三', '13812345678', '2025-12-13', 7600.00, 200.00, 7400.00, 'completed', 3, '2025-12-22 14:14:55');
INSERT INTO `sale_order` VALUES (2, '李四', '13987654321', '2025-12-22', 3800.00, 0.00, 3800.00, 'completed', NULL, '2025-12-23 10:41:09');
INSERT INTO `sale_order` VALUES (4, 'q', '1111', '2025-12-23', 5800.00, 0.00, 5800.00, 'cancelled', NULL, '2025-12-23 11:05:38');
INSERT INTO `sale_order` VALUES (5, '1', '1', '2025-12-24', 15200.00, 1000.00, 14200.00, 'completed', NULL, '2025-12-24 08:13:38');
INSERT INTO `sale_order` VALUES (6, '2', '2', '2025-12-24', 3800.00, 0.00, 3800.00, 'completed', 1, '2025-12-24 08:23:29');

-- ----------------------------
-- Table structure for supplier
-- ----------------------------
DROP TABLE IF EXISTS `supplier`;
CREATE TABLE `supplier`  (
  `supplier_id` int NOT NULL AUTO_INCREMENT COMMENT '供应商ID，主键，自增长',
  `supplier_name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '供应商名称',
  `contact_person` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '联系人姓名',
  `phone` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '联系电话',
  `address` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL COMMENT '供应商地址',
  `created_by` int NULL DEFAULT NULL COMMENT '创建人ID，外键关联user表',
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  PRIMARY KEY (`supplier_id`) USING BTREE,
  INDEX `created_by`(`created_by` ASC) USING BTREE,
  CONSTRAINT `supplier_ibfk_1` FOREIGN KEY (`created_by`) REFERENCES `user` (`user_id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '供应商信息表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of supplier
-- ----------------------------
INSERT INTO `supplier` VALUES (1, '华东家具厂', '张经理', '13800138000', '浙江省杭州市余杭区', 1, '2025-10-22 08:27:40');
INSERT INTO `supplier` VALUES (2, '南方木业', '李厂长', '13900139000', '广东省佛山市顺德区', 1, '2025-10-22 08:27:40');
INSERT INTO `supplier` VALUES (3, '北方沙发厂', '王总', '13700137000', '河北省廊坊市', 1, '2025-10-22 08:27:40');

-- ----------------------------
-- Table structure for user
-- ----------------------------
DROP TABLE IF EXISTS `user`;
CREATE TABLE `user`  (
  `user_id` int NOT NULL AUTO_INCREMENT COMMENT '用户ID，主键，自增长',
  `username` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '用户名，唯一',
  `password` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '密码（加密存储）',
  `real_name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '真实姓名',
  `role` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '用户角色（如：admin, manager, staff等）',
  `phone` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '联系电话',
  `email` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '邮箱',
  `status` enum('active','inactive') CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT 'active' COMMENT '用户状态：激活/未激活',
  `last_login` timestamp NULL DEFAULT NULL COMMENT '最后登录时间',
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `updated_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  PRIMARY KEY (`user_id`) USING BTREE,
  UNIQUE INDEX `username`(`username` ASC) USING BTREE,
  INDEX `idx_username`(`username` ASC) USING BTREE COMMENT '用户名索引',
  INDEX `idx_user_role`(`role` ASC) USING BTREE COMMENT '用户角色索引'
) ENGINE = InnoDB AUTO_INCREMENT = 5 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '用户信息表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of user
-- ----------------------------
INSERT INTO `user` VALUES (1, 'admin', 'e10adc3949ba59abbe56e057f20f883e', '系统管理员', 'admin', '13800138001', 'admin@furniture.com', 'active', '2025-12-24 08:57:45', '2025-10-22 08:27:40', '2025-12-03 08:45:26');
INSERT INTO `user` VALUES (2, 'manager', 'e10adc3949ba59abbe56e057f20f883e', '张经理', 'manager', '13800138002', 'manager@furniture.com', 'active', '2025-12-03 08:42:31', '2025-10-22 08:27:40', '2025-12-03 08:42:30');
INSERT INTO `user` VALUES (3, 'staff1', 'e10adc3949ba59abbe56e057f20f883e', '李销售', 'sales', '13800138003', 'sales1@furniture.com', 'active', NULL, '2025-10-22 08:27:40', '2025-11-26 09:26:06');
INSERT INTO `user` VALUES (4, 'staff2', 'e10adc3949ba59abbe56e057f20f883e', '王采购', 'purchase', '13800138004', 'purchase@furniture.com', 'active', NULL, '2025-10-22 08:27:40', '2025-11-26 09:26:10');

-- ----------------------------
-- Table structure for warehouse
-- ----------------------------
DROP TABLE IF EXISTS `warehouse`;
CREATE TABLE `warehouse`  (
  `warehouse_id` int NOT NULL AUTO_INCREMENT COMMENT '仓库ID，主键，自增长',
  `warehouse_name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '仓库名称',
  `location` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '仓库位置',
  `capacity` int NULL DEFAULT NULL COMMENT '仓库容量',
  `manager` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '仓库管理员',
  `created_by` int NULL DEFAULT NULL COMMENT '创建人ID，外键关联user表',
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  PRIMARY KEY (`warehouse_id`) USING BTREE,
  INDEX `created_by`(`created_by` ASC) USING BTREE,
  CONSTRAINT `warehouse_ibfk_1` FOREIGN KEY (`created_by`) REFERENCES `user` (`user_id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 3 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '仓库信息表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of warehouse
-- ----------------------------
INSERT INTO `warehouse` VALUES (1, '主仓库', '商场B1层', 5000, '张主管', 1, '2025-10-22 08:27:40');
INSERT INTO `warehouse` VALUES (2, '临时仓库', '商场2楼', 2000, '李助理', 1, '2025-10-22 08:27:40');

-- ----------------------------
-- View structure for v_inventory_summary
-- ----------------------------
DROP VIEW IF EXISTS `v_inventory_summary`;
CREATE ALGORITHM = UNDEFINED SQL SECURITY DEFINER VIEW `v_inventory_summary` AS select `f`.`furniture_id` AS `furniture_id`,`f`.`furniture_name` AS `furniture_name`,`c`.`category_name` AS `category_name`,`w`.`warehouse_name` AS `warehouse_name`,`i`.`quantity` AS `quantity`,`f`.`purchase_price` AS `purchase_price`,`f`.`sale_price` AS `sale_price`,(`i`.`quantity` * `f`.`purchase_price`) AS `inventory_cost`,(`i`.`quantity` * `f`.`sale_price`) AS `inventory_value` from (((`inventory` `i` join `furniture` `f` on((`i`.`furniture_id` = `f`.`furniture_id`))) join `category` `c` on((`f`.`category_id` = `c`.`category_id`))) join `warehouse` `w` on((`i`.`warehouse_id` = `w`.`warehouse_id`)));

-- ----------------------------
-- View structure for v_sales_daily
-- ----------------------------
DROP VIEW IF EXISTS `v_sales_daily`;
CREATE ALGORITHM = UNDEFINED SQL SECURITY DEFINER VIEW `v_sales_daily` AS select cast(`sale_order`.`sale_date` as date) AS `sale_day`,count(0) AS `order_count`,sum(`sale_order`.`final_amount`) AS `total_sales`,sum((`sale_order`.`final_amount` - `sale_order`.`total_amount`)) AS `total_discount` from `sale_order` where (`sale_order`.`status` = 'completed') group by cast(`sale_order`.`sale_date` as date);

-- ----------------------------
-- View structure for v_user_operations
-- ----------------------------
DROP VIEW IF EXISTS `v_user_operations`;
CREATE ALGORITHM = UNDEFINED SQL SECURITY DEFINER VIEW `v_user_operations` AS select `u`.`user_id` AS `user_id`,`u`.`username` AS `username`,`u`.`real_name` AS `real_name`,`u`.`role` AS `role`,count(distinct `po`.`purchase_id`) AS `purchase_orders`,count(distinct `so`.`sale_id`) AS `sale_orders`,`u`.`last_login` AS `last_login` from ((`user` `u` left join `purchase_order` `po` on((`u`.`user_id` = `po`.`created_by`))) left join `sale_order` `so` on((`u`.`user_id` = `so`.`created_by`))) group by `u`.`user_id`,`u`.`username`,`u`.`real_name`,`u`.`role`,`u`.`last_login`;

SET FOREIGN_KEY_CHECKS = 1;
