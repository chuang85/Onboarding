define(['plugins/router', 'services/dataservices'],
    function (router, dataservices) {
    var vm = {
        editableRequest: ko.observable(),
        availableEnvironment: ko.observableArray(['Not specified', 'grn001', 'grn002', 'grnppe']),
        activate: activate,
        saveChanges: saveChanges,
        goBack: goBack
    };

    var manager = dataservices.manager();

    function activate(id) {
        return getRequest(id);
    }

    /// <summary>
    /// Get a specific SPT given its id.
    /// </summary>
    /// <param name="id">The id of SPT to be queried</param>
    function getRequest(id) {
        var query = breeze.EntityQuery.
            from("OnboardingRequests").
            where("RequestId", "==", id);

        return manager
            .executeQuery(query)
            .then(querySucceeded)
            .fail(queryFailed);


        function querySucceeded(data) {
            vm.editableRequest(data.results[0]);
        }

        function queryFailed(error) {
            toastr.error("Query failed: " + error.message);
        }
    };

    /// <summary>
    /// Listener for update button.
    /// Make change to DB.
    /// </summary>
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