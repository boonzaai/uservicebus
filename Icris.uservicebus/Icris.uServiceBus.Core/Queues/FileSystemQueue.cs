using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Threading;
using Newtonsoft.Json;
using System.Transactions;

namespace Icris.uServiceBus.Core.Queues
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
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            this.path = path;
        }
        /// <summary>
        /// Little helper tries to open a file, returns false if it fails so threads can wait on it to be free.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="writer"></param>
        /// <returns></returns>
        protected bool TryToOpenFile(string file, out StreamWriter writer)
        {
            try
            {
                writer = new StreamWriter(File.Open(file, FileMode.CreateNew, FileAccess.Write));
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                writer = null;
                return false;
            }
            finally
            {
            }

            //file is not locked
            return true;
        }
        /// <summary>
        /// Little helper tries to open a file, returns false if it fails so threads can wait on it to be free.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="writer"></param>
        /// <returns></returns>
        protected bool TryReadAndDeleteFile(string file, out string content)
        {
            try
            {
                using (var stream = File.Open(file, FileMode.Open, FileAccess.ReadWrite))
                {
                    content = new StreamReader(stream).ReadToEnd();                    
                }
                File.Delete(file);
            }           
            catch (Exception e)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                content = null;
                return false;
            }
            finally
            {
            }

            //file is not locked
            return true;
        }
        private StreamWriter GetNewMessageStream()
        {
            StreamWriter writer;
            while (!TryToOpenFile(this.path + "\\" + DateTime.Now.Ticks + ".uq.json", out writer)) ;
            return writer;

        }

        public int Count => Directory.EnumerateFiles(this.path, "*.uq.json").Count();

        public bool IsReadOnly => false;


        public void Add(T item)
        {
            using (var w = GetNewMessageStream())
            {
                w.Write(JObject.FromObject(item).ToString());
                w.Flush();
            }
        }

        public void Clear()
        {
            Directory.EnumerateFiles(this.path, "*.uq.json").ToList().ForEach(x => File.Delete(x));
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

        public T Peek()
        {
            using (var t = new TransactionScope())
            {
                var first = Directory.EnumerateFiles(this.path, "*.uq.json").OrderBy(x => x).First();
                t.Complete();
                return JsonConvert.DeserializeObject<T>(File.ReadAllText(first));
            }
        }

        public T Receive()
        {
            string content;
            var first = Directory.EnumerateFiles(this.path, "*.uq.json").OrderBy(x => x).First();
            while (!TryReadAndDeleteFile(first, out content))
                return Receive();
            if (content == null)
                return Receive();
            var obj = JsonConvert.DeserializeObject<T>(content);
            //File.Delete(first);
            return obj;
        }
    }
}
