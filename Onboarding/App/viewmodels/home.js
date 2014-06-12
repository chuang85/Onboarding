define(['repositories/sptsRepository'], function (sptsRepository) {

    return {
        spts: ko.observable(),

        activate: function () {
            this.spts(sptsRepository.listSpts());
        }
    };
});