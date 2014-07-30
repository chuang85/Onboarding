﻿define(['plugins/router', 'durandal/app', 'services/dataservices', 'services/dbhelper'],
    function (router, app, dataservices, dbhelper) {

        var vm = {
            /* Request data */
            contact: ko.observable(window.currentUser),
            requestSubject: ko.observable(),
            requestType: ko.observable(),
            
            /* SPT data */
            sptList: ko.observableArray([]),
            chosenSptName: ko.observable(),

            /* public functions */
            activate: activate,
            canDeactivate: canDeactivate,
            search: search,
            createEntity: createEntity,

            /* descripstions */
            descContact: ko.observable(),
            descRequestSubject: ko.observable(),
            descDisplayName: ko.observable(),
            descServiceType: ko.observable(),
            descAppPrincipalId: ko.observable(),
            descEnvironment: ko.observable(),
        };

        var serviceName = dataservices.serviceName();

        var manager = dataservices.manager();

        var hasSubmitted = false;

        var hasCreated = false;

        // Prevent metaData not fetched exception
        var metaDataFetched = false;

        function activate() {
            clearInputOnloading();
            if (!manager.metadataStore.hasMetadataFor(serviceName)) {
                loadDataFromDb();
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
        }

        function canDeactivate() {
            if (!hasCreated) {
                return app.showMessage('Add certificate is not finished, are you sure you want to leave this page?', 'Add Certificate Not Finished', ['Yes', 'No']);
            } else {
                return true;
            }
        };

        function createEntity() {
            if (metaDataFetched && !hasSubmitted) {
                hasSubmitted = true;
                determinRequestType();

                var newOnboardingRequest = manager.
                    createEntity('OnboardingRequest:#Onboarding.Models',
                    {
                        CreatedBy: vm.contact(),
                        RequestSubject: vm.requestSubject(),
                        Type: vm.requestType(),
                        State: RequestState.Created
                    });
                manager.addEntity(newOnboardingRequest);
                manager.saveChanges()
                    .then(createSucceeded)
                    .fail(createFailed);

                function createSucceeded(data) {
                    hasCreated = true;
                    toastr.success("Created");
                    router.navigate('#/viewRequest');
                }

                function createFailed(error) {
                    hasSubmitted = false;
                    toastr.error("Create failed");
                    console.log(error);
                    manager.rejectChanges();
                }
            }
        }

        function search() {
            if (metaDataFetched) {
                dbhelper.getSptByName(vm);
            }
        }

        /********************PRIVATE METHODS********************/
        function clearInputOnloading() {
            hasCreated = false;
            hasSubmitted = false;
        }

        function loadDataFromDb() {
            dbhelper.getExistingSptsNames(vm);
            dbhelper.getDescriptions(vm);
        }

        function determinRequestType() {
            // TODO: Modify this
            vm.requestType(RequestType.AddCertToKeyGroup);
        }

        return vm;
    });