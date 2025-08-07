function SurveyManager(baseUrl, accessKey) {
    var self = this;
    self.availableSurveys = ko.observableArray();

    self.loadSurveys = function () {
        var xhr = new XMLHttpRequest();
        xhr.open("GET", baseUrl + "/Service/getActive");
        xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        xhr.onload = function () {
            var result = xhr.response ? JSON.parse(xhr.response) : {};
            self.availableSurveys(
              Object.keys(result).map(function (key) {
                  return {
                      id: result[key].key,
                      name: result[key].name || key,
                      survey: result[key].json || result[key]
                  };
              })
            );
        };
        xhr.send();
    };

    self.createSurvey = function (name, onCreate) {
        var xhr = new XMLHttpRequest();
        xhr.open(
          "GET",
          baseUrl + "/Service/create?name=" + name
        );
        xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        xhr.onload = function () {
            var result = xhr.response ? JSON.parse(xhr.response) : null;
            !!onCreate && onCreate(xhr.status == 200, result, xhr.response);
        };
        xhr.send();
    };

    self.deleteSurvey = function (id, onDelete) {
        if (confirm("Are you sure?")) {
            var xhr = new XMLHttpRequest();
            xhr.open("GET", baseUrl + "/Service/delete?id=" + id);
            xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            xhr.onload = function () {
                var result = xhr.response ? JSON.parse(xhr.response) : null;
                !!onDelete && onDelete(xhr.status == 200, result, xhr.response);
            };
            xhr.send();
            window.location = "/Service/Index";
        }
    };

    self.loadSurveys();
}

ko.applyBindings(
  new SurveyManager(""),
  document.getElementById("surveys-list")
);
