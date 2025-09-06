using DocuWare.Platform.ServerClient;
using EasierDocuware.Interfaces.Internal;
using EasierDocuware.Interfaces.Public;
using EasierDocuware.Models;


namespace EasierDocuware.Services.Internal
{
    /// <summary>
    /// Public service for accessing DocuWare file cabinets.
    /// Acts as a wrapper around the internal file cabinet service.
    /// </summary>
    public class FileCabinetService : IFileCabinetService
    {
        private readonly IFileCabinetServiceInternal _fileCabinetServiceInternal;

        public FileCabinetService(IFileCabinetServiceInternal fileCabinetServiceInternal)
        {
            _fileCabinetServiceInternal = fileCabinetServiceInternal;
        }

        /// <summary>
        /// Retrieves a file cabinet by its ID.
        /// </summary>
        /// <param name="fileCabinetId">The ID of the file cabinet to retrieve.</param>
        /// <returns>A <see cref="ServiceResult{T}"/> containing a <see cref="FileCabinet"/> or an error message if not found.</returns>
        public ServiceResult<FileCabinet> GetFileCabinetById(string fileCabinetId)
        {
            return _fileCabinetServiceInternal.GetFileCabinetById(fileCabinetId);
        }

        /// <summary>
        /// Retrieves all file cabinets asynchronously.
        /// </summary>
        /// <returns>A <see cref="ServiceResult{T}"/> containing a <see cref="List{FileCabinet}"/> or an error message if retrieval fails.</returns>
        public Task<ServiceResult<List<FileCabinet>>> GetFileCabinetsAsync()
        {
            return _fileCabinetServiceInternal.GetFileCabinetsAsync();
        }
    }
}