using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spit.DataStructures
{
    class Stack
    {
        List stack = new List();

        public void Push(Card inp)
        {
            stack.AddLast(inp);
        }

        public Card Pop()
        {
            Card card = stack.Remove(stack.Count());
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
