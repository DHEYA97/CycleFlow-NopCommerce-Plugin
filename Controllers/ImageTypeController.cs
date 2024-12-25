using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Plugin.Misc.CycleFlow.Models.ImageType;
using Nop.Plugin.Misc.CycleFlow.Permission;
using Nop.Plugin.Misc.CycleFlow.Factories;
using Nop.Plugin.Misc.CycleFlow.Services;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Core;
using Nop.Plugin.Misc.Accounting.Domain;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Plugin.Misc.CycleFlow.Constant;

namespace Nop.Plugin.Misc.CycleFlow.Controllers
{
    public class ImageTypeController : BaseCycleFlowController
    {
        #region Fields

        private readonly IImageTypeService _imageTypeService;
        private readonly IImageTypeModelFactory _imageTypeModelFactory;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        protected readonly IWorkContext _workContext;
        #endregion

        #region Ctor

        public ImageTypeController(
            IImageTypeModelFactory imageTypeModelFactory,
            IImageTypeService imageTypeService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IWorkContext workContext
            )
        {
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _imageTypeModelFactory = imageTypeModelFactory;
            _imageTypeService = imageTypeService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _workContext = workContext;
        
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateLocalesAsync(ImageType imageType, ImageTypeModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(imageType,
                    x => x.Name,
                    localized.Name,
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
            
            var model = await _imageTypeModelFactory.PrepareImageTypeSearchModelAsync(new ImageTypeSearchModel());
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(ImageTypeSearchModel searchModel)
        {
            var result = await CheckPermissionAndRoleAsync(CycleFlowPluginPermissionProvider.ManageCycleFlowPlugin, SystemDefaults.CYCLE_FLOW_USER_ROLE_SYSTEM_NAME,true);
            if (result != null)
                return result;

            var model = await _imageTypeModelFactory.PrepareImageTypeListModelAsync(searchModel);
            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            var result = await CheckPermissionAndRoleAsync(CycleFlowPluginPermissionProvider.ManageCycleFlowPlugin, SystemDefaults.CYCLE_FLOW_USER_ROLE_SYSTEM_NAME);
            if (result != null)
                return result;

            var model = await _imageTypeModelFactory.PrepareImageTypeModelAsync(new ImageTypeModel(), null);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual async Task<IActionResult> Create(ImageTypeModel model, bool continueEditing, IFormCollection form)
        {
            var result = await CheckPermissionAndRoleAsync(CycleFlowPluginPermissionProvider.ManageCycleFlowPlugin, SystemDefaults.CYCLE_FLOW_USER_ROLE_SYSTEM_NAME);
            if (result != null)
                return result;


            if (ModelState.IsValid)
            {
                var imageType = model.ToEntity<ImageType>();
                await imageType.SetBaseInfoAsync<ImageType>(_workContext);
                await _imageTypeService.InsertImageTypeAsync(imageType);
                await UpdateLocalesAsync(imageType, model);
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugin.Misc.CycleFlow.ImageType.Notification.Added"));
                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = imageType.Id });
            }
            model = await _imageTypeModelFactory.PrepareImageTypeModelAsync(model, null, true);
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            var result = await CheckPermissionAndRoleAsync(CycleFlowPluginPermissionProvider.ManageCycleFlowPlugin, SystemDefaults.CYCLE_FLOW_USER_ROLE_SYSTEM_NAME);
            if (result != null)
                return result;


            var imageType = await _imageTypeService.GetImageTypeByIdAsync(id);
            if (imageType == null)
                return RedirectToAction("List");

            var model = await _imageTypeModelFactory.PrepareImageTypeModelAsync(null, imageType);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(ImageTypeModel model, bool continueEditing, IFormCollection form)
        {
            var result = await CheckPermissionAndRoleAsync(CycleFlowPluginPermissionProvider.ManageCycleFlowPlugin, SystemDefaults.CYCLE_FLOW_USER_ROLE_SYSTEM_NAME);
            if (result != null)
                return result;


            var imageType = await _imageTypeService.GetImageTypeByIdAsync(model.Id);
            if (imageType == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                imageType = model.ToEntity(imageType);
                var check = await imageType.SetBaseInfoAsync<ImageType>(_workContext);
                if (!check.success)
                {
                    _notificationService.ErrorNotification(check.message);

                    return View("Edit", model);
                }
                await _imageTypeService.UpdateImageTypeAsync(imageType);

                await UpdateLocalesAsync(imageType, model);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugin.Misc.CycleFlow.ImageType.Notification.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = imageType.Id });
            }
            model = await _imageTypeModelFactory.PrepareImageTypeModelAsync(model, imageType, true);
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            var result = await CheckPermissionAndRoleAsync(CycleFlowPluginPermissionProvider.ManageCycleFlowPlugin, SystemDefaults.CYCLE_FLOW_USER_ROLE_SYSTEM_NAME);
            if (result != null)
                return result;


            var imageType = await _imageTypeService.GetImageTypeByIdAsync(id);
            if (imageType == null)
                return RedirectToAction("List");

            await imageType.SetBaseInfoAsync<ImageType>(_workContext);
            await _imageTypeService.DeleteImageTypeAsync(imageType);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugin.Misc.CycleFlow.ImageType.Notification.Deleted"));

            return RedirectToAction("List");
        }
        #endregion
    }
}
