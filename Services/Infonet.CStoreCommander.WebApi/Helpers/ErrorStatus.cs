using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Infonet.CStoreCommander.WebApi.Helpers
{
    /// <summary>
    /// Service Status class
    /// </summary>
    [Serializable]
    [DataContract]
    public class ErrorStatus
    {
        #region Public properties.
        /// <summary>
        /// Get/Set property for accessing Error message
        /// </summary>
        [JsonProperty("Error")]
        [DataMember]
        public string Error { get; set; }
        #endregion
    }
}