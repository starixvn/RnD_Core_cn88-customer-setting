namespace CN88.CustomerSetting.Models
{
    using System.Collections.Generic;
    using System.Linq;

    public class Product
    {
        internal Product(
            byte id,
            string name,
            IEnumerable<SettingType> settingTypes = null,
            bool isDisabled = false,
            bool isHidden = false)
        {
            Id = id;
            Name = name;
            SettingTypes = settingTypes ?? Enumerable.Empty<SettingType>();
            IsDisabled = isDisabled;
            IsHidden = isHidden;
        }

        public byte Id { get; private set; }

        public string Name { get; private set; }

        public bool IsDisabled { get; private set; }

        public bool IsHidden { get; private set; }

        public IEnumerable<SettingType> SettingTypes { get; private set; }
    }
}