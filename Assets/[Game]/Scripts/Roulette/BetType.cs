namespace Game.Roulette
{
    public enum BetType
    {
        // Inside Bets
        Straight, // Tek sayı
        Split,    // 2 sayı arası
        Street,   // 3lü sıra
        Corner,   // 4lü kare
        SixLine,  // 6lı sıra

        // Outside Bets
        Red,
        Black,
        Even,
        Odd,
        High,     // 19-36
        Low,      // 1-18
        Dozen1,   // 1-12
        Dozen2,   // 13-24
        Dozen3,   // 25-36
        Column1,
        Column2,
        Column3
    }
}