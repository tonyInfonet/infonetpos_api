using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.ADOData
{
    public interface IThemeService
    {

        /// <summary>
        /// Method to get active theme
        /// </summary>
        /// <returns>Theme</returns>
        Theme GetActiveTheme();
    }
}
