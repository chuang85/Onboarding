define(['plugins/router', 'durandal/app', 'services/guidgenerator', 'services/dataformatter'],
    function(router, app, guidgenerator, dataformatter) {

        var vm = {
            contact: ko.observable(),
            displayName: ko.observable(),
            serviceType: ko.observable(),
            appPrincipalId: ko.observable(),
            constrainedDelegationTo: ko.observable(),
            externalUserAccountDelegationsAllowed: ko.observable(),
            microsoftPolicyGroup: ko.observable(),
            managedExternally: ko.observable(),
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

        var serviceName = 'breeze/Breeze';

        var manager = new breeze.EntityManager(serviceName);

        var hasCreated = false;

        // Prevent metaData not fetched exception
        var metaDataFetched = false;

        function activate() {
            clearInputOnloading();
            collapsePanels();
            generateAppId();

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
                var xmlString = dataformatter.formatXml(dataformatter.json2xml(createJSONSpt()));
                console.log(xmlString);

                var newOnboardingRequest = manager.
                    createEntity('OnboardingRequest:#Onboarding.Models',
                    {
                        CreatedBy: vm.contact(),
                        DisplayName: vm.displayName(),
                        TempXmlStore: xmlString,
                        //State: RequestState.Created,
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
                .then(function(dialogResult) {
                    if (dialogResult == 'Yes') {
                        clearInputOnloading();
                        collapsePanels();
                    }
                });
        }

        function goBack() {
            router.navigateBack();
        };

        /********************PRIVATE METHODS********************/
        function generateAppId() {
            vm.appPrincipalId(guidgenerator.generateGuid());
        }

        function clearInputOnloading() {
            hasCreated = false;
            hasSubmitted = false;
            vm.displayName("");
            vm.serviceType("");
            $(":input").not("#appPrincipalId").val("");
        }

        function collapsePanels() {
            $('.fieldwrapper').remove();
            $('.panel-collapse').removeClass('in');
        }

        function addItem(envType, itemType) {
            var fieldWrapper = $("<div class=\"fieldwrapper\" />");
            var inputField = $("<input class=\"form-control\" />");
            var removeButton = $("<span class=\"pull-right pointerLink glyphicon glyphicon-trash\"></span>");

            removeButton.click(function() {
                $(this).parent().remove();
            });

            fieldWrapper.append(inputField);
            fieldWrapper.append(removeButton);
            $("." + envType + "-" + itemType + "-section").append(fieldWrapper);
        }

        function createJSONSpt() {
            var envMap = {
                "prod": "grn001",
                "gallatin": "grn002",
                "ppe": "grnppe",
                "default": "*"
            };

            var json = {};
            // DirectoryChanges
            json["DirectoryChanges"] = {};
            json["DirectoryChanges"]["@xmlns:xsi"] = "http://www.w3.org/2001/XMLSchema-instance";
            json["DirectoryChanges"]["@xmlns"] = "http://schemas.microsoft.com/online/directoryservices/change/2008/11";

            // DirectoryObject
            var body = json["DirectoryChanges"]["DirectoryObject"] = {};
            body["@xsi:type"] = "Service";
            body["@ContextId"] = "00000000-0000-0000-0000-000000000000";
            body["@ObjectId"] = guidgenerator.generateGuid().toString();

            // ServiceInstanceMap
            body["ServiceInstanceMap"] = {};
            body["ServiceInstanceMap"]["Value"] = {};
            body["ServiceInstanceMap"]["Value"]["Maps"] = {};
            var maps = body["ServiceInstanceMap"]["Value"]["Maps"]["Map"] = [];
            var map = {};
            map["WeightedServiceInstances"] = {};
            var wsis = map["WeightedServiceInstances"]["WeightedServiceInstance"] = [];
            var wsi = {};
            wsi["@Name"] = vm.serviceType() + "/SDF";
            wsi["@Weight"] = "1";
            wsis.push(wsi);
            maps.push(map);

            // ServiceType
            body["ServiceType"] = {};
            body["ServiceType"]["Value"] = vm.serviceType();

            // ServicePrincipalTemplate
            body["ServicePrincipalTemplate"] = {};
            body["ServicePrincipalTemplate"]["Value"] = {};
            body["ServicePrincipalTemplate"]["Value"]["ServicePrincipals"] = {};
            body["ServicePrincipalTemplate"]["Value"]["ServicePrincipals"]["@xmlns"] = "";

            var sp = body["ServicePrincipalTemplate"]["Value"]["ServicePrincipals"]["ServicePrincipal"] = {};
            // DisplayName
            sp["DisplayName"] = vm.displayName();

            // AppClass
            sp["AppClass"] = vm.serviceType();

            // Environments
            sp["Environments"] = {};
            var envArr = sp["Environments"]["Environment"] = [];

            $("#environment .env-title a").each(function() {
                var env = {};
                var envType = $(this).text().toLowerCase();
                env["@name"] = envMap[envType];

                // Init flags to prevent adding emtpy array
                var hostnameEmpty = spnameEmpty = appaddressEmpty = true;

                // Hostnames
                var hostnameArr = [];
                $("." + envType + "-hostname-section input").each(function() {
                    var value = $(this).val();
                    if (value != "") {
                        hostnameArr.push(value);
                        hostnameEmpty = false;
                    }
                });
                if (!hostnameEmpty) {
                    env["Hostnames"] = {};
                    env["Hostnames"]["Hostname"] = hostnameArr;
                }

                // AdditionalSPNames
                var additionalSPNameArr = [];
                $("." + envType + "-spname-section input").each(function() {
                    var value = $(this).val();
                    if (value != "") {
                        additionalSPNameArr.push(value);
                        spnameEmpty = false;
                    }
                });
                if (!spnameEmpty) {
                    env["AdditionalServicePrincipalNames"] = {};
                    env["AdditionalServicePrincipalNames"]["ServicePrincipalName"] = additionalSPNameArr;
                }

                // AppAddresses
                var appAddressArr = [];
                $("." + envType + "-appaddress-section input").each(function() {
                    var value = $(this).val();
                    if (value != "") {
                        var appAddress = {};
                        appAddress["@Address"] = value;
                        appAddress["@AddressType"] = "Reply";
                        appAddressArr.push(appAddress);
                        appaddressEmpty = false;
                    }
                });
                if (!appaddressEmpty) {
                    env["AppAddresses"] = {};
                    env["AppAddresses"]["AppAddress"] = appAddressArr;
                }

                // Add environments if arrays are not all empty
                if (!(hostnameEmpty && spnameEmpty && appaddressEmpty)) {
                    envArr.push(env);
                }
            });

            // AppPrincipalID
            sp["AppPrincipalID"] = vm.appPrincipalId();

            // ExternalUserAccountDelegationsAllowed
            sp["ExternalUserAccountDelegationsAllowed"] = "";

            // KeyGroupID
            sp["KeyGroupID"] = guidgenerator.generateGuid().toString();

            // MicrosoftPolicyGroup
            sp["MicrosoftPolicyGroup"] = "";

            // ManagedExternally
            sp["ManagedExternally"] = "";

            return json;
        };

        return vm;

    });