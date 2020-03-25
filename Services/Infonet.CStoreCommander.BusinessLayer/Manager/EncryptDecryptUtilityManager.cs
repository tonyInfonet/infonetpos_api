using CryptoLightLib;
using Infonet.CStoreCommander.ADOData;
using Microsoft.VisualBasic;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class EncryptDecryptUtilityManager : IEncryptDecryptUtilityManager
    {
        private readonly IEncryptDecryptUtilityService _encryptDecryptUtilityService;
        private readonly CryptoLight _myCryptoCtl;
        private bool _myEncryptEnabled;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="encryptDecryptUtilityService"></param>
        public EncryptDecryptUtilityManager(IEncryptDecryptUtilityService encryptDecryptUtilityService)
        {
            _encryptDecryptUtilityService = encryptDecryptUtilityService;
            // this._myCryptoCtl = CryptoLight
            _myCryptoCtl = new CryptoLight();
            _myCryptoCtl.Password = GetPassword();
        }

        /// <summary>
        /// Method to decrypt
        /// </summary>
        /// <param name="strToDecrypt"></param>
        /// <returns></returns>
        public string Decrypt(string strToDecrypt)
        {
            // _myCryptoCtl.Password = GetPassword();
            string returnValue = "";
            try
            {
                if (!_myEncryptEnabled)
                {
                    returnValue = strToDecrypt;
                    return returnValue;
                }
                //  - EKO decryption is crashing if card is not encrypted- Crash is only with exe- error handling is not happening with exe
                if (strToDecrypt.Length < 24) 
                {
                    returnValue = strToDecrypt;
                    return returnValue;
                }
                if (Information.IsNumeric(strToDecrypt)) 
                {
                    returnValue = strToDecrypt;
                    return returnValue;
                }
                // 
                if (!string.IsNullOrEmpty(strToDecrypt))
                {
                    

                    returnValue = _myCryptoCtl.Encode(_myCryptoCtl.Decrypt(_myCryptoCtl.Decode(strToDecrypt.Trim())), pcfEncodingFormat.pcfBinary);
                }
            }
            catch
            {
                //   If couldn't decrypt it display the passing values
                returnValue = strToDecrypt;
                //Shiny end

            }

            return returnValue;
        }

        /// <summary>
        /// Method to encrypt
        /// </summary>
        /// <param name="strToEncrypt"></param>
        /// <param name="strCardType"></param>
        /// <returns></returns>
        public string Encrypt(string strToEncrypt, string strCardType)
        {
            string returnValue = "";


            try
            {
                //@ Reji 07 Mar 2013 - To mask forcefully; Credit and Debit card numbers
                if (!string.IsNullOrEmpty(strCardType)
                    && !string.IsNullOrEmpty(strToEncrypt) && strToEncrypt.Length > 4)
                {
                    if (strCardType.ToUpper() == "C" || strCardType.ToUpper() == "D")
                    {
                        returnValue = new string('X', strToEncrypt.Length - 4)
                            + strToEncrypt.Substring(strToEncrypt.Length - 4, 4);
                        return returnValue;
                    }
                }

                if (!_myEncryptEnabled)
                {
                    returnValue = strToEncrypt;
                    return returnValue;
                }

                if (!string.IsNullOrEmpty(strToEncrypt))
                {
                    var vtInput = strToEncrypt.Trim();

                    // decrypt the text using Crypto Light Encrypt method

                    object vtOutput = _myCryptoCtl.Encrypt(vtInput);

                    returnValue = _myCryptoCtl.Encode(vtOutput); //pcfHex)

                }
            }
            catch
            {
                //   If couldn't Encrypt it display the passing values
                //     Encrypt = ""
                returnValue = strToEncrypt;
                //Shiny end
            }

            return returnValue;
        }

        #region Private methods

        /// <summary>
        /// Method get password
        /// </summary>
        /// <returns></returns>
        private string GetPassword()
        {
            var password = _encryptDecryptUtilityService.GetPassword();
            if (password == null)
            {
                password = "InfonetPassword";
                SavePassword(password);
            }
            _myEncryptEnabled = _encryptDecryptUtilityService.ClassInitialize();
            var strPassword = password;
            // return strPassword;
            _myCryptoCtl.Password = "InfonetPassword";
            strPassword = Decrypt(strPassword);
            _myCryptoCtl.Password = strPassword;
            return strPassword;
        }

        /// <summary>
        /// Method to save password
        /// </summary>
        /// <param name="strPassword"></param>
        private void SavePassword(string strPassword)
        {
            if (strPassword == null) return;
            // var myEncryptEnabled = _encryptDecryptUtilityService.ClassInitialize();
            var strEncryptPassword = Encrypt(strPassword.Trim(), "");
            _encryptDecryptUtilityService.SavePassword(strEncryptPassword);
        }

        #endregion
    }
}
