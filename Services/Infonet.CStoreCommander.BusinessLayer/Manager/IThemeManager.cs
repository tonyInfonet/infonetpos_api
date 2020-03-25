using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface IThemeManager
    {
        /// <summary>
        /// Method to get active theme
        /// </summary>
        /// <returns></returns>
        Theme GetActiveTheme();
    }
}
