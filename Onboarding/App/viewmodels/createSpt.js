define(['plugins/router', 'durandal/app', 'services/guidgenerator', 'services/dataformatter', 'services/dataservices', 'services/jsonbuilder', 'services/dbhelper'],
    function (router, app, guidgenerator, dataformatter, dataservices, jsonbuilder, dbhelper) {

        var vm = {
            contact: ko.observable(window.currentUser),
            requestSubject: ko.observable(),
            displayName: ko.observable(),
            serviceType: ko.observable(),
            appPrincipalId: ko.observable(),
            serviceTypeList: ko.observableArray(),
            constrainedDelegationTo: ko.observableArray(),
            externalUserAccountDelegationsAllowed: ko.observable(),
            microsoftPolicyGroup: ko.observable(),
            managedExternally: ko.observable(),
            optionsValue: ko.observable(),
            taskSetList: ko.observableArray(),
            permissions: ko.observableArray(),
            // Set request type by default when navigating to this page
            requestType: "CreateSPT",
            activate: activate,
            canDeactivate: canDeactivate,
            createEntity: createEntity,
            clearInput: clearInput,
            goBack: goBack,
            addItem: addItem,
        };

        var hasSubmitted = false;

        var serviceName = dataservices.serviceName();

        var manager = dataservices.manager();

        var hasCreated = false;

        // Prevent metaData not fetched exception
        var metaDataFetched = false;

        function activate() {
            clearInputOnloading();
            collapsePanels();
            generateAppId();
            dbhelper.getServiceTypes(vm);
            dbhelper.getTaskSets(vm);

            if (!manager.metadataStore.hasMetadataFor(serviceName)) {
                manager.metadataStore.fetchMetadata(serviceName, fetchMetadataSuccess, fetchMetadataSuccess);
                //.then(function (data) {
                //    console.log(data.schema.enumType);
                //    // extract all enums as global objects
                //    console.log("out");
                //    ko.utils.arrayForEach(data.schema.enumType, function (c) {
                //        console.log("mid");
                //        window[c.name] = {};
                //        ko.utils.arrayForEach(c.member, function (m) {
                //            console.log("in");
                //            window[c.name][m.name] = m.value;
                //            console.log(c.name);
                //            console.log(m.name);
                //        });
                //    });
                //    console.log("start");
                //    console.log(window);
                //    console.log("end");
                //});
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
        /// Pop up a window to make sure to navigate away.
        /// </summary>
        function canDeactivate() {
            if (!hasCreated) {
                return app.showMessage('Create is not finished, are you sure you want to leave this page?', 'Create Not Finished', ['Yes', 'No']);
            } else {
                return true;
            }
        };

        /// <summary>
        /// Listener for create button.
        /// Create an entity.
        /// </summary>
        function createEntity() {
            if (metaDataFetched && !hasSubmitted) {
                hasSubmitted = true;
                var xmlString = dataformatter.removeUndefined(dataformatter.formatXml(dataformatter.json2xml(jsonbuilder.createJSONSpt(vm))));

                var newOnboardingRequest = manager.
                    createEntity('OnboardingRequest:#Onboarding.Models',
                    {
                        CreatedBy: vm.contact(),
                        RequestSubject: vm.requestSubject(),
                        TempXmlStore: xmlString,
                        Type: vm.requestType
                    });
                manager.addEntity(newOnboardingRequest);
                manager.saveChanges()
                    .then(createSucceeded)
                    .fail(createFailed);

                function createSucceeded(data) {
                    hasCreated = true;
                    toastr.success("Created");
                    router.navigate('#/request');
                }

                function createFailed(error) {
                    hasSubmitted = false;
                    toastr.error("Create failed");
                    console.log(error);
                    manager.rejectChanges();
                }
            }
        };

        /// <summary>
        /// Listener for clear button, clear input filled EXCEPT appId
        /// And close all the panels and added items
        /// </summary>
        function clearInput() {
            app.showMessage('All the input fields will be cleaned, continue?', 'Clear All', ['Yes', 'No'])
                .then(function (dialogResult) {
                    if (dialogResult == 'Yes') {
                        clearInputOnloading();
                        collapsePanels();
                    }
                });
        }

        function goBack() {
            router.navigateBack();
        };

        function addItem(envType, itemType) {
            var fieldWrapper = $("<div class=\"fieldwrapper\" />");
            var inputField = $("<input class=\"form-control\" />");
            var removeButton = $("<span class=\"pull-right pointerLink glyphicon glyphicon-trash\"></span>");

            removeButton.click(function () {
                $(this).parent().remove();
            });

            fieldWrapper.append(inputField);
            fieldWrapper.append(removeButton);
            $("." + envType + "-" + itemType + "-section").append(fieldWrapper);
        }

        /********************PRIVATE METHODS********************/
        function generateAppId() {
            vm.appPrincipalId(guidgenerator.generateGuid());
        }

        function clearInputOnloading() {
            hasCreated = false;
            hasSubmitted = false;
            vm.requestSubject("");
            vm.displayName("");
            vm.serviceType("");
            vm.externalUserAccountDelegationsAllowed("");
            vm.microsoftPolicyGroup("");
            vm.managedExternally("");
            $("input[type=radio]").attr("checked", false);
            $(":input").not("#appPrincipalId, #contact").val("");
        }

        function collapsePanels() {
            $('.fieldwrapper').remove();
            $('.panel-collapse').removeClass('in');
        }

        return vm;

    });