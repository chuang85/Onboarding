define(['services/dataservices'],
    function (dataservices) {

        var vm = {
            getServiceTypes: getServiceTypes,
            getTaskSets: getTaskSets
        };

        var manager = dataservices.manager();

        function getServiceTypes(vm) {
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

        return vm;
    });