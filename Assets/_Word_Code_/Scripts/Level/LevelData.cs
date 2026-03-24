namespace WordCode
{
    public struct LevelData
    {
        public int ID;
        public int IDTheme;
        public string Source;
        public string Quote;
        public bool HasLockedLetters;
        public int LevelType;
        public int[] UnlockedLetters;

        public LevelData(int id, int idTheme, string source, string quote, bool hasLockedLetters, int levelType,
            int[] unlockedLetters)
        {
            ID = id;
            IDTheme = idTheme;
            Source = source;
            Quote = quote;
            HasLockedLetters = hasLockedLetters;
            LevelType = levelType;
            UnlockedLetters = unlockedLetters;
        }
    }
}