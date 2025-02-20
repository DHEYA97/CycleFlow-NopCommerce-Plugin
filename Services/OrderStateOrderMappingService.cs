using Nop.Core;
using Nop.Data;
using Nop.Plugin.Misc.CycleFlow.Constant.Enum;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Plugin.Misc.CycleFlow.Models.CycleFlowSetting;
using Nop.Plugin.Misc.CycleFlow.Models.Deportation;
using Nop.Plugin.Misc.POSSystem.Services;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using RestSharp;

using System.Transactions;

namespace Nop.Plugin.Misc.CycleFlow.Services
{
    public class OrderStateOrderMappingService : IOrderStateOrderMappingService
    {
        #region Fields
        private readonly ICycleFlowSettingService _cycleFlowSettingService;
        private readonly ILocalizationService _localizationService;
        protected readonly INotificationService _notificationService;
        private readonly IRepository<OrderStateOrderImageMapping> _orderStateOrderImageMapping;
        private readonly IRepository<OrderStateOrderMapping> _orderStateOrderMapping;
        private readonly IPictureService _pictureService;
        private readonly IPosOrderService _posOrderService;
        private readonly IOrderStatusService _orderStatusService;
        protected readonly IWorkContext _workContext;

        #endregion

        #region Ctor
        public OrderStateOrderMappingService(
            ICycleFlowSettingService cycleFlowSettingService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IRepository<OrderStateOrderImageMapping> orderStateOrderImageMapping,
            IRepository<OrderStateOrderMapping> orderStateOrderMapping,
            IPictureService pictureService,
            IPosOrderService posOrderService,
            IOrderStatusService orderStatusService,
            IWorkContext workContext
            )
        {
            _cycleFlowSettingService = cycleFlowSettingService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _orderStateOrderImageMapping = orderStateOrderImageMapping;
            _orderStateOrderMapping = orderStateOrderMapping;
            _pictureService = pictureService;
            _posOrderService = posOrderService;
            _orderStatusService = orderStatusService;
            _workContext = workContext;
        }
        #endregion
        #region Method
        public virtual async Task InsertOrderStateOrderImageMappingAsync(OrderStateOrderImageMapping orderStateOrderImageMapping)
        {
            await orderStateOrderImageMapping.SetBaseInfoAsync<OrderStateOrderImageMapping>(_workContext);
            await _orderStateOrderImageMapping.InsertAsync(orderStateOrderImageMapping);
        }
        public virtual async Task UpdateOrderStateOrderImageMappingAsync(OrderStateOrderImageMapping orderStateOrderImageMapping)
        {
            await orderStateOrderImageMapping.SetBaseInfoAsync<OrderStateOrderImageMapping>(_workContext);
            await _orderStateOrderImageMapping.UpdateAsync(orderStateOrderImageMapping);
        }
        public virtual async Task DeleteOrderStateOrderImageMappingAsync(OrderStateOrderImageMapping orderStateOrderImageMapping)
        {
            await orderStateOrderImageMapping.SetBaseInfoAsync<OrderStateOrderImageMapping>(_workContext);
            await _orderStateOrderImageMapping.DeleteAsync(orderStateOrderImageMapping);
        }
        public async Task<OrderStateOrderImageMapping> GetOrderStateOrderImageMappingByIdAsync(int orderStateOrderImageMappingId)
        {
            return await _orderStateOrderImageMapping.GetByIdAsync(orderStateOrderImageMappingId);
        }
        public virtual async Task<IList<OrderStateOrderImageMapping>> GetAllOrderStateOrderImageMappingPictureOrderStatusIdAsync(int posUserId,int orderId,int orderStatusId)
        {
             return await _orderStateOrderImageMapping.Table.Where(x=> x.PosUserId == posUserId && x.OrderId == orderId && x.OrderStatusId == orderStatusId).ToListAsync();
                
        }
        public virtual async Task<OrderStateOrderMapping> GetOrderStateOrderMappingByIdAsync(int id)
        {
            return await _orderStateOrderMapping.GetByIdAsync(id);
        }
        public virtual async Task<IList<OrderStateOrderMapping>> GeAllOrderStateOrderMappingByOrderIdAsync(int orderId)
        {
            return await _orderStateOrderMapping.Table.Where(x => x.OrderId == orderId).OrderBy(x=>x.InsertionDate).ToListAsync();
        }
        public virtual async Task<OrderStateOrderMapping> GeOrderStateOrderMappingAsync(OrderStatusPictureSearchModel searchModel)
        {
            return await _orderStateOrderMapping.Table.Where(x => x.PosUserId == searchModel.PosUserId && x.OrderId == searchModel.OrderId && x.OrderStatusId == searchModel.OrderStatusId).FirstOrDefaultAsync();
        }
        public virtual async Task<IList<OrderStateOrderMapping>> GeAllOrderStateOrderMappingAsync()
        {
            return await _orderStateOrderMapping.Table.OrderBy(x => x.InsertionDate).ToListAsync();
        }
        public virtual async Task<IList<AllDeportationModel>> GetAllDeportationModelByIdAsync(int posOrderId,bool skipLast = true)
        {
            var order = _posOrderService.GetNopOrderByPosOrderIdAsync(posOrderId);
            if (order == null)
            {
                throw new ArgumentNullException(nameof(CycleFlowSettingModel), $"PosOrderId  {posOrderId} not Found");
            }
            var orderMapping = await GeAllOrderStateOrderMappingByOrderIdAsync(posOrderId);
            if (orderMapping == null)
            {
                throw new ArgumentNullException(nameof(CycleFlowSettingModel), $"Thar is No Deportation to order : {posOrderId}");
            }
            List<AllDeportationModel> AllDeportationModelList = new List<AllDeportationModel>();
            foreach (var item in skipLast ? orderMapping.Take(orderMapping.Count - 1) : orderMapping)
            {
                var pictureList = await GetAllOrderStateOrderImageMappingPictureOrderStatusIdAsync(item.PosUserId,item.OrderId, item.OrderStatusId);
                var imgTypeList = new List<string>();
                foreach (var img in pictureList)
                {
                    imgTypeList.Add(
                            await _pictureService.GetPictureUrlAsync(img.PictureId) ?? string.Empty
                        );
                }
                AllDeportationModelList.Add(
                    new AllDeportationModel
                    {
                        DeportationDate = item.InsertionDate,
                        StatusName = await _orderStatusService.GetOrderStatusNameAsync(item.OrderStatusId),
                        NextStatusName =  await _orderStatusService.GetOrderStatusNameAsync((await _cycleFlowSettingService.GetNextStepByFirstStepAsync(item.OrderStatusId, item.PosUserId)??0)),
                        ImageType = imgTypeList,
                        Note = item.Note,
                        IsReturn = item.IsReturn,
                    });
            }
            return AllDeportationModelList;
        }
        public virtual async Task InsertStepAsync(DeportationModel model, Deportation deportationType)
        {
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    bool? isReturn = deportationType == Deportation.Return ? true : null;
                    if(!string.IsNullOrEmpty(model.Note))
                    {
                        var currentOrderStatusOrderMapping = await GetOrderStateOrderMappingByIdAsync(model.Id);
                        currentOrderStatusOrderMapping.Note = model.Note;
                        await currentOrderStatusOrderMapping.SetBaseInfoAsync<OrderStateOrderMapping>(_workContext);
                        await _orderStateOrderMapping.UpdateAsync(currentOrderStatusOrderMapping);
                    }
                    
                    var nextStep = deportationType == Deportation.NextStep ? model.NextOrderStatusId : model.ReturnStepId;
                    var customer = await _cycleFlowSettingService.GetCustomerByOrderStatusIdAsync(model.PosUserId, (int)nextStep!);
                    var orderStatusOrderMapping = new OrderStateOrderMapping()
                    {
                        OrderId = model.OrderId,
                        OrderStatusId = (int)nextStep!,
                        NopStoreId = model.NopStoreId,
                        PosUserId = model.PosUserId,
                        CustomerId = customer.Id,
                        ReturnOrderStatusId =  deportationType == Deportation.Return ? model.OrderStatusId : null,
                        IsReturn =  isReturn??false
                    };
                    await orderStatusOrderMapping.SetBaseInfoAsync<OrderStateOrderMapping>(_workContext);
                    await _orderStateOrderMapping.InsertAsync(orderStatusOrderMapping);
                    if (model.OrderStatusPictureModels?.Count() > 0)
                    {
                        foreach (var img in model.OrderStatusPictureModels)
                        {
                            var orderStatusOrderImageTypingMapping = await GetOrderStateOrderImageMappingByIdAsync(img.Id);
                            orderStatusOrderImageTypingMapping.OrderStatusId = model.OrderStatusId;
                            orderStatusOrderImageTypingMapping.OrderStateOrderMappingId = model.Id;
                            await UpdateOrderStateOrderImageMappingAsync(orderStatusOrderImageTypingMapping);
                        }
                    }
                    if(model.IsEnableSendToClient)
                    {
                        //await SendByWhatsApp(model.CustomerName,model.CurrentOrderStatusName!,model.ClientSmsTemplateId??0);
                    }
                    if (model.IsEnableSendToUser)
                    {
                        //await SendByWhatsApp(model.PosUserName, model.CurrentOrderStatusName!, model.UserSmsTemplateId ?? 0);
                    }
                    _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.Deportation.NextStep.Successfully"), encode: false);
                    transaction.Complete();
                }
                catch (Exception exp)
                {
                    _notificationService.ErrorNotification(exp.Message, encode: false);
                    transaction.Dispose();
                }
            }
        }
        #endregion

        public async Task SendByWhatsApp(string name,string statusName,int TemplateId)
        {
            var url = "https://api.ultramsg.com/instance104502/messages/chat";
            var client = new RestClient(url);

            var request = new RestRequest(url, Method.Post);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("token", "w67uvy5g1339gnrh");
            request.AddParameter("to", "+966538308241");
            var message = $"مرحبا {name} طلبك حاليا في مرحلة {statusName} ورقم القالب {TemplateId}";
            request.AddParameter("body", message);
            RestResponse response = await client.ExecuteAsync(request);
        }
    }
}
