using Microsoft.Extensions.Options;
using Practice.Client;

namespace ShopMVC {
    public class PracticeClientFactoryConfigOptions {
        public string? ApiUrl { get; set; }
    }

    public interface IPracticeClientFactory {
        public PracticeClient CreateClient();
    }

    public class PracticeClientFactory: IPracticeClientFactory {

        private readonly PracticeClientFactoryConfigOptions _config;
        private readonly HttpClient _httpClient;
        public PracticeClientFactory(
            IHttpClientFactory httpClientFactory,
            IOptions<PracticeClientFactoryConfigOptions> configureOptions
        ) {
            _config = configureOptions.Value;
            _httpClient = httpClientFactory.CreateClient();
        }

        public PracticeClient CreateClient() {
            return new PracticeClient(_config.ApiUrl, _httpClient);
        }
    }
    public static class PracticeClientExtensions {
        public static IServiceCollection AddPracticeClient(this IServiceCollection services, Action<PracticeClientFactoryConfigOptions> configureOptions) {
            services.Configure(configureOptions);
            services.AddSingleton<IPracticeClientFactory, PracticeClientFactory>();
            return services;
        }
    }
}
