using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FinTech.Domain;
using Furion.DatabaseAccessor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MySql.EntityFrameworkCore;

namespace Nexter.Fintech.Database.Migrations;

/// <summary>
/// 设计时 DbContext 工厂，供 `dotnet ef migrations` 命令使用。
/// 绕开 Furion 运行时（AppDbContext 依赖 Furion 容器初始化），
/// 直接用原生 DbContext + 反射注册所有 IEntity 实体。
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DesignTimeDbContext>
{
    public DesignTimeDbContext CreateDbContext(string[] args)
    {
        // 设计时使用占位连接字符串，仅用于生成迁移 C# 代码，不真实连接数据库
        var connectionString = "server=localhost;userid=root;password=123456;database='FinTech'";

        var optionsBuilder = new DbContextOptionsBuilder<DesignTimeDbContext>()
            .UseMySQL(connectionString);

        return new DesignTimeDbContext(optionsBuilder.Options);
    }
}

/// <summary>
/// 设计时专用 DbContext，不继承 Furion 的 AppDbContext，
/// 通过反射注册 FinTech.Domain 中所有实现 IEntity 的实体。
/// 注意：表名/列名约定与 DefaultDbContext 保持一致（依赖 [Table]/[Column] 特性）。
/// </summary>
public class DesignTimeDbContext : DbContext
{
    public DesignTimeDbContext(DbContextOptions<DesignTimeDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 扫描 FinTech.Domain 程序集中所有实现 IEntity 接口的非抽象类型并注册。
        // 注意：若后续需要为"已存在的生产库"生成基线迁移，可临时在此排除新增实体，
        // 生成空 Up 的 Baseline 后再恢复，后续迁移即自动 diff 出增量。
        var entityAssembly = typeof(BaseEntity).Assembly;
        var iEntityType = typeof(IEntity);

        var entityTypes = entityAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && iEntityType.IsAssignableFrom(t));

        foreach (var type in entityTypes)
        {
            // 避免重复注册（已通过 DbSet 或特性注册的跳过）
            if (modelBuilder.Model.FindEntityType(type) != null) continue;
            modelBuilder.Entity(type);
        }

        base.OnModelCreating(modelBuilder);
    }
}
