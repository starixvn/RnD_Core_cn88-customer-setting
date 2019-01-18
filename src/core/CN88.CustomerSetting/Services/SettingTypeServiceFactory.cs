namespace CN88.CustomerSetting.Services
{
    using System;
    using System.Collections.Concurrent;
    using Models;

    public interface SettingTypeServiceFactory
    {
        SettingTypeService GetInstance(SettingType settingType);
    }

    public class SettingTypeServiceFactoryImpl : SettingTypeServiceFactory
    {
        private const string AssemblyNameFormat = "CN88.CustomerSetting.SettingType{0}";
        private const string TypeNameFormat = "{0}.Services.SettingTypeServiceImpl";

        private static ConcurrentDictionary<string, SettingTypeService> serviceInstanceCached = new ConcurrentDictionary<string, SettingTypeService>();

        public SettingTypeService GetInstance(SettingType settingType)
        {
            var settingTypeService = GetInstanceFromCache(settingType);

            if (settingTypeService == null)
            {
                settingTypeService = CreateInstanceAndCache(settingType);
            }

            return settingTypeService;
        }

        private static SettingTypeService GetInstanceFromCache(SettingType settingType)
        {
            SettingTypeService settingTypeService = null;

            serviceInstanceCached.TryGetValue(settingType.ToString(), out settingTypeService);

            return settingTypeService;
        }

        private static SettingTypeService CreateInstanceAndCache(SettingType settingType)
        {
            SettingTypeService settingTypeService = null;
            var fullTypeName = GetFullTypeServiceName(settingType);
            Type settingTypeServiceType = Type.GetType(fullTypeName);

            if (settingTypeServiceType == null)
            {
                throw new DllNotFoundException(string.Format("{0} not found", fullTypeName));
            }

            settingTypeService = Activator.CreateInstance(settingTypeServiceType) as SettingTypeService;

            serviceInstanceCached.TryAdd(settingType.ToString(), settingTypeService);

            return settingTypeService;
        }

        private static string GetFullTypeServiceName(SettingType settingType)
        {
            var assemblyName = string.Format(AssemblyNameFormat, settingType.Id);
            var typeName = string.Format(TypeNameFormat, assemblyName);
            return string.Join(", ", typeName, assemblyName);
        }
    }
}