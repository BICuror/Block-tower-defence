namespace CuroLocalization
{
    public static class LocalizationStringExstentions
    {
        public static string Localize(this string key) => LocalizationManager.GetLocalization(key);
    }
}