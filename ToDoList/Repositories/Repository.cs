using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoList.Data;
using ToDoList.Models;

namespace ToDoList.Repositories
{
    public class Repository<T> : IDisposable where T : class
    {
        private readonly ToDoListContext dbContext;


        public Repository(ToDoListContext dbContext)
        {
            this.dbContext = dbContext;
        }


        public async Task<IEnumerable<T>> GetAll()
        {
            return await dbContext.GetDbSet<T>().ToListAsync();
        }

        public async Task<T> GetByID(int itemId)
        {
            var userTask = await dbContext.GetDbSet<T>().FindAsync(itemId);

            return userTask;
        }

        public async Task Insert(T item)
        {
            dbContext.GetDbSet<T>().Add(item);
            await Save();
        }

        public async Task Delete(int itemId)
        {
            dbContext.GetDbSet<T>().Remove(await GetByID(itemId));
            await Save();
        }

        public async Task<bool> Update(T item)
        {
            dbContext.GetDbSet<T>().Update(item);

            try
            {
                await Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemExists(item))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }

            return true;
        }

        public async Task Save()
        {
            await dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
        }
        private bool ItemExists(T item)
        {
            return dbContext.GetDbSet<T>().Find() != null;
        }
    }
}
