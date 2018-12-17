namespace CN88.CustomerSetting.Models.Enumerations
{
    using System;

    [Serializable]
    public class Level : Enumeration
    {
        public static readonly Level Company = new Level(5, "Company");
        public static readonly Level Super = new Level(4, "Super");
        public static readonly Level Master = new Level(3, "Master");
        public static readonly Level Agent = new Level(2, "Agent");
        public static readonly Level Member = new Level(1, "Member");

        public Level()
        {
        }

        private Level(byte value, string displayName)
            : base(value, displayName)
        {
        }

        public bool IsMemberLevel()
        {
            return this == Member;
        }

        public bool IsNotMemberLevel()
        {
            return !IsMemberLevel();
        }

        public bool IsCompanyLevel()
        {
            return this == Company;
        }

        public bool IsNotCompanyLevel()
        {
            return !IsCompanyLevel();
        }

        public bool IsAgentLevel()
        {
            return this == Agent;
        }

        public bool IsNotAgentLevel()
        {
            return !IsAgentLevel();
        }

        public bool IsMasterLevel()
        {
            return this == Master;
        }

        public bool IsNotMasterLevel()
        {
            return !IsMasterLevel();
        }

        public bool IsSuperLevel()
        {
            return this == Super;
        }

        public bool IsNotSuperLevel()
        {
            return !IsSuperLevel();
        }

        public bool IsDirectUplineOf(byte downlineLevelValue)
        {
            return GapBetweenTwoLevels(Value, downlineLevelValue) == 1;
        }

        public bool IsNotDirectUplineOf(byte downlineLevelValue)
        {
            return !IsDirectUplineOf(downlineLevelValue);
        }

        public bool IsDirectUplineOf(Enumeration customerLevel)
        {
            if (customerLevel == null)
            {
                return false;
            }

            return IsDirectUplineOf(customerLevel.Value);
        }

        public bool IsNotDirectUplineOf(Enumeration customerLevel)
        {
            return !IsDirectUplineOf(customerLevel);
        }

        public bool IsDirectDownlineOf(Enumeration uplineLevel)
        {
            if (uplineLevel == null)
            {
                return false;
            }

            return GapBetweenTwoLevels(uplineLevel.Value, Value) == 1;
        }

        public bool IsNotDirectDownlineOf(Enumeration uplineLevel)
        {
            return !IsDirectDownlineOf(uplineLevel);
        }

        public bool Is(Enumeration level)
        {
            if (level != null)
            {
                return this == level;
            }

            return false;
        }

        public bool IsNot(Enumeration level)
        {
            return !Is(level);
        }

        private static int GapBetweenTwoLevels(byte uplineLevelValue, byte downlineLevelValue)
        {
            return uplineLevelValue - downlineLevelValue;
        }
    }
}
