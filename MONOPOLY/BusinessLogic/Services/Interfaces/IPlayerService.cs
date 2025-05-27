using BusinessLogic.Services.Logic;
using Domain.Models;

namespace Monopoly.BusinessLogic.Services.Interfaces;

public interface IPlayerService
{   
    // перемещение игрока 
    Task MovePlear(Player player,int step);
    // получение баланса 
    Task GetBalace(Player player);
    // аренда с игрока c улицы
    Task PayRentForStreet(Player otdast, Player polych, int propertyId);
    // аренда с игрока c дороги 
    Task PayRentForDoroga(Player otdast, Player polych, int propertyId);
    // аренда с игрока c коммунального предприятия
    Task PayRentForCommunal(Player otdast, Player polych, int propertyId, Dice dice);
    //отправка в тюрьму
    Task SendToJail(Player player);
    // оплата для выхода из тюрьмы
    Task PayToLeaveJail(Player player);
    // функция для выхода через дубль и через 2 хода с оплатой
    Task AttemptToLeaveJail(Player player, Dice dice);
    // обмен между игроками 
    Task TradeWithPlayer(Player currentPlayer, Player otherPlayer, List<PropertyData> currentPlayerProperties, List<PropertyData> otherPlayerProperties, decimal moneyOffered = 0);

}