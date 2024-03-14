using System;
using System.Collections.Generic;
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

        List<int[]> previousOrders = new List<int[]>();



        public AIPlayer(Game game, int difficulty)
        {
            this.difficulty = difficulty;

            if(difficulty == 0 )
            {
                delay = 1500;
                depth = 1;
            }
            if (difficulty == 1)
            {
                delay = 1200;
                depth = 2;
            }
            if (difficulty == 2)
            {
                delay = 900;
                depth = 3;
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

            bool cardPlaced = false;

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
                        cardPlaced = true;
                        return Task.CompletedTask;
                    }
                    else if (hand.piles[x].pile.Peek().GetNumber() == target3 || hand.piles[x].pile.Peek().GetNumber() == target4)
                    {
                        game.placePiles[1].pile.Push(hand.piles[x].pile.Pop());
                        cardPlaced = true;
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

            if (!cardPlaced)
            {
                MoveCardsBetweenPiles();
            }

            

            return Task.CompletedTask;
        }

        public void MoveCardsBetweenPiles()
        {
            List<Pile> emptyPiles = new List<Pile>();

            int numOfEmptyPiles = 0;
            for (int i = 0; i < hand.piles.Length; i++)
            {
                if (hand.piles[i].pile.IsEmpty())
                {
                    emptyPiles.Add(hand.piles[i]);
                    numOfEmptyPiles++;
                }
            }

            if (hand.GetNumOfCards() > 5)
            {
                if (numOfEmptyPiles != 0)
                {
                    emptyPiles[0].pile.Push(hand.piles[hand.LongestPile()].pile.Pop());
                    MoveCardsBetweenPiles();
                }
            }
        }

        public void CalcTargets()
        {
            target1 = -1;
            target2 = -1;
            target3 = -1;
            target4 = -1;

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

        public int[] CalcTempTargets()
        {
            int[] targets = new int[4];

            if (!game.placePiles[0].pile.IsEmpty())
            {
                targets[0] = tempPile1.pile.Peek().GetNumber();
                targets[1] = tempPile1.pile.Peek().GetNumber();
            }
            if (!game.placePiles[1].pile.IsEmpty())
            {
                targets[2] = tempPile2.pile.Peek().GetNumber();
                targets[3] = tempPile2.pile.Peek().GetNumber();
            }

            if (targets[0] == 1) { targets[0] = 13; }
            else { targets[0] -= 1; }
            if (targets[1] == 13) { targets[1] = 1; }
            else { targets[1] += 1; }
            if (targets[2] == 1) { targets[2] = 13; }
            else { targets[2] -= 1; }
            if (targets[3] == 13) { targets[3] = 1; }
            else { targets[3] += 1; }

            return targets;
        }

        public override void MinMax()
        {
            bestMove = 0;

            int bestScore = 0;
            int score = 0;

            int[] bestScoreOrder = new int[depth];
            for (int i = 0; i < bestScoreOrder.Length; i++)
            {
                bestScoreOrder[i] = -1;
            }

            tempHand = new Hand(hand);

            tempPile1 = new Pile(game.pile1);
            tempPile2 = new Pile(game.pile2);

            int[] targets = CalcTempTargets();

            int[] order = new int[depth];
            for (int i = 0; i < order.Length; i++)
            {
                order[i] = -1;
            }

            int currentDepth = 0;

            Card cardToPlace = null;

            while (currentDepth < depth)
            {
                targets = CalcTempTargets();
                for (int i = 0; i < tempHand.piles.Length; i++)
                {
                    cardToPlace = null;
                    if (!tempHand.piles[i].pile.IsEmpty())
                    {
                        int cardNum = tempHand.piles[i].pile.Peek().GetNumber();
                        if (!tempHand.piles[i].pile.IsEmpty() && (cardNum == targets[0] || cardNum == targets[1] || cardNum == targets[2] || cardNum == targets[3]))
                        {
                            cardToPlace = tempHand.piles[i].pile.Peek();
                            PlaceTemp(i, targets);
                            order[currentDepth] = i;
                            currentDepth++;
                            break;
                        }
                    }
                }
                if(cardToPlace == null || currentDepth == depth && currentDepth != 0)
                {
                    if (bestScore < currentDepth)
                    {
                        bestScore = currentDepth;
                        bestScoreOrder = order;
                    }
                    break;
                }
            }


            if (bestScore != 0)
            {
                Place(bestScoreOrder[0], targets);
            }
















            /*for (int x = 0; x < hand.piles.Length; x++)
            {
                for (int i = 0; i < tempHand.piles.Length; i++)
                {
                    if (tempHand.piles[i].pile.Peek().GetNumber() == target1 || tempHand.piles[i].pile.Peek().GetNumber() == target2)
                    {
                        target1 = tempHand.piles[i].pile.Peek().GetNumber() + 1;
                        if (target1 > 13) target1 = 1;

                        target2 = tempHand.piles[i].pile.Peek().GetNumber() - 1;
                        if (target2 < 1) target2 = 13;

                        piles[i].AddLast(tempHand.piles[i].pile.Pop());

                        order.Add(i);

                        score++;
                    }

                    else if (tempHand.piles[i].pile.Peek().GetNumber() == target3 || tempHand.piles[i].pile.Peek().GetNumber() == target4)
                    {
                        target3 = tempHand.piles[i].pile.Peek().GetNumber() + 1;
                        if (target3 > 13) target3 = 1;

                        target4 = tempHand.piles[i].pile.Peek().GetNumber() - 1;
                        if (target4 < 1) target4 = 13;

                        piles[i].AddLast(tempHand.piles[i].pile.Pop());

                        order.Add(i);

                        score++;
                    }
                }
            }

            List<Card> cardsToPlace = new List<Card>();


            if (order.Count == 0)
            {
                for (int i = 0; i < order.Count; i++)
                {
                    cardsToPlace.Add(piles[order.Last()].RemoveAt(piles[order.Last()].Count()));
                }
            }

            CalcTargets();*/

            /*if(cardsToPlace.Last().GetNumber() == target1 || cardsToPlace.Last().GetNumber() == target2)
            {
                game.pile1.Push(cardsToPlace.Last());
            } else if(cardsToPlace.Last().GetNumber() == target3 || cardsToPlace.Last().GetNumber() == target4)
            {
                game.pile2.Push(cardsToPlace.Last());
            }*/
        }

        public void PlaceTemp(int pileIndex, int[] targets)
        {
            if (pileIndex == -1)
            {
                return;
            }
            if (tempHand.piles[pileIndex].pile.Peek().GetNumber() == targets[0] || tempHand.piles[pileIndex].pile.Peek().GetNumber() == targets[1])
            {
                tempPile1.pile.Push(tempHand.piles[pileIndex].pile.Pop());
            }
            else if (tempHand.piles[pileIndex].pile.Peek().GetNumber() == targets[2] || tempHand.piles[pileIndex].pile.Peek().GetNumber() == targets[3])
            {
                tempPile2.pile.Push(tempHand.piles[pileIndex].pile.Pop());
            }



            /*if (pileIndex == -1)
            {
                return;
            }
            if (hand.piles[pileIndex].pile.Peek().GetNumber() == target1 || hand.piles[pileIndex].pile.Peek().GetNumber() == target2)
            {
                game.placePiles[0].pile.Push(hand.piles[pileIndex].pile.Pop());
            }
            else if (hand.piles[pileIndex].pile.Peek().GetNumber() == target3 || hand.piles[pileIndex].pile.Peek().GetNumber() == target4)
            {
                game.placePiles[1].pile.Push(hand.piles[pileIndex].pile.Pop());
            }*/
        }

        public void Place(int pileIndex, int[] target)
        {
            if (pileIndex == -1)
            {
                return;
            }
            CalcTargets();
            if (hand.piles[pileIndex].pile.Peek().GetNumber() == target1 || hand.piles[pileIndex].pile.Peek().GetNumber() == target2)
            {
                game.placePiles[0].pile.Push(hand.piles[pileIndex].pile.Pop());
            }
            else if (hand.piles[pileIndex].pile.Peek().GetNumber() == target3 || hand.piles[pileIndex].pile.Peek().GetNumber() == target4)
            {
                game.placePiles[1].pile.Push(hand.piles[pileIndex].pile.Pop());
            }
        }
    }
}
