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
namespace Nop.Plugin.Misc.CycleFlow.Controllers
{
    [Area(AreaNames.Admin)]
    [AuthorizeAdmin]
    [AutoValidateAntiforgeryToken]
    public class OrderStatusController : BasePluginController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IOrderStatusModelFactory _orderStatusModelFactory;
        private readonly IOrderStatusService _orderStatusService;
        protected readonly IWorkContext _workContext;
        #endregion

        #region Ctor

        public OrderStatusController(IPermissionService permissionService,
            INotificationService notificationService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            IOrderStatusModelFactory orderStatusModelFactory,
            IOrderStatusService orderStatusService,
            IWorkContext workContext
            )
        {
            _permissionService = permissionService;
            _notificationService = notificationService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _orderStatusModelFactory = orderStatusModelFactory;
            _orderStatusService = orderStatusService;
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
            if (!await _permissionService.AuthorizeAsync(CycleFlowPluginPermissionProvider.ManageCycleFlowPlugin))
                return AccessDeniedView();
            //prepare model
            var model = await _orderStatusModelFactory.PrepareOrderStatusSearchModelAsync(new OrderStatusSearchModel());
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(OrderStatusSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(CycleFlowPluginPermissionProvider.ManageCycleFlowPlugin))
                return await AccessDeniedDataTablesJson();

            var model = await _orderStatusModelFactory.PrepareOrderStatusListModelAsync(searchModel);
            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(CycleFlowPluginPermissionProvider.ManageCycleFlowPlugin))
                return AccessDeniedView();
            var model = await _orderStatusModelFactory.PrepareOrderStatusModelAsync(new OrderStatusModel(), null);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual async Task<IActionResult> Create(OrderStatusModel model, bool continueEditing, IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(CycleFlowPluginPermissionProvider.ManageCycleFlowPlugin))
                return AccessDeniedView();

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
            if (!await _permissionService.AuthorizeAsync(CycleFlowPluginPermissionProvider.ManageCycleFlowPlugin))
                return AccessDeniedView();

            var orderStatus = await _orderStatusService.GetOrderStatusByIdAsync(id);
            if (orderStatus == null)
                return RedirectToAction("List");

            var model = await _orderStatusModelFactory.PrepareOrderStatusModelAsync(null, orderStatus);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(OrderStatusModel model, bool continueEditing, IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(CycleFlowPluginPermissionProvider.ManageCycleFlowPlugin))
                return AccessDeniedView();

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
            if (!await _permissionService.AuthorizeAsync(CycleFlowPluginPermissionProvider.ManageCycleFlowPlugin))
                return AccessDeniedView();

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
