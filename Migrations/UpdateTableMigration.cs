using Nop.Data.Migrations;
using FluentMigrator;
using Nop.Core;
using Nop.Data.Mapping;
using Nop.Plugin.Misc.POSSystem.Domains;
using Nop.Plugin.Misc.CycleFlow.Domain;

namespace Nop.Plugin.Misc.CycleFlow.Migrations
{
    [NopMigration("2024-11-03 00:00:00", "CycleFlowPlugin Update Table", MigrationProcessType.Update)]
    public class UpdateTableMigration : Migration
    {
        public static string TableName<T>() where T : BaseEntity
        {
            return NameCompatibilityManager.GetTableName(typeof(T));
        }
        public override void Down()
        {
            
        }

        public override void Up()
        {
            if (!Schema.Table(TableName<OrderStatusSorting>()).Column(nameof(OrderStatusSorting.IsFirstStep)).Exists())
            {
                Alter.Table(TableName<OrderStatusSorting>())
                    .AddColumn(nameof(OrderStatusSorting.IsFirstStep)).AsBoolean().Nullable();
            }

            if (!Schema.Table(TableName<OrderStatusSorting>()).Column(nameof(OrderStatusSorting.IsLastStep)).Exists())
            {
                Alter.Table(TableName<OrderStatusSorting>())
                    .AddColumn(nameof(OrderStatusSorting.IsLastStep)).AsBoolean().Nullable();
            }
        }
    }
}
