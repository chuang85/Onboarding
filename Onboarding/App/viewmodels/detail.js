define(['plugins/router', 'durandal/app'], function (router, app) {
    var vm = {
        spt: ko.observable(),
        activate: activate,
        deleteSpt: deleteSpt,
        goBack: goBack
    };

    var serviceName = 'breeze/servicePrincipalTemplate';

    var manager = new breeze.EntityManager(serviceName);

    function activate(context) {
        return getSpt(context.id);
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
            vm.spt(data.results[0]);
        }

        function queryFailed(error) {
            toastr.error("Query failed: " + error.message);
        }
    };

    function deleteSpt() {
        app.showMessage("Are you sure you want to delete this SPT?", "Delete SPT", ['Yes', 'No'])
            .then(goDelete);
    }

    function goDelete(data) {
        if (data == 'No') {
            toastr.info('Aborted');
        }
        else {
            vm.servicePrincipalTemplates().entityAspect.setDeleted();

            manager.saveChanges()
                .then(deleteSucceeded)
                .fail(deleteFailed);
        }


        function deleteSucceeded(data) {
            toastr.success("Deleted");
            router.navigate('#/spt');
        }

        function deleteFailed(error) {
            toastr.error("Delete failed");
            manager.rejectChanges();
            app.showMessage("The SPT could not be deleted.", "Delete failed");
        }
    }

    function goBack() {
        router.navigateBack();
    }

    return vm;
});