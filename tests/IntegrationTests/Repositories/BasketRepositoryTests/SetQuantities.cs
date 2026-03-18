using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Microsoft.eShopWeb.ApplicationCore.Services;
using Microsoft.eShopWeb.Infrastructure.Data;
using Microsoft.eShopWeb.UnitTests.Builders;
using Xunit;

namespace Microsoft.eShopWeb.IntegrationTests.Repositories.BasketRepositoryTests;

public class SetQuantities
{
    private readonly CatalogContext _catalogContext;
    private readonly EfRepository<Basket> _basketRepository;
    private readonly BasketBuilder _basketBuilder = new();

    public SetQuantities()
    {
        var dbOptions = new DbContextOptionsBuilder<CatalogContext>()
            .UseInMemoryDatabase(databaseName: "TestCatalog")
            .Options;
        _catalogContext = new CatalogContext(dbOptions);
        _basketRepository = new EfRepository<Basket>(_catalogContext);
    }

    [Fact]
    public async Task RemoveEmptyQuantities()
    {
        var basket = _basketBuilder.WithOneBasketItem(); //Arrange
        var basketService = new BasketService(_basketRepository, null); //Arrange
        await _basketRepository.AddAsync(basket, TestContext.Current.CancellationToken); //Act
        _catalogContext.SaveChanges();
        //Act
        await basketService.SetQuantities(_basketBuilder.BasketId, new Dictionary<string, int>() { { _basketBuilder.BasketId.ToString(), 0 } });

        Assert.Empty(basket.Items); //Assert
    }
}
