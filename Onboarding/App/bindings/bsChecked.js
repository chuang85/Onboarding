define(['knockout', 'jquery'], function (ko, $) {
    var install = function () {
        ko.bindingHandlers.bsChecked = {
            init: function (element, valueAccessor, allBindingsAccessor,
            viewModel, bindingContext) {
                var value = valueAccessor();
                var newValueAccessor = function () {
                    return {
                        change: function () {
                            value(element.value);
                        }
                    }
                };
                ko.bindingHandlers.event.init(element, newValueAccessor,
                allBindingsAccessor, viewModel, bindingContext);
            },
            update: function (element, valueAccessor, allBindingsAccessor,
            viewModel, bindingContext) {
                if ($(element).val() == ko.unwrap(valueAccessor())) {
                    $(element).closest('.btn').button('toggle');
                }
            }
        }
    };

    return {
        install: install
    };
});