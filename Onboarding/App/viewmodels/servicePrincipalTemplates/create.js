define(['durandal/system', 'durandal/app', 'plugins/router', 'repositories/sptsRepository'],
    function (system, app, router, sptsRepository) {
        return {
            sptToAdd: {
                name: ko.observable(),
                environment: ko.observable()
            },

            activate: function () {
                this.sptToAdd.name("");
                this.sptToAdd.environment("");
                this._sptToAdd = false;
            },

            //canDeactivate: function () {
            //    if (this._sptToAdd == false) {
            //        return app.showMessage('Are you sure you want to leave this page?', 'Navigate', ['Yes', 'No']);
            //    } else {
            //        return true;
            //    }
            //},

            createSPT: function () {
                // Add spt to db
                sptsRepository.createSpts(ko.toJS(this.sptToAdd));

                // flag new spt
                this._sptToAdd = true;
                toastr.info('Create successfully');
                // return to list of spts
                router.navigate("");
            }
        };

    });