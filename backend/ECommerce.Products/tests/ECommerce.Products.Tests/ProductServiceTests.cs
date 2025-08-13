using ECommerce.Products.Application.DTOs;
using ECommerce.Products.Application.Repositories;
using ECommerce.Products.Application.Services;
using FluentAssertions;
using Moq;

namespace ECommerce.Products.Tests;

public class ProductServiceTests
{
    [Fact]
    public async Task Create_Should_Throw_When_CodeExists()
    {
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.CodeExistsAsync("X", It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var service = new ProductService(repo.Object);

        var act = async () => await service.CreateAsync(new ProductRequest("X", "Desc", "010", 1, true), CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}