using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    /// <summary>
    /// A simple buffer of objects. You can add objects as you need to. When the buffer is full addItem will return true.
    /// The buffer can also be reset back to 0, which only resets the counters and does not change the items in the buffer,
    /// to do that call clear.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectBuffer<T>
    {
        private int bufferSize;
        private int currentPosition;
        private T[] dataFileBuffer;

        /// <summary>
        /// Constructor, can specify buffer size.
        /// </summary>
        /// <param name="bufferSize">The buffer size before full.</param>
        public ObjectBuffer(int bufferSize = 200)
        {
            this.bufferSize = bufferSize;
            clear();
        }

        /// <summary>
        /// Reset the current position, does not change the current items, although you won't be
        /// able to access them.
        /// </summary>
        public void reset()
        {
            currentPosition = 0;
        }

        /// <summary>
        /// Clear the buffer creating a new buffer array and resetting the position.
        /// </summary>
        public void clear()
        {
            currentPosition = 0;
            dataFileBuffer = new T[bufferSize];
        }

        /// <summary>
        /// Add an item to the buffer. Returns true if the buffer is full. Will throw an OutOfRangeException if
        /// it was previously full. You should handle this returning true somehow.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>True if the buffer is now full, false if not.</returns>
        public bool addItem(T item)
        {
            dataFileBuffer[currentPosition++] = item;
            return currentPosition == bufferSize;
        }

        /// <summary>
        /// An enumerator over the current items. This will enumerate the items from 0 to the current position.
        /// Note that unless you called clear there could be items after current positon that will not be enumerated.
        /// </summary>
        public IEnumerable<T> Items
        {
            get
            {
                for (int i = 0; i < currentPosition; ++i)
                {
                    yield return dataFileBuffer[i];
                }
            }
        }

        /// <summary>
        /// Returns true if there are items in this buffer.
        /// </summary>
        public bool HasItems
        {
            get
            {
                return currentPosition != 0;
            }
        }
    }
}
