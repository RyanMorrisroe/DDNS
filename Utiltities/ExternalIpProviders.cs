namespace DDNS.Utilities
{
    public static class ExternalIpProviders
    {
        public static IEnumerable<string> Providers { get; private set; }

        static ExternalIpProviders()
        {
            Providers = new List<string>()
            {
                "https://icanhazip.com/",
                "https://whatismyip.akamai.com",
            };
        }
    }
}
