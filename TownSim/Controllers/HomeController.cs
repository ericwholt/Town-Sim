using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TownSim.Models;
using TownSim.ViewModels;

namespace TownSim.Controllers
{
    public class HomeController : Controller
    {
        TownSimDbEntities db = new TownSimDbEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult GameView()
        {
            GameViewModel gameViewModel = new GameViewModel();

            //List<Building> buildings = db.Buildings.ToList();
            //gameViewModel.Buildings = db.Buildings.ToList().First();

            //List<Resource> resources = db.Resources.ToList();
            //gameViewModel.Resources = db.Resources.ToList().First();

            //List<Villager> villagers = db.Villagers.ToList();
            //gameViewModel.Villagers = db.Villagers.ToList().First();
            gameViewModel.gameDatas = db.GameDatas.ToList().First();
            return View(gameViewModel);
        }
    }
}