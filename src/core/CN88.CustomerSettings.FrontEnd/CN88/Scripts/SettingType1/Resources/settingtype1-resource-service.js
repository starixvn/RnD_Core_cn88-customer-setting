/*jshint -W101,-W106,-W044,-W098*/

(function () {
    "use strict";

    var labels = {};

    labels["en-us"] = {
        "Master": "Master",
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
        "AvailableCredit": "Available Credit",
        "Currency": "Currency",
        "PositionTaking": "Position Taking",
        "SuperMinPT": "Super Min PT",
        "SuperMaxPT": "Super Max PT",
        "SuperPT": "Super PT",
        "MasterPT": "Master PT",
        "AgentPT": "Agent PT",
        "MinAllowed": "Min. Allowed:",
        "MaxAllowed": "Max. Allowed:"
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
        "InvalidPassword": "Invalid password.",
        "SpreadGroupRule": "Once the account is created, Spread Group can no longer be changed.",
        "SuperMinPTMustBeGreaterThanOrEqualTo": "Super Min PT must be greater than or equal to {0}.",
        "SuperMinPTMustBeLessThanOrEqualTo": "Super Min PT must be less than or equal to {0}.",
        "SuperMaxPTMustBeGreaterThanOrEqualTo": "Super Max PT must be greater than or equal to {0}.",
        "SuperMaxPTMustBeLessThanOrEqualTo": "Super Max PT must be less than or equal to {0}.",
        "SuperMinPTMustBeLessThanOrEqualToSuperMaxPT": "Super Min PT must be less than or equal to Super Max PT.",
        "SuperPTMustBeGreaterThanOrEqualTo": "Super PT must be greater than or equal to {0}.",
        "SuperPTMustBeLessThanOrEqualTo": "Super PT must be less than or equal to {0}.",
        "MasterPTMustBeLessThanOrEqualTo": "Master PT must be greater than or equal to {0}.",
        "AgentPTMustBeLessThanOrEqualTo": "Agent PT must be less than or equal to {0}.",
        "TotalPTMustBeInRange": "Total Position Taking must be between {0}% and {1}%. Please try again.",
        "CreditMustBeLessThanOrEqualTo": "Credit must be less than or equal to {0}.",
        "InvalidCurrency": "Invalid currency."
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

    angular.module("customerSetting").value("settingType1Resources", settingType1Resources);
})();