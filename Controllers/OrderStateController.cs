using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Plugin.Misc.CycleFlow.Models.OrderStatus;
using Nop.Plugin.Misc.CycleFlow.Permission;
using Nop.Plugin.Misc.CycleFlow.Factories;
using Nop.Plugin.Misc.CycleFlow.Services;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Core;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Plugin.Misc.CycleFlow.Constant;
namespace Nop.Plugin.Misc.CycleFlow.Controllers
{
    public class OrderStatusController : BaseCycleFlowController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly INotificationService _notificationService;
        private readonly IOrderStatusModelFactory _orderStatusModelFactory;
        private readonly IOrderStatusService _orderStatusService;
        private readonly IPermissionService _permissionService;
        protected readonly IWorkContext _workContext;
        #endregion

        #region Ctor

        public OrderStatusController(
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            INotificationService notificationService,
            IOrderStatusModelFactory orderStatusModelFactory,
            IOrderStatusService orderStatusService,
            IPermissionService permissionService,
            IWorkContext workContext
            )
        {
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _notificationService = notificationService;
            _orderStatusModelFactory = orderStatusModelFactory;
            _orderStatusService = orderStatusService;
            _permissionService = permissionService;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateLocalesAsync(OrderStatus orderStatus, OrderStatusModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(orderStatus,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValueAsync(orderStatus,
                    x => x.Description,
                    localized.Description,
                    localized.LanguageId);
            }
        }

        #endregion

        #region Methods
        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            var result = await CheckPermissionAndRoleAsync(CycleFlowPluginPermissionProvider.ManageCycleFlowPlugin, SystemDefaults.CYCLE_FLOW_USER_ROLE_SYSTEM_NAME);
            if (result != null)
                return result;

            var model = await _orderStatusModelFactory.PrepareOrderStatusSearchModelAsync(new OrderStatusSearchModel());
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(OrderStatusSearchModel searchModel)
        {
            var result = await CheckPermissionAndRoleAsync(CycleFlowPluginPermissionProvider.ManageCycleFlowPlugin, SystemDefaults.CYCLE_FLOW_USER_ROLE_SYSTEM_NAME,true);
            if (result != null)
                return result;

            var model = await _orderStatusModelFactory.PrepareOrderStatusListModelAsync(searchModel);
            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            var result = await CheckPermissionAndRoleAsync(CycleFlowPluginPermissionProvider.ManageCycleFlowPlugin, SystemDefaults.CYCLE_FLOW_USER_ROLE_SYSTEM_NAME);
            if (result != null)
                return result;

            var model = await _orderStatusModelFactory.PrepareOrderStatusModelAsync(new OrderStatusModel(), null);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual async Task<IActionResult> Create(OrderStatusModel model, bool continueEditing, IFormCollection form)
        {
            var result = await CheckPermissionAndRoleAsync(CycleFlowPluginPermissionProvider.ManageCycleFlowPlugin, SystemDefaults.CYCLE_FLOW_USER_ROLE_SYSTEM_NAME);
            if (result != null)
                return result;

            if (ModelState.IsValid)
            {
                var orderStatus = model.ToEntity<OrderStatus>();
                await orderStatus.SetBaseInfoAsync<OrderStatus>(_workContext);
                await _orderStatusService.InsertOrderStatusAsync(orderStatus);
                await UpdateLocalesAsync(orderStatus, model);
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugin.Misc.CycleFlow.OrderStatus.Notification.Added"));
                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = orderStatus.Id });
            }
            model = await _orderStatusModelFactory.PrepareOrderStatusModelAsync(model, null, true);
            return View(model);
        }
        public virtual async Task<IActionResult> Edit(int id)
        {
            var result = await CheckPermissionAndRoleAsync(CycleFlowPluginPermissionProvider.ManageCycleFlowPlugin, SystemDefaults.CYCLE_FLOW_USER_ROLE_SYSTEM_NAME);
            if (result != null)
                return result;

            var orderStatus = await _orderStatusService.GetOrderStatusByIdAsync(id);
            if (orderStatus == null)
                return RedirectToAction("List");

            var model = await _orderStatusModelFactory.PrepareOrderStatusModelAsync(null, orderStatus);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(OrderStatusModel model, bool continueEditing, IFormCollection form)
        {
            var result = await CheckPermissionAndRoleAsync(CycleFlowPluginPermissionProvider.ManageCycleFlowPlugin, SystemDefaults.CYCLE_FLOW_USER_ROLE_SYSTEM_NAME);
            if (result != null)
                return result;

            var orderStatus = await _orderStatusService.GetOrderStatusByIdAsync(model.Id);
            if (orderStatus == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {

                orderStatus = model.ToEntity(orderStatus);
                var check = await orderStatus.SetBaseInfoAsync<OrderStatus>(_workContext);
                if (!check.success)
                {
                    _notificationService.ErrorNotification(check.message);

                    return View("Edit", model);
                }
                await _orderStatusService.UpdateOrderStatusAsync(orderStatus);

                await UpdateLocalesAsync(orderStatus, model);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugin.Misc.CycleFlow.OrderStatus.Notification.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = orderStatus.Id });
            }
            model = await _orderStatusModelFactory.PrepareOrderStatusModelAsync(model, orderStatus, true);
            return View(model);
        }
        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            var result = await CheckPermissionAndRoleAsync(CycleFlowPluginPermissionProvider.ManageCycleFlowPlugin, SystemDefaults.CYCLE_FLOW_USER_ROLE_SYSTEM_NAME);
            if (result != null)
                return result;
            
            var orderStatus = await _orderStatusService.GetOrderStatusByIdAsync(id);
            if (orderStatus == null)
                return RedirectToAction("List");
            await orderStatus.SetBaseInfoAsync<OrderStatus>(_workContext);
            await _orderStatusService.DeleteOrderStatusAsync(orderStatus);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugin.Misc.CycleFlow.OrderStatus.Notification.Deleted"));

            return RedirectToAction("List");
        }
        #endregion
    }
}
