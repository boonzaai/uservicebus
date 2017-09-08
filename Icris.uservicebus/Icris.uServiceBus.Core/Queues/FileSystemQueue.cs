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
using Icris.uServiceBus.Core.Messages;

namespace Icris.uServiceBus.Core.Queues
{
    /// <summary>
    /// A thread-safe, disk-persisted message queue.
    /// </summary>
    /// <typeparam name="T">Json serializable object type stored in this queue.</typeparam>
    public class FileSystemQueue<T>
    {
        object qlock = new object();
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

        private StreamWriter GetNewMessageStream()
        {
            StreamWriter writer;
            while (!TryToOpenFile(this.path + "\\" + Guid.NewGuid().ToString() + ".uq.json", out writer)) ;
            return writer;

        }

        /// <summary>
        /// Un-processed and processing messages in the queue.
        /// </summary>
        public int Count => Directory.EnumerateFiles(this.path, "*.uq.json").Count();

        public bool IsReadOnly => false;

        /// <summary>
        /// Add a message to the queue.
        /// </summary>
        /// <param name="item">The object to wrap in the message.</param>
        public void Add(T item)
        {
            using (var w = GetNewMessageStream())
            {
                w.Write(JObject.FromObject(item).ToString());
                w.Flush();
            }
        }

        /// <summary>
        /// Clears the queue of all its messages (deletes from disk)
        /// </summary>
        public void Clear()
        {
            Directory.EnumerateFiles(this.path, "*.uq.json").ToList().ForEach(x => File.Delete(x));
            Directory.EnumerateFiles(this.path, "*.uq.json.lock").ToList().ForEach(x => File.Delete(x));
        }
  

        /// <summary>
        /// Receive a message from the queue.
        /// </summary>
        /// <returns></returns>
        public IMessage<T> Receive()
        {
            var files = Directory.EnumerateFiles(this.path, "*.uq.json").OrderBy(x => Guid.NewGuid());
            foreach (var file in files)
            {
                //Skip locked messages
                if (File.Exists(file + ".lock") && File.GetCreationTime(file + ".lock") > DateTime.Now.AddSeconds(-300))
                    continue;
                //We'll need locking to assure we're atomically fetching this message and no-one interferes.
                lock (qlock)
                {
                    var message = new FileSystemMessage<T>(file);
                    if (message.IsValid)
                        return message;
                }                
            }
            return null;

        }
    }
}
