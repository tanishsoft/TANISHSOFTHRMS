
var currnetid = 0;
var validate = true;
//var employeeid;
var oTable;

$(document).ready(function () {
    LoadLocations();
    //Loademployeesbydepartment();
    Loademployeesbytoassign();
    $(".accountsandfiance").hide();
    $("#CategoryOfComplaint").change(function () {
        if ($(this).val() == "Others") {
            $("#divothers").show();
        } else {
            $("#divothers").hide();
        }
    });
    var dateFormat = "dd/mm/yy",
        from = $("#txtfromdate")
            .datepicker({
                changeMonth: true,
                changeYear: true,
                dateFormat: "dd/mm/yy"
            })
            .on("change", function () {
                to.datepicker("option", "minDate", getDate(this));
            }),
        to = $("#txttodate").datepicker({
            changeMonth: true,
            changeYear: true,
            dateFormat: "dd/mm/yy"
        })
            .on("change", function () {
                from.datepicker("option", "maxDate", getDate(this));
            });


    function getDate(element) {
        var date;
        try {
            date = $.datepicker.parseDate(dateFormat, element.value);
        } catch (error) {
            date = null;
        }

        return date;
    }

    //setTimeout(datatablebind(), 60000);

    //setInterval(TaskCompleted(this.id), 5000);

    $(".clsdatepicker").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "mm/dd/yy"
    });

    $(".clsdatepicker1").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy"
    });
    $(".brandcomunication").hide();
});
var deptcategories = [];
function DepartmentChange_New() {
    $(".Capexifpurchase").hide();
    if ($("#ddlDepartment").val() == "16" || $("#ddlDepartment").val() == "44" || $("#ddlDepartment").val() == "95") {
        $("#assertidifbiomedical").show();
        $("#assertnameifbiomedical").show();
    }
    else {
        $("#assertidifbiomedical").hide();
        $("#assertnameifbiomedical").hide();
    }

    if ($("#ddlDepartment").val() == "20" || $("#ddlDepartment").val() == "146" || $("#ddlDepartment").val() == "190" | $("#ddlDepartment").val() == "264" | $("#ddlDepartment").val() == "295") {
        $("#lbldocumentname").html("All user collection Report");
        $(".accountsandfiance").show();
    }
    else {
        $("#lbldocumentname").html("Document");
        $(".accountsandfiance").hide();
    }

    $("#lblCategoryOfComplaint").html("Category Of Complaint /Service <span style='color:red;'>*</span>");

    if ($("#ddlDepartment option:selected").text().toLowerCase() == "academics") {
        $("#lblCategoryOfComplaint").html("New Request <span style='color:red;'>*</span>");
    }
    if ($("#ddlDepartment option:selected").text().toLowerCase() == "purchase") {
        $("#lblCategoryOfComplaint").html("Invoice/Bill <span style='color:red;'>*</span>");
        $(".Capexifpurchase").show();
    }
    if ($("#ddlDepartment option:selected").text().includes("Brand")) {
        $("#lblCategoryOfComplaint").html("Select <span style='color:red;'>*</span>");
        $("#lbldocumentname").html("Document <span style='color:red;'>*</span>");
        $(".brandcomunication").show();
    }
    $.get("/Common/GetCategoryByDepartmentId", { id: $("#ddlDepartment").val() }, function (data) {
        deptcategories = data;
        var options = [];
        if ($("#ddlDepartment option:selected").text().toLowerCase() == "purchase") {
            options.push('<option value="">- Select Invoice/Bill -</option>');
        } else {
            options.push('<option value="">- Select Category -</option>');
        }
        for (var i = 0; i < data.length; i++) {
            options.push('<option value="' + data[i].Name + '">' + data[i].Name + '</option>');
        }
        $("#CategoryOfComplaint").html(options.join(''));
    });
}
function typeofrequestchange() {
    var options = [];
    if ($("#ddlDepartment option:selected").text().toLowerCase() == "purchase") {
        options.push('<option value="">- Select Invoice/Bill -</option>');
    } else {
        options.push('<option value="">- Select Category -</option>');
    }
    for (var i = 0; i < deptcategories.length; i++) {
        if ($("#TaskType").val() == deptcategories[i].Description) {
            options.push('<option value="' + deptcategories[i].Name + '">' + deptcategories[i].Name + '</option>');
        }
    }
    $("#CategoryOfComplaint").html(options.join(''));
}


function Applyclassdt() {
    $("#myDataTable_length").addClass("col-md-6");
    $("#myDataTable_info").addClass("col-md-6");

}
function datatablebind() {
    oTable = $('#myDataTable').dataTable({
        "bServerSide": true,
        "sAjaxSource": "/Home/AjaxUserviewnew",
        "bProcessing": true,
        "bDestroy": true,
        "aoColumns": [
            { "sName": "TaskId" },
            { "sName": "Call_Date" },
            { "sName": "ResTime" },
            //{ "sName": "CreatorLocationName" },
            { "sName": "Creator" },
            { "sName": "Floor" },
            { "sName": "ExtensionNo" },
            { "sName": "CategoryOfComplaint" },
            //{ "sName": "AssertEquipName" },
            { "sName": "Subject" },
            { "sName": "Assign" },
            //{ "sName": "Work_done" },
            { "sName": "Total_Time_Taken" },
            { "sName": "Status" },
            { "sName": "SloverStatus" },
            {
                "sName": "id",//myModalAssignTasktoepmloyee
                "bSortable": false,
                "render": function (data) {
                    return GetHtmlContent(data);
                }
            }

        ],
        "aLengthMenu": [[10, 20, 30, 50, 100, 150, 200, 500, 1000], [10, 20, 30, 50, 100, 150, 200, 500, 1000]],
        //"aaSorting": [[0, "asc"]],


        "fnServerParams": function (aoData) {
            aoData.push(
                { "name": "locationid", "value": $("#ddlLocationForFilter").val() },
                { "name": "departmentid", "value": $("#ddlDepartmentForFilter").val() },
                { "name": "status", "value": $("#ddlStatusTypeForFilter").val() },
                { "name": "Emp", "value": $("#ddlEmployeesForFilter").val() },
                { "name": "fromdate", "value": $("#txtfromdate").val() },
                { "name": "todate", "value": $("#txttodate").val() },
                { "name": "category", "value": $("#ddlCategoryTypeForFilter").val() },
                { "name": "FormType", "value": $("#ddlPriority").val() },
                { "name": "Expired", "value": $("#ddlDocumentreceived").val() }


            );
        },

        "sDom": 'T<"clear">lfrtip',
        "oTableTools": {
            "sSwfPath": "/swf/copy_csv_xls_pdf.swf",
            "aButtons": [
                "copy", "xls", "pdf"
            ]
        },

        //initComplete: function () {
        //    $("#myDataTable_length").append(' <span class="label label-default" style="font-size:medium">R Time: Response Time</span>&nbsp;&nbsp;            <span class="label label-primary" style="font-size:medium">Ext: Extension Number</span>&nbsp;&nbsp;            <span class="label label-success" style="font-size:medium">T.T.T: Total Time Taken</span>&nbsp;&nbsp;');
        //},
        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            $('td:eq(7)', nRow).html('<a href="javascript:void(0);" id="' + aData[12] + '" onclick="ViewDescription(this.id);">' + aData[7] + '</a>');
            //<a href="javascript:void(0);" id="' + data + '" onclick="StartTask(this.id);">Pick Up</a>
            switch (aData[11]) {
                case 'Pending':
                    //$(nRow).css('class', 'myClassred');
                    $('td:eq(11)', nRow).css('color', '#E67D21').css('font-weight', 'bold');
                    break;
                case 'New':
                    //$(nRow).css('background-color', 'green')
                    $('td:eq(11)', nRow).css('color', '#3598dc').css('font-weight', 'bold');
                    break;
                case 'In Progress':
                    //$(nRow).css('background-color', 'blue')
                    $('td:eq(11)', nRow).css('color', '#8E44AD').css('font-weight', 'bold');
                    break;
                case 'Pending from user':
                    $('td:eq(11)', nRow).html("Pending at user");
                    $('td:eq(11)', nRow).css('color', '#E67D21').css('font-weight', 'bold');
                    break;
                case 'Reopen':
                    $('td:eq(11)', nRow).css('color', '#5FC29D').css('font-weight', 'bold');
                    break;
                case 'Rejected':
                    $('td:eq(11)', nRow).css('color', '#a71f23').css('font-weight', 'bold');
                    break;
                default:
                    $('td:eq(11)', nRow).css('color', 'Green').css('font-weight', 'bold');
                    break;
            }
        }
    });

    //setInterval(function () {
    //    oTable.api().ajax.reload(null, false);
    //}, 30000);
    //Applyclassdt();
}
function ViewDescription(id) {
    $.ajax({
        type: "GET",
        url: "/Home/ViewDescription",
        data: "id=" + id,
        success: function (data) {

            $("#divTaskDetailsdesc").html(data);
            $("#myModalTaskDetailsdescription").modal('show');
        }
    });
}

function WorkDonwstatus(id) {
    $.ajax({
        type: "GET",
        url: "/Home/WorkDonwstatus",
        data: "id=" + id,
        success: function (data) {

            $("#divTaskwork").html(data);
            $("#mymodelworkdonedetailwork").modal('show');
        }
    });

}



function GetHtmlContent(data) {
    var deptvalue = $("#currentuserDepartmentId").val();
    var htmlcontect = "";
    htmlcontect += '<div class="btn-group">';
    htmlcontect += '<div class="btn btn-default dropdown-toggle" data-toggle="dropdown"><span class="glyphicon glyphicon-th-list"></span></div>';
    htmlcontect += '<ul class="dropdown-menu dropdown-menu-right">';
    htmlcontect += '<li class="dontScoll"><a href="javascript:void(0);" id="' + data + '" onclick="ViewTaskDetails(this.id);">View Task</a></li>';
    var departmentslist = "12,13,16,20,26,41,44,50,68,76,85,95,102,111,146,190,302,272,264,279,178,273,117,360";
    if ($("#LoginEmployeeType").val() == "OutSource") {
        htmlcontect += '<li><a href="javascript:void(0);" id="' + data + '" onclick="StartTask(this.id);">Pick Up</a></li>';
        htmlcontect += '<li><a href="javascript:void(0);" id="' + data + '" onclick="EditDescription(this.id);">Edit Description</a></li>';
        htmlcontect += '<li><a href="javascript:void(0);" id="' + data + '" onclick="myWorkDonecomment(this.id);">Edit Comments</a></li>';
        htmlcontect += '<li><a href="javascript:void(0);" id="' + data + '" onclick="myWorkDone(this.id);">Completed</a></li> ';
        htmlcontect += '<li><a href="javascript:void(0);" id="' + data + '" onclick="DoneTaks(this.id);"> Close</a></li> ';
    } else {
        if (departmentslist.includes(deptvalue)) {
            htmlcontect += '<li><a href="javascript:void(0);" id="' + data + '" onclick="StartTask(this.id);">Pick Up</a></li>';
            htmlcontect += '<li><a href="javascript:void(0);" id="' + data + '" onclick="EditDescription(this.id);">Edit Description</a></li>';
            htmlcontect += '<li><a href="javascript:void(0);" id="' + data + '" onclick="myWorkDonecomment(this.id);">Edit Comments</a></li>';
            htmlcontect += '<li><a href="javascript:void(0);" id="' + data + '" onclick="myWorkDone(this.id);">Completed</a></li> ';
            htmlcontect += '<li><a href="javascript:void(0);" id="' + data + '" onclick="DoneTaks(this.id);"> Close</a></li> ';
            htmlcontect += '<li><a href="javascript:void(0);" id="' + data + '" onclick="ReopenTask(this.id);">Re Issue</a></li> ';
            htmlcontect += '<li><a href="javascript:void(0);" id="' + data + '" onclick="AssignTask(this.id);"> Assign to other department</a></li>';
            htmlcontect += '<li><a href="javascript:void(0);" id="' + data + '" onclick="AssignTasktoemployees(this.id);">Assign To Employee</a></li>';
            htmlcontect += '<li><a href="javascript:void(0);" id="' + data + '" onclick="AssignTasktoVendor(this.id);">Assign To Vendor</a></li>';

        }

        else {
            // htmlcontect += '<li><a href="/Home/EditTask/' + data + '" id="' + data + '">Edit Task</a></li>';
            // htmlcontect += '<li><a href="/Home/Update/' + data + '" id="' + data + '">Update Task</a></li>';
            htmlcontect += '<li><a href="javascript:void(0);" id="' + data + '" onclick="EditDescription(this.id);">Edit Description</a></li>';
            htmlcontect += '<li><a href="javascript:void(0);" id="' + data + '" onclick="DoneTaks(this.id);"> Close</a></li> ';
            htmlcontect += '<li><a href="javascript:void(0);" id="' + data + '" onclick="ReopenTask(this.id);">Re Issue</a></li> ';
        }

    }

    //htmlcontect += '<li><a href="javascript:void(0);" id="' + data + '" onclick="ApproveCPX(this.id);">Approve CPX</a></li>';
    //htmlcontect += '<li><a href="javascript:void(0);" id="' + data + '" onclick="RejectCPX(this.id);">Reject CPX</a></li>';
    htmlcontect += '<li><a href="javascript:void(0);" id="' + data + '" onclick="ViewComments(this.id);">View Comments</a></li> ';
    htmlcontect += '<li><a href="javascript:void(0);" id="' + data + '" onclick="WorkDonwstatus(this.id);">WorkDone Comments</a></li> ';
    htmlcontect += '<li><a href="javascript:void(0);" id="' + data + '" onclick="ViewFiles(this.id);">View Files</a></li>';
    //htmlcontect += '<li><a href="javascript:void(0);" id="' + data + '" onclick="DeleteTask(this.id);">Delete Task</a></li> ';

    htmlcontect += '</ul></div>';
    return htmlcontect;
}
function ViewTaskDetails(id) {
    //$(function () {
    //    $('.dontScoll').click(function () {
    //        $(window).scroll(function () { return false; });
    //    });
    //});
    //   divTaskDetails
    $.ajax({
        type: "GET",
        url: "/Home/GetTask",
        data: "id=" + id,
        success: function (data) {
            var html = "<table class='table table-bordered'>";
            html += "<tr>";
            //html += "<td>Call Date</td>";
            //html += "<td>" + functionDate(data.CallDateTime) + "</td>";
            html += "<td><b>Call Start Date</b></td>";
            html += "<td>" + functionDate(data.CallStartDateTime) + "</td>";
            html += "</tr>";
            html += "<tr>";
            html += "<td><b>From Location</b></td>";
            html += "<td>" + data.CreatorLocationName + "</td>";
            html += "</tr>";
            html += "<tr>";
            html += "<td><b>From Department</b></td>";
            html += "<td>" + data.CreatorDepartmentName + "</td>";
            html += "</tr>";
            html += "<tr>";
            html += "<td><b>Name</b></td>";
            html += "<td>" + data.CreatorName + "</td>";
            //html += "<td>Creator Place</td>";
            //if (data.CreatorPlace != null) {
            //    html += "<td>" + data.CreatorPlace + "</td>";
            //} else {
            //    html += "<td> </td>";
            //}
            html += "</tr>";
            if (data.AssertEquipId != null && data.AssertEquipName != null) {
                html += "<tr>";
                html += "<td><b>Assert EquipId</b></td>";

                html += "<td>" + data.AssertEquipId + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td><b>Assert EquipName</b></td>";
                html += "<td>" + data.AssertEquipName + "</td>";
                html += "</tr>";
            }
            html += "<tr>";
            html += "<td><b>Category Of Complaint</b></td>";
            if (data.CategoryOfComplaint != null) {
                html += "<td>" + data.CategoryOfComplaint + "</td>";
            } else {
                html += "<td></td>";
            }
            html += "</tr>";
            html += "<tr>";
            html += "<td><b>Description</b></td>";
            html += "<td>" + data.Description + "</td>";
            html += "</tr>";
            html += "<tr>";
            html += "<td><b>Assign To Dept</b></td>";
            html += "<td>" + data.AssignLocationName + " - " + data.AssignDepartmentName + "</td>";
            html += "</tr>";
            if (data.AssignName != null && data.AssignName != "") {
                html += "<tr>";
                html += "<td><b>Assign To</b></td>";
                html += "<td>" + data.AssignName + "</td>";
                html += "</tr>";
            }
            if (data.CapexPrepareDate != null && data.CapexPrepareDate != "") {
                html += "<tr>";
                html += "<td><b>Bill Date</b></td>";
                html += "<td>" + functionDate(data.CapexPrepareDate) + "</td>";
                html += "</tr>";
                html += "<tr><td>Change Document Status</td><td><select class='form-control' onchange='changedocumentreceived(this.id)' id='selectdocumentreceived" + id + "'><option>Select Document Received</option><option value='true'>Yes</option><option value='false'>No</option></select></td>";

                html += "</tr>";

                html += "<tr>";
                html += "<td><b>Document Received</b></td>";
                if (data.DocumentReceived) {
                    html += "<td id='lbldocumentreceived'>Yes</td>";
                } else {
                    html += "<td id='lbldocumentreceived'>No</td>";
                }
                html += "</tr>";

            }
            if (data.CapexApproveDate != null && data.CapexApproveDate != "") {
                html += "<tr>";
                html += "<td><b>Submission Date</b></td>";
                html += "<td>" + functionDate(data.CapexApproveDate) + "</td>";
                html += "</tr>";
            }
            if (data.Priority != null && data.Priority != "") {
                html += "<tr>";
                html += "<td><b>Priority</b></td>";
                html += "<td>" + data.Priority + "</td>";
                html += "</tr>";
            }
            html += "</table>";
            $("#divTaskDetails").html(html);
            $("#myModalTaskDetails").modal('show');
        }
    });
}
function changedocumentreceived(id) {
    var status = $("#" + id).val();
    var ticketid = id.replace("selectdocumentreceived", "");
    $.get("/Home/UpdateDocumentStatusTicket?ticketid=" + ticketid + "&status=" + status, function (data) {
        alert(data);
        $("#lbldocumentreceived").html(status);
        oTable.api().ajax.reload(null, false);
        //$("#myModalTaskDetails").modal('hide');
        //ViewTaskDetails(ticketid);
    });
}
function AssignTasktoVendor(id) {
    currnetid = id;
    $("#myModalAssignToVendor").modal("show");
}


function SaveassigntoVendorRemarks() {
    if ($("#assigntoVendorRemarks").val() != null) {
        var tdata = {
            id: currnetid,
            remarks: $("#assigntoVendorRemarks").val()
        };
        $.ajax({
            type: "POST",
            url: "/Home/AssigntoVendor",
            data: tdata,
            success: function (data) {
                $("#assigntoVendorRemarks").val('');
                $("#myModalAssignToVendor").modal("hide");

                oTable.api().ajax.reload(null, false);

            },
            error: function () {
                alert("error");
            }
        });
    } else {
        $("#assigntoVendorRemarks").css("border", "solid 1px red");
    }
}

function ViewTask(id) {
    //   divTaskDetails
    $.ajax({
        type: "GET",
        url: "/Home/GetTask",
        data: "id=" + id,
        success: function (data) {
            var html = "<table class='table table-bordered'>";
            html += "<tr>";
            html += "<td>Call Date</td>"; html += "<td>" + functionDate(data.CallDateTime) + "</td>";
            html += "<td>Call Start Date</td>"; html += "<td>" + functionDate(data.CallStartDateTime) + "</td>";
            html += "</tr>";
            html += "<tr>";
            html += "<td>Location</td>"; html += "<td>" + data.CreatorLocationName + "</td>";
            html += "<td>Department</td>"; html += "<td>" + data.CreatorDepartmentName + "</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td>Name</td>"; html += "<td>" + data.CreatorName + "</td>";
            html += "<td>Creator Place</td>";
            if (data.CreatorPlace != null) {
                html += "<td>" + data.CreatorPlace + "</td>";
            } else {
                html += "<td> </td>";
            }
            html += "</tr>";
            if (data.AssertEquipId != null && data.AssertEquipName != null) {
                html += "<tr>";
                html += "<td>Assert EquipId</td>";

                html += "<td>" + data.AssertEquipId + "</td>";
                html += "<td>Assert EquipName</td>";
                html += "<td>" + data.AssertEquipName + "</td>";
                html += "</tr>";
            }
            html += "<tr>";
            html += "<td>Category Of Complaint</td>";
            if (data.CategoryOfComplaint != null) {
                html += "<td>" + data.CategoryOfComplaint + "</td>";
            } else {
                html += "<td></td>";
            }
            html += "<td>Description</td>";
            html += "<td>" + data.Description + "</td>";
            html += "</tr>";

            html += "</table>";
            $("#divTaskDetails").html(html);
            $("#myModalTaskDetails").modal('show');
        }
    });
}






function ViewComments(id) {
    currnetid = id;
    $.ajax({
        type: "GET",
        url: "/Home/GetMyTaskComments",
        data: "taskid=" + id,
        success: function (data) {
            var html = "";
            var count = 1;
            for (var i = 0; i < data.length; i++) {
                html += "<tr>";
                html += "<td>" + count + "</td>";
                html += "<td>" + data[i].Comment + "</td>";
                html += "<td>" + data[i].CommentedBy + "</td>";
                html += "<td>" + functionDate(data[i].CommentDate) + "</td>";
                html += "</tr>";
                count = count + 1;
            }
            $("#tblbodycomments").html(html);
            $("#myModalComments").modal('show');
            currnetid = id;
        },
        error: function () {
            alert("error")
        }
    });
}
function SaveNewComment() {
    var data = {
        taskid: currnetid,
        comment: $("#workcomments").val()
    }
    $.ajax({
        type: "GET",
        url: "/Home/SaveMyTaskComment",
        data: data,
        success: function (data) {
            $("#workcomments").val('');
            ViewComments(currnetid);
        },
        error: function () {
            alert("error")
        }
    });
}
var currentid;
function ViewFiles(id) {
    currentid = id;
    //alert(id);
    BindListoffiles(id);
    $("#divManageDocuments").modal("show");
}

function Uploaddataclick() {

    // Checking whether FormData is available in browser
    if (window.FormData !== undefined) {

        var fileUpload = $("#selectfiletoupload").get(0);
        var files = fileUpload.files;

        // Create FormData object
        var fileData = new FormData();

        // Looping over all files and add it to FormData object
        for (var i = 0; i < files.length; i++) {
            fileData.append(files[i].name, files[i]);
        }

        // Adding one more key to FormData object
        fileData.append('id', currentid);
        fileData.append('public', $("#documentisprivate").val());
        $.ajax({
            url: '/Home/UploadFiles',
            type: "POST",
            contentType: false, // Not to set any content header
            processData: false, // Not to process data
            data: fileData,
            success: function (result) {
                alert(result);
                BindListoffiles(currentid);
            },
            error: function (err) {
                alert(err.statusText);
            }
        });
    } else {
        alert("FormData is not supported.");
    }
}
function BindListoffiles(id) {
    $.get("/Home/GetListOfFiles", { id: id }, function (data) {
        var html = "<table class='table  table-bordered'><tr style='background-color:teal;color:white;'><td>Name</td><td> Action</td></tr>";
        for (var i = 0; i < data.length; i++) {
            html += "<tr><td>" + data[i].DocumentName + "</td><td><a  target='_blank' href='/ExcelUplodes/" + data[i].DocumentPath + "'>View</a> </td></tr>";
        }
        html + "</table>";
        $("#Divdocumentshistory").html(html);
    });
}

var currentid = 0;

function EditDescription(id) {
    currentid = id;

    $("#ddlselectcategoryedit").html("");
    $("#ddlselectcategoryedit").html('<option value="">Select</option>');

    $.ajax({
        type: "GET",
        url: "/Home/GetTask",
        data: "id=" + id,
        success: function (data1) {
            $.get("/Task/GetCategory", function (data) {
                var options = [];
                for (var i = 0; i < data.length; i++) {
                    options.push('<option value="' + data[i].Name + '">' + data[i].Name + '</option>');
                }
                $("#ddlselectcategoryedit").html(options.join(''));
                $("#ddlselectcategoryedit").val(data1.CategoryOfComplaint);
                $("#myModalEditdescription").modal('show');
                $("#EditDescriptiontxt").text(data1.Description);
            });

        }
    });
    //EditDescriptiontxt
}
function SaveDescription() {
    $("#myModalEditdescription").modal("hide");
    waitingDialog.show();
    var data = {
        id: currentid,
        Description: $("#EditDescriptiontxt").val(),
        Category: $("#ddlselectcategoryedit").val()
    }
    $.ajax({
        type: "GET",
        url: "/Home/UpdateDescription",
        data: data,
        success: function (data) {
            waitingDialog.hide();
            $("#myModalEditdescription").modal('hide');
            oTable.api().ajax.reload(null, false);
            if (data != "Success") {
                alert(data);
            }
        }
    });
}




function LoadLocations() {
    $.ajax({
        type: "POST",
        url: "/Common/GetLocations",
        //data: "id=",
        success: function (data) {
            var options = [];
            options.push('<option value="">- All -</option>');
            for (var i = 0; i < data.length; i++) {
                //if (data[i].LocationId == $("#currentuserlocationid").val()) {
                //    options.push('<option value="' + data[i].LocationId + '" selected>' + data[i].LocationName + '</option>');
                //} else {
                options.push('<option value="' + data[i].LocationId + '">' + data[i].LocationName + '</option>');
                //}
            }
            $("#ddlLocation").html(options.join(''));
            $("#ddlLocationForFilter").html(options.join(''));
            //  LoadDepartment();
            datatablebind();
        },
        error: function () {
            alert("error")
        }
    });
}
function LoadDepartment() {
    if ($("#ddlLocation").val() != null && $("#ddlLocation").val() != "") {
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
                    if (data[i].ShowInHelpDesk) {
                        options.push('<option value="' + data[i].DepartmentId + '">' + data[i].DepartmentName + '</option>');
                    }
                }
                $("#ddlDepartment").html(options.join(''));
            },
            error: function () {
                alert("error")
            }
        });
    }
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
            $("#ddlLocation1").html(options.join(''));
        },
        error: function () {
            alert("error")
        }
    });
}
function LoadDepartment1() {
    $("#LocationId").val($("#ddlLocation1").val());
    var skillsSelect = document.getElementById("ddlLocation1");
    var selectedText = skillsSelect.options[skillsSelect.selectedIndex].text;
    $("#LocationName").val(selectedText);
    $.ajax({
        type: "POST",
        url: "/Common/GetDepartmentByLocation",
        data: "id=" + $("#ddlLocation1").val(),
        success: function (data) {
            var options = [];
            options.push('<option value="">- Select Department -</option>');
            for (var i = 0; i < data.length; i++) {
                if (data[i].DepartmentName == "Information Technology" || data[i].DepartmentName == "Biomedical" || data[i].DepartmentName == "Maintenance" || data[i].DepartmentName == "Academics")
                    options.push('<option value="' + data[i].DepartmentId + '">' + data[i].DepartmentName + '</option>');
            }
            $("#ddlDepartment1").html(options.join(''));
        },
        error: function () {
            alert("error")
        }
    });
}
function LoadEmployees() {
    $("#DepartmentId").val($("#ddlDepartment1").val());
    var skillsSelect = document.getElementById("ddlDepartment1");
    var selectedText = skillsSelect.options[skillsSelect.selectedIndex].text;
    $("#DepartmentName").val(selectedText);
    $.ajax({
        type: "POST",
        url: "/Common/GetEmployeesByDepartment",
        data: "id=" + $("#ddlDepartment1").val(),
        success: function (data) {
            var options = [];
            options.push('<option value="">- Select Employee -</option>');
            for (var i = 0; i < data.length; i++) {
                options.push('<option value="' + data[i].UserId + '">' + data[i].FirstName + " " + data[i].LastName + '</option>');
            }
            $("#ddlEmployees1").html(options.join(''));
        },
        error: function () {
            alert("error")
        }
    });
}
/*employees will come for assign the task*/
function EmployeeChange() {
    $("#UserId").val($("#ddlEmployees1").val());
    $("#UserIdofemp").val($("#ddlassignemployeetothetask").val());
    var skillsSelect = document.getElementById("ddlEmployees1");
    var skillsSelects = document.getElementById("ddlassignemployeetothetask");
    var selectedText = skillsSelect.options[skillsSelect.selectedIndex].text;
    var selectedText1 = skillsSelects.options[skillsSelects.selectedIndex].text;
    $("#UserName").val(selectedText);
    $("#UserNameofemp").val(selectedText1);
}

function EmployeeChangebyassigntotask() {
    $("#UserIdofemp").val($("#ddlassignemployeetothetask").val());
    var skillsSelects = document.getElementById("ddlassignemployeetothetask");
    var selectedText = skillsSelects.options[skillsSelects.selectedIndex].text;
    $("#UserNameofemp").val(selectedText);
}
function DepartmentChange() {
    $("#UserId").val(0);
    $("#UserName").val("");
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
            options.push('<option value="">- Select Employee -</option>');
            for (var i = 0; i < data.length; i++) {
                options.push('<option value="' + data[i].UserId + '">' + data[i].FirstName + " " + data[i].LastName + '</option>');
            }
            $("#ddlEmployeesnew").html(options.join(''));
        },
        error: function () {
            alert("error")
        }
    });
}
function EmployeeChange_New() {
    $("#UserId").val($("#ddlEmployeesnew").val());
    var skillsSelect = document.getElementById("ddlEmployeesnew");
    var selectedText = skillsSelect.options[skillsSelect.selectedIndex].text;
    $("#UserName").val(selectedText);
}
function SaveTaskData() {
    if ($("#ddlLocation").val() != "" && $("#Description").val() != "" && $("#extensionno").val() != "" && $("#Subject").val() != "" && $("#CategoryOfComplaint").val() != "") {
        $("#myModal").modal("hide");
        waitingDialog.show();
        if (window.FormData !== undefined) {

            debugger;
            // Create FormData object
            var fileData = new FormData();
            var fileUpload = $("#NewTaskDocuments").get(0);
            var files = fileUpload.files;
            fileData.append("Upload", files[0]);
            var fileUpload1 = $("#DailyCashcollction").get(0);
            if (fileUpload1 != null && fileUpload1.files != null) {
                var files1 = fileUpload1.files;
                fileData.append("DailyCashcollction", files1[0]);
            }
            var fileUpload2 = $("#loginuserwisecollections").get(0);
            if (fileUpload2 != null && fileUpload2.files != null) {
                var files1 = fileUpload2.files;
                fileData.append("loginuserwisecollections", files1[0]);
            }
            var NASReport = $("#NASReport").get(0);
            if (NASReport != null && NASReport.files != null) {
                var files1 = NASReport.files;
                fileData.append("NASReport", files1[0]);
            }
            var Others1 = $("#OthersDocument").get(0);
            if (Others1 != null && Others1.files != null) {
                var files1 = Others1.files;
                fileData.append("OthersDocument", files1[0]);
            }
            //// Looping over all files and add it to FormData object
            //for (var i = 0; i < files.length; i++) {

            //}

            // Adding one more key to FormData object
            fileData.append('CreatorLocationId', $("#CreatorLocationId").val());
            fileData.append('CreatorLocationName', $("#CreatorLocation").val());
            fileData.append('CreatorDepartmentId', $("#CreatorDepartmentId").val());
            fileData.append('CreatorDepartmentName', $("#CreatorDepartment").val());
            fileData.append('CreatorId', $("#CreatorId").val());
            fileData.append('CreatorName', $("#CreatorName").val());
            fileData.append('CreatorPlace', $("#CreatorPlace").val());
            fileData.append('AssertEquipId', $("#Assertno").val());
            fileData.append('AssertEquipName', $("#EquipName").val());
            fileData.append('Subject', $("#Subject").val());
            fileData.append('Description', $("#Description").val());
            fileData.append('AssignLocationId', $("#ddlLocation").val());
            fileData.append('AssertEquipName', $("#EquipName").val());
            fileData.append('Subject', $("#Subject").val());
            fileData.append('Description', $("#Description").val());
            fileData.append('AssignLocationId', $("#ddlLocation").val());
            fileData.append('AssignLocationName', $("#ddlLocation option:selected").text());
            fileData.append('AssignDepartmentId', $("#ddlDepartment").val());
            fileData.append('AssignDepartmentName', $("#ddlDepartment option:selected").text());
            fileData.append('CreatorStatus', 'New');
            fileData.append('AssignStatus', 'New');
            fileData.append('IsActive', true);
            fileData.append('CategoryOfComplaint', $("#CategoryOfComplaint").val());
            fileData.append('AssignId', $("#UserId").val());
            fileData.append('AssignName', $("#UserName").val());
            fileData.append('ExtensionNo', $("#extensionno").val());
            fileData.append('EmailId', $("#eamilid").val());
            fileData.append('Others', $("#txtothers").val());
            fileData.append('CapexPrepareDate', $("#CapexPrepareDate").val());
            fileData.append('CapexApproveDate', $("#CapexApproveDate").val());
            fileData.append('Priority', $("#Priority").val());
            fileData.append('TaskType', $("#TaskType").val());
            var task = {
                CreatorLocationId: $("#CreatorLocationId").val(),
                CreatorLocationName: $("#CreatorLocation").val(),
                CreatorDepartmentId: $("#CreatorDepartmentId").val(),
                CreatorDepartmentName: $("#CreatorDepartment").val(),
                CreatorId: $("#CreatorId").val(),
                CreatorName: $("#CreatorName").val(),
                CreatorPlace: $("#CreatorPlace").val(),
                AssertEquipId: $("#Assertno").val(),
                AssertEquipName: $("#EquipName").val(),
                Subject: $("#Subject").val(),
                Description: $("#Description").val(),
                AssignLocationId: $("#ddlLocation").val(),
                AssignLocationName: $("#ddlLocation option:selected").text(),
                AssignDepartmentId: $("#ddlDepartment").val(),
                AssignDepartmentName: $("#ddlDepartment option:selected").text(),
                CreatorStatus: "New",
                AssignStatus: "New",
                IsActive: true,
                CategoryOfComplaint: $("#CategoryOfComplaint").val(),
                AssignId: $("#UserId").val(),
                AssignName: $("#UserName").val(),
                ExtensionNo: $("#extensionno").val(),
                EmailId: $("#eamilid").val(),
                Others: $("#txtothers").val(),
                CapexPrepareDate: $("#CapexPrepareDate").val(),
                CapexApproveDate: $("#CapexApproveDate").val(),
                Priority: $("#Priority").val()
            };

            $.ajax({
                cache: true,
                type: "POST",
                url: "/Home/AddNewTask",
                contentType: false,
                processData: false,
                data: fileData,

                success: function (data) {
                    $("#Subject").val('');
                    $("#extensionno").val('');
                    var today = new Date().getHours();
                    if (today <= 7 && today >= 21) {
                        data = data + " Engineers not available. We will get back to you soon.";
                    }
                    OpenSuccessAlertPopUpBox_ConfirmPopUpJS(data);//alert messege added on 16-01-2016
                    waitingDialog.hide();
                    oTable.api().ajax.reload(null, false);
                    clearall();


                },
                error: function () {
                    alert("error")
                }
            });
        } else {
            alert("FormData is not supported.");
        }


    }
    else {
        alert("Please Enter all required fields");
    }
}
function AssignTask(id) {
    currnetid = id;
    $("#myModalAssignTask").modal("show");
    LoadLocations1();
}
function Assigntakstoemployee() {
    var task = {
        id: currnetid,
        locid: $("#ddlLocation1").val(),
        locname: $("#ddlLocation1 option:selected").text(),
        deptid: $("#ddlDepartment1").val(),
        deptname: $("#ddlDepartment1 option:selected").text(),//ddlDepartment1
        //empid: $("#UserId").val(),
        // empname: $("#UserName").val()//ddlEmployees1
    };
    $.ajax({
        type: "POST",
        url: "/Home/Assigntasktoemployee",
        data: task,
        success: function (data) {
            OpenSuccessAlertPopUpBox_ConfirmPopUpJS(data);//alert messege added on 16-01-2016
            $("#myModalAssignTask").modal('hide');
            oTable.api().ajax.reload(null, false);
        },
        error: function () {
            alert("error")
        }
    });
}

function StartTask(id) {
    $.ajax({
        type: "POST",
        url: "/Home/StartTask",
        data: "id=" + id,
        success: function (data) {
            if (data == "Success") {
                oTable.api().ajax.reload(null, false);
            } else {
                alert(data);
            }
        },
        error: function () {
            alert("error")
        }
    });
}
function ReopenTask(id) {
    $.ajax({
        type: "GET",
        url: "/Home/ReOpenTask",
        data: "id=" + id,
        success: function (data) {
            oTable.api().ajax.reload(null, false);
        },
        error: function () {
            alert("error")
        }
    });
}
function EndTask(id) {
    $.ajax({
        type: "POST",
        url: "/Home/EndTask",
        data: "id=" + id,
        success: function (data) {
            OpenSuccessAlertPopUpBox_ConfirmPopUpJS(data);
            oTable.api().ajax.reload(null, false);

        },
        error: function () {
            alert("error")
        }
    });
}
function Reject(id) {
    $.ajax({
        type: "POST",
        url: "/Home/RejectTask",
        data: "id=" + id,
        success: function (data) {
            OpenSuccessAlertPopUpBox_ConfirmPopUpJS(data);
            oTable.api().ajax.reload(null, false);
        },
        error: function () {
            alert("error")
        }
    });
}
function myWorkDone(id) {
    currnetid = id;
    $("#myModalWorkDone").modal("show");
}
function SaveMyworkDone() {
    if ($("#workdonereamrks").val() != null
    ) {
        var tdata = {
            id: currnetid,
            remarks: $("#workdonereamrks").val()
        };
        $.ajax({
            type: "POST",
            url: "/Home/WorkDoneRemarks",
            data: tdata,
            success: function (data) {
                if (data == 'Success') {
                    $("#workdonereamrks").val('');
                    $("#myModalWorkDone").modal("hide");

                    oTable.api().ajax.reload(null, false);
                } else {
                    alert(data);
                }

            },
            error: function () {
                alert("error")
            }
        });
    } else {
        $("#workdonereamrks").css("border", "solid 1px red");
    }
}

function myWorkDonecomment(id) {
    currnetid = id;
    $("#myModalWorkDonecomment").modal("show");
}

function SaveMyworkcomment() {
    if ($("#workdonereamrkscomment").val() != null
    ) {
        var tdata = {
            id: currnetid,
            remarks: $("#workdonereamrkscomment").val()
        };
        $.ajax({
            type: "POST",
            url: "/Home/WorkDoneRemarksComment",
            data: tdata,
            success: function (data) {
                $("#workdonereamrkscomment").val('');
                $("#myModalWorkDonecomment").modal("hide");

                oTable.api().ajax.reload(null, false);
            },
            error: function () {
                alert("error")
            }
        });
    } else {
        $("#workdonereamrkscomment").css("border", "solid 1px red");
    }

}



function clearall() {
    $("#ddlLocation").val("");
    $("#ddlDepartment").val("");
    $("#CategoryOfComplaint").val("");
    $("#Description").val("");
}
function DoneTaks(id) {
    var tdata = {
        id: id
    };
    $.ajax({
        type: "GET",
        url: "/Home/DoneTask",
        data: tdata,
        success: function (data) {
            if (data == "Success") {
                oTable.api().ajax.reload(null, false);
            } else { alert(data); }
        },
        error: function () {
            alert("error")
        }
    });
}

//function TaskCompleted(id)
//{
//    var tdata = {
//        id: id
//    };
//    $.ajax({
//        type: "GET",
//        url: "/Home/DoneTask",
//        data: tdata,
//        success: function (data) {
//            if (data == "Success") {
//                datatablebind();
//            } else { alert(data); }
//        },
//        error: function () {
//            alert("error")
//        }
//    });



//}





function DeleteTask(id) {
    if (confirm("Are you sure you want to delete?")) {
        var tdata = {
            id: id
        };
        $.ajax({
            type: "GET",
            url: "/Home/DeleteTask",
            data: tdata,
            success: function (data) {
                if (data == "Success") {
                    oTable.api().ajax.reload(null, false);
                } else { alert(data); }
            },
            error: function () {
                alert("error")
            }
        });
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
                options.push('<option value="">- Select Department -</option>');
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
function LoadEmployeesForFilter() {
    if ($("#ddlDepartmentForFilter").val() != null && $("#ddlDepartmentForFilter").val() != "") {
        $.ajax({
            type: "POST",
            url: "/Common/GetEmployeesByDepartment",
            data: "id=" + $("#ddlDepartmentForFilter").val(),
            success: function (data) {
                var options = [];
                options.push('<option value="">- Select Employee -</option>');
                for (var i = 0; i < data.length; i++) {
                    options.push('<option value="' + data[i].UserId + '">' + data[i].FirstName + " " + data[i].LastName + '</option>');
                }
                $("#ddlEmployeesForFilter").html(options.join(''));
            },
            error: function () {
                alert("error")
            }
        });
    }
}
//load employees by department by if admin
function LoadDepartmentForFilterbyadmin() {
    if ($("#ddlLocationForFilterbyadmin").val() != null && $("#ddlLocationForFilterbyadmin").val() != "") {
        $.ajax({
            type: "POST",
            url: "/Common/GetDepartmentByLocation",
            data: "id=" + $("#ddlLocationForFilterbyadmin").val(),
            success: function (data) {
                var options = [];
                options.push('<option value="">- Select Department -</option>');
                for (var i = 0; i < data.length; i++) {
                    options.push('<option value="' + data[i].DepartmentId + '">' + data[i].DepartmentName + '</option>');
                }
                $("#ddlDepartmentForFilterifadmin").html(options.join(''));
            },
            error: function () {
                alert("error")
            }
        });
    } else {
        var options = [];
        options.push('<option value="">- Select Department -</option>');

        $("#ddlDepartmentForFilterifadmin").html(options.join(''));
    }
}
function LoadEmployeesForFilterbyadmin() {
    if ($("#ddlDepartmentForFilterifadmin").val() != null && $("#ddlDepartmentForFilterifadmin").val() != "") {
        $.ajax({
            type: "POST",
            url: "/Common/GetEmployeesByDepartment",
            data: "id=" + $("#ddlDepartmentForFilterifadmin").val(),
            success: function (data) {
                var options = [];
                options.push('<option value="">- Select Employee -</option>');
                for (var i = 0; i < data.length; i++) {
                    options.push('<option value="' + data[i].CustomUserId + '">' + data[i].FirstName + " " + data[i].LastName + '</option>');
                }
                $("#ddlEmployeesForFilterbyadmin").html(options.join(''));
            },
            error: function () {
                alert("error")
            }
        });
    }
}
function Loademployeesbydepartment() {
    $.ajax({
        type: "GET",
        url: "/Common/GetEmployeesByDepartmentname",
        success: function (data) {
            var options = [];
            options.push('<option value="">- Select Employee -</option>');
            for (var i = 0; i < data.length; i++) {
                options.push('<option value="' + data[i].CustomUserId + '">' + data[i].FirstName + '</option>');
            }
            $("#ddlEmployeesForFilter").html(options.join(''));
            // $("#ddlEmployees1").html(options.join(''));

        },
        error: function () {
            alert("error")
        }
    });
}
function Loademployeesbytoassign() {
    $.ajax({
        type: "GET",
        url: "/Common/GetEmployeesByDepartmentname",
        success: function (data) {
            var options = [];
            // options('<option value="">- Select Employee -</option>');
            for (var i = 0; i < data.length; i++) {
                options.push('<option value="' + data[i].UserId + '">' + data[i].FirstName + '</option>');
            }
            $("#ddlassignemployeetothetask").html(options.join(''));
            // $("#ddlEmployees1").html(options.join(''));

        },
        error: function () {
            alert("error")
        }
    });
}
/*task is assigned to the employee*/
function AssignTasktoemployees(id) {
    currnetid = id;
    $("#myModalAssignTasktoepmloyee").modal("show");

}
function Assigntakstoemployeesbydepartment() {
    var task = {
        id: currnetid,
        empid: $("#UserIdofemp").val(),
        empname: $("#UserNameofemp").val()
    };
    $.ajax({
        type: "POST",
        url: "/Home/Assigntasktoemployeeesbydepartment",
        data: task,
        success: function (data) {
            OpenSuccessAlertPopUpBox_ConfirmPopUpJS(data); //alert messege added on 16-01-2016
            //   $("#UserNameofemp").val('');
            $("#myModalAssignTasktoepmloyee").modal('hide');
            oTable.api().ajax.reload(null, false);
        },
        error: function () {
            alert("error")
        }
    });
}
function ExportToExcelissues() {
    var params = "FromDate=" + $("#txtfromdate").val() + "&ToDate=" + $("#txttodate").val();
    if ($("#ddlLocationForFilter").val() != null && $("#ddlLocationForFilter").val() != "") {
        params += "&locationid=" + $("#ddlLocationForFilter").val();
    } else {
        params += "&locationid=0";
    }
    if ($("#ddlDepartmentForFilter").val() != null && $("#ddlDepartmentForFilter").val() != "") {
        params += "&departmentid=" + $("#ddlDepartmentForFilter").val();
    } else {
        params += "&departmentid=0";
    }
    params += "&status=" + $("#ddlStatusTypeForFilter").val();
    params += "&emp=" + $("#ddlEmployeesForFilter").val();
    params += "&category=" + $("#ddlCategoryTypeForFilter").val();
    window.location = "/Home/MyTasks_ExportToExcel?" + params;
}

$(document).ready(function () {
    var plan1 = document.getElementById('ddlDepartmentForFilter');
    plan1.addEventListener('change', enableBilling1, false);
    var select1 = document.getElementById('ddlCategoryTypeForFilter');
    /*start it opetions*/
    var it = {
        "Option": ["Shivam", "System", "Printer", "Others", "Leave Management", "Help Desk"],
        "Value": ["Shivam", "Printer", "Printer", "Others", "Leave Management", "Help Desk"]
    }
    /*start maintainence opetions*/
    var maintenance = {
        "Option": ["Electrician", "AC operator", "Plumber", "Carpenter", "Painter", "Gas plant Operator", "Others"],
        "Value": ["Electrician", "AC_operator", "Plumber", "Carpenter", "Painter", "Gas_Plant_Operator", "Others"]
    }
    /*start bio medical opetions*/
    var biomedical = {
        "Option": ["OT", "ICU", "POW", "Scan", "KMC", "1st ward", "2nd ward", "3rd ward", "Lab", "Labour ward", "HDU", "CSSD", "Sample Collection", "Engineer", "OP", "Weight & Measure", "NICU", "ER", " 3 floor A side", "3 floor B side", "FMU", "Gyn", "Follicular study", "Others"],
        "Value": ["OT", "ICU", "POW", "Scan", "KMC", "1st ward", "2nd ward", "3rd ward", "Lab", "Labour ward", "HDU", "CSSD", "Sample Collection", "Engineer", "OP", "Weight & Measure", "NICU", "ER", " 3 floor A side", "3 floor B side", "FMU", "Gyn", "Follicular study", "Others"]
    }

    function populateBilling1(planName) {
        select1.options.length = 1;
        if (planName == "information technology") {
            for (i = 0; i < it.Option.length; i++) {
                var temp = document.createElement('option');
                temp.value = it.Value[i];
                temp.text = it.Option[i];
                select1.appendChild(temp);
            }

        } else if (planName == "maintenance") {
            for (i = 0; i < maintenance.Option.length; i++) {
                var temp = document.createElement('option');
                temp.value = maintenance.Value[i];
                temp.text = maintenance.Option[i];
                select1.appendChild(temp);
            }
        }

        else if (planName == "biomedical") {
            for (i = 0; i < biomedical.Option.length; i++) {
                var temp = document.createElement('option');
                temp.value = biomedical.Value[i];
                temp.text = biomedical.Option[i];
                select1.appendChild(temp);
            }
        }
    }
    function enableBilling1(e) {
        document.getElementById('ddlCategoryTypeForFilter').disabled = false;
        var planName = plan1.options[plan1.selectedIndex].text.toLowerCase();
        populateBilling1(planName);
    }
});
