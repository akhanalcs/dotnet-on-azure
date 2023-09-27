using System.Net;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MunsonPickles.API.Data;
using MunsonPickles.API.Models;
using MunsonPickles.Shared.Entities;
using MunsonPickles.Shared.Models;

namespace MunsonPickles.API.Endpoints;

public static class ReviewEndpoints
{
    public static void MapReviewEndpoints(this WebApplication app)
    {
        var reviewsRouteGroup = app.MapGroup("/reviews");
        reviewsRouteGroup.MapGet("/{ReviewId:int}", GetReview);
        //https://localhost:7285/reviews?productId=1
        reviewsRouteGroup.MapGet("/", GetReviewsOfAProduct);
        reviewsRouteGroup.MapPost("/", CreateReview);
        reviewsRouteGroup.MapPost("/uploadimages", UploadReviewImages).DisableAntiforgery();
    }
    
    private static async Task<IResult> GetReviewsOfAProduct(int productId, PickleDbContext db)
        => TypedResults.Ok(await db.Reviews.AsNoTracking().Where(r => r.Product.Id == productId).ToListAsync());

    private static async Task<IResult> GetReview(int reviewId, PickleDbContext db)
        => await db
            .Reviews
            .Include(r => r.Product)
            .Include(r => r.Photos)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == reviewId)
            is Review review
            ? TypedResults.Ok(review)
            : TypedResults.NotFound();

    private static async Task<IResult> CreateReview(NewReview newNewReview, PickleDbContext db)
    {
        var userId = "ashish"; // this will get changed out when we add auth

        try
        {
            // Add photos
            List<ReviewPhoto> photos = new();
            foreach (var photoUrl in newNewReview.PhotoUrls)
            {
                photos.Add(new ReviewPhoto {  PhotoUrl = photoUrl });
            }
            
            // create the new review
            Review review = new()
            {
                Date = DateTime.Now,
                Photos = photos,
                Text = newNewReview.ReviewText,
                UserId = userId
            };

            var product = await db.Products
                .Where(p => p.Id == newNewReview.ProductId)
                .Include(p=>p.Reviews)
                .FirstOrDefaultAsync();

            if (product is null) return TypedResults.BadRequest();

            product.Reviews ??= new List<Review>();

            product.Reviews.Add(review);

            await db.SaveChangesAsync();

            return TypedResults.Created($"/products/{newNewReview.ProductId}/reviews/{review.Id}", review);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            return TypedResults.Problem(ex.Message);
        }
    }

    private static async Task<IResult> UploadReviewImages(IFormFileCollection files,
        PickleDbContext db,
        BlobServiceClient blobServiceClient,
        IOptions<AzureStorageConfigOpts> azStorageConfigOpts)
    {
        var loggedInUser = "ashish"; // this will get changed out when we add auth
        var maxAllowedFiles = 3;
        long maxFileSize = 2097152; // 2097152 bytes = 2MB
        var filesProcessed = 0;
        List<ImageUploadResult> uploadResults = new();

        foreach (var file in files)
        {
            var uploadResult = new ImageUploadResult();
            var untrustedFileName = file.FileName;
            var trustedFileNameForDisplay = WebUtility.HtmlEncode(untrustedFileName);
            uploadResult.FileName = untrustedFileName;

            if (filesProcessed >= maxAllowedFiles)
            {
                uploadResult.ErrorMessage = $"{trustedFileNameForDisplay} not uploaded because the request " +
                                            $"exceeded the allowed number of {maxAllowedFiles} files.";
                continue;
            }
            
            if (file.Length == 0)
            {
                uploadResult.ErrorMessage = $"{trustedFileNameForDisplay} length is 0.";
            }
            else if (file.Length > maxFileSize)
            {
                uploadResult.ErrorMessage = $"{trustedFileNameForDisplay} of {file.Length} bytes is larger than the limit of {maxFileSize} bytes.";
            }
            else
            {
                try
                {
                    var trustedFileNameForStorage = Path.ChangeExtension(Path.GetRandomFileName(), Path.GetExtension(file.FileName));
                    
                    var containerClient = blobServiceClient.GetBlobContainerClient(blobContainerName: loggedInUser);
                    await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob); // Create folder based on the logged in User's name if it's not present

                    //var blobClient = containerClient.GetBlobClient(trustedFileNameForStorage);
                    //await blobClient.UploadAsync(file.OpenReadStream()); // This works as well but I prefer one liner below
                    await containerClient.UploadBlobAsync(trustedFileNameForStorage, file.OpenReadStream());

                    // Update the rest of the fields of uploadResult object
                    // Make note of the url of the file that has been uploaded. You'll need this when you add the review.
                    var imageUrl = $"{azStorageConfigOpts.Value.CdnEndpoint}/{loggedInUser}/{trustedFileNameForStorage}";
                    uploadResult.StoredFileUrl = imageUrl;
                    uploadResult.Uploaded = true;
                    uploadResult.StoredFileName = trustedFileNameForStorage;
                }
                catch (IOException ex)
                {
                    uploadResult.ErrorMessage = $"{trustedFileNameForDisplay} error on upload. {ex.Message}.";
                }
            }

            filesProcessed++;
            uploadResults.Add(uploadResult);
        }
        
        return TypedResults.Created($"/reviews/uploadimages", uploadResults);
    }
}