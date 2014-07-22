define(['plugins/router', 'durandal/app', 'services/savehelper'],
    function (router, app, savehelper) {

        var vm = {
            request: ko.observable(),
            activate: activate,
            deleteSpt: deleteSpt,
            goBack: goBack
        };

        var serviceName = 'breeze/Breeze';

        var manager = new breeze.EntityManager(serviceName);

        var currUser = '';

        function activate(id) {
            getRequest(id);
            //validateIdentity();
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
                vm.request(data.results[0]);
                validateIdentity();
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
            app.showMessage("Are you sure you want to cancel this request?", "Cancel Request", ['Yes', 'No'])
                .then(goCancel);
        }

        /// <summary>
        /// Handle a delete operation.
        /// </summary>
        function goCancel(data) {
            if (data == 'No') {
                toastr.info('Aborted');
            } else {
                vm.request().entityAspect.setDeleted();

                manager.saveChanges()
                    .then(deleteSucceeded)
                    .fail(deleteFailed);
            }


            function deleteSucceeded(data) {
                toastr.success("Deleted");
                router.navigate('#request');
            }

            function deleteFailed(error) {
                toastr.error("Delete failed");
                manager.rejectChanges();
                app.showMessage("The SPT could not be deleted.", "Delete failed");
            }
        }

        function goBack() {
            router.navigateBack();
        };

        /********************PRIVATE METHODS********************/
        function validateIdentity() {
            if (vm.request().CreatedBy() == savehelper.removeDomain(window.currentUser)) {
                $(".need-auth-btns").show();
            }
        }

        return vm;
    });