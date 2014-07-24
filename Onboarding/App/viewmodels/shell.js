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
                { route: 'createSpt', title: 'Create SPT', moduleId: 'viewmodels/createSpt', nav: true },
                { route: 'request', title: 'Request', moduleId: 'viewmodels/request', nav: true },
                { route: 'request/detail/:id', title: 'Detail', moduleId: 'viewmodels/detail' },
                { route: 'request/update/:id', title: 'Update', moduleId: 'viewmodels/update' },
                { route: 'request/cert/:id', title: 'Cert', moduleId: 'viewmodels/cert' }
            ]).buildNavigationModel();

            return router.activate();
        }
    };
});