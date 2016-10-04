(function () {
    'use strict';

    var serviceId = 'gameModels';

    angular.module('hsapp').service(serviceId, GameModels);

    function GameModels() {
        this.game = {
            playerHand: [],
            opponentHand: [],
            opponentCardsPlayed: [],
            possibleCardsToPlay: [],
            turnNumber: 1,
            decks: [],
            playersTurn: false
        };
        this.Card = Card;
        this.Coin = Coin;
        this.clear = clear;

        function clear() {
            this.game.playerHand = [];
            this.game.opponentHand = [];
            this.game.opponentCardsPlayed = [];
            this.game.possibleCardsToPlay = [];
            this.game.turnNumber = 1;
            this.game.decks = [];
            this.game.playersTurn = false;
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