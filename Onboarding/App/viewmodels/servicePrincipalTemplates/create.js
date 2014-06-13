define(['durandal/system', 'durandal/app', 'plugins/router'],
    function (system, app, router) {

        var vm = {
            name: ko.observable(),
            environment: ko.observable(),
            activate: activate
        };

        function activate() {
            this.name("");
            this.environment("");
            this._sptToAdd = false;
        };

        //canDeactivate: function () {
        //    if (this._sptToAdd == false) {
        //        return app.showMessage('Are you sure you want to leave this page?', 'Navigate', ['Yes', 'No']);
        //    } else {
        //        return true;
        //    }
        //},

        return vm;

    });