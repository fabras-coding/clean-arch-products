using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CleanArch_Products.Application.Idempotency
{
    public interface IIdempotencyStore
    {

        /// <summary>
        /// Atomically reserves a key. Returns true only for the FIRST caller
        /// All concurrent callers with the same key get false until the reservation expires.
        /// </summary>
        /// <param name="key">The key to reserve.</param>
        /// <param name="timeToLive">The duration for which the reservation is valid.</param>
        /// <returns>True if the key was successfully reserved, false otherwise.</returns>
        Task<bool> TryReserveAsync(string key, TimeSpan timeToLive);

        /// <summary>
        /// Overwrites the reservation with the actual HTTP response
        /// </summary>
        /// <param name="key">The key for which the response is being saved.</param>
        /// <param name="statusCode">The HTTP status code of the response.</param>
        /// <param name="body">The body of the HTTP response.</param>
        /// <param name="timeToLive">The duration for which the response is valid.</param>
        Task SaveResponseAsync(string key, int statusCode, string body, TimeSpan timeToLive);

        /// <summary>
        /// Returns the saved response, or null if the key is stil PROCESSING or doesn't exists.
        /// </summary>
        /// <param name="key">The key for which the response is being retrieved.</param>
        Task<(int StatusCode, string Body)?> GetResponseAsync(string key);

        /// <summary>
        /// Deletes the reservation on failure so the client can retry with the same key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task DeleteReservationAsync(string key);

    }
}