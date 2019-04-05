/*jshint -W101,-W106,-W044,-W098*/

(function () {
    var labels = {};

    labels["en-us"] = {
        "CommissionAndBetLimit": "Commission & Bet Limit",
        "MaxBet": "Max per Bet",
        "MaxPerEvent": "Max per Event",
        "MemberCommission": "Member Commission"
    };

    labels["zh-hans"] = {
        "CommissionAndBetLimit": "退水和限额",
        "MaxBet": "单注最高",
        "MaxPerEvent": "单场最高",
        "MemberCommission": "会员退水设定"
    };

    labels["zh-tw"] = {
        "CommissionAndBetLimit": "退水和限額",
        "MaxBet": "單注最高",
        "MaxPerEvent": "單場最高",
        "MemberCommission": "會員退水設定"
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

    messages["zh-hans"] = {
        "RequiredField": "必填字段",
        "CommissionIsOutOfRange": "退水设定超出范围。",
        "MaxBetNote": "单注的最高投注额。",
        "MaxBetMustBeGreaterThanOrEqualTo": "单注最高金额必须大于或等于{0}。",
        "MaxBetMustBeLessThanOrEqualTo": "单注最高金额必须小于或等于{0}。",
        "MaxBetMustBeLessThanOrEqualToMaxPerEvent": "单注最高金额必须小于或等于单场最高",
        "MaxPerEventNote": "单场最高投注金额的限制是根据个别玩法和场次号, 单双除外。",
        "MaxPerEventMustBeGreaterThanOrEqualTo": "单场最高金额必须大于或等于{0}。",
        "MaxPerEventMustBeLessThanOrEqualTo": "单场最高金额必须小于或等于{0}。"
    };

    messages["zh-tw"] = {
        "RequiredField": "必填欄位",
        "CommissionIsOutOfRange": "退水設定超出範圍。",
        "MaxBetNote": "單注的最高投注額。",
        "MaxBetMustBeGreaterThanOrEqualTo": "單注最高金額必須大於或等於{0}。",
        "MaxBetMustBeLessThanOrEqualTo": "單注最高金額必須小於或等於{0}。",
        "MaxBetMustBeLessThanOrEqualToMaxPerEvent": "單注最高金額必須小於或等於單場最高",
        "MaxPerEventNote": "單場最高投注金額的限制是根據個別玩法和場次號, 單雙除外。",
        "MaxPerEventMustBeGreaterThanOrEqualTo": "單場最高金額必須大於或等於{0}。",
        "MaxPerEventMustBeLessThanOrEqualTo": "單場最高金額必須小於或等於{0}。"
    };

    var settingType2Resources = {
        "labels": labels,
        "messages": messages
    };

    angular.module("customerSetting").value("st2Resources", settingType2Resources);
})();