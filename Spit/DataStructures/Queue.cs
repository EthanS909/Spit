using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spit.DataStructures
{
    class Queue
    {
        private List q = new List();

        private int end = 0;

        public void EnQueue(Card input)
        {
            q.AddLast(input);
            end++;
        }

        public Card DeQueue()
        {
            Card data = q.RemoveAt(end);
            end--;

            return data;
        }

        public int Count()
        {
            return q.Count();
        }
    }
}
