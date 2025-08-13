using ECommerce.Products.Application.DTOs;
using ECommerce.Products.Application.Validators;
using FluentAssertions;

namespace ECommerce.Products.Tests;

public class ProductRequestValidatorTests
{
    [Theory]
    [InlineData("C123", "Produto X", "010", 10.50, true)]
    public void Valid_Request_Should_Pass(string code, string desc, string dep, decimal price, bool active)
    {
        var v = new ProductRequestValidator();
        var result = v.Validate(new ProductRequest(code, desc, dep, price, active));
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void NegativePrice_Should_Fail()
    {
        var v = new ProductRequestValidator();
        var result = v.Validate(new ProductRequest("C1", "Desc", "010", -1m, true));
        result.IsValid.Should().BeFalse();
    }
}