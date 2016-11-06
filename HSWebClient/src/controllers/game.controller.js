(function () {
    'use strict';

    var controllerId = "gameController";

    angular.module('hsapp')
        .controller(controllerId, ['$scope', 'constants', 'managementService', 'gameService', 'gameModels', '$filter', function ($scope, constants, managementService, gameService, gameModels, $filter) {

            // signalr init
            $.connection.hub.url = 'http://localhost:8088/signalr/hubs';
            var proxy = $.connection.hshub;
            $scope.classes = constants.classes;
            $scope.cardName = '';
            $scope.$watch('game.decks', function (decks) {
                $('li[data-toggle="tooltip"]').tooltip({
                    animated: 'fade',
                    placement: 'top',
                    html: true
                });
            });

            $scope.$watch('cardName', function (cardName) {
                $scope.game.decks = $filter('filterByCard')($scope.game.decks, cardName);
            });
            managementService.proxy = proxy;
            gameService.proxy = proxy;


            // 1. start(init) new game
            /*managementService.game = gameService.game;*/
            $scope.game = gameModels.game;
            /*gameService.startNewGame();*/

            $scope.loaded = false;
            $scope.getDecks = function (className) {
                managementService.getDecks(className).then(applyDecks);
                $scope.newGameInit = false;
            }

            $scope.filterByCost = function (number) {
                managementService.filterByCost(number);
            }
            $scope.startNewGame = function () {
                $scope.newGameInit = true;
                gameService.startNewGame();
            }

            $scope.changeDeck = function (deckLink) {
                managementService.changeDeck(deckLink);
            }

            /*  function applyDecks(decks) {
                  gameService.game.decks = decks;
               
              }*/

            // end game

            proxy.client.sendMessage = function (message) {
                gameService.handleEvent(message);
                $scope.$apply();
            }


            $.connection.hub.start().done(function (err) {
                $scope.loaded = true;
                //$scope.$apply();
            });

            $scope.cardDisplayClass = function (card) {
                if (card.timesPlayed === 0)
                    return "";
                return card.timesPlayed >= card.count ? 'hide-card' : 'blur-card';
            }
        }]);

})();