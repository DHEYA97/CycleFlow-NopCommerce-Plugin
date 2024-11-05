using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Plugin.Misc.CycleFlow.Models.ImageType; // تعديل النموذج ليكون متعلقًا بـ ImageType
using Nop.Plugin.Misc.CycleFlow.Permission;
using Nop.Plugin.Misc.CycleFlow.Factories;
using Nop.Plugin.Misc.CycleFlow.Services;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;

namespace Nop.Plugin.Misc.CycleFlow.Controllers
{
    [Area(AreaNames.Admin)]
    [AuthorizeAdmin]
    [AutoValidateAntiforgeryToken]
    public class ImageTypeController : BasePluginController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IImageTypeModelFactory _imageTypeModelFactory;
        private readonly IImageTypeService _imageTypeService; // تغيير الخدمة إلى ImageType
        #endregion

        #region Ctor

        public ImageTypeController(IPermissionService permissionService,
            INotificationService notificationService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            IImageTypeModelFactory imageTypeModelFactory,
            IImageTypeService imageTypeService // تعديل المعاملات
            )
        {
            _permissionService = permissionService;
            _notificationService = notificationService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _imageTypeModelFactory = imageTypeModelFactory;
            _imageTypeService = imageTypeService;
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
            if (!await _permissionService.AuthorizeAsync(CycleFlowPluginPermissionProvider.ManageCycleFlowPlugin))
                return AccessDeniedView();
            // إعداد النموذج
            var model = await _imageTypeModelFactory.PrepareImageTypeSearchModelAsync(new ImageTypeSearchModel());
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(ImageTypeSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(CycleFlowPluginPermissionProvider.ManageCycleFlowPlugin))
                return await AccessDeniedDataTablesJson();

            var model = await _imageTypeModelFactory.PrepareImageTypeListModelAsync(searchModel);
            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(CycleFlowPluginPermissionProvider.ManageCycleFlowPlugin))
                return AccessDeniedView();
            var model = await _imageTypeModelFactory.PrepareImageTypeModelAsync(new ImageTypeModel(), null);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual async Task<IActionResult> Create(ImageTypeModel model, bool continueEditing, IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(CycleFlowPluginPermissionProvider.ManageCycleFlowPlugin))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var imageType = model.ToEntity<ImageType>();
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
            if (!await _permissionService.AuthorizeAsync(CycleFlowPluginPermissionProvider.ManageCycleFlowPlugin))
                return AccessDeniedView();

            var imageType = await _imageTypeService.GetImageTypeByIdAsync(id);
            if (imageType == null)
                return RedirectToAction("List");

            var model = await _imageTypeModelFactory.PrepareImageTypeModelAsync(null, imageType);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(ImageTypeModel model, bool continueEditing, IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(CycleFlowPluginPermissionProvider.ManageCycleFlowPlugin))
                return AccessDeniedView();

            var imageType = await _imageTypeService.GetImageTypeByIdAsync(model.Id);
            if (imageType == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                imageType = model.ToEntity(imageType);
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
            if (!await _permissionService.AuthorizeAsync(CycleFlowPluginPermissionProvider.ManageCycleFlowPlugin))
                return AccessDeniedView();

            var imageType = await _imageTypeService.GetImageTypeByIdAsync(id);
            if (imageType == null)
                return RedirectToAction("List");

            await _imageTypeService.DeleteImageTypeAsync(imageType);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugin.Misc.CycleFlow.ImageType.Notification.Deleted"));

            return RedirectToAction("List");
        }
        #endregion
    }
}
