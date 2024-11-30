using Nop.Data.Migrations;
using FluentMigrator;
using Nop.Core;
using Nop.Data.Mapping;
using Nop.Plugin.Misc.POSSystem.Domains;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Data.Extensions;
using Nop.Plugin.Misc.SmsAuthentication.Domains;

namespace Nop.Plugin.Misc.CycleFlow.Migrations
{
    [NopMigration("2024-11-06 00:00:00", "CycleFlowPlugin Update Table", MigrationProcessType.Update)]
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
            if (!Schema.Table(TableName<OrderStatusSorting>()).Column(nameof(OrderStatusSorting.IsEnableSendToClient)).Exists())
            {
                Alter.Table(TableName<OrderStatusSorting>())
                    .AddColumn(nameof(OrderStatusSorting.IsEnableSendToClient)).AsBoolean().Nullable();
            }

            if (!Schema.Table(TableName<OrderStatusSorting>()).Column(nameof(OrderStatusSorting.IsEnableSendToUser)).Exists())
            {
                Alter.Table(TableName<OrderStatusSorting>())
                    .AddColumn(nameof(OrderStatusSorting.IsEnableSendToUser)).AsBoolean().Nullable();
            }

            if (!Schema.Table(TableName<OrderStatusSorting>()).Column(nameof(OrderStatusSorting.ClientSmsTemplateId)).Exists())
            {
                Alter.Table(TableName<OrderStatusSorting>())
                    .AddColumn(nameof(OrderStatusSorting.ClientSmsTemplateId)).AsInt32().Nullable().ForeignKey<SmsTemplate>(onDelete: System.Data.Rule.None);
            }

            if (!Schema.Table(TableName<OrderStatusSorting>()).Column(nameof(OrderStatusSorting.UserSmsTemplateId)).Exists())
            {
                Alter.Table(TableName<OrderStatusSorting>())
                    .AddColumn(nameof(OrderStatusSorting.UserSmsTemplateId)).AsInt32().Nullable().ForeignKey<SmsTemplate>(onDelete: System.Data.Rule.None);
            }
        }
    }
}
