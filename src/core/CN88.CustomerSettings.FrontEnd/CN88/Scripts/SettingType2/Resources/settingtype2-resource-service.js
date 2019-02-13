/*jshint -W101,-W106,-W044,-W098*/

(function () {
    "use strict";

    var labels = {};

    labels["en-us"] = {
        "CommissionAndBetLimit": "Commission & Bet Limit",
        "MaxBet": "Max per Bet",
        "MaxPerEvent": "Max per Event",
        "MemberCommission": "Member Commission"
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
        "CommissionIsOutOfRange": "Commission is out of range.",
        "MaxBetNote": "Maximum bet for each wager.",
        "MaxBetMustBeGreaterThanOrEqualTo": "Max per Bet must be greater than or equal to {0}.",
        "MaxBetMustBeLessThanOrEqualTo": "Max per Bet must be less than or equal to {0}.",
        "MaxBetMustBeLessThanOrEqualToMaxPerEvent": "Max per Bet must be less than or equal to Max per Event.",
        "MaxPerEventNote": "Maximum bet amount will vary depending on the bet type and event ID, except for Odd/Even.",
        "MaxPerEventMustBeGreaterThanOrEqualTo": "Max per Event must be greater than or equal to {0}.",
        "MaxPerEventMustBeLessThanOrEqualTo": "Max per Event must be less than or equal to {0}."
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

    var settingType2Resources = {
        "labels": labels,
        "messages": messages
    };

    angular.module("customerSetting").value("settingType2Resources", settingType2Resources);
})();