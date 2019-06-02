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

        public ActionResult NewDay()
        {
            Session["Message"] = null;
            GameData game = db.GameDatas.ToList().First();
            //check for raiders if day > 5
            if (game.Villagers <= 0)
            {
                return RedirectToAction("GameLose");
                //show lose game view
            }
            else
            {
                
                if (game.Houses > game.Villagers)
                {
                    int diff = game.Houses - game.Villagers;
                    if (diff == 1)
                    {
                        Session["Message"] = $"A new villager moved in!";
                    }
                    else
                    {
                        Session["Message"] = $"{diff} new villager moved in!";
                    }

                }
                game.Villagers = game.Houses;
            }
            if (game.Villagers >= 10)
            {
                return RedirectToAction("GameWin");
            }
            else
            {                
                game.Water += game.Wells;
                game.Food += game.Farm;

                for (int i = 0; i < game.Villagers; i++)
                {
                    if (game.Water >= 2)
                    {
                        game.Water -= 2;
                    }
                    else
                    {
                        game.Villagers--;
                        game.Actions = game.Villagers;
                    }

                    if (game.Food >= 1 && game.Villagers > 0)
                    {
                        game.Food--;
                    }
                    else
                    {
                        game.Villagers--;
                        game.Actions = game.Villagers;
                    }
                }
                //check for loss again so we don't have to wait for new day click
                if (game.Villagers <= 0)
                {
                    return RedirectToAction("GameLose");
                    //show lose game view
                }
                //Handle day transition
                game.Day++;
                game.Actions = game.Villagers;
                db.GameDatas.AddOrUpdate(game);
                db.SaveChanges();
            }

            return RedirectToAction("GameView");
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
                else if (building == "Well" && game.Stone >= 3 && game.Wood >= 2)
                {
                    game.Actions--;
                    game.Stone -= 3;
                    game.Wood -= 2;
                    game.Wells++;
                }
                else if (building == "Farm" && game.Stone >= 2 && game.Wood >= 3)
                {
                    game.Actions--;
                    game.Stone -= 2;
                    game.Wood -= 3;
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
                int amount = rnd.Next(1, 7) - 1;
                if (resource == "Food")
                {
                    int[] foodFound = { 0, 2, 2, 2, 3, 4 };
                    game.Food += foodFound[amount];
                    Session["Message"] = $"Villager gathered {foodFound[amount]} food.";
                    //Random 0-4 food gathered
                }
                else if (resource == "Water")
                {
                    int[] waterFound = { 1, 2, 2, 3, 4, 5 };
                    game.Water += waterFound[amount];
                    Session["Message"] = $"Villager gathered {waterFound[amount]} gallons of water.";
                }
                else if (resource == "Wood")
                {
                    int[] woodFound = { 1, 2, 2, 3, 4, 5 };
                    game.Wood += woodFound[amount];
                    Session["Message"] = $"Villager gathered {woodFound[amount]} wood.";
                }
                else if (resource == "Stone")
                {
                    int[] stoneFound = { 1, 2, 2, 2, 3, 4 };
                    game.Stone += stoneFound[amount];
                    Session["Message"] = $"Villager gathered {stoneFound[amount]} stones.";
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
                int roll = rnd.Next(1, 7);
                if (roll == 1)
                {
                    int amount = rnd.Next(3, 7);
                    game.Stone += amount;
                    Session["Message"] = $"Your villager brought back {amount} stones";
                }
                else if (roll == 2)
                {
                    int amount = rnd.Next(5, 10);
                    game.Food += amount;
                    Session["Message"] = $"Your villager brought back enough food to feed someone for {amount} days.";
                }
                else if (roll == 3)
                {
                    int amount = rnd.Next(6, 11);
                    game.Water += amount;
                    Session["Message"] = $"Your villager brought back {amount} gallons water";
                }
                else if (roll == 4)
                {
                    int amount = rnd.Next(6, 11);
                    game.Wood += amount;
                    Session["Message"] = $"Your villager brought back {amount} wood";
                }
                else if (roll == 5)
                {
                    Session["Message"] = $"Your villager found a friend to help! you gain a action";
                    game.Actions++;
                }
                else if (roll == 6)
                {
                    
                     game.Villagers--;
                    if (game.Actions > 0)
                    {
                        Session["Message"] = $"Your villager died while exploring! The villagers friend mourns their death. You lose an action";
                        game.Actions--;
                    }
                    else
                    {
                        if (game.Houses > 0)
                        {
                            Session["Message"] = $"Your villager was killed by a rival towns villager, and the enemy burned down your villagers house.";
                            game.Houses--;
                        }
                    }
                }
                db.GameDatas.AddOrUpdate(game);
                db.SaveChanges();
            }
            return RedirectToAction("GameView");
        }

        public ActionResult GameLose()
        {
            return View();
        }

        public ActionResult GameWin()
        {
            return View();
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