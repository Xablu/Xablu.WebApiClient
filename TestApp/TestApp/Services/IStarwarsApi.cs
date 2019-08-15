using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using TestApp.Models.StarWarsAPI;

namespace TestApp.Services
{
    public interface IStarwarsApi
    {
        [Get("/starships")]
        [Headers("Authorization: Bearer")]
        Task GetTask();

        [Get("/starships")]
        [Headers("Authorization: Bearer")]
        Task<Models.StarWarsAPI.ApiResponse<List<Starships>>> GetStarships();
    }

    public interface INewsApi
    {
        [Get("/v2/top-headlines")]
        [Headers("Authorization: Bearer")]
        Task GetTask();

        [Get("/v2/top-headlines")]
        [Headers("Authorization: Bearer")]
        Task<Models.StarWarsAPI.ApiResponse<List<>>> GetStarships();
    }
}