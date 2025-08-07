//Save Brand


function SaveBrand() {
    if ($('#AssetTypeId').val() != null && $('#AssetTypeId').val() != "" && $('#Name').val() != null && $('#Name').val() != "") {
        var model = {
            AssetBrandId:eId,
            AssetTypeId: $('#AssetTypeId').val(),
            Name: $('#Name').val(),
            Description: $("#Description").val(),
            ShortCode: "",
            IsActive: true
        };
        $.ajax({
            type: "GET",
            url: "/Asset/SaveBrand",
            data: model,
            success: function (data) {
                $("#NewBrandmodel").modal("hide");
                ClearAll();
                //alert("Success");
                LoadData();

            }
        });
    } else {
        alert("Please enter Name and Asset type");
    }
}
function ClearAll() {
    eId = 0;
    $('#AssetTypeId').val("");
    $('#Name').val("");
    $("#Description").val("");
}

function LoadAssetType() {
    $.get("/Asset/GetAssetType", function (data) {

        var options = [];
        options.push('<option value="">- select AssetType -</option>');
        for (var i = 0; i < data.length; i++) {
            options.push('<option value="' + data[i].AssetTypeId + '">' + data[i].Name + '</option>');
        }
        $("#AssetTypeId").html(options.join(''));
    });
}

function SaveModel() {
    if ($('#AssetTypeId').val() != null && $('#AssetTypeId').val() != "" && $('#Name').val() != null && $('#Name').val() != "") {
        var model = {
            AssetModelId: ModeleId,
            AssetTypeId: $('#AssetTypeId').val(),
            AssetBrandId: $('#AssetBrandId').val(),
            Name: $('#Name').val(),
            Desciption: $("#Description").val(),
            ShortCode: "",
            IsActive: true
        };
        $.ajax({
            type: "GET",
            url: "/Asset/SaveAssetModel",
            data: model,
            success: function (data) {
                $("#NewAssetmodel").modal("hide");
                //alert("Success");
                LoadData();
                ClearAllModel();
            }
        });
    } else {
        alert("Please enter Name and Asset type");
    }
}
function ClearAllModel() {
    ModeleId = 0;
    $('#AssetTypeId').val("");
    $('#AssetBrandId').val("");
    $('#Name').val("");
    $("#Description").val("");
}
function OpenNewAssetmodel() {
    $("#NewAssetmodel").modal("show");
}
function OpenNewBrand() {
    $("#NewBrandmodel").modal("show");
}

function LoadAssetBrand(val) {
    $.get("/Asset/GetAssetBrandsByType?id=" + $("#AssetTypeId").val(), function (data) {
        var options = [];
        options.push('<option value="">- select Asset Brand -</option>');
        for (var i = 0; i < data.length; i++) {
            options.push('<option value="' + data[i].AssetBrandId + '">' + data[i].Name + '</option>');
        }
        $("#AssetBrandId").html(options.join(''));
        if (val != "") {
            $("#AssetBrandId").val(val);
        }
    });
}