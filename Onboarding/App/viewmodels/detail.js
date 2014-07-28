define(['plugins/router', 'durandal/app', 'services/dataformatter', 'services/dataservices'],
    function (router, app, dataformatter, dataservices) {

        var vm = {
            request: ko.observable(),
            activate: activate,
            cancelRequest: cancelRequest,
            goBack: goBack,
            removeDomain: removeDomain
        };

        var serviceName = dataservices.serviceName();

        var manager = dataservices.manager();

        var hasSubmitted = false;

        // Prevent metaData not fetched exception
        var metaDataFetched = false;

        function activate(id) {
            clearInputOnloading();
            hideButtons();
            if (!manager.metadataStore.hasMetadataFor(serviceName)) {
                manager.metadataStore.fetchMetadata(serviceName, fetchMetadataSuccess, fetchMetadataSuccess)
                .then(dataservices.fetchEnum);
            }

            function fetchMetadataSuccess(rawMetadata) {
                toastr.info("Loading data on initialization...");
                metaDataFetched = true;
            }

            function fetchMetadataFail(exception) {
                toastr.error("Fetch metadata failed");
            }
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
                toastr.info('Request not canceled');
            } else {
                if (metaDataFetched && !hasSubmitted) {
                    hasSubmitted = true;
                    vm.request().State(RequestState.Canceled);
                    manager.saveChanges()
                        .then(cancelSucceeded)
                        .fail(cancelFailed);
                }
            }
        }

        function cancelSucceeded(data) {
            toastr.success("Request Canceled");
            router.navigate('#viewRequest');
        }

        function cancelFailed(error) {
            hasSubmitted = false;
            toastr.error("Request Cancel Failed");
            manager.rejectChanges();
            app.showMessage("The request could not be deleted.", "Cancel failed");
        }


        function goBack() {
            router.navigateBack();
        };

        function removeDomain(raw) {
            return dataformatter.removeDomain(raw);
        }

        /********************PRIVATE METHODS********************/
        function hideButtons() {
            $(".cancel-request-btn").hide();
        }

        function clearInputOnloading() {
            hasSubmitted = false;
        }

        function validateIdentity() {
            if (vm.request().CreatedBy() == window.currentUser) {
                if (vm.request().State() != RequestState.Canceled) {
                    $(".cancel-request-btn").show();
                }
            }
        }

        return vm;
    });