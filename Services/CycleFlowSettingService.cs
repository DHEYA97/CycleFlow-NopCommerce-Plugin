using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Data;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Plugin.Misc.CycleFlow.Models.CycleFlowSetting;
using Nop.Plugin.Misc.POSSystem.Domains;
using Nop.Services.Customers;
using Nop.Services.Messages;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using System.Transactions;
namespace Nop.Plugin.Misc.CycleFlow.Services
{
    public class CycleFlowSettingService : ICycleFlowSettingService
    {
        #region Fields
        private readonly IRepository<Domain.OrderStatus> _orderStatusRepository;
        private readonly IRepository<OrderStatusSorting> _orderStatusSortingTypeRepository;
        private readonly IRepository<OrderStatusPermissionMapping> _orderStatusPermissionMappingRepository;
        private readonly IRepository<OrderStatusImageTypeMapping> _orderStatusImageTypeMappingRepository;
        private readonly INotificationService _notificationService;
        private readonly IImageTypeService _imageTypeService;
        private readonly IStoreService _storeService;
        private readonly IShippingService _shippingService;
        private readonly IOrderStatusService _orderStatusService;
        #endregion

        #region Ctor
        public CycleFlowSettingService(
            IRepository<Domain.OrderStatus> orderStatusRepository,
            IRepository<OrderStatusSorting> orderStatusSortingTypeRepository,
            IRepository<OrderStatusPermissionMapping> orderStatusPermissionMappingRepository,
            IRepository<OrderStatusImageTypeMapping> orderStatusImageTypeMappingRepository,
            INotificationService notificationService,
            IImageTypeService imageTypeService,
            IStoreService storeService,
            IShippingService shippingService,
            IOrderStatusService orderStatusService

            )
        {
            _orderStatusRepository = orderStatusRepository;
            _orderStatusSortingTypeRepository = orderStatusSortingTypeRepository;
            _orderStatusPermissionMappingRepository = orderStatusPermissionMappingRepository;
            _orderStatusImageTypeMappingRepository = orderStatusImageTypeMappingRepository;
            _notificationService = notificationService;
            _imageTypeService = imageTypeService;
            _storeService = storeService;
            _shippingService = shippingService ;
            _orderStatusService = orderStatusService;
        }
        #endregion

        #region Methods
        public virtual async Task<IPagedList<OrderStatusSorting>> SearchCycleFlowSettingAsync(
            string orderStatusName = null,
            int storeId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var query = await _orderStatusSortingTypeRepository.GetAllPagedAsync(query =>
            {
                if (storeId > 0)
                    query = query.Where(os => os.NopStoreId == storeId);

                if (!string.IsNullOrWhiteSpace(orderStatusName))
                {
                    query = query.Join(_orderStatusRepository.Table, x => x.OrderStatusId, y => y.Id,
                            (x, y) => new { OrderStatusSorting = x, OrderStatus = y })
                        .Where(z => z.OrderStatus.Name.Contains(orderStatusName))
                        .Select(z => z.OrderStatusSorting)
                        .Distinct();
                }
                return query;

            }, pageIndex, pageSize, getOnlyTotalCount);
            return query;
        }
        public virtual async Task InsertCycleFlowSettingAsync(CycleFlowSettingModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(CycleFlowSettingModel));
            if (IsOrderStatesExsistInSortingAsync(model.CurrentOrderStatusId).Result)
                throw new ArgumentNullException(nameof(CycleFlowSettingModel), "CurrentOrderStatusId already exist");
            if (IsOrderStatesExsistInSortingAsync(model.NextOrderStatusId).Result)
                throw new ArgumentNullException(nameof(CycleFlowSettingModel), "NextOrderStatusId already exist");

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var store = await _storeService.GetStoreByIdAsync(model.StoreId);
                    var nopWarehouse = await _shippingService.GetWarehouseByIdAsync(model.NopWarehouseId);
                    var orderStatus = await _orderStatusService.GetOrderStatusByIdAsync(model.CurrentOrderStatusId);
                    var nextOrderStatus = await _orderStatusService.GetOrderStatusByIdAsync(model.NextOrderStatusId);
                    if(store == null)
                    {
                        throw new Exception($"store not fount with id value {model.StoreId}");
                    }
                    if (nopWarehouse == null)
                    {
                        throw new Exception($"Warehouse not fount with id value {model.NopWarehouseId}");
                    }
                    if (orderStatus == null)
                    {
                        throw new Exception($"orderStatus not fount with id value {model.CurrentOrderStatusId}");
                    }
                    if (nextOrderStatus == null)
                    {
                        throw new Exception($"nextOrderStatus not fount with id value {model.NextOrderStatusId}");
                    }
                    OrderStatusSorting orderStatusSorting = new OrderStatusSorting
                    {
                        OrderStatusId = model.CurrentOrderStatusId,
                        NopStoreId = model.StoreId,
                        WareHouseId = model.NopWarehouseId,
                        NextStep = model.NextOrderStatusId,
                    };
                    await _orderStatusSortingTypeRepository.InsertAsync(orderStatusSorting);
                    OrderStatusPermissionMapping orderStatusPermissionMapping = new OrderStatusPermissionMapping
                    {
                        OrderStatusId = model.CurrentOrderStatusId,
                        NopStoreId = model.StoreId,
                        WareHouseId = model.NopWarehouseId,
                        PosUserId = model.PosUserId,
                    };
                    await _orderStatusPermissionMappingRepository.InsertAsync(orderStatusPermissionMapping);
                    
                    foreach(var image  in model.SelectedImageTypeIds)
                    {
                        var imgtype = await _imageTypeService.GetImageTypeByIdAsync(image);
                        if (imgtype == null)
                        {
                            throw new Exception($"Image not fount with id value {imgtype}");
                        }
                        OrderStatusImageTypeMapping orderStatusImageTypeMapping = new OrderStatusImageTypeMapping
                        {
                            ImageTypeId = imgtype.Id,
                            NopStoreId = model.StoreId,
                            OrderStatusId = model.CurrentOrderStatusId,
                            WareHouseId = model.NextOrderStatusId
                        };
                        await _orderStatusImageTypeMappingRepository.InsertAsync(orderStatusImageTypeMapping);
                    }
                    transaction.Complete();
                }
                catch (Exception exp)
                {
                    _notificationService.ErrorNotification(exp.Message, encode: false);
                    transaction.Dispose();
                }
            }

        }
        public virtual Task<OrderStatusSorting> GetOrderStatusSortingByIdAsync(int orderStatusSortingId)
        {
            if (orderStatusSortingId == 0)
                return null;

            return _orderStatusSortingTypeRepository.GetByIdAsync(orderStatusSortingId);
        }
        public virtual async Task UpdateCycleFlowSettingAsync(CycleFlowSettingModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            if (await IsOrderStatesExsistInSortingAsync(model.NextOrderStatusId, model.Id))
                throw new ArgumentException($"NextOrderStatusId {model.NextOrderStatusId} already exists in sorting.");

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    // جلب الكائنات المطلوبة
                    var store = await _storeService.GetStoreByIdAsync(model.StoreId);
                    var nopWarehouse = await _shippingService.GetWarehouseByIdAsync(model.NopWarehouseId);
                    var orderStatus = await _orderStatusService.GetOrderStatusByIdAsync(model.CurrentOrderStatusId);
                    var nextOrderStatus = await _orderStatusService.GetOrderStatusByIdAsync(model.NextOrderStatusId);

                    // التحقق من وجود الكائنات
                    if (store == null)
                        throw new Exception($"Store not found with id value {model.StoreId}");
                    if (nopWarehouse == null)
                        throw new Exception($"Warehouse not found with id value {model.NopWarehouseId}");
                    if (orderStatus == null)
                        throw new Exception($"OrderStatus not found with id value {model.CurrentOrderStatusId}");
                    if (nextOrderStatus == null)
                        throw new Exception($"NextOrderStatus not found with id value {model.NextOrderStatusId}");

                    var existingOrderStatusSorting = await _orderStatusSortingTypeRepository.Table
                        .FirstOrDefaultAsync(x => x.OrderStatusId == model.CurrentOrderStatusId
                                                  && x.NopStoreId == model.StoreId
                                                  && x.WareHouseId == model.NopWarehouseId);

                    if (existingOrderStatusSorting != null)
                    {
                        existingOrderStatusSorting.NextStep = model.NextOrderStatusId;
                        await _orderStatusSortingTypeRepository.UpdateAsync(existingOrderStatusSorting);
                    }
                    else
                    {
                        OrderStatusSorting orderStatusSorting = new OrderStatusSorting
                        {
                            OrderStatusId = model.CurrentOrderStatusId,
                            NopStoreId = model.StoreId,
                            WareHouseId = model.NopWarehouseId,
                            NextStep = model.NextOrderStatusId,
                        };
                        await _orderStatusSortingTypeRepository.InsertAsync(orderStatusSorting);
                    }

                    var existingOrderStatusPermissionMapping = await _orderStatusPermissionMappingRepository.Table
                        .FirstOrDefaultAsync(x => x.OrderStatusId == model.CurrentOrderStatusId
                                                  && x.NopStoreId == model.StoreId
                                                  && x.WareHouseId == model.NopWarehouseId);

                    if (existingOrderStatusPermissionMapping != null)
                    {
                        existingOrderStatusPermissionMapping.PosUserId = model.PosUserId;
                        await _orderStatusPermissionMappingRepository.UpdateAsync(existingOrderStatusPermissionMapping);
                    }
                    else
                    {
                        OrderStatusPermissionMapping orderStatusPermissionMapping = new OrderStatusPermissionMapping
                        {
                            OrderStatusId = model.CurrentOrderStatusId,
                            NopStoreId = model.StoreId,
                            WareHouseId = model.NopWarehouseId,
                            PosUserId = model.PosUserId,
                        };
                        await _orderStatusPermissionMappingRepository.InsertAsync(orderStatusPermissionMapping);
                    }

                    foreach (var image in model.SelectedImageTypeIds)
                    {
                        var imgtype = await _imageTypeService.GetImageTypeByIdAsync(image);
                        if (imgtype == null)
                        {
                            throw new Exception($"Image not found with id value {image}");
                        }

                        var existingImageTypeMapping = await _orderStatusImageTypeMappingRepository.Table
                            .FirstOrDefaultAsync(x => x.ImageTypeId == imgtype.Id
                                                      && x.NopStoreId == model.StoreId
                                                      && x.OrderStatusId == model.CurrentOrderStatusId
                                                      && x.WareHouseId == model.NopWarehouseId);

                        if (existingImageTypeMapping != null)
                        {
                            existingImageTypeMapping.WareHouseId = model.NextOrderStatusId;
                            await _orderStatusImageTypeMappingRepository.UpdateAsync(existingImageTypeMapping);
                        }
                        else
                        {
                            OrderStatusImageTypeMapping orderStatusImageTypeMapping = new OrderStatusImageTypeMapping
                            {
                                ImageTypeId = imgtype.Id,
                                NopStoreId = model.StoreId,
                                OrderStatusId = model.CurrentOrderStatusId,
                                WareHouseId = model.NextOrderStatusId,
                            };
                            await _orderStatusImageTypeMappingRepository.InsertAsync(orderStatusImageTypeMapping);
                        }
                    }
                    transaction.Complete();
                }
                catch (Exception exp)
                {
                    // عرض رسالة الخطأ للمستخدم
                    _notificationService.ErrorNotification(exp.Message, encode: false);
                    transaction.Dispose();
                }
            }
        }
        public virtual async Task DeleteCycleFlowSettingAsync(OrderStatusSorting model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    // تحقق إذا كان السجل موجوداً في قاعدة البيانات
                    var existingRecord = await _orderStatusSortingTypeRepository.Table
                        .Where(os => os.OrderStatusId == model.OrderStatusId && os.NopStoreId == model.NopStoreId && os.WareHouseId == model.WareHouseId)
                        .FirstOrDefaultAsync();

                    // إذا كان السجل غير موجود، إرجاع خطأ
                    if (existingRecord == null)
                    {
                        throw new Exception($"No record found to delete for OrderStatusId: {model.OrderStatusId}, StoreId: {model.NopStoreId}, WarehouseId: {model.WareHouseId}");
                    }

                    // حذف السجل
                    await _orderStatusSortingTypeRepository.DeleteAsync(existingRecord);

                    // إذا كنت ترغب في حذف mappings أو سجلات مرتبطة، يمكنك إضافة الحذف هنا
                    var permissionMapping = await _orderStatusPermissionMappingRepository.Table
                        .Where(os => os.OrderStatusId == model.OrderStatusId && os.NopStoreId == model.NopStoreId && os.WareHouseId == model.WareHouseId)
                        .FirstOrDefaultAsync();

                    if (permissionMapping != null)
                    {
                        await _orderStatusPermissionMappingRepository.DeleteAsync(permissionMapping);
                    }

                    var imageTypeMapping = await _orderStatusImageTypeMappingRepository.Table
                        .Where(os => os.OrderStatusId == model.OrderStatusId && os.NopStoreId == model.NopStoreId && os.WareHouseId == model.WareHouseId)
                        .FirstOrDefaultAsync();

                    if (imageTypeMapping != null)
                    {
                        await _orderStatusImageTypeMappingRepository.DeleteAsync(imageTypeMapping);
                    }

                    // أكمل المعاملة
                    transaction.Complete();
                }
                catch (Exception exp)
                {
                    _notificationService.ErrorNotification(exp.Message, encode: false);
                    transaction.Dispose();  // إنهاء المعاملة في حالة حدوث خطأ
                }
            }
        }


        #endregion
        #region Utilite
        protected virtual async Task<bool> IsOrderStatesExsistInSortingAsync(int orderStateId, int currentId = 0)
        {
            if (orderStateId > 0)
            {
                var query = _orderStatusSortingTypeRepository.Table.Where(os => os.OrderStatusId == orderStateId);

                if (currentId > 0)
                    query = query.Where(os => os.Id != currentId);  

                return await query.AnyAsync();             
            }
            return true;
        }

        #endregion
    }

}
