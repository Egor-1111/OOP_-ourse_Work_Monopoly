using System.Text.Json;
using System.Text.Json.Serialization;
using Monopoly.Persistence.Interfaces;
using Monopoly.Persistence.Models;

namespace Monopoly.Persistence.Services;

public class GamePersistenceService : IGamePersistenceService
{
    public async Task SaveGameAsync(GameState gameState, string filePath)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            IncludeFields = true,
            ReferenceHandler = ReferenceHandler.Preserve
        };

        string json = JsonSerializer.Serialize(gameState, options);
        await File.WriteAllTextAsync(filePath, json);
    }

    public async Task<GameState> LoadGameAsync(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("Файл сохранения не найден.");

        string json = await File.ReadAllTextAsync(filePath);

        var options = new JsonSerializerOptions
        {
            IncludeFields = true,
            ReferenceHandler = ReferenceHandler.Preserve
        };

        return JsonSerializer.Deserialize<GameState>(json, options) 
               ?? throw new InvalidOperationException("Не удалось загрузить состояние игры");
    }
}