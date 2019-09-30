using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyVet.Common.Helpers
{
    public static class Setting
    {
        private const string _pet = "Pet";
        private static readonly string _settingDefault = string.Empty;
        private static ISettings Appsetting => CrossSettings.Current;

        public static string  Pet 
        {
            get => Appsetting.GetValueOrDefault(_pet, _settingDefault);
            set => Appsetting.AddOrUpdateValue(_pet, value);
        }
    }
}
