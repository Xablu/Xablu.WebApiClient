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
        Task GetTask();

        [Get("/starships")]
        Task<Models.StarWarsAPI.ApiResponse<List<Starships>>> GetStarships();
    }
}