define(function() {
    ko.bindingHandlers.selectPicker = {
        init: function (element, valueAccessor, allBindingsAccessor) {
            if ($(element).is('select')) {
                if (ko.isObservable(valueAccessor())) {
                    if ($(element).prop('multiple') && $.isArray(ko.utils.unwrapObservable(valueAccessor()))) {
                        // in the case of a multiple select where the valueAccessor() is an observableArray, call the default Knockout selectedOptions binding
                        ko.bindingHandlers.selectedOptions.init(element, valueAccessor, allBindingsAccessor);
                    } else {
                        // regular select and observable so call the default value binding
                        ko.bindingHandlers.value.init(element, valueAccessor, allBindingsAccessor);
                    }
                }
                $(element).addClass('selectpicker').selectpicker();
            }
        },
        update: function (element, valueAccessor, allBindingsAccessor) {
            if ($(element).is('select')) {
                var selectPickerOptions = allBindingsAccessor().selectPickerOptions;
                if (typeof selectPickerOptions !== 'undefined' && selectPickerOptions !== null) {
                    var options = selectPickerOptions.optionsArray,
                        optionsText = selectPickerOptions.optionsText,
                        optionsValue = selectPickerOptions.optionsValue,
                        optionsCaption = selectPickerOptions.optionsCaption,
                        isDisabled = selectPickerOptions.disabledCondition || false,
                        resetOnDisabled = selectPickerOptions.resetOnDisabled || false;
                    if (ko.utils.unwrapObservable(options).length > 0) {
                        // call the default Knockout options binding
                        ko.bindingHandlers.options.update(element, options, allBindingsAccessor);
                    }
                    if (isDisabled && resetOnDisabled) {
                        // the dropdown is disabled and we need to reset it to its first option
                        $(element).selectpicker('val', $(element).children('option:first').val());
                    }
                    $(element).prop('disabled', isDisabled);
                }
                if (ko.isObservable(valueAccessor())) {
                    if ($(element).prop('multiple') && $.isArray(ko.utils.unwrapObservable(valueAccessor()))) {
                        // in the case of a multiple select where the valueAccessor() is an observableArray, call the default Knockout selectedOptions binding
                        ko.bindingHandlers.selectedOptions.update(element, valueAccessor);
                    } else {
                        // call the default Knockout value binding
                        ko.bindingHandlers.value.update(element, valueAccessor);
                    }
                }

                $(element).selectpicker('refresh');
            }
        }
    };

    ko.bindingHandlers.bootstrapSwitchOn = {
        init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            var $elem = $(element);
            $(element).bootstrapSwitch();
            $(element).bootstrapSwitch('setState', ko.utils.unwrapObservable(valueAccessor())); // Set intial state
            $elem.on('switch-change', function (e, data) {
                valueAccessor()(data.value);
            }); // Update the model when changed.
        },
        update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            var vStatus = $(element).bootstrapSwitch('state');
            var vmStatus = ko.utils.unwrapObservable(valueAccessor());
            if (vStatus != vmStatus) {
                $(element).bootstrapSwitch('setState', vmStatus);
            }
        }
    };

});