using Infonet.CStoreCommander.Resources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VB = Microsoft.VisualBasic;
using System.IO;
using System.Web;
using Infonet.CStoreCommander.BusinessLayer.Manager;

namespace Infonet.CStoreCommander.BusinessLayer.Utilities
{
  public class SocketWrench : ManagerBase
    {
        static TcpClient client;

        public SocketWrench()
        {
            
        }
        public string Connect(string strRequest, string ip, short port, short timeout, out ErrorMessage error)
        {
            client = new TcpClient();
            error = null;
            error = new ErrorMessage();
            try
            {
                //IAsyncResult ar = client.BeginConnect(ip, port, null, null);
                //WaitHandle wh = ar.AsyncWaitHandle;

                //if (!ar.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(5), false))
                //{
                //    client.Close();
                //    error.MessageStyle = new MessageStyle
                //    {
                //        Message = "Cannot communicate with loyalty server"
                //         };

               var  timeIN = (float)VB.DateAndTime.Timer;
              //  WriteToLog("Send to Kickback Server: " + strRequest);
                while (true)
                {
                    if (VB.DateAndTime.Timer - timeIN > Convert.ToByte(timeout) || client.Connected)
                    {

                        break;
                    }
                    client.Connect(ip, port);
                 

                }
              
                if (!client.Connected)
                    {
                        
                        error.MessageStyle = new MessageStyle
                        {
                            Message = "Cannot Communicate with the Loyalty server. Points will be earned later."
                        };
                        error.StatusCode = HttpStatusCode.Unauthorized;
                    KickBackManager.ExchangeRate = 0;
                    // connection 

                    return "failed";
                    }
                WriteTokickBackLogFile(" Connected to KickBack Server: " + $"{ip}:{port}");
                WriteTokickBackLogFile(" Send to Kickback Server:: " + strRequest);


                //  client.Connect(ip, port);
                var serverStream = client.GetStream();
                //   byte[] outStream = Encoding.ASCII.GetBytes("<GetRewardsRequest><RequestHeader><PosLoyaltyInterfaceVersion>1.0.0</PosLoyaltyInterfaceVersion><VendorName>InfoNet-Tech</VendorName><VendorModelVersion>Pos 3.00.01</VendorModelVersion><POSSequenceID>40464</POSSequenceID><LoyaltySequenceID/><StoreLocationID>GAS1</StoreLocationID><LoyaltyOfflineFlag value='no'/></RequestHeader><LoyaltyID entryMethod='swipe'>6034311109307726</LoyaltyID></GetRewardsRequest>");
                var outStream = Encoding.ASCII.GetBytes(strRequest);
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();

                var inStream = new byte[client.ReceiveBufferSize];
                serverStream.Read(inStream, 0, client.ReceiveBufferSize);
                string returndata = Encoding.ASCII.GetString(inStream);

         
               
                return returndata;
            }
            catch
           {
               
                error.MessageStyle = new MessageStyle
                {
                    Message = "Cannot Communicate with the Loyalty server. Points will be earned later."
                };
                error.StatusCode = HttpStatusCode.Unauthorized;
                KickBackManager.ExchangeRate = 0;
                return "failed";
                //throw ex;
            }
            finally
            {
                // code in finally block is guranteed 
                // to execute irrespective of 
                // whether any exception occurs or does 
                // not occur in the try block
                client.Close();
            }
        }


    }

}

