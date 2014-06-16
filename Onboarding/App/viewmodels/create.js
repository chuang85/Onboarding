define(['plugins/router'],
    function (router) {

        var vm = {
            displayName: ko.observable(),
            environment: ko.observable(),
            availableEnvironment: ko.observableArray(['Not specified', 'grn001', 'grn002', 'grnppe']),
            activate: activate,
            //canDeactivate: canDeactivate,
            createEntity: createEntity,
            goBack: goBack,
            //validationErrors: ko.observableArray([]),
            //getPropertyError: getPropertyError
        };

        var serviceName = 'breeze/servicePrincipalTemplate';

        var manager = new breeze.EntityManager(serviceName);

        //var sptType = manager.metadataStore.getEntityType('ServicePrincipalTemplate');
        //var urlProperty = sptType.getProperty('displayName');
        

        function activate() {
            if (!manager.metadataStore.hasMetadataFor(serviceName)) {
                manager.metadataStore.fetchMetadata(serviceName, fetchMetadataSuccess, fetchMetadataSuccess)
            }

            function fetchMetadataSuccess(rawMetadata) {
            }

            function fetchMetadataFail(exception) {
            }
        }

        function createEntity() {
            //vm.validationErrors([]);

            //var validators = urlProperty.validators;
            //validators.push(Validitor.required());
            //validators.push(Validitor.url());

            var newServicePrincipalTemplate = manager.
                createEntity('ServicePrincipalTemplate:#Onboarding.Models',
                { DisplayName: vm.displayName(), Environment: vm.environment() });
            manager.addEntity(newServicePrincipalTemplate);
            manager.saveChanges()
                .then(createSucceeded)
                .fail(createFailed);


            function createSucceeded(data) {
                toastr.success("Created");
                router.navigate('#/spt');
            }

            function createFailed(error) {
                toastr.error("Create failed");
                //error.entitiesWithErrors.map(function (entity) {
                //    entity.entityAspect.getValidationErrors().map(function (validationError) {
                //        vm.validationErrors.push(validationError);
                //    });
                //});
                manager.rejectChanges();
            }
        };

        function goBack() {
            router.navigateBack();
        };

        //function getPropertyError(propertyName) {
        //    var validationErrors = ko.utils.arrayFilter(vm.validationErrors(), function (validationError) {
        //        return validationError.propertyName == propertyName;
        //    });

        //    if (validationErrors.length > 0)
        //        return validationErrors[0].errorMessage;
        //    else
        //        return '';
        //};

        function canDeactivate() {
            //if (this._sptToAdd == false) {
            //    return app.showMessage('Are you sure you want to leave this page?', 'Navigate', ['Yes', 'No']);
            //} else {
            //    return true;
            //}
        };

        return vm;

    });