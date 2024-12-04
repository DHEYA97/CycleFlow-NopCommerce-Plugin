using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.CycleFlow.Domain;

namespace Nop.Plugin.Misc.CycleFlow.Mapping.Builders
{
    internal class OrderStateOrderImageMappingBuilder : NopEntityBuilder<OrderStateOrderImageMapping>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                 .WithColumn(nameof(OrderStateOrderImageMapping.NopStoreId)).AsInt32().ForeignKey<Store>()
                 .WithColumn(nameof(OrderStateOrderImageMapping.CustomerId)).AsInt32().ForeignKey<Customer>()
                 .WithColumn(nameof(OrderStateOrderImageMapping.OrderStatusId)).AsInt32().ForeignKey<Domain.OrderStatus>()
                 .WithColumn(nameof(OrderStateOrderImageMapping.PosUserId)).AsInt32()
                 .WithColumn(nameof(OrderStateOrderImageMapping.OrderId)).AsInt32().ForeignKey<Order>()
                 .WithColumn(nameof(OrderStateOrderImageMapping.PictureId)).AsInt32().ForeignKey<Picture>()

                .WithColumn(nameof(OrderStateOrderImageMapping.InsertedByUser)).AsInt32().Nullable()
                .WithColumn(nameof(OrderStateOrderImageMapping.InsertionDate)).AsDateTime().Nullable()
                .WithColumn(nameof(OrderStateOrderImageMapping.UpdatedByUser)).AsInt32().Nullable()
                .WithColumn(nameof(OrderStateOrderImageMapping.UpdatingDate)).AsDateTime().Nullable();
        }
    }
}
