(function () {
    'use strict';

    angular.module('hsapp', []);

    angular.module('hsapp').filter('reverse', function () {
        return function (items) {
            return items.slice().reverse();
        };
    });


   


    angular.module('hsapp')
        .controller('MainController', ['$scope', 'constants', function ($scope, constants) {
            $scope.loaded = false;
            var gameEvents = constants.gameEvents;
            $scope.classes = constants.classes;

            var gameId;
            $scope.data = [];
            $scope.playerHand = [];
            $scope.opponentHand = [];
            $scope.opponentCardsPlayed = [];
            $scope.possibleCardsToPlay = [];
            $scope.turnNumber = 1;
            var gameStartFlag = true;
            var gameCounter = 0;
            $scope.playersTurn = false;
            $scope.decks = [];
            $.connection.hub.url = 'http://localhost:8088/signalr/hubs';
            var proxy = $.connection.hshub;
            proxy.client.sendMessage = function (message) {
                if (message.eventType === gameEvents.onPlayerDraw) {
                    $scope.playerHand.push(Card(message));
                    console.log('Player draws.');
                }
                else if (message.eventType === gameEvents.onOpponentDraw) {
                    $scope.opponentHand.push({});
                    if ($scope.opponentHand.length === 4) $scope.playersTurn = true;
                    console.log('Opponent draws.');
                }
                else if (message.eventType === gameEvents.onPlayerGet) {
                    $scope.playerHand.push(Coin(message));
                }
                else if (message.eventType === gameEvents.onOpponentGet) {
                    $scope.opponentHand.push({});
                }
                else if (message.eventType === gameEvents.onTurnStart) {
                    $scope.playersTurn = !$scope.playersTurn;
                    gameCounter++;
                    console.log('players hand - ' + $scope.playerHand.length);
                    console.log('opponent hand - ' + $scope.opponentHand.length);
                    if ($scope.playerHand.length > $scope.opponentHand.length) {
                        console.log('players turn');
                        $scope.playersTurn = true;
                    } else {
                        console.log('opponents turn');
                    }

                    $scope.turnNumber = gameCounter % 2 === 0 ? $scope.turnNumber : ++$scope.turnNumber;
                    console.log('turn number ' + $scope.turnNumber);
                    if ($scope.playersTurn) // opponent has the coin, we go first
                    {
                        playersTurn();
                    } else {
                        console.log('opponents turn;');
                    }
                }
                else if (message.eventType === gameEvents.onOpponentPlay) {
                    var playedCard = {
                        id: message.data.Id,
                        name: message.data.Name,
                        cost: message.data.Cost,
                        count: 1,
                        image: "<img src='http://wow.zamimg.com/images/hearthstone/cards/enus/medium/" + message.data.Id + ".png' />"
                    };

                    // check is card already played, push it to the played cards
                    var index = _.indexOf($scope.opponentCardsPlayed, _.find($scope.opponentCardsPlayed, { id: playedCard.id }));
                    if (index >= 0) {
                        $scope.opponentCardsPlayed[index].count = Number($scope.opponentCardsPlayed[index].count) + 1;
                    } else {
                        $scope.opponentCardsPlayed.push(playedCard);
                    }

                    // re-calculate deck overlap

                    _.each($scope.decks, function(item) {
                        var matchedCard = _.find(item.cards, { cardId: playedCard.id });
                        if (!!matchedCard) {
                            item.percentage += 3;
                        }
                    });
                    var turnData = {
                        gameId: gameId,
                        turnNumber: $scope.turnNumber,
                        cardId: playedCard.id
                    }
                    proxy.invoke('saveTurn', turnData).done(function () {
                        console.log('card sent');
                    });

                }
                else if (message.eventType === gameEvents.onGameStart) {
                    gameId = message.data.gameId;
                } else {
                    console.log('event type untracked ' + message.eventType);
                }


                if ($scope.playersTurn && gameStartFlag) {
                    gameStartFlag = false;
                    gameCounter++;
                    playersTurn();
                }

                $scope.$apply();
                $('li[data-toggle="tooltip"]').tooltip({
                    animated: 'fade',
                    placement: 'bottom',
                    html: true,
                });
            }

            $scope.getDecks = function (className) {
                proxy.invoke('getDecks', className).done(function (decks) {
                    $scope.decks = _.map(decks, function (item) {
                        item.percentage = 0;
                        item.isCheccked = false;
                        _.each(item.cards, function (card) {
                            card.timesPlayed = 0;
                            card.image = "<img style='margin-bottom:50pxl' src='http://wow.zamimg.com/images/hearthstone/cards/enus/medium/" + card.cardId + ".png' />";
                            card.imageUrl = "/src/images/cards/" + card.cardId + ".png";
                        });
                        return item;
                    });
                    $scope.$apply();
                    $('li[data-toggle="tooltip"]').tooltip({
                        animated: 'fade',
                        placement: 'top',
                        html: true,
                    });
                }).fail(function (err) {
                    console.log(err);
                });
            }

            $scope.cardDisplayClass = function (card) {
                if (card.timesPlayed === 0)
                    return "";
                return card.timesPlayed >= card.count ? 'hide-card' : 'blur-card';
            }
            $.connection.hub.start().done(function (err) {
                $scope.loaded = true;
                $scope.getDecks('Druid');
                $scope.$apply();
            });

            $scope.changeDeck = function (deckLink) {
                var decks = _.filter($scope.decks, function(d) {
                    return d.isChecked;
                });
                var possibleCardsOnThisTurn = [];
                _.each(decks, function(item) {
                    var matchedCards = _.filter(item.cards, function(card) {
                        if (card.cost <= $scope.turnNumber) {
                            return card;
                        }
                    });
                    possibleCardsOnThisTurn = _.concat(possibleCardsOnThisTurn, matchedCards);
                });
               
                $scope.possibleCardsToPlay = _.uniqBy(possibleCardsOnThisTurn, 'cardId');
            }

            function Coin() {
                return { id: '0', name: "Coin", cost: "0" };
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

            function playersTurn() {
                // re-calculate possible cards to play
                var possibleCardsOnThisTurn = [];
                _.each($scope.decks, function (item) {
                    var matchedCards = _.filter(item.cards, function (card) {
                        if (card.cost <= $scope.turnNumber) {
                            return card;
                        }
                    });
                    possibleCardsOnThisTurn = _.concat(possibleCardsOnThisTurn, matchedCards);
                });
                $scope.possibleCardsToPlay = _.uniqBy(possibleCardsOnThisTurn, 'cardId');
            }

        }]);
})();