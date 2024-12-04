using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Plugin.Misc.POSSystem.Domains;

namespace Nop.Plugin.Misc.CycleFlow.Mapping.Builders
{
    public class OrderStatusImageTypeMappingBuilder : NopEntityBuilder<OrderStatusImageTypeMapping>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(OrderStatusImageTypeMapping.NopStoreId)).AsInt32().ForeignKey<Store>()
                .WithColumn(nameof(OrderStatusImageTypeMapping.OrderStatusId)).AsInt32().ForeignKey<OrderStatus>()
                .WithColumn(nameof(OrderStatusImageTypeMapping.PosUserId)).AsInt32().ForeignKey<PosUser>()
                .WithColumn(nameof(OrderStatusImageTypeMapping.ImageTypeId)).AsInt32().ForeignKey<ImageType>()

                .WithColumn(nameof(OrderStatusImageTypeMapping.InsertedByUser)).AsInt32().Nullable()
                .WithColumn(nameof(OrderStatusImageTypeMapping.InsertionDate)).AsDateTime().Nullable()
                .WithColumn(nameof(OrderStatusImageTypeMapping.UpdatedByUser)).AsInt32().Nullable()
                .WithColumn(nameof(OrderStatusImageTypeMapping.UpdatingDate)).AsDateTime().Nullable();
        }
    }
}
