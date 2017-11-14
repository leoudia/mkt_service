using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Market.Portal.Models;
using Market.Portal.Repository.Contract;

namespace Market.Portal.Controllers
{
    [Produces("application/json")]
    [Route("api/News")]
    public class NewsController : Controller
    {

        // GET: api/News
        [HttpGet]
        public IEnumerable<News> Get([FromServices]INewsRepository repository)
        {
            return repository.Get();
        }

        // GET: api/News/5
        [HttpGet("{id}", Name = "Get")]
        public News Get(int id, [FromServices]INewsRepository repository)
        {
            return repository.Get(id);
        }
        
        // POST: api/News
        [HttpPost]
        public void Post([FromBody]News news, [FromServices]INewsRepository repository)
        {
            repository.Create(news);
        }
        
        // PUT: api/News/5
        [HttpPut("{id}")]
        public void Put(int id, [FromServices]INewsRepository repository)
        {
            repository.Update(new News() { Id = id});
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id, [FromServices]INewsRepository repository)
        {
            repository.Delete(new News() { Id = id });
        }
    }
}
