define(function() {
    var vm = {
        serviceName: serviceName,
        manager: manager
    };

    function serviceName() {
        return 'breeze/Breeze';
    }

    function manager() {
        return new breeze.EntityManager(vm.serviceName());
    }

    return vm;
});