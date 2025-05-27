using Monopoly.Domain.Enums;
namespace Domain.Models
{
    public class Player
    {
        public string Name { get; set; }
        public decimal Balance { get; set; } = 1500;
        public int Position { get; set; } = 0;
        // купленые карточки улич 
        public List<PropertyData> Properties { get; set; } = new List<PropertyData>();
        public PlayerStatus Status { get; set; } = PlayerStatus.Active;
        public bool IsBankrupt { get; set; } = false;

        public int KolDorog { get; set; } = 0;

        public int KolComunal { get; set; } = 0;

        public int KolInTurma { get; set; } = 0;

        public int DoubleThenTurma { get; set; } = 0;

        public bool IsDouble { get; set; } = false;

        // В класс Player добавить:
        public bool HasGetOutOfJailFreeCard { get; set; } = false;

        public Player() { }
        public Player(string name)
        {
            Name = name;
        }

    }
}
