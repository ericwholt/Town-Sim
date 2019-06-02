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
            int chanceForAttack = rnd.Next(1, 101);

            //check if we have lost all our villagers. If so then lose the game.
            if (game.Villagers <= 0)
            {
                //show lose game view
                return RedirectToAction("GameLose");
            }
            else
            {
                //If we have more houses than villagers display message saying they are moving in.
                if (game.Houses > game.Villagers)
                {
                    int villagersMovingIn = game.Houses - game.Villagers; //How many villagers are moving in

                    //for grammar of message
                    if (villagersMovingIn == 1)
                    {
                        Session["Message"] = $"A new villager moved in!";
                    }
                    else
                    {
                        Session["Message"] = $"{villagersMovingIn} new villagers moved in!";
                    }

                }
                game.Villagers = game.Houses;
            }

            //If we have 10 or more villagers the game is won
            if (game.Villagers >= 10)
            {
                return RedirectToAction("GameWin");
            }
            else
            {
                //Calculate next day
                //Produce before we consume
                game.Water += game.Wells;
                game.Food += game.Farm;

                for (int i = 0; i < game.Villagers; i++)
                {
                    //consume water if we have it
                    if (game.Water >= 2)
                    {
                        game.Water -= 2;
                    }
                    else //We don't have water for this villager so kill it.
                    {
                        //Check to see if we have villager to avoid negative. This should always be true just a saftey in case sky is falling.
                        if (game.Villagers > 0)
                        {
                            game.Villagers--;
                        }
                    }

                    //If we have food and a villager left then consume food
                    if (game.Food >= 1 && game.Villagers > 0)
                    {
                        game.Food--;
                    }
                    else //We don't have food
                    {
                        //Do we have villagers? Then kill one.
                        if (game.Villagers > 0)
                        {
                            game.Villagers--;
                        }
                    }
                }
                //check for loss again so we don't have to wait for new day click
                if (game.Villagers <= 0)
                {
                    //show lose game view
                    return RedirectToAction("GameLose");
                }
                //Handle day transition
                game.Day++;
                game.Actions = game.Villagers;
                db.GameDatas.AddOrUpdate(game);
                db.SaveChanges();
            }
            //Check if raiders can attempt to attack the Town.
            if (chanceForAttack > 90 && game.Day > 15 && game.Castles <= 0)
            {
                return RedirectToAction("Raiders");
            }
            return RedirectToAction("GameView");
        }

        //Add buildings logic
        public ActionResult Build(string building)
        {
            GameData game = db.GameDatas.ToList().First(); //Get latest data from database

            //Do we have the actions to do this? View should always make this true, but double check in case they put url in manually
            if (game.Actions >= 1)
            {

                game.Actions--;// Use one action.

                //Cycle through building types passed through parameter and see if they match and we have resources.
                if (building == "House" && game.Wood >= 5)
                {
                    game.Houses++;
                    game.Wood -= 5;
                }
                else if (building == "Well" && game.Stone >= 3 && game.Wood >= 2)
                {
                    game.Stone -= 3;
                    game.Wood -= 2;
                    game.Wells++;
                }
                else if (building == "Farm" && game.Stone >= 2 && game.Wood >= 3)
                {
                    game.Stone -= 2;
                    game.Wood -= 3;
                    game.Farm++;
                }
                else if (building == "Castle" && game.Stone >= 10 && game.Wood >= 10 && game.Houses >= 5)
                {
                    game.Stone -= 10;
                    game.Wood -= 10;
                    game.Castles++;
                }
                else
                {
                    //Shouldn't get here unless url is put in manually with incorrect building type
                    game.Actions++; // then give them back their action
                    return RedirectToAction("GameView"); //Send them back to gameview. No need to update and save database
                }

                //game state
                db.GameDatas.AddOrUpdate(game);
                db.SaveChanges();
            }
            return RedirectToAction("GameView");
        }

        //Gather Resource logic
        public ActionResult Gather(string resource)
        {
            GameData game = db.GameDatas.ToList().First();

            //Do we have the actions to do this? View should always make this true, but double check in case they put url in manually
            if (game.Actions >= 1)
            {
                game.Actions--;
                int amount = rnd.Next(1, 7) - 1;
                if (resource == "Food")
                {
                    //Random 0-4 food gathered weighted for middle
                    int[] foodFound = { 0, 1, 2, 2, 3, 4 };
                    game.Food += foodFound[amount];
                    Session["Message"] = $"Villager gathered {foodFound[amount]} food.";
                }
                else if (resource == "Water")
                {
                    //Random 1-5 water gathered weighted for middle
                    int[] waterFound = { 1, 2, 3, 3, 4, 5 };
                    game.Water += waterFound[amount];
                    Session["Message"] = $"Villager gathered {waterFound[amount]} gallons of water.";
                }
                else if (resource == "Wood")
                {
                    //Random 1-5 water gathered weighted for middle
                    int[] woodFound = { 1, 2, 3, 3, 4, 5 };
                    game.Wood += woodFound[amount];
                    Session["Message"] = $"Villager gathered {woodFound[amount]} wood.";
                }
                else if (resource == "Stone")
                {
                    //Random 1-5 water gathered weighted for middle
                    int[] stoneFound = { 1, 2, 2, 3, 3, 4 };
                    game.Stone += stoneFound[amount];
                    Session["Message"] = $"Villager gathered {stoneFound[amount]} stones.";
                }
                else
                {
                    //Shouldn't get here unless url is put in manually with incorrect resource type
                    game.Actions++; // then give them back their action
                    return RedirectToAction("GameView"); //Send them back to gameview. No need to update and save database
                }
                db.GameDatas.AddOrUpdate(game);
                db.SaveChanges();
            }
            return RedirectToAction("GameView");
        }

        //Explore Logic
        public ActionResult Explore()
        {
            GameData game = db.GameDatas.ToList().First(); //Get latest game state from database

            //Do we have the actions to do this? View should always make this true, but double check in case they put url in manually

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
                        if (game.Houses > 0 && game.Castles <= 0)
                        {
                            Session["Message"] = $"Your villager was killed by a rival towns villager, and the enemy burned down that villagers house!";
                            game.Houses--;
                        }
                        else
                        {
                            Session["Message"] = $"Your villager was killed by a rival towns villager!";
                        }
                    }
                }
                else
                {
                    //Shouldn't get here
                    game.Actions++; // then give them back their action
                    return RedirectToAction("GameView"); //Send them back to gameview. No need to update and save database
                }

                //Game State
                db.GameDatas.AddOrUpdate(game);
                db.SaveChanges();
            }
            return RedirectToAction("GameView");
        }


        //Raider logic
        public ActionResult Raiders()
        {
            GameData game = db.GameDatas.ToList().First();

            //Weight choices towards food
            int[] weightedRaiderActions = { 1, 2, 2, 2, 3, 3 };

            //Possible animals that can attack
            string[] animals = { "vicuna", "wildcat", "peccary", "ibex", "rabbit", "hog", "bear", "racoon", "chimpanzee", "armadillo", "mongoose", "mule", "rhinoceros", "cat", "meerkat" };

            //This is the action the raider will take
            int raiderAction = weightedRaiderActions[rnd.Next(0, weightedRaiderActions.Length)];

            //Select the animal that will ravage the town
            string animal = animals[rnd.Next(0, animals.Length)];



            switch (raiderAction)
            {
                case 1:
                    int deaths = rnd.Next(1, 3); //1-2 villagers will be ravaged by the choosen animal

                    //for proper grammar
                    if (deaths == 1)
                    {
                        Session["Message"] = $"A {animal} kills a villager and burns down their house.";
                    }
                    else
                    {
                        Session["Message"] = $"A {animal} kills {deaths} villagers and burns down their houses.";
                    }

                    //check to make sure villagers doesn't go into the negative
                    if (game.Villagers >= deaths)
                    {
                        game.Villagers -= deaths; //Remove the villagers that died
                    }
                    else
                    {
                        game.Villagers = 0;
                    }

                    //check to make sure houses doesn't go into the negative
                    if (game.Houses >= deaths)
                    {
                        game.Houses -= deaths; //Remove houses for the villagers
                    }
                    else
                    {
                        game.Houses = 0;
                    }
                    break;
                case 2: //Raider takes food

                    //check to make sure food doesn't go into the negative
                    if (game.Food >= raiderAction)
                    {
                    Session["Message"] = $"A {animal} eats {raiderAction} days of food before it runs off.";
                    game.Food -= raiderAction; //Remove food
                    }
                    else
                    {
                        Session["Message"] = $"A {animal} eats all the food before it runs off.";
                        game.Food = 0;
                    }
                    break;
                case 3: //Raider takes water

                    //check to make sure water doesn't go into the negative
                    if (game.Water >= raiderAction)
                    {
                    Session["Message"] = $"A {animal} drinks {raiderAction} gallons of water before it runs off.";
                    game.Water -= raiderAction; //Remove water
                    }
                    else
                    {
                        Session["Message"] = $"A {animal} drinks all the water before it runs off.";
                        game.Water = 0;
                    }
                    break;
            }

            //Save Game State
            db.GameDatas.AddOrUpdate(game);
            db.SaveChanges();
            return RedirectToAction("GameView");
        }


        //Show you lose view
        public ActionResult GameLose()
        {
            return View();
        }

        //Show you win view
        public ActionResult GameWin()
        {
            return View();
        }

        //Start new game
        public ActionResult NewGame()
        {
            GameData game = db.GameDatas.ToList().First();

            //Set game state to game start
            game.Actions = 1;
            game.Day = 1;
            game.Farm = 0;
            game.Food = 6;
            game.Houses = 1;
            game.Castles = 0;
            game.Stone = 0;
            game.Villagers = 1;
            game.Water = 8;
            game.Wells = 0;
            game.Wood = 0;

            //Save Game State
            db.GameDatas.AddOrUpdate(game);
            db.SaveChanges();

            return RedirectToAction("GameView");
        }
    }
}