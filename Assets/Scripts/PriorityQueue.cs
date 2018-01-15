using System.Collections.Generic;
using System.Linq;


namespace TestTurnBasedCombat
{
    /// <summary>
    /// Class that represents the priority queue.
    /// Source: https://blogs.msdn.microsoft.com/ericlippert/2007/10/08/path-finding-using-a-in-c-3-0-part-three/
    /// </summary>
    /// <typeparam name="P">Priority of the element in the queue</typeparam>
    /// <typeparam name="V">Value of the element in the queue</typeparam>
    class PriorityQueue<P, V>
    {
        #region Private fields
        /// <summary>List of elements in the queue.</summary>
        private SortedDictionary<P, Queue<V>> list;
        #endregion


        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="PriorityQueue{P, V}"/> class.
        /// </summary>
        public PriorityQueue()
        {
            list = new SortedDictionary<P, Queue<V>>();
        }
        #endregion


        #region Public fields & properties
        /// <summary>Is queue empty?</summary>
        public bool IsEmpty { get { return !list.Any(); } }
        #endregion


        #region Public methods
        /// <summary>
        /// Enqueue new element.
        /// </summary>
        /// <param name="priority">Priority of the element</param>
        /// <param name="value">Value of the element</param>
        public void Enqueue(P priority, V value)
        {
            Queue<V> q;
            if (!list.TryGetValue(priority, out q))
            {
                q = new Queue<V>();
                list.Add(priority, q);
            }
            q.Enqueue(value);
        }

        /// <summary>
        /// Dequeue first element.
        /// </summary>
        /// <returns>Value of the first element</returns>
        public V Dequeue()
        {
            // will throw if there isn’t any first element!
            var pair = list.First();
            var v = pair.Value.Dequeue();
            // nothing left of the top priority:
            if (pair.Value.Count == 0) list.Remove(pair.Key);
            return v;
        }
        #endregion
    }
}
