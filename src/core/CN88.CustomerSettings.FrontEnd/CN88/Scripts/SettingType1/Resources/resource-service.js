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
        "MaxAllowed": "Max. Allowed:",
        "NoChange": "No Change"
    };

    labels["zh-cn"] = {
    };

    labels["zh-tw"] = {
    };

    labels["zh-hans"] = {
    };

    labels["ko-kr"] = {
    };

    labels["ja-jp"] = {
    };

    labels["th-th"] = {
    };

    var messages = {};

    messages["en-us"] = {
        "RequiredField": "This field is required.",
        "ConfirmPasswordMustMatchPassword": "Confirm Password must match Password.",
        "PasswordRules": "<div class='password-rule'><div>Password must be at least 8 characters without white-space and contain at least 3 of the following:<ul><li>Uppercase letter [A-Z]</li><li>Lowercase letter [a-z]</li><li>Numeric [0-9]</li><li>Special character (!,,#,etc..)</li></ul></div><div><span class='note'>For example:</span> 59D7!4$h, 493abcDE</div><div><span class='note'>Note:</span> your password is cAsE sEnSiTiVe</div></div>",
        "MemberPasswordRules": "<div class='password-rule'><div>Password must be 6-10 characters without white-space and contain at least 1 uppercase letter, 1 lowercase letter, 1 numeric digit. (e.g. 493abcDE, 59D7!4$h)</div><div><span class='note'>Note:</span> your password is cAsE sEnSiTiVe</div></div>",
        "InvalidPassword": "Invalid password.",
        "SpreadGroupRule": "Once the account is created, Spread Group can no longer be changed.",
        "SuperMinPTMustBeGreaterThanOrEqualTo": "SMA Min Possess must be greater than or equal to {0}.",
        "SuperMinPTMustBeLessThanOrEqualTo": "SMA Min Possess must be less than or equal to {0}.",
        "SuperMaxPTMustBeGreaterThanOrEqualTo": "SMA Max Possess must be greater than or equal to {0}.",
        "SuperMaxPTMustBeLessThanOrEqualTo": "SMA Max Possess must be less than or equal to {0}.",
        "SuperMinPTMustBeLessThanOrEqualToSuperMaxPT": "SMA Min Possess must be less than or equal to SMA Max Possess.",
        "SuperPTMustBeGreaterThanOrEqualTo": "SMA Possess must be greater than or equal to {0}.",
        "SuperPTMustBeLessThanOrEqualTo": "SMA Possess must be less than or equal to {0}.",
        "MasterPTMustBeGreaterThanOrEqualTo": "Master Agent Possess must be greater than or equal to {0}.",
        "MasterPTMustBeLessThanOrEqualTo": "Master Agent Possess must be less than or equal to {0}.",
        "AgentPTMustBeGreaterThanOrEqualTo": "Agent Possess must be greater than or equal to {0}.",
        "AgentPTMustBeLessThanOrEqualTo": "Agent Possess must be less than or equal to {0}.",
        "TotalPTMustBeInRange": "Total Possess must be between {0}% and {1}%. Please try again.",
        "PTEffectiveTime": "Effective after: {0}",
        "CreditsMustBeGreaterThanOrEqualTo": "Credits must be greater than or equal to {0}.",
        "CreditsMustBeLessThanOrEqualTo": "Credits must be less than or equal to {0}.",
        "InvalidHeadline": "Invalid headline."
    };

    messages["zh-cn"] = {
    };

    messages["zh-tw"] = {
    };

    messages["zh-hans"] = {
    };

    messages["ko-kr"] = {
    };

    messages["ja-jp"] = {
    };

    messages["th-th"] = {
    };

    var settingType1Resources = {
        "labels": labels,
        "messages": messages
    };

    angular.module("customerSetting").value("st1Resources", settingType1Resources);
})();