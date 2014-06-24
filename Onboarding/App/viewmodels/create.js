define(['plugins/router', 'durandal/app', 'services/guidgenerator'],
    function (router, app, guidgenerator) {

        var vm = {
            displayName: ko.observable(),
            appClass: ko.observable(),
            environment: ko.observable(),
            appPrincipalId: ko.observable(guidgenerator.generateGuid()),
            availableEnvironment: ko.observableArray(['Not specified', 'grn001', 'grn002', 'grnppe']),
            activate: activate,
            canDeactivate: canDeactivate,
            createEntity: createEntity,
            goBack: goBack,
            //validationErrors: ko.observableArray(),
            //getPropertyError: getPropertyError
        };

        var serviceName = 'breeze/servicePrincipalTemplate';

        var manager = new breeze.EntityManager(serviceName);

        var hasCreated = false;

        // Prevent metaData not fetched exception
        var metaDataFetched = false;

        function activate() {
            clearInputOnLoading();
            if (!manager.metadataStore.hasMetadataFor(serviceName)) {
                manager.metadataStore.fetchMetadata(serviceName, fetchMetadataSuccess, fetchMetadataSuccess)
            }

            function fetchMetadataSuccess(rawMetadata) {
                toastr.success("Fetch metadata succeed");
                metaDataFetched = true;
            }

            function fetchMetadataFail(exception) {
                toastr.error("Fetch metadata failed");
            }
        }

        /// <summary>
        /// When "create" page is activated, clear input filed. 
        /// Otherwise the input from last time is still there.
        /// </summary>
        function clearInputOnLoading() {
            hasCreated = false;
            metaDataFetched = false;
            vm.displayName("");
            vm.appClass("");
            vm.environment("Not specified");
        }

        /// <summary>
        /// Listener for create button.
        /// Create an entity.
        /// </summary>
        function createEntity() {
            if (metaDataFetched) {
                //vm.validationErrors([]);

                var newServicePrincipalTemplate = manager.
                    createEntity('ServicePrincipalTemplate:#Onboarding.Models',
                    {
                        DisplayName: vm.displayName(),
                        AppClass: vm.appClass(),
                        Environment: vm.environment(),
                        AppPrincipalID: vm.appPrincipalId()
                    });
                manager.addEntity(newServicePrincipalTemplate);
                manager.saveChanges()
                    .then(createSucceeded)
                    .fail(createFailed);

                function createSucceeded(data) {
                    hasCreated = true;
                    toastr.success("Created");
                    router.navigate('#/spt');
                }

                function createFailed(error) {
                    toastr.error("Create failed");
                    error.entitiesWithErrors.map(function (entity) {
                        toastr.info("out");
                        entity.entityAspect.getValidationErrors().map(function (validationError) {
                            vm.validationErrors.push(validationError);
                            toastr.info("in");
                        });
                    });
                    manager.rejectChanges();
                }
            }
        };

        function goBack() {
            router.navigateBack();
        };

        //function getPropertyError(propertyName) {
        //    toastr.info("1");
        //    var validationErrors = ko.utils.arrayFilter(vm.validationErrors(), function (validationError) {
        //        return validationError.propertyName == propertyName;
        //    });

        //    if (validationErrors.length > 0) {
        //        toastr.info("2");
        //        return validationErrors[0].errorMessage;
        //    }
        //    else {
        //        toastr.info("3");
        //        return '';
        //    }
        //};

        /// <summary>
        /// Pop up a window to make sure to navigate away.
        /// </summary>
        function canDeactivate() {
            if (!hasCreated) {
                return app.showMessage('Create is not finished, are you sure you want to leave this page?', 'Create Not Finished', ['Yes', 'No']);
            }
            else {
                return true;
            }
        };

        return vm;

    });