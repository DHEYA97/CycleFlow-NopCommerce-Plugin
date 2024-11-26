using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.CycleFlow.Domain;

namespace Nop.Plugin.Misc.CycleFlow.Mapping.Builders
{
    public class OrderStatusPermissionMappingBuilder : NopEntityBuilder<OrderStatusPermissionMapping>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(OrderStatusPermissionMapping.NopStoreId)).AsInt32().ForeignKey<Store>()
                .WithColumn(nameof(OrderStatusPermissionMapping.OrderStatusId)).AsInt32().ForeignKey<OrderStatus>()
                .WithColumn(nameof(OrderStatusPermissionMapping.CustomerId)).AsInt32().ForeignKey<Customer>()
                .WithColumn(nameof(OrderStatusPermissionMapping.PosUserId)).AsInt32();
        }
    }
}
