(function () {
    'use strict';

    angular.module('hsapp', []);

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

    angular.module('hsapp')
        .controller('MainController', ['$scope', 'constants', 'managementService', 'gameService', function ($scope, constants, managementService, gameService) {

            // signalr init
            $.connection.hub.url = 'http://localhost:8088/signalr/hubs';
            var proxy = $.connection.hshub;
            $scope.classes = constants.classes;

            
            managementService.proxy = proxy;
            gameService.proxy = proxy;


            // 1. start(init) new game
            managementService.game = gameService.game;
            $scope.game = gameService.game;
            gameService.startNewGame();

            $scope.loaded = false;
            $scope.getDecks = function (className) {
                managementService.getDecks(className).then(applyDecks);
            }

            $scope.filterByCost = function (number) {
                managementService.filterByCost(number);
            }

            $scope.changeDeck = function (deckLink) {
                managementService.changeDeck(deckLink);
            }

            function applyDecks(decks) {
                gameService.game.decks = decks;
                $scope.$apply();
                $('li[data-toggle="tooltip"]').tooltip({
                    animated: 'fade',
                    placement: 'top',
                    html: true,
                });
            }

            // end game

            proxy.client.sendMessage = function (message) {
                gameService.handleEvent(message);
                $scope.$apply();
            }


            $.connection.hub.start().done(function (err) {
                $scope.loaded = true;
                $scope.$apply();
            });

            $scope.cardDisplayClass = function (card) {
                if (card.timesPlayed === 0)
                    return "";
                return card.timesPlayed >= card.count ? 'hide-card' : 'blur-card';
            }
            /*
            
                        var gameId;
                        $scope.playerHand = [];
                        $scope.opponentHand = [];
                        $scope.opponentCardsPlayed = [];
                        $scope.possibleCardsToPlay = [];
                        $scope.turnNumber = 1;
                        var gameStartFlag = true;
                        var gameCounter = 0;
                        var playersFirstTurn = false;
                        var playersTurn = false;
                        $scope.decks = [];
            
            
                        proxy.client.sendMessage = function (message) {
                            if (message.eventType === gameEvents.onPlayerDraw) {
                                $scope.playerHand.push(Card(message));
                            }
                            else if (message.eventType === gameEvents.onPlayerGet) {
                                $scope.playerHand.push(Coin(message));
                            }
                            else if (message.eventType === gameEvents.onTurnStart) {
                                console.log('===== turn event =====');
                                playersTurn = !playersTurn;
                                if ($scope.playerHand.length > 0 && gameStartFlag) { // we have coin
                                    playersFirstTurn = false;
                                    $scope.playerHand.push(Coin(message));
                                    gameStartFlag = false;
            
                                } else if (gameStartFlag) { 
                                    playersFirstTurn = true;
                                    $scope.opponentHand.push(Coin(message));
                                    gameStartFlag = false;
                                }
            
                                gameCounter++;
            
                                $scope.turnNumber = gameCounter > 1 ? gameCounter % 2 === 0 ? $scope.turnNumber : ++$scope.turnNumber : 1;
                                console.log('turn number ' + $scope.turnNumber);
                                if (playersTurn)
                                {
                                    recalcPossiblePlays();
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
            
                                _.each($scope.decks, function (item) {
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
            
                                $scope.playerHand = [];
                                $scope.opponentHand = [];
                                $scope.opponentCardsPlayed = [];
                                $scope.possibleCardsToPlay = [];
                                $scope.turnNumber = 1;
                                gameStartFlag = true;
                                
                                gameCounter = 0;
                                playersTurn=false;
                                playersFirstTurn = false;
                                $scope.decks = [];
                                // save game 
                            } else {
                                console.log('event type untracked ' + message.eventType);
                            }
            
                            $scope.$apply();
                            $('li[data-toggle="tooltip"]').tooltip({
                                animated: 'fade',
                                placement: 'bottom',
                                html: true,
                            });
                        }
            
                        $scope.cardDisplayClass = function (card) {
                            if (card.timesPlayed === 0)
                                return "";
                            return card.timesPlayed >= card.count ? 'hide-card' : 'blur-card';
                        }
                        $.connection.hub.start().done(function (err) {
                            $scope.loaded = true;
                            $scope.$apply();
                        });
            
                        /*$scope.changeDeck = function (deckLink) {
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
                        }#1#
            
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
                    
            
            */
        }]);
})();