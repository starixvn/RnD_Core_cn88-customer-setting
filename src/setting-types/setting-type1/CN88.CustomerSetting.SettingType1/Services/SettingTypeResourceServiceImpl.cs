namespace CN88.CustomerSetting.SettingType1.Services
{
    using CN88.CustomerSetting.Services;
    using Fanex.RemotingRef;

    public class SettingTypeResourceServiceImpl : ResourceServiceImpl
    {
        public SettingTypeResourceServiceImpl(IRemotingRefService remotingRef)
            : base(remotingRef)
        {
        }

        public override string GetSportTypeName(int sportTypeId, string language)
        {
            var sportTypeName = AppResources.Resources.ResourceManager.GetString($"SportTypeName_{sportTypeId}");

            if(!string.IsNullOrWhiteSpace(sportTypeName))
            {
                return sportTypeName;
            }

            return base.GetSportTypeName(sportTypeId, language);
        }
    }
}