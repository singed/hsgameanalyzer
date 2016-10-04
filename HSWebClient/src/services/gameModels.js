(function () {
    'use strict';

    var serviceId = 'gameModels';

    angular.module('hsapp').service(serviceId, [
        'constants', GameModels]);

    function GameModels(constants) {
        return {
            Card: Card,
            Coin: Coin,
            game: {
                playerHand: [],
                opponentHand: [],
                opponentCardsPlayed: [],
                possibleCardsToPlay: [],
                turnNumber: 1,
                decks:[],
                playersTurn: false
            },
            initNewGame: init
        }

        function init() {
            this.playerHand = [];
            this.opponentHand = [];
            this.opponentCardsPlayed = [];
            this.possibleCardsToPlay = [];
            this.turnNumber = 1;
            this.playersTurn = false;
        }

        function Card(message) {
            return {
                id: message.data.Id,
                name: message.data.Name,
                cost: message.data.Cost,
                count: 1,
                image: "<img src='http://wow.zamimg.com/images/hearthstone/cards/enus/medium/" + message.data.Id + ".png' />"
            }
        }

        function Coin() {
            return { id: '0', name: "Coin", cost: "0" };
        }
    }
})();