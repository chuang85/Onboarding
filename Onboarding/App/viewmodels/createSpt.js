define(['plugins/router', 'durandal/app', 'services/guidgenerator', 'services/dataformatter'],
    function (router, app, guidgenerator, dataformatter) {

        var vm = {
            displayName: ko.observable(),
            appClass: ko.observable(),
            environment: ko.observable(),
            appPrincipalId: ko.observable(guidgenerator.generateGuid()),
            availableEnvironment: ko.observableArray(['grn001', 'grn002', 'grnppe', 'default']),
            activate: activate,
            canDeactivate: canDeactivate,
            createEntity: createEntity,
            goBack: goBack,
            //validationErrors: ko.observableArray(),
            //getPropertyError: getPropertyError,
            addEnvironment: addEnvironment
        };

        var serviceName = 'breeze/servicePrincipalTemplate';

        var manager = new breeze.EntityManager(serviceName);

        var hasCreated = false;

        // Prevent metaData not fetched exception
        var metaDataFetched = false;

        function activate() {
            /********TEST********/
            var list = [];
            $("#test-each-area").each(function () {
                list.push($("p").innerHTML);
            });
            console.log("start");
            console.log(list.toString());
            console.log("end");

            clearInputOnLoading();
            if (!manager.metadataStore.hasMetadataFor(serviceName)) {
                manager.metadataStore.fetchMetadata(serviceName, fetchMetadataSuccess, fetchMetadataSuccess)
            }

            function fetchMetadataSuccess(rawMetadata) {
                toastr.success("Fetch metadata succeed");
                metaDataFetched = true;
                // Enable "create" button when metadata has been fetched
                enableButton();
            }

            function fetchMetadataFail(exception) {
                toastr.error("Fetch metadata failed");
            }
        }

        /// <summary>
        /// When "create" page is activated, clear input filled. 
        /// Otherwise the input from last time is still there.
        /// </summary>
        function clearInputOnLoading() {
            hasCreated = false;
            vm.displayName("");
            vm.appClass("");
        }

        /// <summary>
        /// Listener for create button.
        /// Create an entity.
        /// </summary>
        function createEntity() {
            if (metaDataFetched) {
                //vm.validationErrors([]);


                console.log(dataformatter.json2xml(createJSONSpt()));

                // Disable "create" button after hit
                // Prevent multiple submits
                disableButton();

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
                    enableButton();
                    router.navigate('#/request');
                }

                function createFailed(error) {
                    toastr.error("Create failed");
                    enableButton();
                    //error.entitiesWithErrors.map(function (entity) {
                    //    toastr.info("out");
                    //    entity.entityAspect.getValidationErrors().map(function (validationError) {
                    //        vm.validationErrors.push(validationError);
                    //        toastr.info("in");
                    //    });
                    //});
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

        function enableButton() {
            $("#create-btn").attr("disabled", false);
        }

        function disableButton() {
            $("#create-btn").attr("disabled", false);
        }

        function addEnvironment() {
            $("#env-group").append("<h1>TEST</h1>");
        }

        function createJSONSpt() {
            var json = {};
            json["ServicePrincipalTemplate"] = {};
            json["ServicePrincipalTemplate"]["Value"] = {};
            json["ServicePrincipalTemplate"]["Value"]["ServicePrincipals"] = {};
            json["ServicePrincipalTemplate"]["Value"]["ServicePrincipals"]["@xmlns"] = "";

            var body = json["ServicePrincipalTemplate"]["Value"]["ServicePrincipals"]["ServicePrincipal"] = {};
            body["DisplayName"] = vm.displayName();
            body["AppClass"] = vm.appClass();            

            /********HARD CODING********/
            // Environments
            body["Environments"] = {};
            var envArr = body["Environments"]["Environment"] = [];
            var env = {};
            env["@name"] = "grn001";
            // Hostnames
            env["Hostnames"] = {};
            var hostnameArr = env["Hostnames"]["Hostname"] = [];
            hostnameArr.push("portal.partner.microsoftonline.cn");
            hostnameArr.push("portal.bosint.bosxlab.com");
            // AdditionalSPNames
            env["AdditionalServicePrincipalNames"] = {};
            var additionalSPNameArr = env["AdditionalServicePrincipalNames"]["ServicePrincipalName"] = [];
            additionalSPNameArr.push("https://odc.officeapps.live.com");
            additionalSPNameArr.push("https://roaming.officeapps.live.com");
            // AppAddresses
            env["AppAddresses"] = {};
            var appAddressArr = env["AppAddresses"]["AppAddress"] = [];
            var appAddress = {};
            appAddress["@Address"] = "https://account.activedirectory.windowsazure.com";
            appAddress["@AddressType"] = "Reply";        
            appAddressArr.push(appAddress);
            appAddress = {};
            appAddress["@Address"] = "https://account.activedirectory-ppe.windowsazure.com";
            appAddress["@AddressType"] = "Reply";
            appAddressArr.push(appAddress);
            envArr.push(env);

            body["AppPrincipalID"] = vm.appPrincipalId();

            return json;
        };

        return vm;

    });