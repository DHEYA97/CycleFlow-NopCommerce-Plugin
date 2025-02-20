using Nop.Data.Migrations;
using FluentMigrator;
using Nop.Core;
using Nop.Data.Mapping;
using Nop.Plugin.Misc.POSSystem.Domains;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Data.Extensions;

namespace Nop.Plugin.Misc.CycleFlow.Migrations
{
    [NopMigration("2024-11-29 00:00:00", "CycleFlowPlugin Update Base Info Table", MigrationProcessType.Update)]
    public class UpdateBaseInfoToTableMigration : Migration
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
            if (!Schema.Table(TableName<OrderStateOrderImageMapping>()).Column(nameof(OrderStateOrderImageMapping.InsertedByUser)).Exists())
            {
                Alter.Table(TableName<OrderStateOrderImageMapping>())
                    .AddColumn(nameof(OrderStateOrderImageMapping.InsertedByUser)).AsInt32().Nullable();
            }
            if (!Schema.Table(TableName<OrderStateOrderImageMapping>()).Column(nameof(OrderStateOrderImageMapping.InsertionDate)).Exists())
            {
                Alter.Table(TableName<OrderStateOrderImageMapping>())
                    .AddColumn(nameof(OrderStateOrderImageMapping.InsertionDate)).AsDateTime().Nullable();
            }
            if (!Schema.Table(TableName<OrderStateOrderImageMapping>()).Column(nameof(OrderStateOrderImageMapping.UpdatedByUser)).Exists())
            {
                Alter.Table(TableName<OrderStateOrderImageMapping>())
                    .AddColumn(nameof(OrderStateOrderImageMapping.UpdatedByUser)).AsInt32().Nullable();
            }
            if (!Schema.Table(TableName<OrderStateOrderImageMapping>()).Column(nameof(OrderStateOrderImageMapping.UpdatingDate)).Exists())
            {
                Alter.Table(TableName<OrderStateOrderImageMapping>())
                    .AddColumn(nameof(OrderStateOrderImageMapping.UpdatingDate)).AsDateTime().Nullable();
            }

            if (!Schema.Table(TableName<OrderStateOrderMapping>()).Column(nameof(OrderStateOrderMapping.InsertedByUser)).Exists())
            {
                Alter.Table(TableName<OrderStateOrderMapping>())
                    .AddColumn(nameof(OrderStateOrderMapping.InsertedByUser)).AsInt32().Nullable();
            }
            if (!Schema.Table(TableName<OrderStateOrderMapping>()).Column(nameof(OrderStateOrderMapping.InsertionDate)).Exists())
            {
                Alter.Table(TableName<OrderStateOrderMapping>())
                    .AddColumn(nameof(OrderStateOrderMapping.InsertionDate)).AsDateTime().Nullable();
            }
            if (!Schema.Table(TableName<OrderStateOrderMapping>()).Column(nameof(OrderStateOrderMapping.UpdatedByUser)).Exists())
            {
                Alter.Table(TableName<OrderStateOrderMapping>())
                    .AddColumn(nameof(OrderStateOrderMapping.UpdatedByUser)).AsInt32().Nullable();
            }
            if (!Schema.Table(TableName<OrderStateOrderMapping>()).Column(nameof(OrderStateOrderMapping.UpdatingDate)).Exists())
            {
                Alter.Table(TableName<OrderStateOrderMapping>())
                    .AddColumn(nameof(OrderStateOrderMapping.UpdatingDate)).AsDateTime().Nullable();
            }

            if (!Schema.Table(TableName<OrderStatus>()).Column(nameof(OrderStatus.InsertedByUser)).Exists())
            {
                Alter.Table(TableName<OrderStatus>())
                    .AddColumn(nameof(OrderStatus.InsertedByUser)).AsInt32().Nullable();
            }
            if (!Schema.Table(TableName<OrderStatus>()).Column(nameof(OrderStatus.InsertionDate)).Exists())
            {
                Alter.Table(TableName<OrderStatus>())
                    .AddColumn(nameof(OrderStatus.InsertionDate)).AsDateTime().Nullable();
            }
            if (!Schema.Table(TableName<OrderStatus>()).Column(nameof(OrderStatus.UpdatedByUser)).Exists())
            {
                Alter.Table(TableName<OrderStatus>())
                    .AddColumn(nameof(OrderStatus.UpdatedByUser)).AsInt32().Nullable();
            }
            if (!Schema.Table(TableName<OrderStatus>()).Column(nameof(OrderStatus.UpdatingDate)).Exists())
            {
                Alter.Table(TableName<OrderStatus>())
                    .AddColumn(nameof(OrderStatus.UpdatingDate)).AsDateTime().Nullable();
            }

            
            if (!Schema.Table(TableName<OrderStatusPermissionMapping>()).Column(nameof(OrderStatusPermissionMapping.InsertedByUser)).Exists())
            {
                Alter.Table(TableName<OrderStatusPermissionMapping>())
                    .AddColumn(nameof(OrderStatusPermissionMapping.InsertedByUser)).AsInt32().Nullable();
            }
            if (!Schema.Table(TableName<OrderStatusPermissionMapping>()).Column(nameof(OrderStatusPermissionMapping.InsertionDate)).Exists())
            {
                Alter.Table(TableName<OrderStatusPermissionMapping>())
                    .AddColumn(nameof(OrderStatusPermissionMapping.InsertionDate)).AsDateTime().Nullable();
            }
            if (!Schema.Table(TableName<OrderStatusPermissionMapping>()).Column(nameof(OrderStatusPermissionMapping.UpdatedByUser)).Exists())
            {
                Alter.Table(TableName<OrderStatusPermissionMapping>())
                    .AddColumn(nameof(OrderStatusPermissionMapping.UpdatedByUser)).AsInt32().Nullable();
            }
            if (!Schema.Table(TableName<OrderStatusPermissionMapping>()).Column(nameof(OrderStatusPermissionMapping.UpdatingDate)).Exists())
            {
                Alter.Table(TableName<OrderStatusPermissionMapping>())
                    .AddColumn(nameof(OrderStatusPermissionMapping.UpdatingDate)).AsDateTime().Nullable();
            }

            if (!Schema.Table(TableName<OrderStatusSorting>()).Column(nameof(OrderStatusSorting.InsertedByUser)).Exists())
            {
                Alter.Table(TableName<OrderStatusSorting>())
                    .AddColumn(nameof(OrderStatusSorting.InsertedByUser)).AsInt32().Nullable();
            }
            if (!Schema.Table(TableName<OrderStatusSorting>()).Column(nameof(OrderStatusSorting.InsertionDate)).Exists())
            {
                Alter.Table(TableName<OrderStatusSorting>())
                    .AddColumn(nameof(OrderStatusSorting.InsertionDate)).AsDateTime().Nullable();
            }
            if (!Schema.Table(TableName<OrderStatusSorting>()).Column(nameof(OrderStatusSorting.UpdatedByUser)).Exists())
            {
                Alter.Table(TableName<OrderStatusSorting>())
                    .AddColumn(nameof(OrderStatusSorting.UpdatedByUser)).AsInt32().Nullable();
            }
            if (!Schema.Table(TableName<OrderStatusSorting>()).Column(nameof(OrderStatusSorting.UpdatingDate)).Exists())
            {
                Alter.Table(TableName<OrderStatusSorting>())
                    .AddColumn(nameof(OrderStatusSorting.UpdatingDate)).AsDateTime().Nullable();
            }

        }
    }
}
