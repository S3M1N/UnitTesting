using ASPNET_CORE_MVC_CRUD.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

public class ProductsControllerTests
{
    [Fact]
    public void Index_ReturnsViewResult()
    {
        // Arrange
        var controller = new ProductsController();

        // Act
        var result = controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.NotNull(viewResult);
    }

    [Fact]
    public void Index_ReturnsCorrectViewName()
    {
        // Arrange
        var controller = new ProductsController();

        // Act
        var result = controller.Index() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Index", result?.ViewName ?? "Index");
    }

    [Fact]
    public void Index_HasNoModel()
    {
        // Arrange
        var controller = new ProductsController();

        // Act
        var result = controller.Index() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Null(result?.Model);
    }
}
