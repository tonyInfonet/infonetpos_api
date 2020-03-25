namespace Infonet.CStoreCommander.Entities
{
    public class CIP
    {
        private object myFC_IP;
        private object myFC_UDP_Port;
        private object myFC_TCP_Port;
        private object myPOS_IP;
        private object myPOS_TCP_Port;
        private object myPOS_UDP_Port;
        private byte myNumberOfPOS;

        public byte NumberOfPOS
        {
            get
            {
                byte returnValue = 0;
                returnValue = myNumberOfPOS;
                return returnValue;
            }
            set
            {
                myNumberOfPOS = value;
            }
        }


        public dynamic FC_IP
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myFC_IP;
                return returnValue;
            }
            set
            {
                myFC_IP = value;
            }
        }


        public dynamic FC_UDP_Port
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myFC_UDP_Port;
                return returnValue;
            }
            set
            {
                myFC_UDP_Port = value;
            }
        }


        public dynamic FC_TCP_Port
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myFC_TCP_Port;
                return returnValue;
            }
            set
            {
                myFC_TCP_Port = value;
            }
        }


        public dynamic POS_IP
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myPOS_IP;
                return returnValue;
            }
            set
            {
                myPOS_IP = value;
            }
        }

        public dynamic POS_TCP_Port
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myPOS_TCP_Port;
                return returnValue;
            }
            set
            {
                myPOS_TCP_Port = value;
            }
        }

        public dynamic POS_UDP_Port
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myPOS_UDP_Port;
                return returnValue;
            }
            set
            {
                myPOS_UDP_Port = value;
            }
        }
    }
}
