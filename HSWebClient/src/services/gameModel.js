(function () {
    'use strict';

    var serviceId = 'gameModel';

    angular.module('hsapp').service(serviceId, [
        'constants', GameModel]);

    function GameModel(constants) {
        return {
            init: init
        }

        function init() {
            return {
                playerHand: [],
                opponentHand: [],
                opponentCardsPlayed: [],
                possibleCardsToPlay: [],
                turnNumber: 1,
                decks: []
            }
        }
    }
})();