using AutoMapper;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Misc.CycleFlow.Models.OrderStatus;
using Nop.Plugin.Misc.CycleFlow.Models.ImageType;
using Nop.Plugin.Misc.CycleFlow.Domain;

namespace Nop.Plugin.Misc.CycleFlow.Infrastructure
{
    public class MapperConfiguration : Profile, IOrderedMapperProfile
    {
        #region Properties

        public int Order => 1;

        #endregion

        #region Ctor
        public MapperConfiguration()
        {
            CreateMap<OrderStatusModel, OrderStatus>()
                .ReverseMap();
            CreateMap<ImageTypeModel, ImageType>()
                .ReverseMap();
        }
        #endregion
    }
}
