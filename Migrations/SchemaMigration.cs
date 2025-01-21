using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.CycleFlow.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.CycleFlow.Migrations
{
    [NopMigration("2024-11-02 00:00:00", "CycleFlowPlugin base schema", MigrationProcessType.Installation)]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<OrderStatus>();
            Create.TableFor<OrderStatusSorting>();
            Create.TableFor<OrderStateOrderMapping>();
            Create.TableFor<OrderStatusPermissionMapping>();
            Create.TableFor<OrderStateOrderImageMapping>();
        }
    }
}
