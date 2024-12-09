using DocumentFormat.OpenXml.Spreadsheet;
using iTextSharp.text.html.simpleparser;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Plugin.Misc.CycleFlow.Models.CycleFlowSetting;
using Nop.Plugin.Misc.POSSystem.Domains;
using Nop.Plugin.Misc.POSSystem.Services;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using System.Security.Policy;
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
        protected readonly IWorkContext _workContext;
        protected readonly ILocalizationService _localizationService;
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
            ICustomerService customerService,
            IWorkContext workContext,
            ILocalizationService localizationService
            )
        {
            _orderStatusRepository = orderStatusRepository;
            _orderStatusSortingTypeRepository = orderStatusSortingTypeRepository;
            _orderStatusPermissionMappingRepository = orderStatusPermissionMappingRepository;
            _orderStatusImageTypeMappingRepository = orderStatusImageTypeMappingRepository;
            _notificationService = notificationService;
            _imageTypeService = imageTypeService;
            _storeService = storeService;
            _shippingService = shippingService;
            _orderStatusService = orderStatusService;
            _posUserService = posUserService;
            _customerService = customerService;
            _workContext = workContext;
            _localizationService = localizationService;

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
            if (IsCurrentOrderStatesExsistInSortingAsync(model.CurrentOrderStatusId, model.PosUserId).Result)
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
                    if (store == null)
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
                    OrderStatusSorting orderStatusSorting = model.ToEntity<OrderStatusSorting>();
                    await orderStatusSorting.SetBaseInfoAsync<OrderStatusSorting>(_workContext);
                    orderStatusSorting.ClientSmsTemplateId = model.ClientSmsTemplateId <= 0 ? null : model.ClientSmsTemplateId;
                    orderStatusSorting.UserSmsTemplateId = model.UserSmsTemplateId <= 0 ? null : model.UserSmsTemplateId;
                    orderStatusSorting.ReturnStepId = model.ReturnStepId <= 0 ? null : model.ReturnStepId;

                    await _orderStatusSortingTypeRepository.InsertAsync(orderStatusSorting);
                    OrderStatusPermissionMapping orderStatusPermissionMapping = new OrderStatusPermissionMapping
                    {
                        OrderStatusId = model.CurrentOrderStatusId,
                        NopStoreId = model.StoreId,
                        PosUserId = model.PosUserId,
                        CustomerId = model.CustomerId,
                    };
                    await orderStatusPermissionMapping.SetBaseInfoAsync<OrderStatusPermissionMapping>(_workContext);
                    await _orderStatusPermissionMappingRepository.InsertAsync(orderStatusPermissionMapping);

                    foreach (var image in model.SelectedImageTypeIds)
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
                        await orderStatusImageTypeMapping.SetBaseInfoAsync<OrderStatusImageTypeMapping>(_workContext);
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
            if (await IsNextOrderStatesExsistInSortingAsync(model.NextOrderStatusId, model.PosUserId, model.Id))
                throw new ArgumentException($"NextOrderStatusId {model.NextOrderStatusId} already exists in sorting.");

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var store = await _storeService.GetStoreByIdAsync(model.StoreId);
                    var posUser = await _posUserService.GetPosUserByIdAsync(model.PosUserId);
                    var customer = await _customerService.GetCustomerByIdAsync(model.CustomerId);
                    var orderStatus = await _orderStatusService.GetOrderStatusByIdAsync(model.CurrentOrderStatusId);
                    var nextOrderStatus = await _orderStatusService.GetOrderStatusByIdAsync(model.NextOrderStatusId);

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
                        existingOrderStatusSorting.ClientSmsTemplateId = model.ClientSmsTemplateId <= 0 ? null : model.ClientSmsTemplateId;
                        existingOrderStatusSorting.UserSmsTemplateId = model.UserSmsTemplateId <= 0 ? null : model.UserSmsTemplateId;
                        existingOrderStatusSorting.ReturnStepId = model.ReturnStepId <= 0 ? null : model.ReturnStepId;

                        await existingOrderStatusSorting.SetBaseInfoAsync<OrderStatusSorting>(_workContext);
                        await _orderStatusSortingTypeRepository.UpdateAsync(existingOrderStatusSorting);
                    }
                    else
                    {
                        var orderStatusSorting = model.ToEntity<OrderStatusSorting>();
                        await orderStatusSorting.SetBaseInfoAsync<OrderStatusSorting>(_workContext);
                        await _orderStatusSortingTypeRepository.InsertAsync(orderStatusSorting);
                    }

                    var existingOrderStatusPermissionMapping = await _orderStatusPermissionMappingRepository.Table
                        .FirstOrDefaultAsync(x => x.PosUserId == model.PosUserId && x.OrderStatusId == model.CurrentOrderStatusId);

                    if (existingOrderStatusPermissionMapping != null)
                    {
                        existingOrderStatusPermissionMapping.PosUserId = model.PosUserId;
                        existingOrderStatusPermissionMapping.CustomerId = model.CustomerId;
                        existingOrderStatusPermissionMapping.NopStoreId = model.StoreId;

                        await existingOrderStatusPermissionMapping.SetBaseInfoAsync<OrderStatusPermissionMapping>(_workContext);
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
                        await orderStatusPermissionMapping.SetBaseInfoAsync<OrderStatusPermissionMapping>(_workContext);
                        await _orderStatusPermissionMappingRepository.InsertAsync(orderStatusPermissionMapping);
                    }

                    var existingOrderStatusImageMapping = await _orderStatusImageTypeMappingRepository.Table
                                                                    .Where(x => x.PosUserId == model.PosUserId && x.OrderStatusId == model.CurrentOrderStatusId)
                                                                    .Select(x => x.ImageTypeId)
                                                                    .ToListAsync();

                    var removedImage = existingOrderStatusImageMapping.ToList().Except(model.SelectedImageTypeIds.ToList());

                    foreach (var imageId in model.SelectedImageTypeIds.Where(x => x != 0).ToList())
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

                            await img.SetBaseInfoAsync<OrderStatusImageTypeMapping>(_workContext);
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
                            await orderStatusImageTypeMapping.SetBaseInfoAsync<OrderStatusImageTypeMapping>(_workContext);
                            await _orderStatusImageTypeMappingRepository.InsertAsync(orderStatusImageTypeMapping);
                        }
                    }
                    foreach (var imageId in removedImage)
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
                    var existingRecord = await _orderStatusSortingTypeRepository.Table
                        .Where(os => os.OrderStatusId == model.OrderStatusId && os.NopStoreId == model.NopStoreId && os.PosUserId == model.PosUserId)
                        .FirstOrDefaultAsync();

                    if (existingRecord == null)
                    {
                        throw new Exception($"No record found to delete for OrderStatusId: {model.OrderStatusId}, StoreId: {model.NopStoreId}, PosUserId: {model.PosUserId}");
                    }

                    await _orderStatusSortingTypeRepository.DeleteAsync(existingRecord);

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

                    transaction.Complete();
                }
                catch (Exception exp)
                {
                    _notificationService.ErrorNotification(exp.Message, encode: false);
                    transaction.Dispose();
                }
            }
        }
        public virtual async Task<IList<(string, string)>> GetFirstOrderStatusAsync(int posUserId, int currentId = 0, bool exclude = false)
        {
            var query = _orderStatusRepository.Table
                .Where(z => z.IsActive && !z.Deleted);

            if (exclude)
            {
                var currentOrderStatusSorting = await _orderStatusSortingTypeRepository.Table
                    .Where(x => x.PosUserId == posUserId)
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

        public virtual async Task<IList<(string, string, int)>> GetNextOrderStatusAsync(int posUserId, int currentId = 0, bool exclude = false)
        {
            var query = _orderStatusRepository.Table
                .Where(z => z.IsActive && !z.Deleted);

            var next = await GetNextStepByFirstStepAsync(currentId, posUserId);

            if (exclude)
            {
                var currentOrderStatusSorting = await _orderStatusSortingTypeRepository.Table
                    .Where(o => o.PosUserId == posUserId)
                    .Select(o => o.NextStep)
                    .ToListAsync();
                
                var currentNext =  (await _orderStatusSortingTypeRepository.Table
                    .FirstOrDefaultAsync(o => o.PosUserId == posUserId && o.IsFirstStep))?.OrderStatusId ?? 0;
                    
                query = query.Where(o =>
                        (o.Id != currentId) &&
                        (!currentOrderStatusSorting.Contains(o.Id) || o.Id == next) &&
                        o.Id != currentNext
                        );
            }

            var list = await query
                .Select(x => new
                {
                    Id = x.Id.ToString(),
                    Name = x.Name.ToString(),
                    Next = next
                })
                .ToListAsync();
            var result = list.Select(x => (x.Id, x.Name, next)).ToList();
            return result;
        }
        public virtual async Task<IList<(string, string,int)>> GetReturnOrderStatusAsync(int posUserId, int currentId = 0, bool exclude = false)
        {
            var query = _orderStatusRepository.Table
                .Where(z => z.IsActive && !z.Deleted);
            var orderStetsList = new List<int?>();

            var returnstep = await GetReturnStepByNextStepAsync(currentId, posUserId);
            if (exclude && currentId > 0)
            {
                var isFirst = await _orderStatusSortingTypeRepository.Table
                    .AnyAsync(x => x.PosUserId == posUserId && x.OrderStatusId == currentId && x.IsFirstStep);
                if (isFirst)
                {
                    return new List<(string, string,int)>();
                }

                //var current = await _orderStatusSortingTypeRepository.Table
                //    .FirstOrDefaultAsync(x => x.PosUserId == posUserId && x.OrderStatusId == currentId);
                //if (current == null)
                //{
                //    return new List<(string, string)>();
                //}

                var prefId = await GetFirstStepByNextStepAsync(currentId, posUserId);
                while (true)
                {
                    if (prefId == null) break;

                    orderStetsList.Add(prefId);

                    var isPrefFirstStep = await _orderStatusSortingTypeRepository.Table
                        .AnyAsync(x => x.PosUserId == posUserId && x.OrderStatusId == prefId && x.IsFirstStep);
                    if (isPrefFirstStep)
                    {
                        break;
                    }
                    var current = await _orderStatusSortingTypeRepository.Table
                        .FirstOrDefaultAsync(x => x.PosUserId == posUserId && x.OrderStatusId == prefId);
                    if (current == null)
                    {
                        break;
                    }
                    prefId = await GetFirstStepByNextStepAsync(current.OrderStatusId, current.PosUserId);
                }
            }

            var list = await query
                .Where(x => orderStetsList.Where(o => o != null).Contains(x.Id))
                .Select(x => new
                {
                    Id = x.Id.ToString(),
                    Name = x.Name.ToString(),
                    Return = returnstep ?? 0
                })
                .ToListAsync();

            return list.Select(x => (x.Id, x.Name,x.Return)).ToList();
        }

        public virtual async Task<int> GetNextStepByFirstStepAsync(int firstStep, int posUserId)
        {
            return await _orderStatusSortingTypeRepository.Table
                    .Where(x => x.OrderStatusId == firstStep && x.PosUserId == posUserId)
                    .Select(o => o.NextStep)
                    .FirstOrDefaultAsync();
        }
        public virtual async Task<IList<int>> GetAllOrderCurrentSelectedImageTypeAsync(int posUserId, int orderStatusId)
        {
            return await _orderStatusImageTypeMappingRepository.Table.Where(i => i.PosUserId == posUserId && i.OrderStatusId == orderStatusId).Select(i => i.ImageTypeId).ToListAsync();
        }
        public async Task<Customer> GetCustomerAsync(int posUserId, int orderStatusId)
        {
            var customerId = await _orderStatusPermissionMappingRepository.Table
                                        .Where(p => p.PosUserId == posUserId && p.OrderStatusId == orderStatusId)
                                        .Select(x => x.CustomerId).FirstAsync();
            var customer = await _customerService.GetCustomerByIdAsync(customerId);
            return customer;
        }
        public virtual async Task<bool> EnableIsFirstStepAsync(int posUserId, int currentId = 0)
        {
            return await _orderStatusSortingTypeRepository.Table.AnyAsync(x => x.IsFirstStep && x.PosUserId == posUserId && x.Id != currentId);
        }
        public virtual async Task<bool> EnableIsLastStepAsync(int posUserId, int currentId = 0)
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
        public virtual async Task<(string,bool)> CheckOrderStatusSequenceAsync(int posUserId)
        {
            var orderStatusSorting = await _orderStatusSortingTypeRepository.Table
                .Where(x => x.PosUserId == posUserId)
                .ToListAsync();

            var firstStep = orderStatusSorting.FirstOrDefault(x => x.IsFirstStep);
            var lastStep = orderStatusSorting.FirstOrDefault(x => x.IsLastStep);
            bool status = false;

            if (firstStep == null)
            {
                return ($"<div class='alert alert-danger'>لا توجد حالة أولى (IsFirstStep == true).</div>", status);
            }

            if (lastStep == null)
            {
                return ($"<div class='alert alert-danger'>لا توجد حالة أخيرة (IsLastStep == true).</div>", status);
            }

            var currentStatus = firstStep;
            HashSet<int> visitedStatuses = new HashSet<int>();
            visitedStatuses.Add(currentStatus.OrderStatusId);

            var firstStatusName = await _orderStatusService.GetOrderStatusNameAsync(currentStatus.OrderStatusId);

            string sequenceHtml = $"<div class='sequence-box'>{firstStatusName} ({currentStatus.OrderStatusId})</div>";
            while (currentStatus != null)
            {
                var nextStatus = orderStatusSorting.FirstOrDefault(x => x.OrderStatusId == currentStatus.NextStep);
                if (nextStatus == null)
                {
                    sequenceHtml += "<div class='arrow'></div>";
                    sequenceHtml += $"<div class='sequence-box'>{_orderStatusService.GetOrderStatusNameAsync(currentStatus.NextStep).Result} ({currentStatus.NextStep})</div>";

                    sequenceHtml += $"<div class='alert alert-danger'>انقطاع في السلسلة: لم يتم العثور على الحالة التالية بعد {currentStatus.NextStep} ({_orderStatusService.GetOrderStatusNameAsync(currentStatus.NextStep).Result}).</div>";
                    break;
                }

                if (visitedStatuses.Contains(nextStatus.OrderStatusId))
                {
                    sequenceHtml += "<div class='arrow'></div>";
                    sequenceHtml += $"<div class='sequence-box'>{_orderStatusService.GetOrderStatusNameAsync(currentStatus.NextStep).Result} ({currentStatus.NextStep})</div>";

                    sequenceHtml += $"<div class='alert alert-danger'>انقطاع في السلسلة: تم العثور على حلقة بين الحالة {currentStatus.OrderStatusId} ({firstStatusName}) والحالة {nextStatus.OrderStatusId}.</div>";
                    break;
                }

                visitedStatuses.Add(nextStatus.OrderStatusId);

                var nextStatusName = await _orderStatusService.GetOrderStatusNameAsync(nextStatus.OrderStatusId);

                sequenceHtml += "<div class='arrow'></div>";
                sequenceHtml += $"<div class='sequence-box'>{nextStatusName} ({nextStatus.OrderStatusId})</div>";

                if (nextStatus.IsLastStep)
                {
                    var lastStatusName = await _orderStatusService.GetOrderStatusNameAsync(nextStatus.NextStep);

                    sequenceHtml += "<div class='arrow'></div>";
                    sequenceHtml += $"<div class='sequence-box'>{lastStatusName} ({nextStatus.NextStep})</div>";

                    sequenceHtml += "<div class='alert alert-success'>السلسلة مكتملة.</div>";
                    status = true;
                    break;
                }

                currentStatus = nextStatus;
            }
            return (sequenceHtml,status);
        }
        public virtual async Task<Customer> GetCustomerByOrderStatusIdAsync(int orderStateId, int posUserId)
        {
            var customerId = await _orderStatusPermissionMappingRepository.Table
                                                                    .Where(x => x.OrderStatusId == orderStateId && x.PosUserId == posUserId)
                                                                    .Select(x=>x.CustomerId)
                                                                    .FirstOrDefaultAsync();
            var customer = await _customerService.GetCustomerByIdAsync(customerId);
            return customer;
        }
        public virtual async Task<OrderStatusSorting> GetFirstStepInPosUserAsync(int posUserId)
        {
            return await _orderStatusSortingTypeRepository.Table
                                                          .Where(x => x.PosUserId == posUserId && x.IsFirstStep)
                                                          .FirstOrDefaultAsync();
        }
        public virtual async Task NotificationPosUserAsync()
        {
            var posUsers = await (await _posUserService.GetPosUserListAsync()).Where(ps => ps.Active).ToListAsync();
            foreach (var user in posUsers)
            {
                 var posUserId = (await _posUserService.GetPosUserByNopCustomerIdAsync(user.Id)).Id;
                 var posUserName = (await _customerService.GetCustomerFullNameAsync(user));
                 var (text, status) = await CheckOrderStatusSequenceAsync(posUserId);
                 if(!status)
                 {
                    _notificationService.ErrorNotification(string.Format(await _localizationService.GetResourceAsync("Admin.Plugin.Misc.CycleFlow.Deportation.Check"), posUserId, posUserName, $"<a href='/Admin/CheckPosOrderStatus/View/{posUserId}'>Her</a>", $"<a href='/Admin/CycleFlowSetting/List'>Her</a>"), false);
                 }
            }
        }
        public virtual async Task<int?> GetReturnStepByNextStepAsync(int statusId, int posUserId)
        {
            return await _orderStatusSortingTypeRepository.Table
                    .Where(x => x.OrderStatusId == statusId && x.PosUserId == posUserId)
                    .Select(o => o.ReturnStepId)
                    .FirstOrDefaultAsync();
        }

        #endregion
        #region Utilite
        protected virtual async Task<int?> GetFirstStepByNextStepAsync(int nextstep, int posUserId)
        {
            return await _orderStatusSortingTypeRepository.Table
                    .Where(x => x.NextStep == nextstep && x.PosUserId == posUserId)
                    .Select(o => o.OrderStatusId)
                    .FirstOrDefaultAsync();
        }
        #endregion
    }
}
