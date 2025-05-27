using Domain.Models;

namespace Monopoly.BusinessLogic.Services.Interfaces;

public interface IGameService
{

    Player GetCurrentPlayer();

    Task MoveToNextPlayer();
    
}