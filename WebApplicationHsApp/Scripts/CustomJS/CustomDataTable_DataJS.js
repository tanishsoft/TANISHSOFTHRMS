

var AdminEntityID;
$(document).ready(function () {
    AdminEntityID = jQuery("#LayOutHiddenAdminID").val();
});

/*Normal DataTable. Enable By TableID*/
function EnableDataTableByTableID_CustomDataTableDataJS(TableID) {
    $("#" + TableID).dataTable({
        "bDestroy": true,
        "bAutoWidth": true,
        "iDisplayLength": 10,
        "sPaginationType": "full_numbers",
        "iDisplayLength": 10,
        "aLengthMenu": [[10, 20, 30, 50, 100, 150, 200, 500, 1000], [10, 20, 30, 50, 100, 150, 200, 500, 1000]],
    });
}

/*DataTable With Last Column Sortable Disable. Enable By TableIDe*/
function EnableDataTableByTableIDWithLastColumnSortableDisabled_CustomDataTableDataJS(TableID) {
    $("#" + TableID).dataTable({
        "bDestroy": true,
        "bAutoWidth": true,
        "iDisplayLength": 10,
        "sPaginationType": "full_numbers",
        "iDisplayLength": 10,
        "aLengthMenu": [[10, 20, 30, 50, 100, 150, 200, 500, 1000], [10, 20, 30, 50, 100, 150, 200, 500, 1000]],
        "aoColumnDefs": [{ "bSortable": false, "aTargets": [-1], /* 1st colomn, starting from the right */ }]
    });
}

/*DataTable With First Column Sortable Disable. Enable By ClassName*/
function EnableDataTableByClassNameWithFirstColumnSortableDisabled_CustomDataTableDataJS(ClassName) {
    $("." + ClassName).dataTable({
        "bDestroy": true,
        "bAutoWidth": true,
        "iDisplayLength": 10,
        "sPaginationType": "full_numbers",
        "iDisplayLength": 10,
        "aLengthMenu": [[10, 20, 30, 50, 100, 150, 200, 500, 1000], [10, 20, 30, 50, 100, 150, 200, 500, 1000]],
        "aoColumnDefs": [{ "bSortable": false, "aTargets": [0], /* 1st colomn, starting from the right */ }]
    });
}

/*DataTable With First Column Sortable Disable. Enable By TableID*/
function EnableDataTableByTableIDWithFirstColumnSortableDisabled_CustomDataTableDataJS(TableID) {
    $("#" + TableID).dataTable({
        "bDestroy": true,
        "bAutoWidth": true,
        "iDisplayLength": 10,
        "sPaginationType": "full_numbers",
        "iDisplayLength": 10,
        "aLengthMenu": [[10, 20, 30, 50, 100, 150, 200, 500, 1000], [10, 20, 30, 50, 100, 150, 200, 500, 1000]],
        "aoColumnDefs": [{ "bSortable": false, "aTargets": [0], /* 1st colomn, starting from the right */ }]
    });
}

/*DataTable With Display Length 5. Enable By Class Name*/
function EnableDataTableByClassName_CustomDataTableDataJS(ClassName) {
    $('.' + ClassName).dataTable({
        "bDestroy": true,
        "iDisplayLength": 5,
        "sPaginationType": "full_numbers",
        "aLengthMenu": [[5, 10, 20, 30, 50, 100, 150, 200, 500, 1000], [5, 10, 20, 30, 50, 100, 150, 200, 500, 1000]]
    });
}

/*DataTable With Display Length 5. Enable By TableID*/
function EnableDataTableByTableIDWithDisplayLength_5_CustomDataTableDataJS(TableID) {
    $('#' + TableID).dataTable({
        "bDestroy": true,
        "iDisplayLength": 5,
        "sPaginationType": "full_numbers",
        "aLengthMenu": [[5, 10, 20, 30, 50, 100, 150, 200, 500, 1000], [5, 10, 20, 30, 50, 100, 150, 200, 500, 1000]]
    });
}

/*DataTable With Copy/XSL/PDF and Last Column Sortable Disable. Enable By TableID*/
function EnableDataTableWithCopyXSLPDFAndLCSortableDisable_CustomDataTableDataJS(TableID) {
    $("#" + TableID).dataTable({
        "bDestroy": true,
        "bAutoWidth": true,
        "iDisplayLength": 10,
        "sPaginationType": "full_numbers",
        "iDisplayLength": 10,
        "aLengthMenu": [[10, 20, 30, 50, 100, 150, 200, 500, 1000], [10, 20, 30, 50, 100, 150, 200, 500, 1000]],
        "aoColumnDefs": [{ "bSortable": false, "aTargets": [-1], /* 1st colomn, starting from the right */ }],
        "sDom": 'T<"clear">lfrtip',
        "oTableTools": {
            "sSwfPath": "/swf/copy_csv_xls_pdf.swf",
            "aButtons": [
                "copy", "xls", "pdf"
            ]
        }
    });
}

/*DataTable With First Column Sortable Disable. Enable By TableIDe*/
function EnableDataTableByTableIDWithFirstColumnSortableDisabled_CustomDataTableDataJS(TableID) {
    $("#" + TableID).dataTable({
        "bDestroy": true,
        "bAutoWidth": true,
        "iDisplayLength": 10,
        "sPaginationType": "full_numbers",
        "iDisplayLength": 10,
        "aLengthMenu": [[10, 20, 30, 50, 100, 150, 200, 500, 1000], [10, 20, 30, 50, 100, 150, 200, 500, 1000]],
        "aoColumnDefs": [{ "bSortable": false, "aTargets": [0], /* 1st colomn, starting from the right */ }]
    });
}