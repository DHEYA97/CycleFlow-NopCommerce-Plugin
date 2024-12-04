using FluentMigrator;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.CycleFlow.Migrations
{
    [NopMigration("2024-11-03 00:00:00", "CycleFlowPlugin LocalizationResource", MigrationProcessType.Installation)]
    public class InstallLocalizationResourceMigration : MigrationBase
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
