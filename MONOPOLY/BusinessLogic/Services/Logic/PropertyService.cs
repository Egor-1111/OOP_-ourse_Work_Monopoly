using Monopoly.Domain.Enums;
using Monopoly.BusinessLogic.Services.Interfaces;
using Domain.Enums;
using Domain.Models;
namespace Monopoly.BusinessLogic.Services.Logic;

public class PropertyService : IPropertyService
{
    private readonly PropertyRepository _propertyRepository;

    public PropertyService(PropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }
    //Покупка свободной недвижимости
    public void BuyProperty(Player buyer, int propertyId) {
        var property = _propertyRepository.GetById(propertyId);

        if (property.Status != PropertyStatus.Onsale)
        {
            throw new InvalidOperationException("Недвижимость уже куплена.");
        }

        if (buyer.Balance < property.Price)
        {
            throw new InvalidOperationException("Недостаточео денег для покупки.");
        }
 
        buyer.Balance -= property.Price;
        property.Owner = buyer;
        property.Status = PropertyStatus.sold;
        buyer.Properties.Add(property);
        property.Owner = buyer;

        Console.WriteLine($"{buyer.Name} купил {property.Name} за {property.Price}$");
    }


    public void BuildHouse(Player player, int propertyId)
    {
        var property = _propertyRepository.GetById(propertyId);

        // Базовые проверки
        if (property.Type != PropertyType.street)
            throw new InvalidOperationException("Можно строить только на улицах.");

        if (property.Owner != player)
            throw new InvalidOperationException("Вы не владеете этой недвижимостью.");

        // Проверка, что у игрока все свойства этой группы
        var groupProperties = _propertyRepository.GetByGroup(property.Group);
        if (groupProperties.Any(p => p.Owner != player))
            throw new InvalidOperationException("У вас не все свойства этой группы!");

        // Проверка на максимальное количество домов (4 дома + 1 отель)
        if (property.KolHouse >= 5)
            throw new InvalidOperationException("Здесь уже построен отель. Дальше строить нельзя.");

        if (property.KolHouse == 4)
        {
            // Проверяем, можно ли строить отель (на всех улицах должно быть по 4 дома)
            if (groupProperties.Any(p => p.KolHouse < 4))
                throw new InvalidOperationException("Нельзя построить отель, пока на всех улицах группы нет по 4 дома.");

            // Строим отель
            player.Balance -= property.HouseCost;
            property.KolHouse = 5; // 5 = отель
            Console.WriteLine($"Отель построен на {property.Name}.");
            return;
        }

        // Проверка равномерности строительства
        int minHousesInGroup = groupProperties.Min(p => p.KolHouse);
        if (property.KolHouse > minHousesInGroup)
            throw new InvalidOperationException($"Сначала нужно построить дома на других улицах этой группы. Минимальное количество домов в группе: {minHousesInGroup}");

        // Проверка баланса
        if (player.Balance < property.HouseCost)
            throw new InvalidOperationException("Недостаточно средств.");

        // Строим дом
        player.Balance -= property.HouseCost;
        property.KolHouse++;
        Console.WriteLine($"Дом построен на {property.Name}. Всего домов: {property.KolHouse}");
    }


    public void MortgageProperty(Player player, int propertyId)
    {
        var property = _propertyRepository.GetById(propertyId);

        if (property.Owner != player)
            throw new InvalidOperationException("Вы не владеете этой недвижимостью.");

        if (property.KolHouse > 0)
            throw new InvalidOperationException("Нельзя заложить имущество с домами.");

        player.Balance += property.Price / 2;
        property.Status = PropertyStatus.pledged;
        Console.WriteLine($"{property.Name} заложено. Вы получили {property.Price / 2}$.");
    }

    // В PropertyService.cs
    public void SellHouse(Player player, int propertyId)
    {
        var property = _propertyRepository.GetById(propertyId);

        // Проверка базовых условий
        if (property.Type != PropertyType.street)
            throw new InvalidOperationException("Можно сносить только дома на улицах.");

        if (property.Owner != player)
            throw new InvalidOperationException("Вы не владеете этой недвижимостью.");

        if (property.KolHouse <= 0)
            throw new InvalidOperationException("Здесь нет домов для сноса.");

        // Получаем все свойства этой группы
        var groupProperties = _propertyRepository.GetByGroup(property.Group);

        // Проверка равномерности сноса (разница не более 1)
        int maxHousesInGroup = groupProperties.Max(p => p.KolHouse);
        if (property.KolHouse < maxHousesInGroup - 1)
            throw new InvalidOperationException("Сначала нужно снести дома на других улицах этой группы.");

        // Расчет возвращаемой суммы (половина стоимости дома)
        decimal refundAmount = property.HouseCost / 2;

        // Снос отеля (5 = отель)
        if (property.KolHouse == 5)
        {
            property.KolHouse = 4; // Возвращаем 4 дома
            player.Balance += refundAmount ; // Возвращаем за 4 дома
            Console.WriteLine($"Отель снесен на {property.Name}. Возвращено {refundAmount }$");
        }
        // Снос обычного дома
        else
        {
            property.KolHouse--;
            player.Balance += refundAmount;
            Console.WriteLine($"Снесен 1 дом на {property.Name}. Возвращено {refundAmount}$");
        }
    }

    // В PropertyService.cs
    public void UnmortgageProperty(Player player, int propertyId)
    {
        var property = _propertyRepository.GetById(propertyId);

        // Проверки
        if (property.Owner != player)
            throw new InvalidOperationException("Вы не владеете этой недвижимостью.");

        if (property.Status != PropertyStatus.pledged)
            throw new InvalidOperationException("Эта недвижимость не в залоге.");

        // Расчет стоимости выкупа (110% от залоговой стоимости) с округлением
        decimal unmortgageCost = Math.Round((property.Price / 2) * 1.1m, MidpointRounding.AwayFromZero);

        if (player.Balance < unmortgageCost)
            throw new InvalidOperationException($"Недостаточно средств. Нужно {unmortgageCost}$, у вас {player.Balance}$");

        // Выкуп
        player.Balance -= unmortgageCost;
        property.Status = PropertyStatus.sold;
        Console.WriteLine($"{property.Name} выкуплена из залога за {unmortgageCost}$");
    }
    
}