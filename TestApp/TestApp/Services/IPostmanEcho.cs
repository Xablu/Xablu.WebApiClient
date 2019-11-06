using System;
using System.Threading.Tasks;
using Refit;
using TestApp.Models;

namespace TestApp.Services
{
    public interface IPostmanEcho
    {
        [Get("/basic-auth")]
        [Headers("Authorization: Basic cG9zdG1hbjpwYXNzd29yZA ==")]
        Task<string> BasicAuth();

        [Get("/oauth1")]
        [Headers("Authorization: OAuth")]
        Task<string> OAuth();

        [Post("/post")]
        [Headers("Authorization: Bearer", "Content-Type: application/json; charset=UTF-8")]
        Task<string> Post([Body(BodySerializationMethod.Json)] string data);

        [Post("/post")]
        [Headers("Authorization: Bearer", "Content-Type: application/json; charset=UTF-8")]
        Task<string> PostObject(PostObject data);
    }
}