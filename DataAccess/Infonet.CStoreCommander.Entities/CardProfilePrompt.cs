namespace Infonet.CStoreCommander.Entities
{
    public class CardProfilePrompt
    {
        public int TillNumber { get; set; }
        public int SaleNumber { get; set; }
        public string CardNumber { get; set; }
        public string ProfileID { get; set; }
        public short PromptID { get; set; }
        public string PromptAnswer { get; set; }
        //added for a reciept print only
        public string DisplayText { get; set; }
    }
}
