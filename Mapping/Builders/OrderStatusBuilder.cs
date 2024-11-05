using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.CycleFlow.Domain;

namespace Nop.Plugin.Misc.CycleFlow.Mapping.Builders
{
    public class OrderStatusBuilder : NopEntityBuilder<OrderStatus>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(OrderStatus.Name)).AsString(100).NotNullable()
                .WithColumn(nameof(OrderStatus.Description)).AsString(500).Nullable()
                .WithColumn(nameof(OrderStatus.IsActive)).AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn(nameof(OrderStatus.Deleted)).AsBoolean().Nullable();
        }
    }
}
