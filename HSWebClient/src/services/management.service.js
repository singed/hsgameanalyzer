(function () {
    'use strict';

    var serviceId = "managementService";
    angular.module('hsapp').factory(serviceId, ['constants', 'gameModels', GameService]);

    function GameService(constants, gameModels) {
        var service = {
            changeDeck: changeDeck,
            getDecks: getDecks,
            init: init,
            game: gameModels.game,
            filterByCost: filterByCost,
            proxy :null
        };
        return service;

        function changeDeck(deckLink) {
            var decks = _.filter(this.game.decks, function (d) {
                return d.isChecked;
            });
            var possibleCardsOnThisTurn = [];
            _.each(decks, function (item) {
                var matchedCards = _.filter(item.cards, function (card) {
                    if (card.cost <= service.game.turnNumber) {
                        return card;
                    }
                });
                possibleCardsOnThisTurn = _.concat(possibleCardsOnThisTurn, matchedCards);
            });

            this.game.possibleCardsToPlay = _.uniqBy(possibleCardsOnThisTurn, 'cardId');
        }

        function getDecks(className) {
            this.game.opponentClass = className;
            return this.proxy.invoke('getDecks', className)
                .done(manageDecks)
                .fail(function (err) { console.log(err); });
        }

        function filterByCost(number) {
            var decks = _.filter(this.game.decks, function (d) {
                return d.isChecked;
            });
            var possibleCardsOnThisTurn = [];
            _.each(decks, function (item) {
                var matchedCards = _.filter(item.cards, function (card) {
                    if (card.cost === number) {
                        return card;
                    }
                });
                possibleCardsOnThisTurn = _.concat(possibleCardsOnThisTurn, matchedCards);
            });
            this.game.possibleCardsToPlay = _.uniqBy(possibleCardsOnThisTurn, 'cardId');
        }

        function manageDecks(decks) {
            service.game.decks = _.map(decks, function (item) {
                item.percentage = 0;
                item.isVisible = true;
                item.isCheccked = false;
                _.each(item.cards, function (card) {
                    card.timesPlayed = 0;
                    card.image = "<img style='margin-bottom:50pxl' src='http://wow.zamimg.com/images/hearthstone/cards/enus/medium/" + card.cardId + ".png' />";
                    card.imageUrl = "/src/images/cards/" + card.cardId + ".png";
                });
                return item;
            });

            return service.game.decks;
        }

        function init(proxy) {
            this.proxy = proxy;
        }
    }
})();