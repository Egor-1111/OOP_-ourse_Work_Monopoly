using Domain.Models;
using Monopoly.Domain.Enums;
using Monopoly.BusinessLogic.Services.Interfaces;
using BusinessLogic.Services.Logic;
using Domain.Enums;
using BusinessLogic.Services.Interfaces;


namespace Monopoly.BusinessLogic.Services.Logic;

public class PlayerService : IPlayerService
{
    private readonly PropertyRepository _propertyRepository;

    public PlayerService(PropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public void SendToJail(Player player)
    {
        player.Position = 10;
        player.Status = PlayerStatus.InJail;
        player.KolInTurma = 0;
        Console.WriteLine($"{player.Name} отправлен в тюрьму!");
    }



    public void AttemptToLeaveJail(Player player, Dice dice)
    {


        dice.Roll();
        Console.WriteLine($"Игрок {player.Name} бросает кубики: {dice.Value1} и {dice.Value2}");

        if (dice.IsDouble)
        {
            player.Status = PlayerStatus.Active;
            player.KolInTurma = 0;
            player.DoubleThenTurma = 1;
            Console.WriteLine($"{player.Name} выбросил дубль и выходит из тюрьмы!");
            return;
        }

        player.KolInTurma++;

        if (player.KolInTurma >= 3)
        {
            // Автоматическая оплата после 3 ходов
            PayToLeaveJail(player);
        }
        else
        {
            Console.WriteLine($"{player.Name} остается в тюрьме. Ход {player.KolInTurma}/2");
           
        }
    }

    public void PayToLeaveJail(Player player)
    {
        player.Balance -= 50;
        player.Status = PlayerStatus.Active;
        player.KolInTurma = 0;
        Console.WriteLine($"{player.Name} заплатил {50}$ и вышел из тюрьмы.");
    }
    public void  MovePlear (Player player,int step)
    {
         player.Position += step;
        if(player.Position > 39)
        {
            player.Balance += 200;
        }
        int newpoz = player.Position % 40;
        player.Position = newpoz;
        Console.WriteLine($"Игрок перешел на клетку {player.Position}");
    }

    public void GetBalace(Player player) {
        Console.WriteLine($"Балнс у угрока {player.Balance}");    
    }

    // оплата оренды для улиц
    public void PayRentForStreet(Player otdast, Player polych , int propertyId) {
        var property = _propertyRepository.GetById(propertyId);
        int rent = 0;
        if (property.KolHouse == 0) rent = property.BaseRent;
        else if (property.KolHouse == 1) rent = property.RentWith1House;
        else if (property.KolHouse == 2) rent = property.RentWith2Houses;
        else if (property.KolHouse == 3) rent = property.RentWith3Houses;
        else if (property.KolHouse == 4) rent = property.RentWith4Houses;
        else if (property.KolHouse == 5) rent = property.RentWithHotel;

        otdast.Balance -= rent;
        polych.Balance += rent;

    }
    // оплата оренды для дорог
    public void PayRentForDoroga(Player otdast, Player polych, int propertyId)
    {
        int rent_otdast = 0;
        var property = _propertyRepository.GetById(propertyId);
        var allDorogi = _propertyRepository.GetByGroup(PropertyGroup.doroga);

        // Считаем сколько дорог принадлежит игроку-получателю
        int kolDorogVladelca = allDorogi.Count(d => d.Owner == polych);
        polych.KolDorog = kolDorogVladelca;
        int rent = property.BaseRent;
        if (polych.KolDorog == 1) rent_otdast = rent;
        if (polych.KolDorog == 2) rent_otdast = rent*2;
        if (polych.KolDorog == 3) rent_otdast = rent*4;
        if (polych.KolDorog == 4) rent_otdast = rent*8;
        otdast.Balance -= rent;
        polych.Balance += rent;

    }
    // оплата оренды для коммунальных предприятий
    public void PayRentForCommunal(Player otdast, Player polych, int propertyId,Dice dice)
    {
        var property = _propertyRepository.GetById(propertyId);
        var allCommunal = _propertyRepository.GetByGroup(PropertyGroup.communal);
        int kolCommunalVladelca = allCommunal.Count(d => d.Owner == polych);
        polych.KolComunal = kolCommunalVladelca;
        int rent = 0;
        Console.WriteLine($"Кубик бросает {polych.Name}. Нажмите Enter чтобы бросить кубики...");
        Console.ReadLine();
        // Бросок кубиков
        dice.Roll();
        if (kolCommunalVladelca == 1)
        {
            Console.WriteLine($"Выпало: {dice.Total} тоесть игрок {polych.Name} получит {dice.Total} * 4");
            rent = dice.Total * 4;
        }else if(kolCommunalVladelca == 2)
        {
            Console.WriteLine($"Выпало: {dice.Total} тоесть игрок {polych.Name} получит {dice.Total} * 10");
            rent = dice.Total * 10;
        }
        otdast.Balance -= rent;
        polych.Balance += rent;
    }

    public void TradeWithPlayer(Player currentPlayer, Player otherPlayer, List<PropertyData> currentPlayerProperties, List<PropertyData> otherPlayerProperties, decimal moneyOffered = 0)
    {

        Console.WriteLine("Игрок с которам обмениваются согласен (y/n)?");
        var input = Console.ReadLine();

        if (input?.ToLower() == "y")
        {
            // Проверка, что у игроков есть выбранные свойства
            if (currentPlayerProperties.Any(p => p.Owner != currentPlayer))
                throw new InvalidOperationException("У текущего игрока нет некоторых из выбранных свойств.");

            if (otherPlayerProperties.Any(p => p.Owner != otherPlayer))
                throw new InvalidOperationException("У другого игрока нет некоторых из выбранных свойств.");

            // Проверка баланса, если есть денежная часть обмена
            if (moneyOffered > 0 && currentPlayer.Balance < moneyOffered)
                throw new InvalidOperationException("Недостаточно средств для обмена.");


            // Передача свойств
            foreach (var prop in currentPlayerProperties)
            {
                prop.Owner = otherPlayer;
                otherPlayer.Properties.Add(prop);
                currentPlayer.Properties.Remove(prop);
            }

            foreach (var prop in otherPlayerProperties)
            {
                prop.Owner = currentPlayer;
                currentPlayer.Properties.Add(prop);
                otherPlayer.Properties.Remove(prop);
            }

            // Передача денег (если есть)
            if (moneyOffered > 0)
            {
                currentPlayer.Balance -= moneyOffered;
                otherPlayer.Balance += moneyOffered;
            }

            Console.WriteLine("Обмен успешно завершен!");
        }
        else
        {
            Console.WriteLine("Обмен не завершен!");
        }
    }

    
}