define(['plugins/router', 'durandal/app'], function(router, app) {

    var vm = {
        spt: ko.observable(),
        activate: activate,
        deleteSpt: deleteSpt,
        goBack: goBack
    };

    var serviceName = 'breeze/Breeze';

    var manager = new breeze.EntityManager(serviceName);

    function activate(id) {
        return getSpt(id);
    }

    /// <summary>
    /// Get a specific SPT given its id.
    /// </summary>
    /// <param name="id">The id of SPT to be queried</param>
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

    /// <summary>
    /// Listener for delete button.
    /// If confirmed, "goDelete" will be called.
    /// </summary>
    function deleteSpt() {
        app.showMessage("Are you sure you want to delete this SPT?", "Delete SPT", ['Yes', 'No'])
            .then(goDelete);
    }

    /// <summary>
    /// Handle a delete operation.
    /// </summary>
    function goDelete(data) {
        if (data == 'No') {
            toastr.info('Aborted');
        } else {
            vm.spt().entityAspect.setDeleted();

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