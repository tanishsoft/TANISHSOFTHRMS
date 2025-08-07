
$(function () {

    $(document).ajaxStart(function () {
        HoldOn.open({
            theme: 'sk-rect',
            message: "<h5 style='color:black;!important;'>Loading...! Please Wait.</h5>",
            textColor: "White"
        });
    });

    $(document).ajaxComplete(function () {
        //setTimeout(function () {
        //    HoldOn.close();
        //}, 1000);
        HoldOn.close();
    });

    MultiValidationFun_GlobalValidationJS();
});

/*****************Start Of Code To Validate Email By Element ID*********************************************************************/
function ValidateEmailByID_GlobalValidationJS(ElementID) {
    var sEmail = $('#' + ElementID).val();
    if (!ValidateEmailFormat_GlobalValidationJS(sEmail)) {
        alert("Invalid E-mail Address.Please Re-enter Mail");
        $('#' + ElementID).val('');
        $('#' + ElementID).addClass('RedColorBorderCSS');
    }
    else {
        $('#' + ElementID).removeClass('RedColorBorderCSS');
    }
}

/*****************Start Of Code To Get Minimum 17 Years Date To DatePicker**********************************************************/
function GetMaxDate_GlobalValidationJS() {
    var date = new Date();
    var lastyear = date.getFullYear() - 1;
    var year = new Date(lastyear - 17, 12 - 1, 31)
    return year;
}

/*****************Start Of Code To Validate Email By Its Nature[Check Is Valid Or Not]********************************************/
function ValidateEmailFormat_GlobalValidationJS(sEmail) {
    var filter = /^[\w\-\.\+]+\@[a-zA-Z0-9\.\-]+\.[a-zA-z0-9]{2,4}$/;
    var ReturnData = filter.test(sEmail) ? true : false;
    return ReturnData;
}

/*****************Start Of Code To Validate Multi Validations********************************************/
function MultiValidationFun_GlobalValidationJS() {

    ///*****************Start Of Code To Start Of Ajax Animation When Ajax Calling Start[Preventing User Actions While Ajax Call]*********************/
    //$(document).ajaxStart(function () {
    //    HoldOn.open({
    //        theme: 'sk-rect',
    //        message: "<h5 style='color:#ddd;!important;'>Loading...</h5>",
    //        textColor:"White"
    //    });

    //});

    ///*****************Start Of Code To Start Of Ajax Animation When Ajax Calling End[Preventing User Actions While Ajax Call]*********************/
    //$(document).ajaxComplete(function () {
    //    setTimeout(function () {
    //        HoldOn.close();
    //    }, 1500);

    //});

    /*****************Start Of Code To Restrict User To Do Not Enter First Characeter As Space*******************************************************/
    $('input[type=text],input[type=email],input[type=password], textarea').keydown(function (e) {
        if (e.which === 32 && !this.value.length) {
            e.preventDefault();
        }
    });

    $(".ClearTextBoxValue").bind("click", function () {
        $(this).val("");
    });

    /*****************Start Of Code To Restrict User To Do Not Allow Right Click on particular Element***********************************************/
    $('.NotAllowRightClick').bind("contextmenu", function (e) {
        return false;
    });

    /*****************Start Of Code To Restrict User To Enter Numbers 256 Max Characters Only*******************************************************/
    $(".MaxCharTextBox_256").attr("maxlength", 256);


    /*****************Start Of Code To Restrict User To Enter Numbers 10 Max Characters Only*******************************************************/
    $(".MaxCharTextBox_10").attr("maxlength", 10);


    /*****************Start Of Code To Restrict User To Enter Numbers 20 Max Characters Only*******************************************************/
    $(".MaxCharTextBox_20").attr("maxlength", 20);


    /*****************Start Of Code To Restrict User To Enter Numbers 50 Max Characters Only*******************************************************/
    $(".MaxCharTextBox_50").attr("maxlength", 50);


    /*****************Start Of Code To Restrict User To Enter Numbers 100 Max Characters Only*******************************************************/
    $(".MaxCharTextBox_100").attr("maxlength", 100);


    /*****************Start Of Code To Restrict User To Enter Numbers 150 Max Characters Only*******************************************************/
    $(".MaxCharTextBox_150").attr("maxlength", 150);


    /*****************Start Of Code To Restrict User To Enter Numbers Only************************************************************************/
    $('.Numbers').keydown(function (e) {
        if (e.which === 32 && !this.value.length) {
            e.preventDefault();
        } else {
            var key = e.keyCode;
            if (!((key == 8) || (key == 9) || (key >= 35 && key <= 40) || (key >= 48 && key <= 57) || (key >= 96 && key <= 105) || (key == 46))) {
                e.preventDefault();
            }
        }
    });


    $(".ReadOnlyField_CSS").keydown(function (e) {
        e.preventDefault();
    });

    //$(".NormalBSDatePicker").attr('readonly', 'true');
    //$(".NormalBSDatePicker").datepicker({
    //    format: "dd/mm/yyyy",
    //    calendarWeeks: true,
    //    autoclose: true,
    //    todayHighlight: true,
    //    toggleActive: true
    //});

    /*****************Start Of Code To Select DatePicker[Maximum Date Allowed Is Restricted To Today Only]****************************************/
    $(".MaxDatePicker").attr('readOnly', 'true');
    $(".MaxDatePicker").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'dd/mm/yy',
        yearRange: "c-60:c+60",
        showAnim: 'slideDown',
        maxDate: 'today'
    });

    /*****************Start Of Code To Select DatePicker[Minimum Date Allowed Is Restricted To Today Only]****************************************/
    $(".MinDatePicker").attr('readOnly', 'true');
    $(".MinDatePicker").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'mm/dd/yy',
        yearRange: "c-60:c+60",
        showAnim: 'slideDown',
        minDate: 'today',
    });

    /*****************Start Of Code To Select DatePicker[Maximum Date Allowed Is Restricted To Minimum 17 Years]*********************************/
    $(".txtDOBDate").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "mm/dd/yy",
        yearRange: "c-50:c+50",
        showAnim: "slideDown",
        maxDate: GetMaxDate_GlobalValidationJS()
    });

    /*****************Start Of Code To Select DatePicker[All Dates]****************************************/
    $(".NormalDatePicker").attr('readOnly', 'true');
    $(".NormalDatePicker").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'mm/dd/yy',
        yearRange: "c-60:c+60",
        showAnim: 'slideDown',
    });

    /*****************Start Of Code To Restrict User To Enter Alphabets Only**********************************************************************/
    $('.txtOnlyText').keydown(function (e) {
        if (e.which === 32 && !this.value.length) {
            e.preventDefault();
        } else {
            var key = e.keyCode;
            if (!((key == 8) || (key == 9) || (key == 32) || (key == 46) || (key >= 35 && key <= 40) || (key >= 65 && key <= 90))) {
                e.preventDefault();
            }
        }
    });

    /*****************Start Of Code To Show Tooltip When User Hover The Particular Element**********************************************************/
    $(".TitleToolTip").tooltip({
        track: true,
        show: {
            effect: "slideDown",
            delay: 100
        }
    });

    /*****************Start Of Code To Restrict User To Enter AlphabetNumerics & Special Characters Only********************************************/
    $(".txtAlphaNumSplChar").keypress(function (e) {
        if (e.which === 32 && !this.value.length) {
            e.preventDefault();
        } else {
            var regex = new RegExp("^[a-zA-Z0-9\-\(\)\]+$");
            var key = String.fromCharCode(!e.charCode ? e.which : e.charCode);
            if (!regex.test(key)) {
                e.preventDefault();
                return false;
            }
        }
    });

    /*****************Start Of Code To Restrict User To Enter AlphabetNumerics & Special Characters Only********************************************/
    $(".txtSplChar").keypress(function (e) {
        if (e.which === 32 && !this.value.length) {
            e.preventDefault();
        } else {
            var regex = new RegExp("^[a-zA-Z0-9\-\,\.\+\(\)\]+$");
            var key = String.fromCharCode(!e.charCode ? e.which : e.charCode);
            if (!regex.test(key)) {
                e.preventDefault();
                return false;
            }
        }
    });

    /*****************Start Of Code To Restrict User To Enter AlphabetNumerics & Space Only**********************************************************/
    $(".TextNdSpcNdNums").keypress(function (e) {
        if (e.which === 32 && !this.value.length) {
            e.preventDefault();
        } else {
            var regex = new RegExp("^[a-zA-Z0-9\ \]+$");
            var key = String.fromCharCode(!e.charCode ? e.which : e.charCode);
            if (!regex.test(key)) {
                e.preventDefault();
                return false;
            }
        }
    });

    /*****************Start Of Code To Restrict User To Enter AlphabetNumerics & Special Characters Only********************************************/
    $(".txtspecialChars").keypress(function (e) {
        if (e.which === 32 && !this.value.length) {
            e.preventDefault();
        } else {
            var regex = new RegExp("^[a-zA-Z0-9\-\,\.\+\(\)\ \]+$");
            var key = String.fromCharCode(!e.charCode ? e.which : e.charCode);
            if (!regex.test(key)) {
                e.preventDefault();
                return false;
            }
        }
    });

    /*****************Start Of Code To Restrict User To Enter Alphabets Only************************************************************************/
    $('.TxtOnly').keydown(function (e) {
        if (e.which === 32 && !this.value.length) {
            e.preventDefault();
        } else {
            var key = e.keyCode;
            if (!((key == 8) || (key == 9) || (key == 32) || (key == 46) || (key >= 35 && key <= 40) || (key >= 65 && key <= 90))) {
                e.preventDefault();
            }
        }
    });



    $('.MobileNumber').keydown(function (e) {
        if (e.which === 32 && !this.value.length) {
            e.preventDefault();
        } else {
            var key = e.keyCode;
            if (!((key == 8) || (key == 9) || (key >= 35 && key <= 40) || (key >= 48 && key <= 57) || (key >= 96 && key <= 105) || (key == 46))) {
                e.preventDefault();
            }
        }
    });

    /*****************Start Of Code To Restrict User To Do Not Leave First Entered Input As Space***************************************************/
    $('.FirstLetterSpc').keydown(function (e) {
        if (e.which === 32 && !this.value.length) {
            e.preventDefault();
        }
    });

    /*****************Start Of Code To Restrict User Enter Only Numbers & One Dot***************************************************/
    $('.NumDot').keypress(function (event) {
        if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57) && event.which != 8 && event.which != 0) {
            event.preventDefault();
        }
    });

    /*****************Start Of Code To Validate TextBox To Accept Numbers Along With Copy& Paste**************************************/
    $(".TxtNumCP").keydown(function (event) {
        if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 27 || event.keyCode == 13 || (event.keyCode == 65 && event.ctrlKey === true) ||
        (event.keyCode == 67 && event.ctrlKey === true) || (event.keyCode == 86 && event.ctrlKey === true) || (event.keyCode >= 35 && event.keyCode <= 39)) {
            return;
        } else {
            if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)) {
                event.preventDefault();
            }
        }
    });


    /*************Start Of Code To Validate EmailID Using Class Name******************************************************************/
    $('.ValidateEmailID').focusout(function () {
        var EnteredEmail = $(this).val();
        if (EnteredEmail != null && EnteredEmail != "") {
            if (!ValidateEmailFormat_GlobalValidationJS(EnteredEmail)) {
                OpenSuccessAlertPopUpBox_ConfirmPopUpJS("Invalid E-mail Address.Please Re-enter Mail");
                $(this).val('');
                //$(this).addClass('RedColorBorderCSS');
            }
            else {
                //$(this).removeClass('RedColorBorderCSS');
            }
        } else {

        }
    });
}

/*****************Start Of Code To Convert Number To Word********************************************/
function ConertNumberToWord_GlobalValidationJS(num) {
    var a = ['', 'One ', 'Two ', 'Three ', 'Four ', 'Five ', 'Six ', 'Seven ', 'Eight ', 'Nine ', 'Ten ', 'Eleven ', 'Twelve ', 'Thirteen ', 'Fourteen ', 'Fifteen ', 'Sixteen ', 'Seventeen ', 'Eighteen ', 'Nineteen '];
    var b = ['', '', 'Twenty', 'Thirty', 'Forty', 'Fifty', 'Sixty', 'Seventy', 'Eighty', 'Ninety'];

    if ((num = num.toString()).length > 9) return 'overflow';
    n = ('000000000' + num).substr(-9).match(/^(\d{2})(\d{2})(\d{2})(\d{1})(\d{2})$/);
    if (!n) return;
    var str = '';
    str += (n[1] != 0) ? (a[Number(n[1])] || b[n[1][0]] + ' ' + a[n[1][1]]) + 'Crore ' : '';
    str += (n[2] != 0) ? (a[Number(n[2])] || b[n[2][0]] + ' ' + a[n[2][1]]) + 'Lakh ' : '';
    str += (n[3] != 0) ? (a[Number(n[3])] || b[n[3][0]] + ' ' + a[n[3][1]]) + 'Thousand ' : '';
    str += (n[4] != 0) ? (a[Number(n[4])] || b[n[4][0]] + ' ' + a[n[4][1]]) + 'Hundred ' : '';
    str += (n[5] != 0) ? ((str != '') ? 'and ' : '') + (a[Number(n[5])] || b[n[5][0]] + ' ' + a[n[5][1]]) + '' : '';
    return str
}

/*****************Start Of Code To Validate TextBox To Accept Numbers Along With Copy& Paste**************************************/
function AcceptNumCP() {
    $(".TxtNumCp").keydown(function (event) {
        if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 27 || event.keyCode == 13 || (event.keyCode == 65 && event.ctrlKey === true) ||
        (event.keyCode == 67 && event.ctrlKey === true) || (event.keyCode == 86 && event.ctrlKey === true) || (event.keyCode >= 35 && event.keyCode <= 39)) {
            return;
        } else {
            if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)) {
                event.preventDefault();
            }
        }
    });
}


/**********************Start Of Code To User Multi Date Functions.Added By Edukondalu on 27-Nov-2015 *****************************************/
function DatePickerMultiUseFun_GlobalValidationJS() {
    /*****************Start Of Code To Select DatePicker[Maximum Date Allowed Is Restricted To Today Only]****************************************/
    $(".MaxDatePicker").attr('readOnly', 'true');
    $(".MaxDatePicker").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'dd/mm/yy',
        yearRange: "c-60:c+60",
        showAnim: 'slideDown',
        maxDate: 'today'
    });

    /*****************Start Of Code To Select DatePicker[Minimum Date Allowed Is Restricted To Today Only]****************************************/
    $(".MinDatePicker").attr('readOnly', 'true');
    $(".MinDatePicker").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'mm/dd/yy',
        yearRange: "c-60:c+60",
        showAnim: 'slideDown',
        minDate: 'today',
    });

    /*****************Start Of Code To Select DatePicker[Maximum Date Allowed Is Restricted To Minimum 17 Years]*********************************/
    $(".txtDOBDate").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "mm/dd/yy",
        yearRange: "c-50:c+50",
        showAnim: "slideDown",
        maxDate: GetMaxDate_GlobalValidationJS()
    });

    /*****************Start Of Code To Select DatePicker[All Dates]****************************************/
    $(".NormalDatePicker").attr('readOnly', 'true');
    $(".NormalDatePicker").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'mm/dd/yy',
        yearRange: "c-60:c+60",
        showAnim: 'slideDown',
    });

}

