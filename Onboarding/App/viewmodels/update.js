/// <reference path="../../Scripts/breeze.intellisense.js" />
define(['plugins/router'], function (router) {
    var vm = {
        editableSpt: ko.observable(),
        activate: activate,
        saveChanges: saveChanges,
        goBack: goBack
    };

    var serviceName = 'breeze/servicePrincipalTemplate';

    var manager = new breeze.EntityManager(serviceName);

    function activate(id) {
        return getSpt(id);
    }

    function getSpt(id) {
        var query = breeze.EntityQuery.
                from("ServicePrincipalTemplates").
                where("Id", "==", id);

        return manager
            .executeQuery(query)
            .then(querySucceeded)
            .fail(queryFailed);


        function querySucceeded(data) {
            vm.editableSpt(data.results[0]);
        }

        function queryFailed(error) {
            toastr.error("Query failed: " + error.message);
        }
    };

    function saveChanges() {
        if (manager.hasChanges()) {
            manager.saveChanges()
                .then(saveSucceeded)
                .fail(saveFailed);
        } else {
            toastr.info("Nothing to save");
        };


        function saveSucceeded(data) {
            toastr.success("Saved");
        }

        function saveFailed(error) {
            toastr.error("Save failed");
        }
    };

    function goBack() {
        router.navigateBack();
    }

    return vm;
});