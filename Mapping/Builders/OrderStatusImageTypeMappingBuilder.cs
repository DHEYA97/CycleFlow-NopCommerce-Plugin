using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.CycleFlow.Domain;

namespace Nop.Plugin.Misc.CycleFlow.Mapping.Builders
{
    public class OrderStatusImageTypeMappingBuilder : NopEntityBuilder<OrderStatusImageTypeMapping>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(OrderStatusImageTypeMapping.NopStoreId)).AsInt32().ForeignKey<Store>()
                .WithColumn(nameof(OrderStatusImageTypeMapping.OrderStatusId)).AsInt32().ForeignKey<OrderStatus>()
                .WithColumn(nameof(OrderStatusImageTypeMapping.WareHouseId)).AsInt32().ForeignKey<Warehouse>()
                .WithColumn(nameof(OrderStatusImageTypeMapping.ImageTypeId)).AsInt32().ForeignKey<ImageType>();
        }
    }
}
