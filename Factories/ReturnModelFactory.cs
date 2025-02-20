using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Data;
using Nop.Plugin.Misc.CycleFlow.Constant;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Plugin.Misc.CycleFlow.Models.CycleFlowSetting;
using Nop.Plugin.Misc.CycleFlow.Models.Deportation;
using Nop.Plugin.Misc.CycleFlow.Models.Return;
using Nop.Plugin.Misc.CycleFlow.Services;
using Nop.Plugin.Misc.POSSystem.Services;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Models.Extensions;
using static Nop.Plugin.Misc.CycleFlow.Models.Return.ReturnModel;

namespace Nop.Plugin.Misc.CycleFlow.Factories
{
    public class ReturnModelFactory : IReturnModelFactory
    {
        #region Felid
        private readonly IBaseCycleFlowModelFactory _baseCycleFlowModelFactory;
        private readonly ICycleFlowSettingService _cycleFlowSettingService;
        private readonly ICustomerService _customerService;
        private readonly IDeportationService _deportationService;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderStatusService _orderStatusService;
        private readonly IOrderStateOrderMappingService _orderStateOrderMappingService;
        private readonly IPosUserService _posUserService;
        private readonly IStoreService _storeService;
        private readonly IPictureService _pictureService;
        
        #endregion
        #region Ctor
        public ReturnModelFactory(
            IBaseCycleFlowModelFactory baseCycleFlowModelFactory,
            ICycleFlowSettingService cycleFlowSettingService,
            ICustomerService customerService,
            IDeportationService deportationService,
            ILocalizationService localizationService,
            IOrderStatusService orderStatusService,
            IOrderStateOrderMappingService orderStateOrderMappingService,
            IPosUserService posUserService,
            IStoreService storeService,
            IPictureService pictureService
            )
        {
            _baseCycleFlowModelFactory = baseCycleFlowModelFactory;
            _cycleFlowSettingService = cycleFlowSettingService;
            _customerService = customerService;
            _deportationService = deportationService;
            _localizationService = localizationService;
            _orderStatusService = orderStatusService;
            _orderStateOrderMappingService = orderStateOrderMappingService;
            _posUserService = posUserService;
            _storeService = storeService;
            _pictureService = pictureService;
        }
        #endregion
        #region Methods
        public async Task<ReturnSearchModel> PrepareReturnSearchModelAsync(ReturnSearchModel searchModel)
        {
            searchModel ??= new ReturnSearchModel();

            await  _baseCycleFlowModelFactory.PrepareCustomerListAsync(searchModel.AvailableCustomers);
            await PrepareYearListAsync(searchModel.AvailableYears);
            await PrepareMonthListAsync(searchModel.AvailableMonths);
            
            searchModel.SearchCustomerIds
                = searchModel.SearchYearIds
                = searchModel.SearchMonthIds
                = new List<int>() { 0 };
            searchModel.SetGridPageSize();

            return searchModel;
        }
        public async Task<ReturnListModel> PrepareReturnModelListModelAsync(ReturnSearchModel searchModel)
        {
            var deportation = await _deportationService.SearchReturnAsync(
                customerIds:searchModel.SearchCustomerIds,
                years:searchModel.SearchYearIds,
                months:searchModel.SearchMonthIds
            );

            return await new ReturnListModel().PrepareToGridAsync(searchModel, deportation, () =>
            {
                return deportation.SelectAwait(async FilterReturn =>
                {
                    return await PrepareReturnModelAsync(null, FilterReturn,true);
                });
            });
        }
        public async Task<ReturnModel> PrepareReturnModelAsync(ReturnModel model, FilterReturnModel filterReturnModel, bool excludeProperties = false)
        {
            if (filterReturnModel != null)
            {
                model ??= filterReturnModel.ToModel<ReturnModel>();
                var customer = await _customerService.GetCustomerByIdAsync(filterReturnModel.CustomerId);

                model.CustomerName = await _customerService.GetCustomerFullNameAsync(customer) ?? string.Empty;

                if (!excludeProperties)
                {
                    var AllReturnList = new List<AllReturnModel>();
                    if (filterReturnModel?.OrderDetail != null && filterReturnModel?.OrderDetail.Count > 0)
                    {
                        foreach(var orderMapp in filterReturnModel.OrderDetail)
                        {
                            var OrderStateOrderMapping = await _orderStateOrderMappingService.GetOrderStateOrderMappingByIdAsync(orderMapp.OrderStateOrderMappingId);
                            var RetuneFromOrderStateOrderMapping = await _orderStateOrderMappingService.GetOrderStateOrderMappingByIdAsync(OrderStateOrderMapping.ReturnOrderStatusId??0)??null;
                            var posUser = await _posUserService.GetPosUserByIdAsync(OrderStateOrderMapping.PosUserId);
                            var store = await _storeService.GetStoreByIdAsync(OrderStateOrderMapping.NopStoreId);
                            var ImgList = await _orderStateOrderMappingService.GetAllOrderStateOrderImageMappingPictureOrderStatusIdAsync(OrderStateOrderMapping.PosUserId, OrderStateOrderMapping.OrderId, OrderStateOrderMapping.OrderStatusId);
                            var CustomerReturnFrom = await _cycleFlowSettingService.GetCustomerByOrderStatusIdAsync(OrderStateOrderMapping.PosUserId, OrderStateOrderMapping.ReturnOrderStatusId ?? 0);
                            var CustomerReturnFromName = CustomerReturnFrom!= null ? await _customerService.GetCustomerFullNameAsync(CustomerReturnFrom) ?? string.Empty : string.Empty;
                            var ImageTypeList = new List<string>();
                            if (ImgList != null && ImgList?.Count > 0)
                            {
                                foreach (var img in ImgList)
                                {
                                    ImageTypeList.Add(
                                        (
                                            await _pictureService.GetPictureUrlAsync(img.PictureId)
                                        )
                                     );
                                }
                            }
                            AllReturnList.Add(
                                new AllReturnModel
                                {
                                    CustomerReturnName = model.CustomerName,
                                    Note = RetuneFromOrderStateOrderMapping?.Note?? string.Empty,
                                    OrderId = OrderStateOrderMapping.OrderId,
                                    OrderStateOrderMappingId = OrderStateOrderMapping.Id,
                                    PosUserName = _customerService.GetCustomerFullNameAsync(_posUserService.GetUserByIdAsync(posUser.Id).Result).Result ?? string.Empty,
                                    PosStoreName = store?.Name ?? string.Empty,
                                    ReturnDate = OrderStateOrderMapping.InsertionDate!.Value,
                                    ReturnStatusName = _orderStatusService.GetOrderStatusNameAsync(OrderStateOrderMapping.OrderStatusId).Result,
                                    ReturnFromStatusName = _orderStatusService.GetOrderStatusNameAsync(OrderStateOrderMapping.ReturnOrderStatusId??0).Result,
                                    CustomerReturnFromName = CustomerReturnFromName,
                                    ImageType = ImageTypeList
                                }
                            );
                        }
                        model.AllReturn = AllReturnList;
                    }
                }
            }
            return model;
        }
        #endregion
        #region Utilite
        protected async Task PrepareYearListAsync(IList<SelectListItem> items)
        {
            var orderStateOrderMappingList = await _orderStateOrderMappingService.GeAllOrderStateOrderMappingAsync();
            var yearList =  orderStateOrderMappingList.GroupBy(x => x.InsertionDate.Value.Year);
            foreach (var year in yearList)
            {
                items.Add(new SelectListItem { Value = year.Key.ToString(), Text = year.Key.ToString() });
            }
            items.Insert(0, new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Plugin.Misc.CycleFlow.Common.Select"), Value = "0" });
        }
        protected async Task PrepareMonthListAsync(IList<SelectListItem> items)
        {
            var orderStateOrderMappingList = await _orderStateOrderMappingService.GeAllOrderStateOrderMappingAsync();
            var monthList = orderStateOrderMappingList.GroupBy(x => x.InsertionDate.Value.Month);
            foreach (var month in monthList)
            {
                items.Add(new SelectListItem { Value = month.Key.ToString(), Text = month.Key.ToString() });
            }
            items.Insert(0, new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Plugin.Misc.CycleFlow.Common.Select"), Value = "0" });
        }
        #endregion
    }
}
