using CryptoAlert.Api.Models.Requests;
using CryptoAlert.Api.Models.Responses;

namespace CryptoAlert.Api.Services;

public interface IAlertService
{
    Task<AlertResponse> CreateAsync(CreateAlertRequest request, CancellationToken cancellationToken);
    Task<AlertResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<AlertResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
    Task<AlertResponse?> DeactivateAsync(int id, CancellationToken cancellationToken);
}