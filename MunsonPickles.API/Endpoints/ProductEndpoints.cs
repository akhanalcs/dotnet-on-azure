using Microsoft.EntityFrameworkCore;
using MunsonPickles.API.Data;
using MunsonPickles.Shared.Entities;
using MunsonPickles.Shared.Models;

namespace MunsonPickles.API.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        var productsRouteGroup = app.MapGroup("/products");
        productsRouteGroup.MapGet("/", GetAllProducts);
        productsRouteGroup.MapGet("/{ProductId:int}", GetProduct);
    }

    private static async Task<IResult> GetAllProducts(PickleDbContext db)
    {
        var products = await db.Products
            .Include(p=>p.ProductType)
            .ToListAsync();
        return TypedResults.Ok(products);
    }

    private static async Task<IResult> GetProduct(int productId, PickleDbContext db)
        => await db.Products.FindAsync(productId)
            is Product product
            ?  TypedResults.Ok(product)
            : TypedResults.NotFound();
}