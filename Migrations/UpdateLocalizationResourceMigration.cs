using Nop.Data.Migrations;
using FluentMigrator;
using Nop.Core;
using Nop.Data.Mapping;
using Nop.Plugin.Misc.POSSystem.Domains;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Data.Extensions;
using Nop.Plugin.Misc.SmsAuthentication.Domains;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Localization;

namespace Nop.Plugin.Misc.CycleFlow.Migrations
{
    [NopMigration("2025-01-20 00:02:00", "CycleFlowPlugin Update LocalizationResource", MigrationProcessType.Update)]
    public class UpdateLocalizationResourceMigration : MigrationBase
    {
        public override void Down()
        {

        }

        public async override void Up()
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

            var languageService = EngineContext.Current.Resolve<ILanguageService>();


            var lang_list = await languageService.GetAllLanguagesAsync();


            foreach (var lang in lang_list)
            {
                await localizationService.AddOrUpdateLocaleResourceAsync(CycleFlowPluginLocalizationResources.PluginResources(lang.UniqueSeoCode), languageId: lang.Id);
            }
        }
    }
}
