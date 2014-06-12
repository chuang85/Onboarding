define(['plugins/router', 'durandal/app'], function (router, app) {
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
                { route: 'create', title: 'Create', moduleId: 'viewmodels/servicePrincipalTemplates/create', nav: true },
            { route: 'configure', title: 'Configure', moduleId: 'viewmodels/servicePrincipalTemplates/configure', nav: true }
            ]).buildNavigationModel();

            return router.activate();
        }
    };
});