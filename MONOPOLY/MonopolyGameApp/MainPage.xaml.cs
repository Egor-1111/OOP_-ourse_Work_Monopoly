using BusinessLogic.Services.Interfaces;
using Monopoly.BusinessLogic.Services.Interfaces;

namespace MonopolyGameApp
{
    public partial class MainPage : ContentPage , IGameUIHandler
    {
        private readonly GameService _gameService;

        public MainPage()
        {
            InitializeComponent();
            // ❗ Получаем зарегистрированные сервисы вручную
            var playerService = App.Services.GetService<IPlayerService>();
            var propertyService = App.Services.GetService<IPropertyService>();
            var propertyRepository = App.Services.GetService<PropertyRepository>();

            _gameService = new GameService(playerService, propertyService, propertyRepository, this);

            _gameService.Start();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await DisplayAlert("DEBUG", "OnAppearing Called", "OK");
            await _gameService.Start();
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

        public Task UpdateLabelAsync(string text)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                MessageLabel.Text = text;
            });
            return Task.CompletedTask;
        }
    }

}
