define(function() {
    var vm = {
        removeDomain: removeDomain
    };

    function removeDomain(raw) {
        if (raw.indexOf("\\") > -1) {
            var res = raw.split("\\");
            return res[res.length - 1];
        } else {
            return raw;
        }
    }

    return vm;
});