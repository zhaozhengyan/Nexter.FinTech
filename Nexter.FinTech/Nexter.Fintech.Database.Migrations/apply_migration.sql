-- ============================================================================
-- Nexter.FinTech 增量迁移脚本（MySQL 兼容，幂等可重复执行）
-- 对应迁移：
--   20260615010136_Baseline        (no-op 基线，标记现有库结构已存在)
--   20260615010253_AddItemTables   (Members 加 DefaultTab 列 + 建 Items/ItemCategories 表)
--
-- 用法：在目标库直接整段执行即可，已应用的部分会自动跳过。
-- ============================================================================

-- 1. 确保 __EFMigrationsHistory 表存在
CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 2. 记录 Baseline（no-op，仅写 history，保证迁移项目与数据库对齐）
INSERT IGNORE INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260615010136_Baseline', '10.0.7');

-- 3. AddItemTables：Members 表加 DefaultTab 列（已存在则跳过）
SET @col_exists := (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME = 'Members'
      AND COLUMN_NAME = 'DefaultTab'
);
SET @sql := IF(@col_exists = 0,
    'ALTER TABLE `Members` ADD COLUMN `DefaultTab` varchar(16) NULL',
    'SELECT "Members.DefaultTab already exists, skipped." AS msg');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- 4. AddItemTables：建 ItemCategories 表（已存在则跳过）
SET @tbl_exists := (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ItemCategories'
);
SET @sql := IF(@tbl_exists = 0,
    'CREATE TABLE `ItemCategories` (
        `Id` bigint NOT NULL AUTO_INCREMENT,
        `Name` varchar(32) NULL,
        `Icon` varchar(32) NULL,
        `MemberId` bigint NULL,
        PRIMARY KEY (`Id`)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4',
    'SELECT "ItemCategories already exists, skipped." AS msg');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- 5. AddItemTables：建 Items 表（已存在则跳过）
SET @tbl_exists := (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Items'
);
SET @sql := IF(@tbl_exists = 0,
    'CREATE TABLE `Items` (
        `Id` bigint NOT NULL AUTO_INCREMENT,
        `Name` varchar(64) NULL,
        `Price` decimal(18,2) NOT NULL,
        `AdditionalCost` decimal(18,2) NOT NULL,
        `PurchaseDate` datetime(6) NOT NULL,
        `RetireDate` datetime(6) NULL,
        `WarrantyEndDate` datetime(6) NULL,
        `MemberId` bigint NOT NULL,
        `CategoryId` bigint NULL,
        `Icon` varchar(32) NULL,
        `PriceCalcMethod` varchar(16) NULL,
        `Note` varchar(256) NULL,
        `CreatedAt` datetime(6) NOT NULL,
        `LastModifiedAt` datetime(6) NOT NULL,
        PRIMARY KEY (`Id`)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4',
    'SELECT "Items already exists, skipped." AS msg');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- 6. 记录 AddItemTables（已存在则跳过）
INSERT IGNORE INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260615010253_AddItemTables', '10.0.7');

-- 7.（可选推荐）给老用户补 DefaultTab 默认值
UPDATE `Members` SET `DefaultTab` = 'index' WHERE `DefaultTab` IS NULL;

SELECT 'Migration applied successfully.' AS result;
