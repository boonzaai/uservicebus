using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace Icris.uservicebus.Queues
{
    public class FileSystemQueue<T> : System.Collections.Generic.ICollection<T>, System.Collections.Generic.IEnumerable<T>
    {
        string path;
        /// <summary>
        /// Create the queue in the specified directory. Each message will be serialized here.
        /// </summary>
        /// <param name="path"></param>
        public FileSystemQueue(string path)
        {
            this.path = path;
        }

        private string GetNewMessagePath()
        {
            while (File.Exists(GetNewMessagePath()))
            {
                
                return GetNewMessagePath();
            }
            return this.path + "\\" + DateTime.Now.Ticks;
        }

        public int Count => Directory.EnumerateFiles(this.path).Count();

        public bool IsReadOnly => false;


        public void Add(T item)
        {
            File.WriteAllText(GetNewMessagePath(), JObject.FromObject(item).ToString());
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
