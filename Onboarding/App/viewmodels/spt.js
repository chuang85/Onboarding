define(['services/logger'], function (logger) {

    var vm = {
        servicePrincipalTemplates: ko.observableArray(),
        activate: activate
    };

    var serviceName = 'breeze/servicePrincipalTemplate';

    var manager = new breeze.EntityManager(serviceName);

    function activate() {
        return getServicePrincipalTemplates();
    };

    function getServicePrincipalTemplates() {
        toastr.info("Querying...");
        var query = breeze.EntityQuery.from("ServicePrincipalTemplates");

        return manager
            .executeQuery(query)
            .then(querySucceeded)
            .fail(queryFailed);

        function querySucceeded(data) {
            vm.servicePrincipalTemplates(data.results);
            toastr.success("Spts loaded!");
        }

        function queryFailed(error) {
            toastr.error("Query failed: " + error.message);
        }
    };

    return vm;
});