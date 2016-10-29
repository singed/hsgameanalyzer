(function() {
    'use strict';

    // tooltiop directive
    angular.module('hsapp').directive('cardTooltip', function () {
        return {
            restrict: 'EA',
            scope: {
                title: '@'
            },
            link: function ($scope, element, attrs) {
                $(element).tooltip({
                    animated: 'fade',
                    placement: 'bottom',
                    html: true,
                    content: "<img src='http://wow.zamimg.com/images/hearthstone/cards/enus/medium/CS2_045.png' />",

                });
            }
        };
    });

})();


