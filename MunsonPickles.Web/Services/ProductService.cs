using Microsoft.EntityFrameworkCore;
using MunsonPickles.Web.Data;
using MunsonPickles.Web.Models;

namespace MunsonPickles.Web.Services;

public class ProductService
{
    private readonly PickleDbContext _db;

    public ProductService(PickleDbContext context)
    {
        _db = context;
    }

    public async Task<IEnumerable<Product>> GetAllProducts()
    {
        return await _db
            .Products
            .Include(p => p.ProductType)
            .AsNoTracking()
            .ToListAsync();          
    }

    public async Task<Product?> GetProductById(int productId)
    {
        return await _db.Products.Where(p => p.Id == productId).AsNoTracking().FirstOrDefaultAsync();
    }
}