using ASPNET_CORE_MVC_CRUD.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

public class EmployeesControllerTests
{
    [Fact]
    public void Index_ReturnsAViewResult()
    {
        // Arrange
        var controller = new EmployeesController();

        // Act
        var result = controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Index_ReturnsCorrectView()
    {
        // Arrange
        var controller = new EmployeesController();

        // Act
        var result = controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Null(viewResult.ViewName); // When ViewName is null, it means it uses the default view name which is "Index"
    }

    [Fact]
    public void Index_HasCorrectViewData()
    {
        // Arrange
        var controller = new EmployeesController();

        // Act
        var result = controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        // Example assertion, modify based on actual ViewData expected
        // Assert.Equal("Expected Value", viewResult.ViewData["SomeKey"]);
    }

    [Fact]
    public void Index_ReturnsNotNullViewResult()
    {
        // Arrange
        var controller = new EmployeesController();

        // Act
        var result = controller.Index();

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void Index_ReturnsViewResultInstance()
    {
        // Arrange
        var controller = new EmployeesController();

        // Act
        var result = controller.Index();

        // Assert
        Assert.IsAssignableFrom<ViewResult>(result);
    }

    [Fact]
    public void Index_ModelIsNull()
    {
        // Arrange
        var controller = new EmployeesController();

        // Act
        var result = controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Null(viewResult.Model); // Since no model is passed to the view in the controller
    }

    [Fact]
    public void Index_ActionResultIsView()
    {
        // Arrange
        var controller = new EmployeesController();

        // Act
        var result = controller.Index();

        // Assert
        var actionResult = Assert.IsType<ActionResult>(result);
        Assert.IsType<ViewResult>(actionResult);
    }

    [Fact]
    public void Index_ReturnsViewResultWithDefaultViewName()
    {
        // Arrange
        var controller = new EmployeesController();

        // Act
        var result = controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Null(viewResult.ViewName); // Default view name will be used
    }
}
