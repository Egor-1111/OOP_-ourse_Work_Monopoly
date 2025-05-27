using Domain.Enums;
using Domain.Models;


public class PropertyRepository
{
    private readonly Dictionary<int, PropertyData> _properties;

    public PropertyRepository()
    {
        _properties = new Dictionary<int, PropertyData>
        {
            // Коричневые (Brown) - 2 свойства
            {
                1, new PropertyData
                {
                    Id = 1,
                    Name = "ШКОЛА ГАРРИ ХЕРПСОНА",
                    Status = Monopoly.Domain.Enums.PropertyStatus.Onsale,
                    Group = PropertyGroup.Brown,
                    Type = PropertyType.street,
                    Price = 60,
                    BaseRent = 2,
                    KolHouse =0,
                    RentWith1House = 10,
                    RentWith2Houses = 30,
                    RentWith3Houses = 90,
                    RentWith4Houses = 160,
                    RentWithHotel = 250,
                    HouseCost = 50,
                    BoardPosition = 1
                }
            },
            {
                3, new PropertyData
                {
                    Id = 3,
                    Name = "МАГАЗИН НУЖНЫЕ ВЕЩИ",
                    Status = Monopoly.Domain.Enums.PropertyStatus.Onsale,
                    Group = PropertyGroup.Brown,
                    Type = PropertyType.street,
                    Price = 60,
                    BaseRent = 4,
                    KolHouse =0,
                    RentWith1House = 20,
                    RentWith2Houses = 60,
                    RentWith3Houses = 180,
                    RentWith4Houses = 320,
                    RentWithHotel = 450,
                    HouseCost = 50,
                    BoardPosition = 3
                }
            },

            // Светло-синие (Light Blue) - 3 свойства
            {
                6, new PropertyData
                {
                    Id = 6,
                    Name = "АНАТОМИЧЕСКИЙ ПАРК",
                    Status = Monopoly.Domain.Enums.PropertyStatus.Onsale,
                    Group = PropertyGroup.LightBlue,
                    Type = PropertyType.street,
                    Price = 100,
                    BaseRent = 6,
                    KolHouse =0,
                    RentWith1House = 30,
                    RentWith2Houses = 90,
                    RentWith3Houses = 270,
                    RentWith4Houses = 400,
                    RentWithHotel = 550,
                    HouseCost = 50,
                    BoardPosition = 6
                }
            },
            {
                8, new PropertyData
                {
                    Id = 8,
                    Name = "ЗЕМЛЯ С-137",
                    Status = Monopoly.Domain.Enums.PropertyStatus.Onsale,
                    Group = PropertyGroup.LightBlue,
                    Type = PropertyType.street,
                    Price = 100,
                    BaseRent = 6,
                    KolHouse =0,
                    RentWith1House = 30,
                    RentWith2Houses = 90,
                    RentWith3Houses = 270,
                    RentWith4Houses = 400,
                    RentWithHotel = 550,
                    HouseCost = 50,
                    BoardPosition = 8
                }
            },
            {
                9, new PropertyData
                {
                    Id = 9,
                    Name = "ГАЗОРПАЗОРП",
                    Status = Monopoly.Domain.Enums.PropertyStatus.Onsale,
                    Group = PropertyGroup.LightBlue,
                    Type = PropertyType.street,
                    Price = 120,
                    BaseRent = 8,
                    KolHouse =0,
                    RentWith1House = 40,
                    RentWith2Houses = 100,
                    RentWith3Houses = 300,
                    RentWith4Houses = 450,
                    RentWithHotel = 600,
                    HouseCost = 50,
                    BoardPosition = 9
                }
            },

            // Розовые (Pink) - 3 свойства
            {
                11, new PropertyData
                {
                    Id = 11,
                    Name = "БЛИПС ЭНД ЧИТЦ",
                    Status = Monopoly.Domain.Enums.PropertyStatus.Onsale,
                    Group = PropertyGroup.Pink,
                    Type = PropertyType.street,
                    Price = 140,
                    BaseRent = 10,
                    KolHouse =0,
                    RentWith1House = 50,
                    RentWith2Houses = 150,
                    RentWith3Houses = 450,
                    RentWith4Houses = 625,
                    RentWithHotel = 750,
                    HouseCost = 100,
                    BoardPosition = 11
                }
            },
            {
                13, new PropertyData
                {
                    Id = 13,
                    Name = "ПЛАНЕТА МИР ШЕСТЕРЁНОК",
                    Status = Monopoly.Domain.Enums.PropertyStatus.Onsale,
                    Group = PropertyGroup.Pink,
                    Type = PropertyType.street,
                    Price = 140,
                    BaseRent = 10,
                    KolHouse =0,
                    RentWith1House = 50,
                    RentWith2Houses = 150,
                    RentWith3Houses = 450,
                    RentWith4Houses = 625,
                    RentWithHotel = 750,
                    HouseCost = 100,
                    BoardPosition = 13
                }
            },
            {
                14, new PropertyData
                {
                    Id = 14,
                    Name = "ДЖЕРИ-СЛИЗНЯК",
                    Status = Monopoly.Domain.Enums.PropertyStatus.Onsale,
                    Group = PropertyGroup.Pink,
                    Type = PropertyType.street,
                    Price = 160,
                    BaseRent = 12,
                    KolHouse =0,
                    RentWith1House = 60,
                    RentWith2Houses = 180,
                    RentWith3Houses = 500,
                    RentWith4Houses = 700,
                    RentWithHotel = 900,
                    HouseCost = 100,
                    BoardPosition = 14
                }
            },

            // Оранжевые (Orange) - 3 свойства
            {
                16, new PropertyData
                {
                    Id = 16,
                    Name = "МИР ФАНТАЗИЙ",
                    Status = Monopoly.Domain.Enums.PropertyStatus.Onsale,
                    Group = PropertyGroup.Orange,
                    Type = PropertyType.street,
                    Price = 180,
                    BaseRent = 14,
                    KolHouse =0,
                    RentWith1House = 70,
                    RentWith2Houses = 200,
                    RentWith3Houses = 550,
                    RentWith4Houses = 750,
                    RentWithHotel = 950,
                    HouseCost = 100,
                    BoardPosition = 16
                }
            },
            {
                18, new PropertyData
                {
                    Id = 18,
                    Name = "ПЛУТОН",
                    Status = Monopoly.Domain.Enums.PropertyStatus.Onsale,
                    Group = PropertyGroup.Orange,
                    Type = PropertyType.street,
                    Price = 180,
                    BaseRent = 14,
                    KolHouse =0,
                    RentWith1House = 70,
                    RentWith2Houses = 200,
                    RentWith3Houses = 550,
                    RentWith4Houses = 750,
                    RentWithHotel = 950,
                    HouseCost = 100,
                    BoardPosition = 18
                }
            },
            {
                19, new PropertyData
                {
                    Id = 19,
                    Name = "ПЛАНЕТА СУДНОЙ НОЧИ",
                    Status = Monopoly.Domain.Enums.PropertyStatus.Onsale,
                    Group = PropertyGroup.Orange,
                    Type = PropertyType.street,
                    Price = 200,
                    BaseRent = 16,
                    KolHouse =0,
                    RentWith1House = 80,
                    RentWith2Houses = 220,
                    RentWith3Houses = 600,
                    RentWith4Houses = 800,
                    RentWithHotel = 1000,
                    HouseCost = 100,
                    BoardPosition = 19
                }
            },

            // Красные (Red) - 3 свойства
            {
                21, new PropertyData
                {
                    Id = 21,
                    Name = "НУПТИИ 4",
                    Status = Monopoly.Domain.Enums.PropertyStatus.Onsale,
                    Group = PropertyGroup.Red,
                    Type = PropertyType.street,
                    Price = 220,
                    BaseRent = 18,
                    KolHouse =0,
                    RentWith1House = 90,
                    RentWith2Houses = 250,
                    RentWith3Houses = 700,
                    RentWith4Houses = 875,
                    RentWithHotel = 1050,
                    HouseCost = 150,
                    BoardPosition = 21
                }
            },
            {
                23, new PropertyData
                {
                    Id = 23,
                    Name = "ПЛАНЕТА ЛОМБАРДОВ",
                    Status = Monopoly.Domain.Enums.PropertyStatus.Onsale,
                    Group = PropertyGroup.Red,
                    Type = PropertyType.street,
                    Price = 220,
                    BaseRent = 18,
                    KolHouse =0,
                    RentWith1House = 90,
                    RentWith2Houses = 250,
                    RentWith3Houses = 700,
                    RentWith4Houses = 875,
                    RentWithHotel = 1050,
                    HouseCost = 150,
                    BoardPosition = 23
                }
            },
            {
                24, new PropertyData
                {
                    Id = 24,
                    Name = "ПЛАНЕТА ЮНИТИ",
                    Status = Monopoly.Domain.Enums.PropertyStatus.Onsale,
                    Group = PropertyGroup.Red,
                    Type = PropertyType.street,
                    Price = 240,
                    BaseRent = 20,
                    KolHouse =0,
                    RentWith1House = 100,
                    RentWith2Houses = 300,
                    RentWith3Houses = 750,
                    RentWith4Houses = 925,
                    RentWithHotel = 1100,
                    HouseCost = 150,
                    BoardPosition = 24
                }
            },

            // Жёлтые (Yellow) - 3 свойства
            {
                26, new PropertyData
                {
                    Id = 26,
                    Name = "ГОСПИТАЛЬ СВ. ГЛУПИ НУПСА",
                    Status = Monopoly.Domain.Enums.PropertyStatus.Onsale,
                    Group = PropertyGroup.Yellow,
                    Type = PropertyType.street,
                    Price = 260,
                    BaseRent = 22,
                    KolHouse =0,
                    RentWith1House = 110,
                    RentWith2Houses = 330,
                    RentWith3Houses = 800,
                    RentWith4Houses = 975,
                    RentWithHotel = 1150,
                    HouseCost = 150,
                    BoardPosition = 26
                }
            },
            {
                27, new PropertyData
                {
                    Id = 27,
                    Name = "ИЗМЕРЕНИЕ КРОМУЛА",
                    Status = Monopoly.Domain.Enums.PropertyStatus.Onsale,
                    Group = PropertyGroup.Yellow,
                    Type = PropertyType.street,
                    Price = 260,
                    BaseRent = 22,
                    KolHouse =0,
                    RentWith1House = 110,
                    RentWith2Houses = 330,
                    RentWith3Houses = 800,
                    RentWith4Houses = 975,
                    RentWithHotel = 1150,
                    HouseCost = 150,
                    BoardPosition = 27
                }
            },
            {
                29, new PropertyData
                {
                    Id = 29,
                    Name = "ЦИТАДЕЛЬ РИКОВ",
                    Status = Monopoly.Domain.Enums.PropertyStatus.Onsale,
                    Group = PropertyGroup.Yellow,
                    Type = PropertyType.street,
                    Price = 280,
                    BaseRent = 24,
                    KolHouse =0,
                    RentWith1House = 120,
                    RentWith2Houses = 360,
                    RentWith3Houses = 850,
                    RentWith4Houses = 1025,
                    RentWithHotel = 1200,
                    HouseCost = 150,
                    BoardPosition = 29
                }
            },

            // Зелёные (Green) - 3 свойства
            {
                31, new PropertyData
                {
                    Id = 31,
                    Name = "ПЛАНЕТА ЧЕЛОВЕКОПТИЦ",
                    Status = Monopoly.Domain.Enums.PropertyStatus.Onsale,
                    Group = PropertyGroup.Green,
                    Type = PropertyType.street,
                    Price = 300,
                    BaseRent = 26,
                    KolHouse =0,
                    RentWith1House = 130,
                    RentWith2Houses = 390,
                    RentWith3Houses = 900,
                    RentWith4Houses = 1100,
                    RentWithHotel = 1275,
                    HouseCost = 200,
                    BoardPosition = 31
                }
            },
            {
                32, new PropertyData
                {
                    Id = 32,
                    Name = "ПЛАНЕТА СКВОЧИ",
                    Status = Monopoly.Domain.Enums.PropertyStatus.Onsale,
                    Group = PropertyGroup.Green,
                    Type = PropertyType.street,
                    Price = 300,
                    BaseRent = 26,
                    KolHouse =0,
                    RentWith1House = 130,
                    RentWith2Houses = 390,
                    RentWith3Houses = 900,
                    RentWith4Houses = 1100,
                    RentWithHotel = 1275,
                    HouseCost = 200,
                    BoardPosition = 32
                }
            },
            {
                34, new PropertyData
                {
                    Id = 34,
                    Name = "КРОШЕЧНАЯ ПЛАНЕТА 9",
                    Status = Monopoly.Domain.Enums.PropertyStatus.Onsale,
                    Group = PropertyGroup.Green,
                    Type = PropertyType.street,
                    Price = 320,
                    BaseRent = 28,
                    KolHouse =0,
                    RentWith1House = 150,
                    RentWith2Houses = 450,
                    RentWith3Houses = 1000,
                    RentWith4Houses = 1200,
                    RentWithHotel = 1400,
                    HouseCost = 200,
                    BoardPosition = 34
                }
            },

            // Тёмно-синие (Dark Blue) - 2 свойства
            {
                37, new PropertyData
                {
                    Id = 37,
                    Name = "СПАЛЬНЯ МОРТИ",
                    Status = Monopoly.Domain.Enums.PropertyStatus.Onsale,
                    Group = PropertyGroup.DarkBlue,
                    Type = PropertyType.street,
                    Price = 350,
                    BaseRent = 35,
                    KolHouse =0,
                    RentWith1House = 175,
                    RentWith2Houses = 500,
                    RentWith3Houses = 1100,
                    RentWith4Houses = 1300,
                    RentWithHotel = 1500,
                    HouseCost = 200,
                    BoardPosition = 37
                }
            },
            {
                39, new PropertyData
                {
                    Id = 39,
                    Name = "ГАРАЖ РИКА",
                    Status = Monopoly.Domain.Enums.PropertyStatus.Onsale,
                    Group = PropertyGroup.DarkBlue,
                    Type = PropertyType.street,
                    Price = 400,
                    BaseRent = 50,
                    KolHouse =0,
                    RentWith1House = 200,
                    RentWith2Houses = 600,
                    RentWith3Houses = 1400,
                    RentWith4Houses = 1700,
                    RentWithHotel = 2000,
                    HouseCost = 200,
                    BoardPosition = 39
                }
            },

            // Железные дороги (Railroads) - 4 свойства
            {
                5, new PropertyData
                {
                    Id = 5,
                    Name = "ПОЕЗД-ПРИЗРАК",
                    Status = Monopoly.Domain.Enums.PropertyStatus.Onsale,
                    Group = PropertyGroup.doroga,
                    Price = 200,
                    BaseRent = 25,// Аренда: 25/50/100/200 в зависимости от количества ЖД
                    BoardPosition = 5,

                }
            },
            {
                15, new PropertyData
                {
                    Id = 15,
                    Name = "КОРАБЛЬ ЗИГЕРИОН",
                    Status = Monopoly.Domain.Enums.PropertyStatus.Onsale,
                    Group = PropertyGroup.doroga,
                    Price = 200,
                    BaseRent = 25,
                    BoardPosition = 15,
                }
            },
            {
                25, new PropertyData
                {
                    Id = 25,
                    Name = "КОРАБЛЬ S.S. ИНДЕПЕНДЕНС",
                    Status = Monopoly.Domain.Enums.PropertyStatus.Onsale,
                    Group = PropertyGroup.doroga,
                    Price = 200,
                    BaseRent = 25,
                    BoardPosition = 25,
                }
            },
            {
                35, new PropertyData
                {
                    Id = 35,
                    Name = "КОРАБЛЬ БЕТА 7",
                    Status = Monopoly.Domain.Enums.PropertyStatus.Onsale,
                    Group = PropertyGroup.doroga,
                    Price = 200,
                    BaseRent = 25,
                    BoardPosition = 35,
                }
            },

            // Коммунальные предприятия (Utilities) - 2 свойства
            {
                12, new PropertyData
                {
                    Id = 12,
                    Name = "МИКРОВСЕЛЕННАЯ",
                    Status = Monopoly.Domain.Enums.PropertyStatus.Onsale,
                    Group = PropertyGroup.communal,
                    Price = 150,
                    BoardPosition = 12,
                }
            },
            {
                28, new PropertyData
                {
                    Id = 28,
                    Name = "МИНИВСЕЛЕННАЯ",
                    Status = Monopoly.Domain.Enums.PropertyStatus.Onsale,
                    Group = PropertyGroup.communal,
                    Price = 150,
                    BoardPosition = 28,
                }
            },
            {
                0, new PropertyData
                {
                    Id = 0,
                    Name = "ВПЕРЕД",
                    Group = PropertyGroup.start,
                    BaseRent = 200,
                    BoardPosition = 0,
                }
            },
            {
                2, new PropertyData
                {
                    Id = 2,
                    Group = PropertyGroup.kazna,
                    Name = "Казна",
                    BoardPosition = 2,
                }
            },
            {
                7, new PropertyData
                {
                    Id = 7,
                    Group = PropertyGroup.shans,
                    Name = "Шанс",
                    BoardPosition = 7,
                }
            },
            {
                10, new PropertyData
                {
                    Id = 10,
                    Group = PropertyGroup.prostou,
                    Name = "Сидим в тюрьме или проходим мимо ее",
                    BoardPosition = 10,
                }
            },
            {
                17, new PropertyData
                {
                    Id = 17,
                    Group = PropertyGroup.kazna,
                    Name = "Казна",
                    BoardPosition = 17,
                }
            },
            {
                22, new PropertyData
                {
                    Id = 22,
                    Group = PropertyGroup.shans,
                    Name = "Шанс",
                    BoardPosition = 22,
                }
            },
            {
                33, new PropertyData
                {
                    Id = 33,
                    Group = PropertyGroup.kazna,
                    Name = "Казна",
                    BoardPosition = 33,
                }
            },
            {
                36, new PropertyData
                {
                    Id = 36,
                    Group = PropertyGroup.shans,
                    Name = "Шанс",
                    BoardPosition = 36,
                }
            },
            {
                4, new PropertyData
                {
                    Id = 4,
                    Group = PropertyGroup.kazna200,
                    Name = "Сбор денег",
                    BoardPosition = 4,
                }
            },
            {
                38, new PropertyData
                {
                    Id = 38,
                    Group = PropertyGroup.kazna100,
                    Name = "Сбор денег",
                    BoardPosition = 38,
                }
            },
            {
                30, new PropertyData
                {
                    Id = 30,
                    Group = PropertyGroup.turma,
                    Name = "Тюрьма",
                    BoardPosition = 30,
                }
            },
            {
                20, new PropertyData
                {
                    Id = 20,
                    Group = PropertyGroup.prostou,
                    Name = "Простой",
                    BoardPosition = 20,
                }
            },
        }; 
    }

    public PropertyData GetById(int id) => _properties[id];
    public List<PropertyData> GetByGroup(PropertyGroup group)
        => _properties.Values.Where(p => p.Group == group).ToList();
    
    public List<PropertyData> GetAll() => _properties.Values.ToList();
}