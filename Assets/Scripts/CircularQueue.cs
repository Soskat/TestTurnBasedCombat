using System.Collections.Generic;


namespace TestTurnBasedCombat
{
    /// <summary>
    /// Circular queue.
    /// </summary>
    public class CicrularQueue<T>
    {
        #region Fields
        /// <summary>Table represents the queue.</summary>
        private List<T> queue;
        /// <summary>The index of currently active queue element.</summary>
        private int iterator;
        #endregion


        #region Properties
        /// <summary>The number of elements in the queue.</summary>
        public int Count
        {
            get { return queue.Count; }
        }
        #endregion


        #region Constructors
        /// <summary>
        /// Default constructor. Creates a new instance of class <see cref="CicrularQueue"/>.
        /// </summary>
        public CicrularQueue()
        {
            queue = new List<T>();
            iterator = -1;
        }
        #endregion


        #region Methods
        /// <summary>
        /// Adds new object to the queue.
        /// </summary>
        /// <param name="obj">Object to add</param>
        public void Add(T obj)
        {
            queue.Add(obj);
        }

        /// <summary>
        /// Removes object from the queue.
        /// </summary>
        /// <param name="obj">Object to remove</param>
        public void Remove(T obj)
        {
            queue.Remove(obj);
            iterator %= queue.Count;
        }

        /// <summary>
        /// Gets value of the next element in the queue.
        /// </summary>
        /// <returns>Value of the element</returns>
        public T Next()
        {
            iterator = ++iterator % queue.Count;
            return queue[iterator];
        }

        /// <summary>
        /// Clears the queue.
        /// </summary>
        public void Clear()
        {
            queue.Clear();
        }

        /// <summary>
        /// Overrides default ToString() method to correctly show queue elements.
        /// </summary>
        /// <returns>CircularQueue in string format</returns>
        public override string ToString()
        {
            return queue.ToString();
        }
        #endregion
    }
}
