using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using TownSim.Models;
using TownSim.ViewModels;

namespace TownSim.Controllers
{
    public class HomeController : Controller
    {
        TownSimDbEntities db = new TownSimDbEntities();
        Random rnd = new Random();

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

            GameData game = db.GameDatas.ToList().First();
            return View(game);
        }

        public ActionResult Build(string building)
        {
            GameData game = db.GameDatas.ToList().First();
            if (game.Actions >= 1)
            {
                
                if (building == "House" && game.Wood >= 5)
                {
                    game.Actions--;
                    game.Houses++;
                    game.Wood -= 5;
                }
                else if (building == "Well" && game.Stone >= 4 && game.Wood >= 2)
                {
                    game.Actions--;
                    game.Stone -= 4;
                    game.Wood -= 2;
                    game.Wells++;
                }
                else if (building == "Farm" && game.Stone >= 2 && game.Wood >= 4)
                {
                    game.Actions--;
                    game.Stone -= 2;
                    game.Wood -= 4;
                    game.Farm++;
                }
                db.GameDatas.AddOrUpdate(game);
                db.SaveChanges();
            }
            return RedirectToAction("GameView");
        }
        public ActionResult Gather(string resource)
        {
            GameData game = db.GameDatas.ToList().First();
            if (game.Actions >= 1)
            {
                game.Actions--;
                if (resource == "Food")
                {
                    game.Food += rnd.Next(0, 5);
                    //Random 0-4 food gathered
                }
                else if (resource == "Water")
                {
                    game.Water += rnd.Next(1, 6);
                    //Random 1-5 Water gathered
                }
                else if (resource == "Wood")
                {
                    game.Wood += rnd.Next(1, 6);
                    //Random 1-5 Wood Gathered
                }
                else if (resource == "Stone")
                {
                    game.Stone += rnd.Next(1, 3);
                    //Random 1-3 Stone Gathered
                }
                db.GameDatas.AddOrUpdate(game);
                db.SaveChanges();
            }
            return RedirectToAction("GameView");
        }

        public ActionResult Explore()
        {
            GameData game = db.GameDatas.ToList().First();
            if (game.Actions >= 1)
            {
                game.Actions--;
                int roll = rnd.Next(1, 101);
                if (roll > 95)
                {
                    game.Stone += rnd.Next(3, 7);
                }
                else if (roll > 85)
                {
                    game.Food += rnd.Next(5, 10);
                }
                else if (roll > 70)
                {
                    game.Water += rnd.Next(6, 11);
                }
                else if (roll > 55)
                {
                    game.Wood += rnd.Next(6, 11);
                }
                else if (roll > 0)
                {

                }
                db.GameDatas.AddOrUpdate(game);
                db.SaveChanges();
            }
            return RedirectToAction("GameView");
        }

        public ActionResult NewGame()
        {
            GameData game = db.GameDatas.ToList().First();
            game.Actions = 1;
            game.Day = 1;
            game.Farm = 0;
            game.Food = 6;
            game.Houses = 1;
            game.Stone = 0;
            game.Villagers = 1;
            game.Water = 6;
            game.Wells = 0;
            game.Wood = 0;
            db.GameDatas.AddOrUpdate(game);
            db.SaveChanges();
            return RedirectToAction("GameView");
        }
    }
}