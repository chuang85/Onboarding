define(function (require) {

    return {
        _lastId: 1,
        _servicePrincipalTemplates: [
            { id: 0, name: "Office365", environment: "grn001" },
            { id: 1, name: "Exchange", environment: "grnppe" },
        ],

        listSpts: function () {
            return this._servicePrincipalTemplates;
        },

        createSpts: function (sptToAdd) {
            sptToAdd.id = ++this._lastId;
            this._servicePrincipalTemplates.push(sptToAdd);
        }
    };
});