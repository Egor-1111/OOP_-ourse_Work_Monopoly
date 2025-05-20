using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Services.Interfaces;
using Domain.Models;
using Monopoly.BusinessLogic.Services.Logic;

namespace BusinessLogic.Services.Logic
{
   
        public class Dice : IDice
        {
            private static readonly Random _random = new Random();

            // Значение первого кубика (1-6)
            public int Value1 { get; private set; }

            // Значение второго кубика (1-6)
            public int Value2 { get; private set; }

            // Сумма значений кубиков
            public int Total => Value1 + Value2;

            // Флаг выпадения дубля (одинаковые значения)
            public bool IsDouble => Value1 == Value2;

            // Количество последовательных дублей
            public int DoubleCount { get; set; }

            // Бросок кубиков
            public void Roll()
            {
                Value1 = _random.Next(1, 7);
                Value2 = _random.Next(1, 7);


               if (IsDouble)
               { 
                    DoubleCount++; 
                    return;
                
               }
               else
                DoubleCount = 0;
            }

            // Сброс счетчика дублей
            public void ResetDoubles()
            {
                DoubleCount = 0;
            }
        }
    
}
