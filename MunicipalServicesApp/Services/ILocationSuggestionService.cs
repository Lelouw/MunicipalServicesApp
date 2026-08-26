using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Services
{
    public interface ILocationSuggestionService
    {
        Task<IReadOnlyList<LocationSuggestion>> SearchAsync(string query, CancellationToken cancellationToken);
    }
}
