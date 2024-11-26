using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Plugin.Misc.POSSystem.Domains;

namespace Nop.Plugin.Misc.CycleFlow.Mapping.Builders
{
    public class OrderStatusSortingBuilder : NopEntityBuilder<OrderStatusSorting>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(OrderStatusSorting.NopStoreId)).AsInt32().ForeignKey<Store>()
                .WithColumn(nameof(OrderStatusSorting.OrderStatusId)).AsInt32().ForeignKey<OrderStatus>()
                .WithColumn(nameof(OrderStatusSorting.PosUserId)).AsInt32().ForeignKey<PosUser>()
                .WithColumn(nameof(OrderStatusSorting.NextStep)).AsInt32()
                .WithColumn(nameof(OrderStatusSorting.IsFirstStep)).AsBoolean().Nullable()
                .WithColumn(nameof(OrderStatusSorting.IsLastStep)).AsBoolean().Nullable();
        }
    }
}
