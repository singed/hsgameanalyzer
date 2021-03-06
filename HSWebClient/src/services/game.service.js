﻿(function () {
    'use strict';

    var serviceId = "gameService";
    angular.module('hsapp').factory(serviceId, ['constants', 'gameModels', 'managementService', '$rootScope', GameService]);

    function GameService(constants, gameModels, managementService, $rootScope) {

        var service = {
            handleEvent: handleEvent,
            startNewGame: startNewGame,
            game: gameModels.game,
            recalcPossiblePlays: recalcPossiblePlays,
            proxy: null,
            gameId:0
        };

        return service;

        var playersTurn, isGameStarted, gameCounter, playersFirstTurn;

        function startNewGame() {
            gameModels.clear();
            isGameStarted = true;
            gameCounter = 0;
            playersFirstTurn = false;
            playersTurn = false;
        }

        function handleEvent(message) {
            if (message.eventType === constants.gameEvents.onTurnStart) {
                console.log('===== turn event =====');
                playersTurn = !playersTurn;
                if (message.data.PlayerHasCoin !== undefined) {
                    playersFirstTurn = !message.data.PlayerHasCoin;
                    managementService.getDecks(message.data.OpponentClass).then(function(decks) {
                        service.game.decks = decks;
                        $rootScope.$apply();
                    }); 
                }
                gameCounter++;

                this.game.turnNumber = gameCounter > 1 ? gameCounter % 2 === 0 ? this.game.turnNumber : ++this.game.turnNumber : 1;
                console.log('turn number ' + this.game.turnNumber);
                if (playersTurn) {
                    this.recalcPossiblePlays();
                }
            }
            else if (message.eventType === constants.gameEvents.onOpponentPlay) {
                var playedCard = {
                    id: message.data.Id,
                    name: message.data.Name,
                    cost: message.data.Cost,
                    count: 1,
                    image: "<img src='http://wow.zamimg.com/images/hearthstone/cards/enus/medium/" + message.data.Id + ".png' />"
                };

                // check is card already played, push it to the played cards and cross out from the decks
                var index = _.indexOf(this.game.opponentCardsPlayed, _.find(this.game.opponentCardsPlayed, { id: playedCard.id }));
                if (index >= 0) {
                    this.game.opponentCardsPlayed[index].count = Number(this.game.opponentCardsPlayed[index].count) + 1;
                } else {
                    this.game.opponentCardsPlayed.push(playedCard);
                }

                // re-calculate deck overlap
                _.each(this.game.decks, function (item) {
                    var matchedCard = _.find(item.cards, { cardId: playedCard.id });
                    if (!!matchedCard) {
                        matchedCard.timesPlayed++;
                        item.percentage += 3;
                    }
                });

                // save turn 
                var turnData = {
                    gameId: service.gameId,
                    turnNumber: this.game.turnNumber,
                    cardId: playedCard.id
                }
                this.proxy.invoke('saveTurn', turnData).done(function () {
                    console.log('card sent');
                });

            }
            else if (message.eventType === constants.gameEvents.onGameStart) {
                startNewGame();
                service.gameId = message.data.GameId;
            }
            else if (message.eventType === constants.gameEvents.onGameLost || message.eventType === constants.gameEvents.onGameWon) {
                var deck = _.last(_.orderBy(service.game.decks, ['percentage']));
                if (deck) {
                    var won = message.eventType === constants.gameEvents.onGameWon ? true : false;
                    var game = {
                        gameId: service.gameId,
                        opponentDeckId: deck.id,
                        opponentDeckType: deck.type,
                        opponentDeckMatch: deck.percentage,
                        won: won
                    };

                    this.proxy.invoke('endGame', game).done(function() {
                        console.log('game saved');
                    });
                }
            } else {
                console.log('event type untracked ' + message.eventType);
            }
        }
        function recalcPossiblePlays() {
            // re-calculate possible cards to play
            var decks = _.filter(this.game.decks, function (d) {
                return d.isChecked;
            });
            var possibleCardsOnThisTurn = [];
            _.each(decks, function (item) {
                var matchedCards = _.filter(item.cards, function (card) {
                    if (card.cost === service.game.turnNumber) {
                        return card;
                    }
                });
                possibleCardsOnThisTurn = _.concat(possibleCardsOnThisTurn, matchedCards);
            });
            this.game.possibleCardsToPlay = _.uniqBy(possibleCardsOnThisTurn, 'cardId');
        }
    }

})();