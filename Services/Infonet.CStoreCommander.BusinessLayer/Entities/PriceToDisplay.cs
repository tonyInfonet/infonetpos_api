using System.Collections.Generic;

namespace Infonet.CStoreCommander.BusinessLayer.Entities
{
    public class PriceToDisplay
    {
        public List<string> Grades { get; set; } = new List<string>();
        public List<string> Tiers { get; set; } = new List<string>();
        public List<string> Levels { get; set; } = new List<string>();

        public List<PriceToDisplayComboBox> GradesState { get; set; } = new List<PriceToDisplayComboBox>();
        public List<PriceToDisplayComboBox> TiersState { get; set; } = new List<PriceToDisplayComboBox>();
        public List<PriceToDisplayComboBox> LevelsState { get; set; } = new List<PriceToDisplayComboBox>();
    }

    public class PriceToDisplayComboBox
    {
        public bool IsEnabled { get; set; }
        public string SelectedValue { get; set; }
    }
}
