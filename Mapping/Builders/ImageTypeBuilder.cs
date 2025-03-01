﻿using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.Accounting.Domain;
using Nop.Plugin.Misc.CycleFlow.Domain;

namespace Nop.Plugin.Misc.CycleFlow.Mapping.Builders
{
    public class ImageTypeBuilder : NopEntityBuilder<ImageType>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ImageType.Name)).AsString(100).NotNullable()
                .WithColumn(nameof(ImageType.Deleted)).AsBoolean().Nullable()

                .WithColumn(nameof(ImageType.InsertedByUser)).AsInt32().Nullable()
                .WithColumn(nameof(ImageType.InsertionDate)).AsDateTime().Nullable()
                .WithColumn(nameof(ImageType.UpdatedByUser)).AsInt32().Nullable()
                .WithColumn(nameof(ImageType.UpdatingDate)).AsDateTime().Nullable();
        }
    }
}
