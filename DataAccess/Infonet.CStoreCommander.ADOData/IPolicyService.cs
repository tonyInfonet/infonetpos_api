using System.Collections.Generic;
using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.ADOData
{
    public interface IPolicyService
    {
        dynamic AddPolicy(string pName);

        /// <summary>
        /// Method to load all policies
        /// </summary>
        /// <returns>List of policies</returns>
        List<Policy> LoadAllPolicies();

        /// <summary>
        /// Method to load all policy can be
        /// </summary>
        /// <returns>List of Policy can be</returns>
        List<PolicyCanbe> LoadAllPolicyCanbe();

        /// <summary>
        /// Method to load all set policies
        /// </summary>
        /// <returns>Lsit of set polices</returns>
        List<PolicySet> LoadAllPolicySet();

        /// <summary>
        /// Set up Policy 
        /// </summary>
        /// <param name="security"></param>
        void SetUpPolicy(Security security);


        /// <summary>
        /// Get All Policies
        /// </summary>
        /// <returns></returns>
        List<BackOfficePolicy> GetAllPolicies();
    }
}