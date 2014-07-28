define(['plugins/router', 'durandal/app', 'services/bindings'], function (router, app, bindings) {
    return {
        router: router,
        search: function () {
            //It's really easy to show a message box.
            //You can add custom options too. Also, it returns a promise for the user's response.
            app.showMessage('Search not yet implemented...');
        },
        activate: function () {
            router.map([
                { route: '', title: 'Home', moduleId: 'viewmodels/home', nav: true },
                { route: 'createRequest', title: 'Create Request', moduleId: 'viewmodels/createRequest', nav:true },
                { route: 'viewRequest', title: 'View Request', moduleId: 'viewmodels/viewRequest', nav: true },
                { route: 'updateProperties', title: 'Update Properties', moduleId: 'viewmodels/updateProperties', nav: true },
                { route: 'addCert', title: 'Add Certificate', moduleId: 'viewmodel/addCert', nav: true},
                { route: 'info', title: 'More Info', moduleId: 'viewmodels/info', nav: true },
                { route: 'request/detail/:id', title: 'Detail', moduleId: 'viewmodels/detail' },
            ]).buildNavigationModel();

            return router.activate();
        }
    };
});