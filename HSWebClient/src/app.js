(function () {
    'use strict';

    angular.module('hsapp', ['ui.router']);


    angular.module('hsapp').filter('reverse', function () {
        return function (items) {
            return items.slice().reverse();
        };
    });

    angular.module('hsapp').filter('range', function () {
        return function (input, total) {
            total = parseInt(total);

            for (var i = 1; i < total; i++) {
                input.push(i);
            }

            return input;
        };
    });

    angular.module('hsapp').filter('filterByCard', function () {
        return function (decks, cardName) {
            _.each(decks, function (item) {
                var result = _.some(item.cards, function(elm) {
                    return elm.name.indexOf(cardName.toUpperCase()) !== -1;
                });
                item.isVisible = true;

                if (!result) {
                    item.isVisible = false;
                }
            });
            return decks;
        };
    });


    angular.module('hsapp').config(['$stateProvider', '$urlRouterProvider', '$locationProvider', function ($stateProvider, $urlRouterProvider, $locationProvider) {

        //$urlRouterProvider.otherwise("/login");
        $urlRouterProvider.otherwise('/');

        $locationProvider.html5Mode({
            enabled: false,
            requireBase: false
        });
        $stateProvider

          .state('public', {
              url: "",
              abstarct: true,
              views: {
                  "app": {
                      templateUrl: "src/index.html"
                  }
              }
          })
        .state('main', {

                 url: "/game",
                 templateUrl: "src/views/game.html",
                 controller: 'gameController',
                 controllerAs: 'vm'
             })
         .state('public.game', {
             
                 url: "/game",
                 templateUrl: "src/views/game.html",
                 controller: 'gameController',
                 controllerAs: 'vm'
         })
        //.state('public.stats', {
        //    url: "/stats",
        //    views: {
        //        "container@public": {
        //            templateUrl: "src/app/views/stats.html",
        //            controller: 'statsController',
        //            controllerAs: 'vm'
        //        }
        //    }
        //})
    }]);

})();