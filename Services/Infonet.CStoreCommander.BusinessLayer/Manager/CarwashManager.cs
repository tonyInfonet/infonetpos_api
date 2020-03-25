using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.Entities;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    /// <summary>
    /// manager to perform the actions for carwash functionality 
    /// </summary>
    public class CarwashManager: ManagerBase,ICarwashManager
    {
        private readonly IPolicyManager _policyManager;
        private bool _isCodeValid = false;
        private short[] _responseArray = new short[1];
        private int _operation;
        private string _carwashCode;


        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="policyManager"></param>
        public CarwashManager(IPolicyManager policyManager)
        {
            _policyManager = policyManager;
        }

        /// <summary>
        /// method to find out the status of carwash server
        /// </summary>
        /// <returns></returns>
        public bool GetCarwashServerStatus()
        {
            return ConnectToServer();
        }

        /// <summary>
        /// method to get the carwash code at the time of sale of carwash product
        /// </summary>
        /// <returns></returns>
        public bool GetCarwashCode()
        {
          return ProcessCarwash(1);
        }

        /// <summary>
        /// method to validate the carwash code
        /// </summary>
        /// <param name="carwashCode"></param>
        /// <returns></returns>
        public bool ValidateCarwash(string carwashCode)
        {
            _carwashCode = carwashCode;
            var isRequestSent = ProcessCarwash(3);
            if(isRequestSent)
            {
                return _isCodeValid;
            }
            _isCodeValid = false;
            return _isCodeValid;
        }

        /// <summary>
        /// method to void the carwash code at the time of refund of the carwash product
        /// </summary>
        public void RefundCarwash()
        {
            ProcessCarwash(2);
        }

        /// <summary>
        /// method to process the request related to carwash server
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        private bool ProcessCarwash(int operation) //value of operation for (sending the sale request=1/ sending refund request=2/ sending the validate request=3) 
        {
            _operation = operation;
            var returnValue = false;
            Sale_Line saleCount ;
            var request = "";
            var i = (short)1; //refers to the number of products in a sale
            var j = (short)0; //refers to number of carwash products
            var stockArr = new string[1];
            var isRequestSend = false;
            const string xmlInitials = "<? xml version =\"1.0\" encoding=\"utf-8\" ?><FCSCommand>";

            if (operation == 1)
            {
                request = xmlInitials+"<GetCarwashCode><InvoiceID>" + System.Convert.ToString(Chaps_Main.SA.Sale_Num) + "</InvoiceID><Carwash>";

                foreach (Sale_Line saleLine in Chaps_Main.SA.Sale_Lines)
                {
                    saleCount = saleLine;
                    if (saleCount.IsCarwashProduct && saleCount.Quantity > 0 && saleCount.CarwashCode == "")
                    {
                        Array.Resize(ref stockArr, (stockArr.Length - 1) + 1 + 1);
                        Array.Resize(ref _responseArray, (_responseArray.Length - 1) + 1 + 1);
                        stockArr[j] = Chaps_Main.SA.Sale_Lines[i].Stock_Code; // stock code
                        _responseArray[j] = i; // position of the product in a sale line
                        request = request + "<Data><Product ID=\"" + Chaps_Main.SA.Sale_Lines[i].Stock_Code + "\" Description = \"" + Chaps_Main.SA.Sale_Lines[i].Description + "\"></Product>";
                        request = request + "<Qty>" + Convert.ToString(Chaps_Main.SA.Sale_Lines[i].Quantity) + "</Qty>";
                        request = request + "<Price>" + Convert.ToString(Chaps_Main.SA.Sale_Lines[i].price) + "</Price></Data>";
                        j++;
                    }
                   
                    i++;
                }
                request = request + "</Carwash></GetCarwashCode></FCSCommand>";
                
                if (stockArr[0] == "")
                {
                    return false;// return from here if the stock is not available 
                }
                isRequestSend = SendCarwashRequest(request);
                returnValue = isRequestSend;
            }

            if (operation == 2)
            {
                request = xmlInitials+"<CancelCarwashCode><InvoiceID>" + System.Convert.ToString(Chaps_Main.SA.Sale_Num) + "</InvoiceID><Carwash>";

                foreach (Sale_Line saleLine in Chaps_Main.SA.Sale_Lines)
                {
                    saleCount = saleLine;

                    if (saleCount.IsCarwashProduct && saleCount.Quantity < 0 && saleCount.CarwashCode != "")
                    {
                        request = request + "<Data><Code>" + saleCount.CarwashCode + "</Code></Data>";
                        saleCount.CarwashCode = ""; //clear the code since it will be refunded
                    }
                }

                request = request + "</Carwash></CancelCarwashCode></FCSCommand>";

                isRequestSend = SendCarwashRequest(request);
                returnValue = isRequestSend;
            }

            if (operation == 3)
            {
                request = xmlInitials+"<CheckCarwashCode><Code>" + modStringPad.PadL(_carwashCode, (short)5, "0") + "</Code></CheckCarwashCode></FCSCommand>";
                isRequestSend = SendCarwashRequest(request);
                returnValue = isRequestSend;
            }
            
            return returnValue;
          
        }

        /// <summary>
        /// method to send the carwash request to the server
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual bool SendCarwashRequest(string request)
        {
            var returnValue = false;
            var responce = new byte[2048];
            var xmlRenamed = new XML(_policyManager);
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var ip = _policyManager.CarwashIP;
            var port = _policyManager.CarwashPort;
            if (ip != null) 
            {
                var ipAddress = IPAddress.Parse(ip);
                var remoteEndPoint = new IPEndPoint(ipAddress, port);
                try
                {
                    socket.Connect(remoteEndPoint);
                    if (socket.Connected)
                    {
                        var msg = Encoding.ASCII.GetBytes(request);
                        socket.Send(msg);
                        Task.Delay(500);
                        var bytesRec = socket.Receive(responce);
                        var strBuffer = Encoding.ASCII.GetString(responce, 0, bytesRec);
                        if (_operation == 1)
                        {
                            xmlRenamed.AnalyzeCarwashCodeResponse(strBuffer, _responseArray);
                        }
                        else
                        {
                            xmlRenamed.AnalyzeCarwashResponse(strBuffer);
                            var validity = xmlRenamed.getValidity();
                            if (validity == "VALIDCODE")
                            {
                                _isCodeValid = true;
                            }
                        }
                        returnValue = true;
                    }
                    else
                    {
                        goto err1;
                    }
                }
                catch (Exception ex)
                {
                    WriteToLogFile("SendCarwashRequest method is throwing this exception" + ex.ToString());
                    goto err1;
                }
             }
            return returnValue;

            err1:
            Chaps_Main.SA.IsCarwashProcessed = false;
            return false;
        }

        /// <summary>
        /// method to connect to the server
        /// </summary>
        /// <returns></returns>
        private bool ConnectToServer()
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var ip = _policyManager.CarwashIP;
            var port = _policyManager.CarwashPort;

            if (ip != null)
            {
                var ipAddress = IPAddress.Parse(ip);
                var remoteEndPoint = new IPEndPoint(ipAddress, port);
                try
                {
                    var result = socket.BeginConnect(remoteEndPoint, null, null);

                    var success = result.AsyncWaitHandle.WaitOne(_policyManager.CarwashTout*1000 , true);

                    return socket.Connected;
                }
                catch(Exception ex)
                {
                    WriteToLogFile("ConnectToServer method is throwing this exception" + ex.ToString());
                    return false;
                }
            }
            return false;
        }
    }

}

