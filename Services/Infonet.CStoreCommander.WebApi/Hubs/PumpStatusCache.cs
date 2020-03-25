using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer;
using Infonet.CStoreCommander.BusinessLayer.Manager;
using Infonet.CStoreCommander.Logging;
using Infonet.CStoreCommander.Resources;
using log4net;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Infonet.CStoreCommander.WebApi.Hubs
{
    /// <summary>
    /// 
    /// </summary>
    public class PumpStatusCache : ManagerBase
    {
        private static readonly PumpStatusCache instance
            = new PumpStatusCache();
        private readonly ILog _pumplog = LoggerManager.PumpLogger;
        private readonly ILog _performlog = LoggerManager.PerformanceLogger;

        private IFuelPumpManager _fuelPumpManager;
        private readonly Timer _timer;
        private static Timer _keepAliveTimer;
        private static Thread _th;
        private volatile bool _executing;

        static PumpStatusCache()
        {
            StartKeepAlive();
        }

        private static void StartKeepAlive()
        {
            _keepAliveTimer = new Timer(30000);
            _keepAliveTimer.Elapsed += new ElapsedEventHandler(KeepAliveEvent);
            _keepAliveTimer.AutoReset = true;
            _keepAliveTimer.Start();
        }

        private PumpStatusCache()
        {
            _timer = new Timer { Interval = 100 };
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
            _timer.AutoReset = true;
        }

        private static void KeepAliveEvent(object sender, ElapsedEventArgs e)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<PumpStatusHub>();
            if (hubContext != null)
            {
                hubContext.Clients.All.keepConnectionAlive("KeepAlive");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static PumpStatusCache Instance => instance;

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ReadUdpPort();
        }

        private void ReadDataFromUdpPort()
         {
            _fuelPumpManager = GetFuelPumpManagerObject();
            try
            {
                try
                {
                    UDPAgent.Instance.OpenPort();
                }
                catch (Exception ex)
                {
                }
                try
                {
                    string strPacket = UDPAgent.Instance.ReceiveData();
                    _pumplog.Debug(strPacket);
                    var msg = _fuelPumpManager.ReadUdp(strPacket);
                    if (msg != null)
                    {
                        var hubContext = GlobalHost.ConnectionManager.GetHubContext<PumpStatusHub>();
                        var camelCaseFormatter = new JsonSerializerSettings();
                        camelCaseFormatter.ContractResolver = new CamelCasePropertyNamesContractResolver();
                        var json = JsonConvert.SerializeObject(msg, camelCaseFormatter);
                        _pumplog.Debug(json);
                        hubContext.Clients.All.readUdpData(json);
                        _pumplog.Debug("Data populated");
                     //   WriteUDPData("ReadDataFromUdpPort " + msg.Pumps[2].BasketButtonCaption);
                    
                        //_customlog.Info("UDPREADDATA" + json);
                       // WriteLog11("abc1", json);
                    }
                }
                catch (Exception ex)
                {

                }
            }
            catch (Exception ex)
            {
                WriteToLogFile("SignalR Exception Message: " + ex.Message);
                WriteToLogFile("SignalR Exception StackTrace: " + ex.StackTrace);
            }
        }


        internal void ReadUdpPort()
        {
            if (_executing && _th != null && _th.ThreadState == ThreadState.Running)
                return;

            Task.Run(async () =>
            {
                _executing = true;
                while (_executing)
                {
                    _th = new Thread(ReadDataFromUdpPort);
                    _th.Start();
                    _th.Join();
                }
            });
        }






        private IFuelPumpManager GetFuelPumpManagerObject()
        {
            var promomanager = new PromoManager(
                             new ApiResourceManager(),
                             new PromoService());

            var policyManager = new PolicyManager(
                         new PolicyService(),
                         new LoginService(),
                         new UserService(),
                         promomanager,
                         new DipInputService(),
                         new FuelService());

            var loginManager = new LoginManager(
                            new UtilityService(),
                            new UserService(),
                            new LoginService(),
                            new ApiResourceManager(),
                            new TillService(),
                            new ShiftService(),
                            policyManager);
            var stockManager = new StockManager(new StockService(), policyManager
                , new TaxService(),
                new ApiResourceManager(),
                 loginManager);

            var saleLineManager = new SaleLineManager(
                new ApiResourceManager(),
                policyManager,
                new StockService(),
                new FuelService(),
                new UtilityService(),
                loginManager,
                promomanager,
                stockManager
            );

            var creditCardManager = new CreditCardManager(
                        new CardService(),
                        new ApiResourceManager(),
                        new PolicyManager(
                        new PolicyService(),
                        new LoginService(),
                        new UserService(),
                        promomanager,
                        new DipInputService(), new FuelService()), new TenderService(),
                        new CustomerService(),
                        new CardPromptManager(
                            new CardService(),
                            policyManager)
                        );
            var cashBonusService = new CashBonusService();

            var carwashManager = new CarwashManager(policyManager); 

            var resourceManager = new ApiResourceManager(); 

            var customerManager = new CustomerManager(
                    new CustomerService(),
                    policyManager,
                    new ApiResourceManager(),
                    creditCardManager
                    );

            var teSystemManager = new TeSystemManager(policyManager, new TreatyService(),
                new TaxService(), new TeSystemService(), new FuelPumpService(), new ReasonService(),
                new StockService());
            var treatyManager = new TreatyManager(policyManager, new ApiResourceManager(), new TreatyService(), teSystemManager);

            var prepaymanager = new PrepayManager(new PrepayService(), new ApiResourceManager());

            var wexManager = new WexManager(new WexService(), resourceManager, policyManager, new EncryptDecryptUtilityManager(new EncryptDecryptUtilityService()));

           // var cashBonusManager = new CashBonusManager(cashBonusService, policyManager, loginManager, resourceManager);

            return new FuelPumpManager(
                    new GetPropertyManager(new FuelPumpService()),
                    new FuelPumpService(),
                    policyManager,
                    new SaleManager(
                        policyManager,
                        new SaleService(),
                        new ApiResourceManager(),
                        loginManager,
                        new LoginService(),
                        new StockService(),
                        new UtilityService(),
                        new TillService(),
                        new CustomerService(),
                        new CardService(),
                        new TaxService(),
                        saleLineManager,
                        new SaleHeadManager(
                            new SaleService(),
                            customerManager,
                            policyManager),
                        customerManager,
                        new ReasonService(),
                        new GivexClientManager(
                            new ApiResourceManager(),
                            policyManager),
                        creditCardManager, treatyManager,
                        new EncryptDecryptUtilityManager(new EncryptDecryptUtilityService()),
                        new MainManager(new UtilityService(), policyManager),
                        prepaymanager,
                        carwashManager),
                    saleLineManager,
                    new ApiResourceManager(),
                    new TeSystemManager(
                        new PolicyManager(
                            new PolicyService(),
                            new LoginService(),
                            new UserService(),
                            new PromoManager(
                                new ApiResourceManager(),
                                new PromoService()),
                            new DipInputService(), new FuelService()),
                        new TreatyService(),
                        new TaxService(),
                        new TeSystemService(),
                        new FuelPumpService(),
                        new ReasonService(),
                        new StockService()),
                    new FuelService(),
                    new PrepayManager(
                        new PrepayService(),
                        new ApiResourceManager()),
                    new TillService(),
                    new TillCloseService());
        }
    }
}