using Market.Portal.Repository.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Market.Portal.Models;
using Market.Portal.Data;
using Microsoft.Extensions.Logging;

namespace Market.Portal.Repository
{
    public class NewsRepositoryImpl : INewsRepository
    {
        private ApplicationDbContext dbContext;
        private ILogger<NewsRepositoryImpl> logger;

        public NewsRepositoryImpl(ApplicationDbContext dbContext, ILogger<NewsRepositoryImpl> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public bool Create(News news)
        {
            try
            {
                dbContext.News.Add(news);
                dbContext.SaveChanges();
            }catch(Exception e)
            {
                logger.LogError("Create", e);
                return false;
            }

            return true;
        }

        public bool Delete(News news)
        {
            try
            {
                dbContext.News.Remove(news);
                dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                logger.LogError("Create", e);
                return false;
            }

            return true;
        }

        public List<News> Get()
        {
            return dbContext.News.OrderByDescending(n => n.DtCreation).ToList();
        }

        public News Get(int id)
        {
            return dbContext.News.Where(n => n.Id == id).FirstOrDefault();
        }

        public bool Update(News news)
        {
            try
            {
                dbContext.News.Update(news);
                dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                logger.LogError("Create", e);
                return false;
            }

            return true;
        }
    }
}
