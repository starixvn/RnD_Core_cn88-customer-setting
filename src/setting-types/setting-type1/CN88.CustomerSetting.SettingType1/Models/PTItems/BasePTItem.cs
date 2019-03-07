namespace CN88.CustomerSetting.SettingType1.Models.PTItems
{
    using CN88.CustomerSetting.Models;

    public class BasePTItem : SettingItemDefault
    {
        public virtual void AdjustPTForQuery(bool isCreateNew)
        {
        }

        public virtual bool IsNoChangePT() => true;
    }
}