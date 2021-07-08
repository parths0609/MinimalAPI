using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinimalAPI
{
    public class ToDoDbContext:DbContext
    {
        public ToDoDbContext(DbContextOptions options) : base(options)
        {
        }

        protected ToDoDbContext()
        {
        }

        public DbSet<Items> ToDoItems { get; set; }
    }
}
