using Market.Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Market.Portal.Repository.Contract
{
    public interface INewsRepository
    {
        bool Create(News news);
        bool Update(News news);
        bool Delete(News News);
        List<News> Get();
        News Get(int id);
    }
}
