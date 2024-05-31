using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public static class DbSetMockHelper
{
    public static Mock<DbSet<T>> CreateDbSetMock<T>(List<T> elements) where T : class
    {
        var queryable = elements.AsQueryable();
        var dbSetMock = new Mock<DbSet<T>>();

        dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
        dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
        dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

        dbSetMock.Setup(d => d.AddAsync(It.IsAny<T>(), It.IsAny<CancellationToken>()))
            .Callback<T, CancellationToken>((s, _) => elements.Add(s))
            .ReturnsAsync((T _) => null);

        dbSetMock.Setup(d => d.Remove(It.IsAny<T>())).Callback<T>(s => elements.Remove(s));

        return dbSetMock;
    }
}
