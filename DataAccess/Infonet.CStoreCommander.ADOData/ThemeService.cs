using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Logging;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;

namespace Infonet.CStoreCommander.ADOData
{
    public class ThemeService : SqlDbService, IThemeService
    {
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;

        /// <summary>
        /// Method to get active theme
        /// </summary>
        /// <returns>Theme</returns>
        public Theme GetActiveTheme()
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,ThemeService,GetActiveTheme,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            var query = "SELECT Theme.Id as ThemeId, Name, IsActive, ThemeData.Id as DataId, ColorName, ColorCode FROM Theme INNER JOIN ThemeData ON Theme.Id = ThemeData.ThemeId WHERE IsActive=1 ORDER BY Theme.Id";
            var recordSet = GetRecords(query, DataSource.CSCAdmin);

            if (recordSet != null && recordSet.Rows.Count > 0)
            {
                var theme = new Theme
                {
                    Id = Guid.Parse(Convert.ToString(recordSet.Rows[0]["ThemeId"])),
                    IsActive = Convert.ToBoolean(recordSet.Rows[0]["IsActive"]),
                    Name = Convert.ToString(recordSet.Rows[0]["Name"]),
                    Data = new List<ThemeData>()
                };

                foreach (DataRow dataRow in recordSet.Rows)
                {
                    var themeData = new ThemeData
                    {
                        Id = Guid.Parse(CommonUtility.GetStringValue(dataRow["DataId"])),
                        Name = CommonUtility.GetStringValue(dataRow["ColorName"]),
                        ColorCode = CommonUtility.GetStringValue(dataRow["ColorCode"])
                    };

                    theme.Data.Add(themeData);
                }

                return theme;
            }

            _performancelog.Debug($"End,ThemeService,GetActiveTheme,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return null;
        }
    }
}
