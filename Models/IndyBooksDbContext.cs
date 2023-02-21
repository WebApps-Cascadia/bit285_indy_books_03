using System;
using Microsoft.EntityFrameworkCore;

namespace IndyBooks.Models
{
    public class IndyBooksDbContext:DbContext
    {
        public IndyBooksDbContext(DbContextOptions<IndyBooksDbContext> options) : base(options)
        {}

        //Define DbSets for Collections representing DB tables
        public DbSet<Book> Books { get; set; }
        public DbSet<Writer> Writers { get; set; }
    }
}
