function getParams() {
  var url = window.location.href
    .slice(window.location.href.indexOf("?") + 1)
    .split("&");
  var result = {};
  url.forEach(function(item) {
    var param = item.split("=");
    result[param[0]] = param[1];
  });
  return result;
}

function SurveyManager(baseUrl, accessKey) {
  var self = this;
  self.surveyId = decodeURI(getParams()["id"]);
  self.results = ko.observableArray();
  Survey.dxSurveyService.serviceUrl = "";
  var survey = new Survey.Model({
    surveyId: self.surveyId,
    surveyPostId: self.surveyId
  });
  self.columns = ko.observableArray();

  self.loadResults = function() {
    var xhr = new XMLHttpRequest();
    xhr.open("GET", baseUrl + "/Service/GetResults?postId=" + self.surveyId);
    xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
    xhr.onload = function() {
      var result = xhr.response ? JSON.parse(xhr.response) : [];
      self.results(
        result.map(function(r) {
          return JSON.parse(r || "{}");
        })
      );
      self.columns(
        survey.getAllQuestions().map(function(q) {
          return {
            data: q.name,
            sTitle: (q.title || "").trim(" ") || q.name,
            mRender: function(rowdata) {
              return (
                (typeof rowdata === "string"
                  ? rowdata
                  : JSON.stringify(rowdata)) || ""
              );
            }
          };
        })
      );
      self.columns.push({
        targets: -1,
        data: null,
        sortable: false,
        defaultContent:
          "<button style='min-width: 150px;'>Show in Survey</button>"
      });
      var table = $("#resultsTable").DataTable({
          columns: self.columns(),
          data: self.results(),
          dom: 'Bfrtip',
         
          buttons: ["copy",  "csv"]
      });

      var json = new Survey.JsonObject().toJsonObject(survey);
      var windowSurvey = new Survey.SurveyWindow(json);
      windowSurvey.survey.mode = "display";
      windowSurvey.survey.title = self.surveyId;
      windowSurvey.show();
      $("#btnExport").click(function (e) {
          window.open('data:application/vnd.ms-excel,' + $('#resultsTable').html());
          e.preventDefault();
      });
      $(document).on("click", "#resultsTable td", function(e) {
        var row_object = table.row(this).data();
        windowSurvey.survey.data = row_object;
        windowSurvey.isExpanded = true;
      });
    };
    xhr.send();
  };

  survey.onLoadSurveyFromService = function() {
    self.loadResults();
  };
}

ko.applyBindings(new SurveyManager(""), document.body);
