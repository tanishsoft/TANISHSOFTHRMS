var empid;

$(document).ready(function () {
    $(".date")
        .datepicker({
            changeMonth: true,
            changeYear: true,
            dateFormat: "dd-mm-yy",
            Edit: true
        })
    GetAsset();
    LoadData();
    LoadLocations1();
    $('#btnExport').click(function () {
        validateDate();

        if (validate) {
            FromDate = $("#FromDateExcel").val();
            ToDate = $("#ToDateExcel").val();
            window.location = '/Asset/ExportExcelUserAsset?FromDate=' + FromDate + '&ToDate=' + ToDate;
            $("#FromDateExcel").val("");
            $("#ToDateExcel").val("");
        }
    });
});
function validateDate() {
    debugger;
    validate = true;
    $(".css-error").removeClass('css-error');
    if ($("#FromDateExcel").val() == "") {
        $("#FromDateExcel").addClass('css-error');
        validate = false;
    }
    if ($("#ToDateExcel").val() == "") {
        $("#ToDateExcel").addClass('css-error');
        validate = false;
    }
}
function GetAsset() {
    $.get('/Asset/GetAssets', function (data) {
        var options = [];
        options.push('<option value="0">- Select Asset -</option>');
        for (var i = 0; i < data.length; i++) {
            options.push('<option value="' + data[i].Asset_id + '">' + data[i].Asset_Name + '</option>');
        }
        $("#ParentAssetName").html(options.join(''));
       $("#ddlAsset").html(options.join(''));

        $("#EditParentAssetName").html(options.join(''));
    });
}
function LoadLocations1() {
    $.ajax({
        type: "POST",
        url: "/Common/GetLocations",
        //data: "id=",
        success: function (data) {
            var options = [];
            options.push('<option value="">- Select Location -</option>');
            for (var i = 0; i < data.length; i++) {
                options.push('<option value="' + data[i].LocationId + '">' + data[i].LocationName + '</option>');
            }
            $("#ddlLocation").html(options.join(''));
            $("#EditddlLocation").html(options.join(''));

            $("#ddllocation1").html(options.join(''));
        },
        error: function () {
            alert("error")
        }
    });
}
function LoadDepartment1() {
    $("#LocationId").val($("#ddlLocation").val());
    var skillsSelect = document.getElementById("ddlLocation");
    var selectedText = skillsSelect.options[skillsSelect.selectedIndex].text;
    $("#LocationName").val(selectedText);
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
}
function LoadDepartment123() {

    $.ajax({
        type: "GET",
        url: "/Common/GetDepartmentByLocation",
        data: "id=" + $("#ddllocation1").val(),
        success: function (data) {
            var options = [];
            options.push('<option value="">- Select Department -</option>');
            for (var i = 0; i < data.length; i++) {
                options.push('<option value="' + data[i].DepartmentId + '">' + data[i].DepartmentName + '</option>');
            }
            $("#ddldepartment12").html(options.join(''));
        },
        error: function () {
            alert("error")
        }
    });
}


function LoadDepartmentEmployees() {

    $.ajax({
        type: "POST",
        url: "/Common/GetEmployeesByDepartment",
        data: "id=" + $("#ddldepartment12").val(),
        success: function (data) {
            var options = [];
            options.push('<option value="">- Select User -</option>');
            for (var i = 0; i < data.length; i++) {
                options.push('<option value="' + data[i].CustomUserId + '">' + data[i].FirstName + " " + data[i].LastName + '</option>');
            }
            $("#ddlEmployee123").html(options.join(''));
        },
        error: function () {
            alert("error")
        }
    });
}
function LoadEmployees() {
    $("#DepartmentId").val($("#ddlDepartment").val());
    var skillsSelect = document.getElementById("ddlDepartment");
    var selectedText = skillsSelect.options[skillsSelect.selectedIndex].text;
    $("#DepartmentName").val(selectedText);
    $.ajax({
        type: "POST",
        url: "/Common/GetEmployeesByDepartment",
        data: "id=" + $("#ddlDepartment").val(),
        success: function (data) {
            var options = [];
            options.push('<option value="">- Select User -</option>');
            for (var i = 0; i < data.length; i++) {
                options.push('<option value="' + data[i].CustomUserId + '">' + data[i].FirstName + " " + data[i].LastName + '</option>');
            }
            $("#UserName").html(options.join(''));
        },
        error: function () {
            alert("error")
        }
    });
}
function LoadUserData(id) {
    empid = id;
    $('#tblmyUserAssetMaster').dataTable({
        "bServerSide": true,
        "sAjaxSource": "/Asset/AjaxGetmyUserAssetDetails?id=" + id,
        "bProcessing": true,
        "bDestroy": true,
        "aLengthMenu": [[10, 20, 30, 50, 100, 150, 200, -1], [10, 20, 30, 50, 100, 150, 200, "All"]],
        "aoColumns": [
            //{ "sName": "LocationName" },
            //{ "sName": "DepartmentName" },
            //{ "sName": "CustomUserId" },
            //{ "sName": "User_id" },
            { "sName": "Parent_AssetId" },
            { "sName": "Asset_id" },
            { "sName": "Date_Assign" },
            { "sName": "UserAsset_Remarks" },
            { "sName": "UserAsset_IsActive" },
            { "sName": "DeallocateDate" },
            { "sName": "DeallocateComments" },
            {
                "sName": "UserAsset_id",
                "bSortable": false,
                "render": function (data) {
                    return '<div class="btn-group">' +
                        '<div class="btn btn-default dropdown-toggle" data-toggle="dropdown"><span class="glyphicon glyphicon-th-list"></span></div>' +
                        '<ul class="dropdown-menu dropdown-menu-right">' +
                        '<li><a href="javascript:void(0);" id="' + data + '" onclick="Edit(this.id);">Edit</a></li>' +
                        '<li><a href="javascript:void(0);" id="' + data + '" onclick="Deallocate(this.id);">Deallocate</a></li>' +
                        '<li><a href="javascript:void(0);" id="' + data + '" onclick="Delete(this.id);">Delete</a></li>' +
                        '</ul></div>';
                }
            }
        ]
        ,
        "fnServerParams": function (aoData) {
            aoData.push(
                { "name": "locationid", "value": $("#ddllocation1").val() },
                { "name": "departmentid", "value": $("#ddldepartment12").val() },
                { "name": "LeaveTypeid", "value": $("#ddlAsset").val() },
                { "name": "fromdate", "value": $("#FromDateExcel").val() },
                { "name": "todate", "value": $("#ToDateExcel").val() },
                { "name": "Emp", "value": $("#ddlEmployee123").val() }
            );
        }
    });
    $("#tblUserAssetMaster_length").addClass("col-md-6");
    $("#tblUserAssetMaster_length").addClass("col-md-6");
}
function OnclickSaveUserAsset() {
    validateAsset();
    if (validate) {
        $(".css-error").removeClass('css-error');
        debugger;
        var task = {
            Parent_AssetId: $("#ParentAssetName").val(),
            User_id: empid,
            Asset_id: $("#AssetName").val(),
            UserAsset_Remarks: $("#Remarks").val(),
            Date_Assign: $("#Date").val()
        };
        $.ajax({
            type: "POST",
            url: "/Asset/SaveNewUserAsset",
            data: task,
            success: function (data) {
                $("#myModalNewUserAsset").modal('hide');
                //alert(data);
                clearAll(); LoadUserData(empid);
                console.log("category added successfully")
            }
        });
    }
    else {
        alert("Fields Required");
    }
}
function validateAsset() {
    validate = true;
    $(".css-error").removeClass('css-error');
    if ($("#UserName").val() == 0) {
        $("#UserName").addClass('css-error');
        validate = false;
    }
    if ($("#ParentAssetName").val() == 0) {
        $("#ParentAssetName").addClass('css-error');
        validate = false;
    }
    if ($("#Remarks").val() == "") {
        $("#Remarks").addClass('css-error');
        validate = false;
    }
    if ($("#Date").val() == "") {
        $("#Date").addClass('css-error');
        validate = false;
    }
}
function Edit(UserAssetId) {
    debugger;
    var Url = "/Asset/GetUserAssetDeatilsById?UserAssetId=" + UserAssetId;
    $.ajax({
        type: "GET",
        url: Url,
        success: function (data) {
            $("#myModalEdit").modal();
            var obj = data;
            $("#EditID").val(obj.UserAsset_id);
            $("#EditUserName").val(obj.User_id);
            $("#EditParentAssetName").val(obj.Parent_AssetId);
            debugger;
            BindSubAsset('EditParentAssetName', 'Edit');
            $("#EditAssetName").val(obj.Asset_id);
            $("#EditRemarks").val(obj.UserAsset_Remarks);
            $("#EditDate").val(obj.Date_Assign);
        }
    })

}
function Deallocate(UserAssetId) {
    debugger;

            $("#myModalDeallocate").modal();
    $("#DeEditID").val(UserAssetId);
            
}
function DeallocateAsset() {
    if ($("#DeallocateDate").val() != "" && $("#DeallocateComments").val() != "") {
        var task = {
         
            UserAsset_id: $("#DeEditID").val(),
            DeallocateDate : $("#DeallocateDate").val(),
            DeallocateComments: $("#DeallocateComments").val(),
        };
        $.ajax({
            type: "POST",
            url: "/Asset/DeallocateAsset",
            data: task,
            success: function (data) {
                $("#myModalDeallocate").modal('hide');
                LoadUserData(empid);
                console.log("Asset Deallocated successfully")
            }
        });
    }
    else {
        alert("please Enter fileds For Deallocate");
    }
}
function OnclickUpdateUserAsset() {
    validateEdit();
    if (validate) {
        var task = {
            Parent_AssetId: $("#EditParentAssetName").val(),
            UserAsset_id: $("#EditID").val(),
            User_id: empid,
            Asset_id: $("#EditAssetName").val(),
            UserAsset_Remarks: $("#EditRemarks").val(),
            Date_Assign: $("#EditDate").val(),
        };
        $.ajax({
            type: "POST",
            url: "/Asset/UpdateUserAsset",
            data: task,
            success: function (data) {
                $("#myModalEdit").modal('hide');
                LoadUserData(empid);
                console.log("Asset Updated successfully")
            }
        });

    }
    else {
        alert("Fields Required");
    }

}
function validateEdit() {
    validate = true;
    $(".css-error").removeClass('css-error');
    if ($("#EditUserName").val() == 0) {
        $("#EditUserName").addClass('css-error');
        validate = false;
    }
    if ($("#EditParentAssetName").val() == 0) {
        $("#EditParentAssetName").addClass('css-error');
        validate = false;
    }
    if ($("#EditRemarks").val() == "") {
        $("#EditRemarks").addClass('css-error');
        validate = false;
    }
    if ($("#EditDate").val() == "") {
        $("#EditDate").addClass('css-error');
        validate = false;
    }
}
function clearAll() {
    debugger;
    $(".css-error").removeClass('css-error');
    $("#UserName").val("0");
    $("#ParentAssetName").val("0");
    $("#divAssetName").hide();
    $("#divEditAssetName").hide();
    $("#Remarks").val("");
    $("#Date").val("");
    $("#ParentAssetName").val("0");
}
var Delete = function (Id) {
    debugger;

    $("#Cid").val(Id);
    $("#DeleteConfirmation").modal("show");
}
var ConfirmDelete = function () {
    debugger;
    var CId = $("#Cid").val();
    $.ajax({
        type: "POST",
        url: "/Asset/DeleteUserAsset?UserAssetId=" + CId,
        success: function (result) {
            $("#DeleteConfirmation").modal("hide");
        }
    })
    LoadUserData(empid);
}