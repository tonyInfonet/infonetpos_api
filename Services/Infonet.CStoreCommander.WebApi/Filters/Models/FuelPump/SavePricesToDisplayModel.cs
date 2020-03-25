using System.Collections.Generic;

namespace Infonet.CStoreCommander.WebApi.Models.FuelPump
{
    public class SavePricesToDisplayModel
    {
        public List<string> SelectedGrades { get; set; }
        public List<string> SelectedTiers { get; set; }
        public List<string> SelectedLevels { get; set; }
    }
}