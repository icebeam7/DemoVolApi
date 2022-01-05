namespace DemoVolApi.Context
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options)
        {

        }

        public DbSet<Book> Books => Set<Book>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "James Bond", Year = 2020 },
                new Book { Id = 2, Title = "Untamed", Year = 2021 },
                new Book { Id = 3, Title = "Azure Developer", Year = 2022 }
                );
        }
    }
}