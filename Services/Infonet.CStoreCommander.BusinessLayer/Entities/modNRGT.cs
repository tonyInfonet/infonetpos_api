using System;


namespace Infonet.CStoreCommander.BusinessLayer.Entities
{
    sealed class Constants
    {

        public const string Trainer = "Trainer";
        public const string TrainFirstTill = "91";
        public const string TrainLastTill = "99";
        public const string TrainerRecipt = "(( Training Mode ))";
        private static DateTime NRGTDate;
        public struct NRGTReport
        {
            public decimal ClosingTotal;
            public decimal OpeningTotal;
            public decimal ClosingTrainTotal;
            public decimal OpeningTrainTotal;
        }

    }
}
