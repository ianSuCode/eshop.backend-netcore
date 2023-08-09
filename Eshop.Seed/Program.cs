using Eshop.Data;
using Eshop.Domain.Entities;
using Eshop.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

void Seed(EshopDbContext dbContext)
{
    dbContext.Product.RemoveRange(dbContext.Product);
    dbContext.Category.RemoveRange(dbContext.Category);
    dbContext.User.RemoveRange(dbContext.User);

    dbContext.Category.AddRange(
        new Category { Name = "Armour" },
        new Category { Name = "Shield" },
        new Category { Name = "Weapon" },
        new Category { Name = "Item" });
    dbContext.SaveChanges();

    var categoryAmour = dbContext.Category.Where(it => it.Name == "Armour").First();
    var categoryShield = dbContext.Category.Where(it => it.Name == "Shield").First();
    var categoryWeapon = dbContext.Category.Where(it => it.Name == "Weapon").First();
    var categoryItem = dbContext.Category.Where(it => it.Name == "Item").First();

    var productData = new List<(string, string, int, string)> // (1)Name, (2)CategoryName, (3)Price, (4)Description
        {
            ("Clothes", "Armour", 20, "An outfit woven from thick, sturdy thread"),
            ("Leather armour", "Armour", 70, "Warrior's clothes made from tanned animal skins."),
            ("Chain mail", "Armour", 300, "Made from interlocking loops of sturdy steel."),
            ("Iron armour", "Armour", 1000, "Armour favored by the knights of Tantegel."),
            ("Full plate armour", "Armour", 3000, "A peerlessly protective suit of armour."),
            ("Magic armour", "Armour", 7700, "Restores HP while you walk."),
            ("Leather shield", "Shield", 90, "A light shield of simple construction."),
            ("Iron shield", "Shield", 800, "A large, sturdy shield of solid iron."),
            ("Silver Shield", "Shield", 14800, "A shield made of purest beaten silver."),
            ("Bamboo lance", "Weapon", 10, "A long spear made of bamboo."),
            ("Oaken Club", "Weapon", 60, "A weapon hewn from a stout log."),
            ("Copper Sword", "Weapon", 180, "A sturdy sword of copper."),
            ("Iron Axe", "Weapon", 560, "Can fell feebler foes with a single swing."),
            ("Steel Sword", "Weapon", 1500, "Slices through enemies like a knife through butter."),
            ("Flame Sword", "Weapon", 9800, "Useful in battle, and not only when wielded..."),
            ("Dragon's scale", "Item", 20, "A talisman carved from the scale of a dragon."),
            ("Fairy water", "Item", 38, "It is used to avoid random encounters"),
            ("Herb", "Item", 24, "Restores 20-35 HP."),
            ("Torch", "Item", 8, "Lights up the darkness of dungeons."),
            ("Wings", "Item", 70, "?")
        };

    int GetCategoryId(string name)
    {
        switch (name)
        {
            case "Armour":
                return categoryAmour!.Id;

            case "Shield":
                return categoryShield!.Id;

            case "Weapon":
                return categoryWeapon!.Id;

            case "Item":
                return categoryItem!.Id;

            default:
                break;
        }
        return -1;
    };

    var products = productData.Select(it => new Product
    {
        Name = it.Item1,
        Price = it.Item3,
        Description = it.Item4,
        CategoryId = GetCategoryId(it.Item2)
    });

    dbContext.Product.AddRange(products);
    dbContext.SaveChanges();

    var now = DateTime.Now;
    dbContext.User.RemoveRange(dbContext.User);
    dbContext.User.Add(new User
    {
        Email = "iansucode@mail.com",
        Password = BCrypt.Net.BCrypt.HashPassword("0000"),
        Roles = new List<UserRole> { new UserRole { Role = EnumRole.User }, new UserRole { Role = EnumRole.Admin } },
        CreatedAt = now,
        UpdatedAt = now,
        Active = true
    });
    dbContext.SaveChanges();
}

Console.WriteLine("Hello, Seed!");
var connectionString = "Data Source=(LocalDb)\\MSSqlLocalDB; Initial Catalog=Eshop; Integrated Security=True";

var services = new ServiceCollection()
    .AddDbContext<EshopDbContext>(options => options.UseSqlServer(connectionString))
    .BuildServiceProvider();

using (var scope = services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EshopDbContext>();

    dbContext.Database.EnsureCreated();

    Seed(dbContext);
}