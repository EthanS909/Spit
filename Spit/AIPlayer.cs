using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Spit.DataStructures;

namespace Spit
{
    internal class AIPlayer : Player
    {
        private Game game;

        // Reaction Speed of the AI
        private int delay;

        // Number of moves the AI looks ahead to choose the best card to place
        private int depth;
        private int bestMove;
        private int difficulty;

        private int target1;
        private int target2;
        private int target3;
        private int target4;

        private Pile tempPile1 = new Pile();
        private Pile tempPile2 = new Pile();
        
        private Hand tempHand = new Hand();

        private List<Stack> tempPiles = new List<Stack>();



        public AIPlayer(Game game, int difficulty)
        {
            this.difficulty = difficulty;

            if(difficulty == 0 )
            {
                delay = 1500;
                depth = 0;
            }
            if (difficulty == 1)
            {
                delay = 1200;
                depth = 1;
            }
            if (difficulty == 2)
            {
                delay = 900;
                depth = 2;
            }

            this.game = game;

            tempPile1 = game.placePiles[0];
            tempPile2 = game.placePiles[1];

            tempHand = hand;
        }

        public override int GetDelay()
        {
            return delay;
        }

        public override int GetDifficulty()
        {
            return difficulty;
        }

        public override async void Move()
        {
            await CalcBestMove();
        }

        public  Task CalcBestMove()
        {
            bestMove = 0;

            int bestScore = 0;
            int score = 0;

            List pile1 = new List();
            List pile2 = new List();
            List pile3 = new List();
            List pile4 = new List();
            List pile5 = new List();

            List<List> piles = new List<List>
            {
                pile1,
                pile2,
                pile3,
                pile4,
                pile5
            };

            List<int> order = new List<int>();

            CalcTargets();

            for (int x = 0; x < hand.piles.Length; x++)
            {
                /*for (int i = 0; i < tempPiles.Count; i++)
                {
                    if (tempPiles[i].Peek().GetNumber() == target1 || tempPiles[i].Peek().GetNumber() == target2)
                    {
                        target1 = tempPiles[i].Peek().GetNumber() + 1;
                        if (target1 > 13) target1 = 1;

                        target2 = tempPiles[i].Peek().GetNumber() - 1;
                        if (target2 < 1) target2 = 13;

                        piles[i].AddLast(tempPiles[i].Pop());

                        order.Add(i);

                        score++;
                    }

                    else if (tempPiles[i].Peek().GetNumber() == target3 || tempPiles[i].Peek().GetNumber() == target4)
                    {
                        target3 = tempPiles[i].Peek().GetNumber() + 1;
                        if (target3 > 13) target3 = 1;

                        target4 = tempPiles[i].Peek().GetNumber() - 1;
                        if (target4 < 1) target4 = 13;

                        piles[i].AddLast(tempPiles[i].Pop());

                        order.Add(i);

                        score++;
                    }
                }*/

                if (!hand.piles[x].pile.IsEmpty())
                {
                    if (hand.piles[x].pile.Peek().GetNumber() == target1 || hand.piles[x].pile.Peek().GetNumber() == target2)
                    {
                        game.placePiles[0].pile.Push(hand.piles[x].pile.Pop());
                        return Task.CompletedTask;
                    }
                    else if (hand.piles[x].pile.Peek().GetNumber() == target3 || hand.piles[x].pile.Peek().GetNumber() == target4)
                    {
                        game.placePiles[1].pile.Push(hand.piles[x].pile.Pop());
                        return Task.CompletedTask;
                    }
                }
            }

            /*List<Card> cardsToPlace = new List<Card>();


            if (order.Count == 0)
            {
                for (int i = 0; i < order.Count; i++)
                {
                    cardsToPlace.Add(piles[order.Last()].RemoveAt(piles[order.Last()].Count()));
                }
            }

            CalcTargets();

            if(cardsToPlace.Last().GetNumber() == target1 || cardsToPlace.Last().GetNumber() == target2)
            {
                game.pile1.Push(cardsToPlace.Last());
            } else if(cardsToPlace.Last().GetNumber() == target3 || cardsToPlace.Last().GetNumber() == target4)
            {
                game.pile2.Push(cardsToPlace.Last());
            }*/


            

            return Task.CompletedTask;
        }

        public void CalcTargets()
        {
            if (!game.placePiles[0].pile.IsEmpty())
            {
                target1 = game.placePiles[0].pile.Peek().GetNumber();
                target2 = game.placePiles[0].pile.Peek().GetNumber();
            }
            if (!game.placePiles[1].pile.IsEmpty())
            {
                target3 = game.placePiles[1].pile.Peek().GetNumber();
                target4 = game.placePiles[1].pile.Peek().GetNumber();
            }

            if (target1 == 1) { target1 = 13; }
            else { target1 -= 1; }
            if (target2 == 13) { target2 = 1; }
            else { target2 += 1; }
            if (target3 == 1) { target3 = 13; }
            else { target3 -= 1; }
            if (target4 == 13) { target4 = 1; }
            else { target4 += 1; }
        }

        public void MinMax()
        {
            bestMove = 0;

            int bestScore = 0;
            int score = 0;

            List pile1 = new List();
            List pile2 = new List();
            List pile3 = new List();
            List pile4 = new List();
            List pile5 = new List();

            List<List> piles = new List<List>
            {
                pile1,
                pile2,
                pile3,
                pile4,
                pile5
            };

            List<int> order = new List<int>();

            CalcTargets();

            for (int x = 0; x < hand.piles.Length; x++)
            {
                /*for (int i = 0; i < tempPiles.Count; i++)
                {
                    if (tempPiles[i].Peek().GetNumber() == target1 || tempPiles[i].Peek().GetNumber() == target2)
                    {
                        target1 = tempPiles[i].Peek().GetNumber() + 1;
                        if (target1 > 13) target1 = 1;

                        target2 = tempPiles[i].Peek().GetNumber() - 1;
                        if (target2 < 1) target2 = 13;

                        piles[i].AddLast(tempPiles[i].Pop());

                        order.Add(i);

                        score++;
                    }

                    else if (tempPiles[i].Peek().GetNumber() == target3 || tempPiles[i].Peek().GetNumber() == target4)
                    {
                        target3 = tempPiles[i].Peek().GetNumber() + 1;
                        if (target3 > 13) target3 = 1;

                        target4 = tempPiles[i].Peek().GetNumber() - 1;
                        if (target4 < 1) target4 = 13;

                        piles[i].AddLast(tempPiles[i].Pop());

                        order.Add(i);

                        score++;
                    }
                }*/
            }

            /*List<Card> cardsToPlace = new List<Card>();


            if (order.Count == 0)
            {
                for (int i = 0; i < order.Count; i++)
                {
                    cardsToPlace.Add(piles[order.Last()].RemoveAt(piles[order.Last()].Count()));
                }
            }

            CalcTargets();

            if(cardsToPlace.Last().GetNumber() == target1 || cardsToPlace.Last().GetNumber() == target2)
            {
                game.pile1.Push(cardsToPlace.Last());
            } else if(cardsToPlace.Last().GetNumber() == target3 || cardsToPlace.Last().GetNumber() == target4)
            {
                game.pile2.Push(cardsToPlace.Last());
            }*/
        }
    }
}
