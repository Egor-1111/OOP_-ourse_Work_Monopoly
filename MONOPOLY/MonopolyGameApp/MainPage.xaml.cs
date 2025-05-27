using BusinessLogic.Services.Interfaces;
using Monopoly.BusinessLogic.Services.Interfaces;
using Domain.Models;
using Monopoly.Domain.Enums;
using BusinessLogic.Services.Logic;
using Monopoly.BusinessLogic.Services.Logic;
using Monopoly.Persistence.Interfaces;

namespace MonopolyGameApp
{
    public partial class MainPage : ContentPage, IGameUIHandler
    {
        private readonly GameService _gameService;
        private readonly Color[] _playerColors = { Colors.Blue, Colors.Green, Colors.Orange, Colors.Purple };

        public MainPage()
        {
            InitializeComponent();
            var playerService = App.Services.GetService<IPlayerService>();
            var propertyService = App.Services.GetService<IPropertyService>();
            var propertyRepository = App.Services.GetService<PropertyRepository>();
            var persistenceService = App.Services.GetService<IGamePersistenceService>();

            _gameService = new GameService(playerService, propertyService, propertyRepository, persistenceService);
            _gameService.SetUIHandler(this);

        }
        private async void OnStartGameClicked(object sender, EventArgs e)
        {
            var names = PlayerNamesEditor.Text?
        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (names == null || names.Length < 2 || names.Length > 4)
            {
                await DisplayAlert("Ошибка", "Введите от 2 до 4 имён игроков через запятую, например: Анна,Борис", "OK");
                return;
            }
            GameBoard.IsVisible = true;
            PlayerNamesEditor.IsVisible = false;
            StartGameButton.IsVisible = false;
            StatusLabel.IsVisible = true;
            //LabalVvod.IsVisible = false;

            _gameService.ResetPlayers();
            GameBoard.PlayerTokens.Clear();

            for (int i = 0; i < names.Length; i++)
            {
                var player = new Player(names[i]) { Position = 0 };
                _gameService.AddPlayer(player);
                AddPlayerToBoard(player.Name, i);
            }
            GameBoard.Refresh();

            RollDiceButton.IsVisible = true;
            EndTurnButton.IsVisible = false;

            await UpdateLabelAsync($"Первый игрок ходит: {_gameService.GetCurrentPlayer().Name}");
        }

        private async void OnRollDiceClicked(object sender, EventArgs e)
        {
            var currentPlayer = _gameService.GetCurrentPlayer();
            if (currentPlayer.Status == PlayerStatus.InJail)
            {
                if (currentPlayer.Status == PlayerStatus.InJail && currentPlayer.KolInTurma == 2)
                {
                    await _gameService.ForJoil2();
                    await ShowMessageAsync($"{currentPlayer.Name} заплатил 50 и вышел из тюрьмы.");

                }
                else if (currentPlayer.Status == PlayerStatus.InJail)
                {
                    int jailChoice = await ShowChoiceAsync(
                    $"{currentPlayer.Name} в тюрьме. Выберите действие:",
                    "1)Попытаться выкинуть дубль",
                    "2)Заплатить $50 за выход",
                    "3)Использовать карточку 'Освобождение' (если есть)");
                    await _gameService.ForJoil1(jailChoice);
                    if (jailChoice == 0)
                    {
                        await ShowMessageAsync($"Игрок {currentPlayer.Name} бросает кубики {_gameService.GetDice()}");
                    }
                    else if (jailChoice == 1)
                    {
                        await ShowMessageAsync($"Игрок {currentPlayer.Name} заплатил 50 и вышел");

                    }
                    else if (jailChoice == 2)
                    {
                        if (currentPlayer.HasGetOutOfJailFreeCard)
                        {
                            await ShowMessageAsync("Использована карточка 'Освобождение из тюрьмы'");

                        }
                        else
                        {
                            await ShowMessageAsync("У вас нет карточки 'Освобождение из тюрьмы'");
                        }
                    }
                }
            }
            else
            {
                await _gameService.OnRollDice();
                RollDiceButton.IsVisible = false;
                EndTurnButton.IsVisible = true;
                await UpdateLabelAsync($"Походил игрок: {_gameService.GetCurrentPlayer().Name} его баланс {_gameService.GetCurrentPlayer().Balance}");
                ActionButton.IsVisible = true;
                GameBoard.MovePlayer(currentPlayer.Name, currentPlayer.Position);

                //GameBoard.Refresh();

                var property = await _gameService.GetPropertyToBuy();
                if (property != null)
                {
                    StatusLabel.Text = $"Вы можете купить {property.Name} за {property.Price}$";
                    BuyButton.IsVisible = true;
                    SkipBuyButton.IsVisible = true;
                }
                await _gameService.HandlePropertyCellAsync();
                GameBoard.MovePlayer(currentPlayer.Name, currentPlayer.Position);
            }

        }

        private async void OnEndTurnClicked(object sender, EventArgs e)
        {

            var player = _gameService.GetCurrentPlayer();
            EndTurnButton.IsVisible = false;
            RollDiceButton.IsVisible = true;
            BuyButton.IsVisible = false;
            SkipBuyButton.IsVisible = false;
            if (player.IsDouble)
            {
                await ShowMessageAsync("Игрок выкинул дубль - ходит ещё раз");
            }
            else
            {
                await _gameService.MoveToNextPlayer();
            }
        }

        private void OnActionClicked(object sender, EventArgs e)
        {
            ActionPanel.IsVisible = true;
        }

        private void OnActionsDoneClicked(object sender, EventArgs e)
        {
            ActionPanel.IsVisible = false;
        }

        private async void OnTradeClicked(object sender, EventArgs e)
        {
            var currentPlayer = _gameService.GetCurrentPlayer();
            var players = _gameService.GetAllPlayers().Where(p => p != currentPlayer && !p.IsBankrupt).ToList();

            if (!players.Any())
            {
                await ShowMessageAsync("Нет доступных игроков для обмена.");
                return;
            }

            // Выбор игрока
            var playerListMessage = string.Join("\n", players.Select((p, i) => $"{i + 1}. {p.Name}"));
            var choiceInput = await GetInputAsync($"С кем хочешь обменяться?\n{playerListMessage}");
            if (!int.TryParse(choiceInput, out int index) || index < 1 || index > players.Count)
            {
                await ShowMessageAsync("Неверный выбор игрока.");
                return;
            }
            var otherPlayer = players[index - 1];

            // Свойства текущего игрока
            string currentPropsText = currentPlayer.Properties.Any()
                ? string.Join("\n", currentPlayer.Properties.Select(p => $"{p.BoardPosition}: {p.Name}"))
                : "Нет участков";
            var offeredInput = await GetInputAsync($"Твои участки:\n{currentPropsText}\nВведите позиции через запятую:");
            var offeredProps = ParsePropertyInputByBoardPosition(offeredInput, currentPlayer);

            // Свойства другого игрока
            string otherPropsText = otherPlayer.Properties.Any()
                ? string.Join("\n", otherPlayer.Properties.Select(p => $"{p.BoardPosition}: {p.Name}"))
                : "Нет участков";
            var requestedInput = await GetInputAsync($"Участки игрока {otherPlayer.Name}:\n{otherPropsText}\nВведите позиции через запятую:");
            var requestedProps = ParsePropertyInputByBoardPosition(requestedInput, otherPlayer);

            // Деньги   
            var moneyStr = await GetInputAsync("Сколько денег предлагаешь? (0 — если ничего)");
            decimal.TryParse(moneyStr, out decimal moneyOffer);

            // Подтверждение от второго игрока
            var confirm = await GetInputAsync($"Игрок {otherPlayer.Name}, согласен на обмен? (y/n)");
            if (confirm?.Trim().ToLower() != "y")
            {
                await ShowMessageAsync("Обмен отменён.");
                return;
            }

            try
            {
                await _gameService.HandleTradeAsync(currentPlayer, otherPlayer, offeredProps, requestedProps, moneyOffer);
                foreach (var prop in offeredProps)
                {
                    GameBoard.MarkCellOwned(prop.BoardPosition, GameBoard.GetPlayerColor(otherPlayer.Name));
                }
                foreach (var prop in requestedProps)
                {
                    GameBoard.MarkCellOwned(prop.BoardPosition, GameBoard.GetPlayerColor(currentPlayer.Name));
                }
                GameBoard.Refresh();

                await ShowMessageAsync("Обмен успешно завершён!");
            }
            catch (Exception ex)
            {
                await ShowMessageAsync($"Ошибка: {ex.Message}");
            }
        }



        private List<PropertyData> ParsePropertyInputByBoardPosition(string input, Player owner)
        {
            var result = new List<PropertyData>();

            var positions = input.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var posStr in positions)
            {
                if (int.TryParse(posStr.Trim(), out int pos))
                {
                    var prop = owner.Properties.FirstOrDefault(p => p.BoardPosition == pos);
                    if (prop != null)
                        result.Add(prop);
                }
            }

            return result;
        }


        private async void OnBuyClicked(object sender, EventArgs e)
        {
            var balance = _gameService.GetCurrentPlayer().Balance;
            var Prace = _gameService.GetPrece();
            if (balance < Prace)
            {
                await ShowMessageAsync("У вас не хватет денег");
            }
            else
            {
                await _gameService.BuyPropertyIfAvailableAsync();
                var currentPlayer = _gameService.GetCurrentPlayer();
                GameBoard.MarkCellOwned(currentPlayer.Position, GameBoard.GetPlayerColor(currentPlayer.Name));
                GameBoard.Refresh();
                StatusLabel.Text = "Покупка завершена.";
                BuyButton.IsVisible = false;
                SkipBuyButton.IsVisible = false;
                EndTurnButton.IsVisible = true;
            }
        }

        private void OnSkipBuyClicked(object sender, EventArgs e)
        {
            StatusLabel.Text = "Вы отказались от покупки.";
            BuyButton.IsVisible = false;
            SkipBuyButton.IsVisible = false;
            EndTurnButton.IsVisible = true;
        }

        private async void OnBuildClicked(object sender, EventArgs e)
        {
            var player = _gameService.GetCurrentPlayer();
            var buildableProps = _gameService.GetBuildableProperties(player);

            if (!buildableProps.Any())
            {
                await ShowMessageAsync("У вас нет полных комплектов улиц для постройки.");
                return;
            }

            string list = "Вы можете построить дома на следующих участках:\n\n";
            foreach (var prop in buildableProps)
                list += $"Позиция: {prop.BoardPosition} | {prop.Name} (Домов: {prop.KolHouse})\n";

            await ShowMessageAsync(list);

            var input = await GetInputAsync("Введите позицию участка (BoardPosition):");

            if (!int.TryParse(input, out int boardPos))
            {
                await ShowMessageAsync("Некорректный ввод.");
                return;
            }

            var targetProperty = buildableProps.FirstOrDefault(p => p.BoardPosition == boardPos);
            if (targetProperty == null)
            {
                await ShowMessageAsync("Указанная позиция недоступна для строительства.");
                return;
            }

            try
            {
                await _gameService.BuildHouseAsync(player, targetProperty.Id);
                await ShowMessageAsync("Строительство завершено.");
            }
            catch (Exception ex)
            {
                await ShowMessageAsync($"Ошибка: {ex.Message}");
            }
        }

        private async void OnDemolishClicked(object sender, EventArgs e)
        {
            var player = _gameService.GetCurrentPlayer();
            var props = _gameService.GetDemolishableProperties(player);

            if (!props.Any())
            {
                await ShowMessageAsync("Нет построек для сноса.");
                return;
            }

            foreach (var prop in props)
            {
                string type = prop.KolHouse == 5 ? "Отель" : $"{prop.KolHouse} дом(ов)";
                await ShowMessageAsync($"{prop.Id}. {prop.Name} ({type})");
            }

            var input = await GetInputAsync("Введите ID:");
            if (int.TryParse(input, out int id))
            {
                try
                {
                    await _gameService.SellHouseAsync(player, id);
                }
                catch (Exception ex)
                {
                    await ShowMessageAsync($"Ошибка: {ex.Message}");
                }
            }
        }

        private async void OnMortgageClicked(object sender, EventArgs e)
        {
            var player = _gameService.GetCurrentPlayer();
            var props = _gameService.GetMortgageableProperties(player);

            if (!props.Any())
            {
                await ShowMessageAsync("Нет доступных участков для залога.");
                return;
            }

            string list = "Доступно для залога:\n";
            foreach (var p in props)
                list += $"{p.Id}. {p.Name}\n";

            await ShowMessageAsync(list);

            var input = await GetInputAsync("Введите ID:");
            if (int.TryParse(input, out int id))
            {
                try
                {
                    await _gameService.MortgagePropertyAsync(player, id);
                    await ShowMessageAsync("Участок успешно заложен.");
                }
                catch (Exception ex)
                {
                    await ShowMessageAsync($"Ошибка: {ex.Message}");
                }
            }
        }


        private async void OnUnmortgageClicked(object sender, EventArgs e)
        {
            var player = _gameService.GetCurrentPlayer();
            var props = _gameService.GetUnmortgageableProperties(player);

            if (!props.Any())
            {
                await ShowMessageAsync("Нет заложенных участков.");
                return;
            }

            string list = "Заложенные участки:\n";
            foreach (var p in props)
            {
                var cost = _gameService.CalculateUnmortgageCost(p);
                list += $"{p.Id}. {p.Name} (Выкуп: {cost}$)\n";
            }

            await ShowMessageAsync(list);

            var input = await GetInputAsync("Введите ID:");
            if (int.TryParse(input, out int id))
            {
                try
                {
                    await _gameService.UnmortgagePropertyAsync(player, id);
                    await ShowMessageAsync("Участок успешно выкуплен.");
                }
                catch (Exception ex)
                {
                    await ShowMessageAsync($"Ошибка: {ex.Message}");
                }
            }
        }


        private async void OnSaveGameClicked(object sender, EventArgs e)
        {
            await _gameService.SaveGameAsync("savegame.json");
        }

        private async void OnLoadGameClicked(object sender, EventArgs e)
        {
            await _gameService.LoadGameAsync("savegame.json");

            // Обновляем UI
            var players = _gameService.GetAllPlayers();
            foreach (var player in players)
            {
                GameBoard.MovePlayer(player.Name, player.Position);
                foreach (var property in player.Properties)
                {
                    GameBoard.MarkCellOwned(property.BoardPosition, GameBoard.GetPlayerColor(player.Name));
                }
            }

            GameBoard.IsVisible = true;
            PlayerNamesEditor.IsVisible = false;
            StartGameButton.IsVisible = false;
            StatusLabel.IsVisible = true;
            RollDiceButton.IsVisible = true;
            EndTurnButton.IsVisible = false;
        }
        





















        public async Task ShowMessageAsync(string message)
        {
            await DisplayAlert("Сообщение", message, "OK");
        }

        public async Task<string> GetInputAsync(string prompt)
        {
            return await DisplayPromptAsync("Ввод", prompt);
        }

        public async Task<int> ShowChoiceAsync(string title, params string[] options)
        {
            string choice = await DisplayActionSheet(title, "Отмена", null, options);
            return Array.IndexOf(options, choice);
        }

        public async Task UpdateLabelAsync(string text)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                StatusLabel.Text = text;
            });
            await Task.CompletedTask;

        }

        public void AddPlayerToBoard(string name, int index)
        {
            Color color = _playerColors[index];
            GameBoard.PlayerTokens.Add((name, 0, color));
        }

        public async Task UpdatePlayerPosition(string name, int position)
        {
            GameBoard.MovePlayer(name, position);
        } 
    }

}
