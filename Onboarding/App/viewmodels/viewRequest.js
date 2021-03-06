﻿define(['services/logger', 'services/dataservices', 'services/dataformatter'],
    function (logger, dataservices, dataformatter) {

    var vm = {
        requests: ko.observableArray(),
        activate: activate,
        filterText: ko.observable().extend({ rateLimit: 400 }),
        removeDomain: removeDomain,
        refresh: refresh
    };

    var manager = dataservices.manager();

    function activate() {
        vm.filterText.subscribe(onFilterChange);
        return getRequests();
    };

    /// <summary>
    /// Once there is text input, SPT will be quried accordingly.
    /// </summary>
    function onFilterChange() {
        if (vm.filterText().length >= 0) {
            getRequests();
        }
    }

    /// <summary>
    /// Query requests data.
    /// </summary>
    function getRequests() {
        var query = breeze.EntityQuery.
            from("OnboardingRequests");

        // Create where clause for filtering
        if (vm.filterText() && vm.filterText().length > 0) {
            var predicate = breeze.Predicate;
            var p1 = predicate.create("RequestSubject", "substringof", vm.filterText());
            var p2 = predicate.create("RequestSubject", "contains", vm.filterText());
            var whereClause = p1.or(p2);

            query = query.where(whereClause);
        }

        return manager
            .executeQuery(query)
            .then(querySucceeded)
            .fail(queryFailed);

        function querySucceeded(data) {
            vm.requests(data.results);
        }

        function queryFailed(error) {
            toastr.error("Query failed: " + error.message);
        }
    };

    function removeDomain(raw) {
        return dataformatter.removeDomain(raw);
    };

    function refresh() {
        getRequests();
        toastr.success("Requests updated");
    };

    return vm;
});