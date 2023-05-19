﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spit.DataStructures;

namespace Spit
{
    class Deck
    {
        private Stack deck = new Stack();

        public void CreateDeck()
        {
            for(int x = 0; x < 4; x++)
            {
                for(int i = 0; i < 13; i++)
                {
                    if(x == 0)
                    {
                        deck.Push(new Card("heart", i + 1));
                    }
                    if (x == 1)
                    {
                        deck.Push(new Card("diamond", i + 1));
                    }
                    if (x == 2)
                    {
                        deck.Push(new Card("club", i + 1));
                    }
                    if (x == 3)
                    {
                        deck.Push(new Card("spade", i + 1));
                    }

                }
            }
        }

        public Card GetTopCard()
        {
            return deck.Peek();
        }

        public Stack GetCards()
        {
            return deck;
        }

        public void Shuffle()
        {
            //Unloads cards into an array
            /*Card[] unshuffledDeck = new Card[52];
            int count = 0;
            while(!deck.IsEmpty())
            {
                unshuffledDeck[count] = deck.Pop();
                count++;
            }*/

            //Shuffle cards
            Card[] shuffledDeck = new Card[52];
            Random rnd = new Random();
            for(int x = 0; x < shuffledDeck.Length; x++)
            {
                int randomSpace = rnd.Next(0, 51);
                if (!deck.IsEmpty())
                {
                    if (shuffledDeck[randomSpace] == null)
                    {
                        shuffledDeck[randomSpace] = deck.Pop();
                    }
                    else
                    {
                        int i = randomSpace;
                        while (shuffledDeck[i] != null)
                        {
                            i = (i + 1) % shuffledDeck.Length;
                        }
                        shuffledDeck[i] = deck.Pop();
                    }
                }
            }

            //Loads cards back into the deck
            for(int x = 0; x < shuffledDeck.Length; x++)
            {
                deck.Push(shuffledDeck[x]);
            }
        }

        public bool DisplayCards()
        {
            bool display = true;
            int numberOfCardsHidden = 5;
            for(int x = 0; x < numberOfCardsHidden; x++)
            {
                //oxxxx
                // oxxx
                //  oxx
                //   ox
                //    o
            }
            return display;
        }
    }
}
