define(['plugins/router', 'durandal/app'],
    function (router, app) {

        var vm = {
            certSpt: ko.observable(),
            availableEnvironment: ko.observableArray(['Not specified', 'grn001', 'grn002', 'grnppe']),
            activate: activate,
            saveChanges: saveChanges,
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
                vm.certSpt(data.results[0]);
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