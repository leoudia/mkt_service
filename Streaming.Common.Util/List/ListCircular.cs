using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streaming.Common.Util.List
{
    /// <summary>
    /// No thread safe class
    /// Circular list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListCircular<T>
    {
        private NodeValue<T> first;
        private NodeValue<T> currentAdd;
        private NodeValue<T> currentGet;
        
        public void Add(T val)
        {
            if(!object.Equals(val, default(T)))
            {
                NodeValue<T> node = new NodeValue<T>(val);

                if(first == null)
                {
                    first = node;
                    currentAdd = node;
                    currentGet = node;

                    currentAdd.Next = node;
                }
                else
                {
                    node.Next = first;

                    var lastNode = currentAdd;
                    currentAdd = node;

                    lastNode.Next = currentAdd;
                }
            }

        }

        public T GetCurrent()
        {
            if (first == null)
                return default(T);

            return currentGet.GetValue();
        }

        public T Get()
        {
            if (first == null)
                return default(T);

            var resp = currentGet;
            currentGet = currentGet.Next;

            return resp.GetValue();
        }
    }

    public class NodeValue<T>
    {
        private T value;
        
        public NodeValue<T> Next { get; set; }

        public NodeValue(T value)
        {
            this.value = value;
        }

        public T GetValue()
        {

            return value;
        }
    }
}
