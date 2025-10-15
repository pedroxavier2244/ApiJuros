using ApiJuros.Application.DTOs;
using ApiJuros.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed; 
using Microsoft.Extensions.Logging;
using System;
using System.Globalization; 
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ApiJuros.Application.Services 
{
    public class TaxaJurosProvider : ITaxaJurosProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TaxaJurosProvider> _logger;
        private readonly IDistributedCache _cache;
        private ILogger<TaxaJurosProvider> @object;
        private const string BcbApiUrl = "https://api.bcb.gov.br/dados/serie/bcdata.sgs.432/dados/ultimos/1?formato=json";

        public TaxaJurosProvider(HttpClient httpClient, ILogger<TaxaJurosProvider> logger, IDistributedCache cache)
        {
            _httpClient = httpClient;
            _logger = logger;
            _cache = cache; 
        }

        public TaxaJurosProvider(HttpClient httpClient, ILogger<TaxaJurosProvider> @object)
        {
            _httpClient = httpClient;
            this.@object = @object;
        }

        public async Task<decimal> GetTaxaJurosAtualAsync()
        {
            const string cacheKey = "TaxaJurosAtual";
            string cachedTaxa;

            try
            {
                cachedTaxa = await _cache.GetStringAsync(cacheKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao acessar o cache Redis. Buscando da API diretamente.");
                cachedTaxa = null;
            }

            if (!string.IsNullOrEmpty(cachedTaxa))
            {
                _logger.LogInformation("Taxa de juros obtida do CACHE.");
                return decimal.Parse(cachedTaxa, CultureInfo.InvariantCulture);
            }

            _logger.LogInformation("Cache miss. Iniciando busca da taxa de juros na API do BCB.");

            try
            {
                var bcbResponse = await _httpClient.GetFromJsonAsync<BcbApiResponse[]>(BcbApiUrl);
                if (bcbResponse is null || bcbResponse.Length == 0)
                {
                    _logger.LogWarning("A API do BCB retornou uma resposta vazia ou nula.");
                    throw new Exception("Não foi possível obter a taxa de juros. A resposta do serviço externo foi vazia.");
                }

                var taxaJuros = bcbResponse[0].valor;
                _logger.LogInformation("Taxa de juros obtida com sucesso da API: {TaxaJuros}%", taxaJuros);

                var cacheOptions = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(4)); 

                await _cache.SetStringAsync(cacheKey, taxaJuros.ToString(CultureInfo.InvariantCulture), cacheOptions);
                _logger.LogInformation("Taxa de juros salva no cache.");

                return taxaJuros;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao buscar a taxa de juros na API do BCB.");
                throw;
            }
        }
    }
}