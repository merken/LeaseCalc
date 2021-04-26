using System;

namespace LeaseCalc.Contract
{
    public interface IConfigurationService
    {
        string GetConfigurationValueForKey(string key);
    }
}