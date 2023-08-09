using Eshop.Domain;
using Eshop.Domain.Dtos;
using Eshop.Domain.Entities;
using Eshop.Domain.Enums;
using System.Data;
using System.Security.Claims;

namespace Eshop.Api
{
    public static class AppRoute
    {
        public static void Setup(WebApplication app)
        {
            app.MapGet("/", () => "Hello iansucode.eshop.backend-netcore!");

            // category
            app.MapGet("/api/category", async (IUnitOfWork uow) =>
            {
                return await uow.CategoryRepository.GetAllAsync();
            });

            // product
            var productRoute = app.MapGroup("/api/product");

            productRoute.MapGet("/", async (IUnitOfWork uow) =>
            {
                return await uow.ProductRepository.GetAllAsync();
            });

            productRoute.MapGet("/{id}", async (int id, IUnitOfWork uow) =>
            {
                return await uow.ProductRepository.GetByIdAsync(id);
            });

            // cart
            var cartRoute = app.MapGroup("/api/cart");
            cartRoute.MapPost("/", async (CartItemCountDto cartItemCountDto, ClaimsPrincipal user, IUnitOfWork uow) =>
            {
                var userId = int.Parse(user.Claims.FirstOrDefault(p => p.Type == "jti")!.Value);
                var isSuccess = await uow.CartItemRepository.CreateOrUpdateItemAsync(cartItemCountDto, userId);
                return isSuccess ? Results.Ok(new { Status = "success" }) : Results.BadRequest(new { Message = $"Product not found" });
            }).RequireAuthorization();

            cartRoute.MapGet("/", async (ClaimsPrincipal user, IUnitOfWork uow) =>
            {
                var userId = int.Parse(user.Claims.FirstOrDefault(p => p.Type == "jti")!.Value);
                return await uow.CartItemRepository.GetPersonalCartItemsAsync(userId);
            }).RequireAuthorization();

            cartRoute.MapDelete("/remove/{id}", async (int id, ClaimsPrincipal user, IUnitOfWork uow) =>
            {
                var userId = int.Parse(user.Claims.FirstOrDefault(p => p.Type == "jti")!.Value);
                var isSuccess = await uow.CartItemRepository.DeleteAsync(userId, id);
                return isSuccess ? Results.Ok(new { Status = "success" }) : Results.BadRequest(new { Message = $"CartItem not found" });
            }).RequireAuthorization();

            cartRoute.MapDelete("/clear", async (ClaimsPrincipal user, IUnitOfWork uow) =>
            {
                var userId = int.Parse(user.Claims.FirstOrDefault(p => p.Type == "jti")!.Value);
                await uow.CartItemRepository.ClearAsync(userId);
                return Results.Ok(new { Status = "success" });
            }).RequireAuthorization();

            // user
            var userRoute = app.MapGroup("/api/user");

            userRoute.MapPost("/", async (UserLoginDto login, IUnitOfWork uow) =>
            {
                if (login?.Email == null || login?.Password == null)
                {
                    return Results.BadRequest(new { Message = "Email and passowrd are required" });
                }
                if (await uow.UserRepository.IsExisted(login!.Email))
                {
                    return Results.Conflict(new { Message = "Duplicate email" });
                }
                var hashedPwd = BCrypt.Net.BCrypt.HashPassword(login.Password);
                var now = DateTime.Now;
                var newUser = new User
                {
                    Email = login!.Email,
                    Password = hashedPwd,
                    Roles = new List<UserRole> { new UserRole { Role = EnumRole.User } },
                    CreatedAt = now,
                    UpdatedAt = now,
                };
                uow.UserRepository.Add(newUser);
                await uow.CompleteAsync();

                return Results.Created($"/api/user/{newUser.Id}", newUser);
            });

            // order
            app.MapPost("/api/order", async (int[] productIds, ClaimsPrincipal user, IUnitOfWork uow) =>
            {
                var userId = int.Parse(user.Claims.FirstOrDefault(p => p.Type == "jti")!.Value);
                await uow.OrderRepository.PlaceAsync(userId, productIds);
                return Results.Ok(new { Status = "success" });
            }).RequireAuthorization();

            app.MapGet("/api/order", async (ClaimsPrincipal user, IUnitOfWork uow) =>
            {
                var userId = int.Parse(user.Claims.FirstOrDefault(p => p.Type == "jti")!.Value);
                return Results.Ok(await uow.OrderRepository.GetPersonalOrders(userId));
            }).RequireAuthorization();

            // auth
            var authRoute = app.MapGroup("/api/auth");
            authRoute.MapPost("/login", async (UserLoginDto login, IUnitOfWork uow, AuthService authService) =>
            {
                var user = await uow.UserRepository.FindByEmailAsync(login.Email);
                if (user == null || !user.Active) return Results.Unauthorized();

                var verified = BCrypt.Net.BCrypt.Verify(login.Password, user.Password);
                if (!verified) return Results.Unauthorized();

                var roles = (await uow.UserRoleRepository.FindAsync(it => it.UserId == user.Id)).Select(it => it.Role.ToString()).ToList();
                var userInfo = new UserInfoDto
                {
                    Id = user.Id,
                    Email = login.Email,
                    Roles = roles,
                };

                var claims = authService.GenerateClaims(userInfo);
                var accessToken = authService.GenerateTokenByClaims(claims);
                return Results.Ok(new { accessToken, userInfo });
            });

            authRoute.MapGet("/claims", (ClaimsPrincipal user) =>
            {
                return Results.Ok(user.Claims.Select(p => new { p.Type, p.Value }));
            }).WithName("Claims").RequireAuthorization();

            authRoute.MapGet("/username", (ClaimsPrincipal user) =>
            {
                return Results.Ok(user.Identity?.Name);
            }).WithName("Username").RequireAuthorization();

            authRoute.MapGet("/isInRole", (ClaimsPrincipal user, string name) =>
            {
                return Results.Ok(user.IsInRole(name));
            }).WithName("IsInRole").RequireAuthorization();

            authRoute.MapGet("/jwtid", (ClaimsPrincipal user) =>
            {
                return Results.Ok(user.Claims.FirstOrDefault(p => p.Type == "jti")?.Value);
            }).WithName("JwtId").RequireAuthorization();

            authRoute.MapGet("/admin", (ClaimsPrincipal user) =>
            {
                return Results.Ok(new { message = $"Authenticated as {user?.Identity?.Name}" });
            }).RequireAuthorization("Admin");

            authRoute.MapGet("/user-info/{token}", (string token, AuthService authService) =>
            {
                return authService.GetUserInfoFromToken(token);
            });

            // admin
            var adminRoute = app.MapGroup("/api/admin").RequireAuthorization("Admin");

            adminRoute.MapGet("/user", async (IUnitOfWork uow) =>
            {
                return Results.Ok(await uow.UserRepository.GetAllInfosAsync());
            });

            adminRoute.MapPatch("/user/active", async (UserActiveDto userActiveDto, IUnitOfWork uow) =>
            {
                var isSuccess = await uow.UserRepository.ChangeActiveAsync(userActiveDto);
                return isSuccess ? Results.Ok(new { Message = "User active updated" }) : Results.BadRequest(new { Message = "User not found" });
            });

            adminRoute.MapDelete("/user/{id}", async (int id, IUnitOfWork uow) =>
            {
                var isSuccess = await uow.UserRepository.DeleteCompletelyAsync(id);
                return isSuccess ? Results.Ok(new { Message = "User deleted" }) : Results.BadRequest(new { Message = "User not found" });
            });

            adminRoute.MapGet("/order", async (IUnitOfWork uow) =>
            {
                return Results.Ok(await uow.OrderRepository.GetAllUserOrders());
            });

            adminRoute.MapPatch("/order/change-state", async (OrderStateDto orderStateDto, IUnitOfWork uow) =>
            {
                var order = await uow.OrderRepository.GetByIdAsync(orderStateDto.Id);
                if (order == null) return Results.BadRequest(new { Message = "Order not found" });

                order.State = orderStateDto.State;
                order.UpdatedAt = DateTime.Now;
                await uow.CompleteAsync();

                return Results.Ok(new { order.UpdatedAt });
            });

            adminRoute.MapDelete("/order/{id}", async (int id, IUnitOfWork uow) =>
            {
                var order = await uow.OrderRepository.GetByIdAsync(id);
                if (order == null) return Results.BadRequest(new { Message = "Order not found" });

                uow.OrderRepository.Remove(order);
                await uow.CompleteAsync();

                return Results.Ok(new { Message = "Order deleted" });
            });
        }
    }
}