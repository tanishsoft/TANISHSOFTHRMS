
var currentId;
var currentPerId;
var validate = true;

var allemployeelist;
$(document).ready(function () {
    GetEmployeesListtobindnames();
    $(".panel-success").css("display", "none");
    $("#myleaestableshow").css("display", "block");
    $("#txtDate,#txtDate1").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy",
        minDate: -30
    });

    bindLeaveDates();


    $("#LeaveFromDate1").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy",
        minDate: -30
    });
    $("#LeaveTodate1").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy",
        minDate: -30
    });
    $("#DateofAvailableCompoff,#DateofAvailableCompoff1").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy",
        minDate: -30
    });
    $("#txtCompOffDate").datepicker({
        maxDate: '0',
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy",
        minDate: -30
    });
    LoadLeaveTypes();
    //LoadApprover();
    GetMyLeaves();
    LoadEmployees();
    LoadMyEmployeelist();

    LoadOfMyApproveLeaveCount();
    GetApprovePermissionsCount();
    GetApproveRequestCompOffLeave();
    GetRequestedComOffLeaveListToUser_PageLevel();
    if ($("#LoginEmployeeType").val() != "Manager") {
        $("#tronmanaerleave").css("display", "none");
    }


    if (Getdays() >= 3 && ($("#LoginEmployeeType").val() != "Manager")) {
        $("#tronmanaerleave").hide();
        //$("#tronmanaerleave").show();

    } else if (Getdays() >= 3 && ($("#LoginEmployeeType").val() == "Manager")) {
        $("#tronmanaerleave").show();

    } else { $("#tronmanaerleave").hide(); }

    $("#daytype").addClass("trCompoff");
    BindEditshifttypes();
});

function BindEditshifttypes() {
    var options = [];
    options.push('<option value="">Select</option>');
    $.ajax({
        type: 'POST',
        url: "/Hr/GetShiftType",
        data: 'id=1',
        dataType: "json",
        success: function (result) {
            for (var i = 0; i < result.length; i++) {
                if (result[i].Name != null && result[i].id != 3) {
                    options.push('<option value="' + result[i].id + '">' + result[i].Name + '</option>');
                }
            }
            $("#ddlshifttypesofcompoffrequest").html(options.join(''));
        },
        error: function () {
            alert('error');
        }
    });
}
function bindLeaveDates() {
    var dateFormat = "dd/mm/yy",
         from = $("#LeaveFromDate")
           .datepicker({
               changeMonth: true,
               changeYear: true,
               dateFormat: "dd/mm/yy",
               onSelect: function (selected) {
                   $("#LeaveTodate").datepicker("option", "minDate", selected)
               }
           })
           //.on("change", function () {
           //    to.datepicker("option", "minDate", getDate(this));
           //})
           ,
         to = $("#LeaveTodate").datepicker({
             changeMonth: true,
             changeYear: true,
             dateFormat: "dd/mm/yy",
             minDate: -30,
              onSelect: function(selected) {
                  $("#LeaveFromDate").datepicker("option", "maxDate", selected)
        }
         })
    //.on("change", function () {
    //    from.datepicker("option", "maxDate", getDate(this));
    //})
    ;


    function getDate(element) {
        var date;
        try {
            date = $.datepicker.parseDate(dateFormat, element.value);
        } catch (error) {
            date = null;
        }

        return date;
    }
}

//function enable()
//{
//    if ($("#LoginEmployeeType").val() == "Manager" && Getdays() >= 3) {
//        //document.getElementById('Level1Approver').disabled = true;
//        $("#tronmanaerleave").show();

//    }
//    else {
//        $("#tronmanaerleave").hide();
//        //document.getElementById('Level1Approver').disabled = false;
//    }
//}


function LoadOfMyApproveLeaveCount() {
    $.ajax({
        type: "GET",
        url: "/Hr/GetApproveLeaveCount",
        //data: "id=",
        success: function (data) {
            $("#lblApproveLeavescount").html(data);

        }
    });
}
function GetApprovePermissionsCount() {
    $.ajax({
        type: "GET",
        url: "/Hr/GetApprovePermissionsCount",
        //data: "id=",
        success: function (data) {
            $("#lblApprovePermissioncount").html(data);
        }
    });
}
function GetApproveRequestCompOffLeave() {
    $.ajax({
        type: "GET",
        url: "/Hr/GetApproveRequestCompOffLeave",
        //data: "id=",
        success: function (data) {
            $("#lblApproveCompOffcount").html(data);
        }
    });
}


function LoadLeaveTypes() {

    $.ajax({
        type: "GET",
        url: "/Common/GetLeaveTypes",
        //data: "id=",
        cache: false,
        success: function (data) {
            var options = [];
            options.push('<option value="">- Select LeaveType -</option>');
            for (var i = 0; i < data.length; i++) {
                options.push('<option value="' + data[i].LeaveTypeId + '">' + data[i].LeaveName + '</option>');
            }
            $("#ddlLeaveType,#ddlLeaveType1").html(options.join(''));

        },
        error: function () {
            alert("error")
        }
    });
}
function ClicklistItem(clsname) {
    clearAll();
    validate = true;
    $(".css-error").removeClass('css-error');
    validate = true;
    $(".panel-success").css("display", "none");
    var id = "." + clsname;
    $(id).css("display", "block");
    if (clsname == 'mydashboard') {
        $("#myleaestableshow").css("display", "block");
    }
    if (clsname == 'StatusApplyLeave') {
        LoadLeaveStatus();
    }
    if (clsname == 'ApproveLeave') {
        ApprovedDataTable();
    }
    if (clsname == 'Permissions') {
        GetPermissionsleaves();
    }
    if (clsname == 'ApprovePermissions') {
        GetPermissionsApproveleaves();
    }
    if (clsname == 'OtherLeaves') {
        // GetPermissionsApproveleaves();
    }
    if (clsname == 'ViewCompOffEncashRequestList') {
        CompOffEncashDataTable();
    }


    //$(".Dashboarddefault").css("display", "block");
}
function cbcheckedchange(value) {
    if (value) {
        $("#trCompoff").removeClass("trCompoff");
        $("#trfromdate").addClass("trCompoff");
    }
    else {
        $("#trfromdate").removeClass("trCompoff");
        $("#trCompoff").addClass("trCompoff");
    }
}
function cbcheckedchange1(value) {
    if (value) {
        $("#trCompoff1").removeClass("trCompoff");
        $("#trfromdate1").addClass("trCompoff");
    }
    else {
        $("#trfromdate1").removeClass("trCompoff");
        $("#trCompoff1").addClass("trCompoff");
    }
}

function dropdowncheckedchange(id) {
    $("#cbfullday").prop("checked", true);
    if ($("#" + id).val() == "6") {
        $("#trfromdate").addClass("trCompoff");
        //$("#typeoftheday").addClass("daytype");

    }
    else {
        $("#trfromdate").removeClass("trCompoff");
        //$("#typeoftheday").removeClass("daytype");
    }

}
function dropdowncheckedchange1(id) {
    if ($("#" + id).val() == "6") {
        $("#trfromdate1").addClass("trCompoff");
    }
    else {
        $("#trfromdate1").removeClass("trCompoff");
    }
}
function LoadApprover() {
    $.ajax({
        type: "POST",
        url: "/Common/GetEmployeesByUserLocation",
        //data: "id=",
        success: function (data) {
            var options = [];
            options.push('<option value="">- Select Approver -</option>');
            for (var i = 0; i < data.length; i++) {
                options.push('<option value="' + data[i].CustomUserId + '">' + data[i].FirstName + ' ' + data[i].LastName + '</option>');
            }
            $("#Level1Approver").html(options.join(''));
            $("#Level2Approver").html(options.join(''));
            // $("#LevelApprover").html(options.join(''));

        },
        error: function () {
            alert("error")
        }
    });
}

function ApplyNewCompOffLeave_PageLevel() {
    var ReqCompOffDate = $("#txtCompOffDate").val();
    var ReasonForCompOff = $("#txtReasonForCompOffLeave").val() + " - The Shift worked : " + $("#ddlshifttypesofcompoffrequest option:selected").text();;
    if (ValidateRequiredFieldsByClassName_GlobalJS("CompOffRequiredField")) {
        var Dataparam = {
            CompOffDate: ReqCompOffDate,
            RequestReason: ReasonForCompOff,
            ShiftTypeId: $("#ddlshifttypesofcompoffrequest").val(),
            DayWorkedType: $("#ddltypeofdayworked").val()
        };
        $.ajax({
            type: "POST",
            url: "/Hr/RequestNewCompOffLeave",
            data: Dataparam,
            success: function (data) {
                $("#txtCompOffDate").val('');
                $("#txtReasonForCompOffLeave").val('');
                OpenSuccessAlertPopUpBox_ConfirmPopUpJS(data);
                if (data == "Success") {
                    GetRequestedComOffLeaveListToUser_PageLevel();
                    ClicklistItem('ViewCompOffLeaveRequestList');
                    GetRequestedComOffLeaveListToManagerToApprove_PageLevel();
                }
            },
            error: function () {
                alert("error")
            }
        });

    } else {
        OpenSuccessAlertPopUpBox_ConfirmPopUpJS("Please Enter Mandatory Fields");
    }
}

function GetRequestedComOffLeaveListToUser_PageLevel() {

    $('#CompOffRequestList_DataTable').dataTable({
        "iDisplayLength": 10,
        "bServerSide": true,
        "sAjaxSource": "/Hr/AjaxMyRequestCompOffView",
        "bProcessing": true,
        "bAutoWidth": true,
        "bDestroy": true,
        "aLengthMenu": [[10, 20, 30, 50, 100, 150, 200, -1], [10, 20, 30, 50, 100, 150, 200, "All"]],
        //"aaSorting": [[0, "desc"]],
        "aoColumns": [
             { "sName": "UserName" },
             { "sName": "DepartmentName" },
             { "sName": "LocationName" },
             { "sName": "CompOffDate" },
             { "sName": "RequestReason" },
             { "sName": "Leave_Status" },
             { "sName": "CreatedDateTime" },
             {
                 "sName": "ID",
                 "bSortable": false,
                 "render": function (data) {

                     return '<div class="btn-group"><a href="javascript:void(0);" id="' + data + '" onclick="CancelCmpRequest(this.id);">Cancel</a>' +
'</div>';
                 }
             }

        ]
    });
    $("#CompOffRequestList_DataTable").addClass("col-md-6");
    $("#CompOffRequestList_DataTable").addClass("col-md-6");
    GetApprovedComOffLeaveListToUser_PageLevel();
}
function CancelCmpRequest(id) {
    if (confirm("Are you sure you want to cancel the request?")) {
        $.ajax({
            type: "GET",
            url: "/Hr/CancelCompOffrequest",
            data: "id=" + id,
            success: function (data) {
                alert(data);
                GetRequestedComOffLeaveListToUser_PageLevel();
            },
            error: function () {
                alert("error")
            }
        });
    }

}


function GetApprovedComOffLeaveListToUser_PageLevel() {
    $('#CompOffApprovedList_DataTable').dataTable({
        "iDisplayLength": 10,
        "bServerSide": true,
        "sAjaxSource": "/Hr/AjaxMyRequestCompOffViewApproved",
        "bProcessing": true,
        "bAutoWidth": true,
        "bDestroy": true,
        "aLengthMenu": [[10, 20, 30, 50, 100, 150, 200, -1], [10, 20, 30, 50, 100, 150, 200, "All"]],
        //"aaSorting": [[0, "desc"]],
        "aoColumns": [
             { "sName": "UserName" },
             { "sName": "DepartmentName" },
             { "sName": "LocationName" },
             { "sName": "CompOffDate" },
             { "sName": "RequestReason" },
             { "sName": "Leave_Status" },
              { "sName": "CreatedDateTime" }


        ]
    });
    $("#CompOffApprovedList_DataTable").addClass("col-md-6");
    $("#CompOffApprovedList_DataTable").addClass("col-md-6");

    GetRequestedComOffLeaveListToManagerToApprove_PageLevel();
}




function GetRequestedComOffLeaveListToManagerToApprove_PageLevel() {

    $('#ApproveCompOffRequestList_DataTable').dataTable({
        "iDisplayLength": 10,
        "bServerSide": true,
        "sAjaxSource": "/Hr/AjaxMyPendingRequestCompOff",
        "bProcessing": true,
        "bAutoWidth": true,
        "bDestroy": true,
        "aLengthMenu": [[10, 20, 30, 50, 100, 150, 200, -1], [10, 20, 30, 50, 100, 150, 200, "All"]],
        //"aaSorting": [[0, "desc"]],
        "aoColumns": [
             { "sName": "UserName" },
             { "sName": "DepartmentName" },
             { "sName": "LocationName" },
             { "sName": "CompOffDate" },
             { "sName": "RequestReason" },
             { "sName": "Leave_Status" },
             { "sName": "CreatedDateTime" },
             {
                 "sName": "ID",
                 "bSortable": false,
                 "render": function (data) {

                     return '<div class="btn-group">' +
'<div class="btn btn-default dropdown-toggle" data-toggle="dropdown"><span class="glyphicon glyphicon-th-list"></span></div>' +
'<ul class="dropdown-menu dropdown-menu-right">' +
'  <li><a href="javascript:void(0);" id="' + data + '" onclick="AcceptRequestedCompOffLeave_PageLevel(this.id);">Approve</a></li><li><a href="javascript:void(0);" onclick="RejectRequestedCompOffLeave_PageLevel(this.id);" id="' + data + '">Reject</a></li>' +
'</ul></div>';

                 }
             }
        ]
    });
    $("#ApproveCompOffRequestList_DataTable").addClass("col-md-6");
    $("#ApproveCompOffRequestList_DataTable").addClass("col-md-6");
}

function RejectRequestedCompOffLeave_PageLevel(ID) {
    if ($.isNumeric(ID)) {
        if (confirm("Are You Sure You Want To Reject The CompOff Leave")) {
            $.ajax({
                type: "POST",
                url: "/Hr/AcceptOrRejectRequestedCompOffListByStatus",
                data: { "ID": ID, "Status": "Rejected" },
                success: function (data) {
                    OpenSuccessAlertPopUpBox_ConfirmPopUpJS(data);

                    GetRequestedComOffLeaveListToManagerToApprove_PageLevel();
                    GetApproveRequestCompOffLeave();
                },
                error: function () {
                    alert("error")
                }
            });
        }
    }
}


function AcceptRequestedCompOffLeave_PageLevel(ID) {
    if ($.isNumeric(ID)) {
        if (confirm("Are You Sure You Want To Accept The CompOff Leave")) {
            $.ajax({
                type: "POST",
                url: "/Hr/AcceptOrRejectRequestedCompOffListByStatus",
                data: { "ID": ID, "Status": "Approved" },
                success: function (data) {
                    OpenSuccessAlertPopUpBox_ConfirmPopUpJS(data);
                    GetApproveRequestCompOffLeave();
                    GetRequestedComOffLeaveListToManagerToApprove_PageLevel();
                },
                error: function () {
                    alert("error")
                }
            });
        }
    }
}

function GetDateDiffeCount() {

}
function LoadMyEmployeelist() {

    $.ajax({
        type: "GET",
        url: "/Manager/GetMyEmployeelist",
        //data: "id=" + $("#ddlDepartment").val(),
        success: function (data) {
            var options = [];
            options.push('<option value="">- Select Employee -</option>');
            for (var i = 0; i < data.length; i++) {
                options.push('<option value="' + data[i].CustomUserId + '">' + data[i].FirstName + " " + data[i].LastName + '</option>');
            }
            $("#Level1Approver").html(options.join(''));

        },
        error: function () {
            alert("error")
        }
    });

}
//389
//434
/*Days count based on the date selection*/

function Getdays() {
    var Fromdate = Changeandgetdate($("#LeaveFromDate").val());
    var Todate = Changeandgetdate($("#LeaveTodate").val());
    //var week = new Array('Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday');

    var totaldays = Todate - Fromdate;
    var count = (totaldays / (1000 * 60 * 60 * 24)) + 1;
    if (count == 1) {
        if (!$("#cbfullday").prop("checked")) {
            $("#LeaveFromDate").val(LeaveToDate);
        }
    }
    if (count >= 3 && ($("#LoginEmployeeType").val() != "Manager")) {
        $("#tronmanaerleave").hide();
        //$("#tronmanaerleave").show();

    } else if (count >= 3 && ($("#LoginEmployeeType").val() == "Manager")) {
        $("#tronmanaerleave").show();

    } else { $("#tronmanaerleave").hide(); }
    return count;
}
function Changeandgetdate(date) {
    var newfromdate = date.split('/');
    var datenew = new Date(newfromdate[2] + "/" + newfromdate[1] + "/" + newfromdate[0]);
    return datenew;
}

function SaveLeave() {
    var LeaveType = $("#ddlLeaveType").val();
    var LeaveToDate = $("#LeaveTodate").val();
    var CompOff = false;
    if (LeaveType == 6) {
        $("#LeaveFromDate").val(LeaveToDate);
        $("#DateofAvailableCompoff").val(LeaveToDate);
        CompOff = true;
    }
    if (!$("#cbfullday").prop("checked")) {
        $("#LeaveFromDate").val(LeaveToDate);
    }
    var LeaveFromDate = $("#LeaveFromDate").val();
    validateLeave();
    if (validate) {
        var showconfirmmsg = false;
        //LeaveFromDate = ConvertDatefromoneddmmyyytommddyyy(LeaveFromDate);
        //LeaveToDate = ConvertDatefromoneddmmyyytommddyyy(LeaveToDate);
        var date1 = new Date((ConvertDatefromoneddmmyyytommddyyy(LeaveFromDate)));
        var date2 = new Date((ConvertDatefromoneddmmyyytommddyyy(LeaveToDate)));
        var timeDiff = Math.abs(date2.getTime() - date1.getTime());
        var diffDays = Math.ceil(timeDiff / (1000 * 3600 * 24));
        diffDays = diffDays + 1;
        if (diffDays > 3 && LeaveType == 4) {
            showconfirmmsg = true;
        }
        var fullday = $('input:checkbox[name=cbfullday]').is(':checked');
        //  var CompOffCheckDatesStatus = (CompOff) ? (GetParsedDateValueFromStringDateForDateFormat3_GlobalJSFun(LeaveToDate) >= GetParsedDateValueFromStringDateForDateFormat3_GlobalJSFun($("#DateofAvailableCompoff").val())) : (GetParsedDateValueFromStringDateForDateFormat3_GlobalJSFun(LeaveToDate) >= GetParsedDateValueFromStringDateForDateFormat3_GlobalJSFun(LeaveFromDate));
        // if (CompOffCheckDatesStatus) {
        //  waitingDialog.show();

        var msg = "Please submit sickness certificate to hr from a registered medical practitioner, not below the rank of MBBS. ";
        var task = {
            LeaveTypeId: LeaveType,
            LeaveTypeName: $("#ddlLeaveType option:selected").text(),
            IsFullday: fullday,
            IsCompOff: CompOff,
            DateofAvailableCompoff: $("#LeaveTodate").val(),
            LeaveFromDate: $("#LeaveFromDate").val(),
            LeaveTodate: $("#LeaveTodate").val(),
            ReasonForLeave: $("#ReasonForLeave").val(),
            AddressOnLeave: $("#AddressOnLeave").val(),
            Level1Approver: $("#Level1Approver").val(),
            LeaveSessionDay: $("#typeoftheday").val(),
            WeeklyOffDay: $("#WeeklyOffDay").val()

        };
        if (showconfirmmsg) {
            //check leaves
            $.ajax({
                type: "POST",
                url: "/Hr/GetCountOfMMedicalHarLeave",
                data: task,
                success: function (data) {
                    if (data > 3) {
                        if (confirm(msg)) {
                            CheckOtherEmployeesindepartmentareLeave(task);
                        }
                    } else {
                        CheckOtherEmployeesindepartmentareLeave(task);
                    }
                }
            });
        }
        else {
            CheckOtherEmployeesindepartmentareLeave(task);
        }
        //} else {
        //    OpenSuccessAlertPopUpBox_ConfirmPopUpJS("Leave To Date Should Be Greater Than Leave From Date.");
        //}
    }
    else {
        OpenSuccessAlertPopUpBox_ConfirmPopUpJS("Please fill all mandatory fields");
    }
}
function PostLeavedata(leave) {
    $.ajax({
        type: "POST",
        url: "/Hr/LeaveManagement",
        data: leave,
        success: function (data) {
            //  waitingDialog.hide();
            alert(data);
            if (data == "Leave applied successfully") {
                //GetMyLeaves();
                //clearAll();
                window.location.reload();
            }
        },
        error: function () {
            alert("error")
        }
    });
}
var currnetLeavesdatapost;
function CheckOtherEmployeesindepartmentareLeave(leave) {
    currnetLeavesdatapost = leave;
    $.ajax({
        type: "POST",
        url: "/Hr/CheckAnyDepartmentPersonOnLeave",
        data: leave,
        success: function (data) {
            if (data == "") {
                PostLeavedata(leave);
            } else {
                var html = "";
                for (var i = 0; i < data.length; i++) {
                    html += "<tr><td>" + data[i].UserId + "</td>";
                    html += "<td>" + data[i].Name + "</td>";
                    html += "<td>" + data[i].LeaveFromDate + "</td>";
                    html += "<td>" + data[i].LeaveTodate + "</td>";
                    html += "<td>" + data[i].Status + "</td>";
                    html += "<td>" + data[i].Reason + "</td>";
                    html += "</tr>";
                }
                $("#tblDepartmentEmployeesareinleav").html(html);
                $("#mymodalDepartmentEmployeesareinleave").modal('show');
            }
        },
        error: function () {
            alert("error")
        }
    });
}
function PostNewLeaveConfirmation() {
    PostLeavedata(currnetLeavesdatapost);
}

function validateLeave() {
    validate = true;
    var CompOff = $('input:checkbox[name=cbIsCompOff]').is(':checked');
    $(".css-error").removeClass('css-error');
    if (!CompOff) {
        if ($("#ddlLeaveType").val() == "") {
            $("#ddlLeaveType").addClass('css-error');
            validate = false;
        }
    }
    if (!CompOff) {
        if ($("#LeaveFromDate").val() == "") {
            $("#LeaveFromDate").addClass('css-error');
            validate = false;
        }
    } else {
        if ($("#DateofAvailableCompoff").val() == "") {
            $("#DateofAvailableCompoff").addClass('css-error');
            validate = false;
        }
    }

    if ($("#LeaveTodate").val() == "") {
        $("#LeaveTodate").addClass('css-error');
        validate = false;
    }
    if ($("#ReasonForLeave").val() == "") {
        $("#ReasonForLeave").addClass('css-error');
        validate = false;
    }
    //if ($("#AddressOnLeave").val() == "") {
    //    $("#AddressOnLeave").addClass('css-error');
    //    validate = false;
    //}
    //if Days should be gater than equal to 3 & the value of the drop down is null, On Replace of My Activites will show the error messege 
    //if (Getdays() >= 3 && ($("#Level1Approver").val() == "")) {
    //    $("#Level1Approver").addClass('css-error');
    //    validate = false;
    //}
}

function LoadLeaveStatus() {
    $('#myDataTable').dataTable({
        "bServerSide": true,
        "sAjaxSource": "/HR/AjaxMyLeavesView",
        "bProcessing": true,
        "bDestroy": true,
        "aoColumns": [
              //{ "sName": "LeaveId" },
                        { "sName": "LeaveTypeName" },
                        //{ "sName": "IsFullday" },
                        {
                            "sName": "IsFullday",
                            "bSortable": false,
                            "render": function (data) {
                                if (data == "True") {
                                    return "Full Day";
                                }
                                if (data == "False") {
                                    return "Half Day";
                                }
                            }
                        },
                        { "sName": "LeaveSessionDay" },
                        { "sName": "LeaveFromDate" },
                        { "sName": "LeaveTodate" },
                        { "sName": "TotalLeaves" },

                        { "sName": "Level1Approver" },
                        //{ "sName": "Level2Approver" },

                        { "sName": "LeaveStatus" },
                        { "sName": "LeaveCreatedOn" },
                        {
                            "sName": "LeaveId",
                            "bSortable": false,
                            "render": function (data) {

                                return '<div class="btn-group">' +
'<div class="btn btn-default dropdown-toggle" data-toggle="dropdown"><span class="glyphicon glyphicon-th-list"></span></div>' +
'<ul class="dropdown-menu dropdown-menu-right">' +
' <li><a href="javascript:void(0);" id="' + data + '" onclick="CancelLeave(this.id);">Cancel Leave</a></li><li><a href="javascript:void(0);" onclick="ViewDetails(this.id);" id="' + data + '">View Details</a></li>' +

'</ul></div>';
                            }
                        }

        ],
        "aLengthMenu": [[10, 20, 30, 50, 100, 150, 200, 500, 1000], [10, 20, 30, 50, 100, 150, 200, 500, 1000]],
        "sDom": 'T<"clear">lfrtip',
        "oTableTools": {
            "sSwfPath": "/swf/copy_csv_xls_pdf.swf",
            "aButtons": [
                "copy", "xls", "pdf"
            ]
        }

    }); Applyclassdt();
}

function CancelLeave(id) {

    var r = confirm("Are you sure!");
    if (r == true) {
        $.ajax({
            type: "POST",
            url: "/Hr/CancelLeave",
            data: "id=" + id,
            success: function (data) {
                alert(data);
                LoadLeaveStatus();
                GetMyLeaves();
            },
            error: function () {
                alert("error")
            }
        });
    }

}
function ViewDetails(id) {
    $.ajax({
        type: "POST",
        url: "/Hr/ViewLeave",
        data: "id=" + id,
        success: function (data) {
            $("#FormBody").html(data);
            $("#myModalWorkDone").modal("show");
        },
        error: function () {
            alert("error")
        }
    });

}
function EditLeaves(id) {
    $.ajax({
        type: "POST",
        url: "/Hr/EditLeave",
        data: "id=" + id,
        success: function (data) {
            var html = "<div style='width:800px;height:550px;padding:10px;'>";
            html += "<table class='table' style='width:100%;'>";
            html += "<tr><td><label>Leave Status</label></td><td> <div class='input-control text'><input type='text' id='LeaveStatus' class='LeaveStatus' disabled  value='" + data.LeaveStatus + "' /></div></td></tr>";
            html += "<tr><td><label>Leave Type</label></td><td> <div class='input-control text'><input type='text' id='LeaveTypeName' class='LeaveTypeName' disabled  value='" + data.LeaveTypeName + "' /></div></td></tr>";
            html += "<tr><td><label>From Date</label></td><td> <div class='input-control text'><input type='datetime' class='required txtonly' disabled title='LeaveFromDate' id='LeaveFromDate' value='" + data.LeaveFromDate + "' /></div></td></tr>";
            html += "<tr><td><label>To Date</label></td><td> <div class='input-control text'><input type='text' id='LeaveTodate' class='LeaveTodate'  value='" + data.LeaveTodate + "' disabled /></div></td></tr></tr>";
            html += "<tr><td><label>Reson For Leave</label></td><td> <div class='input-control text'><input type='text' id='ReasonForLeave' class='ReasonForLeave'  value='" + data.ReasonForLeave + "' disabled /></div></td></tr>";
            html += "<tr><td><label>Address On Leave</label></td><td> <div class='input-control text'><input type='text' id='AddressOnLeave' class='AddressOnLeave' disabled  value='" + data.AddressOnLeave + "' /></div></td></tr>";
            html += "<tr><td><label>Approval</label></td><td> <div class='input-control text'><input type='text' id='Level1Approver' class='Level1Approver'  value='" + GetNameofempbyempid(data.Level1Approver) + "' disabled /></div></td></tr></table></div>";
            $("#FormBody").html(html);
            $("#myModalWorkDone").modal("show");


        }
    });
}
function GetEmployeesListtobindnames() {
    $.ajax({
        'url': '/Hr/GetAllDepartmentsandemployees',
        'data': 'dept=1',
        'type': 'POST',
        success: function (result) {
            allemployeelist = result;

        }
    });
}
function GetNameofempbyempid(id) {
    for (var i = 0; i < allemployeelist.length; i++) {
        if (allemployeelist[i].CustomUserId == id)
            return allemployeelist[i].FirstName;
    }
}
function cbfullday_change() {
    $("#typeoftheday").val("");
    if (!$("#cbfullday").prop("checked")) {
        $("#trfromdate").addClass("trCompoff");
        $("#daytype").removeClass("trCompoff");
    }
    else {
        $("#trfromdate").removeClass("trCompoff");
        $("#daytype").addClass("trCompoff");
    }
}
function ApproveLeavesActionsbyemployee(data) {

    return '<div class="btn-group">' +
'<div class="btn btn-default dropdown-toggle" data-toggle="dropdown"><span class="glyphicon glyphicon-th-list"></span></div>' +
'<ul class="dropdown-menu dropdown-menu-right">' +
         // '<li><a href="javascript:void(0);" onclick="Testdata(this.id);" id="' + data + '">Test Data</a></li>' +
' <li><a href="javascript:void(0);" id="' + data + '" onclick="Approve(this.id);">Approve</a></li><li><a href="javascript:void(0);" id="' + data + '" onclick="Reject(this.id);">Reject</a></li><li><a href="javascript:void(0);" id="' + data + '" onclick="CancelLeave(this.id);">Cancel</a></li><li><a href="javascript:void(0);" onclick="ViewDetails(this.id);" id="' + data + '">View Details</a></li>' +
'</ul></div>';
}


function ApprovedDataTable() {
    $('#ApprovedDataTable').dataTable({
        "bServerSide": true,
        "sAjaxSource": "/HR/AjaxMyApprovedView",
        "bProcessing": true,
        "bDestroy": true,
        "aoColumns": [
                {
                    "sName": "LeaveId",
                    "bSortable": false,
                    "render": function (data) {
                        return "<input type='checkbox' id='" + data + "' class='cbleaveapprove' /> ";
                    }
                },
                        { "sName": "LeaveTypeName" },
                        {
                            "sName": "IsFullday",
                            "render": function (data) {
                                if (data == null || data == "True") {
                                    return "Full Day";
                                }
                                if (data == "False") {
                                    return "Half Day";
                                }
                            }
                        },
                        { "sName": "LeaveSessionDay" },
                        { "sName": "LeaveFromDate" },
                        { "sName": "LeaveTodate" },
                        { "sName": "TotalLeaves" },

                        { "sName": "UserName" },
                        { "sName": "DepartmentName" },
                        { "sName": "LeaveStatus" }, { "sName": "LeaveCreatedOn" },

                        {
                            "sName": "LeaveId",
                            "bSortable": false,
                            "render": function (data) {
                                return ApproveLeavesActionsbyemployee(data);
                            }
                        }
        ],
        "aLengthMenu": [[10, 20, 30, 50, 100, 150, 200, 500, 1000], [10, 20, 30, 50, 100, 150, 200, 500, 1000]],
        "sDom": 'T<"clear">lfrtip',
        "oTableTools": {
            "sSwfPath": "/swf/copy_csv_xls_pdf.swf",
            "aButtons": [
                "copy", "xls", "pdf"
            ]
        },
        dom: 'l<"toolbar">frtip',
        initComplete: function () {
            $("#ApprovedDataTable_length").append('&nbsp;&nbsp;&nbsp; <input type="button" id="btnsubmit" onclick="DoactionForLeaveApprove();" value="Approve" class="btn btn-success" /> <input type="button" id="btnsubmit" onclick="DoActionForRejectLeave();" value="Reject" class="btn btn-warning" />  <input type="button" onclick="DoActionCancelLeave();" id="btnsubmit" value="Cancel" class="btn btn-danger" />');
        }

    });
    $("#ApprovedDataTable_length").addClass("col-md-9");
    $("#ApprovedDataTable_info").addClass("col-md-3");
}
function DoactionForLeaveApprove() {
    var ids = GetCheckBoxCheckedIds();
    if (confirm("Are you sure you want to approve all")) {
        $.ajax({
            type: "GET",
            url: "/Hr/BulkApproveLave",
            data: "ids=" + ids,
            success: function (data) {
                ApprovedDataTable();
                GetMyLeaves();
                LoadOfMyApproveLeaveCount();
            },
            error: function () {
                alert("error")
            }
        });
    }
}
function DoActionForRejectLeave() {

    $("#myModalBulkLeaveReject").modal('show');

    //}
}
function SaveBulkDoActionForRejectLeave() {
    var ids = GetCheckBoxCheckedIds();
    if (confirm("Are you sure you want to Reject Selected Applications")) {
        var data = {
            ids: ids,
            comments: $("#txtBulkLeavecomments").val()
        }
        $.ajax({
            type: "GET",
            url: "/Hr/BulkRejectLeave",
            data: data,
            success: function (data) {
                ApprovedDataTable();
                GetMyLeaves();
                LoadOfMyApproveLeaveCount();
            },
            error: function () {
                alert("error")
            }
        });
    }
}

function DoActionCancelLeave() {
    //var ids = GetCheckBoxCheckedIds();
    //if (confirm("Are you sure you want to Cancel Selected Applications")) {
    $("#myModalBulkLeaveCancel").modal('show');
    //}
}
function SaveBulkDoActionCancelLeave() {
    var ids = GetCheckBoxCheckedIds();
    if (confirm("Are you sure you want to Cancel Selected Applications")) {
        var data = {
            ids: ids,
            comments: $("#txtBulkLeaveCancelcomments").val()
        }
        $.ajax({
            type: "GET",
            url: "/Hr/BulkCancelLeave",
            data: data,
            success: function (data) {
                ApprovedDataTable();
                GetMyLeaves();
                LoadOfMyApproveLeaveCount();
            },
            error: function () {
                alert("error")
            }
        });
    }
}

function Reject(id) {
    $(".css-error").removeClass('css-error');
    currentId = id;
    $("#myModalReject").modal("show");
}
function Approve(id) {
    var table = $('#ApprovedDataTable').DataTable();
    var data = table.rows().data();
    var arraycurrent = new Array();
    for (var i = 0; i < data.length; i++) {
        var cidata = data[i];
        if (cidata[11] == id) {
            arraycurrent = cidata;
        }
    }
    if (arraycurrent.length > 0) {
        if (arraycurrent[0] == "Sick Leave") {
            var LeaveFromDate = ConvertDatefromoneddmmyyytommddyyy(arraycurrent[3]);
            var LeaveToDate = ConvertDatefromoneddmmyyytommddyyy(arraycurrent[4]);
            var date1 = new Date((LeaveFromDate));
            var date2 = new Date((LeaveToDate));
            var timeDiff = Math.abs(date2.getTime() - date1.getTime());
            var diffDays = Math.ceil(timeDiff / (1000 * 3600 * 24));
            diffDays = diffDays + 1;
            if (diffDays > 3) {
                var msg = "Employee need to submit sickness certificate to hr from a registered medical practitioner, not below the rank of MBBS. ";
                if (confirm(msg)) {
                    $.ajax({
                        type: "GET",
                        url: "/Hr/ApproveLeave",
                        data: "id=" + id,
                        success: function (data) {
                            ApprovedDataTable();
                            GetMyLeaves();
                            LoadOfMyApproveLeaveCount();
                        },
                        error: function () {
                            alert("error")
                        }
                    });
                }
            } else {
                $.ajax({
                    type: "GET",
                    url: "/Hr/ApproveLeave",
                    data: "id=" + id,
                    success: function (data) {
                        ApprovedDataTable();
                        GetMyLeaves(); LoadOfMyApproveLeaveCount();
                    },
                    error: function () {
                        alert("error")
                    }
                });
            }

        } else {
            $.ajax({
                type: "GET",
                url: "/Hr/ApproveLeave",
                data: "id=" + id,
                success: function (data) {
                    ApprovedDataTable();
                    GetMyLeaves(); LoadOfMyApproveLeaveCount();
                },
                error: function () {
                    alert("error")
                }
            });
        }
    }
}

function SaveComments() {
    validate = true;
    $(".css-error").removeClass('css-error');
    if ($("#txtcomments").val() == "") {
        $("#txtcomments").addClass('css-error');
        validate = false;
    }
    if (validate) {
        var tdata = {
            id: currentId,
            comments: $("#txtcomments").val()
        };
        $.ajax({
            type: "POST",
            url: "/Hr/RejectLeave",
            data: tdata,
            success: function (data) {
                $("#myModalReject").modal("hide");
                ApprovedDataTable(); LoadOfMyApproveLeaveCount();
                $("#txtcomments").val("");
                GetMyLeaves();
            },
            error: function () {
                alert("error")
            }
        });
    }
    else {
        alert("Please fill all mandatory fields");
    }
}

function SavePermissions() {
    validatePermissions();

    if (validate) {
        var cdate = ConvertDatefromoneddmmyyytommddyyy($("#txtDate").val());
        var dataRow = {
            'date': cdate,
            'starttime': $("#FromTime").val(),
            'endtime': $("#ToTime").val(),
            'reason': $("#perReasonForLeave").val(),
            //'approval': $("#LevelApprover").val()
        };
        $.ajax({
            type: 'POST',
            url: "/Hr/SaveLeavePermission",
            data: dataRow,
            dataType: "json",
            success: function (response) {
                OpenSuccessAlertPopUpBox_ConfirmPopUpJS(response);
                if (response == 'Success') {
                    $("#myModalPermision").modal("hide");
                    clearAll();
                    GetPermissionsleaves();
                    GetMyLeaves();
                }

            },
            error: function () {
                alert('error');
            }
        });
    }
    else {
        OpenSuccessAlertPopUpBox_ConfirmPopUpJS("Please fill all mandatory fields");
    }
}

function validatePermissions() {

    validate = true;
    $(".css-error").removeClass('css-error');
    if ($("#txtDate").val() == "") {
        $("#txtDate").addClass('css-error');
        validate = false;
    }
    if ($("#FromTime").val() == "") {
        $("#FromTime").addClass('css-error');
        validate = false;
    }
    if ($("#ToTime").val() == "") {
        $("#ToTime").addClass('css-error');
        validate = false;
    }
    if ($("#perReasonForLeave").val() == "") {
        $("#perReasonForLeave").addClass('css-error');
        validate = false;
    }
    //if ($("#LevelApprover").val() == "") {
    //    $("#LevelApprover").addClass('css-error');
    //    validate = false;
    //}
}
function GetPermissionsleaves() {
    $('#tblpermission').dataTable({
        "bServerSide": true,
        "sAjaxSource": "/Hr/AjaxGetMyLeavePermission",
        "bProcessing": true,
        "bDestroy": true,
        "aLengthMenu": [[10, 20, 30, 50, 100, 150, 200, -1], [10, 20, 30, 50, 100, 150, 200, "All"]],
        //"aaSorting": [[0, "desc"]],
        "aoColumns": [
            { "sName": "PermissionDate" },
                   { "sName": "StartDate" },
                   { "sName": "EndDate" },
                   { "sName": "Requestapprovename" },
                   { "sName": "Status" },
                   { "sName": "Reason" },
                   { "sName": "CreatedOn" },
                   {
                       "sName": "id",
                       "bSortable": false,
                       "render": function (data) {

                           return '<div class="btn-group">' +
'<div class="btn btn-default dropdown-toggle" data-toggle="dropdown"><span class="glyphicon glyphicon-th-list"></span></div>' +
'<ul class="dropdown-menu dropdown-menu-right">' +
'  <li><a href="javascript:void(0);" id="' + data + '" onclick="CancelPermissions(this.id);">Cancel</a></li><li><a href="javascript:void(0);" onclick="ViewPermisionDetails(this.id);" id="' + data + '">View Details</a></li>' +
'</ul></div>';

                       }
                   }
        ],

    });
    $("#tblpermission_length").addClass("col-md-6");
    $("#tblpermission_length").addClass("col-md-6");
}
function GetPermissionsApproveleaves() {
    $('#tblApprovepermission').dataTable({
        "iDisplayLength": 10,
        "bServerSide": true,
        "sAjaxSource": "/Hr/AjaxGetApprovePermission",
        "bProcessing": true,

        "bAutoWidth": true, "bDestroy": true,
        "aLengthMenu": [[10, 20, 30, 50, 100, 150, 200, -1], [10, 20, 30, 50, 100, 150, 200, "All"]],
        //"aaSorting": [[0, "desc"]],
        "aoColumns": [


             { "sName": "UserName" },
             { "sName": "DepartmentName" },
             { "sName": "LocationName" },
                   { "sName": "PermissionDate" },
                   { "sName": "StartDate" },
                   { "sName": "EndDate" },

                   { "sName": "Requestapprovename" },
                   { "sName": "Status" },
                   { "sName": "Reason" }, { "sName": "CreatedOn" },

                   {
                       "sName": "id",
                       "bSortable": false,
                       "render": function (data) {
                           return ApproveLeavesActionsbyemployeePermissions(data);
                       }
                   }
        ],

    });
    $("#tblApprovepermission").addClass("col-md-6");
    $("#tblApprovepermission").addClass("col-md-6");
}
function ApproveLeavesActionsbyemployeePermissions(data) {
    //if ($("#LoginEmployeeType").val() == "Manager") {
    return '<div class="btn-group">' +
'<div class="btn btn-default dropdown-toggle" data-toggle="dropdown"><span class="glyphicon glyphicon-th-list"></span></div>' +
'<ul class="dropdown-menu dropdown-menu-right">' +
'  <li><a href="javascript:void(0);" id="' + data + '" onclick="RejectPermision(this.id);">Reject</a></li><li><a href="javascript:void(0);" id="' + data + '" onclick="ApprovePermision(this.id);">Approve</a></li><li><a href="javascript:void(0);" onclick="ViewPermisionDetails(this.id);" id="' + data + '">View Details</a></li>' +
'</ul></div>';
    //    } else {
    //        return '<div class="btn-group">' +
    //'<div class="btn btn-default dropdown-toggle" data-toggle="dropdown"><span class="glyphicon glyphicon-th-list"></span></div>' +
    //'<ul class="dropdown-menu dropdown-menu-right">' +
    //'  <li><a href="javascript:void(0);" onclick="ViewPermisionDetails(this.id);" id="' + data + '">View Details</a></li>' +
    //'</ul></div>';
    //    }
}
function newpermissionclick() {
    clearAll();
    $("#myModalPermision").modal("show");

}
function CancelPermissions(id) {
    if (confirm("Are you sure you want to Cancel")) {
        $.ajax({
            'url': '/Hr/DeletePermissions',
            'data': 'id=' + id,
            'type': 'POST',
            success: function (result) {
                alert(result);
                GetPermissionsleaves();
                GetMyLeaves();
            },
            error: function (response) {
                alert("error");
            }
        });
    }
}
function RejectPermision(id) {
    currentPerId = id;
    $("#myModalPermisionReject").modal("show");
}
function ApprovePermision(id) {
    $.ajax({
        type: "POST",
        url: "/Hr/ApprovePermision",
        data: "id=" + id,
        success: function (data) {
            GetPermissionsApproveleaves();
            GetMyLeaves();
        },
        error: function () {
            alert("error")
        }
    });
}
function SavePermisionComments() {
    validate = true;
    $(".css-error").removeClass('css-error');
    if ($("#txtPercomments").val() == "") {
        $("#txtPercomments").addClass('css-error');
        validate = false;
    }
    if (validate) {

        var tdata = {
            id: currentPerId,
            comments: $("#txtPercomments").val()
        };
        $.ajax({
            type: "POST",
            url: "/Hr/RejectPermision",
            data: tdata,
            success: function (data) {
                $("#myModalPermisionReject").modal("hide");
                GetPermissionsApproveleaves();
                GetMyLeaves();
            },
            error: function () {
                alert("error");
                $("#txtPercomments").val("");

            }
        });
    }
    else {
        alert("Please fill all mandatory fields");
    }
}

function ViewPermisionDetails(id) {
    $.ajax({
        type: "POST",
        url: "/Hr/ViewPermision",
        data: "id=" + id,
        success: function (data) {
            $("#PermisionBody").html(data);
            $("#myViewPermision").modal("show");
        },
        error: function () {
            alert("error")
        }
    });

}
function Applyclassdt() {
    $("#myDataTable_length").addClass("col-md-6");
    $("#myDataTable_info").addClass("col-md-6");
}
function GetMyLeaves() {
    $.ajax({
        type: "GET",
        url: "/Hr/GetMyLeavesCount",
        success: function (data) {
            var html = "";
            var count = 1;

            for (var i = 0; i < data.length; i++) {
                html += "<tr><th>" + count + " </th>";
                html += "<th>" + CheckNull(data[i].LeaveTypeName); + "</th>";
                //html += " <th>" + CheckNull(data[i].CountOfLeave); + " </th>";
                html += "  <th>" + CheckNull(data[i].AvailableLeave); + " </th>";
                //html += "  <th>" + functionDate(data[i].CreatedOn); + " </th>";
                //html += "  <th>" + functionDate(data[i].ExpireDate); + " </th>";
                html += "</tr>";
                count = count + 1;
            }
            $("#empleavestable").html(html);
            //GetComoffLeavesAvailable();
        },
        error: function () {
            alert("error")
        }
    });
}

function GetComoffLeavesAvailable() {
    $.ajax({
        type: "GET",
        url: "/Hr/GetComoffLeavesAvailable",
        success: function (data) {

            $("#lblCompOffLeaveBalance").html(data);
        },
        error: function () {
            alert("error")
        }
    });
}

function clearAll() {
    $("#ddlLeaveType").val("");
    $("#DateofAvailableCompoff").val("");
    $("#LeaveFromDate").val("");
    $("#LeaveTodate").val("");
    $("#ReasonForLeave").val("");
    $("#AddressOnLeave").val("");
    $("#Level1Approver").val("");
    $("#Level2Approver").val("");
    $("#txtDate").val("");
    $("#FromTime").val("");
    $("#ToTime").val("");
    $("#perReasonForLeave").val("");
    $("#LevelApprover").val("");
    bindLeaveDates();
}

function LoadEmployees() {

    $.ajax({
        type: "POST",
        url: "/Common/GetEmployeesByReportingmgr",
        success: function (data) {
            var options = [];
            options.push('<option value="">- Select Employee -</option>');
            for (var i = 0; i < data.length; i++) {
                options.push('<option value="' + data[i].CustomUserId + '">' + data[i].FirstName + " " + data[i].LastName + '</option>');
            }
            $("#ddlEmployees1").html(options.join(''));
        },
        error: function () {
            alert("error")
        }
    });
}


function SaveOtherLeave() {
    validateOtherLeave();

    if (validate) {

        var fullday = $('input:checkbox[name=cbfullday1]').is(':checked');
        var CompOff = $('input:checkbox[name=cbIsCompOff1]').is(':checked');

        var task = {
            LeaveTypeId: $("#ddlLeaveType1").val(),
            LeaveTypeName: $("#ddlLeaveType1 option:selected").text(),
            IsFullday: fullday,
            IsCompOff: CompOff,
            DateofAvailableCompoff: $("#DateofAvailableCompoff1").val(),
            LeaveFromDate: $("#LeaveFromDate1").val(),
            LeaveTodate: $("#LeaveTodate1").val(),
            ReasonForLeave: $("#ReasonForLeave1").val(),
            AddressOnLeave: $("#AddressOnLeave1").val(),
            UserId: $("#ddlEmployees1").val()

        };
        $.ajax({
            type: "POST",
            url: "/Hr/LeaveOtherLeave",
            data: task,
            success: function (data) {
                alert(data);
                if (data == "Success")
                    clearOtherAll();


            },
            error: function () {
                alert("error")
            }
        });
    }
    else {
        alert("Please fill all mandatory fields");
    }
}

function validateOtherLeave() {
    validate = true;
    $(".css-error").removeClass('css-error');
    if ($("#ddlLeaveType1").val() == "") {
        $("#ddlLeaveType1").addClass('css-error');
        validate = false;
    }
    if ($("#LeaveFromDate1").val() == "") {
        $("#LeaveFromDate1").addClass('css-error');
        validate = false;
    }
    if ($("#LeaveTodate1").val() == "") {
        $("#LeaveTodate1").addClass('css-error');
        validate = false;
    }
    if ($("#ReasonForLeave1").val() == "") {
        $("#ReasonForLeave1").addClass('css-error');
        validate = false;
    }
    //if ($("#AddressOnLeave1").val() == "") {
    //    $("#AddressOnLeave1").addClass('css-error');
    //    validate = false;
    //} 
    if ($("#ddlEmployees1").val() == " ") {
        $("#ddlEmployees1").addClass('css-error');
        validate = false;
    }
}

function clearOtherAll() {
    $("#ddlEmployees1").val("");
    $("#ddlLeaveType1").val("");
    $("#cbfullday1").val("");
    $("#cbIsCompOff1").val("");
    $("#DateofAvailableCompoff1").val("");
    $("#LeaveFromDate1").val("");
    $("#LeaveTodate1").val("");
    $("#ReasonForLeave1").val("");
    $("#AddressOnLeave1").val("");
    GetMyLeaves();
}
function cbcheckboxassigndev_changed() {
    //cbcheckall
    if ($("#selectall").prop("checked") == true) {
        $(".cbleaveapprove").each(function () {
            $(this).prop("checked", true);
        });
    } else {
        $(".cbleaveapprove").each(function () {
            $(this).prop("checked", false);
        });
    }
}

function GetCheckBoxCheckedIds() {
    var leaveid = '';
    $(".cbleaveapprove").each(function () {
        if ($(this).prop("checked")) {
            leaveid = leaveid + $(this).prop("id") + ',';
        }
    });
    return leaveid;
}
function CompOffEncashDataTable() {
    $('#CompOffEncashList_DataTable').dataTable({
        "bServerSide": true,
        "sAjaxSource": "/Hr/AjaxGetMyCompOffEncashes",
        "bProcessing": true,
        "bDestroy": true,
        "aoColumns": [
                 { "sName": "UserId" },

                        { "sName": "UserName" },
                        { "sName": "SubmitedOn" },
                        { "sName": "NoOfDays" },
                        { "sName": "Remarks" },
                        { "sName": "ReportingToName" },
                        { "sName": "IsApproved" },
                        { "sName": "HRApproved" },
                        { "sName": "HrRemarks" },
                        { "sName": "HrEnchashedDate" },
                        { "sName": "Status" },
                        { "sName": "CompOffEncashId" }
        ],
        "aLengthMenu": [[10, 20, 30, 50, 100, 150, 200, 500, 1000], [10, 20, 30, 50, 100, 150, 200, 500, 1000]],
        "sDom": 'T<"clear">lfrtip',
        "oTableTools": {
            "sSwfPath": "/swf/copy_csv_xls_pdf.swf",
            "aButtons": [
                "copy", "xls", "pdf"
            ]
        },
        "fnServerParams": function (aoData) {
            aoData.push(
                { "name": "locationid", "value": $("#ddlLocation").val() },
                { "name": "departmentid", "value": $("#ddlDepartment").val() },
                { "name": "fromdate", "value": $("#fromdate").val() },
                { "name": "todate", "value": $("#todate").val() },
                 { "name": "Emp", "value": $("#ddlEmployees").val() }
                 );
        }
    });
    $("#CompOffEncashList_DataTable_length").addClass("col-md-6");
    $("#CompOffEncashList_DataTable_info").addClass("col-md-6");
}
function myModalCompOffEnchashShow() {
    $("#myModalCompOffEnchash").modal('show');
    $("#lblcompoffactuvalbalance").html("Note - The Days should be less than or equal to - " + $("#lblCompOffLeaveBalance").html());

}
function SaveCompOffEncash() {
    if ($("#txtcommentscomoffenchash").val() != null && $("#txtcommentscomoffenchash").val() != "" && $("#txtcompoffencash").val() != null && $("#txtcompoffencash").val() != "") {
        var Acount = parseInt($("#lblCompOffLeaveBalance").html());

        var Rcount = parseInt($("#txtcompoffencash").val());
        if (Rcount <= Acount) {
            var task = {
                remarks: $("#txtcommentscomoffenchash").val(),
                count: $("#txtcompoffencash").val()
            };
            $.ajax({
                type: "GET",
                url: "/Hr/SaveCompOffEncash",
                data: task,
                success: function (data) {
                    alert(data);
                    if (data == "Success") {
                        GetComoffLeavesAvailable();
                        $("#myModalCompOffEnchash").modal('hide');
                        CompOffEncashDataTable();
                    }
                },
                error: function () {
                    alert("error")
                }
            });
        } else {
            alert("Please check the days should be less than");
        }
    } else {
        alert("Please enter all the required fields");
    }
}