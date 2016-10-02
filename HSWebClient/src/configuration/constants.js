(function () {
    'use strict';
    var serviceId = "constants";
    angular.module('hsapp').service(serviceId, constants);

    function constants() {
        var service = {
            gameEvents: gameEvents(),
            classes: getClasses()
        };
        return service;
    }

    function gameEvents() {
        return {
            onGameStart: 1,
            onTurnStart: 2,
            onPlayerMulligan: 3,
            onPlayerGet: 4,
            onOpponentGet: 5,
            onOpponentDraw: 6,
            onOpponentPlay: 7,
            onOpponentHandDiscard: 8,
            onOpponentPlayToDeck: 9,
            onOpponentPlayToHand: 10,
            onGameEnd: 11,
            onPlayerDraw: 12,
            onOpponentHeroPower:13,
            onGameLost: 14,
            onGameWon: 15
        };
    }

    function getClasses() {
        return ["Druid", "Hunter", "Mage", "Paladin", "Priest",
        "Rogue", "Shaman", "Warlock", "Warrior"];
    }
})();