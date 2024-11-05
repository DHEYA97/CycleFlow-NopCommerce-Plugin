using Nop.Data.Mapping;
using Nop.Plugin.Misc.CycleFlow.Domain;
namespace Nop.Plugin.Misc.CycleFlow.Mapping
{
    public class NameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>()
        {
            {typeof(OrderStatus), $"CF_{nameof(OrderStatus)}" },
            {typeof(ImageType), $"CF_{nameof(ImageType)}" },
            {typeof(OrderStatusSorting), $"CF_{nameof(OrderStatusSorting)}" },
            {typeof(OrderStateOrderMapping), $"CF_{nameof(OrderStateOrderMapping)}" },
            {typeof(OrderStatusPermissionMapping), $"CF_{nameof(OrderStatusPermissionMapping)}" },
            {typeof(OrderStatusImageTypeMapping), $"CF_{nameof(OrderStatusImageTypeMapping)}" },
            {typeof(OrderStateOrderImageMapping), $"CF_{nameof(OrderStateOrderImageMapping)}" },
        };

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string>();
    }
}
