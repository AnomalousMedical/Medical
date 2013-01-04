using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    class Node<T>
    {
        public Node(T data = default(T), Node<T> next = null, Node<T> previous = null)
        {
            this.Data = data;
        }

        public T Data { get; set; }

        public Node<T> Next { get; set; }

        public bool HasNext
        {
            get
            {
                return Next != null;
            }
        }

        public Node<T> Previous { get; set; }

        public bool HasPrevious
        {
            get
            {
                return Previous != null;
            }
        }
    }
}
