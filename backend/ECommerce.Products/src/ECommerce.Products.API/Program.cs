using Dapper;
using ECommerce.Products.Application.DTOs;
using ECommerce.Products.Application;
using ECommerce.Products.Application.Repositories;
using ECommerce.Products.Application.Services;
using ECommerce.Products.Application.Validators;
using ECommerce.Products.Infrastructure.Data;
using ECommerce.Products.Infrastructure.Repositories;
using Microsoft.OpenApi.Models;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DbOptions>(builder.Configuration.GetSection("DbOptions"));
builder.Services.AddSingleton<IConnectionFactory, NpgsqlConnectionFactory>();

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<ProductService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocal", policy =>
        policy.WithOrigins(
                "http://localhost:4200",   
                "http://localhost:5281",  
                "https://localhost:5281"   
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
    );
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = builder.Configuration["Swagger:Title"],
        Version = builder.Configuration["Swagger:Version"]
    });
});

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<ProductRequestValidator>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("AllowLocal");

app.UseSwagger();
app.UseSwaggerUI();

app.Use(async (ctx, next) =>
{
    ctx.Items["UserId"] = "mock-user";
    await next();
});

app.MapGet("/api/departments", async (IDepartmentRepository repo, CancellationToken ct) =>
{
    var deps = await repo.ListAsync(ct);
    return Results.Ok(deps.Select(d => new DepartmentResponse(d.Code, d.Description)));
})
.WithName("GetDepartments")
.WithTags("Departments")
.Produces<IEnumerable<DepartmentResponse>>(StatusCodes.Status200OK);

app.MapGet("/api/products", async (
    IProductRepository repo,
    string? search, string? departmentCode, bool? isActive, int page, int pageSize,
    CancellationToken ct) =>
{
    var service = new ProductService(repo);
    var res = await service.ListAsync(search, departmentCode, isActive, Math.Max(1, page), Math.Clamp(pageSize, 1, 100), ct);
    return Results.Ok(res);
})
.WithName("ListProducts")
.WithTags("Products")
.Produces<PagedResult<ProductResponse>>(StatusCodes.Status200OK);

app.MapGet("/api/products/{id:guid}", async (Guid id, IProductRepository repo, CancellationToken ct) =>
{
    var service = new ProductService(repo);
    var prod = await service.GetAsync(id, ct);
    return prod is null ? Results.NotFound() : Results.Ok(prod);
})
.WithName("GetProduct")
.WithTags("Products")
.Produces<ProductResponse>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

app.MapPost("/api/products", async (ProductRequest req, IValidator<ProductRequest> validator, ProductService service, CancellationToken ct) =>
{
    var val = await validator.ValidateAsync(req, ct);
    if (!val.IsValid) return Results.ValidationProblem(val.ToDictionary());
    var id = await service.CreateAsync(req, ct);
    return Results.Created($"/api/products/{id}", new { id });
})
.WithName("CreateProduct")
.WithTags("Products")
.Produces(StatusCodes.Status201Created)
.ProducesValidationProblem();

app.MapPut("/api/products/{id:guid}", async (Guid id, ProductUpdateRequest req, IValidator<ProductUpdateRequest> validator, ProductService service, CancellationToken ct) =>
{
    var val = await validator.ValidateAsync(req, ct);
    if (!val.IsValid) return Results.ValidationProblem(val.ToDictionary());
    await service.UpdateAsync(id, req, ct);
    return Results.NoContent();
})
.WithName("UpdateProduct")
.WithTags("Products")
.Produces(StatusCodes.Status204NoContent)
.ProducesValidationProblem();

app.MapDelete("/api/products/{id:guid}", async (Guid id, ProductService service, CancellationToken ct) =>
{
    await service.DeleteAsync(id, ct);
    return Results.NoContent();
})
.WithName("DeleteProduct")
.WithTags("Products")
.Produces(StatusCodes.Status204NoContent);

app.Run();
