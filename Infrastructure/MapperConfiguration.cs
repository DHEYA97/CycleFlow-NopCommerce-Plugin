﻿using AutoMapper;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Misc.CycleFlow.Models.OrderStatus;
using Nop.Plugin.Misc.CycleFlow.Models.ImageType;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Plugin.Misc.CycleFlow.Models.CycleFlowSetting;

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


            CreateMap<CycleFlowSettingModel, OrderStatusSorting>()
                .ForMember(m => m.OrderStatusId, opt => opt.MapFrom(o => o.CurrentOrderStatusId))
                .ForMember(m => m.WareHouseId, opt => opt.MapFrom(o => o.NopWarehouseId))
                .ForMember(m => m.NopStoreId, opt => opt.MapFrom(o => o.StoreId))
                .ForMember(m => m.NextStep, opt => opt.MapFrom(o => o.NextOrderStatusId))
                .ReverseMap();
        }
        #endregion
    }
}
