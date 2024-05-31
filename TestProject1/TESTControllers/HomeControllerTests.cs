using ASPNET_CORE_MVC_CRUD.Controllers;
using ASPNET_CORE_MVC_CRUD.Data;
using ASPNET_CORE_MVC_CRUD.Models;
using ASPNET_CORE_MVC_CRUD.Models.CustomerModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class HomeControllerTests
{
    private List<Customer> GetTestCustomers()
    {
        return new List<Customer>
        {
            new Customer { Id = Guid.NewGuid(), Name = "Alice", Email = "alice@example.com", Phone = "1234567890", Address = "123 Street", DateOfBirth = DateTime.Parse("1990-01-01") },
            new Customer { Id = Guid.NewGuid(), Name = "Bob", Email = "bob@example.com", Phone = "0987654321", Address = "456 Avenue", DateOfBirth = DateTime.Parse("1985-05-05") }
        };
    }

    [Fact]
    public async Task Index_ReturnsAViewResult_WithAListOfCustomers()
    {
        // Arrange
        var mockSet = DbSetMockHelper.CreateDbSetMock(GetTestCustomers());
        var mockContext = new Mock<DataContext>();
        mockContext.Setup(m => m.Customers).Returns(mockSet.Object);
        var mockLogger = new Mock<ILogger<HomeController>>();
        var controller = new HomeController(mockLogger.Object, mockContext.Object);

        // Act
        var result = await controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Customer>>(viewResult.ViewData.Model);
        Assert.Equal(2, model.Count());
    }

    [Fact]
    public void Privacy_ReturnsAViewResult()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<HomeController>>();
        var mockContext = new Mock<DataContext>();
        var controller = new HomeController(mockLogger.Object, mockContext.Object);

        // Act
        var result = controller.Privacy();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Error_ReturnsAViewResult_WithErrorViewModel()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<HomeController>>();
        var mockContext = new Mock<DataContext>();
        var controller = new HomeController(mockLogger.Object, mockContext.Object);
        var expectedRequestId = "testRequestId";
        var activity = new Activity("test");
        activity.Start();
        activity.SetIdFormat(ActivityIdFormat.W3C);
        Activity.Current = activity;

        // Act
        var result = controller.Error();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<ErrorViewModel>(viewResult.ViewData.Model);
        Assert.Equal(expectedRequestId, model.RequestId);
        Assert.True(model.ShowRequestId);
    }

    [Fact]
    public async Task Index_ReturnsEmptyListWhenNoCustomers()
    {
        // Arrange
        var mockSet = DbSetMockHelper.CreateDbSetMock(new List<Customer>());
        var mockContext = new Mock<DataContext>();
        mockContext.Setup(m => m.Customers).Returns(mockSet.Object);
        var mockLogger = new Mock<ILogger<HomeController>>();
        var controller = new HomeController(mockLogger.Object, mockContext.Object);

        // Act
        var result = await controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Customer>>(viewResult.ViewData.Model);
        Assert.Empty(model);
    }

    [Fact]
    public void Error_ReturnsViewResult_WithCorrectRequestId()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<HomeController>>();
        var mockContext = new Mock<DataContext>();
        var controller = new HomeController(mockLogger.Object, mockContext.Object);
        Activity.Current = new Activity("test");
        Activity.Current.Start();
        var expectedRequestId = Activity.Current.Id;

        // Act
        var result = controller.Error();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<ErrorViewModel>(viewResult.ViewData.Model);
        Assert.Equal(expectedRequestId, model.RequestId);
    }

    [Fact]
    public void Privacy_ReturnsViewResult_NotNull()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<HomeController>>();
        var mockContext = new Mock<DataContext>();
        var controller = new HomeController(mockLogger.Object, mockContext.Object);

        // Act
        var result = controller.Privacy();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Error_SetsResponseCacheAttributes()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<HomeController>>();
        var mockContext = new Mock<DataContext>();
        var controller = new HomeController(mockLogger.Object, mockContext.Object);

        // Act
        var methodInfo = typeof(HomeController).GetMethod(nameof(HomeController.Error));
        var attributes = methodInfo.GetCustomAttributes(typeof(ResponseCacheAttribute), false);

        // Assert
        var responseCacheAttribute = Assert.Single(attributes) as ResponseCacheAttribute;
        Assert.NotNull(responseCacheAttribute);
        Assert.Equal(0, responseCacheAttribute.Duration);
        Assert.Equal(ResponseCacheLocation.None, responseCacheAttribute.Location);
        Assert.True(responseCacheAttribute.NoStore);
    }

    [Fact]
    public void Index_ReturnsViewResult_WithCorrectType()
    {
        // Arrange
        var mockSet = DbSetMockHelper.CreateDbSetMock(GetTestCustomers());
        var mockContext = new Mock<DataContext>();
        mockContext.Setup(m => m.Customers).Returns(mockSet.Object);
        var mockLogger = new Mock<ILogger<HomeController>>();
        var controller = new HomeController(mockLogger.Object, mockContext.Object);

        // Act
        var result = controller.Index();

        // Assert
        Assert.IsType<Task<IActionResult>>(result);
    }
}
