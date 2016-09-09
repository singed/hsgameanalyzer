namespace HSCore
{
    public enum HSGameEventTypes
    {
        OnGameStart = 1,
        OnTurnStart,
        OnPlayerMulligan,
        OnPlayerGet,
        OnOpponentGet,
        OnOpponentDraw, 
        OnOpponentPlay,
        OnOpponentHandDiscard,
        OnOpponentPlayToDeck,
        OnOpponentPlayToHand,
        OnGameEnd,
        OnPlayerDraw,
        OnOpponentHeroPower
    }
}
