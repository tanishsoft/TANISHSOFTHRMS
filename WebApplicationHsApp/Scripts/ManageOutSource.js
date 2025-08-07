var currentempid = 0;
var UserList_PageLevel, EdiUserID_PageLevel;
$(document).ready(function () {
    LoadData();
   
    $('.datetotext').datepicker({
        changeMonth: true,
        changeYear: true,
        // dateFormat: "dd/mm/yy",
        maxDate: "0",
        yearRange: "-100:+0",
    });

});
function LoadData() {
    $('#tbltable').dataTable({
        "bServerSide": true,
        "sAjaxSource": "/Admin/AjaxGetOutSources",
        "bProcessing": true,

        "bDestroy": true,
        "aLengthMenu": [[10, 20, 30, 50, 100, 150, 200, -1], [10, 20, 30, 50, 100, 150, 200, "All"]],
        //"aaSorting": [[0, "desc"]],
        "aoColumns": [
                   //{ "sName": "UserId" },
                   { "sName": "LocationName" },
                   { "sName": "FirstName" },
                   { "sName": "CustomUserId" },
                   { "sName": "EmailId" },
                   //{ "sName": "PlaceAllocation" },
                   { "sName": "PhoneNumber" },
                    //{ "sName": "Extenstion" },
                   { "sName": "DepartmentName" }, { "sName": "SubDepartmentName" },
            { "sName": "Designation" },
            { "sName": "Id" },
                   {
                       "sName": "UserId",
                       "bSortable": false,
                       "render": function (data) {
                           return '<div class="btn-group">' +
'<div class="btn btn-default dropdown-toggle" data-toggle="dropdown"><span class="glyphicon glyphicon-th-list"></span></div>' +
'<ul class="dropdown-menu dropdown-menu-right">' +

//'<li><a href="javascript:void(0);" id="' + data + '" onclick="EditEmployeeData_PageLevel(this.id);">Edit Employee</a></li>' +
'<li><a href="javascript:void(0);" id="' + data + '" onclick="DeleteEmployee(this.id);">Delete Employee</a></li>' +

'<li><a href="javascript:void(0);" id="' + data + '" onclick="ResetPassword(this.id);">Reset Password</a></li>' +
'<li><a href="javascript:void(0);" id="' + data + '" onclick="CreateLogin(this.id);">Create Login Option</a></li>' +
'<li><a href="javascript:void(0);" id="' + data + '" onclick="RemoveLoginForuser(this.id);">Remove Login Option</a></li>' +

'</ul></div>';
                       }
                   }
        ],
        "fnServerParams": function (aoData) {
            //aoData.push(
            //    { "name": "locationid", "value": $("#ddlLocationForFilter").val() },
            //    { "name": "departmentid", "value": $("#ddlDepartmentForFilter").val() }
            //);
        }

    });
    $("#tbltable_length").addClass("col-md-6");
    $("#tbltable_length").addClass("col-md-6");
}
function AssignRole(id) {
    currentempid = id;
    $("#ddlRole").val("");
    $("#myModal").modal('show');
}

function LoadCommonDepartments() {
    $.ajax({
        type: "POST",
        url: "/Common/GetCommonDepartments",
        //data: "id=",
        success: function (data) {
            var options = [];
            options.push('<option value="">- All Departments -</option>');
            for (var i = 0; i < data.length; i++) {
                options.push('<option value="' + data[i].CommonDepartmentId + '">' + data[i].Name + '</option>');
            }
            $("#ddlDepartmentForFilter").html(options.join(''));
        },
        error: function () {
            alert("error")
        }
    });
}
function SaveRoleToEmployee() {
    var task = {
        empid: currentempid,
        role: $("#ddlRole").val()
    };
    $.ajax({
        type: "POST",
        url: "/Account/AssignRoleToemployee",
        data: task,
        success: function (data) {
            alert(data);
            $("#myModal").modal("hide");
        },
        error: function () {
            alert("error")
        }
    });
}
function DeleteEmployee(id) {
    currentempid = id;
    $('#DateOfLeaving').datepicker({
        changeMonth: true,
        changeYear: true,
        //dateFormat: "dd/mm/yy",
        maxDate: "0",
        yearRange: "-100:+0",
    });
    $("#DeleteEmployeeModal").modal('show');
}
function ConfirmDeleteEmployee() {
    var task = {
        empid: currentempid,
        DateOfLeaving: $("#DateOfLeaving").val()
    };
    $.ajax({
        type: "POST",
        url: "/Account/DeleteOutsource",
        data: task,
        success: function (data) {
            $("#DeleteEmployeeModal").modal('hide');
            alert(data);
            LoadData();
        },
        error: function () {
            alert("error")
        }
    });
}
function TransferEmployee(id) {
    currentempid = id;
    LoadLocation();
    // LoadDepartment();
    $("#myTransfer").modal('show');
}

function SaveTransferEmployee() {
    if (ValidateRequiredFieldsByClassName_GlobalJS("TransferRequiredData")) {

        var skillsSelect = document.getElementById("ddlLocation");
        var selectedText = skillsSelect.options[skillsSelect.selectedIndex].text;
        var deptSelect = document.getElementById("ddlDepartment");
        var deptText = deptSelect.options[deptSelect.selectedIndex].text;
        var deptsubSelect = document.getElementById("ddlSubDepartment");
        var SubdeptText = deptsubSelect.options[deptsubSelect.selectedIndex].text;
        var SubDepartmentName = "";
        if ($("#ddlSubDepartment").val() != null && $("#ddlSubDepartment").val() != "") {
            SubDepartmentId = $("#ddlSubDepartment").val();
            SubDepartmentName = SubdeptText;
        }
        var user = {
            CustomUserId: currentempid,
            UserId: currentempid,
            LocationId: $("#ddlLocation").val(),
            DepartmentId: $("#ddlDepartment").val(),
            LocationName: selectedText,
            DepartmentName: deptText,
            SubDepartmentId: $("#ddlSubDepartment").val(),
            SubDepartmentName: SubDepartmentName
        };
        $.ajax({
            type: "POST",
            url: "/Hr/SaveTransferEmployee",
            data: user,
            success: function (data) {
                alert(data);
                $("#myTransfer").modal("hide");
                LoadData();
            },
            error: function () {
                alert("error")
            }
        });
    }
    else {
        alert("Fields Required");
    }
}
function LoadLocation() {
    $.ajax({
        type: "POST",
        url: "/Common/GetLocations",
        //data: "id=",
        success: function (data) {
            var options = [];
            options.push('<option value="">- All Locations -</option>');
            for (var i = 0; i < data.length; i++) {
                options.push('<option value="' + data[i].LocationId + '">' + data[i].LocationName + '</option>');
            }
            $("#ddlLocation").html(options.join(''));
            $("#ddlLocationForFilter").html(options.join(''));
        },
        error: function () {
            alert("error")
        }
    });
}
function LoadDepartment() {
    if ($("#ddlLocation").val() != null && $("#ddlLocation").val() != " ") {
        $.ajax({
            type: "POST",
            url: "/Common/GetDepartmentByLocation",
            data: "id=" + $("#ddlLocation").val(),
            success: function (data) {
                var options = [];
                options.push('<option value="">- Select Department -</option>');
                for (var i = 0; i < data.length; i++) {
                    options.push('<option value="' + data[i].DepartmentId + '">' + data[i].DepartmentName + '</option>');
                }
                $("#ddlDepartment").html(options.join(''));

            },
            error: function () {
                alert("error")
            }
        });
    } else {
        var options = [];
        options.push('<option value="">- Select Department -</option>');
        $("#ddlDepartment").html(options.join(''));
    }
}
function DepartmentChange() {
    if ($("#ddlDepartment").val() != null && $("#ddlDepartment").val() != " ") {
        $.ajax({
            type: "POST",
            url: "/Common/GetSubDepartmentByDepartment",
            data: "id=" + $("#ddlDepartment").val(),
            success: function (data) {
                var options = [];
                options.push('<option value="">- Select Sub Department -</option>');
                for (var i = 0; i < data.length; i++) {
                    options.push('<option value="' + data[i].SubDepartmentId + '">' + data[i].Name + '</option>');
                }
                $("#ddlSubDepartment").html(options.join(''));
            },
            error: function () {
                alert("error")
            }
        });
    } else {
        var options = [];
        options.push('<option value="">- Select Sub Department -</option>');

        $("#ddlSubDepartment").html(options.join(''));
    }
}
function ResetPassword(id) {
    currentempid = id;
    $("#ResetPasswordEmployeeModal").modal('show');
}
function CreateLogin(id) {
    currentempid = id;
    $.get("/Account/CreateLoginForOutsourceByUserID", { UserID: id }, function (Result) { OpenSuccessAlertPopUpBox_ConfirmPopUpJS(Result); LoadData(); });
}
function RemoveLoginForuser(id) {
    if (confirm("Are you sure you want to remove the login")) {
        var dataparam = {
            UserId: id
        }
        $.ajax({
            type: "GET",
            url: "/Account/RemoveLoginForOutSourceUserByUserID",
            data: dataparam,
            success: function (data) {
                alert(data);
            },
            error: function () {
                alert("error")
            }
        });
    }
}
function DeactivateEmployee(id) {
    if (confirm("Are you sure you want to deactive the employee")) {
        var dataparam = {
            UserId: id
        }
        $.ajax({
            type: "GET",
            url: "/HrAdmin/DeactivateEmployee",
            data: dataparam,
            success: function (data) {
                alert(data);
            },
            error: function () {
                alert("error")
            }
        });
    }
}
function UpdatePasswordToEmployee() {
    if ($("#Password").val() == $("#ConfirmPassword").val()) {

        var dataparam = {
            UserId: currentempid,
            Password: $("#Password").val(),
            ConfirmPassword: $("#ConfirmPassword").val()
        }
        $.ajax({
            type: "GET",
            url: "/Account/UpdatePasswordFromAdminOutSource",
            data: dataparam,
            success: function (data) {

                $("#Password").val('');
                $("ConfirmPassword").val('');
                alert(data);
                $("#ResetPasswordEmployeeModal").modal('hide');
            },
            error: function () {
                alert("error")
            }
        });
    } else {
        alert("Please Check Password and Confirm Password should be same");
    }
}
function UpdatePasswordToAllEmployee() {
    if ($("#txtPassword").val() == $("#txtConfirmPassword").val()) {

        var dataparam = {
            Password: $("#txtPassword").val(),
            Confirmpassord: $("#txtConfirmPassword").val()
        }
        $.ajax({
            type: "GET",
            url: "/Account/ResetAllPasswords",
            data: dataparam,
            success: function (data) {
                $("#txtPassword").val('');
                $("#txtConfirmPassword").val('');
                alert(data);
                $("#ResetPasswordToAllEmployeeModal").modal('hide');
            },
            error: function () {
                alert("error")
            }
        });
    } else {
        alert("Please Check Password and Confirm Password should be same");
    }
}
function LoadDepartmentForFilter() {
    if ($("#ddlLocationForFilter").val() != null && $("#ddlLocationForFilter").val() != "") {
        $.ajax({
            type: "POST",
            url: "/Common/GetDepartmentByLocation",
            data: "id=" + $("#ddlLocationForFilter").val(),
            success: function (data) {
                var options = [];
                options.push('<option value="">- All Departments -</option>');
                for (var i = 0; i < data.length; i++) {
                    options.push('<option value="' + data[i].DepartmentId + '">' + data[i].DepartmentName + '</option>');
                }
                $("#ddlDepartmentForFilter").html(options.join(''));
            },
            error: function () {
                alert("error")
            }
        });
    } else {
        var options = [];
        options.push('<option value="">- Select Department -</option>');

        $("#ddlDepartmentForFilter").html(options.join(''));
    }
}

function GetAllUserListToAdmin_PageLevel() {
    UserList_PageLevel = new Array();
    $.get("/HrAdmin/GetAllUserListToAdmin_HRAdmin", function (Result) {
        UserList_PageLevel = Result;
    });
}

function EditEmployeeData_PageLevel(UserId) {
    EdiUserID_PageLevel = "";
    if ($.isNumeric(UserId)) {
        for (var i in UserList_PageLevel) {
            if (UserList_PageLevel[i].UserId == parseFloat(UserId)) {
                EdiUserID_PageLevel = UserId;
                $("#EditEmployeeDataModal").modal('show');
                $("#txtEditUserName").val(UserList_PageLevel[i].FirstName);
                $("#txtEditLocationName").val(UserList_PageLevel[i].LocationName);
                $("#txtEditDepartmentName").val(UserList_PageLevel[i].DepartmentName);
                $("#txtEditSubDepartmentName").val(UserList_PageLevel[i].SubDepartmentName);
                $("#txtEditMobileNumber").val(UserList_PageLevel[i].PhoneNumber);
                $("#txtEditEmailID").val(UserList_PageLevel[i].EmailId);
                $("#DDLGender").val(UserList_PageLevel[i].Gender);
                $("#DDLEditDesignation").val(UserList_PageLevel[i].DesignationID);
                $("#txtDateOfjoining").val(functionDate(UserList_PageLevel[i].DateOfJoining));
                $("#txtDateOfBirth").val(functionDate(UserList_PageLevel[i].DateOfBirth));
                $("#ddlemployeetype").val(UserList_PageLevel[i].UserType);
                break;
            }
        }
    }
}
function LoadDesignationData() {
    $.ajax({
        type: "GET",
        url: "/Common/GetDesignationListToAdmin",
        //data: "id=" + $("#ddlDepartment1").val(),
        success: function (data) {
            var options = [];
            options.push('<option value="">- Select Designation -</option>');
            for (var i = 0; i < data.length; i++) {
                options.push('<option value="' + data[i].ID + '">' + data[i].Designation_Name + '</option>');
            }
            $("#DDLEditDesignation").html(options.join(''));
        }
    });
}

function ProceedToEditEmployeeData_PageLevel() {
    if ($.isNumeric(EdiUserID_PageLevel) && ValidateRequiredFieldsByClassName_GlobalJS("EditUserRequiredField")) {
        var Dataparam = {
            UserId: EdiUserID_PageLevel,
            FirstName: $("#txtEditUserName").val(),
            EmailId: $("#txtEditEmailID").val(),
            PhoneNumber: $("#txtEditMobileNumber").val(),
            Gender: $("#DDLGender").val(),
            Designation: $("#DDLEditDesignation option:selected").text(),
            DesignationID: $("#DDLEditDesignation").val(),
            //DateOfBirth: $("#txtDateOfBirth").val(),
            //DateOfJoining: $("#txtDateOfjoining").val(),
            UserType: $("#ddlemployeetype").val()
        };
        $.post("/HrAdmin/EditUserDataByUserID_HRAdmin", { "Data": JSON.stringify(Dataparam) }, function (Result) {
            $("#EditEmployeeDataModal").modal('hide');
            OpenSuccessAlertPopUpBox_ConfirmPopUpJS(Result);
            LoadData();
            GetAllUserListToAdmin_PageLevel();

            DateOfBirth: $("#txtDateOfBirth").val('');
            DateOfJoining: $("#txtDateOfjoining").val();

            //GetAllUserEmailListToAdmin_GlobalFunctionJS();
        });
    }
}

function ValidateEmailIDForEdit_PageLevel(ElementID) {
    var EnteredEmailID = $("#" + ElementID).val();
    if (ValidateEmailFormat_GlobalValidationJS(EnteredEmailID)) {
        var EmailExistStatus = CheckUserEmailIDExistsOrNotForEdit_GlobalFunJS(EnteredEmailID, EdiUserID_PageLevel);
        if (!EmailExistStatus) {
            alert("Email ID Already Exist. Please Enter Another Email ID");
            $("#" + ElementID).val('');
        }
    } else {
        alert("Please Enter Valid Email ID");
        $("#" + ElementID).val('');
    }
}
function ExcelExpoertInavtiveEmp() {
    window.location = "/HrAdmin/ExportToExcelEmployeesWhoHaveLogin";
}
function ViewHistoryOfEmpTransfer(id) {
    $.get("/HrAdmin/GetEmployeeHistory/" + id, function (data) {
        var html = "";
        for (var i = 0; i < data.length; i++) {
            html += "<div class'well well-s'> Location : " + data[i].LocationName + " -  Department : " + data[i].DepartmentName + " -  Updated On : " + data[i].MovedDate + "  - by " + data[i].UpdatedBy + "</div>";
            html += "<span style='text-align:center;width:100%;' class='glyphicon glyphicon-arrow-down'></span>";
        }
        $("#DivemployeeTransferHistory").html(html);
        $("#modalEmployeeTransferHistory").modal('show');
    });
}
var currentempidtomanage = 0;
function UpdateRemarksOfEmp(id) {
    currentempidtomanage = id;
    $.get("/HrAdmin/GetRemarksOfEmployee/" + id, function (data) {
        $("#modalEmployeeUpdateRemarksnew").modal('show');
        $("#txtupdateemployeeremarsk").val(data.Comments);

    });
}
function UpdateRemarksofemppost() {
    $.get("/HrAdmin/UpdateRemarksOfEmployee/", {
        id: currentempidtomanage,
        Remarks: $("#txtupdateemployeeremarsk").val()
    }, function (data) {
        $("#modalEmployeeUpdateRemarks").modal('hide');
        alert("success");
    });
}