using Nop.Data.Migrations;
using FluentMigrator;
using Nop.Core;
using Nop.Data.Mapping;
using Nop.Plugin.Misc.POSSystem.Domains;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Data.Extensions;
using Nop.Plugin.Misc.CycleFlow.Mapping.Builders;

namespace Nop.Plugin.Misc.CycleFlow.Migrations
{
    [NopMigration("2025-02-18 12:13:04", "CycleFlowPlugin Update Table", MigrationProcessType.Update)]
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
                    .AddColumn(nameof(OrderStatusSorting.ClientSmsTemplateId)).AsInt32().Nullable();
            }
            if (!Schema.Table(TableName<OrderStatusSorting>()).Column(nameof(OrderStatusSorting.UserSmsTemplateId)).Exists())
            {
                Alter.Table(TableName<OrderStatusSorting>())
                    .AddColumn(nameof(OrderStatusSorting.UserSmsTemplateId)).AsInt32().Nullable();
            }
            
            if (!Schema.Table(TableName<OrderStatusSorting>()).Column(nameof(OrderStatusSorting.IsEnableReturn)).Exists())
            {
                Alter.Table(TableName<OrderStatusSorting>())
                    .AddColumn(nameof(OrderStatusSorting.IsEnableReturn)).AsBoolean().Nullable();
            }
            if (!Schema.Table(TableName<OrderStatusSorting>()).Column(nameof(OrderStatusSorting.ReturnStepId)).Exists())
            {
                Alter.Table(TableName<OrderStatusSorting>())
                    .AddColumn(nameof(OrderStatusSorting.ReturnStepId)).AsInt32().Nullable().ForeignKey<OrderStatus>(onDelete: System.Data.Rule.None);
            }

            if (!Schema.Table(TableName<OrderStateOrderMapping>()).Column(nameof(OrderStateOrderMapping.Note)).Exists())
            {
                Alter.Table(TableName<OrderStateOrderMapping>())
                    .AddColumn(nameof(OrderStateOrderMapping.Note)).AsString().Nullable();
            }

            
            if (!Schema.Table(TableName<OrderStateOrderImageMapping>()).Column(nameof(OrderStateOrderImageMapping.OrderStateOrderMappingId)).Exists())
            {
                Alter.Table(TableName<OrderStateOrderImageMapping>())
                    .AddColumn(nameof(OrderStateOrderImageMapping.OrderStateOrderMappingId)).AsInt32().Nullable().ForeignKey<OrderStateOrderMapping>(onDelete: System.Data.Rule.None);
            }
            if (!Schema.Table(TableName<OrderStateOrderMapping>()).Column(nameof(OrderStateOrderMapping.IsReturn)).Exists())
            {
                Alter.Table(TableName<OrderStateOrderMapping>())
                    .AddColumn(nameof(OrderStateOrderMapping.IsReturn)).AsBoolean().Nullable();
            }
            if (!Schema.Table(TableName<OrderStateOrderMapping>()).Column(nameof(OrderStateOrderMapping.ReturnOrderStatusId)).Exists())
            {
                Alter.Table(TableName<OrderStateOrderMapping>())
                    .AddColumn(nameof(OrderStateOrderMapping.ReturnOrderStatusId)).AsInt32().Nullable().ForeignKey<Domain.OrderStatus>(onDelete: System.Data.Rule.None);
            }


            if (!Schema.Table(TableName<OrderStatusSorting>()).Column(nameof(OrderStatusSorting.IsAddPictureRequired)).Exists())
            {
                Alter.Table(TableName<OrderStatusSorting>())
                    .AddColumn(nameof(OrderStatusSorting.IsAddPictureRequired)).AsBoolean().Nullable();
            }
            if (Schema.Table(TableName<OrderStateOrderImageMapping>()).Index("IX_CF_OrderStateOrderImageMapping_ImageTypeId").Exists())
            {
                Delete.Index("IX_CF_OrderStateOrderImageMapping_ImageTypeId").OnTable(TableName<OrderStateOrderImageMapping>());
            }

            if (Schema.Table(TableName<OrderStateOrderImageMapping>()).Constraint("FK_CF_OrderStateOrderImageMapping_ImageTypeId_CF_ImageType_Id").Exists())
            {
                Delete.ForeignKey("FK_CF_OrderStateOrderImageMapping_ImageTypeId_CF_ImageType_Id").OnTable(TableName<OrderStateOrderImageMapping>());
            }
            if (Schema.Table(TableName<OrderStateOrderImageMapping>()).Column("ImageTypeId").Exists())
            {
                Delete.Column("ImageTypeId").FromTable(TableName<OrderStateOrderImageMapping>());
            }

            if (Schema.Table("ImageType").Exists())
            {
                Delete.Table("ImageType");
            }

            if (Schema.Table("OrderStatusImageTypeMapping").Exists())
            {
                Delete.Table("OrderStatusImageTypeMapping");
            }

            if (Schema.Table(TableName<OrderStatusSorting>()).Constraint("FK_CF_OrderStatusSorting_ClientSmsTemplateId_SmsAuthentication_SmsTemplate_Id").Exists())
            {
                Delete.ForeignKey("FK_CF_OrderStatusSorting_ClientSmsTemplateId_SmsAuthentication_SmsTemplate_Id").OnTable(TableName<OrderStatusSorting>());
            }
            if (Schema.Table(TableName<OrderStatusSorting>()).Constraint("FK_CF_OrderStatusSorting_UserSmsTemplateId_SmsAuthentication_SmsTemplate_Id").Exists())
            {
                Delete.ForeignKey("FK_CF_OrderStatusSorting_UserSmsTemplateId_SmsAuthentication_SmsTemplate_Id").OnTable(TableName<OrderStatusSorting>());
            }

        }
    }
}
