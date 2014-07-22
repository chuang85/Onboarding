define(['plugins/router', 'durandal/app', 'services/guidgenerator', 'services/dataformatter', 'services/savehelper', 'services/dataservices'],
    function (router, app, guidgenerator, dataformatter, savehelper, dataservices) {

        var vm = {
            contact: ko.observable(window.currentUser),
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
            getServiceTypes();
            getTaskSets();

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
                        CreatedBy: savehelper.removeDomain(vm.contact()),
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

        /********************PRIVATE METHODS********************/
        function getServiceTypes() {
            var query = breeze.EntityQuery
                .from("ServiceTypes")
                .select("ServiceTypeName")
                .orderBy("ServiceTypeName");

            return manager
            .executeQuery(query)
            .then(querySucceeded)
            .fail(queryFailed);

            function querySucceeded(data) {
                for (var i = 0; i < data.results.length; i++) {
                    vm.serviceTypeList.push(data.results[i]["ServiceTypeName"]);
                }
            }

            function queryFailed(error) {
                toastr.error("Query failed: " + error.message);
            }
        }

        function getTaskSets() {
            var query = breeze.EntityQuery
                .from("TaskSets")
                .select("TaskSetName")
                .orderBy("TaskSetName");

            return manager
            .executeQuery(query)
            .then(querySucceeded)
            .fail(queryFailed);

            function querySucceeded(data) {
                for (var i = 0; i < data.results.length; i++) {
                    vm.taskSetList.push(data.results[i]["TaskSetName"]);
                }
            }

            function queryFailed(error) {
                toastr.error("Query failed: " + error.message);
            }
        }

        function generateAppId() {
            vm.appPrincipalId(guidgenerator.generateGuid());
        }

        function clearInputOnloading() {
            hasCreated = false;
            hasSubmitted = false;
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

            // ConstrainedDelegationTo
            sp["ConstrainedDelegationTo"] = generateDelegationStr(vm.constrainedDelegationTo());

            // Environments
            sp["Environments"] = {};
            var envArr = sp["Environments"]["Environment"] = [];

            $("#environment .env-title a").each(function () {
                var env = {};
                var envType = $(this).text().toLowerCase();
                env["@name"] = envMap[envType];

                // Init flags to prevent adding emtpy array
                var hostnameEmpty = spnameEmpty = appaddressEmpty = true;

                // Hostnames
                var hostnameArr = [];
                $("." + envType + "-hostname-section input").each(function () {
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
                $("." + envType + "-spname-section input").each(function () {
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
                $("." + envType + "-appaddress-section input").each(function () {
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
            sp["ExternalUserAccountDelegationsAllowed"] = vm.externalUserAccountDelegationsAllowed();

            // KeyGroupID
            sp["KeyGroupID"] = guidgenerator.generateGuid().toString();

            // MicrosoftPolicyGroup
            sp["MicrosoftPolicyGroup"] = vm.microsoftPolicyGroup();

            // ManagedExternally
            sp["ManagedExternally"] = vm.managedExternally();

            return json;
        };

        function generateDelegationStr(sourceArray) {
            var delegationStr = "";
            if (sourceArray) {
                for (var i = 0; i < sourceArray.length - 1; i++) {
                    delegationStr += sourceArray[i];
                    delegationStr += ", ";
                }
                delegationStr += sourceArray[sourceArray.length - 1];
            }
            return delegationStr;
        }

        return vm;

    });