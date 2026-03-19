using Seasoned.Backend.Models;
using Seasoned.Backend.Data;
using Pgvector;

namespace Seasoned.Backend.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Recipes.Any()) return;

            string testUserId = "0c55c0ed-8080-482d-8ebd-1054cdc3b672";

            var dummyEmbedding = new Vector(new float[768]);

            var recipes = new List<Recipe>
            {
                new Recipe
                {
                    Title = "Miso-Glazed Smashed Burger",
                    UserId = testUserId,
                    ImageUrl = "https://images.unsplash.com/photo-1550547660-d9450f859349?q=80&w=800",
                    Ingredients = new List<string> { "1/2 lb Ground Beef", "1 tbsp White Miso", "Brioche Bun", "Pickled Ginger" },
                    Instructions = new List<string> { "Mix miso into beef.", "Smash thin on high heat.", "Sear until crispy." },
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    Embedding = dummyEmbedding
                },
                new Recipe
                {
                    Title = "Heirloom Tomato Galette",
                    UserId = testUserId,
                    ImageUrl = "https://images.unsplash.com/photo-1595126731003-7337def8d5ee?q=80&w=800",
                    Ingredients = new List<string> { "Puff Pastry", "Heirloom Tomatoes", "Ricotta", "Thyme" },
                    Instructions = new List<string> { "Spread ricotta on pastry.", "Layer tomatoes.", "Bake at 200°C until golden." },
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    Embedding = dummyEmbedding
                },
                new Recipe
                {
                    Title = "Whipped Feta & Hot Honey Toast",
                    UserId = testUserId,
                    ImageUrl = "https://images.unsplash.com/photo-1525351484163-7529414344d8?q=80&w=800",
                    Ingredients = new List<string> { "Sourdough", "Feta Cheese", "Greek Yogurt", "Hot Honey" },
                    Instructions = new List<string> { "Whip feta and yogurt.", "Toast sourdough.", "Spread and drizzle honey." },
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    Embedding = dummyEmbedding
                },
                new Recipe
                {
                    Title = "Thai Basil Pesto Pasta",
                    UserId = testUserId,
                    ImageUrl = "https://images.unsplash.com/photo-1473093226795-af9932fe5856?q=80&w=800",
                    Ingredients = new List<string> { "Linguine", "Thai Basil", "Cashews", "Garlic", "Chili Flakes", "Olive Oil" },
                    Instructions = new List<string> { "Blend basil, cashews, and garlic into a paste.", "Toss with hot pasta.", "Garnish with chili flakes and fresh basil." },
                    CreatedAt = DateTime.UtcNow.AddDays(-4),
                    Embedding = dummyEmbedding
                },
                new Recipe
                {
                    Title = "Smoked Gouda & Broccolini Soup",
                    UserId = testUserId,
                    ImageUrl = "https://images.unsplash.com/photo-1547592166-23ac45744acd?q=80&w=800",
                    Ingredients = new List<string> { "Broccolini", "Smoked Gouda", "Heavy Cream", "Vegetable Broth", "Nutmeg" },
                    Instructions = new List<string> { "Simmer broccolini in broth until tender.", "Blend until smooth.", "Whisk in cream and grated gouda until melted." },
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    Embedding = dummyEmbedding
                },
                new Recipe
                {
                    Title = "Charred Octopus with Romesco",
                    UserId = testUserId,
                    ImageUrl = "https://images.unsplash.com/photo-1551024506-0bccd828d307?q=80&w=800",
                    Ingredients = new List<string> { "Octopus Tentacles", "Roasted Red Peppers", "Almonds", "Sherry Vinegar", "Smoked Paprika" },
                    Instructions = new List<string> { "Blanch octopus then grill over high heat.", "Process peppers, nuts, and vinegar for romesco.", "Serve charred over a bed of sauce." },
                    CreatedAt = DateTime.UtcNow.AddDays(-6),
                    Embedding = dummyEmbedding
                },
                new Recipe
                {
                    Title = "Truffle Honey Fried Chicken",
                    UserId = testUserId,
                    ImageUrl = "https://images.unsplash.com/photo-1562967914-608f82629710?q=80&w=800",
                    Ingredients = new List<string> { "Chicken Thighs", "Buttermilk", "Flour", "Truffle Oil", "Wildflower Honey" },
                    Instructions = new List<string> { "Marinate chicken in buttermilk.", "Dredge and deep fry until 75°C internal.", "Drizzle with truffle-infused honey immediately." },
                    CreatedAt = DateTime.UtcNow.AddDays(-7),
                    Embedding = dummyEmbedding
                },
                new Recipe
                {
                    Title = "Burrata with Roasted Grapes",
                    UserId = testUserId,
                    ImageUrl = "https://images.unsplash.com/photo-1604908176997-125f25cc6f3d?q=80&w=800",
                    Ingredients = new List<string> { "Burrata Cheese", "Red Grapes", "Balsamic Glaze", "Fresh Mint", "Sourdough" },
                    Instructions = new List<string> { "Roast grapes at 200°C until they burst.", "Place burrata in the center of the plate.", "Top with warm grapes and balsamic drizzle." },
                    CreatedAt = DateTime.UtcNow.AddDays(-8),
                    Embedding = dummyEmbedding
                },
                new Recipe
                {
                    Title = "Black Garlic Shoyu Ramen",
                    UserId = testUserId,
                    ImageUrl = "https://images.unsplash.com/photo-1569718212165-3a8278d5f624?q=80&w=800",
                    Ingredients = new List<string> { "Ramen Noodles", "Pork Belly", "Black Garlic Oil", "Soft Boiled Egg", "Nori" },
                    Instructions = new List<string> { "Slow roast pork belly until tender.", "Prepare shoyu broth with black garlic oil.", "Assemble with noodles and traditional toppings." },
                    CreatedAt = DateTime.UtcNow.AddDays(-9),
                    Embedding = dummyEmbedding
                },
                new Recipe
                {
                    Title = "Espresso Rubbed Skirt Steak",
                    UserId = testUserId,
                    ImageUrl = "https://images.unsplash.com/photo-1546241072-48010ad28c2c?q=80&w=800",
                    Ingredients = new List<string> { "Skirt Steak", "Fine Coffee Grounds", "Brown Sugar", "Ancho Chili Powder", "Cumin" },
                    Instructions = new List<string> { "Mix coffee and spices for the rub.", "Coat steak and let sit for 30 minutes.", "Sear on a cast iron skillet for 3 minutes per side." },
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    Embedding = dummyEmbedding
                },
                new Recipe
                {
                    Title = "Matcha White Chocolate Blondies",
                    UserId = testUserId,
                    ImageUrl = "https://images.unsplash.com/photo-1515037893149-de7f840978e2?q=80&w=800",
                    Ingredients = new List<string> { "Matcha Powder", "White Chocolate Chips", "Butter", "Flour", "Sea Salt" },
                    Instructions = new List<string> { "Melt butter and whisk with sugar and matcha.", "Fold in flour and chocolate chips.", "Bake at 175°C for 25 minutes until edges set." },
                    CreatedAt = DateTime.UtcNow.AddDays(-11),
                    Embedding = dummyEmbedding
                }
            };

            context.Recipes.AddRange(recipes);
            context.SaveChanges();
        }
    }
}