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

        private int target1;
        private int target2;
        private int target3;
        private int target4;

        private Stack tempPile1 = new Stack();
        private Stack tempPile2 = new Stack();
        
        private Stack tempPile1st = new Stack();
        private Stack tempPile2nd = new Stack();
        private Stack tempPile3rd = new Stack();
        private Stack tempPile4th = new Stack();
        private Stack tempPile5th = new Stack();

        private List<Stack> tempPiles = new List<Stack>();



        public AIPlayer(Game game, int difficulty)
        {
            if(difficulty == 0 )
            {
                delay = 1000;
                depth = 1;
            }
            if (difficulty == 1)
            {
                delay = 800;
                depth = 1;
            }
            if (difficulty == 2)
            {
                delay = 600;
                depth = 3;
            }

            this.game = game;

            tempPile1 = game.pile1;
            tempPile2 = game.pile2;

            tempPile1st = piles[0];
            tempPile2nd = piles[1];
            tempPile3rd = piles[2];
            tempPile4th = piles[3];
            tempPile5th = piles[4];

            tempPiles.Add(tempPile1st);
            tempPiles.Add(tempPile2nd);
            tempPiles.Add(tempPile3rd);
            tempPiles.Add(tempPile4th);
            tempPiles.Add(tempPile5th);
        }

        public override int GetDelay()
        {
            return delay;
        }

        public override async void Move()
        {
            await CalcBestMove();
        }

        public override Task CalcBestMove()
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

            for (int x = 0; x < tempPiles.Count; x++)
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

                if (!tempPiles[x].IsEmpty())
                {
                    if (tempPiles[x].Peek().GetNumber() == target1 || tempPiles[x].Peek().GetNumber() == target2)
                    {
                        game.pile1.Push(tempPiles[x].Pop());
                        game.Update();
                        return Task.CompletedTask;
                    }
                    else if (tempPiles[x].Peek().GetNumber() == target3 || tempPiles[x].Peek().GetNumber() == target4)
                    {
                        game.pile2.Push(tempPiles[x].Pop());
                        game.Update();
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

        public override void CalcTargets()
        {
            target1 = game.pile1.Peek().GetNumber();
            target2 = game.pile1.Peek().GetNumber();
            target3 = game.pile2.Peek().GetNumber();
            target4 = game.pile2.Peek().GetNumber();

            if (target1 == 1) { target1 = 13; }
            else { target1 -= 1; }
            if (target2 == 13) { target2 = 1; }
            else { target2 += 1; }
            if (target3 == 1) { target3 = 13; }
            else { target3 += 1; }
            if (target4 == 13) { target4 = 1; }
            else { target4 -= 1; }
        }
    }
}
