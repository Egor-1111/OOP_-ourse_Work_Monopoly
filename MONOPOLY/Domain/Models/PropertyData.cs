using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;
using Monopoly.Domain.Enums;

namespace Domain.Models
{
    public class PropertyData
    {
        // Основные параметры
        public int Id { get; set; }
        public string Name { get; set; }
        public PropertyStatus Status { get; set; }
        public PropertyGroup Group { get; set; }
        public PropertyType Type { get; set; }
        public decimal Price { get; set; }
        public int BaseRent { get; set; }
        public decimal HouseCost { get; set; }

        public int KolHouse { get; set; } 

        // Аренда в зависимости от построек
        public int RentWith1House { get; set; }
        public int RentWith2Houses { get; set; }
        public int RentWith3Houses { get; set; }
        public int RentWith4Houses { get; set; }
        public int RentWithHotel { get; set; }

        // Позиция на поле
        public int BoardPosition { get; set; }
        // Владелец (null если нет владельца)
        public Player Owner { get; set; }
    }
}
