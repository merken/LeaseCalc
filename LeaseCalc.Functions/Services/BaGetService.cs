using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using BaGet.Protocol;
using NuGet.Versioning;
using System.IO;
using LeaseCalc.Functions.Models;

namespace LeaseCalc.Functions.Services
{
    public class BaGetService : INugetServerService
    {
        private readonly NuGetClient client;
        public BaGetService(NuGetClient client)
        {
            this.client = client;
        }

        public async Task<IEnumerable<NuGetPackage>> GetPackageVersions(string packageId)
        {
            var packages = await client.ListPackageVersionsAsync(packageId, false);
            return packages.Select(p => new NuGetPackage
            {
                Name = packageId,
                Version = ShortVersion(p.Version)
            });
        }

        private Version ShortVersion(Version version) => new Version(version.ToString().Substring(0, version.ToString().LastIndexOf('.')));

        public async Task<byte[]> DownloadPackage(string packageId, string version)
        {
            var package = await client.DownloadPackageAsync(packageId, new NuGetVersion(version));
            using (MemoryStream ms = new MemoryStream())
            {
                package.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}