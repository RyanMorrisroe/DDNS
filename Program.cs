using CloudFlare.Client;
using CloudFlare.Client.Api.Authentication;
using CloudFlare.Client.Api.Zones.DnsRecord;
using CloudFlare.Client.Enumerators;
using DDNS.Utilities;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Text.RegularExpressions;

namespace DDNS
{
    internal partial class Program
    {
        private static IAuthentication? authenticator;

        public static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddUserSecrets<Program>();
            var configurationRoot = builder.Build();

            authenticator = new ApiTokenAuthentication(configurationRoot.GetSection("cloudFlareToken").Value);

            await UpdateDnsAsync();
        }

        public static async Task UpdateDnsAsync()
        {
            using CloudFlareClient client = new(authenticator);
            IPAddress? externalIPAddress = await GetExternalIPAddressAsync() ?? throw new Exception("No IP, no Goldblum!");
            var zones = (await client.Zones.GetAsync()).Result;
            foreach(var zone in zones )
            {
                var records = (await client.Zones.DnsRecords.GetAsync(zone.Id, new DnsRecordFilter { Type = DnsRecordType.A })).Result;
                foreach(var record in records )
                {
                    Console.WriteLine($"Found record {record.Name} in Zone {zone.Name}");
                    if(record.Type == DnsRecordType.A && record.Content != externalIPAddress.ToString())
                    {
                        var modified = new ModifiedDnsRecord
                        {
                            Type = DnsRecordType.A,
                            Name = record.Name,
                            Content = externalIPAddress.ToString()
                        };

                        var updateResult = await client.Zones.DnsRecords.UpdateAsync(zone.Id, record.Id, modified);

                        if(!updateResult.Success)
                        {
                            throw new Exception($"Error updating {record.Name} in {zone.Name} with error: {updateResult.Errors}");
                        }
                        else
                        {
                            Console.WriteLine($"Updated {record.Name} in {zone.Name} to {externalIPAddress.ToString()}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"{record.Name} in {zone.Name}'s IP is already up to date");
                    }
                }
            }
        }

        public static async Task<IPAddress?> GetExternalIPAddressAsync()
        {
            IPAddress? ipAddress = null;
            HttpClient client = new();

            foreach(string provider in ExternalIpProviders.Providers)
            {
                if(ipAddress != null)
                {
                    break;
                }

                HttpResponseMessage response = await client.GetAsync(provider);
                if(!response.IsSuccessStatusCode)
                {
                    continue;
                }

                string ip = await response.Content.ReadAsStringAsync();
                ip = IPRegex().Replace(ip, string.Empty);
                ipAddress = IPAddress.Parse(ip);
            }

            return ipAddress;
        }

        [GeneratedRegex("\\t|\\n|\\r")]
        private static partial Regex IPRegex();
    }
}