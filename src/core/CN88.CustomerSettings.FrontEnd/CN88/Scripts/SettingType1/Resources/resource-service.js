/*jshint -W101,-W106,-W044,-W098*/

(function () {
    var labels = {};

    labels["en-us"] = {
        "Headline": "Headline",
        "Master": "Master Agent",
        "Agent": "Agent",
        "AccountDetail": "Account Detail",
        "GeneralData": "General Data",
        "Name": "Name",
        "Username": "Username",
        "Password": "Password",
        "ConfirmPassword": "Confirm Password",
        "FirstName": "First Name",
        "LastName": "Last Name",
        "Phone": "Phone",
        "MobilePhone": "Mobile Phone",
        "Fax": "Fax",
        "SpreadGroup": "Spread Group",
        "Credit": "Credit",
        "Credits": "Credits",
        "AvailableCreditRange": "Available Credit Range",
        "Currency": "Currency",
        "PositionTaking": "Possess",
        "SuperMinPT": "SMA Min Possess",
        "SuperMaxPT": "SMA Max Possess",
        "SuperPT": "SMA Possess",
        "MasterPT": "Master Agent Possess",
        "AgentPT": "Agent Possess",
        "MinAllowed": "Min. Allowed:",
        "MaxAllowed": "Max. Allowed:"
    };

    labels["zh-hans"] = {
        "Headline": "股东",
        "Master": "总代理",
        "Agent": "代理商",
        "AccountDetail": "帐户详细设定",
        "GeneralData": "基本资料",
        "Name": "名称",
        "Username": "帐户",
        "Password": "密码",
        "ConfirmPassword": "确认密码",
        "FirstName": "名字",
        "LastName": "姓",
        "Phone": "电话",
        "MobilePhone": "移动电话",
        "Fax": "传真",
        "SpreadGroup": "盘口种类",
        "Credit": "信用额度",
        "Credits": "信用额度",
        "AvailableCreditRange": "剩余额度",
        "Currency": "货币",
        "PositionTaking": "占成数",
        "SuperMinPT": "超级代理最低占成数",
        "SuperMaxPT": "超级代理最高占成数",
        "SuperPT": "超级代理占成数",
        "MasterPT": "总代理占成数",
        "AgentPT": "代理商占成数",
        "MinAllowed": "最低可设:",
        "MaxAllowed": "最高可设:"
    };

    labels["zh-tw"] = {
        "Headline": "股東",
        "Master": "總代理",
        "Agent": "代理商",
        "AccountDetail": "帳戶詳細設定",
        "GeneralData": "基本資料",
        "Name": "名稱",
        "Username": "帳戶",
        "Password": "密碼",
        "ConfirmPassword": "確認密碼",
        "FirstName": "名字",
        "LastName": "姓",
        "Phone": "電話",
        "MobilePhone": "行動電話",
        "Fax": "傳真",
        "SpreadGroup": "盤口種類",
        "Credit": "信用額度",
        "Credits": "信用額度",
        "AvailableCreditRange": "剩餘額度",
        "Currency": "貨幣",
        "PositionTaking": "佔成數",
        "SuperMinPT": "超級代理最低佔成數",
        "SuperMaxPT": "超級代理最高佔成數",
        "SuperPT": "超級代理佔成數",
        "MasterPT": "總代理佔成數",
        "AgentPT": "代理商佔成數",
        "MinAllowed": "最低可設:",
        "MaxAllowed": "最高可設:"
    };

    var messages = {};

    messages["en-us"] = {
        "RequiredField": "This field is required.",
        "ConfirmPasswordMustMatchPassword": "Confirm Password must match Password.",
        "PasswordRules": "<div class='password-rule'><div>Password must be at least 8 characters without white-space and contain at least 3 of the following:<ul><li>Uppercase letter [A-Z]</li><li>Lowercase letter [a-z]</li><li>Numeric [0-9]</li><li>Special character (!,,#,etc..)</li></ul></div><div><span class='note'>For example:</span> 59D7!4$h, 493abcDE</div><div><span class='note'>Note:</span> your password is cAsE sEnSiTiVe</div></div>",
        "MemberPasswordRules": "<div class='password-rule'><div>Password must be 6-10 characters without white-space and contain at least 1 uppercase letter, 1 lowercase letter, 1 numeric digit. (e.g. 493abcDE, 59D7!4$h)</div><div><span class='note'>Note:</span> your password is cAsE sEnSiTiVe</div></div>",
        "InvalidPassword": "Invalid password.",
        "SpreadGroupRule": "Once the account is created, Spread Group can no longer be changed.",
        "SuperMinPTIsOutOfRange": "SMA Min Possess is out of range.",
        "SuperMaxPTIsOutOfRange": "SMA Max Possess is out of range.",
        "SuperMinPTMustBeLessThanOrEqualToSuperMaxPT": "SMA Min Possess must be less than or equal to SMA Max Possess.",
        "SuperPTIsOutOfRange": "SMA Possess is out of range.",
        "MasterPTIsOutOfRange": "Master Agent Possess is out of range.",
        "AgentPTIsOutOfRange": "Agent Possess is out of range.",
        "TotalPTMustBeInRange": "Total Possess must be between {0}% and {1}%. Please try again.",
        "TotalPTRangeConflict": "Conflict between current possess and new possess values. Please try again after the new possess value takes effect.",
        "PTEffectiveTime": "Effective after: {0}",
        "CreditsMustBeGreaterThanOrEqualTo": "Credits must be greater than or equal to {0}.",
        "CreditsMustBeLessThanOrEqualTo": "Credits must be less than or equal to {0}.",
        "InvalidHeadline": "Invalid headline."
    };

    messages["zh-hans"] = {
        "RequiredField": "必填字段",
        "ConfirmPasswordMustMatchPassword": "确认密码必须与密码匹配。",
        "PasswordRules": "<div class='password-rule'><div>密码必须至少为8个字符且没有空格，并且至少包含以下3个条件：<ul><li>大写字母[A-Z]</li><li>小写字母[a-z]</li><li>数字[0-9]</li><li>特殊字符（!,,#等...）</li></ul></div><div><span class='note'>例如：</span> 59D7!4$h, 493abcDE</div><div><span class='note'>注意：</span> 您的密码输入的时候要注意字母的大小写</div></div>",
        "MemberPasswordRules": "<div class='password-rule'><div>密码必须是6-10个字符，没有空格，并且包含至少1个大写字母，1个小写字母，1个数字。（例如493abcDE，59D7!4$h）</div><div><span class='note'>注意：</span> 您的密码输入的时候要注意字母的大小写</div></div>",
        "InvalidPassword": "无效的密码",
        "SpreadGroupRule": "帐号一旦新增后, 此项目无法更改。",
        "SuperMinPTIsOutOfRange": "超级代理最低占成数超出范围。",
        "SuperMaxPTIsOutOfRange": "超级代理最高占成数超出范围。",
        "SuperMinPTMustBeLessThanOrEqualToSuperMaxPT": "超级代理最低占成数必须小于或等于超级代理最高占成数",
        "SuperPTIsOutOfRange": "超级代理占成数超出范围。",
        "MasterPTIsOutOfRange": "总代理占成数超出范围。",
        "AgentPTIsOutOfRange": "代理商占成数超出范围。",
        "TotalPTMustBeInRange": "总占成数必须在{0}%和 {1}%之间。请重试",
        "TotalPTRangeConflict": "占成数数值冲突。请在新占成数生效后重试。",
        "PTEffectiveTime": "{0} 后生效",
        "CreditsMustBeGreaterThanOrEqualTo": "信用额度必须大于或等于{0}。",
        "CreditsMustBeLessThanOrEqualTo": "信用额度必须小于或等于{0}。",
        "InvalidHeadline": "无效的股东。"
    };

    messages["zh-tw"] = {
        "RequiredField": "必填欄位",
        "ConfirmPasswordMustMatchPassword": "確認密碼必須與密碼相符。",
        "PasswordRules": "<div class='password-rule'><div>密碼必須至少為8個字元且沒有空格，並且至少包含以下3個條件：<ul><li>大寫字母[A-Z]</li><li>小寫字母[a-z]</li><li>數字[0-9]</li><li>特殊字元（!,,#等...）</li></ul></div><div><span class='note'>例如：</span> 59D7!4$h, 493abcDE</div><div><span class='note'>注意：</span> 您的密碼輸入的時候要注意字母的大小寫</div></div>",
        "MemberPasswordRules": "<div class='password-rule'><div>密碼必須是6-10個字元，沒有空格，並且包含至少1個大寫字母，1個小寫字母，1個數字。 （例如493abcDE，59D7!4$h）</div><div><span class='note'>注意：</span> 您的密碼輸入的時候要注意字母的大小寫</div></div>",
        "InvalidPassword": "無效的密碼",
        "SpreadGroupRule": "帳號一旦新增後, 此項目即無法更改。",
        "SuperMinPTIsOutOfRange": "超級代理最低佔成數超出範圍。",
        "SuperMaxPTIsOutOfRange": "超級代理最高佔成數超出範圍。",
        "SuperMinPTMustBeLessThanOrEqualToSuperMaxPT": "超級代理最低佔成數必須小於或等於超級代理最高佔成數",
        "SuperPTIsOutOfRange": "超級代理佔成數超出範圍。",
        "MasterPTIsOutOfRange": "總代理佔成數超出範圍。",
        "AgentPTIsOutOfRange": "代理商佔成數超出範圍。",
        "TotalPTMustBeInRange": "總佔成數必須在{0}%和 {1}%之間。請重試",
        "TotalPTRangeConflict": "佔成數數值衝突。請在新佔成數生效後重試。",
        "PTEffectiveTime": "{0} 後生效",
        "CreditsMustBeGreaterThanOrEqualTo": "信用額度必須大於或等於{0}。",
        "CreditsMustBeLessThanOrEqualTo": "信用額度必須小於或等於{0}。",
        "InvalidHeadline": "無效的股東。"
    };

    var settingType1Resources = {
        "labels": labels,
        "messages": messages
    };

    angular.module("customerSetting").value("st1Resources", settingType1Resources);
})();