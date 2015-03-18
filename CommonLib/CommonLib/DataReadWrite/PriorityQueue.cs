using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace CommonLib
{
    public class PriorityQueue<T>    
    {        
        public int Total_size;
        SortedDictionary<T, Queue<T>> storage;
        
        public PriorityQueue ()
        {
            this.storage = new SortedDictionary<T, Queue<T>>();
            this.Total_size = 0;
        }
        
        public bool IsEmpty ()
        {
            return (Total_size == 0);
        }
        
        public T Dequeue()
        {
            if (IsEmpty())
            {
                throw new Exception("Please check that priorityQueue is not empty before dequeing");
            }
            else
            {                
                foreach (Queue<T> q in storage.Values)
                {                    // we use a sorted dictionary
                    if (q.Count > 0)
                    {
                        Total_size--;
                        return q.Dequeue();
                    }
                }
            }
            
            return default(T); // not supposed to reach here.
        }
        
        // same as above, except for peek.
        
        public T Peek()
        {
            if (IsEmpty())
                throw new Exception("Please check that priorityQueue is not empty before dequeing");
            else
            {
                foreach (Queue<T> q in storage.Values)
                {
                    if (q.Count > 0)
                        return q.Peek();
                }
            }
            return default(T); // not supposed to reach here.
        }

        public T PeekLast()
        {
            if (IsEmpty())
                throw new Exception("Please check that priorityQueue is not empty before dequeing");
            else
            {
                Queue<T> q = storage.Values.Last();
                
                if (q.Count > 0)
                    return q.Peek();
                
            }
            return default(T); // not supposed to reach here.
        }
        
        public T Dequeue (T prio)
        {
            Total_size--;

            T ret = default(T);
            if (storage[prio].Count > 0)
            {
                ret = storage[prio].Dequeue();
                if (storage[prio].Count == 0)
                    storage.Remove(prio);
            }
            return ret;
        }
        
        public void Enqueue (T prio)
        {
            if (!storage.ContainsKey (prio)) 
            {
                storage.Add (prio, new Queue<T> ());
                Enqueue (prio);
                // run again
            } 
            else
            {
                storage[prio].Enqueue(prio);
                Total_size++;
            }
        }
    }
}
