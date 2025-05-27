using Monopoly.Persistence.Models;

namespace Monopoly.Persistence.Interfaces;

public interface IGamePersistenceService
{
    Task SaveGameAsync(GameState gameState, string filePath);
    Task<GameState> LoadGameAsync(string filePath);
}