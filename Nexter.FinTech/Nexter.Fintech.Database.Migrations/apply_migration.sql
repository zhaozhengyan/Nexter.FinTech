CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    PRIMARY KEY (`MigrationId`)
);

START TRANSACTION;
INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260615010136_Baseline', '10.0.7');

ALTER TABLE `Members` ADD `DefaultTab` varchar(16) NULL;

CREATE TABLE `ItemCategories` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `Name` varchar(32) NULL,
    `Icon` varchar(32) NULL,
    `MemberId` bigint NULL,
    PRIMARY KEY (`Id`)
);

CREATE TABLE `Items` (
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
);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260615010253_AddItemTables', '10.0.7');

ALTER TABLE `Items` ADD `AdditionalItemsJson` varchar(2048) NULL;

ALTER TABLE `Items` ADD `SortOrder` int NOT NULL DEFAULT 0;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260622000001_AddAdditionalItemsJsonAndSortOrder', '10.0.7');

COMMIT;

