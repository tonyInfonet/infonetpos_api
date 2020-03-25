using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using System.IO;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface IMainManager
    {
        /// <summary>
        /// Method to load device
        /// </summary>
        /// <param name="device">Device</param>
        void LoadDevice(ref Device device);

        /// <summary>
        /// Method to get register according to register number
        /// </summary>
        /// <param name="register">Register</param>
        /// <param name="registerNumber">Register</param>
        void SetRegister(ref Register register, short registerNumber);

        /// <summary>
        /// Method to get sound files
        /// </summary>
        /// <returns>Sound</returns>
        Entities.Sound GetSoundFiles();

        /// <summary>
        /// Method to get device setting using register number
        /// </summary>
        /// <param name="registerNumber">Register number</param>
        /// <param name="error">Error message</param>
        /// <returns>Device setting</returns>
        DeviceSetting GetDeviceSetting(int registerNumber, out ErrorMessage error);

        /// <summary>
        /// Method to format display message
        /// </summary>
        /// <param name="register">Register</param>
        /// <param name="st1">Message first part</param>
        /// <param name="st2">Message second part</param>
        /// <returns></returns>
        CustomerDisplay DisplayMsgLcd(Register register, string st1, string st2);

        /// <summary>
        /// Method to format message entyered
        /// </summary>
        /// <param name="register">Register</param>
        /// <param name="strValNo1">Value 1</param>
        /// <param name="strValNo2">Value 2</param>
        /// <returns></returns>
        string FormatLcdString(Register register, string strValNo1, string strValNo2);


        /// <summary>
        /// Method to get error log
        /// </summary>
        FileStream GetErrorLog();

        /// <summary>
        /// Method to get error log
        /// </summary>
        bool ClearErrorLog();

        /// <summary>
        ///  Method to check whether error present or not
        /// </summary>
        bool CheckErrorLog();
    }
}
