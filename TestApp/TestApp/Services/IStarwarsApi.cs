using System;
using System.Threading.Tasks;
using Refit;
namespace TestApp.Services
{
    public interface IStarwarsApi
    {
        [Get("starships/")]
        Task GetTask();
    }
}