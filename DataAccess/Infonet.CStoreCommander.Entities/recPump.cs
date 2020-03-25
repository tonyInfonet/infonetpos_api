using Microsoft.VisualBasic;

namespace Infonet.CStoreCommander.Entities
{
    public struct recPump
    {
        public float Volume;
        public float price;
        public float UnitPrice;
        [VBFixedArray(9)]
        public float[] cashUP;
        [VBFixedArray(9)]
        public float[] creditUP;
        public short Position;
        public string Title;
        public short Button;
        public string[] Stock_Code;
        public short Tier;
        public short Level;
        public bool PayPump;
        public float Amount;
        public bool Pumping;
        public SoundType CallingSound;
        public SoundType StopSound;
        public SoundType PayPumpCallingSound;
        public bool IsHoldPrepay;
        public bool IsPrepay;
        public float PrepayAmount;
        public int PrepayInvoiceID;
        public short PrepayPosition;
        public bool IsPrepayLocked;

        public void Initialize()
        {
            cashUP = new float[10];
            creditUP = new float[10];
            Stock_Code = new string[10];
        }
    }

    public struct SoundType
    {
        public bool NeedPlay;
        public short ListName;
    }
}
