using BusinessLogic.Services.Logic;
using Domain.Models;

namespace Monopoly.BusinessLogic.Services.Interfaces;

public interface IPlayerService
{   
    // перемещение игрока 
    public void MovePlear(Player player,int step);
    // получение баланса 
    public void GetBalace(Player player);
    // аренда с игрока c улицы
    public void PayRentForStreet(Player otdast, Player polych, int propertyId);
    // аренда с игрока c дороги 
    public void PayRentForDoroga(Player otdast, Player polych, int propertyId);
    // аренда с игрока c коммунального предприятия
    public void PayRentForCommunal(Player otdast, Player polych, int propertyId, Dice dice);
    //отправка в тюрьму
    public void SendToJail(Player player);
    // оплата для выхода из тюрьмы
    public void PayToLeaveJail(Player player);
    // функция для выхода через дубль и через 2 хода с оплатой
    public void AttemptToLeaveJail(Player player, Dice dice);
    // обмен между игроками 
    public void TradeWithPlayer(Player currentPlayer, Player otherPlayer, List<PropertyData> currentPlayerProperties, List<PropertyData> otherPlayerProperties, decimal moneyOffered = 0);

}