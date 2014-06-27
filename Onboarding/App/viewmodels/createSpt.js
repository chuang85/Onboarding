define(['plugins/router', 'durandal/app', 'services/guidgenerator', 'services/dataformatter'],
    function (router, app, guidgenerator, dataformatter) {

        var vm = {
            displayName: ko.observable(),
            appClass: ko.observable(),
            appPrincipalId: ko.observable(),
            activate: activate,
            canDeactivate: canDeactivate,
            createEntity: createEntity,
            clearInput: clearInput,
            goBack: goBack,
            addItem: addItem,
        };

        var serviceName = 'breeze/Breeze';

        var manager = new breeze.EntityManager(serviceName);

        var hasCreated = false;

        // Prevent metaData not fetched exception
        var metaDataFetched = false;

        function activate() {
            clearInput();
            // Generate guid every time loaded
            vm.appPrincipalId(guidgenerator.generateGuid());
            if (!manager.metadataStore.hasMetadataFor(serviceName)) {
                manager.metadataStore.fetchMetadata(serviceName, fetchMetadataSuccess, fetchMetadataSuccess);
                //.then(function (data) {
                //    console.log(data);
                //    // extract all enums as global objects
                //    ko.utils.arrayForEach(data.schema.enumType, function (c) {
                //        window[c.name] = {};
                //        ko.utils.arrayForEach(c.member, function (m) {
                //            window[c.name][m.name] = m.value;
                //        });
                //    });
                //    console.log("start");
                //    console.log(window);
                //    console.log("end");
                //});
            } else {
                enableButton();
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

        /// <summary>
        /// Listener for create button.
        /// Create an entity.
        /// </summary>
        function createEntity() {
            if (metaDataFetched) {
                console.log(dataformatter.json2xml(createJSONSpt()));

                // Disable "create" button after hit
                // Prevent multiple submits
                disableButton();

                var newServicePrincipalTemplate = manager.
                    //createEntity('ServicePrincipalTemplate:#Onboarding.Models',
                    createEntity('OnboardingRequest:#Onboarding.Models',
                    {
                        DisplayName: vm.displayName(),
                        //AppClass: vm.appClass(),
                        //AppPrincipalID: vm.appPrincipalId()
                        //RequestState: RequestState.Created
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
                    manager.rejectChanges();
                }
            }
        };

        /// <summary>
        /// When "create" page is activated, clear input filled. 
        /// Otherwise the input from last time is still there.
        /// </summary>
        function clearInput() {
            hasCreated = false;
            vm.displayName("");
            vm.appClass("");
            $(":input").not("#appPrincipalId").val("");
        }

        function goBack() {
            router.navigateBack();
        };

        /********************PRIVATE METHODS********************/
        function enableButton() {
            $("#submit-btn").attr("disabled", false);
        }

        function disableButton() {
            $("#submit-btn").attr("disabled", false);
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
            var map = {
                "prod": "grn001",
                "gallatin": "grn002",
                "ppe": "grnppe",
                "default": "*"
            };

            var json = {};
            json["ServicePrincipalTemplate"] = {};
            json["ServicePrincipalTemplate"]["Value"] = {};
            json["ServicePrincipalTemplate"]["Value"]["ServicePrincipals"] = {};
            json["ServicePrincipalTemplate"]["Value"]["ServicePrincipals"]["@xmlns"] = "";

            var body = json["ServicePrincipalTemplate"]["Value"]["ServicePrincipals"]["ServicePrincipal"] = {};
            body["DisplayName"] = vm.displayName();
            body["AppClass"] = vm.appClass();

            // Environments
            body["Environments"] = {};
            var envArr = body["Environments"]["Environment"] = [];

            $("#environment .env-title a").each(function () {
                var env = {};
                var envType = $(this).text().toLowerCase();
                env["@name"] = map[envType];

                // Hostnames
                env["Hostnames"] = {};
                var hostnameArr = env["Hostnames"]["Hostname"] = [];
                $("." + envType + "-hostname-section input").each(function () {
                    var value = $(this).val();
                    if (value != "") {
                        hostnameArr.push(value);
                    }
                });

                // AdditionalSPNames
                env["AdditionalServicePrincipalNames"] = {};
                var additionalSPNameArr = env["AdditionalServicePrincipalNames"]["ServicePrincipalName"] = [];
                $("." + envType + "-spname-section input").each(function () {
                    var value = $(this).val();
                    if (value != "") {
                        additionalSPNameArr.push(value);
                    }
                });

                // AppAddresses
                env["AppAddresses"] = {};
                var appAddressArr = env["AppAddresses"]["AppAddress"] = [];
                $("." + envType + "-appaddress-section input").each(function () {
                    var value = $(this).val();
                    if (value != "") {
                        var appAddress = {};
                        appAddress["@Address"] = value;
                        appAddress["@AddressType"] = "Reply";
                        appAddressArr.push(appAddress);
                    }
                });

                // Finish up Environments
                envArr.push(env);
            });
            body["AppPrincipalID"] = vm.appPrincipalId();

            return json;
        };

        return vm;

    });