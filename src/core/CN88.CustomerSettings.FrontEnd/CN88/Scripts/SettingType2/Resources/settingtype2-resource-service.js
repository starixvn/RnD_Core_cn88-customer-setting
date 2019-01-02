/*jshint -W101,-W106,-W044,-W098*/

(function () {
    "use strict";

    var labels = {};

    labels["en-us"] = {
        "CommissionAndBetLimit": "Commission & Bet Limit",
        "MaxBet": "Max Bet",
        "MaxPerMatch": "Max per Match",
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
        "CommissionIsOutOfRange": "Commission is out of range.",
        "MaxBetMustBeGreaterThanOrEqualTo": "Max Bet must be greater than or equal to {0}.",
        "MaxBetMustBeLessThanOrEqualTo": "Max Bet must be less than or equal to {0}.",
        "MaxPerMatchMustBeGreaterThanOrEqualTo": "Max per Match must be greater than or equal to {0}.",
        "MaxPerMatchMustBeLessThanOrEqualTo": "Max per Match must be less than or equal to {0}."
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