using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Domain.Enums;

// Models/Card.cs
namespace Domain.Models
{
    public abstract class Card
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public CardType Type { get; set; }
    }

    public class ChanceCard : Card
    {
        public ChanceCard()
        {
            Type = CardType.Chance;
        }

        public decimal MoneyChange { get; set; }
        public int? MoveToPosition { get; set; }
        public bool GetOutOfJailFree { get; set; }
        public bool GoToJail { get; set; }
    }

    public class CommunityChestCard : Card
    {
        public CommunityChestCard()
        {
            Type = CardType.CommunityChest;
        }

        public decimal MoneyChange { get; set; }
        public int? MoveToPosition { get; set; }
        public bool GetOutOfJailFree { get; set; }
    }
}
