using System;
using LeaseCalc.Contract;

namespace LeaseCalc.Functions
{
    public class EnvironmentConfigurationService : IConfigurationService
    {
        public string GetConfigurationValueForKey(string key)
        {
            return System.Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Process);
        }
    }
}