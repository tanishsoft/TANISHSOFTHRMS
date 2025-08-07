var eventslist;
var dutyrosterresult;
var shifttypes;
var ddlshifttypesoptions;
var deptemployees;
var isholiday = false;
var isleave = false;
$(document).ready(function () {
    debugger;
    //var date = new Date();
    //var day = date.getDate();
    //var mindatedays = 100;
   
   
    var maxDate = "";
   
    var valuedate = $("#LoginEmployeeType").val();
    if (valuedate != "Admin") {
        //if (day > 25) {
        //    mindatedays = day - 25;

        //} else {
        //    mindatedays = day + 7;
        //}
        var dtToday = new Date();
        var month = dtToday.getMonth();
        var day = dtToday.getDate();
        var year = dtToday.getFullYear();
        if (month < 10)
            month = '0' + month.toString();

        if (day < 22) {
            month -= 1;
            day = 21;
        }
        else {
            day = 22;
        }
        if (day < 10)
            day = '0' + day.toString();
        maxDate=  day + '/' + month + '/' + year;
    }
    
    $("#txtfromdate").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy",
        minDate: maxDate,
        //onSelect: function (selected) {
        //    $("#txttodate").datepicker("option", "minDate", selected)
        //}
    }).val();
    $("#txtfromdate").datepicker("option", "minDate", new Date(year, month, day, 1));
    $("#txttodate").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy",
        minDate: maxDate,
        //onSelect: function (selected) {
        //    $("#txtfromdate").datepicker("option", "maxDate", selected)
        //}
    }).val();
    $("#txttodate").datepicker("option", "minDate", new Date(year, month, day, 1));
    $("#etxtfromdate").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy"

        //onSelect: function (selected) {
        //    var dt = new Date(selected);
        //    dt.setDate(dt.getDate() + 1);
        //    $("#etxttodate").datepicker("option", "minDate", dt);
        //}
    });
    $("#etxttodate").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy"
        //onSelect: function (selected) {
        //    var dt = new Date(selected);
        //    dt.setDate(dt.getDate() - 1);
        //    $("#etxtfromdate").datepicker("option", "maxDate", dt);
        //}
    });
    $("#txtfromdate1").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy",
        onSelect: function (selected) {
            $("#txttodate1").datepicker("option", "minDate", selected)
        }
    });
    $("#txttodate1").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy",
        onSelect: function (selected) {
            $("#txtfromdate1").datepicker("option", "maxDate", selected)
        }
    });
    BindEditshifttypes();
    getmydetemployees();

    $('.bs-component [data-toggle="tooltip"]').tooltip();
});
function formatinputdate(inputDate) {
    var date = new Date(inputDate);
    if (!isNaN(date.getTime())) {
        var month = date.getMonth() + 1;
        // Months use 0 index.
        //return ('0' + date.getDate()).slice(-2) + '/' + ('0' + month).slice(-2) + '/' + date.getFullYear();      
        return ('0' + month).slice(-2) + '/' + ('0' + date.getDate()).slice(-2) + '/' + date.getFullYear();
    }
}
function Idgetdate(inputDate) {
    var date = new Date(inputDate);
    if (!isNaN(date.getTime())) {
        // Months use 0 index.
        return date.getMonth() + 1 + '_' + date.getDate() + '_' + date.getFullYear();
    }
}

function btncalenderclikc() {
    var fromdate = $("#txtfromdate").val();
    var todate = $("#txttodate").val();
    if (fromdate != "" && todate != "") {
        var dataparam = {
            fromdate: $("#txtfromdate").val(),
            todate: $("#txttodate").val(),
            locationid: $("#ddlSelectLocation").val(),
            deptid: $("#ddlSelectDepartment").val()
        };
        $.ajax({
            type: 'POST',
            url: "/Hr/GetemployeeshiftsJson",
            data: dataparam,
            dataType: "json",
            success: function (result) {
                eventslist = result;
                getdatabindnewformat(result);

                        BindShiftTypes();
                    }
                });
            }
        }
        function BindShiftTypes() {
            var options = "<option value=''>Select Shift Type</option>";
            for (var i = 0; i < shifttypes.length; i++) {
                if (shifttypes[i].Name != null) {
                    options += "<option value='" + shifttypes[i].id + "'>" + shifttypes[i].Name + "</option>";
                }
            }
            $("#ddlSelectShifttypes").html(options);
            $("#divshowafterrosterload").css("display", "block");
        }
        function GetCheckBoxCheckedIds() {
            var leaveid = '';
            $(".cballemployeescheck").each(function () {
                if ($(this).prop("checked")) {
                    leaveid = leaveid + $(this).prop("id") + ',';
                }
            });
            return leaveid;
        }
        function cbcheckboxassigndev_changed() {
            //cbcheckall
            var checkedval = false;
            $(".cballemployeescheckAll").each(function () {
                if ($(this).prop("checked")) {
                    checkedval = true;
                }
            });
            if (checkedval == true) {
                $(".cballemployeescheck").each(function () {
                    $(this).prop("checked", true);
                });
            } else {
                $(".cballemployeescheck").each(function () {
                    $(this).prop("checked", false);
                });
            }
        }
        function SubmitSaveBulkAssignShift() {
            var ids = GetCheckBoxCheckedIds();
            if (ids != null && ids != "") {
                if (confirm("Are you sure you want to Assign " + $("#ddlSelectShifttypes option:selected").text() + " Shift To Selected Employees")) {
                    var Data = {
                        Fromdate: $("#txtfromdate").val(),
                        Todate: $("#txttodate").val(),
                        ShiftTypeId: $("#ddlSelectShifttypes").val(),
                        ShiftTypeName: $("#ddlSelectShifttypes option:selected").text(),
                        SelectType: $("#cbselecttype").val(),
                        UserIds: ids
                    }
                    $.ajax({
                        type: 'GET',
                        url: "/Hr/SaveEmployeesShiftdataBulk",
                        data: Data,
                        dataType: "json",
                        success: function (response) {
                            btncalenderclikc();
                            alert(response);
                        },
                        error: function () {
                            alert('error');
                        }
                    });
                }
            }
            else {
                alert("Please select some employees");
            }
        }
        function getdatabindnewformat(datatest) {
            var fromdate = $("#txtfromdate").val();
            var todate = $("#txttodate").val();
            if (fromdate != "" && todate != "") {
                var table = "";
                fromdate = ConvertDatefromoneddmmyyytommddyyy(fromdate);
                todate = ConvertDatefromoneddmmyyytommddyyy(todate);
                var stdate = new Date(fromdate);
                var enddate = new Date(todate);

                table += "<table id='edittable' style='margin:0px;' class='table table-bordered'><thead><tr><td> <input type='checkbox' id='cballemployeescheckAll' onchange='cbcheckboxassigndev_changed()' class='cballemployeescheckAll' /></td><td>Name</td><td>Emp ID</td>";
                var andate = stdate;
                while (andate <= enddate) {
                    var headmonth = parseInt(andate.getMonth()) + 1;
                    //if (andate.getDay() == 0) {
                    table += "<td>" + andate.getDate() + "/" + headmonth + "</td>";
                    //} else {
                    //    table += "<td>" + andate.toDateString() + "</td>";
                    //}
                    andate.setDate(andate.getDate() + 1);
                }
                table += "</tr></thead>";
                var result = deptemployees;
                var filterresults = false;
                var locationIdfilter = $("#ddlSelectLocation").val();
                var DepartmentIdfilter = $("#ddlSelectDepartment").val();
                if ($("#ddlSelectLocation").val() != null && $("#ddlSelectLocation").val() != "0" && $("#ddlSelectLocation").val() != "") {
                    filterresults = true;
                }


                for (var i = 0; i < result.length; i++) {
                    dutyrosterresult = result;
                    var resulrstoenter = true;
                    if (filterresults) {
                        if (result[i].DepartmentId == DepartmentIdfilter && result[i].LocationId == locationIdfilter) {
                            resulrstoenter = true;
                        } else {
                            resulrstoenter = false;
                        }
                    }
                    if (resulrstoenter) {
                        var ssdate = new Date(fromdate);
                        var eedate = enddate;
                        var deptty = "";
                        table += "<tr id='" + result[i].CustomUserId + "'>";
                        table += "<td> <input type='checkbox' id='" + result[i].CustomUserId + "' class='cballemployeescheck' /></td>";
                        table += "<td>" + result[i].FirstName + "</td>";
                        table += "<td>" + result[i].CustomUserId.replace("FH_", "") + "</td>";
                        while (ssdate <= eedate) {
                            var headmonth = parseInt(ssdate.getMonth()) + 1;
                            var idch = result[i].CustomUserId + ";" + Idgetdate(ssdate);
                            table += "<td data-toggle='tooltip' data-placement='top' title='' data-original-title='" + result[i].FirstName + " " + ssdate.getDate() + "/" + headmonth + "'>";
                            isholiday = false;
                            isleave = false;
                            var optionsselect = getddloptions(ConvertJsDatetoMonthdateyear(ssdate), result[i].CustomUserId, datatest);
                            if (isholiday || isleave) {
                                table += "<select id='" + idch + "' disabled style='border:solid 2px red;'> " + optionsselect + "</select>";
                            } else {
                                table += "<select id='" + idch + "'> " + optionsselect + "</select>";
                            }
                            isholiday = false;
                            isleave = false;
                            table += "</td>";
                            ssdate.setDate(ssdate.getDate() + 1);
                        }
                        table += "</tr>";
                    }
                }

                table += "</table>";
                $("#myroster").html(table);

                var table1 = $('#edittable').DataTable({
                    //"scrollX": "100%",
                    //"scrollCollapse": true,
                    destroy: true,
                    //fixedColumns: true,
                    //"bJQueryUI": true,
                    scrollX: true,
                    scrollY: 350,
                    scrollCollapse: true,
                    paging: false,

                    //fixedColumns: {
                    //    leftColumns: 3
                    //},
                    //fixedHeader: {
                    //    header: true

                    //}
                });
                $("#edittable_filter").css("display", "none");
            }
            else {
                alert("Please Select Date or Corretdate");
            }
        }
        var currentshiftname = "";
        function Getempdutyshiftid(date, empid, data) {
            var shifttype = "";
            isholiday = false;
            isleave = false;
            if (data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    if (empid == data[i].EmployeeId) {
                        var checkdate123 = formatinputdate(data[i].Fromdate);
                        //var checkdate123 = ConvertDatefromoneddmmyyytommddyyy(data[i].Fromdate);

                        if (date == checkdate123) {
                            // console.log("Edit - PassDate - " + date + " FromDate : " + checkdate123 + " Holiday -" + data[i].IsHoliday + " ISLeave - " + data[i].CheckLeave);
                            shifttype = data[i].ShiftId;
                            isholiday = data[i].IsHoliday;
                            currentshiftname = data[i].ShiftTypeName;
                            if (data[i].LeaveType != null && data[i].LeaveType != "")
                                isleave = true;
                        }
                    }
                }
            }
            return shifttype;
        }

        function getddloptions(date, emp, data) {
            isholiday = false;
            isleave = false;
            currentshiftname = "";
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
                            options += "<option value='" + shifttypes[i].id + "' selected>" + currentshiftname + "</option>";

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
            fromdate = ConvertDatefromoneddmmyyytommddyyy(fromdate);
            todate = ConvertDatefromoneddmmyyytommddyyy(todate);
            var stdate = new Date(fromdate);
            var enddate = new Date(todate);
            if (dutyrosterresult != null && dutyrosterresult.length > 0) {
                for (var i = 0; i < dutyrosterresult.length; i++) {
                    var ssdate = new Date(fromdate);
                    var eedate = enddate;
                    while (ssdate <= eedate) {
                        var idch = dutyrosterresult[i].CustomUserId + ";" + Idgetdate(ssdate);
                        if (document.getElementById(idch) != null && document.getElementById(idch) != "") {
                            var value = document.getElementById(idch).value;
                            var t = document.getElementById(idch);
                            if (value != null && value != "") {
                                empshitdata.push({
                                    ShiftTypeId: value,
                                    ShiftTypeName: t.options[t.selectedIndex].text,
                                    UserId: dutyrosterresult[i].CustomUserId,
                                    ShiftDate: ConvertJsDatetoMonthdateyear(ssdate),
                                });
                            }
                        }
                        ssdate.setDate(ssdate.getDate() + 1);
                    }
                }
                SaveorupdateShifts(empshitdata);
            }
        }
        function ConvertJsDatetoMonthdateyear(sysdate) {
            //return () + '/' + sysdate.getDate() + '/' + sysdate.getFullYear();
            var intmonth = parseInt(sysdate.getMonth());
            intmonth = intmonth + 1;
            return ('0' + intmonth).slice(-2) + '/' + ('0' + sysdate.getDate()).slice(-2) + '/' + sysdate.getFullYear();
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
            var fromdate = $("#txtfromdate1").val();
            var todate = $("#txttodate1").val();
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
                        Bindtabletoview(result, fromdate, todate);
                    }
                });
            }
            else {
                alert("Please Select Date");
            }
        }
        function LoadDutyRoster() {
            var fromdate = $("#txtfromdate1").val();
            var todate = $("#txttodate1").val();
            if (fromdate != "" && todate != "" && $("#ddlLocation").val() != null && $("#ddlLocation").val() != "") {
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
                        Bindtabletoview(result, fromdate, todate);
                    }
                });
            }
            else {
                alert("Please Select Date and Location ");
            }
        }

        function Bindtabletoview(eventslist, fromdate, todate) {
            //var fromdate = $("#txtfromdate1").val();
            //var todate = $("#txttodate1").val();
            var table = "";
            fromdate = ConvertDatefromoneddmmyyytommddyyy(fromdate);
            todate = ConvertDatefromoneddmmyyytommddyyy(todate);
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
            //Check Conditions
            var checkcondition = false;
            var locationId = null;
            var deparmtnetId = null;
            var employeeidnew = null;
            if ($("#ddlLocation").val() != null && $("#ddlLocation").val() != "") {
                checkcondition = true;
                locationId = $("#ddlLocation").val();
            }
            if ($("#ddlDepartment").val() != null && $("#ddlDepartment").val() != "") {
                checkcondition = true;
                deparmtnetId = $("#ddlDepartment").val();
            }
            if ($("#ddlEmployees").val() != null && $("#ddlEmployees").val() != "") {
                employeeidnew = $("#ddlEmployees").val();
            }
            var result = deptemployees;
            for (var i = 0; i < result.length; i++) {
                if (checkcondition) {
                    if (result[i].LocationId == locationId) {
                        if (deparmtnetId != null) {
                            if (result[i].DepartmentId == deparmtnetId) {
                                var ssdate = new Date(fromdate);
                                var eedate = enddate;
                                if (employeeidnew != null && employeeidnew != "" && employeeidnew != "0") {
                                    if (result[i].CustomUserId == employeeidnew) {
                                        table += "<tr id='" + result[i].CustomUserId + "'>";
                                        table += "<td>" + result[i].FirstName + "</td>";
                                        table += "<td>" + result[i].CustomUserId.replace("FH_", "") + "</td>";
                                        while (ssdate <= eedate) {
                                            var idch = result[i].CustomUserId + ";" + ssdate.toDateString();
                                            table += "<td  title='" + result[i].FirstName + "' id='" + idch + "'>" + Getempduty(ConvertJsDatetoMonthdateyear(ssdate), result[i].CustomUserId, eventslist) + "</td>";
                                            ssdate.setDate(ssdate.getDate() + 1);
                                        }
                                        table += "</tr>";
                                    }

                                } else {

                                    table += "<tr id='" + result[i].CustomUserId + "'>";
                                    table += "<td>" + result[i].FirstName + "</td>";
                                    table += "<td>" + result[i].CustomUserId.replace("FH_", "") + "</td>";
                                    while (ssdate <= eedate) {
                                        var idch = result[i].CustomUserId + ";" + ssdate.toDateString();
                                        table += "<td  title='" + result[i].FirstName + "' id='" + idch + "'>" + Getempduty(ConvertJsDatetoMonthdateyear(ssdate), result[i].CustomUserId, eventslist) + "</td>";
                                        ssdate.setDate(ssdate.getDate() + 1);
                                    }
                                    table += "</tr>";
                                }
                            }
                        } else {
                            var ssdate = new Date(fromdate);
                            var eedate = enddate;
                            table += "<tr id='" + result[i].CustomUserId + "'>";
                            table += "<td>" + result[i].FirstName + "</td>";
                            table += "<td>" + result[i].CustomUserId.replace("FH_", "") + "</td>";
                            while (ssdate <= eedate) {
                                var idch = result[i].CustomUserId + ";" + ssdate.toDateString();
                                table += "<td title='" + result[i].FirstName + "' id='" + idch + "'>" + Getempduty(ConvertJsDatetoMonthdateyear(ssdate), result[i].CustomUserId, eventslist) + "</td>";
                                ssdate.setDate(ssdate.getDate() + 1);
                            }
                            table += "</tr>";
                        }
                    }
                } else {
                    var ssdate = new Date(fromdate);
                    var eedate = enddate;
                    table += "<tr id='" + result[i].CustomUserId + "'>";
                    table += "<td>" + result[i].FirstName + "</td>";
                    table += "<td>" + result[i].CustomUserId.replace("FH_", "") + "</td>";
                    while (ssdate <= eedate) {
                        var idch = result[i].CustomUserId + ";" + ssdate.toDateString();
                        table += "<td title='" + result[i].FirstName + "' id='" + idch + "'>" + Getempduty(ConvertJsDatetoMonthdateyear(ssdate), result[i].CustomUserId, eventslist) + "</td>";
                        ssdate.setDate(ssdate.getDate() + 1);
                    }
                    table += "</tr>";
                }
            }
            table += "</table>";
            $("#viewdivroster").html(table);
            var table = $('#myDataTable').DataTable({
                //"scrollX": "100%",
                //"scrollCollapse": true,
                "bJQueryUI": true,
                "paging": false,
                //"sDom": 'T<"clear">lfrtip',
                //"oTableTools": {
                //    "sSwfPath": "/swf/copy_csv_xls_pdf.swf"
                //},
                dom: 'Bfrtip',
                buttons: [
                    'print'
                ],
                "aaSorting": [[1, "asc"]]
            });
        }

        function Getempduty(date, empid, data) {
            var shifttype = "";
            if (data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    if (empid == data[i].EmployeeId) {
                        //alert(data[i].Fromdate);
                        //var datadate = new Date(data[i].Fromdate);
                        //data[i].Fromdate = ConvertDatefromoneddmmyyytommddyyy(data[i].Fromdate);
                        var datetestcomp = formatinputdate(data[i].Fromdate);
                        //var datetestcomp = ConvertDatefromoneddmmyyytommddyyy(data[i].Fromdate);
                        // console.log("View - PassDate - " + date + " FromDate : " + datetestcomp + " " + data[i].LeaveType);
                        if (datetestcomp == date) {
                            shifttype = data[i].ShiftTypeName;

                            if (shifttype == "WO" || shifttype == "H") {
                                shifttype = "<b style='color:green;'>" + data[i].ShiftTypeName + "</b>";
                            } else if (shifttype.indexOf("/") !== -1) {
                                shifttype = "<b style='color:orange;'>" + data[i].ShiftTypeName + "</b>";
                            }
                            if (data[i].LeaveType != null && data[i].LeaveType != "") {
                                shifttype += " <b style='color:red;'>" + data[i].LeaveType + "</b>";
                            }
                        }
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
                type: 'GET',
                url: "/Hr/Getmydeptemps",
                data: { typeofview: $("#hidtypeofview").val() },
                dataType: "json",
                success: function (result) {
                    deptemployees = result;
                    //var options = [];
                    //options.push('<option value="0">- Select Employee -</option>');
                    //for (var i = 0; i < deptemployees.length;   i++) {
                    //    options.push('<option value="' + deptemployees[i].CustomUserId + '">' + deptemployees[i].FirstName + '</option>');
                    //}
                    //$("#ddlselectemployees").html(options.join(''));
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
                    options.push('<option value="0">- All Locations -</option>');
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
        function ExportExcelData() {
            window.location = "/Hr/ExportToExcelViewDutyRoster?fromdate=" + $("#txtfromdate1").val() + "&todate=" + $("#txttodate1").val();
        }
