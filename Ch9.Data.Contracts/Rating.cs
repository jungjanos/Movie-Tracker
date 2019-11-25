namespace Ch9.Data.Contracts
{
    //TODO : would be nice to eliminate this stand alone class by serializing its 
    // UI counterpart to JSON and feeding the JSON into the ApiClient via string
    // no value in this data structure
    public enum Rating
    {
        Half = 1,
        One = 2,
        OneAndHalf = 3,
        Two = 4,
        TwoAndHalf = 5,
        Three = 6,
        ThreeAndHalf = 7,
        Four = 8,
        FourAndHalf = 9,
        Five = 10,
        FiveAndHalf = 11,
        Six = 12,
        SixAndHalf = 13,
        Seven = 14,
        SevenAndHalf = 15,
        Eight = 16,
        EightAndHalf = 17,
        Nine = 18,
        NineAndHalf = 19,
        Ten = 20
    }
}