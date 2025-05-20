using Domain.Models;

namespace Monopoly.BusinessLogic.Services.Interfaces;

public interface IPropertyService
{
    // Покупка недвижимости у банка
    void BuyProperty(Player buyer, int propertyId);

    public void BuildHouse(Player player, int propertyId);

    public void SellHouse(Player player, int propertyId);

    public void UnmortgageProperty(Player player, int propertyId);

    public void MortgageProperty(Player player, int propertyId);

    
}