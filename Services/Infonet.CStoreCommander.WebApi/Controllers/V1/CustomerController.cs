using Infonet.CStoreCommander.BusinessLayer.Manager;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Logging;
using Infonet.CStoreCommander.Resources;
using Infonet.CStoreCommander.WebApi.Filters;
using Infonet.CStoreCommander.WebApi.Resources;
using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using Infonet.CStoreCommander.WebApi.Models.Common;
using Infonet.CStoreCommander.WebApi.Models.Customer;
using Swashbuckle.Swagger.Annotations;
using Infonet.CStoreCommander.WebApi.Models.Sale;
using Infonet.CStoreCommander.WebApi.Utilities;
using Infonet.CStoreCommander.WebApi.Mapper;

namespace Infonet.CStoreCommander.WebApi.Controllers.V1
{
    /// <summary>
    /// Controller for Customer
    /// </summary>
    [RoutePrefix("api/v1/customer")]
    [ApiAuthorization]
    public class CustomerController : ApiController
    {
        private readonly ICustomerManager _customerManager;
        private readonly ISaleManager _saleManager;
        private readonly IApiResourceManager _resourceManager;
        private readonly ILog _performancelog = LoggerManager.PerformanceLogger;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="customerManager"></param>
        /// <param name="saleManager"></param>
        /// <param name="resourceManager"></param>
        public CustomerController(ICustomerManager customerManager, ISaleManager saleManager, IApiResourceManager resourceManager)
        {
            _customerManager = customerManager;
            _saleManager = saleManager;
            _resourceManager = resourceManager;
        }

        /// <summary>
        /// Get list of all customers
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of customers</returns>
        [Route("")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<CustomerResponseModel>))]
        public HttpResponseMessage Index(int pageIndex = 1, int pageSize = 100)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CustomerController,Index,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            var customers = _customerManager.GetCustomers(pageIndex, pageSize);

            if (customers.Count == 0 && (pageIndex == 1 || pageIndex == 0))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                {
                    Error = new MessageStyle
                    {
                        Message = Resource.CustomerNotDefined,
                        MessageType = MessageType.OkOnly
                    }
                });
            }
            var customerResponseModel = GetCustomerResponseModels(customers);
            _performancelog.Debug($"End,CustomerController,Index,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, customerResponseModel);
        }

        /// <summary>
        /// Get a customer by customer code
        /// </summary>
        /// <param name="code">Customer code</param>
        /// <returns>Response message</returns>
        [Route("getByCode")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(LoyaltyCustomerResponseModel))]
        public HttpResponseMessage GetCustomerByCode(string code)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CustomerController,GetCustomerByCode,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            if (string.IsNullOrEmpty(code))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                {
                    Error = new MessageStyle
                    {
                        Message = Resource.CodeRequired,
                        MessageType = MessageType.OkOnly
                    }
                });
            }

            var customer = _customerManager.GetCustomerByCode(code);
            if (customer == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                {
                    Error = new MessageStyle
                    {
                        Message = Resource.CustomerNotExists,
                        MessageType = MessageType.OkOnly
                    }
                });
            }

            var response = GetLoyaltyCustomerResponseModel(customer);
            _performancelog.Debug($"End,CustomerController,GetCustomerByCode,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        /// <summary>
        /// Get a customer by customer code
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Response message</returns>
        [Route("getByCard")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(LoyaltyCustomerResponseModel))]
        public HttpResponseMessage GetCustomerByCard([FromBody]SearchBycardModel model)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CustomerController,GetCustomerByCard,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            MessageStyle message;
            if (string.IsNullOrEmpty(model.CardNumber))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                {
                    Error = new MessageStyle
                    {
                        Message = Resource.CardRequired,
                        MessageType = MessageType.OkOnly
                    }
                });
            }
            var customer = _customerManager.SearchCustomerCard(model.CardNumber, model.IsLoyaltycard, out message);
           
            if (!string.IsNullOrEmpty(message.Message))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                {
                    Error = new MessageStyle
                    {
                        Message = message.Message,
                        MessageType = MessageType.OkOnly
                    }
                });
            }
            if (customer == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                {
                    Error = new MessageStyle
                    {
                        Message = Resource.CustomerNotExists,
                        MessageType = MessageType.OkOnly
                    }
                });
            }

            string userCode;
            ErrorMessage error = new ErrorMessage();
            HttpResponseMessage httpResponseMessage;
            if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
            var sale = _saleManager.SetCustomer(customer.Code, model.SaleNumber, model.TillNumber,
                        userCode, 0, model.CardNumber, out error);
            if (!string.IsNullOrEmpty(error.MessageStyle.Message))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                {
                    Error = error.MessageStyle
                });
            }
            if (sale != null)
            {
                var editLines = _saleManager.CheckEditOptions(sale.Sale_Lines, userCode);
                var enableButtons = _saleManager.EnableCashButton(sale, userCode);
                var userCanWriteOff = _saleManager.EnableWriteOffButton(userCode);
                var saleModel = SaleMapper.CreateSaleModel(sale, editLines, enableButtons, userCanWriteOff);
            }

            var customerModel = GetLoyaltyCustomerResponseModel(customer);
            _performancelog.Debug($"End,CustomerController,GetCustomerByCard,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, customerModel);
        }

        /// <summary>
        /// Search customers
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of customers</returns>
        [Route("search")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<CustomerResponseModel>))]
        [SwaggerResponse(HttpStatusCode.Accepted, Type = typeof(SaleModel))]
        public HttpResponseMessage SearchCustomer(string searchTerm, int tillNumber, int saleNumber, int pageIndex = 1, int pageSize = 100)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CustomerController,SearchCustomer,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            int totalRecords;
            var customers = _customerManager.Search(searchTerm, out totalRecords, pageIndex, pageSize);
            if (totalRecords == -999)
            {
                string userCode;
                HttpResponseMessage httpResponseMessage;
                if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
                var firstOrDefault = customers.FirstOrDefault();
                if (firstOrDefault != null)
                {
                    var code = firstOrDefault.Code;
                    ErrorMessage error;
                    var sale = _saleManager.SetCustomer(code, saleNumber, tillNumber,
                        userCode, 0, searchTerm, out error);
                    if (!string.IsNullOrEmpty(error.MessageStyle.Message))
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                        {
                            Error = error.MessageStyle
                        });
                    }
                    if (sale != null)
                    {
                        var editLines = _saleManager.CheckEditOptions(sale.Sale_Lines, userCode);
                        var enableButtons = _saleManager.EnableCashButton(sale, userCode);
                        var userCanWriteOff = _saleManager.EnableWriteOffButton(userCode);
                        var saleModel = SaleMapper.CreateSaleModel(sale, editLines, enableButtons, userCanWriteOff);
                        return Request.CreateResponse(HttpStatusCode.Accepted, saleModel);
                    }
                }
            }
            if (totalRecords == 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                {
                    Error = new MessageStyle
                    {
                        Message = Resource.CustomerNotExists,
                        MessageType = MessageType.OkOnly
                    }
                });
            }

            var customerList = GetCustomerResponseModels(customers);
            _performancelog.Debug($"End,CustomerController,SearchCustomer,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, customerList);
        }

        /// <summary>
        /// Get list of all loyalty customers
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of customers</returns>
        [Route("loyalty")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<LoyaltyCustomerResponseModel>))]
        public HttpResponseMessage GetLoyaltyCustomers(int pageIndex = 1, int pageSize = 100)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CustomerController,GetLoyaltyCustomers,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            var store = CacheManager.GetStoreInfo();
            var offSet = store?.OffSet ?? 0;
            var customers = _customerManager.GetLoyaltyCustomers(pageIndex, pageSize);

            if (customers.Count == 0 && (pageIndex == 1 || pageIndex == 0))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                {
                    Error = _resourceManager.CreateMessage(offSet, 16, 89, null)
                });
            }

            var listCustomerModel = GetLoyaltyCustomerResponseModels(customers);

            _performancelog.Debug($"End,CustomerController,GetLoyaltyCustomers,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, listCustomerModel);
        }


        /// <summary>
        /// Search loyalty customers
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number for the sale</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of customers</returns>
        [Route("loyalty/search")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<LoyaltyCustomerResponseModel>))]
        [SwaggerResponse(HttpStatusCode.Accepted, Type = typeof(SaleModel))]
        public HttpResponseMessage SearchLoyaltyCustomer(string searchTerm, int tillNumber, int saleNumber, int pageIndex = 1, int pageSize = 100)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CustomerController,SearchLoyaltyCustomer,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            int totalResults;
            var customers = _customerManager.SearchLoyaltyCustomer(searchTerm, out totalResults, pageIndex, pageSize);

            if (totalResults == -999)
            {
                string userCode;
                HttpResponseMessage httpResponseMessage;
                if (GetUserCode(out userCode, out httpResponseMessage)) return httpResponseMessage;
                var firstOrDefault = customers.FirstOrDefault();
                if (firstOrDefault != null)
                {
                    var code = firstOrDefault.Code;
                    ErrorMessage error;
                    var sale = _saleManager.SetCustomer(code, saleNumber, tillNumber,
                        userCode, 0, searchTerm, out error);
                    if (!string.IsNullOrEmpty(error.MessageStyle.Message))
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                        {
                            Error = error.MessageStyle
                        });
                    }
                    if (sale != null)
                    {
                        var editLines = _saleManager.CheckEditOptions(sale.Sale_Lines, userCode);
                        var enableButtons = _saleManager.EnableCashButton(sale, userCode);
                        var userCanWriteOff = _saleManager.EnableWriteOffButton(userCode);
                        var saleModel = SaleMapper.CreateSaleModel(sale, editLines, enableButtons, userCanWriteOff);
                        return Request.CreateResponse(HttpStatusCode.Accepted, saleModel);
                    }
                }
            }

            if (totalResults == 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                {
                    Error = new MessageStyle
                    {
                        Message = Resource.CustomerNotExists,
                        MessageType = MessageType.OkOnly
                    }
                });
            }

            var listCust = GetLoyaltyCustomerResponseModels(customers);
            _performancelog.Debug($"End,CustomerController,SearchLoyaltyCustomer,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(HttpStatusCode.OK, listCust);
        }


        /// <summary>
        /// Add customer 
        /// </summary>
        /// <param name="customerModel">Search term</param>
        /// <returns>Success/Failure</returns>
        [Route("loyalty/add")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SuccessReponse))]
        public HttpResponseMessage AddCustomer([FromBody]LoyaltyCustomerResponseModel customerModel)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CustomerController,AddCustomer,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            const MessageType criticalMessageType = (int)MessageType.Critical + MessageType.OkOnly;

            HttpResponseMessage addCustomer;
            if (VerifyCustomerAddRequest(customerModel, criticalMessageType, out addCustomer)) return addCustomer;

            var customer = new Customer
            {
                Name = customerModel.Name,
                Code = customerModel.Code,
                Loyalty_Points = Convert.ToDouble(customerModel.LoyaltyPoints),
                Phone = customerModel.Phone,
                Loyalty_Code = customerModel.LoyaltyNumber
            };

            var result = _customerManager.SaveCustomer(customer);
            var successReponse = new SuccessReponse
            {
                Success = result
            };
            _performancelog.Debug($"End,CustomerController,AddCustomer,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            return Request.CreateResponse(result ? HttpStatusCode.OK : HttpStatusCode.Conflict, successReponse);
        }


        /// <summary>
        /// Set customer as loyalty customer
        /// </summary>
        /// <param name="customerModel">Search term</param>
        /// <returns>Success/Failure</returns>
        [Route("loyalty/set")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Forbidden, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SuccessReponse))]
        public HttpResponseMessage SetLoyaltyCustomer([FromBody]LoyaltyCustomerResponseModel customerModel)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CustomerController,SetLoyaltyCustomer,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");
            ErrorMessage error;


            var customer = new Customer
            {
                Code = customerModel.Code,
                Loyalty_Code = customerModel.LoyaltyNumber
            };

            var result = _customerManager.SetLoyaltyCustomer(customer, out error);

            _performancelog.Debug($"End,CustomerController,SetLoyaltyCustomer,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");
            var success = new SuccessReponse { Success = result };
            if (result)
            {
                return Request.CreateResponse(HttpStatusCode.OK, success);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
            {
                Error = error.MessageStyle
            });
        }

        /// <summary>
        /// Get list of all AR customers
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of customers</returns>
        [Route("ar")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<ArCustomerModel>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage GetArCustomers(int pageIndex = 1, int pageSize = 100)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CustomerController,GetArCustomers,{string.Empty},{dateStart:hh.mm.ss.ffffff}");

            ErrorMessage error;
            var customers = _customerManager.GetArCustomers(out error, pageIndex, pageSize);

            if (!string.IsNullOrEmpty(error?.MessageStyle?.Message))
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new ErrorResponse
                {
                    Error = error.MessageStyle
                });
            }

            var arcustomers = GetArCustomers(customers);

            _performancelog.Debug($"End,CustomerController,GetArCustomers,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, arcustomers);
        }

        /// <summary>
        /// Search AR customers
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of customers</returns>
        [Route("arsearch")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<ArCustomerModel>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage SearchArCustomer(string searchTerm, int pageIndex = 1, int pageSize = 100)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CustomerController,SearchArCustomer,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            ErrorMessage error;
            var customers = _customerManager.SearchArCustomer(searchTerm, out error, pageIndex, pageSize);

            if (!string.IsNullOrEmpty(error?.MessageStyle?.Message))
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new ErrorResponse
                {
                    Error = error.MessageStyle
                });
            }

            var arcustomers = GetArCustomers(customers);

            _performancelog.Debug($"End,CustomerController,SearchArCustomer,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, arcustomers);
        }

        /// <summary>
        /// Get an AR customer by customer code
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <returns>Response message</returns>
        [Route("ar/getByCard")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<ArCustomerModel>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ErrorResponse))]
        public HttpResponseMessage GetArCustomerByCard([FromBody]string cardNumber)
        {
            var dateStart = DateTime.Now;
            _performancelog.Debug($"Start,CustomerController,GetArCustomerByCard,{string.Empty},{DateTime.Now:hh.mm.ss.ffffff}");

            ErrorMessage error;
            if (string.IsNullOrEmpty(cardNumber))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                {
                    Error = new MessageStyle
                    {
                        Message = Resource.CardRequired,
                        MessageType = MessageType.OkOnly
                    }
                });
            }
            var customer = _customerManager.GetArCustomerByCardNumber(cardNumber, out error);

            if (!string.IsNullOrEmpty(error?.MessageStyle?.Message))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                {
                    Error = error.MessageStyle
                });
            }

            var customerModel = GetArCustomerModel(customer);
            _performancelog.Debug($"End,CustomerController,GetArCustomerByCard,{DateTime.Now.Subtract(dateStart).TotalMilliseconds},{DateTime.Now:hh.mm.ss.ffffff}");

            return Request.CreateResponse(HttpStatusCode.OK, customerModel);
        }

        #region Private methods

        /// <summary>
        /// Method to get AR customer model
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        private static ArCustomerModel GetArCustomerModel(Customer customer)
        {
            var customerModel = new ArCustomerModel
            {
                Code = customer.Code,
                Name = customer.Name,
                Phone = customer.Phone,
                Balance = customer.Current_Balance.ToString(Constants.CurrencyFormat),
                CreditLimit = customer.Credit_Limit.ToString(Constants.CurrencyFormat)
            };
            return customerModel;
        }

        /// <summary>
        /// Check Valid Code
        /// </summary>
        /// <param name="customerCode"></param>
        /// <returns></returns>
        private static bool IsValidCustomerCode(string customerCode)
        {
            var myRegExp = new Regex("^[A-Za-z0-9]+$");
            var returnValue = Convert.ToBoolean(myRegExp.IsMatch(customerCode));
            return returnValue;
        }

        /// <summary>
        /// Add customer validations
        /// </summary>
        /// <param name="customerModel">Customer model</param>
        /// <param name="criticalMessageType">Message type</param>
        /// <param name="addCustomer">Add customer</param>
        /// <returns></returns>
        private bool VerifyCustomerAddRequest(LoyaltyCustomerResponseModel customerModel, MessageType criticalMessageType,
        out HttpResponseMessage addCustomer)
        {
            addCustomer = null;
            var store = CacheManager.GetStoreInfo();
            var offSet = store?.OffSet ?? 0;
            if (string.IsNullOrEmpty(customerModel.Code))
            {
                {
                    addCustomer = Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                    {
                        Error = _resourceManager.CreateMessage(offSet, 0, 8108, null, criticalMessageType)
                    });
                    return true;
                }
            }
            if (!IsValidCustomerCode(customerModel.Code))
            {
                {
                    addCustomer = Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                    {
                        Error = _resourceManager.CreateMessage(offSet, 0, 6581, null, criticalMessageType)
                    });
                    return true;
                }
            }
            if (customerModel.Code.Length > 10)
            {
                {
                    addCustomer = Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                    {
                        Error = _resourceManager.CreateMessage(offSet, 16, 90, null, criticalMessageType)
                    });
                    return true;
                }
            }
            if (string.IsNullOrEmpty(customerModel.LoyaltyNumber) || customerModel.LoyaltyNumber.Equals("0"))
            {
                {
                    addCustomer = Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                    {
                        Error = _resourceManager.CreateMessage(offSet, 16, 94, null, MessageType.OkOnly)
                    });
                    return true;
                }
            }
            if (string.IsNullOrEmpty(customerModel.Name))
            {
                {
                    addCustomer = Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                    {
                        Error = _resourceManager.CreateMessage(offSet, 16, 96, null, MessageType.OkOnly)
                    });
                    return true;
                }
            }

            if (_customerManager.IsCustomerExist(customerModel.Code))
            {
                {
                    addCustomer = Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                    {
                        Error = _resourceManager.CreateMessage(offSet, 16, 95, null)
                    });
                    return true;
                }
            }

            if (_customerManager.CheckCustomerByLoyaltyNumber(customerModel.LoyaltyNumber))
            {
                {
                    addCustomer = Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse
                    {
                        Error = _resourceManager.CreateMessage(offSet, 16, 93, null, MessageType.OkOnly)
                    });
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Method to get AR customers
        /// </summary>
        /// <param name="customers"></param>
        /// <returns></returns>
        private static List<ArCustomerModel> GetArCustomers(List<Customer> customers)
        {
            var arcustomers = (from customer in customers
                               select new ArCustomerModel
                               {
                                   Code = customer.Code,
                                   Name = customer.Name,
                                   Phone = customer.Phone,
                                   Balance = customer.Current_Balance.ToString(Constants.CurrencyFormat),
                                   CreditLimit = customer.Credit_Limit.ToString(Constants.CurrencyFormat)
                               }).ToList();
            return arcustomers;
        }

        /// <summary>
        /// Method to get loyalty customer modle
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns></returns>
        private static LoyaltyCustomerResponseModel GetLoyaltyCustomerResponseModel(Customer customer)
        {
            var customerModel = new LoyaltyCustomerResponseModel
            {
                Code = customer.Code,
                Name = customer.Name,
                Phone = customer.Phone,
                LoyaltyNumber = customer.Loyalty_Code,
                LoyaltyPoints = Math.Round(customer.Loyalty_Points, 7).ToString(CultureInfo.InvariantCulture)
            };
            return customerModel;
        }

        /// <summary>
        /// Method to get list of customer model
        /// </summary>
        /// <param name="customers"></param>
        /// <returns></returns>
        private static List<CustomerResponseModel> GetCustomerResponseModels(List<Customer> customers)
        {
            var customerList = customers.Select(customer => new CustomerResponseModel
            {
                Code = customer.Code,
                Name = customer.Name,
                Phone = customer.Phone
            }).ToList();
            return customerList;
        }

        /// <summary>
        /// Method to get loyalty customers model
        /// </summary>
        /// <param name="customers"></param>
        /// <returns></returns>
        private static List<LoyaltyCustomerResponseModel> GetLoyaltyCustomerResponseModels(IList<Customer> customers)
        {
            var listCust = customers.Select(customer => new LoyaltyCustomerResponseModel
            {
                Code = customer.Code,
                Name = customer.Name,
                Phone = customer.Phone,
                LoyaltyNumber = customer.Loyalty_Code,
                LoyaltyPoints =
                    customer.Loyalty_Points <= 0d
                        ? string.Empty
                        : Math.Round(customer.Loyalty_Points, 7).ToString(CultureInfo.InvariantCulture)
            }).ToList();
            return listCust;
        }


        /// <summary>
        /// Method to get user code 
        /// </summary>
        /// <param name="userCode">Usercode</param>
        /// <param name="httpResponseMessage">HttpResponse</param>
        /// <returns>True or false</returns>
        private bool GetUserCode(out string userCode, out HttpResponseMessage httpResponseMessage)
        {
            userCode = string.Empty;
            httpResponseMessage = null;
            var accessToken = Request.GetFirstHeaderValueOrDefault<string>("authToken");
            if (accessToken == null)
            {
                {
                    httpResponseMessage = Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        new ErrorResponse
                        {
                            Error = new MessageStyle { Message = Resource.Error, MessageType = 0 }
                        });
                    return true;
                }
            }

            userCode = TokenValidator.GetUserCode(accessToken);
            return false;
        }

        #endregion

    }//end class
}//end namespace
