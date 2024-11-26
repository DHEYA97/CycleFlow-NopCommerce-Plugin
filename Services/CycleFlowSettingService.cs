using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Plugin.Misc.CycleFlow.Models.CycleFlowSetting;
using Nop.Plugin.Misc.POSSystem.Domains;
using Nop.Plugin.Misc.POSSystem.Services;
using Nop.Services.Customers;
using Nop.Services.Messages;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
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
        private readonly IPosUserService _posUserService;
        private readonly ICustomerService _customerService;
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
            IOrderStatusService orderStatusService,
            IPosUserService posUserService,
            ICustomerService customerService
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
            _posUserService = posUserService;
            _customerService = customerService;

        }
        #endregion

        #region Methods
        public virtual async Task<IPagedList<OrderStatusSorting>> SearchCycleFlowSettingAsync(
            string orderStatusName = null,
             IList<int> posUserIds = null,
            IList<int> storeIds = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var query = await _orderStatusSortingTypeRepository.GetAllPagedAsync(query =>
            {
                
                if (storeIds?.Any(x => x != 0) ?? false)
                    query = query.Where(p => storeIds.Contains(p.NopStoreId));
                
                if (posUserIds?.Any(x => x != 0) ?? false)
                    query = query.Where(p => posUserIds.Contains(p.PosUserId));

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
            if (IsCurrentOrderStatesExsistInSortingAsync(model.CurrentOrderStatusId,model.PosUserId).Result)
                throw new ArgumentNullException(nameof(CycleFlowSettingModel), "CurrentOrderStatusId already exist");
            if (IsNextOrderStatesExsistInSortingAsync(model.NextOrderStatusId, model.PosUserId).Result)
                throw new ArgumentNullException(nameof(CycleFlowSettingModel), "NextOrderStatusId already exist");

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var store = await _storeService.GetStoreByIdAsync(model.StoreId);
                    var posUser = await _posUserService.GetPosUserByIdAsync(model.PosUserId);
                    var customer = await _customerService.GetCustomerByIdAsync(model.CustomerId);
                    var orderStatus = await _orderStatusService.GetOrderStatusByIdAsync(model.CurrentOrderStatusId);
                    var nextOrderStatus = await _orderStatusService.GetOrderStatusByIdAsync(model.NextOrderStatusId);
                    if(store == null)
                    {
                        throw new Exception($"store not fount with id value {model.StoreId}");
                    }
                    if (posUser == null)
                    {
                        throw new Exception($"posUser not fount with id value {model.PosUserId}");
                    }
                    if (customer == null)
                    {
                        throw new Exception($"customer not fount with id value {model.CustomerId}");
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
                        PosUserId = model.PosUserId,
                        NextStep = model.NextOrderStatusId,
                        IsFirstStep = model.IsFirstStep,
                        IsLastStep  = model.IsLastStep,

                    };
                    await _orderStatusSortingTypeRepository.InsertAsync(orderStatusSorting);
                    OrderStatusPermissionMapping orderStatusPermissionMapping = new OrderStatusPermissionMapping
                    {
                        OrderStatusId = model.CurrentOrderStatusId,
                        NopStoreId = model.StoreId,
                        PosUserId = model.PosUserId,
                        CustomerId = model.CustomerId,
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
                            PosUserId = model.PosUserId,

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
            if (await IsNextOrderStatesExsistInSortingAsync(model.NextOrderStatusId,model.PosUserId, model.Id))
                throw new ArgumentException($"NextOrderStatusId {model.NextOrderStatusId} already exists in sorting.");

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    // جلب الكائنات المطلوبة
                    var store = await _storeService.GetStoreByIdAsync(model.StoreId);
                    var posUser = await _posUserService.GetPosUserByIdAsync(model.PosUserId);
                    var customer = await _customerService.GetCustomerByIdAsync(model.CustomerId);
                    var orderStatus = await _orderStatusService.GetOrderStatusByIdAsync(model.CurrentOrderStatusId);
                    var nextOrderStatus = await _orderStatusService.GetOrderStatusByIdAsync(model.NextOrderStatusId);

                    // التحقق من وجود الكائنات
                    if (store == null)
                        throw new Exception($"Store not found with id value {model.StoreId}");
                    if (posUser == null)
                        throw new Exception($"Warehouse not found with id value {model.PosUserId}");
                    if (customer == null)
                        throw new Exception($"customer not found with id value {model.CustomerId}");
                    if (orderStatus == null)
                        throw new Exception($"OrderStatus not found with id value {model.CurrentOrderStatusId}");
                    if (nextOrderStatus == null)
                        throw new Exception($"NextOrderStatus not found with id value {model.NextOrderStatusId}");

                    var existingOrderStatusSorting = await _orderStatusSortingTypeRepository.Table
                        .FirstOrDefaultAsync(x => x.PosUserId == model.PosUserId && x.OrderStatusId == model.CurrentOrderStatusId);

                    if (existingOrderStatusSorting != null)
                    {
                        existingOrderStatusSorting = model.ToEntity(existingOrderStatusSorting);
                        existingOrderStatusSorting.NextStep = model.NextOrderStatusId;
                        await _orderStatusSortingTypeRepository.UpdateAsync(existingOrderStatusSorting);
                    }
                    else
                    {
                        var orderStatusSorting = model.ToEntity<OrderStatusSorting>();
                        await _orderStatusSortingTypeRepository.InsertAsync(orderStatusSorting);
                    }

                    var existingOrderStatusPermissionMapping = await _orderStatusPermissionMappingRepository.Table
                        .FirstOrDefaultAsync(x => x.PosUserId == model.PosUserId && x.OrderStatusId == model.CurrentOrderStatusId);

                    if (existingOrderStatusPermissionMapping != null)
                    {
                        existingOrderStatusPermissionMapping.PosUserId = model.PosUserId;
                        existingOrderStatusPermissionMapping.CustomerId = model.CustomerId;
                        existingOrderStatusPermissionMapping.NopStoreId = model.StoreId;
                        await _orderStatusPermissionMappingRepository.UpdateAsync(existingOrderStatusPermissionMapping);
                    }
                    else
                    {
                        OrderStatusPermissionMapping orderStatusPermissionMapping = new OrderStatusPermissionMapping
                        {
                            OrderStatusId = model.CurrentOrderStatusId,
                            NopStoreId = model.StoreId,
                            PosUserId = model.PosUserId,
                            CustomerId = model.CustomerId,
                        };
                        await _orderStatusPermissionMappingRepository.InsertAsync(orderStatusPermissionMapping);
                    }

                    var existingOrderStatusImageMapping = await _orderStatusImageTypeMappingRepository.Table
                                                                    .Where(x => x.PosUserId == model.PosUserId && x.OrderStatusId == model.CurrentOrderStatusId)
                                                                    .Select(x=>x.ImageTypeId)
                                                                    .ToListAsync();
                    
                    var removedImage = existingOrderStatusImageMapping.ToList().Except(model.SelectedImageTypeIds.ToList());
                    
                    foreach (var imageId in model.SelectedImageTypeIds.Where(x=> x!= 0 ).ToList())
                    {
                        var imgType = await _imageTypeService.GetImageTypeByIdAsync(imageId);
                        if (imgType == null)
                        {
                            throw new Exception($"Image not found with id value {imageId}");
                        }

                        if (existingOrderStatusImageMapping.Contains(imageId))
                        {
                            var img = await _orderStatusImageTypeMappingRepository.Table
                                                                    .FirstOrDefaultAsync(x => x.PosUserId == model.PosUserId && x.OrderStatusId == model.CurrentOrderStatusId && x.ImageTypeId == imageId);
                            img.NopStoreId = model.StoreId;
                            await _orderStatusImageTypeMappingRepository.UpdateAsync(img);
                        }
                        else
                        {
                            OrderStatusImageTypeMapping orderStatusImageTypeMapping = new OrderStatusImageTypeMapping
                            {
                                ImageTypeId = imgType.Id,
                                NopStoreId = model.StoreId,
                                OrderStatusId = model.CurrentOrderStatusId,
                                PosUserId = model.PosUserId,
                                
                            };
                            await _orderStatusImageTypeMappingRepository.InsertAsync(orderStatusImageTypeMapping);
                        }
                    }
                    foreach(var imageId in removedImage)
                    {
                        var imgType = await _imageTypeService.GetImageTypeByIdAsync(imageId);
                        if (imgType == null)
                        {
                            throw new Exception($"Image not found with id value {imageId}");
                        }
                            var img = await _orderStatusImageTypeMappingRepository.Table
                                                                    .FirstOrDefaultAsync(x => x.PosUserId == model.PosUserId && x.OrderStatusId == model.CurrentOrderStatusId && x.ImageTypeId == imageId);
                            await _orderStatusImageTypeMappingRepository.DeleteAsync(img);
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
                        .Where(os => os.OrderStatusId == model.OrderStatusId && os.NopStoreId == model.NopStoreId && os.PosUserId == model.PosUserId)
                        .FirstOrDefaultAsync();

                    // إذا كان السجل غير موجود، إرجاع خطأ
                    if (existingRecord == null)
                    {
                        throw new Exception($"No record found to delete for OrderStatusId: {model.OrderStatusId}, StoreId: {model.NopStoreId}, PosUserId: {model.PosUserId}");
                    }

                    // حذف السجل
                    await _orderStatusSortingTypeRepository.DeleteAsync(existingRecord);

                    // إذا كنت ترغب في حذف mappings أو سجلات مرتبطة، يمكنك إضافة الحذف هنا
                    var permissionMapping = await _orderStatusPermissionMappingRepository.Table
                        .Where(os => os.OrderStatusId == model.OrderStatusId && os.NopStoreId == model.NopStoreId && os.PosUserId == model.PosUserId)
                        .FirstOrDefaultAsync();

                    if (permissionMapping != null)
                    {
                        await _orderStatusPermissionMappingRepository.DeleteAsync(permissionMapping);
                    }

                    var imageTypeMapping = await _orderStatusImageTypeMappingRepository.Table
                        .Where(os => os.OrderStatusId == model.OrderStatusId && os.NopStoreId == model.NopStoreId && os.PosUserId == model.PosUserId)
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
        public virtual async Task<IList<(string, string)>> GetFirstOrderStatusAsync(int posUserId ,int currentId = 0, bool exclude = false)
        {
            var query = _orderStatusRepository.Table
                .Where(z => z.IsActive && !z.Deleted);

            if (exclude)
            {
                var currentOrderStatusSorting = await _orderStatusSortingTypeRepository.Table
                    .Where(x=>x.PosUserId == posUserId)
                    .Select(o => o.OrderStatusId)
                    .ToListAsync();

                query = query.Where(o =>
                    !currentOrderStatusSorting.Contains(o.Id) ||
                    o.Id == currentId);
            }

            var list = await query
                .Select(x => new
                {
                    Id = x.Id.ToString(),
                    Name = x.Name.ToString()
                })
                .ToListAsync();
            var result = list.Select(x => (x.Id, x.Name)).ToList();
            return result;
        }

        public virtual async Task<IList<(string, string)>> GetNextOrderStatusAsync(int posUserId ,int currentId = 0 , bool exclude = false)
        {
            var query = _orderStatusRepository.Table
                .Where(z => z.IsActive && !z.Deleted);

            if (exclude)
            {
                var currentOrderStatusSorting = await _orderStatusSortingTypeRepository.Table
                    .Where(o=>o.PosUserId == posUserId)
                    .Select(o => o.NextStep)
                    .ToListAsync();


                var next = await GetNextStepByFirstStep(currentId);


                query = query.Where(o =>
                        (o.Id != currentId) &&
                        (!currentOrderStatusSorting.Contains(o.Id) || o.Id == next));
            }

            var list = await query
                .Select(x => new
                {
                    Id = x.Id.ToString(),
                    Name = x.Name.ToString()
                })
                .ToListAsync();
            var result = list.Select(x => (x.Id, x.Name)).ToList();
            return result;
        }
        public virtual async Task<int> GetNextStepByFirstStep(int firstStep)
        {
            return await _orderStatusSortingTypeRepository.Table
                    .Where(x => x.OrderStatusId == firstStep)
                    .Select(o => o.NextStep)
                    .FirstOrDefaultAsync();
        }
        public virtual async Task<IList<int>> GetAllOrderCurrentSelectedImageTypeAsync(int posUserId,int orderStatusId)
        {
            return await _orderStatusImageTypeMappingRepository.Table.Where(i=> i.PosUserId == posUserId && i.OrderStatusId == orderStatusId).Select(i=>i.ImageTypeId).ToListAsync();
        }
        public async Task<Customer> GetCustomerAsync(int posUserId, int orderStatusId)
        {
            var customerId = await _orderStatusPermissionMappingRepository.Table
                                        .Where(p=> p.PosUserId == posUserId && p.OrderStatusId == orderStatusId)
                                        .Select(x=>x.CustomerId).FirstAsync();
            var customer = await _customerService.GetCustomerByIdAsync(customerId);
            return customer;             
        }
        public virtual async Task<bool> EnableIsFirstStepAsync(int posUserId,int currentId = 0)
        {
            return await _orderStatusSortingTypeRepository.Table.AnyAsync(x=>x.IsFirstStep && x.PosUserId == posUserId && x.Id != currentId);
        }
        public virtual async Task<bool> EnableIsLastStepAsync(int posUserId,int currentId = 0)
        {
            return await _orderStatusSortingTypeRepository.Table.AnyAsync(x => x.IsLastStep && x.PosUserId == posUserId && x.Id != currentId);
        }
        public virtual async Task<bool> IsCurrentOrderStatesExsistInSortingAsync(int orderStateId, int posUserId, int currentId = 0)
        {
            if (orderStateId > 0 && posUserId > 0)
            {
                var query = _orderStatusSortingTypeRepository.Table.Where(os => os.OrderStatusId == orderStateId && os.PosUserId == posUserId);

                if (currentId > 0)
                    query = query.Where(os => os.Id != currentId);

                return await query.AnyAsync();
            }
            return true;
        }
        public virtual async Task<bool> IsNextOrderStatesExsistInSortingAsync(int orderStateId, int posUserId, int currentId = 0)
        {
            if (orderStateId > 0 && posUserId > 0)
            {
                var query = _orderStatusSortingTypeRepository.Table.Where(os => os.NextStep == orderStateId && os.PosUserId == posUserId);

                if (currentId > 0)
                    query = query.Where(os => os.Id != currentId);

                return await query.AnyAsync();
            }
            return true;
        }
        #endregion
        #region Utilite


        #endregion
    }

}
