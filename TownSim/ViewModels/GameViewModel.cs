using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TownSim.Models;

namespace TownSim.ViewModels
{
    public class GameViewModel
    {
        public Building Buildings { get; set; }
        public Resource Resources { get; set; }
        public Villager Villagers { get; set; }
        public GameData gameDatas { get; set; }
    }
}