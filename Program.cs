using LoncotesLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;


#region Defaults
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//  Allows passing datetimes without time zone data
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

//  Allows our api endpoints to access the database through Entity Framework Core
builder.Services.AddNpgsql<LoncotesLibraryDbContext>(builder.Configuration["LoncotesLibraryDbConnectionString"]);

//  Set the JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
#endregion

#region Endpoints

#region Endpoint--Materials
//  Get all Materials or get Materials by Genre and/or MaterialType
app.MapGet("/api/materials", (LoncotesLibraryDbContext db, int? materialId, int? materialTypeId, int? genreId) =>
{
    var query = db.Materials
        .Include(m => m.Genre)
        .Include(m => m.MaterialType)
        .Include(m => m.Checkouts)
        .Where(m => m.OutOfCirculationSince == null);

    if (materialId.HasValue)
    {
        query = query
            .Where(m => m.Id == materialId);
    }

    if (materialTypeId.HasValue)
    {
        query = query
            .Where(m => m.MaterialTypeId == materialTypeId);
    }

    if (genreId.HasValue)
    {
        query = query
            .Where(m => m.GenreId == genreId);
    }

    return query.ToList();
});

//  Get all available materials
app.MapGet("/api/materials/available", (LoncotesLibraryDbContext db) =>
{
    return db.Materials
        .Where(m => m.OutOfCirculationSince == null)
        .Where(m => m.Checkouts.All(co => co.ReturnDate != null))
        .ToList();
});

//  Get Material by Id
app.MapGet("/api/materials/{id}", (LoncotesLibraryDbContext db, int id) =>
{
    return db.Materials
        .Include(m => m.Genre)
        .Include(m => m.MaterialType)
        .Include(m => m.Checkouts)
        .ThenInclude(co => co.Patron)
        .SingleOrDefault(m => m.Id == id);

    #region First Attempt
    /* return db.Materials
        .Include(m => m.Genre)
        .Include(m => m.MaterialType)
        .Where(m => m.Id == id)
        .Join(
            db.Checkouts,
            m => m.Id,
            c => c.MaterialId,
            (m, c) => new 
            {
                Material = m,
                Checkouts = c
            }
        )
        .Join(db.Patrons,
            c => c.Checkouts.PatronId,
            p => p.Id,
            (c, p) => new
                {
                    c.Material, c.Checkouts, Patron = p
                })
        .ToList(); */
        #endregion
});

//  Add Material
app.MapPost("/api/materials", (LoncotesLibraryDbContext db, Material material) =>
{
    db.Materials.Add(material);
    db.SaveChanges();
    return Results.Created($"/api/materials/{material.Id}", material);
});

//  Remove Material from circulation
app.MapPut("/api/materials/{id}", (LoncotesLibraryDbContext db, int id) =>
{
    Material materialToRemove = db.Materials.SingleOrDefault(material => material.Id == id);
    if (materialToRemove == null)
    {
        return Results.NotFound();
    }
    materialToRemove.OutOfCirculationSince = DateTime.Now;
    db.SaveChanges();
    return Results.NoContent();
});
#endregion

#region Endpoint--MaterialTypes
//  Get all MaterialTypes
app.MapGet("/api/materialtypes", (LoncotesLibraryDbContext db) =>
{
    return db.MaterialTypes.ToList();
});
#endregion

#region Endpoint--Genres
//  Get all Genres
app.MapGet("/api/genres", (LoncotesLibraryDbContext db) =>
{
    return db.Genres.ToList();
});
#endregion

#region Endpoint--Patrons
//  Get all Patrons
app.MapGet("/api/patrons", (LoncotesLibraryDbContext db) =>
{
    return db.Patrons.ToList();
});

//  Get Patron with Checkouts
app.MapGet("/api/patrons/{id}", (LoncotesLibraryDbContext db, int id) =>
{
    return db.Patrons
        .Include(c => c.Checkouts)
        .ThenInclude(co => co.Material)
        .ThenInclude(m => m.MaterialType)
        .SingleOrDefault(p => p.Id == id);
});


//  Update Patron
app.MapPut("/api/patrons/{id}", (LoncotesLibraryDbContext db, int id, Patron patron) =>
{
    Patron foundPatron = db.Patrons.SingleOrDefault(p => p.Id == id);
    if (foundPatron == null)
    {
        return Results.NotFound();
    }
    foundPatron.Address = patron.Address;
    foundPatron.Email = patron.Email;

    db.SaveChanges();
    return Results.NoContent();
});


//  Deactivate Patron
app.MapPut("/api/patrons/{id}/isactive", (LoncotesLibraryDbContext db, int id) =>
{
    Patron patronToDeactivate = db.Patrons.SingleOrDefault(p => p.Id == id);
    if (patronToDeactivate == null)
    {
        return Results.NotFound();
    }
    patronToDeactivate.IsActive = !patronToDeactivate.IsActive;

    db.SaveChanges();
    return Results.NoContent();
});
#endregion

#region Endpoint--Checkouts
//  Get Overdue Checkouts
app.MapGet("/api/checkouts/overdue", (LoncotesLibraryDbContext db) =>
{
    return db.Checkouts
        .Include(p => p.Patron)
        .Include(co => co.Material)
        .ThenInclude(m => m.MaterialType)
        .Where(co =>
            (DateTime.Today - co.CheckoutDate).Days >
            co.Material.MaterialType.CheckoutDays &&
            co.ReturnDate == null)
        .ToList();
});

//  Checkout a Material
app.MapPost("/api/checkouts", (LoncotesLibraryDbContext db, Checkout checkout) =>
{
    try
    {
        checkout.CheckoutDate = DateTime.Now;

        db.Checkouts.Add(checkout);
        db.SaveChanges();
        return Results.Created($"/api/checkouts/{checkout.Id}", checkout);
    }
    catch (DbUpdateException)
    {
        return Results.BadRequest("Invalid data submitted");
    }
});

//  Return a Material
app.MapPut("/api/checkouts/{id}", (LoncotesLibraryDbContext db, int id) =>
{
    Checkout returnedCheckout = db.Checkouts.SingleOrDefault(c => c.Id == id);
    if (returnedCheckout == null)
    {
        return Results.NotFound();
    }
    returnedCheckout.ReturnDate = DateTime.Now;

    db.SaveChanges();
    return Results.NoContent();
});
#endregion

#endregion

app.Run();