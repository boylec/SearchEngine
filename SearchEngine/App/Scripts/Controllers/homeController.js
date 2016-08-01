(function () {
    'use strict';
    angular
        .module('SearchEngine')
        .controller('homeController', homeController);

    homeController.$inject = ["$scope", "searchService"];

    function homeController($scope, searchService) {
        $scope.searchInput = new SearchInput();
        $scope.searchResult = new SearchResult();
        $scope.searchPerformed = false;
        $scope.searchError = null;
        $scope.doSearch = function () {
            searchService.doSearch($scope.searchInput).then(
                function (result) {
                    $scope.searchError = null;
                    $scope.searchResult = result.searchResult;
                    $scope.searchPerformed = true;

                },
                function (error) {
                    $scope.searchError = error.data.Message;
                    $scope.searchPerformed = true;
                });
        }
    }
})();