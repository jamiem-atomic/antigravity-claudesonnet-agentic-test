using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using VehicleMarketplace.Api.Models;

namespace VehicleMarketplace.Api.Data;

public class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Check if already seeded
        if (await context.Users.AnyAsync())
        {
            return;
        }

        // Create users
        var admin = new User
        {
            Email = "admin@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            DisplayName = "Admin User",
            Phone = "+1234567890",
            Location = "New York, NY",
            IsAdmin = true,
            CreatedAt = DateTime.UtcNow.AddMonths(-6)
        };

        var user1 = new User
        {
            Email = "john.doe@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!"),
            DisplayName = "John Doe",
            Phone = "+1234567891",
            Location = "Los Angeles, CA",
            CreatedAt = DateTime.UtcNow.AddMonths(-4)
        };

        var user2 = new User
        {
            Email = "jane.smith@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!"),
            DisplayName = "Jane Smith",
            Phone = "+1234567892",
            Location = "Chicago, IL",
            CreatedAt = DateTime.UtcNow.AddMonths(-3)
        };

        var user3 = new User
        {
            Email = "bob.wilson@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!"),
            DisplayName = "Bob Wilson",
            Phone = "+1234567893",
            Location = "Houston, TX",
            CreatedAt = DateTime.UtcNow.AddMonths(-2)
        };

        context.Users.AddRange(admin, user1, user2, user3);
        await context.SaveChangesAsync();

        // Create listings
        var listings = new List<Listing>
        {
            // Published listings
            new Listing
            {
                Title = "2020 Toyota Camry SE - Excellent Condition",
                Description = "Well-maintained 2020 Toyota Camry SE with low mileage. Single owner, all service records available. Features include backup camera, lane departure warning, and adaptive cruise control.",
                Price = 24500,
                Make = "Toyota",
                Model = "Camry",
                Year = 2020,
                Mileage = 32000,
                FuelType = FuelType.Petrol,
                Transmission = Transmission.Automatic,
                BodyType = BodyType.Sedan,
                Condition = VehicleCondition.Used,
                Location = "Los Angeles, CA",
                Photos = JsonSerializer.Serialize(new[] { "/images/camry1.jpg", "/images/camry2.jpg" }),
                Status = ListingStatus.Published,
                SellerId = user1.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow.AddDays(-25)
            },
            new Listing
            {
                Title = "2019 Honda CR-V EX AWD",
                Description = "Spacious and reliable Honda CR-V with all-wheel drive. Perfect for families. Includes heated seats, sunroof, and Honda Sensing safety suite.",
                Price = 26900,
                Make = "Honda",
                Model = "CR-V",
                Year = 2019,
                Mileage = 45000,
                FuelType = FuelType.Petrol,
                Transmission = Transmission.Automatic,
                BodyType = BodyType.SUV,
                Condition = VehicleCondition.Used,
                Location = "Chicago, IL",
                Photos = JsonSerializer.Serialize(new[] { "/images/crv1.jpg" }),
                Status = ListingStatus.Published,
                SellerId = user2.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-28)
            },
            new Listing
            {
                Title = "2021 Tesla Model 3 Long Range",
                Description = "Premium electric sedan with autopilot. Incredible range and performance. White interior, 19-inch wheels, premium audio system.",
                Price = 45000,
                Make = "Tesla",
                Model = "Model 3",
                Year = 2021,
                Mileage = 18000,
                FuelType = FuelType.Electric,
                Transmission = Transmission.Automatic,
                BodyType = BodyType.Sedan,
                Condition = VehicleCondition.Used,
                Location = "Houston, TX",
                Photos = JsonSerializer.Serialize(new[] { "/images/tesla1.jpg", "/images/tesla2.jpg", "/images/tesla3.jpg" }),
                Status = ListingStatus.Published,
                SellerId = user3.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-20)
            },
            new Listing
            {
                Title = "2018 Ford F-150 XLT 4x4",
                Description = "Powerful and capable pickup truck. 5.0L V8 engine, towing package, bedliner. Great for work or play.",
                Price = 32000,
                Make = "Ford",
                Model = "F-150",
                Year = 2018,
                Mileage = 62000,
                FuelType = FuelType.Petrol,
                Transmission = Transmission.Automatic,
                BodyType = BodyType.Truck,
                Condition = VehicleCondition.Used,
                Location = "Dallas, TX",
                Photos = JsonSerializer.Serialize(new[] { "/images/f150-1.jpg" }),
                Status = ListingStatus.Published,
                SellerId = user1.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-18)
            },
            new Listing
            {
                Title = "2022 Mazda CX-5 Touring",
                Description = "Like-new compact SUV with premium features. Leather seats, Bose audio, navigation system. Still under warranty.",
                Price = 29500,
                Make = "Mazda",
                Model = "CX-5",
                Year = 2022,
                Mileage = 12000,
                FuelType = FuelType.Petrol,
                Transmission = Transmission.Automatic,
                BodyType = BodyType.SUV,
                Condition = VehicleCondition.Used,
                Location = "Seattle, WA",
                Photos = JsonSerializer.Serialize(new[] { "/images/cx5-1.jpg", "/images/cx5-2.jpg" }),
                Status = ListingStatus.Published,
                SellerId = user2.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-15)
            },
            new Listing
            {
                Title = "2017 BMW 3 Series 330i",
                Description = "Luxury sport sedan with excellent performance. M Sport package, navigation, premium sound system.",
                Price = 23000,
                Make = "BMW",
                Model = "3 Series",
                Year = 2017,
                Mileage = 55000,
                FuelType = FuelType.Petrol,
                Transmission = Transmission.Automatic,
                BodyType = BodyType.Sedan,
                Condition = VehicleCondition.Used,
                Location = "Boston, MA",
                Photos = JsonSerializer.Serialize(new[] { "/images/bmw1.jpg" }),
                Status = ListingStatus.Published,
                SellerId = user3.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-14)
            },
            new Listing
            {
                Title = "2020 Subaru Outback Limited",
                Description = "Adventure-ready wagon with AWD. Perfect for outdoor enthusiasts. Roof rails, heated seats, EyeSight safety.",
                Price = 28000,
                Make = "Subaru",
                Model = "Outback",
                Year = 2020,
                Mileage = 38000,
                FuelType = FuelType.Petrol,
                Transmission = Transmission.Automatic,
                BodyType = BodyType.SUV,
                Condition = VehicleCondition.Used,
                Location = "Denver, CO",
                Photos = JsonSerializer.Serialize(new[] { "/images/outback1.jpg" }),
                Status = ListingStatus.Published,
                SellerId = user1.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-12)
            },
            new Listing
            {
                Title = "2019 Chevrolet Silverado 1500 LT",
                Description = "Reliable full-size pickup. V8 engine, crew cab, tow package. Well maintained with service history.",
                Price = 34000,
                Make = "Chevrolet",
                Model = "Silverado 1500",
                Year = 2019,
                Mileage = 48000,
                FuelType = FuelType.Petrol,
                Transmission = Transmission.Automatic,
                BodyType = BodyType.Truck,
                Condition = VehicleCondition.Used,
                Location = "Phoenix, AZ",
                Photos = JsonSerializer.Serialize(new[] { "/images/silverado1.jpg", "/images/silverado2.jpg" }),
                Status = ListingStatus.Published,
                SellerId = user2.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            },
            new Listing
            {
                Title = "2021 Hyundai Tucson SEL",
                Description = "Modern compact SUV with great fuel economy. Apple CarPlay, Android Auto, blind spot monitoring.",
                Price = 25500,
                Make = "Hyundai",
                Model = "Tucson",
                Year = 2021,
                Mileage = 22000,
                FuelType = FuelType.Petrol,
                Transmission = Transmission.Automatic,
                BodyType = BodyType.SUV,
                Condition = VehicleCondition.Used,
                Location = "Atlanta, GA",
                Photos = JsonSerializer.Serialize(new[] { "/images/tucson1.jpg" }),
                Status = ListingStatus.Published,
                SellerId = user3.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-9)
            },
            new Listing
            {
                Title = "2018 Nissan Altima 2.5 SL",
                Description = "Comfortable midsize sedan with premium features. Leather interior, sunroof, heated seats.",
                Price = 18500,
                Make = "Nissan",
                Model = "Altima",
                Year = 2018,
                Mileage = 52000,
                FuelType = FuelType.Petrol,
                Transmission = Transmission.Automatic,
                BodyType = BodyType.Sedan,
                Condition = VehicleCondition.Used,
                Location = "Miami, FL",
                Photos = JsonSerializer.Serialize(new[] { "/images/altima1.jpg" }),
                Status = ListingStatus.Published,
                SellerId = user1.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-8)
            },
            new Listing
            {
                Title = "2020 Volkswagen Jetta GLI",
                Description = "Sporty sedan with turbocharged engine. Manual transmission, sport suspension, premium audio.",
                Price = 22000,
                Make = "Volkswagen",
                Model = "Jetta",
                Year = 2020,
                Mileage = 28000,
                FuelType = FuelType.Petrol,
                Transmission = Transmission.Manual,
                BodyType = BodyType.Sedan,
                Condition = VehicleCondition.Used,
                Location = "Portland, OR",
                Photos = JsonSerializer.Serialize(new[] { "/images/jetta1.jpg", "/images/jetta2.jpg" }),
                Status = ListingStatus.Published,
                SellerId = user2.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-7)
            },
            new Listing
            {
                Title = "2019 Jeep Wrangler Unlimited Sahara",
                Description = "Iconic off-road SUV. 4x4, removable top, upgraded wheels and tires. Adventure awaits!",
                Price = 36000,
                Make = "Jeep",
                Model = "Wrangler",
                Year = 2019,
                Mileage = 35000,
                FuelType = FuelType.Petrol,
                Transmission = Transmission.Automatic,
                BodyType = BodyType.SUV,
                Condition = VehicleCondition.Used,
                Location = "Austin, TX",
                Photos = JsonSerializer.Serialize(new[] { "/images/wrangler1.jpg" }),
                Status = ListingStatus.Published,
                SellerId = user3.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-6)
            },
            new Listing
            {
                Title = "2021 Kia Seltos SX Turbo",
                Description = "Stylish subcompact SUV with turbocharged performance. Loaded with tech and safety features.",
                Price = 26000,
                Make = "Kia",
                Model = "Seltos",
                Year = 2021,
                Mileage = 15000,
                FuelType = FuelType.Petrol,
                Transmission = Transmission.Automatic,
                BodyType = BodyType.SUV,
                Condition = VehicleCondition.Used,
                Location = "San Diego, CA",
                Photos = JsonSerializer.Serialize(new[] { "/images/seltos1.jpg" }),
                Status = ListingStatus.Published,
                SellerId = user1.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            },
            new Listing
            {
                Title = "2017 Audi A4 Premium Plus",
                Description = "Refined luxury sedan. Quattro AWD, virtual cockpit, Bang & Olufsen sound system.",
                Price = 24000,
                Make = "Audi",
                Model = "A4",
                Year = 2017,
                Mileage = 48000,
                FuelType = FuelType.Petrol,
                Transmission = Transmission.Automatic,
                BodyType = BodyType.Sedan,
                Condition = VehicleCondition.Used,
                Location = "San Francisco, CA",
                Photos = JsonSerializer.Serialize(new[] { "/images/audi1.jpg", "/images/audi2.jpg" }),
                Status = ListingStatus.Published,
                SellerId = user2.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-4)
            },
            new Listing
            {
                Title = "2020 GMC Sierra 1500 Denali",
                Description = "Premium full-size truck with luxury appointments. Leather, navigation, advanced towing tech.",
                Price = 48000,
                Make = "GMC",
                Model = "Sierra 1500",
                Year = 2020,
                Mileage = 30000,
                FuelType = FuelType.Petrol,
                Transmission = Transmission.Automatic,
                BodyType = BodyType.Truck,
                Condition = VehicleCondition.Used,
                Location = "Nashville, TN",
                Photos = JsonSerializer.Serialize(new[] { "/images/sierra1.jpg" }),
                Status = ListingStatus.Published,
                SellerId = user3.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            },
            new Listing
            {
                Title = "2019 Mercedes-Benz C-Class C300",
                Description = "Elegant luxury sedan with cutting-edge technology. Premium interior, advanced safety features.",
                Price = 32000,
                Make = "Mercedes-Benz",
                Model = "C-Class",
                Year = 2019,
                Mileage = 38000,
                FuelType = FuelType.Petrol,
                Transmission = Transmission.Automatic,
                BodyType = BodyType.Sedan,
                Condition = VehicleCondition.Used,
                Location = "Charlotte, NC",
                Photos = JsonSerializer.Serialize(new[] { "/images/merc1.jpg" }),
                Status = ListingStatus.Published,
                SellerId = user1.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new Listing
            {
                Title = "2021 Ford Mustang GT Premium",
                Description = "American muscle car with 5.0L V8. Performance package, premium interior, active exhaust.",
                Price = 42000,
                Make = "Ford",
                Model = "Mustang",
                Year = 2021,
                Mileage = 8000,
                FuelType = FuelType.Petrol,
                Transmission = Transmission.Manual,
                BodyType = BodyType.Coupe,
                Condition = VehicleCondition.Used,
                Location = "Detroit, MI",
                Photos = JsonSerializer.Serialize(new[] { "/images/mustang1.jpg", "/images/mustang2.jpg", "/images/mustang3.jpg" }),
                Status = ListingStatus.Published,
                SellerId = user2.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new Listing
            {
                Title = "2018 Toyota RAV4 Adventure",
                Description = "Rugged compact SUV ready for adventure. AWD, roof rails, upgraded suspension.",
                Price = 23500,
                Make = "Toyota",
                Model = "RAV4",
                Year = 2018,
                Mileage = 58000,
                FuelType = FuelType.Petrol,
                Transmission = Transmission.Automatic,
                BodyType = BodyType.SUV,
                Condition = VehicleCondition.Used,
                Location = "Salt Lake City, UT",
                Photos = JsonSerializer.Serialize(new[] { "/images/rav4-1.jpg" }),
                Status = ListingStatus.Published,
                SellerId = user3.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new Listing
            {
                Title = "2020 Chevrolet Corvette Stingray",
                Description = "Mid-engine supercar with breathtaking performance. Z51 package, magnetic ride control.",
                Price = 75000,
                Make = "Chevrolet",
                Model = "Corvette",
                Year = 2020,
                Mileage = 5000,
                FuelType = FuelType.Petrol,
                Transmission = Transmission.Automatic,
                BodyType = BodyType.Coupe,
                Condition = VehicleCondition.Used,
                Location = "Las Vegas, NV",
                Photos = JsonSerializer.Serialize(new[] { "/images/corvette1.jpg", "/images/corvette2.jpg" }),
                Status = ListingStatus.Published,
                SellerId = user1.Id,
                CreatedAt = DateTime.UtcNow
            },
            new Listing
            {
                Title = "2019 Honda Accord Sport",
                Description = "Sporty midsize sedan with excellent reliability. Turbocharged engine, sport suspension.",
                Price = 24000,
                Make = "Honda",
                Model = "Accord",
                Year = 2019,
                Mileage = 42000,
                FuelType = FuelType.Petrol,
                Transmission = Transmission.Automatic,
                BodyType = BodyType.Sedan,
                Condition = VehicleCondition.Used,
                Location = "Orlando, FL",
                Photos = JsonSerializer.Serialize(new[] { "/images/accord1.jpg" }),
                Status = ListingStatus.Published,
                SellerId = user2.Id,
                CreatedAt = DateTime.UtcNow
            },
            new Listing
            {
                Title = "2021 Ram 1500 Big Horn",
                Description = "Smooth-riding full-size truck. V8 HEMI engine, crew cab, luxury interior features.",
                Price = 38000,
                Make = "Ram",
                Model = "1500",
                Year = 2021,
                Mileage = 25000,
                FuelType = FuelType.Petrol,
                Transmission = Transmission.Automatic,
                BodyType = BodyType.Truck,
                Condition = VehicleCondition.Used,
                Location = "Indianapolis, IN",
                Photos = JsonSerializer.Serialize(new[] { "/images/ram1.jpg" }),
                Status = ListingStatus.Published,
                SellerId = user3.Id,
                CreatedAt = DateTime.UtcNow
            },
            new Listing
            {
                Title = "2018 Lexus RX 350 F Sport",
                Description = "Luxury SUV with exceptional comfort and reliability. Premium audio, navigation, heated/cooled seats.",
                Price = 35000,
                Make = "Lexus",
                Model = "RX 350",
                Year = 2018,
                Mileage = 45000,
                FuelType = FuelType.Petrol,
                Transmission = Transmission.Automatic,
                BodyType = BodyType.SUV,
                Condition = VehicleCondition.Used,
                Location = "San Jose, CA",
                Photos = JsonSerializer.Serialize(new[] { "/images/lexus1.jpg", "/images/lexus2.jpg" }),
                Status = ListingStatus.Published,
                SellerId = user1.Id,
                CreatedAt = DateTime.UtcNow
            },
            new Listing
            {
                Title = "2020 Porsche Macan S",
                Description = "Premium compact SUV with sports car performance. AWD, sport exhaust, premium interior.",
                Price = 52000,
                Make = "Porsche",
                Model = "Macan",
                Year = 2020,
                Mileage = 18000,
                FuelType = FuelType.Petrol,
                Transmission = Transmission.Automatic,
                BodyType = BodyType.SUV,
                Condition = VehicleCondition.Used,
                Location = "Scottsdale, AZ",
                Photos = JsonSerializer.Serialize(new[] { "/images/macan1.jpg" }),
                Status = ListingStatus.Published,
                SellerId = user2.Id,
                CreatedAt = DateTime.UtcNow
            },
            new Listing
            {
                Title = "2019 Volkswagen Golf GTI",
                Description = "Hot hatch with turbocharged fun. Manual transmission, plaid seats, performance brakes.",
                Price = 23000,
                Make = "Volkswagen",
                Model = "Golf",
                Year = 2019,
                Mileage = 32000,
                FuelType = FuelType.Petrol,
                Transmission = Transmission.Manual,
                BodyType = BodyType.Hatchback,
                Condition = VehicleCondition.Used,
                Location = "Minneapolis, MN",
                Photos = JsonSerializer.Serialize(new[] { "/images/gti1.jpg", "/images/gti2.jpg" }),
                Status = ListingStatus.Published,
                SellerId = user3.Id,
                CreatedAt = DateTime.UtcNow
            },
            new Listing
            {
                Title = "2021 Toyota Highlander XLE",
                Description = "Three-row family SUV with excellent safety ratings. Hybrid powertrain for great fuel economy.",
                Price = 42000,
                Make = "Toyota",
                Model = "Highlander",
                Year = 2021,
                Mileage = 20000,
                FuelType = FuelType.Hybrid,
                Transmission = Transmission.Automatic,
                BodyType = BodyType.SUV,
                Condition = VehicleCondition.Used,
                Location = "Columbus, OH",
                Photos = JsonSerializer.Serialize(new[] { "/images/highlander1.jpg" }),
                Status = ListingStatus.Published,
                SellerId = user1.Id,
                CreatedAt = DateTime.UtcNow
            },
            new Listing
            {
                Title = "2017 Dodge Charger R/T",
                Description = "Muscle sedan with V8 power. Rear-wheel drive, sport mode, aggressive styling.",
                Price = 26000,
                Make = "Dodge",
                Model = "Charger",
                Year = 2017,
                Mileage = 55000,
                FuelType = FuelType.Petrol,
                Transmission = Transmission.Automatic,
                BodyType = BodyType.Sedan,
                Condition = VehicleCondition.Used,
                Location = "Tampa, FL",
                Photos = JsonSerializer.Serialize(new[] { "/images/charger1.jpg" }),
                Status = ListingStatus.Published,
                SellerId = user2.Id,
                CreatedAt = DateTime.UtcNow
            },

            // Pending approval listings
            new Listing
            {
                Title = "2023 Honda Civic Type R",
                Description = "Brand new performance hatchback. Limited edition, track-ready, manual transmission.",
                Price = 48000,
                Make = "Honda",
                Model = "Civic",
                Year = 2023,
                Mileage = 500,
                FuelType = FuelType.Petrol,
                Transmission = Transmission.Manual,
                BodyType = BodyType.Hatchback,
                Condition = VehicleCondition.New,
                Location = "Los Angeles, CA",
                Photos = JsonSerializer.Serialize(new[] { "/images/typer1.jpg" }),
                Status = ListingStatus.PendingApproval,
                SellerId = user1.Id,
                CreatedAt = DateTime.UtcNow.AddHours(-2)
            },
            new Listing
            {
                Title = "2022 Ford Bronco Wildtrak",
                Description = "Off-road beast with removable roof. Sasquatch package, advanced 4x4 system.",
                Price = 55000,
                Make = "Ford",
                Model = "Bronco",
                Year = 2022,
                Mileage = 3000,
                FuelType = FuelType.Petrol,
                Transmission = Transmission.Automatic,
                BodyType = BodyType.SUV,
                Condition = VehicleCondition.Used,
                Location = "Denver, CO",
                Photos = JsonSerializer.Serialize(new[] { "/images/bronco1.jpg", "/images/bronco2.jpg" }),
                Status = ListingStatus.PendingApproval,
                SellerId = user2.Id,
                CreatedAt = DateTime.UtcNow.AddHours(-3)
            },
            new Listing
            {
                Title = "2021 Audi e-tron Premium Plus",
                Description = "Luxury electric SUV with impressive range. Virtual cockpit, premium sound, fast charging.",
                Price = 58000,
                Make = "Audi",
                Model = "e-tron",
                Year = 2021,
                Mileage = 12000,
                FuelType = FuelType.Electric,
                Transmission = Transmission.Automatic,
                BodyType = BodyType.SUV,
                Condition = VehicleCondition.Used,
                Location = "Seattle, WA",
                Photos = JsonSerializer.Serialize(new[] { "/images/etron1.jpg" }),
                Status = ListingStatus.PendingApproval,
                SellerId = user3.Id,
                CreatedAt = DateTime.UtcNow.AddHours(-5)
            },
            new Listing
            {
                Title = "2020 Nissan Leaf SV Plus",
                Description = "Affordable electric car with good range. ProPilot Assist, heated seats, quick charge port.",
                Price = 22000,
                Make = "Nissan",
                Model = "Leaf",
                Year = 2020,
                Mileage = 28000,
                FuelType = FuelType.Electric,
                Transmission = Transmission.Automatic,
                BodyType = BodyType.Hatchback,
                Condition = VehicleCondition.Used,
                Location = "Portland, OR",
                Photos = JsonSerializer.Serialize(new[] { "/images/leaf1.jpg" }),
                Status = ListingStatus.PendingApproval,
                SellerId = user1.Id,
                CreatedAt = DateTime.UtcNow.AddHours(-6)
            },
            new Listing
            {
                Title = "2019 Cadillac Escalade Platinum",
                Description = "Ultimate luxury SUV. Third row seating, premium leather, advanced tech throughout.",
                Price = 62000,
                Make = "Cadillac",
                Model = "Escalade",
                Year = 2019,
                Mileage = 35000,
                FuelType = FuelType.Petrol,
                Transmission = Transmission.Automatic,
                BodyType = BodyType.SUV,
                Condition = VehicleCondition.Used,
                Location = "Miami, FL",
                Photos = JsonSerializer.Serialize(new[] { "/images/escalade1.jpg", "/images/escalade2.jpg" }),
                Status = ListingStatus.PendingApproval,
                SellerId = user2.Id,
                CreatedAt = DateTime.UtcNow.AddHours(-8)
            },

            // Rejected listings
            new Listing
            {
                Title = "2015 Toyota Corolla",
                Description = "Needs work. Engine issues.",
                Price = 3000,
                Make = "Toyota",
                Model = "Corolla",
                Year = 2015,
                Mileage = 120000,
                FuelType = FuelType.Petrol,
                Transmission = Transmission.Automatic,
                BodyType = BodyType.Sedan,
                Condition = VehicleCondition.ForParts,
                Location = "Unknown",
                Photos = JsonSerializer.Serialize(new[] { "/images/corolla1.jpg" }),
                Status = ListingStatus.Rejected,
                RejectionReason = "Insufficient description and unclear condition. Please provide more details about the vehicle's issues and include better photos.",
                SellerId = user3.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedAt = DateTime.UtcNow.AddDays(-4)
            },
            new Listing
            {
                Title = "Amazing Car - Best Deal!!!",
                Description = "Call now! Won't last long! Cash only!",
                Price = 5000,
                Make = "Honda",
                Model = "Civic",
                Year = 2010,
                Mileage = 150000,
                FuelType = FuelType.Petrol,
                Transmission = Transmission.Manual,
                BodyType = BodyType.Sedan,
                Condition = VehicleCondition.Used,
                Location = "Somewhere",
                Photos = "[]",
                Status = ListingStatus.Rejected,
                RejectionReason = "Listing appears spammy with excessive punctuation and vague details. Please provide accurate vehicle information and professional description.",
                SellerId = user1.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                UpdatedAt = DateTime.UtcNow.AddDays(-2)
            }
        };

        context.Listings.AddRange(listings);
        await context.SaveChangesAsync();

        // Create favourites
        var favourites = new List<Favourite>
        {
            new Favourite { UserId = user2.Id, ListingId = listings[0].Id },
            new Favourite { UserId = user3.Id, ListingId = listings[0].Id },
            new Favourite { UserId = user1.Id, ListingId = listings[1].Id },
            new Favourite { UserId = user3.Id, ListingId = listings[2].Id },
            new Favourite { UserId = user1.Id, ListingId = listings[14].Id }
        };

        context.Favourites.AddRange(favourites);
        await context.SaveChangesAsync();

        // Create message threads and messages
        var thread1 = new MessageThread
        {
            ListingId = listings[0].Id,
            BuyerId = user2.Id,
            SellerId = user1.Id,
            CreatedAt = DateTime.UtcNow.AddDays(-5)
        };

        var thread2 = new MessageThread
        {
            ListingId = listings[2].Id,
            BuyerId = user1.Id,
            SellerId = user3.Id,
            CreatedAt = DateTime.UtcNow.AddDays(-3)
        };

        var thread3 = new MessageThread
        {
            ListingId = listings[1].Id,
            BuyerId = user3.Id,
            SellerId = user2.Id,
            CreatedAt = DateTime.UtcNow.AddDays(-2)
        };

        context.MessageThreads.AddRange(thread1, thread2, thread3);
        await context.SaveChangesAsync();

        var messages = new List<Message>
        {
            new Message
            {
                ThreadId = thread1.Id,
                SenderId = user2.Id,
                Body = "Hi! I'm interested in your Camry. Is it still available?",
                SentAt = DateTime.UtcNow.AddDays(-5)
            },
            new Message
            {
                ThreadId = thread1.Id,
                SenderId = user1.Id,
                Body = "Yes, it's still available! Would you like to schedule a test drive?",
                SentAt = DateTime.UtcNow.AddDays(-5).AddHours(2)
            },
            new Message
            {
                ThreadId = thread1.Id,
                SenderId = user2.Id,
                Body = "That would be great! I'm available this weekend. What times work for you?",
                SentAt = DateTime.UtcNow.AddDays(-4)
            },
            new Message
            {
                ThreadId = thread2.Id,
                SenderId = user1.Id,
                Body = "Hello, does the Tesla come with the original charger and cables?",
                SentAt = DateTime.UtcNow.AddDays(-3)
            },
            new Message
            {
                ThreadId = thread2.Id,
                SenderId = user3.Id,
                Body = "Yes, it includes the mobile connector and all original accessories. Also has the wall connector installed in my garage that I can include.",
                SentAt = DateTime.UtcNow.AddDays(-3).AddHours(3)
            },
            new Message
            {
                ThreadId = thread3.Id,
                SenderId = user3.Id,
                Body = "Is the CR-V price negotiable?",
                SentAt = DateTime.UtcNow.AddDays(-2)
            },
            new Message
            {
                ThreadId = thread3.Id,
                SenderId = user2.Id,
                Body = "There's a little room for negotiation. What price did you have in mind?",
                SentAt = DateTime.UtcNow.AddDays(-2).AddHours(1)
            }
        };

        context.Messages.AddRange(messages);
        await context.SaveChangesAsync();

        // Create reports
        var reports = new List<Report>
        {
            new Report
            {
                ListingId = listings[18].Id,
                ReporterId = user2.Id,
                Reason = ReportReason.Misleading,
                Comment = "The price seems too good to be true for a vehicle in this condition.",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new Report
            {
                ListingId = listings[18].Id,
                ReporterId = user3.Id,
                Reason = ReportReason.Scam,
                Comment = "Seller is asking for payment before viewing the vehicle.",
                CreatedAt = DateTime.UtcNow.AddHours(-12)
            },
            new Report
            {
                ListingId = listings[16].Id,
                ReporterId = user1.Id,
                Reason = ReportReason.Other,
                Comment = "Vehicle VIN doesn't match the description provided.",
                CreatedAt = DateTime.UtcNow.AddHours(-6)
            }
        };

        context.Reports.AddRange(reports);
        await context.SaveChangesAsync();
    }
}
