﻿using FluentMigrator.Builders.Create.Table;
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
                 .WithColumn(nameof(OrderStateOrderMapping.WareHouseId)).AsInt32().ForeignKey<Warehouse>()
                 .WithColumn(nameof(OrderStateOrderMapping.OrderStatusId)).AsInt32().ForeignKey<Domain.OrderStatus>()
                 .WithColumn(nameof(OrderStateOrderMapping.PosUserId)).AsInt32()
                 .WithColumn(nameof(OrderStateOrderMapping.OrderId)).AsInt32().ForeignKey<Order>();
        }
    }
}
