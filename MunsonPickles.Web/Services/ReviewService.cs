using System.Text.Json;
using MunsonPickles.Shared.Entities;
using MunsonPickles.Shared.Models;

namespace MunsonPickles.Web.Services;

public class ReviewService
{
    private readonly HttpClient _reviewClient;

    public ReviewService(HttpClient reviewClient, IConfiguration config)
    {
        _reviewClient = reviewClient;
        _reviewClient.BaseAddress = new Uri(config["DownstreamApi:BaseUrl"]!);
    }
    
    public async Task AddReview(string reviewText, List<string> photoUrls, int productId)
    {
        try
        {
            var newReview = new NewReview
            {
                PhotoUrls = photoUrls,
                ProductId = productId,
                ReviewText = reviewText
            };
            
            await _reviewClient.PostAsJsonAsync("/reviews", newReview);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    public async Task<IEnumerable<Review>?> GetReviewsForProduct(int productId)
    {
        return await _reviewClient.GetFromJsonAsync<IEnumerable<Review>>($"/reviews?productId={productId}");
    }

    public async Task<Review?> GetReviewByIdAsync(int reviewId)
    {
        return await _reviewClient.GetFromJsonAsync<Review>($"/reviews/{reviewId}");
    }

    public async Task<IEnumerable<ImageUploadResult>> UploadReviewImages(MultipartFormDataContent content)
    {
        var uploadResults = new List<ImageUploadResult>();
        var response = await _reviewClient.PostAsync("/reviews/uploadimages", content);
        if (!response.IsSuccessStatusCode) return uploadResults;
        
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        await using var responseStream = await response.Content.ReadAsStreamAsync();
        uploadResults = await JsonSerializer.DeserializeAsync<List<ImageUploadResult>>(responseStream, options) ?? new List<ImageUploadResult>();
        return uploadResults;
    }
}