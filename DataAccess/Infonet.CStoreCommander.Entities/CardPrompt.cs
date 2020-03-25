namespace Infonet.CStoreCommander.Entities
{
    public class CardPrompt
    {

        private short mvarPromptID;
        private string mvarPromptMessage;
        private short mvarMinLength;
        private short mvarMaxLength;
        private byte mvarPromptSeq;
        private string mvarPromptAnswer;


        public short PromptID
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarPromptID;
                return returnValue;
            }
            set
            {
                mvarPromptID = value;
            }
        }


        public string PromptMessage
        {
            get
            {
                string returnValue = "";
                returnValue = mvarPromptMessage;
                return returnValue;
            }
            set
            {
                mvarPromptMessage = value;
            }
        }


        public short MinLength
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarMinLength;
                return returnValue;
            }
            set
            {
                mvarMinLength = value;
            }
        }

        public short MaxLength
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarMaxLength;
                return returnValue;
            }
            set
            {
                mvarMaxLength = value;
            }
        }


        public byte PromptSeq
        {
            get
            {
                byte returnValue = 0;
                returnValue = mvarPromptSeq;
                return returnValue;
            }
            set
            {
                mvarPromptSeq = value;
            }
        }


        public string PromptAnswer
        {
            get
            {
                string returnValue = "";
                returnValue = mvarPromptAnswer;
                return returnValue;
            }
            set
            {
                mvarPromptAnswer = value;
            }
        }
    }
}
