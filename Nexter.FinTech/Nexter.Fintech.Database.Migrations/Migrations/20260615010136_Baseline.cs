using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Nexter.Fintech.Database.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class Baseline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 基线迁移：空操作。
            // 该迁移仅用于在 __EFMigrationsHistory 中标记"现有数据库结构"已被记录，
            // 对应线上数据库已有的 Accounts/Categories/Groups/Members/TimedReminders/Transactions 等表。
            // 实际表结构在生产库中已存在，此处不做任何 DDL 操作。
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 基线迁移的 Down 为空操作（不可回滚现有库结构）。
        }
    }
}
