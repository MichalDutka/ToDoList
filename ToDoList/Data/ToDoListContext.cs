using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoList.Models;

namespace ToDoList.Data
{
    public class ToDoListContext : DbContext
    {
        public ToDoListContext(DbContextOptions<ToDoListContext> options)
            : base(options)
        {
        }

        public DbSet<T> GetDbSet<T>() where T : class
        {
            if (typeof(T) == typeof(UserTask))
            {
                return Tasks as DbSet<T>;
            }
            else if (typeof(T) == typeof(TaskGroup))
            {
                return Groups as DbSet<T>;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public DbSet<UserTask> Tasks { get; set; }
        public DbSet<TaskGroup> Groups { get; set; }
    }
}
