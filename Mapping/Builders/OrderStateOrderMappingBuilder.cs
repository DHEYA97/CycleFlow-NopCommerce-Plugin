using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.CycleFlow.Domain;

namespace Nop.Plugin.Misc.CycleFlow.Mapping.Builders
{
    internal class OrderStateOrderMappingBuilder : NopEntityBuilder<OrderStateOrderMapping>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                 .WithColumn(nameof(OrderStateOrderMapping.NopStoreId)).AsInt32().ForeignKey<Store>()
                 .WithColumn(nameof(OrderStateOrderMapping.CustomerId)).AsInt32().ForeignKey<Customer>()
                 .WithColumn(nameof(OrderStateOrderMapping.OrderStatusId)).AsInt32().ForeignKey<Domain.OrderStatus>()
                 .WithColumn(nameof(OrderStateOrderMapping.PosUserId)).AsInt32()
                 .WithColumn(nameof(OrderStateOrderMapping.OrderId)).AsInt32().ForeignKey<Order>()
                 .WithColumn(nameof(OrderStateOrderMapping.Note)).AsString().Nullable()
                 .WithColumn(nameof(OrderStateOrderMapping.IsReturn)).AsBoolean().Nullable()

                 .WithColumn(nameof(OrderStateOrderMapping.InsertedByUser)).AsInt32().Nullable()
                .WithColumn(nameof(OrderStateOrderMapping.InsertionDate)).AsDateTime().Nullable()
                .WithColumn(nameof(OrderStateOrderMapping.UpdatedByUser)).AsInt32().Nullable()
                .WithColumn(nameof(OrderStateOrderMapping.UpdatingDate)).AsDateTime().Nullable();
        }
    }
}
