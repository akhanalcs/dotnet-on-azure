using Microsoft.EntityFrameworkCore;
using MunsonPickles.Web.Data;
using MunsonPickles.Web.Models;

namespace MunsonPickles.Web.Services;

public class ReviewService
{
    private readonly PickleDbContext _db;

    public ReviewService(PickleDbContext context)
    {
        _db = context;
    }

    public async Task AddReview(string reviewText, List<string> photoUrls, int productId)
    {
        var userId = "ashish"; // this will get changed out when we add auth

        try
        {
            // Add photos
            List<ReviewPhoto> photos = new();
            foreach (var photoUrl in photoUrls)
            {
                photos.Add(new ReviewPhoto {  PhotoUrl = photoUrl });
            }
            
            // create the new review
            Review review = new()
            {
                Date = DateTime.Now,
                Photos = photos,
                Text = reviewText,
                UserId = userId
            };

            var product = await _db.Products
                                   .Where(p => p.Id == productId)
                                   .Include(p=>p.Reviews)
                                   .FirstOrDefaultAsync();

            if (product is null)
                return;

            product.Reviews ??= new List<Review>();

            product.Reviews.Add(review);

            await _db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
        }
    }

    public async Task<IEnumerable<Review>> GetReviewsForProduct(int productId)
    {
        return await _db.Reviews.AsNoTracking().Where(r => r.Product.Id == productId).ToListAsync();
    }
    
    public async Task<Review?> GetReviewByIdAsync(int reviewId)
    {
        return await _db
            .Reviews
            .Include(r => r.Product)
            .Include(r => r.Photos)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == reviewId);
    }
}