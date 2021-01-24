namespace Paya.Automation.Editor.Models
{
    using System.Threading.Tasks;

    [System.Serializable]
    public static class IsolatedStorageSettings
    {
        #region Static Fields

        private static readonly StorageDictionary _ApplicationSettings = StorageDictionary.Load("ApplicationSettings.json");

        private static readonly StorageDictionary _SiteSettings = StorageDictionary.Load("SiteSettings.json");

        #endregion

        #region Public Properties

        public static StorageDictionary ApplicationSettings
        {
            get { return _ApplicationSettings; }
        }

        public static StorageDictionary SiteSettings
        {
            get { return _SiteSettings; }
        }

        #endregion

        #region Public Methods and Operators

        public static async Task SaveAsync()
        {
            await SiteSettings.SaveAsync("SiteSettings.json");
            await ApplicationSettings.SaveAsync("ApplicationSettings.json");
        }

        #endregion
    }
}
