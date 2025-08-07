

var eventslist;
var dutyrosterresult;
var shifttypes;
var ddlshifttypesoptions;
var deptemployees;
var EmpLeaveList_PageLevel;
$(document).ready(function () {

    $("#txtfromdate").datepicker({

        onSelect: function (selected) {
            var dt = new Date(selected);
            dt.setDate(dt.getDate() + 1);
            $("#txttodate").datepicker("option", "minDate", dt);
        }
    });
    $("#txttodate").datepicker({

        onSelect: function (selected) {
            var dt = new Date(selected);
            dt.setDate(dt.getDate() - 1);
            $("#txtfromdate").datepicker("option", "maxDate", dt);
        }
    });
    $("#etxtfromdate").datepicker({

        onSelect: function (selected) {
            var dt = new Date(selected);
            dt.setDate(dt.getDate() + 1);
            $("#etxttodate").datepicker("option", "minDate", dt);
        }
    });
    $("#etxttodate").datepicker({

        onSelect: function (selected) {
            var dt = new Date(selected);
            dt.setDate(dt.getDate() - 1);
            $("#etxtfromdate").datepicker("option", "maxDate", dt);
        }
    });
    GetEmployeeLeaveListToAdmin_PageLevel();
    BindEditshifttypes();
    getmydetemployees();
});



function btncalenderclikc() {
    var fromdate = $("#txtfromdate").val();
    var todate = $("#txttodate").val();
    if (fromdate != "" && todate != "") {
        var table = "";
        var stdate = new Date(fromdate);
        var enddate = new Date(todate);

        var dataparam = {
            fromdate: stdate.toDateString(),
            todate: enddate.toDateString()
        };
        $.ajax({
            type: 'POST',
            url: "/Hr/GetemployeeshiftsJson",
            data: dataparam,
            dataType: "json",
            success: function (result) {
                eventslist = result;
                getdatabindnewformat(result);
            }
        });
    }
}

function getdatabindnewformat(datatest) {
    var fromdate = $("#txtfromdate").val();
    var todate = $("#txttodate").val();
    if (fromdate != "" && todate != "") {
        var table = "";
        var stdate = new Date(fromdate);
        var enddate = new Date(todate);
        table += "<table id='edittable' class='table table-bordered'><thead><tr><td>Name</td><td>Emp ID</td>";
        var andate = stdate;
        while (andate <= enddate) {
            if (andate.getDay() == 0) {
                table += "<td>" + andate.getDate() + "/" + andate.getMonth() + "</td>";
            } else {
                table += "<td>" + andate.getDate() + "/" + andate.getMonth() + "</td>";
            }
            andate.setDate(andate.getDate() + 1);
        }
        table += "</tr></thead>";

        var result = deptemployees;

        for (var i = 0; i < result.length; i++) {
            dutyrosterresult = result;
            var ssdate = new Date(fromdate);

            var eedate = enddate;
            var deptty = "";

            table += "<tr id='" + result[i].CustomUserId + "'>";

            table += "<td>" + result[i].FirstName + "</td>";
            table += "<td>" + result[i].CustomUserId + "</td>";

            while (ssdate <= eedate) {
                var idch = result[i].CustomUserId + ";" + ssdate.toLocaleDateString();
                table += "<td title='" + result[i].FirstName + "'><select id='" + idch + "'> " + getddloptions(ssdate.toLocaleDateString(), result[i].CustomUserId, datatest) + "</select></td>";
                ssdate.setDate(ssdate.getDate() + 1);
            }
            table += "</tr>";
        }

        table += "</table>";
        table += "<div style='width:70%;margin:auto;'><br /> <div class='input-control text'><button type='button' Value='Save Data' id='btnsave' style='font-weight:bold;' class='success' onclick='SaveEmployeeshitsbynew();'><i class='icon-enter'></i>&nbsp;Save Data</button></div></div>";
        $("#myroster").html(table);

        //var table1 = $('#edittable').DataTable({
        //    //"scrollX": "100%",
        //    //"scrollCollapse": true,
        //    "bJQueryUI": true,
        //    "paging": false
        //});
        //new $.fn.dataTable.FixedColumns(table1);
        //new $.fn.dataTable.FixedHeader(table1);
    }
    else {
        alert("Please Select Date or Corretdate");
    }
}

function Getempdutyshiftid(date, empid, data) {
    var shifttype = "";
    if (data.length > 0) {
        for (var i = 0; i < data.length; i++) {
            if (empid == data[i].EmployeeId) {
                //alert(data[i].Fromdate);
                //var datadate = new Date(data[i].Fromdate);
                console.log(data[i].Fromdate + " " + date);
                if (date == data[i].Fromdate)
                    shifttype = data[i].ShiftId;
            }
        }
    }
    return shifttype;
}

function getddloptions(date, emp, data) {
    var shift = Getempdutyshiftid(date, emp, data);
    if (shift == "")
        return ddlshifttypesoptions;
    else {
        //var options = [];
        //options.push('<option value="', "", '">', "-", '</option>');
        var options = "<option value=''>-</option>";
        for (var i = 0; i < shifttypes.length; i++) {
            if (shifttypes[i].Name != null) {
                if (shift == shifttypes[i].id) {
                    options += "<option value='" + shifttypes[i].id + "' selected>" + shifttypes[i].Name + "</option>";

                } else
                    options += "<option value='" + shifttypes[i].id + "'>" + shifttypes[i].Name + "</option>";
            }
        }
        //var dataretun = options.join('');
        return options;
    }
}
function SaveEmployeeshitsbynew() {
    var empshitdata = new Array();

    var fromdate = $("#txtfromdate").val();
    var todate = $("#txttodate").val();
    var stdate = new Date(fromdate);
    var enddate = new Date(todate);
    if (dutyrosterresult != null && dutyrosterresult.length > 0) {
        for (var i = 0; i < dutyrosterresult.length; i++) {
            var ssdate = new Date(fromdate);
            var eedate = enddate;
            while (ssdate <= eedate) {
                var idch = dutyrosterresult[i].CustomUserId + ";" + ssdate.toLocaleDateString();
                var value = document.getElementById(idch).value;
                if (value != null && value != "") {
                    empshitdata.push({
                        ShiftTypeId: value,
                        UserId: dutyrosterresult[i].CustomUserId,
                        ShiftDate: ssdate.toLocaleDateString(),
                    });
                }
                ssdate.setDate(ssdate.getDate() + 1);
            }

        }
        SaveorupdateShifts(empshitdata);
    }
}

function SaveorupdateShifts(empshitdata) {

    var dataparam = {
        fromdate: $("#txtfromdate").val(),
        todate: $("#txttodate").val(),
        data: JSON.stringify(empshitdata)
    }
    $.ajax({
        type: 'POST',
        url: "/Hr/SaveEmployeesShiftdata",
        data: dataparam,
        dataType: "json",
        success: function (response) {

            alert(response);
        },
        error: function () {
            alert('error');
        }
    });
}
function btnviewclick() {
    var fromdate = $("#txtfromdate").val();
    var todate = $("#txttodate").val();
    if (fromdate != "" && todate != "") {
        var dataparam = {
            fromdate: fromdate,
            todate: todate
        };
        $.ajax({
            type: 'POST',
            url: "/Hr/GetemployeeshiftsJson",
            data: dataparam,
            dataType: "json",
            success: function (result) {
                Bindtabletoview(result);
            }
        });
    }
    else {
        alert("Please Select Date");
    }
}
function LoadDutyRoster() {
    var fromdate = $("#txtfromdate").val();
    var todate = $("#txttodate").val();
    if (fromdate != "" && todate != "") {
        var dataparam = {
            fromdate: fromdate,
            todate: todate,
            locationid: $("#ddlLocation").val(),
            deptid: $("#ddlDepartment").val(),
            userid: $("#ddlEmployees").val()

        };
        $.ajax({
            type: 'POST',
            url: "/Hr/GetemployeeshiftsJson",
            data: dataparam,
            dataType: "json",
            success: function (result) {
                Bindtabletoview(result);
            }
        });
    }
    else {
        alert("Please Select Date");
    }
}

function Bindtabletoview(eventslist) {
    var fromdate = $("#txtfromdate").val();
    var todate = $("#txttodate").val();
    var table = "";
    var stdate = new Date(fromdate);
    var enddate = new Date(todate);
    table += "<table id='myDataTable' class='table  table-bordered'><thead><tr><td>Name</td><td>Emp  ID</td>";
    var andate = stdate;
    while (andate <= enddate) {
        if (andate.getDay() == 0) {
            table += "<td>" + andate.toDateString() + "</td>";
        } else {
            table += "<td>" + andate.toDateString() + "</td>";
        }
        andate.setDate(andate.getDate() + 1);
    }
    table += "</tr></thead>";

    var result = deptemployees;
    for (var i = 0; i < result.length; i++) {
        var ssdate = new Date(fromdate);
        var eedate = enddate;

        table += "<tr id='" + result[i].CustomUserId + "'>";
        table += "<td>" + result[i].FirstName + "</td>";
        table += "<td>" + result[i].CustomUserId + "</td>";


        while (ssdate <= eedate) {
            var idch = result[i].CustomUserId + ";" + ssdate.toDateString();
            table += "<td title='" + result[i].FirstName + "' id='" + idch + "'>" + Getempduty(ssdate, result[i].CustomUserId, eventslist) + "</td>";
            ssdate.setDate(ssdate.getDate() + 1);
        }
        table += "</tr>";
    }
    table += "</table>";
    $("#viewdivroster").html(table);
    var table = $('#myDataTable').DataTable({
        //"scrollX": "100%",
        //"scrollCollapse": true,
        "bJQueryUI": true,
        "paging": false,
        "sDom": 'T<"clear">lfrtip',
        "oTableTools": {
            "sSwfPath": "/swf/copy_csv_xls_pdf.swf"
        }
    });

}

function Getempduty(date, empid, data) {

    var shifttype = "";
    if (data.length > 0) {
        for (var i = 0; i < data.length; i++) {
            if (empid == data[i].EmployeeId) {
                var datadate = new Date(data[i].Fromdate);

                if (date.toDateString() == datadate.toDateString())
                    shifttype = data[i].ShiftTypeName + " " + GetLeaveTypeNameByID_PageLevel(date, empid);
            }
        }
    }
    return shifttype;
}

function GetShiftName(id) {
    for (var i = 0; i < shifttypes.length; i++) {
        if (shifttypes[i].id == id)
            return shifttypes[i].Name;
    }
}
function BindEditshifttypes() {
    var options = [];
    options.push('<option value="', "", '">', "-", '</option>');
    $.ajax({
        type: 'POST',
        url: "/Hr/GetShiftType",
        data: 'id=1',
        dataType: "json",
        success: function (result) {
            shifttypes = result;
            var table = "<table class='table table-bordered'><thead><tr class='warning'><td>Type</td><td>Start Time</td><td>End Time</td></tr></thead>";
            for (var i = 0; i < result.length; i++) {
                if (result[i].Name != null) {
                    table += "<tr><td>" + result[i].Name + "</td><td>" + result[i].starttime + "</td><td>" + result[i].endtime + "</td></tr>";
                    options.push('<option value="', result[i].id, '">', result[i].Name, '</option>');
                }
            }
            table += "</table>";
            $("#ViewShifttypes").html(table);
            ddlshifttypesoptions = options.join('');
        },
        error: function () {
            alert('error');
        }
    });
}
function getmydetemployees() {
    $.ajax({
        type: 'POST',
        url: "/Hr/Getmydeptemps",
        data: 'id=1',
        dataType: "json",
        success: function (result) {
            deptemployees = result;
        }
    });
}

function btnviewshiftclick() {
    $("#divviewduty").css("display", "inline");
    $("#diveditshifts").css("display", "none");
    $("#SetDeptType").css("display", "none");
    $("#myroster").html(' ');

}
function btneditshiftsclick() {
    $("#diveditshifts").css("display", "inline");
    $("#SetDeptType").css("display", "none");
    $("#divviewduty").css("display", "none");
    $("#viewdivroster").html(' ');

}
function btnsetdepttypeclick() {
    $("#diveditshifts").css("display", "none");
    $("#divviewduty").css("display", "none");
    $("#SetDeptType").css("display", "inline");
    $("#enittydata").html(' ');
    BindEntitydata();
}

function LoadLocations() {
    $.ajax({
        type: "POST",
        url: "/Common/GetLocations",
        //data: "id=",
        success: function (data) {
            var options = [];
            options.push('<option value="0">- Select Location -</option>');
            for (var i = 0; i < data.length; i++) {
                options.push('<option value="' + data[i].LocationId + '">' + data[i].LocationName + '</option>');
            }
            $("#ddlLocation").html(options.join(''));
        },
        error: function () {
            alert("error")
        }
    });
}
function LoadDepartment() {
    if ($("#ddlLocation").val() != null && $("#ddlLocation").val() != "") {
        $.ajax({
            type: "POST",
            url: "/Common/GetDepartmentByLocation",
            data: "id=" + $("#ddlLocation").val(),
            success: function (data) {
                var options = [];
                options.push('<option value="0">- Select Department -</option>');
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
function LoadEmployeesByDepartment() {
    if ($("#ddlDepartment").val() != null && $("#ddlDepartment").val() != "") {
        $.ajax({
            type: "POST",
            url: "/Common/GetEmployeesByDepartment",
            data: "id=" + $("#ddlDepartment").val(),
            success: function (data) {
                var options = [];
                options.push('<option value="0">- Select Employee -</option>');
                for (var i = 0; i < data.length; i++) {
                    var name = data[i].FirstName + " " + data[i].LastName;
                    options.push('<option value="' + data[i].CustomUserId + '">' + name + '</option>');
                }
                $("#ddlEmployees").html(options.join(''));
            },
            error: function () {
                alert("error")
            }
        });
    } else {
        var options = [];
        options.push('<option value="">- Select Employee -</option>');

        $("#ddlEmployees").html(options.join(''));
    }
}

function GetEmployeeLeaveListToAdmin_PageLevel() {
    EmpLeaveList_PageLevel = new Array();
    $.ajax({
        type: 'POST',
        url: "/Report/GetEmployeeLeaveListToAdmin",
        dataType: "json",
        success: function (Result) {
            EmpLeaveList_PageLevel = Result;
        }
    });
}

function GetLeaveTypeNameByID_PageLevel(LeaveDate, EmployeeId) {
    var ReturnStatus = '';
    for (var i in EmpLeaveList_PageLevel) {
        var datadate = new Date(EmpLeaveList_PageLevel[i].LeaveDate);
        if (LeaveDate.toDateString() == datadate.toDateString() && EmpLeaveList_PageLevel[i].EmployeeId == EmployeeId) {
            ReturnStatus = '<text style="color:red;">[' + EmpLeaveList_PageLevel[i].LeaveTypeName_ShortCut + ']</text>';
            break;
        }
    }
    return ReturnStatus;
}