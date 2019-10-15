namespace Kmd.Logic.Identity.Examples.DatePublisherService
{
    public class ClientCredentialsConfig
    {
        public string ClientId { get; set; }
        public string Secret { get; set; }
        public string TokenEndpointBaseAddress { get; set; }
        public string TokenEndpoint { get; set; }
        public string Scope { get; set; }
    }
}