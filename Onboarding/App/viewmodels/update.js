define(['plugins/router', 'services/dataservices', 'services/dataformatter', 'services/jsonbuilder', 'services/dbhelper'],
    function (router, dataservices, dataformatter, jsonbuilder, dbhelper) {

        var vm = {
            editableRequest: ko.observable(),
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
            requestType: "UpdateSPT",
            activate: activate,
            saveChanges: saveChanges,
            goBack: goBack,
            addItem: addItem
        };

        var manager = dataservices.manager();

        function activate(id) {
            dbhelper.getServiceTypes(vm);
            dbhelper.getTaskSets(vm);
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
                vm.editableRequest(data.results[0]);
                parseSpt(vm.editableRequest().TempXmlStore());
            }

            function queryFailed(error) {
                toastr.error("Query failed: " + error.message);
            }
        };

        /// <summary>
        /// Listener for update button.
        /// Make change to DB.
        /// </summary>
        function saveChanges() {
            var xmlString = dataformatter.formatXml(dataformatter.json2xml(jsonbuilder.createJSONSpt(vm)));
            vm.editableRequest().TempXmlStore(xmlString);
            manager.saveChanges()
                .then(saveSucceeded)
                .fail(saveFailed);

            function saveSucceeded(data) {
                toastr.success("Saved");
                router.navigate('#request');
            }

            function saveFailed(error) {
                toastr.error("Save failed");
            }
        };

        function goBack() {
            router.navigateBack();
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

        /********************PRIVATE METHODS********************/
        function loadXMLString(txt) {
            var xmlDoc;
            if (window.DOMParser) {
                var parser = new DOMParser();
                xmlDoc = parser.parseFromString(txt, "text/xml");
            }
            else // code for IE
            {
                xmlDoc = new ActiveXObject("Microsoft.XMLDOM");
                xmlDoc.async = false;
                xmlDoc.loadXML(txt);
            }
            return xmlDoc;
        }

        function parseSpt(xml) {
            var spt = loadXMLString(xml);
            vm.displayName(spt.getElementsByTagName("DisplayName")[0].childNodes[0].nodeValue);
            vm.serviceType(spt.getElementsByTagName("ServiceType")[0].getElementsByTagName("Value")[0].childNodes[0].nodeValue);
            vm.appPrincipalId(spt.getElementsByTagName("AppPrincipalID")[0].childNodes[0].nodeValue);
            vm.externalUserAccountDelegationsAllowed(spt.getElementsByTagName("ExternalUserAccountDelegationsAllowed")[0].childNodes[0].nodeValue);
            vm.microsoftPolicyGroup(spt.getElementsByTagName("MicrosoftPolicyGroup")[0].childNodes[0].nodeValue);
            vm.managedExternally(spt.getElementsByTagName("ManagedExternally")[0].childNodes[0].nodeValue);
            console.log("start");
            console.log();
            console.log("end");
        }

        function getTagValue(spt, tagname) {
            return spt.getElementsByTagName(tagname)[0].childNodes[0].nodeValue;
        }

        function getTagNested(spt, tagname) {
            return spt.getElementsByTagName(tagname)[0].getElementsByTagName("Value")[0].childNodes[0].nodeValue
        }

        return vm;
    });