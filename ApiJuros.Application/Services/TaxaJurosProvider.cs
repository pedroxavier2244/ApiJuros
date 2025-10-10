using ApiJuros.Application.DTOs;
using ApiJuros.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Json;

namespace ApiJuros.Application.Services
{
    public class TaxaJurosProvider : ITaxaJurosProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TaxaJurosProvider> _logger;
        private const string BcbApiUrl = "https://api.bcb.gov.br/dados/serie/bcdata.sgs.432/dados/ultimos/1?formato=json";

        public TaxaJurosProvider(HttpClient httpClient, ILogger<TaxaJurosProvider> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<decimal> GetTaxaJurosAtualAsync()
        {
            _logger.LogInformation("Iniciando busca da taxa de juros na API do BCB.");

            try
            {
                var bcbResponse = await _httpClient.GetFromJsonAsync<BcbApiResponse[]>(BcbApiUrl);

                if (bcbResponse is null || bcbResponse.Length == 0)
                {
                    _logger.LogWarning("A API do BCB retornou uma resposta vazia ou nula.");
                    throw new Exception("Não foi possível obter a taxa de juros. A resposta do serviço externo foi vazia.");
                }

                var taxaJuros = bcbResponse[0].valor;
                _logger.LogInformation("Taxa de juros obtida com sucesso: {TaxaJuros}%", taxaJuros);

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