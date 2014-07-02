define(['services/logger', 'services/dateformatter'], function (logger, dateformatter) {

    var vm = {
        requests: ko.observableArray(),
        activate: activate,
        filterText: ko.observable().extend({ rateLimit: 400 }),
        testFunc: testFunc
    };

    var serviceName = 'breeze/Breeze';

    var manager = new breeze.EntityManager(serviceName);

    function testFunc() {
        return dateformatter.test();
    }

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
    /// Query all SPT data.
    /// </summary>
    function getRequests() {
        var query = breeze.EntityQuery.
                from("OnboardingRequests");

        // Create where clause for filtering
        if (vm.filterText() && vm.filterText().length > 0) {
            var Predicate = breeze.Predicate;
            var p1 = Predicate.create("DisplayName", "substringof", vm.filterText());
            var p2 = Predicate.create("DisplayName", "contains", vm.filterText());
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

    return vm;
});