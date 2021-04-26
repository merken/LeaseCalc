using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using BaGet.Protocol;
using NuGet.Versioning;
using System.IO;
using LeaseCalc.Functions.Models;

namespace LeaseCalc.Functions
{
    public interface INugetServerService
    {
        Task<IEnumerable<NuGetPackage>> GetPackageVersions(string packageId);
        Task<byte[]> DownloadPackage(string packageId, string version);
    }   
}