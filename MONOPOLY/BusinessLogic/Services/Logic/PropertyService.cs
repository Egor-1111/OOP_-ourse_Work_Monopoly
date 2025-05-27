using Monopoly.Domain.Enums;
using Monopoly.BusinessLogic.Services.Interfaces;
using Domain.Enums;
using Domain.Models;
using BusinessLogic.Services.Interfaces;
using System.Threading.Tasks;
namespace Monopoly.BusinessLogic.Services.Logic;

public class PropertyService : IPropertyService
{
    private readonly PropertyRepository _propertyRepository;


    public PropertyService(PropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }
    private IGameUIHandler? _uiHandler;

    public void SetUIHandler(IGameUIHandler handler)
    {
        _uiHandler = handler;
    }
    //Покупка свободной недвижимости
    public async Task BuyProperty(Player buyer, int propertyId) {
        var property = _propertyRepository.GetById(propertyId);

        if (buyer.Balance < property.Price)
        {
            await _uiHandler.ShowMessageAsync("Недостаточео денег для покупки.");
        }
 
        buyer.Balance -= property.Price;
        property.Owner = buyer;
        property.Status = PropertyStatus.sold;
        buyer.Properties.Add(property);
        property.Owner = buyer;

    }


    public async Task BuildHouse(Player player, int propertyId)
    {
        var property = _propertyRepository.GetById(propertyId)
            ?? throw new InvalidOperationException("Участок не найден.");

        if (property.Type != PropertyType.street)
            throw new InvalidOperationException("Можно строить только на улицах.");

        if (property.Owner != player)
            throw new InvalidOperationException("Вы не владеете этой недвижимостью.");

        var groupProperties = _propertyRepository.GetByGroup(property.Group);
        if (groupProperties.Any(p => p.Owner != player))
            throw new InvalidOperationException("У вас не все свойства этой группы.");

        if (property.KolHouse >= 5)
            throw new InvalidOperationException("Здесь уже построен отель.");

        if (property.KolHouse == 4)
        {
            if (groupProperties.Any(p => p.KolHouse < 4))
                throw new InvalidOperationException("Нельзя построить отель: не на всех улицах 4 дома.");

            if (player.Balance < property.HouseCost)
                throw new InvalidOperationException("Недостаточно средств для строительства отеля.");

            player.Balance -= property.HouseCost;
            property.KolHouse = 5; // Отель
            return;
        }

        int minHousesInGroup = groupProperties.Min(p => p.KolHouse);
        if (property.KolHouse > minHousesInGroup)
            throw new InvalidOperationException($"Сначала выровняйте дома на других улицах. Минимум: {minHousesInGroup}");

        if (player.Balance < property.HouseCost)
            throw new InvalidOperationException("Недостаточно средств.");

        player.Balance -= property.HouseCost;
        property.KolHouse++;
    }



    public async Task MortgageProperty(Player player, int propertyId)
    {
        var property = _propertyRepository.GetById(propertyId)
            ?? throw new InvalidOperationException("Недвижимость не найдена.");

        if (property.Owner != player)
            throw new InvalidOperationException("Вы не владеете этой недвижимостью.");

        if (property.KolHouse > 0)
            throw new InvalidOperationException("Нельзя заложить имущество с постройками.");

        player.Balance += property.Price / 2;
        property.Status = PropertyStatus.pledged;
    }
    // В PropertyService.cs
    public async Task SellHouse(Player player, int propertyId)
    {
        var property = _propertyRepository.GetById(propertyId)
            ?? throw new InvalidOperationException("Недвижимость не найдена.");

        if (property.Type != PropertyType.street)
            throw new InvalidOperationException("Снос возможен только на улицах.");

        if (property.Owner != player)
            throw new InvalidOperationException("Вы не владеете этой недвижимостью.");

        if (property.KolHouse <= 0)
            throw new InvalidOperationException("На этом участке нет построек.");

        var groupProperties = _propertyRepository.GetByGroup(property.Group);
        int maxHousesInGroup = groupProperties.Max(p => p.KolHouse);

        if (property.KolHouse < maxHousesInGroup - 1)
            throw new InvalidOperationException("Нарушение равномерности: сначала снесите дома на других улицах.");

        decimal refundAmount = property.HouseCost / 2;

        if (property.KolHouse == 5)
        {
            property.KolHouse = 4;
            player.Balance += refundAmount;
        }
        else
        {
            property.KolHouse--;
            player.Balance += refundAmount;
        }
    }


    // В PropertyService.cs
    public async Task UnmortgageProperty(Player player, int propertyId)
    {
        var property = _propertyRepository.GetById(propertyId)
            ?? throw new InvalidOperationException("Недвижимость не найдена.");

        if (property.Owner != player)
            throw new InvalidOperationException("Вы не владеете этой недвижимостью.");

        if (property.Status != PropertyStatus.pledged)
            throw new InvalidOperationException("Недвижимость не находится в залоге.");

        decimal unmortgageCost = Math.Round((property.Price / 2) * 1.1m, MidpointRounding.AwayFromZero);

        if (player.Balance < unmortgageCost)
            throw new InvalidOperationException($"Недостаточно средств. Нужно {unmortgageCost}$, у вас {player.Balance}$");

        player.Balance -= unmortgageCost;
        property.Status = PropertyStatus.sold;
    }


}