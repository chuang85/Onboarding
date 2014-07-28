define(function() {
    var vm = {
        serviceName: serviceName,
        manager: manager,
        fetchEnum: fetchEnum
    };

    function serviceName() {
        return 'breeze/Breeze';
    }

    function manager() {
        return new breeze.EntityManager(vm.serviceName());
    }

    function fetchEnum(data) {
        ko.utils.arrayForEach(data.schema.enumType, function (c) {
            window[c.name] = {};
            ko.utils.arrayForEach(c.member, function (m) {
                window[c.name][m.name] = m.value;
            });
        });
    }

    return vm;
});