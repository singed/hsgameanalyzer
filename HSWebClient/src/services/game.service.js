(function () {
    'use strict';

    var serviceId = "gameService";
    angular.module('hsapp').factory(serviceId, ['constants', 'gameModel', GameService]);

    function GameService(constants, gameModel) {
        var service = {
            game: gameModel.init(),
            changeDeck: changeDeck,
            getDecks: getDecks,
            init: init,
        };
        return service;

        function changeDeck(deckLink) {
            var decks = _.filter(service.game.decks, function (d) {
                return d.isChecked;
            });
            var possibleCardsOnThisTurn = [];
            _.each(service.game.decks, function (item) {
                var matchedCards = _.filter(item.cards, function (card) {
                    if (card.cost <= game.turnNumber) {
                        return card;
                    }
                });
                possibleCardsOnThisTurn = _.concat(possibleCardsOnThisTurn, matchedCards);
            });

            game.possibleCardsToPlay = _.uniqBy(possibleCardsOnThisTurn, 'cardId');
        }

        function getDecks(className) {
            return service.proxy.invoke('getDecks', className).done(manageDecks).fail(function (err) { console.log(err); });
        }

        function manageDecks(decks) {
            service.game.decks = _.map(decks, function (item) {
                item.percentage = 0;
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
            service.proxy = proxy;
            service.game = gameModel.init();
        }


    }
})();