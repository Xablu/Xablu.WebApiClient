using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BooksQL.API.Entities;
using BooksQL.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BooksQL.API.Controllers
{ 
    [ApiController]
    [Route("[controller]")]
    public class BooksController : ControllerBase
    { 
        private readonly BooksRepository _booksRepository;

        public BooksController(BooksRepository booksRepository)
        {
            _booksRepository = booksRepository;
        }

        [HttpGet]
        public Task<List<Book>> Get()
        {
            return _booksRepository.GetBooks();
        }
    }
}
