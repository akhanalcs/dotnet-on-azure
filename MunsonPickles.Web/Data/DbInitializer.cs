using MunsonPickles.Web.Models;

namespace MunsonPickles.Web.Data;

public static class DbInitializer
{
    public static void InitializeDatabase(PickleDbContext db)
    {
        if (db.Products.Any()) return;

        var pickleType = new ProductType { Name = "Pickle" };
        var preserveType = new ProductType { Name = "Preserves" };

        var dillReview = new Review
        {
            Date = DateTime.Now,
            Text = "These pickles pack a punch",
            UserId = "ashish"
        };

        var dillPickles = new Product
        {
            Description = "Deliciously dill",
            Name = "Dill Pickles",
            ProductType = pickleType,
            Reviews = new List<Review> { dillReview }
        };

        var beetReview = new Review
        {
            Date = DateTime.Now,
            Text = "Bonafide best beets",
            UserId = "ashish"
        };

        var pickledBeet = new Product
        {
            Description = "unBeetable",
            Name = "Red Pickled Beets",
            ProductType = pickleType,
            Reviews = new List<Review> { beetReview }
        };

        var preserveReview = new Review
        {
            Date = DateTime.Now,
            Text = "Succulent strawberries making biscuits better",
            UserId = "ashish"
        };

        var strawberryPreserves = new Product
        {
            Description = "Sweet and a treat to make your toast the most",
            Name = "Strawberry Preserves",
            ProductType = preserveType,
            Reviews = new List<Review> { preserveReview }
        };
        
        db.Products.Add(dillPickles);
        db.Products.Add(pickledBeet);
        db.Products.Add(strawberryPreserves);

        db.SaveChanges();
    }
}