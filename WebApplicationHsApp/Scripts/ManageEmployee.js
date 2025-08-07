var currentempid = 0;
var UserList_PageLevel, EdiUserID_PageLevel;
$(document).ready(function () {
    $("#ddlEditReportingManagerId").select2({
        ajax: {
            url: "/Chat/GetEmployeeSearch",
            type: "get",
            dataType: 'json',

            //delay: 250,
            data: function (params) {
                return {
                    searchTerm: params.term // search term
                };
            },
            processResults: function (response) {
                return {
                    results: response
                };
            },
            cache: true
        },
        minimumInputLength: 3
    });
    LoadLocation();
    //GetAllUserListToAdmin_PageLevel();
    LoadDesignationData();
    $('.datetotext').datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy",
        maxDate: "0",
        yearRange: "-100:+10",
    });
    LoadCommonDepartments();
    LoadData();
    LoadData_Inactive();
    LoadData_Hold();
});
var loadFile = function (event) {
    var reader = new FileReader();
    reader.onload = function () {
        var output = document.getElementById('Editoutput');
        output.src = reader.result;
    };
    reader.readAsDataURL(event.target.files[0]);
};
var oTable;
function LoadData() {

    oTable = $('#tbltable').dataTable({
        "bServerSide": true,
        "sAjaxSource": "/HrAdmin/AjaxGetEmployees",
        "bProcessing": true,

        "bDestroy": true,
        //"scrollX": true,
        "aLengthMenu": [[10, 20, 30, 50, 100, 150, 200, -1], [10, 20, 30, 50, 100, 150, 200, "All"]],
        //"aaSorting": [[0, "desc"]],
        "aoColumns": [
            //{ "sName": "UserId" },
            { "sName": "LocationName" },
            {
                "sName": "FirstName",
                "bSortable": false,
                "render": function (data, type, row) {
                    return '<a href="javascript:void(0);" id="' + row[9] + '" onclick="Redirecttoempprofile(this.id);">' + data + '</a>';
                }
            },
            { "sName": "CustomUserId" },
            { "sName": "EmailId" },
            //{ "sName": "PlaceAllocation" },
            { "sName": "PhoneNumber" },
            //{ "sName": "Extenstion" },
            { "sName": "DepartmentName" }, { "sName": "SubDepartmentName" },
            { "sName": "Designation" },
            { "sName": "RM" },
            { "sName": "FoodType" },
            {
                "sName": "UserId",
                "bSortable": false,
                "render": function (data) {
                    return '<div class="btn-group">' +
                        '<div class="btn btn-default dropdown-toggle" data-toggle="dropdown"><span class="glyphicon glyphicon-th-list"></span></div>' +
                        '<ul class="dropdown-menu dropdown-menu-right">' +
                        '<li><a href="javascript:void(0);" id="' + data + '" onclick="PrintIDCard(this.id);">Print IDCard</a></li> ' +
                        '<li><a href="javascript:void(0);" id="' + data + '" onclick="AssignRole(this.id);">Assign Role</a></li> ' +
                        '<li><a href="javascript:void(0);" id="' + data + '" onclick="EditEmployeeData_PageLevel(this.id);">Edit Employee</a></li>' +
                        '<li><a href="javascript:void(0);" id="' + data + '" onclick="DeleteEmployee(this.id);">Delete/DeActivate Employee</a></li>' +
                        //'<li><a href="javascript:void(0);" id="' + data + '" onclick="DeactivateEmployee(this.id);">De Activate Employee</a></li>' +
                        '<li><a href="javascript:void(0);" id="' + data + '" onclick="TransferEmployee(this.id);">Transfer Employee</a></li>' +
                        '<li><a href="javascript:void(0);" id="' + data + '" onclick="ResetPassword(this.id);">Reset Password</a></li>' +
                        '<li><a href="javascript:void(0);" id="' + data + '" onclick="CreateLogin(this.id);">Create Login Option</a></li>' +
                        '<li><a href="javascript:void(0);" id="' + data + '" onclick="RemoveLoginForuser(this.id);">Remove Login Option</a></li>' +
                        '<li><a href="javascript:void(0);" id="' + data + '" onclick="ViewHistoryOfEmpTransfer(this.id);">View Transfer History</a></li>' +
                        '<li><a href="javascript:void(0);" id="' + data + '" onclick="UpdateRemarksOfEmp(this.id);">Update Remarks</a></li>' +
                        '<li><a href="javascript:void(0);" id="' + data + '" onclick="updateEmployeeFoodType(this.id);">Update FoodType</a></li>' +
                        '<li><a href="javascript:void(0);" id="' + data + '" onclick="changeapplogin(this.id);">App Login</a></li>' +
                        '<li><a href="javascript:void(0);" id="' + data + '" onclick="changeServiceAnniversary(this.id);">Send Remainder</a></li>' +                        
                        '<li><a href="javascript:void(0);" id="' + data + '" onclick="generatePin(this.id);">GeneratePin</a></li>' +
                        '</ul></div>';
                }
            }
        ],
        "fnServerParams": function (aoData) {
            aoData.push(
                { "name": "locationid", "value": $("#ddlLocationForFilter").val() },
                { "name": "departmentid", "value": $("#ddlDepartmentForFilter").val() }
            );
        }

    });
    $("#tbltable_length").addClass("col-md-6");
    $("#tbltable_length").addClass("col-md-6");
}
function updateEmployeeFoodType(id) {
    currentempid = id;
    $("#modalEmployeeUpdateFoodType").modal('show');
}
function UpdateFoodTypeOfEmployee() {
    $.get("/HrAdmin/UpdateFoodTypeOfEmployee", { id: currentempid, FoodType: $("#ddlFoodType").val() }, function (Result) {
        $("#modalEmployeeUpdateFoodType").modal('hide');
        oTable.api().ajax.reload(null, false);
        //alert(Result);
    });
}
//
function ClicklistItem(clsname, userid) {

    validate = true;
    $(".css-error").removeClass('css-error');
    $(".active").removeClass('active');
    $("#" + clsname).addClass('active');
    validate = true;
    $(".panel-success").css("display", "none");
    var id = "." + clsname;
    $(id).css("display", "block");
    if (clsname == 'employee') {
        EditEmployeeData_PageLevel(userid);
    }
    if (clsname == 'Assignrole') {
        AssignRole(userid);
    }
    if (clsname == 'myTransfer') {
        currentempid = userid;
        LoadLocation();
    }
    if (clsname == 'DeleteEmployee') {
        DeleteEmployee(userid);
    }
    if (clsname == 'ResetPasswordEmployee') {
        currentempid = userid;
    }
    if (clsname == 'EmployeeTransferHistory') {
        ViewHistoryOfEmpTransfer(userid);
    }
    if (clsname == 'EmployeeUpdateRemarksnew') {
        UpdateRemarksOfEmp(userid);
    }
    if (clsname == 'Employeeactiveinactive') {
        changeapplogin(userid);
    }
    if (clsname == 'createlogin') {
        CreateLogin(userid);
    }
    if (clsname == 'removelogin') {
        RemoveLoginForuser(userid);
    }
}
function AssignRole(id) {
    currentempid = id;
    $("#ddlRole").val("");
    $("#myModal").modal('show');
}
function PrintIDCard(id) {
    var url = "/HrAdmin/PrintCard?Id=" + id;
    window.open(url);
}
function Redirecttoempprofile(id) {

    var url = "/EmployeeProfile/EmployeeProfile?UserId=" + id;
    window.location.href = url;
}
function changeapplogin(id) {
    currentempid = id;
    $("#ddlselectstatusofapplogin").val("");
    $("#modalEmployeeactiveinactive").modal('show');
}

function changetheappstatus() {
    $.get("/HrAdmin/UpdateApploginstatus", { id: currentempid, status: $("#ddlselectstatusofapplogin").val() }, function (Result) {
        $("#modalEmployeeactiveinactive").modal('hide');
        oTable.api().ajax.reload(null, false);
        alert(Result);
    });
}

function changeServiceAnniversary(id) {
    currentempid = id;
    $("#ddlselectstatusofServiceAnniversary").val("");
    $("#modalEmployeeServiceAnniversary").modal('show');
}

function changetheServiceAnniversary() {
    $.get("/HrAdmin/UpdateServiceAnniversarystatus", { id: currentempid, status: $("#ddlselectstatusofServiceAnniversary").val() }, function (Result) {
        $("#modalEmployeeServiceAnniversary").modal('hide');
        oTable.api().ajax.reload(null, false);
        alert(Result);
    });
}

function generatePin(id) {
    $.get("/HrAdmin/GeneratePin?userId=" + id, function (Result) {
        alert("Pin :" + Result);

    });
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
var updatereportingmanager = false;
function DeleteEmployee(id) {
    currentempid = id;

    $.get("/HrAdmin/ValidateTheUserIsReportingManagerOrNot", { empid: id }, function (data) {
        updatereportingmanager = data;
        if (data) {
            $("#tralrmid").css("display", "inline");
        }
        $('#DateOfLeaving').datepicker({
            changeMonth: true,
            changeYear: true,
            dateFormat: "dd/mm/yy",
            maxDate: "0",
            yearRange: "-100:+0",
        });
        $("#DeleteEmployeeModal").modal('show');
    });
}
function ConfirmDeleteEmployee() {
    var task = {
        empid: currentempid,
        DateOfLeaving: $("#DateOfLeaving").val(),
        Rmanager: $("#ddlEmployeesForFilterreporting").val()
    };
    $.ajax({
        type: "POST",
        url: "/Account/DeleteEmployee",
        data: task,
        success: function (data) {
            $("#DeleteEmployeeModal").modal('hide');
            alert(data);
            oTable.api().ajax.reload(null, false);
        },
        error: function () {
            alert("error")
        }
    });
}
function TransferEmployee(id) {
    currentempid = id;
    LoadLocation();
    EmployeeSearch();
    LoadDesignationData();
    $("#myTransfer").modal('show');
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
            $("#DesignationID").html(options.join(''));
        }
    });
}
function EmployeeSearch() {
    $("#TranferRMEmpId").select2({
        ajax: {
            url: "/Chat/GetEmployeeSearch",
            type: "get",
            dataType: 'json',
            //delay: 250,
            data: function (params) {
                return {
                    searchTerm: params.term // search term
                };
            },
            processResults: function (response) {
                return {
                    results: response
                };
            },
            cache: true
        },
        minimumInputLength: 3
    });

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
            SubDepartmentName: SubDepartmentName,
            DesignationID: $("#DesignationID").val(),
            ReportingManagerId: $("#TranferRMEmpId").val(),
            Designation: $("#DesignationID :selected").text()
        };
        $.ajax({
            type: "POST",
            url: "/Hr/SaveTransferEmployee",
            data: user,
            success: function (data) {
                alert(data);
                $("#myTransfer").modal("hide");
                oTable.api().ajax.reload(null, false);
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
            $("#ddlLocationForRepaorting").html(options.join(''));

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
    $.get("/Account/CreateLoginForUserByUserID", { UserID: id }, function (Result) { OpenSuccessAlertPopUpBox_ConfirmPopUpJS(Result); LoadData(); });
}
function RemoveLoginForuser(id) {
    if (confirm("Are you sure you want to remove the login")) {
        var dataparam = {
            UserId: id
        }
        $.ajax({
            type: "GET",
            url: "/Account/RemoveLoginForUserByUserID",
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
            url: "/Account/UpdatePasswordFromAdmin",
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
    //if ($.isNumeric(UserId)) {
    //    for (var i in UserList_PageLevel) {
    //        if (UserList_PageLevel[i].UserId == parseFloat(UserId)) {

    $.get("/HrAdmin/GetEmployeeInfoById", { id: UserId }, function (data) {
        EdiUserID_PageLevel = UserId;
        $("#EditPhoto").val("");
        $("#EditEmployeeDataModal").modal('show');
        var image = $('#Editoutput');
        if (data.photo != null)
            image.attr('src', '/Documents/Images/' + data.photo);
        else
            image.attr('src', "");
        $("#txtEditUserName").val(data.FirstName);
        $("#txtEditLocationName").val(data.LocationName);
        $("#txtEditDepartmentName").val(data.DepartmentName);
        $("#txtEditSubDepartmentName").val(data.SubDepartmentName);
        $("#txtEditMobileNumber").val(data.PhoneNumber);
        $("#txtEditEmailID").val(data.EmailId);
        $("#DDLGender").val(data.Gender);
        $("#DDLEditDesignation").val(data.DesignationID);
        $("#txtDateOfjoining").val(data.DateOfJoining);
        $("#txtDateOfBirth").val(data.DateOfBirth);
        $("#ddlemployeetype").val(data.UserType);
        $("#IsOnRollDoctor").prop("checked", data.IsOnRollDoctor);
        $("#IsOffRollDoctor").prop("checked", data.IsOffRollDoctor);
        $("#txtEditAdhaarCard").val((data.AdhaarCard));
        $("#txtEditPanCard").val((data.PanCard));
        $("#txtQualification").val((data.Qualification));
        $("#txtEditCollegeName").val((data.CollageName));
        
        if (data.ReportingManagerId != 0)
            $("#ddlEditReportingManagerId").select2("trigger", "select", { data: { id: data.ReportingManagerId, text: data.strtbl_ReportingManager } });
        else
            $("#ddlEditReportingManagerId").select2("trigger", "select", { data: { id: "", text: "" } });
    });
    //break;
    //}
    //}
    //}
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
            $("#DesignationID").html(options.join(''));
        }
    });
}

function ProceedToEditEmployeeData_PageLevel() {
    if ($.isNumeric(EdiUserID_PageLevel) && ValidateRequiredFieldsByClassName_GlobalJS("EditUserRequiredField")) {
        var reportingmanagerid = "0";
        if ($("#ddlEditReportingManagerId").val() != null && $("#ddlEditReportingManagerId").val() != "") {
            reportingmanagerid = $("#ddlEditReportingManagerId").val();
        }
        var Dataparam = {
            UserId: EdiUserID_PageLevel,
            AdhaarCard: $("#txtEditAdhaarCard").val(),
            PanCard: $("#txtEditPanCard").val(),
            ReportingManagerId: reportingmanagerid,
            FirstName: $("#txtEditUserName").val(),
            EmailId: $("#txtEditEmailID").val(),
            PhoneNumber: $("#txtEditMobileNumber").val(),
            Gender: $("#DDLGender").val(),
            Designation: $("#DDLEditDesignation option:selected").text(),
            DesignationID: $("#DDLEditDesignation").val(),
            DateOfBirth: $("#txtDateOfBirth").val(),
            DateOfJoining: $("#txtDateOfjoining").val(),
            UserType: $("#ddlemployeetype").val(),
            IsOnRollDoctor: $("#IsOnRollDoctor").prop("checked"),
            IsOffRollDoctor: $("#IsOffRollDoctor").prop("checked"),
            Qualification: $("#txtQualification").val(),
            CollageName: $("#txtEditCollegeName").val(),
        };
        $.post("/HrAdmin/EditUserDataByUserID_HRAdmin", { "Data": JSON.stringify(Dataparam) }, function (Result) {
            $("#EditEmployeeDataModal").modal('hide');

            if ($("#EditPhoto").val() != "") {
                EditUploadDocument();
            }
            else {
                OpenSuccessAlertPopUpBox_ConfirmPopUpJS(Result);
                oTable.api().ajax.reload(null, false);
            }
            // GetAllUserListToAdmin_PageLevel();

            //DateOfBirth: $("#txtDateOfBirth").val('');
            //DateOfJoining: $("#txtDateOfjoining").val();

            //GetAllUserEmailListToAdmin_GlobalFunctionJS();
        });
    }
}
function EditUploadDocument() {
    var files = "";
    // Create FormData object
    var fileData = new FormData();
    var fileUpload = $("#EditPhoto").get(0);
    files = fileUpload.files;
    for (var i = 0; i < files.length; i++) {
        fileData.append('ImageFile', files[i]);
    }

    $.ajax({
        type: "POST",
        url: "/HrAdmin/UploadImage?id=" + EdiUserID_PageLevel,
        contentType: false,
        processData: false,
        data: fileData,
        success: function (data) {
            OpenSuccessAlertPopUpBox_ConfirmPopUpJS(data);
            oTable.api().ajax.reload(null, false);
        }
    });
}
function ValidateEmailIDForEdit_PageLevel(ElementID) {
    var EnteredEmailID = $("#" + ElementID).val();
    //if (ValidateEmailFormat_GlobalValidationJS(EnteredEmailID)) {
    //var EmailExistStatus = CheckUserEmailIDExistsOrNotForEdit_GlobalFunJS(EnteredEmailID, EdiUserID_PageLevel);
    //if (!EmailExistStatus) {
    //    alert("Email ID Already Exist. Please Enter Another Email ID");
    //    $("#" + ElementID).val('');
    //}
    //} else {
    //    alert("Please Enter Valid Email ID");
    //    $("#" + ElementID).val('');
    //}
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
        $("#modalEmployeeUpdateRemarksnew").modal('hide');
        oTable.api().ajax.reload(null, false);
        LoadData_Inactive();
        LoadData_Hold();
        alert("success");
    });
}


function ddlLocationForRepaortingChange() {
    if ($("#ddlLocationForRepaorting").val() != null && $("#ddlLocationForRepaorting").val() != " ") {
        $.ajax({
            type: "POST",
            url: "/Common/GetDepartmentByLocation",
            data: "id=" + $("#ddlLocationForRepaorting").val(),
            success: function (data) {
                var options = [];
                options.push('<option value="">- Select Department -</option>');
                for (var i = 0; i < data.length; i++) {
                    options.push('<option value="' + data[i].DepartmentId + '">' + data[i].DepartmentName + '</option>');
                }
                $("#ddlDepartmentForFilterReporting").html(options.join(''));

            },
            error: function () {
                alert("error")
            }
        });
    } else {
        var options = [];
        options.push('<option value="">- Select Department -</option>');
        $("#ddlDepartmentForFilterReporting").html(options.join(''));
    }
}
function LoadEmployeesReporting() {
    if ($("#ddlDepartmentForFilterReporting").val() != null && $("#ddlDepartmentForFilterReporting").val() != " ") {

        $.ajax({
            type: "POST",
            url: "/Common/GetEmployeesByDepartment",
            data: "id=" + $("#ddlDepartmentForFilterReporting").val(),
            success: function (data) {
                var options = [];
                options.push('<option value="">- Select Employee -</option>');
                for (var i = 0; i < data.length; i++) {
                    options.push('<option value="' + data[i].CustomUserId + '">' + data[i].FirstName + " " + data[i].LastName + '</option>');
                }
                $("#ddlEmployeesForFilterreporting").html(options.join(''));
            },
            error: function () {
                alert("error")
            }
        });
    } else {
        var options = [];
        options.push('<option value="">-  Select Employee -</option>');

        $("#ddlEmployeesForFilterreporting").html(options.join(''));
    }

}

function LoadData_Inactive() {
    $('#tbltableinactive').dataTable({

        "bServerSide": true,
        "sAjaxSource": "/Hr/AjaxGetEmployees",
        "bProcessing": true,

        "bDestroy": true,
        "scrollX": true,
        "aLengthMenu": [[10, 20, 30, 50, 100, 150, 200, -1], [10, 20, 30, 50, 100, 150, 200, "All"]],
        //"aaSorting": [[0, "desc"]],
        "aoColumns": [
            { "sName": "CustomUserId" },
            { "sName": "LocationName" },
            { "sName": "FirstName" },
            { "sName": "CustomUserId" },
            { "sName": "EmailId" },
            { "sName": "PlaceAllocation" },
            { "sName": "PhoneNumber" },
            { "sName": "Extenstion" },
            { "sName": "DepartmentName" },
            { "sName": "Designation" },
            { "sName": "RM" },
            {
                "sName": "CustomUserId",
                "bSortable": false,
                "render": function (data) {
                    return '<div class="btn-group">' +
                        '<div class="btn btn-default dropdown-toggle" data-toggle="dropdown"><span class="glyphicon glyphicon-th-list"></span></div>' +
                        '<ul class="dropdown-menu dropdown-menu-right">' +
                        '<li><a href="#" id="' + data + '" onclick="ActiveEmployee_Inactive(this.id);">Active</a></li> ' +
                        '<li><a href="#" id="' + data + '" onclick="DeleteEmployee_Inactive(this.id);">Permanent Delete Employee</a></li>' +

                        '</ul></div>';
                }
            }
        ],

    });
    $("#tbltableinactive_length").addClass("col-md-6");
    $("#tbltableinactive_length").addClass("col-md-6");
}


function DeleteEmployee_Inactive(id) {
    if (confirm("Are you sure you want to delete?")) {
        var task = {
            empid: id
        };
        $.ajax({
            type: "POST",
            url: "/Account/PermanentDeleteEmployee",
            data: task,
            success: function (data) {
                alert(data);
                oTable.api().ajax.reload(null, false);
            },
            error: function () {
                alert("error")
            }
        });
    }
}
function ActiveEmployee_Inactive(id) {
    if (confirm("Are you sure you want to Active?")) {
        var task = {
            empid: id
        };
        $.ajax({
            type: "POST",
            url: "/Account/ActiveEmployee",
            data: task,
            success: function (data) {
                alert(data);
                LoadData_Inactive();
            },
            error: function () {
                alert("error")
            }
        });
    }
}


function LoadData_Hold() {
    $('#tbltablehold').dataTable({

        "bServerSide": true,
        "sAjaxSource": "/Hradmin/AjaxGetEmployeesHold",
        "bProcessing": true,

        "bDestroy": true,
        "scrollX": true,
        "aLengthMenu": [[10, 20, 30, 50, 100, 150, 200, -1], [10, 20, 30, 50, 100, 150, 200, "All"]],
        //"aaSorting": [[0, "desc"]],
        "aoColumns": [
            { "sName": "CustomUserId" },
            { "sName": "LocationName" },
            { "sName": "FirstName" },
            { "sName": "CustomUserId" },
            { "sName": "EmailId" },

            { "sName": "PhoneNumber" },

            { "sName": "DepartmentName" },
            { "sName": "Designation" },
            { "sName": "RM" }
        ],

    });
    $("#tbltablehold_length").addClass("col-md-6");
    $("#tbltablehold_length").addClass("col-md-6");
}