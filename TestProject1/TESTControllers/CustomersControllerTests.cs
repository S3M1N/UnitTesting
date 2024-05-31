using ASPNET_CORE_MVC_CRUD.Controllers;
using ASPNET_CORE_MVC_CRUD.Data;
using ASPNET_CORE_MVC_CRUD.Models.CustomerModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class CustomersControllerTests
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
        var customers = GetTestCustomers();
        var mockSet = DbSetMockHelper.CreateDbSetMock(customers);
        var mockContext = new Mock<DataContext>();
        mockContext.Setup(m => m.Customers).Returns(mockSet.Object);
        var controller = new CustomersController(mockContext.Object);

        // Act
        var result = await controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Customer>>(viewResult.ViewData.Model);
        Assert.Equal(2, model.Count());
    }

    [Fact]
    public async Task Add_Post_AddsCustomerAndRedirects()
    {
        // Arrange
        var customers = GetTestCustomers();
        var mockSet = DbSetMockHelper.CreateDbSetMock(customers);
        var mockContext = new Mock<DataContext>();
        mockContext.Setup(m => m.Customers).Returns(mockSet.Object);
        var controller = new CustomersController(mockContext.Object);
        var newCustomer = new AddCutomerViewModel
        {
            Name = "Charlie",
            Email = "charlie@example.com",
            Phone = "1122334455",
            Address = "789 Boulevard",
            DateOfBirth = DateTime.Parse("2000-12-12")
        };

        // Act
        var result = await controller.Add(newCustomer);

        // Assert
        mockSet.Verify(m => m.AddAsync(It.IsAny<Customer>(), default), Times.Once());
        mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once());
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToActionResult.ActionName);
    }

    [Fact]
    public async Task Index_Post_ReturnsFilteredCustomers()
    {
        // Arrange
        var customers = GetTestCustomers();
        var mockSet = DbSetMockHelper.CreateDbSetMock(customers);
        var mockContext = new Mock<DataContext>();
        mockContext.Setup(m => m.Customers).Returns(mockSet.Object);
        var controller = new CustomersController(mockContext.Object);
        var filter = "Alice";

        // Act
        var result = await controller.Index(filter);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Customer>>(viewResult.ViewData.Model);
        Assert.Single(model);
        Assert.Equal("Alice", model.First().Name);
    }

    [Fact]
    public async Task Update_Get_ReturnsUpdateView_WithCorrectCustomer()
    {
        // Arrange
        var testCustomers = GetTestCustomers();
        var mockSet = DbSetMockHelper.CreateDbSetMock(testCustomers);
        var mockContext = new Mock<DataContext>();
        mockContext.Setup(m => m.Customers).Returns(mockSet.Object);
        var controller = new CustomersController(mockContext.Object);
        var testCustomerId = testCustomers[0].Id;

        // Act
        var result = await controller.Update(testCustomerId);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<UpdateCustomerViewModel>(viewResult.ViewData.Model);
        Assert.Equal(testCustomerId, model.Id);
        Assert.Equal("Alice", model.Name);
    }

    [Fact]
    public async Task Update_Post_UpdatesCustomerAndRedirects()
    {
        // Arrange
        var testCustomers = GetTestCustomers();
        var mockSet = DbSetMockHelper.CreateDbSetMock(testCustomers);
        var mockContext = new Mock<DataContext>();
        mockContext.Setup(m => m.Customers).Returns(mockSet.Object);
        var controller = new CustomersController(mockContext.Object);
        var updatedCustomer = new UpdateCustomerViewModel
        {
            Id = testCustomers[0].Id,
            Name = "Alice Updated",
            Email = "alice.updated@example.com",
            Phone = "9999999999",
            Address = "123 Updated Street",
            DateOfBirth = DateTime.Parse("1990-01-01")
        };

        // Act
        var result = await controller.Update(updatedCustomer);

        // Assert
        mockSet.Verify(m => m.FindAsync(It.IsAny<Guid>()), Times.Once());
        mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once());
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToActionResult.ActionName);
        Assert.Equal("Alice Updated", testCustomers[0].Name);
    }

    [Fact]
    public async Task Delete_Get_ReturnsDeleteView_WithCorrectCustomer()
    {
        // Arrange
        var testCustomers = GetTestCustomers();
        var mockSet = DbSetMockHelper.CreateDbSetMock(testCustomers);
        var mockContext = new Mock<DataContext>();
        mockContext.Setup(m => m.Customers).Returns(mockSet.Object);
        var controller = new CustomersController(mockContext.Object);
        var testCustomerId = testCustomers[0].Id;

        // Act
        var result = await controller.Delete(testCustomerId);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<DeleteCustomerViewModel>(viewResult.ViewData.Model);
        Assert.Equal(testCustomerId, model.Id);
        Assert.Equal("Alice", model.Name);
    }

    [Fact]
    public async Task Delete_Post_DeletesCustomerAndRedirects()
    {
        // Arrange
        var testCustomers = GetTestCustomers();
        var mockSet = DbSetMockHelper.CreateDbSetMock(testCustomers);
        var mockContext = new Mock<DataContext>();
        mockContext.Setup(m => m.Customers).Returns(mockSet.Object);
        var controller = new CustomersController(mockContext.Object);
        var testCustomerId = testCustomers[0].Id;

        // Act
        var result = await controller.Delete(new DeleteCustomerViewModel { Id = testCustomerId });

        // Assert
        mockSet.Verify(m => m.Remove(It.IsAny<Customer>()), Times.Once());
        mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once());
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToActionResult.ActionName);
        Assert.DoesNotContain(testCustomers, c => c.Id == testCustomerId);
    }

    [Fact]
    public async Task Add_Post_InvalidModel_ReturnsViewWithModel()
    {
        // Arrange
        var customers = GetTestCustomers();
        var mockSet =
DbSetMockHelper.CreateDbSetMock(customers);
        var mockContext = new Mock<DataContext>();
        mockContext.Setup(m => m.Customers).Returns(mockSet.Object);
        var controller = new CustomersController(mockContext.Object);
        controller.ModelState.AddModelError("Name", "Required");
        var newCustomer = new AddCutomerViewModel();

        // Act
        var result = await controller.Add(newCustomer);
        //Projectt
        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<AddCutomerViewModel>(viewResult.ViewData.Model);
        Assert.Equal(newCustomer, model);
    }

    [Fact]
    public async Task Update_Post_InvalidModel_ReturnsViewWithModel()
    {
        // Arrange
        var testCustomers = GetTestCustomers();
        var mockSet = DbSetMockHelper.CreateDbSetMock(testCustomers);
        var mockContext = new Mock<DataContext>();
        mockContext.Setup(m => m.Customers).Returns(mockSet.Object);
        var controller = new CustomersController(mockContext.Object);
        controller.ModelState.AddModelError("Name", "Required");
        var updatedCustomer = new UpdateCustomerViewModel { Id = testCustomers[0].Id };

        // Act
        var result = await controller.Update(updatedCustomer);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<UpdateCustomerViewModel>(viewResult.ViewData.Model);
        Assert.Equal(updatedCustomer, model);
    }

    [Fact]
    public async Task Delete_Get_InvalidId_ReturnsRedirectToIndex()
    {
        // Arrange
        var testCustomers = GetTestCustomers();
        var mockSet = DbSetMockHelper.CreateDbSetMock(testCustomers);
        var mockContext = new Mock<DataContext>();
        mockContext.Setup(m => m.Customers).Returns(mockSet.Object);
        var controller = new CustomersController(mockContext.Object);
        var invalidId = Guid.NewGuid();

        // Act
        var result = await controller.Delete(invalidId);

        
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToActionResult.ActionName);
    }

    [Fact]
    public async Task Update_Get_InvalidId_ReturnsRedirectToIndex()
    {
        // Arrange
        var testCustomers = GetTestCustomers();
        var mockSet = DbSetMockHelper.CreateDbSetMock(testCustomers);
        var mockContext = new Mock<DataContext>();
        mockContext.Setup(m => m.Customers).Returns(mockSet.Object);
        var controller = new CustomersController(mockContext.Object);
        var invalidId = Guid.NewGuid();

        // Act
        var result = await controller.Update(invalidId);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToActionResult.ActionName);
    }
}
