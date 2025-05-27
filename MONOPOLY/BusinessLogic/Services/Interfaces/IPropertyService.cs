using Domain.Models;

namespace Monopoly.BusinessLogic.Services.Interfaces;

public interface IPropertyService
{
    // Покупка недвижимости у банка
    Task BuyProperty(Player buyer, int propertyId);

    Task BuildHouse(Player player, int propertyId);

    Task SellHouse(Player player, int propertyId);

    Task UnmortgageProperty(Player player, int propertyId);

    Task MortgageProperty(Player player, int propertyId);

    
}