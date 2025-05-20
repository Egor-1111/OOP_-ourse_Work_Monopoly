using System;
using System.Numerics;
using System.Threading.Tasks;
using BusinessLogic.Services.Interfaces;
using BusinessLogic.Services.Logic;
using Domain.Enums;
using Domain.Models;
using Monopoly.BusinessLogic.Services.Interfaces;
using Monopoly.BusinessLogic.Services.Logic;
using Monopoly.Domain.Enums;


public class GameService : IGameService
{
    private readonly List<Player> _players;
    private int _currentPlayerIndex;
    private readonly IPlayerService _playerService;
    private readonly Dice _dice;
    private readonly IPropertyService _propertyService;
    private readonly PropertyRepository _propertyRepository;
    private readonly CardService _cardService;
    private readonly IGameUIHandler _uiHandler;

    public GameService(IPlayerService playerService, IPropertyService propertyService, PropertyRepository propertyRepository, IGameUIHandler uiHandler)
    {
        _playerService = playerService;
        _propertyService = propertyService;
        _propertyRepository = propertyRepository;
        _players = new List<Player>();
        _currentPlayerIndex = 0;
        _dice = new Dice();
        _cardService = new CardService();
         _uiHandler = uiHandler;
    }

   
    public async Task Start()
    {
        await _uiHandler.UpdateLabelAsync("Игра запускается...");

        await Task.Delay(1000);

        await _uiHandler.ShowMessageAsync("Добро пожаловать в Монополию!");

        await _uiHandler.UpdateLabelAsync("Первый игрок ходит!");
        await InitializePlayersAsync();

        while (!GameOver())
        {
            Player currentPlayer = GetCurrentPlayer();
            await _uiHandler.ShowMessageAsync($"\nХод игрока {currentPlayer.Name}");

            if (currentPlayer.Status == PlayerStatus.InJail)
            {
                if (currentPlayer.KolInTurma == 2)
                {
                    _playerService.PayToLeaveJail(currentPlayer);
                    await _uiHandler.ShowMessageAsync($"{currentPlayer.Name} заплатил и вышел из тюрьмы.");

                }
                else if(currentPlayer.Status == PlayerStatus.InJail){
                    int jailChoice = await _uiHandler.ShowChoiceAsync(
                        $"{currentPlayer.Name} в тюрьме. Выберите действие:",
                        "Попытаться выкинуть дубль",
                        "Заплатить $50 за выход",
                        "Использовать карточку 'Освобождение' (если есть)");


                    switch (jailChoice)
                    {
                        case 1:
                            _playerService.AttemptToLeaveJail(currentPlayer, _dice);
                            break;
                        case 2:
                            _playerService.PayToLeaveJail(currentPlayer);
                            break;
                        case 3:
                            if (currentPlayer.HasGetOutOfJailFreeCard)
                            {
                                currentPlayer.HasGetOutOfJailFreeCard = false;
                                currentPlayer.Status = PlayerStatus.Active;
                                currentPlayer.KolInTurma = 0;
                                await _uiHandler.ShowMessageAsync("Использована карточка 'Освобождение из тюрьмы'");
                                // Вернуть карточку в колоду
                                _cardService.ReturnCard(new CommunityChestCard { GetOutOfJailFree = true });
                            }
                            else
                            {
                                await _uiHandler.ShowMessageAsync("У вас нет карточки 'Освобождение из тюрьмы'");
                            }
                            break;
                    } 
                }
                

            }

            if (currentPlayer.Status == PlayerStatus.Active)
            {
                if (currentPlayer.DoubleThenTurma == 0)
                {
                    await _uiHandler.GetInputAsync("Нажмите Enter чтобы бросить кубики...");
                    // Бросок кубиков
                    _dice.Roll();
                    await _uiHandler.ShowMessageAsync($"Выпало: {_dice.Value1} и {_dice.Value2} — всего {_dice.Total}");
                }
                currentPlayer.DoubleThenTurma = 0;

                if (_dice.DoubleCount != 3)
                {
                    // Обработка хода
                    _playerService.MovePlear(currentPlayer, _dice.Total);

                    int posPlear = currentPlayer.Position;
                    var propertyRep = _propertyRepository.GetById(posPlear);
                    ///////
                    await HandlePropertyCellAsync(currentPlayer, propertyRep);
                    ///////
                    _playerService.GetBalace(currentPlayer);

                    await HandlePlayerActionsAsync(currentPlayer);
                }


                // Переход хода (если не выпал дубль)
                if (!_dice.IsDouble)
                {

                    MoveToNextPlayer();
                }
                else
                {
                    if (_dice.DoubleCount == 3)
                    {
                        _playerService.SendToJail(currentPlayer);
                        _dice.DoubleCount = 0;
                        MoveToNextPlayer();
                    }
                    else
                    {
                        await _uiHandler.ShowMessageAsync("Дубль! Игрок ходит еще раз");
                    }
                }
            }
            if (currentPlayer.Status == PlayerStatus.InJail)
            {
                MoveToNextPlayer();
            }
        }
        await _uiHandler.ShowMessageAsync("Игра окончена!");
    }

    private List<PropertyData> ParsePropertyInput(string input, Player owner)
    {
        var ids = input.Split(',').Select(int.Parse).ToList();
        return owner.Properties.Where(p => ids.Contains(p.Id)).ToList();
    }
    public async Task InitializePlayersAsync()
    {
        int playerCount = 0;
        while (playerCount < 2 || playerCount > 4)
        {
            string input = await _uiHandler.GetInputAsync("Введите количество игроков (2-4):");
            int.TryParse(input, out playerCount);
        }

        for (int i = 0; i < playerCount; i++)
        {
            string name = await _uiHandler.GetInputAsync($"Введите имя игрока {i + 1}:");
            _players.Add(new Player(name));
        }
    }
    public Player GetCurrentPlayer()
    {
        // Возвращаем текущего игрока
        return _players[_currentPlayerIndex];
    }

    public void MoveToNextPlayer()
    {
        if (_players.Count == 0) return;

        // Переходим к следующему игроку
        _currentPlayerIndex = (_currentPlayerIndex + 1) % _players.Count;

        // Пропускаем банкротов
        while (GetCurrentPlayer().IsBankrupt)
        {
            _currentPlayerIndex = (_currentPlayerIndex + 1) % _players.Count;
        }
    }

    private bool GameOver()
    {
        // Игра заканчивается, когда остался 1 небанкрот
        return _players.Count(p => !p.IsBankrupt) <= 1;
    }

    private void ProcessCardEffect(Player player, Card card)
    {
        if (card is ChanceCard chanceCard)
        {
            ApplyChanceCardEffect(player, chanceCard);
        }
        else if (card is CommunityChestCard communityChestCard)
        {
            ApplyCommunityChestCardEffect(player, communityChestCard);
        }
    }

    private async Task ApplyChanceCardEffect(Player player, ChanceCard card)
    {
        if (card.MoneyChange != 0)
        {
            player.Balance += card.MoneyChange;
            await _uiHandler.ShowMessageAsync($"Баланс изменен на {card.MoneyChange}$. Текущий баланс: {player.Balance}$");
        }

        if (card.MoveToPosition.HasValue)
        {
            player.Position = card.MoveToPosition.Value;
            await _uiHandler.ShowMessageAsync($"Вы перемещены на позицию {card.MoveToPosition.Value}");
        }

        if (card.GoToJail)
        {
            _playerService.SendToJail(player);
        }

        if (card.GetOutOfJailFree)
        {
            // Здесь нужно добавить логику для карточки "Освобождение из тюрьмы"
            await _uiHandler.ShowMessageAsync("Вы получили карточку 'Освобождение из тюрьмы'");
        }
    }

    private async Task ApplyCommunityChestCardEffect(Player player, CommunityChestCard card)
    {
        if (card.MoneyChange != 0)
        {
            player.Balance += card.MoneyChange;
            await _uiHandler.ShowMessageAsync($"Баланс изменен на {card.MoneyChange}$. Текущий баланс: {player.Balance}$");
        }

        if (card.MoveToPosition.HasValue)
        {
            player.Position = card.MoveToPosition.Value;
            await _uiHandler.ShowMessageAsync($"Вы перемещены на позицию {card.MoveToPosition.Value}");
        }

        if (card.GetOutOfJailFree)
        {
            // Здесь нужно добавить логику для карточки "Освобождение из тюрьмы"
            await _uiHandler.ShowMessageAsync("Вы получили карточку 'Освобождение из тюрьмы'");
        }
    }
    //+++++++
    async Task HandlePropertyCellAsync(Player currentPlayer, PropertyData propertyRep)
    {
        if (propertyRep != null && propertyRep.Status == PropertyStatus.Onsale && propertyRep.Group != PropertyGroup.kazna
                        && propertyRep.Group != PropertyGroup.shans && propertyRep.Group != PropertyGroup.kazna
                        && propertyRep.Group != PropertyGroup.kazna100 && propertyRep.Group != PropertyGroup.kazna200
                        && propertyRep.Group != PropertyGroup.prostou && propertyRep.Group != PropertyGroup.turma && propertyRep.Group != PropertyGroup.start)
        {
            await _uiHandler.ShowMessageAsync($"Вы можете купить {propertyRep.Name} за {propertyRep.Price}$");
            var input = await _uiHandler.GetInputAsync("Хотите купить? (y/n): ");

            if (input?.ToLower() == "y")
            {
                try
                {
                    _propertyService.BuyProperty(currentPlayer, currentPlayer.Position);
                }
                catch (Exception ex)
                {
                    await _uiHandler.ShowMessageAsync($"Ошибка: {ex.Message}");
                }
            }
        }
        else if (propertyRep.Group == PropertyGroup.kazna)
        {
            var card = _cardService.DrawCommunityChestCard();
            await _uiHandler.ShowMessageAsync($"Карточка казны: {card.Text}");
            ProcessCardEffect(currentPlayer, card);
        }
        else if (propertyRep.Group == PropertyGroup.shans)
        {
            var card = _cardService.DrawChanceCard();
            await _uiHandler.ShowMessageAsync($"Карточка шанса: {card.Text}");
            ProcessCardEffect(currentPlayer, card);
        }
        else if (propertyRep.Group == PropertyGroup.kazna100)
        {
            currentPlayer.Balance -= 100;
        }
        else if (propertyRep.Group == PropertyGroup.kazna200)
        {
            currentPlayer.Balance -= 200;
        }
        else if (propertyRep.Group == PropertyGroup.turma)
        {
            _playerService.SendToJail(currentPlayer);
        }
        else if (propertyRep.Group == PropertyGroup.prostou)
        {
            await _uiHandler.ShowMessageAsync("Постойте");
        }
        else if (propertyRep.Group == PropertyGroup.start)
        {
            await _uiHandler.ShowMessageAsync("Вы перешли на клетку старт и просто получаете 200");
        }
        else if (propertyRep.Status == PropertyStatus.sold && propertyRep.Type == PropertyType.street && currentPlayer != propertyRep.Owner)
        {
            var poluch = propertyRep.Owner;

            _playerService.PayRentForStreet(currentPlayer, poluch, currentPlayer.Position);
        }
        else if (propertyRep.Status == PropertyStatus.sold && propertyRep.Type == PropertyType.doroga && currentPlayer != propertyRep.Owner)
        {
            var poluch = propertyRep.Owner;

            _playerService.PayRentForDoroga(currentPlayer, poluch, currentPlayer.Position);
        }
        else if (propertyRep.Status == PropertyStatus.sold && propertyRep.Type == PropertyType.communal && currentPlayer != propertyRep.Owner)
        {
            var poluch = propertyRep.Owner;

            _playerService.PayRentForCommunal(currentPlayer, poluch, currentPlayer.Position, _dice);
        }
    }
    //+++++++

    private async Task HandlePlayerActionsAsync(Player currentPlayer)
    {
        int finish = 0;
        while (finish == 0)
        {
            var choice = await _uiHandler.GetInputAsync(
                "Выберите действие:\n" +
                "1. Закончить ход\n" +
                "2. Обменяться с игроком\n" +
                "3. Построить дом/отель\n" +
                "4. Снести дом/отель\n" +
                "5. Заложить карточку\n" +
                "6. Вернуть из залога\n" +
                "Введите номер действия: ");

            switch (choice)
            {
                case "1":
                    finish = 1;
                    break;
                case "2":
                    await HandleTradeAsync(currentPlayer);
                    break;
                case "3":
                    await HandleBuildHouseAsync(currentPlayer);
                    break;
                case "4":
                    await HandleDemolishHouseAsync(currentPlayer);
                    break;
                case "5":
                    await HandleMortgageAsync(currentPlayer);
                    break;
                case "6":
                    await HandleUnmortgageAsync(currentPlayer);
                    break;
                default:
                    await _uiHandler.ShowMessageAsync("Неверный выбор, попробуйте снова.");
                    break;
            }
        }
    }

    private async Task HandleTradeAsync(Player currentPlayer)
    {
        await _uiHandler.ShowMessageAsync("С каким игроком хочешь обменяться?");
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i] != currentPlayer && !_players[i].IsBankrupt)
                await _uiHandler.ShowMessageAsync($"{i + 1}. {_players[i].Name}");
        }

        var playerChoice = await _uiHandler.GetInputAsync("Введите номер игрока");
        if (int.TryParse(playerChoice, out int playerIndex) && playerIndex > 0 && playerIndex <= _players.Count)
        {
            Player otherPlayer = _players[playerIndex - 1];

            // Выбор свойств текущего игрока для обмена
            await _uiHandler.ShowMessageAsync("Выбери свои свойства для обмена (ID через запятую):");
            foreach (var prop in currentPlayer.Properties)
                await _uiHandler.ShowMessageAsync($"{prop.Id}. {prop.Name}");

            var currentPlayerPropsInput = await _uiHandler.GetInputAsync("");
            var currentPlayerProps = ParsePropertyInput(currentPlayerPropsInput, currentPlayer);

            // Выбор свойств другого игрока для обмена
            await _uiHandler.ShowMessageAsync("Выбери свойства другого игрока для обмена (ID через запятую):");
            foreach (var prop in otherPlayer.Properties)
                await _uiHandler.ShowMessageAsync($"{prop.Id}. {prop.Name}");

            var otherPlayerPropsInput = await _uiHandler.GetInputAsync("");
            var otherPlayerProps = ParsePropertyInput(otherPlayerPropsInput, otherPlayer);

            // Денежная часть обмена (необязательно)
            var moneyStr = await _uiHandler.GetInputAsync("Сколько денег предлагаешь? (0 если нет)");
            decimal money = decimal.TryParse(moneyStr, out var moneyVal) ? moneyVal : 0;

            try
            {
                _playerService.TradeWithPlayer(currentPlayer, otherPlayer, currentPlayerProps, otherPlayerProps, money);
            }
            catch (Exception ex)
            {
                await _uiHandler.ShowMessageAsync($"Ошибка: {ex.Message}");
            }
        }
    }

    private async Task HandleBuildHouseAsync(Player currentPlayer)
    {
        await _uiHandler.ShowMessageAsync("Выбери свойство для постройки дома (ID):");
        var buildableProperties = currentPlayer.Properties.Where(p => p.Type == PropertyType.street).ToList();

        foreach (var prop in buildableProperties)
            await _uiHandler.ShowMessageAsync($"{prop.Id}. {prop.Name} (Домов: {prop.KolHouse})");

        var input = await _uiHandler.GetInputAsync("");
        if (int.TryParse(input, out int buildPropId))
        {
            try
            {
                _propertyService.BuildHouse(currentPlayer, buildPropId);
            }
            catch (Exception ex)
            {
                await _uiHandler.ShowMessageAsync($"Ошибка: {ex.Message}");
            }
        }
    }


    private async Task HandleDemolishHouseAsync(Player currentPlayer)
    {
        await _uiHandler.ShowMessageAsync("Выбери свойство для сноса домов/отеля (ID):");
        var demolishableProperties = currentPlayer.Properties
            .Where(p => p.Type == PropertyType.street && p.KolHouse > 0)
            .ToList();

        foreach (var prop in demolishableProperties)
        {
            string buildingType = prop.KolHouse == 5 ? "Отель" : $"{prop.KolHouse} дом(ов)";
            await _uiHandler.ShowMessageAsync($"{prop.Id}. {prop.Name} ({buildingType})");
        }

        var input = await _uiHandler.GetInputAsync("");
        if (int.TryParse(input, out int demolishPropId))
        {
            try
            {
                _propertyService.SellHouse(currentPlayer, demolishPropId);
            }
            catch (Exception ex)
            {
                await _uiHandler.ShowMessageAsync($"Ошибка: {ex.Message}");
            }
        }
    }

    private async Task HandleMortgageAsync(Player currentPlayer)
    {
        await _uiHandler.ShowMessageAsync("Выбери свойство для залога (ID):");
        foreach (var prop in currentPlayer.Properties)
            await _uiHandler.ShowMessageAsync($"{prop.Id}. {prop.Name}");

        var input = await _uiHandler.GetInputAsync("");
        if (int.TryParse(input, out int mortgagePropId))
        {
            try
            {
                _propertyService.MortgageProperty(currentPlayer, mortgagePropId);
            }
            catch (Exception ex)
            {
                await _uiHandler.ShowMessageAsync($"Ошибка: {ex.Message}");
            }
        }
    }

    private async Task HandleUnmortgageAsync(Player currentPlayer)
    {
        await _uiHandler.ShowMessageAsync("Выбери заложенную карточку для выкупа (ID):");
        var mortgagedProperties = currentPlayer.Properties.Where(p => p.Status == PropertyStatus.pledged).ToList();

        foreach (var prop in mortgagedProperties)
            await _uiHandler.ShowMessageAsync($"{prop.Id}. {prop.Name}");

        foreach (var prop in mortgagedProperties)
        {
            decimal cost = Math.Round((prop.Price / 2) * 1.1m, MidpointRounding.AwayFromZero);
            await _uiHandler.ShowMessageAsync($"{prop.Id}. {prop.Name} (Стоимость выкупа: {cost}$");
        }

        var input = await _uiHandler.GetInputAsync("");
        if (int.TryParse(input, out int unmortgagePropId))
        {
            try
            {
                _propertyService.UnmortgageProperty(currentPlayer, unmortgagePropId);
            }
            catch (Exception ex)
            {
                await _uiHandler.ShowMessageAsync($"Ошибка: {ex.Message}");
            }
        }
    }

}
