

var UserEmailList_PageLevel;
$(function () {

});



function ValidateRequiredFieldsByClassName_GlobalJS(ClassName) {
    var ReturnStatus = true;
    $("." + ClassName).each(function () {
        if (!CheckTheValueIsNullOrEmpty_GlobalJS($(this).val())) {
            $(this).addClass("RedColorBorderCSS");
            ReturnStatus = false;
        } else {
            $(this).removeClass("RedColorBorderCSS");
        }
    });
    return ReturnStatus;
}

function ResetFieldsByClassName_GlobalJS(ClassName) {
    var ReturnStatus = true;
    $("." + ClassName).each(function () {
        $(this).val('');
        $(this).removeClass("RedColorBorderCSS");
    });
    return ReturnStatus;
}



function CheckTheValueIsNullOrEmpty_GlobalJS(EnteredValue) {
    var ReturnStatus = false;
    if (EnteredValue != null && EnteredValue != "") {
        EnteredValue = EnteredValue.trim();
        if (EnteredValue != null && EnteredValue != "") {
            ReturnStatus = true;
        }
    }
    return ReturnStatus;
}


function GetParsedDateValueFromStringDateForDateFormat1_GlobalJSFun(Data) {/*dd/MM/yyyy*/
    var HiddenDate = jQuery("#_LayOutHiddenDateFormat_1").val();
    var DateToParse = (HiddenDate != null && HiddenDate != "") ? HiddenDate : "01,01,2016";
    var ParsedDate = Date.parse(DateToParse);
    if (CheckTheValueIsNullOrEmpty_GlobalJS(Data) && Data.includes('/')) {
        var Split = Data.split("/");
        ParsedDate = Date.parse(Split[1] + "," + Split[0] + "," + Split[2]);
    }
    return ParsedDate;
}



function GetParsedDateValueFromStringDateForDateFormat2_GlobalJSFun(Data) {/*MM,dd,yyyy*/
    var LayOutHiddenDate = jQuery("#_LayOutHiddenDateFormat_2").val();
    var ParsedDate = Date.parse(LayOutHiddenDate);
    if (CheckTheValueIsNullOrEmpty_GlobalJS(Data)) {
        ParsedDate = Date.parse(Data);
    }
    return ParsedDate;
}

function GetParsedDateValueFromStringDateForDateFormat3_GlobalJSFun(Data) {/*MM/dd/yyyy*/
    var HiddenDate = jQuery("#_LayOutHiddenDateFormat_1").val();
    var DateToParse = (HiddenDate != null && HiddenDate != "") ? HiddenDate : "01,01,2016";
    var ParsedDate = Date.parse(DateToParse);
    if (CheckTheValueIsNullOrEmpty_GlobalJS(Data) && Data.includes('/')) {
        var Split = Data.split("/");
        ParsedDate = Date.parse(Split[0] + "," + Split[1] + "," + Split[2]);
    }
    return ParsedDate;
}

function GetAllUserEmailListToAdmin_GlobalFunctionJS() {
    UserEmailList_PageLevel = new Array();
    $.get("/HrAdmin/GetAllUserEmailListToAdmin_HRAdmin", function (Result) {
        UserEmailList_PageLevel = Result;
    });
}

function CheckUserEmailIDExistsOrNot_GlobalFunJS(NewEmailID) {
    var ReturnStatus = true;
    if (ValidateEmailFormat_GlobalValidationJS(NewEmailID)) {
        var EnteredEmailID = NewEmailID.toLowerCase();
        for (var i in UserEmailList_PageLevel) {
            if (UserEmailList_PageLevel[i].EmailID == EnteredEmailID) {
                ReturnStatus = false;
                break;
            }
        }
    } else {
        ReturnStatus = false;
    }
    return ReturnStatus;
}

function CheckUserEmailIDExistsOrNotForEdit_GlobalFunJS(NewEmailID, UserId) {
    var ReturnStatus = true;
    if (ValidateEmailFormat_GlobalValidationJS(NewEmailID)) {
        var EnteredEmailID = NewEmailID.toLowerCase();
        for (var i in UserEmailList_PageLevel) {
            if (UserEmailList_PageLevel[i].EmailID == EnteredEmailID && UserEmailList_PageLevel[i].UID != parseFloat(UserId)) {
                ReturnStatus = false;
                break;
            }
        }
    } else {
        ReturnStatus = false;
    }
    return ReturnStatus;
}