using Autofac;
using Autofac.Integration.WebApi;
using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Manager;
using Infonet.CStoreCommander.Resources;
using System.Reflection;
using System.Web.Http.Dependencies;

namespace Infonet.CStoreCommander.WebApi
{
    /// <summary>
    /// Class for Resolving the dependencies
    /// </summary>
    public class DependencyResolver
    {
        /// <summary>
        /// Registers and resolves all the dependencies required
        /// </summary>
        /// <returns></returns>
        public IDependencyResolver RegisterDependenciesAndGetResolver()
        {
            var builder = new ContainerBuilder();

            //builder.RegisterType<Encryption>().As<IEncryption>().InstancePerLifetimeScope();

            //DB Services
            builder.RegisterType<PaymentSourceService>().As<IPaymentSourceService>().InstancePerLifetimeScope();
            builder.RegisterType<FuelDiscountService>().As<IFuelDiscountService>().InstancePerLifetimeScope();
            builder.RegisterType<AckrooService>().As<IAckrooService>().InstancePerLifetimeScope();
            builder.RegisterType<CashBonusService>().As<ICashBonusService>().InstancePerLifetimeScope();
            builder.RegisterType<PolicyService>().As<IPolicyService>().InstancePerLifetimeScope();
            builder.RegisterType<LoginService>().As<ILoginService>().InstancePerLifetimeScope();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerService>().As<ICustomerService>().InstancePerLifetimeScope();
            builder.RegisterType<TillService>().As<ITillService>().InstancePerLifetimeScope();
            builder.RegisterType<ShiftService>().As<IShiftService>().InstancePerLifetimeScope();
            builder.RegisterType<UtilityService>().As<IUtilityService>().InstancePerLifetimeScope();
            builder.RegisterType<SaleService>().As<ISaleService>().InstancePerLifetimeScope();
            builder.RegisterType<StockService>().As<IStockService>().InstancePerLifetimeScope();
            builder.RegisterType<TaxService>().As<ITaxService>().InstancePerLifetimeScope();
            builder.RegisterType<BottleReturnService>().As<IBottleReturnService>().InstancePerLifetimeScope();
            builder.RegisterType<ReasonService>().As<IReasonService>().InstancePerLifetimeScope();
            builder.RegisterType<CardService>().As<ICardService>().InstancePerLifetimeScope();
            builder.RegisterType<FuelService>().As<IFuelService>().InstancePerLifetimeScope();
            builder.RegisterType<TenderService>().As<ITenderService>().InstancePerLifetimeScope();
            builder.RegisterType<GivexService>().As<IGivexService>().InstancePerLifetimeScope();
            builder.RegisterType<SuspendedSaleService>().As<ISuspendedSaleService>().InstancePerLifetimeScope();
            builder.RegisterType<EncryptDecryptUtilityService>().As<IEncryptDecryptUtilityService>().InstancePerLifetimeScope();
            builder.RegisterType<ReturnSaleService>().As<IReturnSaleService>().InstancePerLifetimeScope();
            builder.RegisterType<SaleLineService>().As<ISaleLineService>().InstancePerLifetimeScope();
            builder.RegisterType<PromoService>().As<IPromoService>().InstancePerLifetimeScope();
            builder.RegisterType<ReportService>().As<IReportService>().InstancePerLifetimeScope();
            builder.RegisterType<TreatyService>().As<ITreatyService>().InstancePerLifetimeScope();
            builder.RegisterType<SiteMessageService>().As<ISiteMessageService>().InstancePerLifetimeScope();
            builder.RegisterType<AiteCardHolderService>().As<IAiteCardHolderService>().InstancePerLifetimeScope();
            builder.RegisterType<CashService>().As<ICashService>().InstancePerLifetimeScope();
            builder.RegisterType<TeSystemService>().As<ITeSystemService>().InstancePerLifetimeScope();
            builder.RegisterType<SignatureService>().As<ISignatureService>().InstancePerLifetimeScope();
            builder.RegisterType<DipInputService>().As<IDipInputService>().InstancePerLifetimeScope();
            builder.RegisterType<SaleVendorCouponService>().As<ISaleVendorCouponService>().InstancePerLifetimeScope();
            builder.RegisterType<FuelPumpService>().As<IFuelPumpService>().InstancePerLifetimeScope();
            builder.RegisterType<ThemeService>().As<IThemeService>().InstancePerLifetimeScope();
            builder.RegisterType<PrepayService>().As<IPrepayService>().InstancePerLifetimeScope();
            builder.RegisterType<TaxExemptService>().As<ITaxExemptService>().InstancePerLifetimeScope();
            builder.RegisterType<TillCloseService>().As<ITillCloseService>().InstancePerLifetimeScope();
            builder.RegisterType<MaintenanceService>().As<IMaintenanceService>().InstancePerLifetimeScope();
            builder.RegisterType<KickBackService>().As<IKickBackService>().InstancePerLifetimeScope();
            builder.RegisterType<WexService>().As<IWexService>().InstancePerLifetimeScope();

            //Managers
            builder.RegisterType<PaymentSourceManager>().As<IPaymentSourceManager>().InstancePerLifetimeScope();
            builder.RegisterType<FuelDiscountManager>().As<IFuelDiscountManager>().InstancePerLifetimeScope();
            builder.RegisterType<AckrooManager>().As<IAckrooManager>().InstancePerLifetimeScope();
            builder.RegisterType<CarwashManager>().As<ICarwashManager>().InstancePerLifetimeScope();
            builder.RegisterType<CashBonusManager>().As<ICashBonusManager>().InstancePerLifetimeScope();
            builder.RegisterType<ApiResourceManager>().As<IApiResourceManager>().InstancePerLifetimeScope();
            builder.RegisterType<LoginManager>().As<ILoginManager>().InstancePerLifetimeScope();
            builder.RegisterType<PolicyManager>().As<IPolicyManager>().InstancePerLifetimeScope();
            builder.RegisterType<LoginManager>().As<ILoginManager>().InstancePerLifetimeScope();
            builder.RegisterType<TillManager>().As<ITillManager>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerManager>().As<ICustomerManager>().InstancePerLifetimeScope();
            builder.RegisterType<SaleManager>().As<ISaleManager>().InstancePerLifetimeScope();
            builder.RegisterType<StockManager>().As<IStockManager>().InstancePerLifetimeScope();
            builder.RegisterType<ReasonManager>().As<IReasonManager>().InstancePerLifetimeScope();
            builder.RegisterType<BottleManager>().As<IBottleManager>().InstancePerLifetimeScope();
            builder.RegisterType<TaxManager>().As<ITaxManager>().InstancePerLifetimeScope();
            builder.RegisterType<TenderManager>().As<ITenderManager>().InstancePerLifetimeScope();
            builder.RegisterType<GivexManager>().As<IGivexManager>().InstancePerLifetimeScope();
            builder.RegisterType<SaleLineManager>().As<ISaleLineManager>().InstancePerLifetimeScope();
            builder.RegisterType<SaleHeadManager>().As<ISaleHeadManager>().InstancePerLifetimeScope();
            builder.RegisterType<SuspendedSaleManager>().As<ISuspendedSaleManger>().InstancePerLifetimeScope();
            builder.RegisterType<EncryptDecryptUtilityManager>().As<IEncryptDecryptUtilityManager>().InstancePerLifetimeScope();
            builder.RegisterType<ReturnSaleManager>().As<IReturnSaleManager>().InstancePerLifetimeScope();
            builder.RegisterType<PromoManager>().As<IPromoManager>().InstancePerLifetimeScope();
            builder.RegisterType<PaymentManager>().As<IPaymentManager>().InstancePerLifetimeScope();
            builder.RegisterType<GivexClientManager>().As<IGivexClientManager>().InstancePerLifetimeScope();
            builder.RegisterType<ReceiptManager>().As<IReceiptManager>().InstancePerLifetimeScope();
            builder.RegisterType<TreatyManager>().As<ITreatyManager>().InstancePerLifetimeScope();
            builder.RegisterType<CashManager>().As<ICashManager>().InstancePerLifetimeScope();
            builder.RegisterType<PurchaseListManager>().As<IPurchaseListManager>().InstancePerLifetimeScope();
            builder.RegisterType<PurchaseItemManager>().As<IPurchaseItemManager>().InstancePerLifetimeScope();
            builder.RegisterType<TeSystemManager>().As<ITeSystemManager>().InstancePerLifetimeScope();
            builder.RegisterType<PaymentManager>().As<IPaymentManager>().InstancePerLifetimeScope();
            builder.RegisterType<CardManager>().As<ICardManager>().InstancePerLifetimeScope();
            builder.RegisterType<TeCardholderManager>().As<ITeCardholderManager>().InstancePerLifetimeScope();
            builder.RegisterType<OverLimitManager>().As<IOverLimitManager>().InstancePerLifetimeScope();
            builder.RegisterType<OverrideLimitManager>().As<IOverrideLimitManager>().InstancePerLifetimeScope();
            builder.RegisterType<TaxExemptSaleLineManager>().As<ITaxExemptSaleLineManager>().InstancePerLifetimeScope();
            builder.RegisterType<PriceCheckManager>().As<IPriceCheckManager>().InstancePerLifetimeScope();
            builder.RegisterType<CreditCardManager>().As<ICreditCardManager>().InstancePerLifetimeScope();
            builder.RegisterType<SignatureManager>().As<ISignatureManager>().InstancePerLifetimeScope();
            builder.RegisterType<DipInputManager>().As<IDipInputManager>().InstancePerLifetimeScope();
            builder.RegisterType<CardPromptManager>().As<ICardPromptManager>().InstancePerLifetimeScope();
            builder.RegisterType<SaleVendorCouponManager>().As<ISaleVendorCouponManager>().InstancePerLifetimeScope();
            builder.RegisterType<MainManager>().As<IMainManager>().InstancePerLifetimeScope();
            builder.RegisterType<FuelPumpManager>().As<IFuelPumpManager>().InstancePerLifetimeScope();
            builder.RegisterType<GetPropertyManager>().As<IGetPropertyManager>().InstancePerLifetimeScope();
            builder.RegisterType<PayAtPumpManager>().As<IPayAtPumpManager>().InstancePerLifetimeScope();
            builder.RegisterType<MessageManager>().As<IMessageManager>().InstancePerLifetimeScope();
            builder.RegisterType<PayoutManager>().As<IPayoutManager>().InstancePerLifetimeScope();
            builder.RegisterType<ThemeManager>().As<IThemeManager>().InstancePerLifetimeScope();
            builder.RegisterType<PrepayManager>().As<IPrepayManager>().InstancePerLifetimeScope();
            builder.RegisterType<FuelPrepayManager>().As<IFuelPrepayManager>().InstancePerLifetimeScope();
            builder.RegisterType<FuelPumpNotificationManager>().As<IFuelPumpNotificationManager>().InstancePerLifetimeScope();
            builder.RegisterType<TaxExemptSaleManager>().As<ITaxExemptSaleManager>().InstancePerLifetimeScope();
            builder.RegisterType<PropaneManager>().As<IPropaneManager>().InstancePerLifetimeScope();
            builder.RegisterType<UnCompletePrepayManager>().As<IUnCompletePrepayManager>().InstancePerLifetimeScope();
            builder.RegisterType<TierLevelManager>().As<ITierLevelManager>().InstancePerLifetimeScope();
            builder.RegisterType<TillCloseManager>().As<ITillCloseManager>().InstancePerLifetimeScope();
            builder.RegisterType<MaintenanceManager>().As<IMaintenanceManager>().InstancePerLifetimeScope();
            builder.RegisterType<KickBackManager>().As<IKickBackManager>().InstancePerLifetimeScope();
            builder.RegisterType<WexManager>().As<IWexManager>().InstancePerLifetimeScope();
          //  builder.RegisterType<CommManager>().As<ICommManager>().InstancePerLifetimeScope();
            builder.RegisterType<XMLManager>().As<IXMLManager>().InstancePerLifetimeScope();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            var container = builder.Build();
            return new AutofacWebApiDependencyResolver(container);
        }
    }
}