using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Spit.DataStructures
{
    class Stack
    {
        public List stack = new List();

        public void Push(Card inp)
        {
            stack.AddLast(inp);
        }

        public Card Pop()
        {
            Card card = stack.RemoveAt(stack.Count() - 1);
            return card;
        }

        public Card Peek()
        {
            return stack.GetData(stack.Count() - 1);
        }

        public bool IsEmpty() { return stack.IsEmpty(); }

        public int Length() { return stack.Count(); }

        public Card GetCardAt(int pos)
        {
            return stack.GetData(pos);
        }
    }
}
