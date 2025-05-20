// Services/CardService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Enums;
using Domain.Models;

namespace BusinessLogic.Services.Logic
{
    public class CardService
    {
        private readonly LinkedList<ChanceCard> _chanceCards;
        private readonly LinkedList<CommunityChestCard> _communityChestCards;
        private readonly Random _random;

        public CardService()
        {
            _random = new Random();
            _chanceCards = new LinkedList<ChanceCard>(InitializeChanceCards().OrderBy(x => _random.Next()));
            _communityChestCards = new LinkedList<CommunityChestCard>(InitializeCommunityChestCards().OrderBy(x => _random.Next()));
        }

        public Card DrawChanceCard()
        {
            if (_chanceCards.Count == 0)
            {
                ReinitializeChanceCards();
            }

            // Берем карту сверху
            var card = _chanceCards.First.Value;
            _chanceCards.RemoveFirst();

            // Кладем ее в конец
            _chanceCards.AddLast(card);

            return card;
        }

        public Card DrawCommunityChestCard()
        {
            if (_communityChestCards.Count == 0)
            {
                ReinitializeCommunityChestCards();
            }

            // Берем карту сверху
            var card = _communityChestCards.First.Value;
            _communityChestCards.RemoveFirst();

            // Кладем ее в конец
            _communityChestCards.AddLast(card);

            return card;
        }

        public void ReturnCard(Card card)
        {
            if (card is ChanceCard chanceCard)
            {
                // Возвращаем карту в конец колоды
                _chanceCards.AddLast(chanceCard);
            }
            else if (card is CommunityChestCard communityChestCard)
            {
                // Возвращаем карту в конец колоды
                _communityChestCards.AddLast(communityChestCard);
            }
        }

        private void ReinitializeChanceCards()
        {
            foreach (var card in InitializeChanceCards().OrderBy(x => _random.Next()))
            {
                _chanceCards.AddLast(card);
            }
        }

        private void ReinitializeCommunityChestCards()
        {
            foreach (var card in InitializeCommunityChestCards().OrderBy(x => _random.Next()))
            {
                _communityChestCards.AddLast(card);
            }
        }

        private List<ChanceCard> InitializeChanceCards()
        {
            return new List<ChanceCard>
            {
                new ChanceCard { Id = 1, Text = "Отправляйтесь в тюрьму. Не проходите через поле «Вперед», не получайте 200$", GoToJail = true },
                new ChanceCard { Id = 2, Text = "Отправляйтесь на поле «Вперед»", MoveToPosition = 0 },
                new ChanceCard { Id = 3, Text = "Отправляйтесь на поле «Улица 1»", MoveToPosition = 1 },
                new ChanceCard { Id = 4, Text = "Получите 50$", MoneyChange = 50 },
                new ChanceCard { Id = 5, Text = "Вы получаете карту освобождения из тюрьмы", GetOutOfJailFree = true },
                // Добавьте другие карточки шанса
            };
        }

        private List<CommunityChestCard> InitializeCommunityChestCards()
        {
            return new List<CommunityChestCard>
            {
                new CommunityChestCard { Id = 1, Text = "Оплатите штраф за превышение скорости 15$", MoneyChange = -15 },
                new CommunityChestCard { Id = 2, Text = "Вы получили наследство 100$", MoneyChange = 100 },
                new CommunityChestCard { Id = 3, Text = "Вернитесь на поле «Старт»", MoveToPosition = 0 },
                new CommunityChestCard { Id = 4, Text = "Вы получаете карту освобождения из тюрьмы", GetOutOfJailFree = true },
                // Добавьте другие карточки казны
            };
        }
    }
}