define(['services/guidgenerator'],
    function (guidgenerator) {
        var vm = {
            createJSONSpt: createJSONSpt
        };

        function createJSONSpt(vm) {
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
            body["@ObjectId"] = vm.objectId();

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
            sp["KeyGroupID"] = vm.keyGroupId();

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