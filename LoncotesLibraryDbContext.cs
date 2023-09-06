using Microsoft.EntityFrameworkCore;
using LoncotesLibrary.Models;

public class LoncotesLibraryDbContext : DbContext
{
    public DbSet<Checkout> Checkouts { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Material> Materials { get; set; }
    public DbSet<MaterialType> MaterialTypes { get; set; }
    public DbSet<Patron> Patrons { get; set; }

    public LoncotesLibraryDbContext(DbContextOptions<LoncotesLibraryDbContext> context) : base(context)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //  Seed data
        modelBuilder.Entity<MaterialType>().HasData(new MaterialType[]
        {
            new MaterialType {Id = 1, Name = "Paperback Book", CheckoutDays = 12},
            new MaterialType {Id = 2, Name = "Periodical", CheckoutDays = 5},
            new MaterialType {Id = 3, Name = "DVD", CheckoutDays = 4},
            new MaterialType {Id = 4, Name = "CD", CheckoutDays = 4},
            new MaterialType {Id = 5, Name = "Hardcover Book", CheckoutDays = 9}
        });

        modelBuilder.Entity<Patron>().HasData(new Patron[]
        {
            new Patron {Id = 1, FirstName = "Josh", LastName = "Baugh", Address = "145 East Ocean Drive", Email = "joshb@gmail.com", IsActive = true},
            new Patron {Id = 2, FirstName = "Liza", LastName = "Vavrichyna", Address = "963 Highland St", Email = "lizav@gmail.com", IsActive = true},
            new Patron {Id = 3, FirstName = "JD", LastName = "Fitzmartin", Address = "2144 Fairview Lane", Email = "jdfitz@gmail.com", IsActive = true},
            new Patron {Id = 4, FirstName = "Greg", LastName = "Korte", Address = "451 Highway Ave", Email = "gregk@gmail.com", IsActive = true}
        });

        modelBuilder.Entity<Genre>().HasData(new Genre[]
        {
            new Genre {Id = 1, Name = "Fiction"},
            new Genre {Id = 2, Name = "Science Fiction"},
            new Genre {Id = 3, Name = "Nonfiction"},
            new Genre {Id = 4, Name = "Romance"},
            new Genre {Id = 5, Name = "Autobiography"},
            new Genre {Id = 6, Name = "Biography"},
            new Genre {Id = 7, Name = "News"},
            new Genre {Id = 8, Name = "Alternative"}
        });

        modelBuilder.Entity<Material>().HasData(new Material[]
        {
            new Material {Id = 1, MaterialName = "Cat's Cradle", MaterialTypeId = 1, GenreId = 2},
            new Material {Id = 2, MaterialName = "Good Omens", MaterialTypeId = 5, GenreId = 1},
            new Material {Id = 3, MaterialName = "The Count of Monte Cristo", MaterialTypeId = 1, GenreId = 1},
            new Material {Id = 4, MaterialName = "The New York Times", MaterialTypeId = 2, GenreId = 7},
            new Material {Id = 5, MaterialName = "Tron: Legacy", MaterialTypeId = 3, GenreId = 2},
            new Material {Id = 6, MaterialName = "OK Computer", MaterialTypeId = 4, GenreId = 8},
            new Material {Id = 7, MaterialName = "La La Land", MaterialTypeId = 3, GenreId = 4},
            new Material {Id = 8, MaterialName = "Unbroken", MaterialTypeId = 5, GenreId = 6},
            new Material {Id = 9, MaterialName = "Trench", MaterialTypeId = 4, GenreId = 8},
            new Material {Id = 10, MaterialName = "The Storyteller", MaterialTypeId = 5, GenreId = 5},
        });

        modelBuilder.Entity<Checkout>().HasData(new Checkout[]
        {
            new Checkout {Id = 6, MaterialId = 13, PatronId = 4, CheckoutDate = DateTime.Parse("2023-08-10T15:40:34.037788")},
            new Checkout {Id = 7, MaterialId = 5, PatronId = 3, CheckoutDate = DateTime.Parse("2023-08-16T15:40:34.037788")},
            new Checkout {Id = 8, MaterialId = 6, PatronId = 1, CheckoutDate = DateTime.Parse("2023-08-12T15:40:34.037788")}
        });
    }
}