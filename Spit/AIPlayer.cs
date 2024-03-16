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
        private int depth = 1;
        private int difficulty;

        private int target1;
        private int target2;
        private int target3;
        private int target4;

        private Pile tempPile1 = new Pile();
        private Pile tempPile2 = new Pile();
        
        private Hand tempHand = new Hand();

        bool placed;



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
                depth = 3;
            }
            if (difficulty == 2)
            {
                delay = 900;
                depth = 5;
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
            Reset();
            await CalcBestMove();
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

        // Resets all information relating to the last Best Move
        public override void Reset()
        {
            startingCardIndex = 0;
            bestScore = 0;
            bestScoreOrder = new int[depth];
            for (int i = 0; i < bestScoreOrder.Length; i++)
            {
                bestScoreOrder[i] = -1;
            }
        }


        // Calculates the best move for the AI depending on the difficulty set by the player
        int startingCardIndex = 0;
        int[] bestScoreOrder;
        int bestScore = 0;
        public Task CalcBestMove()
        {
            placed = false;

            tempHand = new Hand(hand);

            tempPile1 = new Pile(game.placePiles[0]);
            tempPile2 = new Pile(game.placePiles[1]);

            int[] targets = CalcTempTargets();

            int[] order = new int[depth];
            for (int i = 0; i < order.Length; i++)
            {
                order[i] = -1;
            }

            int currentDepth = 0;
            int lastDepth = 0;

            Card cardToPlace = null;

            while (currentDepth < depth)
            {
                targets = CalcTempTargets();
                for (int i = startingCardIndex; i < tempHand.piles.Length; i++)
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
                            lastDepth = currentDepth;
                            currentDepth++;
                            break;
                        }
                    }
                    if(lastDepth != currentDepth && cardToPlace != null)
                    {
                        i = 0;
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

            if (bestScoreOrder[0] < 4)
            {
                if(startingCardIndex < 4)
                {
                    if(startingCardIndex >= bestScoreOrder[0])
                    {
                        startingCardIndex++;
                    }
                    else
                    {
                        startingCardIndex = bestScoreOrder[0] + 1;
                    }
                    CalcBestMove();
                }
            }

            if (bestScore != 0 && !placed)
            {
                Place(bestScoreOrder[0], targets);
            }

            if(!placed)
            {
                MoveCardsBetweenPiles();
            }

            return Task.CompletedTask;
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
                placed = true;
            }
            else if (hand.piles[pileIndex].pile.Peek().GetNumber() == target3 || hand.piles[pileIndex].pile.Peek().GetNumber() == target4)
            {
                game.placePiles[1].pile.Push(hand.piles[pileIndex].pile.Pop());
                placed = true;
            }
        }
    }
}
