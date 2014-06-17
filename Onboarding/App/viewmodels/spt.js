define(['services/logger'], function (logger) {

    var vm = {
        spts: ko.observableArray(),
        activate: activate,
        filterText: ko.observable().extend({ rateLimit: 400 })
    };

    var serviceName = 'breeze/servicePrincipalTemplate';

    var manager = new breeze.EntityManager(serviceName);

    function activate() {
        vm.filterText.subscribe(onFilterChange);
        return getSpts();
    };

    function onFilterChange() {
        if (vm.filterText().length >= 0) {
            getSpts();
        }
    }

    function getSpts() {
        toastr.info("Querying...");
        var query = breeze.EntityQuery.
                from("ServicePrincipalTemplates");

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
            vm.spts(data.results);
            toastr.success("Spts loaded!");
        }

        function queryFailed(error) {
            toastr.error("Query failed: " + error.message);
        }
    };

    return vm;
});