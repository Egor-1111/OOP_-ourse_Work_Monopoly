using System;
using System.Numerics;
using System.Text.Json;
using System.Threading.Tasks;
using BusinessLogic.Services.Interfaces;
using BusinessLogic.Services.Logic;
using Domain.Enums;
using Domain.Models;
using Monopoly.BusinessLogic.Services.Interfaces;
using Monopoly.BusinessLogic.Services.Logic;
using Monopoly.Domain.Enums;
using System.Text.Json.Serialization;
using Monopoly.Persistence.Interfaces;
using Monopoly.Persistence.Models;

public class GameService : IGameService
{
    private readonly List<Player> _players;
    private int _currentPlayerIndex;
    private readonly IPlayerService _playerService;
    private readonly Dice _dice;
    private readonly IPropertyService _propertyService;
    private readonly PropertyRepository _propertyRepository;
    private readonly CardService _cardService;
    private IGameUIHandler? _uiHandler;
    private readonly IGamePersistenceService _persistenceService;
    public GameService(IPlayerService playerService, IPropertyService propertyService, PropertyRepository propertyRepository, IGamePersistenceService persistenceService)
    {
        _playerService = playerService;
        _propertyService = propertyService;
        _propertyRepository = propertyRepository;
        _players = new List<Player>();
        _currentPlayerIndex = 0;
        _dice = new Dice();
        _cardService = new CardService();
        _persistenceService = persistenceService;
    }

    public void SetUIHandler(IGameUIHandler handler)
    {
        _uiHandler = handler;

        // передаём дальше, если умеет
        if (_playerService is PlayerService player)
            player.SetUIHandler(handler);

        if (_propertyService is PropertyService property)
            property.SetUIHandler(handler);
    }



    private List<PropertyData> ParsePropertyInput(string input, Player owner)
    {
        var ids = input.Split(',').Select(int.Parse).ToList();
        return owner.Properties.Where(p => ids.Contains(p.Id)).ToList();
    }

    public void ResetPlayers() => _players.Clear();

    public void AddPlayer(Player player) => _players.Add(player);

    public async Task OnRollDice()
    {

        Player currentPlayer = GetCurrentPlayer();
        _dice.Roll();

        currentPlayer.IsDouble = _dice.IsDouble;

        await _playerService.MovePlear(currentPlayer, _dice.Total);
        currentPlayer.IsDouble = false;

    }

    public async Task ForJoil1(int jail)
    {
        var currentPlayer = GetCurrentPlayer();


        switch (jail)
        {
            case 0:
                await _playerService.AttemptToLeaveJail(currentPlayer, _dice);
                if (!_dice.IsDouble)
                {
                    await MoveToNextPlayer();
                }
                break;
            case 1:
                await _playerService.PayToLeaveJail(currentPlayer);
                break;
            case 2:
                if (currentPlayer.HasGetOutOfJailFreeCard)
                {
                    currentPlayer.HasGetOutOfJailFreeCard = false;
                    currentPlayer.Status = PlayerStatus.Active;
                    currentPlayer.KolInTurma = 0;
                    // Вернуть карточку в колоду
                    _cardService.ReturnCard(new CommunityChestCard { GetOutOfJailFree = true });
                }

                break;

        }
    }

    public string GetDice()
    {
        return $"{_dice.Value1} и {_dice.Value2}";
    }

    public async Task ForJoil2()
    {
        var currentPlayer = GetCurrentPlayer();
        await _playerService.PayToLeaveJail(currentPlayer);
    }
    public Player GetCurrentPlayer()
    {
        // Возвращаем текущего игрока
        return _players[_currentPlayerIndex];
    }

    public async Task MoveToNextPlayer()
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
            await _playerService.SendToJail(player);
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

    public decimal GetPrece()
    {
        var currentPlayer = GetCurrentPlayer();
        int posPlear = currentPlayer.Position;
        var propertyRep = _propertyRepository.GetById(posPlear);
        return propertyRep.Price;
    }
    public async Task HandlePropertyCellAsync()
    {
        var currentPlayer = GetCurrentPlayer();
        int posPlear = currentPlayer.Position;
        var propertyRep = _propertyRepository.GetById(posPlear);

        if (propertyRep.Group == PropertyGroup.kazna)
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
            await _playerService.SendToJail(currentPlayer);
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

            await _playerService.PayRentForStreet(currentPlayer, poluch, currentPlayer.Position);
        }
        else if (propertyRep.Status == PropertyStatus.sold && propertyRep.Type == PropertyType.doroga && currentPlayer != propertyRep.Owner)
        {
            var poluch = propertyRep.Owner;

            await _playerService.PayRentForDoroga(currentPlayer, poluch, currentPlayer.Position);
        }
        else if (propertyRep.Status == PropertyStatus.sold && propertyRep.Type == PropertyType.communal && currentPlayer != propertyRep.Owner)
        {
            var poluch = propertyRep.Owner;

            await _playerService.PayRentForCommunal(currentPlayer, poluch, currentPlayer.Position, _dice);
        }

    }

    public async Task<PropertyData?> GetPropertyToBuy()
    {
        var player = GetCurrentPlayer();
        var prop = _propertyRepository.GetById(player.Position);

        if (prop == null)
            return null;

        if (prop.Status == PropertyStatus.Onsale &&
            prop.Group != PropertyGroup.kazna &&
            prop.Group != PropertyGroup.kazna100 &&
            prop.Group != PropertyGroup.kazna200 &&
            prop.Group != PropertyGroup.prostou &&
            prop.Group != PropertyGroup.turma &&
            prop.Group != PropertyGroup.start &&
            prop.Group != PropertyGroup.shans &&
            prop.Group != PropertyGroup.start)
        {
            return prop;
        }

        return null;
    }

    public async Task BuyPropertyIfAvailableAsync()
    {
        var player = GetCurrentPlayer();
        await _propertyService.BuyProperty(player, player.Position);
    }


   

    public async Task HandleTradeAsync(
     Player currentPlayer,
     Player otherPlayer,
     List<PropertyData> offeredProperties,
     List<PropertyData> requestedProperties,
     decimal money)
    {
        await _playerService.TradeWithPlayer(currentPlayer, otherPlayer, offeredProperties, requestedProperties, money);
    }


    public List<PropertyData> GetBuildableProperties(Player player)
    {
        // Группируем все улицы игрока по цветовой группе
        var grouped = player.Properties
            .Where(p => p.Type == PropertyType.street)
            .GroupBy(p => p.Group);

        var result = new List<PropertyData>();

        foreach (var group in grouped)
        {
            var propertiesInGroup = group.ToList();
            int totalInGroup = GetGroupSize(group.Key); // метод, который вернёт размер группы (2 или 3)

            // Если игрок владеет всей группой
            if (propertiesInGroup.Count == totalInGroup)
            {
                result.AddRange(propertiesInGroup);
            }
        }

        return result;
    }

    // Пример метода для определения количества участков в группе
    private int GetGroupSize(PropertyGroup group)
    {
        return group switch
        {
            PropertyGroup.Brown or PropertyGroup.DarkBlue => 2,
            _ => 3
        };
    }


    public async Task SaveGameAsync(string filePath)
    {
        var state = new GameState
        {
            Players = _players,
            CurrentPlayerIndex = _currentPlayerIndex
        };
        await _persistenceService.SaveGameAsync(state, filePath);
    }

    public async Task LoadGameAsync(string filePath)
    {
        var state = await _persistenceService.LoadGameAsync(filePath);
        _players.Clear();
        _players.AddRange(state.Players);
        _currentPlayerIndex = state.CurrentPlayerIndex;
    }


    public List<Player> GetAllPlayers()
    {
        return _players;
    }

    public List<PropertyData> GetDemolishableProperties(Player player)
    {
        return player.Properties
            .Where(p => p.Type == PropertyType.street && p.KolHouse > 0)
            .ToList();
    }

    public List<PropertyData> GetMortgageableProperties(Player player)
    {
        return player.Properties
            .Where(p => p.Status == PropertyStatus.sold)
            .ToList();
    }

    public List<PropertyData> GetUnmortgageableProperties(Player player)
    {
        return player.Properties
            .Where(p => p.Status == PropertyStatus.pledged)
            .ToList();
    }

    public decimal CalculateUnmortgageCost(PropertyData property)
    {
        return Math.Round((property.Price / 2) * 1.1m, MidpointRounding.AwayFromZero);
    }

    public async Task BuildHouseAsync(Player player, int propertyId)
    {
        await _propertyService.BuildHouse(player, propertyId);
    }

    public Task SellHouseAsync(Player player, int propertyId)
        => _propertyService.SellHouse(player, propertyId);

    public Task MortgagePropertyAsync(Player player, int propertyId)
        => _propertyService.MortgageProperty(player, propertyId);

    public Task UnmortgagePropertyAsync(Player player, int propertyId)
        => _propertyService.UnmortgageProperty(player, propertyId);

    public async Task HandleBuildHouseAsync(Player currentPlayer)
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
                await _propertyService.BuildHouse(currentPlayer, buildPropId);
            }
            catch (Exception ex)
            {
                await _uiHandler.ShowMessageAsync($"Ошибка: {ex.Message}");
            }
        }
    }

    public async Task HandleDemolishHouseAsync(Player currentPlayer)
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
                await _propertyService.SellHouse(currentPlayer, demolishPropId);
            }
            catch (Exception ex)
            {
                await _uiHandler.ShowMessageAsync($"Ошибка: {ex.Message}");
            }
        }
    }

    public async Task HandleMortgageAsync(Player currentPlayer)
    {
        await _uiHandler.ShowMessageAsync("Выбери свойство для залога (ID):");
        foreach (var prop in currentPlayer.Properties)
            await _uiHandler.ShowMessageAsync($"{prop.Id}. {prop.Name}");

        var input = await _uiHandler.GetInputAsync("");
        if (int.TryParse(input, out int mortgagePropId))
        {
            try
            {
                await _propertyService.MortgageProperty(currentPlayer, mortgagePropId);
            }
            catch (Exception ex)
            {
                await _uiHandler.ShowMessageAsync($"Ошибка: {ex.Message}");
            }
        }
    }

    public async Task HandleUnmortgageAsync(Player currentPlayer)
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
                await _propertyService.UnmortgageProperty(currentPlayer, unmortgagePropId);
            }
            catch (Exception ex)
            {
                await _uiHandler.ShowMessageAsync($"Ошибка: {ex.Message}");
            }
        }
    }
    private async Task ShowInitialPlayerStatesAsync()
    {
        await _uiHandler.ShowMessageAsync("Стартовое состояние игроков:");

        foreach (var player in _players)
        {
            await _uiHandler.ShowMessageAsync($"{player.Name} — Баланс: {player.Balance}$, Позиция: {player.Position}");
        }
    }
}
