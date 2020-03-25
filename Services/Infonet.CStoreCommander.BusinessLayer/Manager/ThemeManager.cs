using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class ThemeManager : IThemeManager
    {
        private readonly IThemeService _themeService;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="themeService"></param>
        public ThemeManager(IThemeService themeService)
        {
            _themeService = themeService;
        }

        /// <summary>
        /// Method to get active theme
        /// </summary>
        /// <returns>Theme</returns>
        public Theme GetActiveTheme()
        {
            return _themeService.GetActiveTheme();
        }
    }
}
