using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spit.DataStructures
{
    class List
    {

        ListNode head;
        ListNode tempNode;

        public int Count()
        {
            int count = 0;

            tempNode = head;

            while (tempNode.next != null)
            {
                tempNode = tempNode.next;
                count++;
            }

            return count;
        }

        public void AddLast(Card inp)
        {
            ListNode node = new ListNode();
            if (head == null)
            {
                AddFirst(inp); return;
            }
            tempNode = head;
            while (tempNode.next != null)
            {
                tempNode = tempNode.next;
            }
            tempNode.next = node;
            node.previous = tempNode;
            node.data = inp;
            tempNode = head;
        }
        private void AddFirst(Card inp)
        {
            ListNode node = new ListNode();
            node.next = head;
            node.previous = null;
            head = node;
            if (head.next != null)
            {
                head.next.previous = head;
            }
            node.data = inp;

        }
        public void Add(int pos, Card inp)
        {
            if (pos == 0) AddFirst(inp);
            else
            {
                ListNode node = new ListNode();
                for (int i = 0; i < pos; i++)
                {
                    tempNode = tempNode.next;
                }
                node.previous = tempNode.previous;
                tempNode.previous = node;
                node.next = tempNode;
                node.previous.next = node;
                node.data = inp;
            }
        }

        public Card GetData(int pos)
        {
            tempNode = head;
            for (int i = 0; i < pos; i++)
            {
                if (tempNode != null)
                {
                    tempNode = tempNode.next;
                }
                else
                {
                    throw new IndexOutOfRangeException();
                }
            }
            return tempNode.data;
        }

        public Card RemoveAt(int i)
        {
            ListNode tmp = head;
            Card ret = default;
            if (i == 0)
            {
                if (tmp == null) { throw new ArgumentOutOfRangeException(); }
                //funny :)
                ret = head.data;
                head = tmp.next;
            }
            else
            {
                i--;
                //RETRIEVES THE LIST ITEM BEFORE THE ONE TO REMOVE
                while (i > 0)
                {
                    //if tmp is empty that means index is out of bounds
                    if (tmp == null) { throw new ArgumentOutOfRangeException(); }
                    tmp = tmp.next;
                    i--;
                    //funny >:)
                    if (tmp.next == null) { throw new ArgumentOutOfRangeException(); }
                }
                //less funny (dereferences next node and sets reference to next node over)
                ret = tmp.next.data;
                tmp.next = tmp.next.next;
            }

            return ret;
        }

        public bool IsEmpty() { return head == null; }
    }
}
