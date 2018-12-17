namespace CN88.CustomerSetting.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Enumerations;
    using Results;

    [Serializable]
    public class SettingType
    {
        private const string SettingTypeXmlFormat = "<ROOT>{0}</ROOT>";
        private IEnumerable<ISettingItem> settingItems;

        public short Id { get; set; }

        public string Name { get; set; }

        public byte SettingLevel { get; set; }

        public byte DisplayOrder { get; set; }

        public byte DisplayMode { get; set; }

        public bool IsError { get; set; }

        public IEnumerable<ISettingItem> SettingItems
        {
            get
            {
                if (settingItems == null)
                {
                    settingItems = new List<ISettingItem>();
                }

                return settingItems;
            }

            set => settingItems = value;
        }

        /// <summary>
        /// For Controller create exactly concrete class when get json from client.
        /// </summary>
        public string TypeName => GetType().AssemblyQualifiedName;

        public virtual bool SatisfyCustomerLevelConstraint(Level actorLevel, Level customerLevel)
            => actorLevel.IsCompanyLevel() || actorLevel.IsDirectUplineOf(customerLevel);

        public override string ToString()
            => string.Join(base.ToString(), Id);

        public virtual string ToXml()
        {
            var settingItemsXml = string.Join(string.Empty, SettingItems.Select(settingItem => settingItem.ToXml()));

            return string.Format(SettingTypeXmlFormat, settingItemsXml);
        }

        public virtual IResult Validate()
        {
            if (!SettingItems.Any())
            {
                return Result.NoChange;
            }

            var failedResult = GetFailedResult();

            return failedResult ?? Result.Success;
        }

        private IResult GetFailedResult()
        {
            foreach (var settingItem in SettingItems)
            {
                var result = settingItem.Validate();

                if (result.IsNotOk)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
