using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Plugin.Misc.POSSystem.Domains;
using Nop.Plugin.Misc.SmsAuthentication.Domains;

namespace Nop.Plugin.Misc.CycleFlow.Mapping.Builders
{
    public class OrderStatusSortingBuilder : NopEntityBuilder<OrderStatusSorting>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(OrderStatusSorting.NopStoreId)).AsInt32().ForeignKey<Store>(onDelete: System.Data.Rule.None)
                .WithColumn(nameof(OrderStatusSorting.OrderStatusId)).AsInt32().ForeignKey<OrderStatus>(onDelete: System.Data.Rule.None)
                .WithColumn(nameof(OrderStatusSorting.PosUserId)).AsInt32().ForeignKey<PosUser>()
                .WithColumn(nameof(OrderStatusSorting.NextStep)).AsInt32().Nullable().ForeignKey<OrderStatus>(onDelete: System.Data.Rule.None)
                .WithColumn(nameof(OrderStatusSorting.IsAddPictureRequired)).AsBoolean().Nullable()
                .WithColumn(nameof(OrderStatusSorting.IsFirstStep)).AsBoolean().Nullable()
                .WithColumn(nameof(OrderStatusSorting.IsLastStep)).AsBoolean().Nullable()
                .WithColumn(nameof(OrderStatusSorting.IsEnableSendToClient)).AsBoolean().Nullable()
                .WithColumn(nameof(OrderStatusSorting.IsEnableSendToUser)).AsBoolean().Nullable()
                .WithColumn(nameof(OrderStatusSorting.ClientSmsTemplateId)).AsInt32().Nullable().ForeignKey<SmsTemplate>(onDelete: System.Data.Rule.None)
                .WithColumn(nameof(OrderStatusSorting.UserSmsTemplateId)).AsInt32().Nullable().ForeignKey<SmsTemplate>(onDelete: System.Data.Rule.None)
                .WithColumn(nameof(OrderStatusSorting.IsEnableReturn)).AsBoolean().Nullable()
                .WithColumn(nameof(OrderStatusSorting.ReturnStepId)).AsInt32().Nullable().ForeignKey<OrderStatus>(onDelete: System.Data.Rule.None)

                .WithColumn(nameof(OrderStatusSorting.InsertedByUser)).AsInt32().Nullable()
                .WithColumn(nameof(OrderStatusSorting.InsertionDate)).AsDateTime().Nullable()
                .WithColumn(nameof(OrderStatusSorting.UpdatedByUser)).AsInt32().Nullable()
                .WithColumn(nameof(OrderStatusSorting.UpdatingDate)).AsDateTime().Nullable();
        }
    }
}
