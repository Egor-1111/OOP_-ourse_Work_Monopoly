using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Monopoly.Persistence.Models
{
    public class GameState
    {
        public List<Player> Players { get; set; } = new();
        public int CurrentPlayerIndex { get; set; }
    }
}
