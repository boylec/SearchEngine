(function() {
    'use strict';

    angular
        .module('SearchEngine')
        .factory('searchService', searchService);

    searchService.$inject = ['$http', '$q'];

    function searchService($http, $q) {
        var fns = {};

        fns.doSearch = function(searchInput) {
            var deferral = $q.defer();

            $http.post("api/Search/DoSearch", searchInput, null)
                .then(
                    function(response) {
                        deferral.resolve(response.data);
                    },
                    function(response) {
                        deferral.reject(response);
                    });
            return deferral.promise;
        };

        fns.getSuggestion = function (searchInput) {
            var deferral = $q.defer();

            $http.post("api/Search/GetSuggestion", searchInput, null)
                .then(
                    function (response) {
                        deferral.resolve(response.data);
                    },
                    function (response) {
                        deferral.reject(response);
                    });
            return deferral.promise;
        };

        return fns;
    }
})();