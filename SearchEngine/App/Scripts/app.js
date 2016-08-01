(function () {
    var app = angular.module('SearchEngine', ['ngRoute', 'ngSanitize']);

    app.config(function ($routeProvider) {
        $routeProvider
            .when("/",
            {
                controller: "homeController",
                templateUrl: "App/Views/Search.html"
            })
            .otherwise("/");
    });

    app.run(function ($rootScope, $location, $http) {

        $http.get2 = function (route, parameters, config) {
            var queryString = route;
            var parsedParameters = {};
            if (angular.isDefined(parameters) && parameters != null) {
                for (var prop in parameters) {
                    if (parameters.hasOwnProperty(prop)) {
                        if (angular.isDefined(parameters[prop]) && parameters[prop] !== null) {
                            parsedParameters[prop] = parameters[prop];
                        }
                    }
                }
            }

            var count = 0;

            for (var parsedProp in parsedParameters) {
                if (parsedParameters.hasOwnProperty(parsedProp)) {
                    if (count === 0) {
                        queryString += "?";
                    } else {
                        queryString += "&";
                    }
                    queryString += parsedProp + "=" + parsedParameters[parsedProp];
                    count++;
                }

            }
            return $http.get(queryString, config);
        };
    });
})();