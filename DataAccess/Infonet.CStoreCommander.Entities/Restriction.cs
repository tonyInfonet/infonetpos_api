using System;
using Microsoft.VisualBasic;

namespace Infonet.CStoreCommander.Entities
{
    [Serializable]
    public class Restriction
    {
        private short mvarCode;
        private string mvarDescription;
        private bool mvarConfirmed;


        public short Code
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarCode;
                return returnValue;
            }
            set
            {
                mvarCode = value;
            }
        }


        public string Description
        {
            get
            {
                string returnValue = "";
                returnValue = mvarDescription;
                return returnValue;
            }
            set
            {
                mvarDescription = value;
            }
        }


        public bool Confirmed
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarConfirmed;
                return returnValue;
            }
            set
            {
                mvarConfirmed = value;
            }
        }

        public bool Exist_Restriction
        {
            get;set;
        }

        private void Class_Initialize_Renamed()
        {
            mvarConfirmed = true;
        }
        public Restriction()
        {
            Class_Initialize_Renamed();
        }
    }
}
