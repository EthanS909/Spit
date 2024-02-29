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
            return Length(head);
        }

        private int Length(ListNode node)
        {
            //if node == null, the list is empty
            if (node != null)
            {
                return 1 + Length(node.next);
            }
            else { return 0; }
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
            if (head == null) return tempNode.data;

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
            //checks bounds before proceeding
            if (i < 0 || i > this.Count() || head == null) { throw new ArgumentOutOfRangeException(); }
            Card ret = default;
            ListNode tmp = head;
            //needs to be handled differently if index is 0
            if (i == 0)
            {
                ret = head.data;
                if (head.next != null)
                {
                    head.next.previous = null;
                    head = head.next;
                }
                else
                {
                    //happens if head.next == null
                    head = null;
                }
            }
            else
            {
                //moves down list to index
                while (i > 0)
                {
                    tmp = tmp.next;
                    i--;
                }

                ret = tmp.data;
                //handles differently if at end of list (tmp.next == null)
                if (tmp.next == null)
                {
                    //simply dereference tmp
                    tmp.previous.next = null;
                }
                else
                {
                    //more complicated dereferencing
                    tmp.previous.next = tmp.next;
                    tmp.next.previous = tmp.previous;
                }
            }
            return ret;
        }

        public bool IsEmpty() { return head == null; }
    }
}
