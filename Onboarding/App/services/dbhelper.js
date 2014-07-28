define(['services/dataservices'],
    function (dataservices) {

        var vm = {
            getExistingSpts: getExistingSpts,
            getServiceTypes: getServiceTypes,
            getTaskSets: getTaskSets,
            getDescriptions: getDescriptions
        };

        var manager = dataservices.manager();

        function getExistingSpts(vm) {
            var query = breeze.EntityQuery
                    .from("ExistingSpts")
                    .select("Name, XmlContent")
                    .orderBy("Name");

            return manager
            .executeQuery(query)
            .then(querySucceeded)
            .fail(queryFailed);

            function querySucceeded(data) {
                for (var i = 0; i < data.results.length; i++) {
                    vm.sptList.push(data.results[i]["XmlContent"]);
                }
            }

            function queryFailed(error) {
                toastr.error("Query failed: " + error.message);
            }
        }

        function getServiceTypes(vm) {
            var query = breeze.EntityQuery
                    .from("ExistingSpts")
                    .select("ServiceType")
                    .orderBy("ServiceType");

            return manager
            .executeQuery(query)
            .then(querySucceeded)
            .fail(queryFailed);

            function querySucceeded(data) {
                for (var i = 0; i < data.results.length; i++) {
                    vm.serviceTypeList.push(data.results[i]["ServiceType"]);
                }
            }

            function queryFailed(error) {
                toastr.error("Query failed: " + error.message);
            }
        }

        function getTaskSets(vm) {
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

        function getDescriptions(vm) {
            var query = breeze.EntityQuery
                .from("Descriptions")
                .select("Name, Content");
            var map = {};

            return manager
            .executeQuery(query)
            .then(querySucceeded)
            .fail(queryFailed);

            function querySucceeded(data) {
                for (var i = 0; i < data.results.length; i++) {
                    map[data.results[i]["Name"]] = data.results[i]["Content"];
                }
                vm.descContact(map["Contact"]);
                vm.descRequestSubject(map["RequestSubject"]);
                vm.descDisplayName(map["DisplayName"]);
                vm.descServiceType(map["ServiceType"]);
                vm.descAppPrincipalId(map["AppPrincipalId"]);
                vm.descEnvironment(map["Environment"]);
            }

            function queryFailed(error) {
                toastr.error("Query failed: " + error.message);
            }
        }

        return vm;
    });