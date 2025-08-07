
function ConvertTasklisttoarry(liReportData) {
    var alist = new Array();    
    for (var j = 0; j < liReportData.length; j++) {
        var data = {};
        data = { name: liReportData[j].Name, data: liReportData[j].data };
        alist.push(data);
    }   
    return alist;
}