define(['plugins/router', 'durandal/app', 'services/savehelper', 'services/dataservices'],
    function (router, app, savehelper, dataservices) {

        var vm = {
            request: ko.observable(),
            activate: activate,
            cancelRequest: cancelRequest,
            goBack: goBack
        };

        var manager = dataservices.manager();

        function activate(id) {
            $(".need-auth-btns").hide();
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
                vm.request(data.results[0]);
                validateIdentity();
            }

            function queryFailed(error) {
                toastr.error("Query failed: " + error.message);
            }
        };

        /// <summary>
        /// Listener for cancel button.
        /// If confirmed, "goCancel" will be called.
        /// </summary>
        function cancelRequest() {
            app.showMessage("Are you sure you want to cancel this request?", "Cancel Request", ['Yes', 'No'])
                .then(goCancel);
        }

        /// <summary>
        /// Handle a cancel operation.
        /// </summary>
        function goCancel(data) {
            if (data == 'No') {
                toastr.info('Aborted');
            } else {
                vm.request().State("Canceled");
                manager.saveChanges()
                    .then(cancelSucceeded)
                    .fail(cancelFailed);
            }

            function cancelSucceeded(data) {
                toastr.success("Request Canceled");
                router.navigate('#request');
            }

            function cancelFailed(error) {
                toastr.error("Request Cancel Failed");
                manager.rejectChanges();
                app.showMessage("The request could not be deleted.", "Cancel failed");
            }
        }

        function goBack() {
            router.navigateBack();
        };

        /********************PRIVATE METHODS********************/
        function validateIdentity() {
            if (vm.request().CreatedBy() == savehelper.removeDomain(window.currentUser)
                && vm.request().State() != "Canceled") {
                $(".need-auth-btns").show();
            }
        }

        return vm;
    });