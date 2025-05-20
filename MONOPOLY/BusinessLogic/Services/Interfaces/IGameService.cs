using Domain.Models;

namespace Monopoly.BusinessLogic.Services.Interfaces;

public interface IGameService
{
    // Начинает новую игру с указанными игроками
     Task Start();

    // Возвращает текущего активного игрока
    Player GetCurrentPlayer();
    // записывает игроков
    Task InitializePlayersAsync();
    //// Передает ход следующему игроку
    void MoveToNextPlayer();
    
}