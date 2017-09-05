using FileLock;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Icris.uServiceBus.Core.Queues
{


    public class FileSystemMessage<T>
    {
        static object creationlock = new object();

        SimpleFileLock fileLock;
        string path;
        int timeout;
        DateTime expiration = DateTime.Now;
        T content;
        public bool IsValid = true;
        public FileSystemMessage(string path, int timeout)
        {
            try
            {
                this.timeout = timeout;
                this.path = path;
                Lock();
                Thread.Sleep(1);
                if (!File.Exists(path))
                {
                    IsValid = false;
                    return;
                }
                content = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            }
            catch (Exception e)
            {
                //throw new Exception("Unable to lock the message");
                IsValid = false;
            }
        }
        private void Lock()
        {
            if (File.Exists(path + ".lock") && File.GetCreationTime(path + ".lock") > DateTime.Now.AddSeconds(-timeout))
                throw new Exception("Unable to aqcuire lock");
            this.fileLock = SimpleFileLock.Create(path + ".lock", TimeSpan.FromSeconds(timeout));
            this.expiration = DateTime.Now.AddSeconds(timeout);
        }
        /// <summary>
        /// Complete this message, remove from queue.
        /// </summary>
        public void Complete()
        {
            if (DateTime.Now >= expiration)
            {
                throw new Exception("Lock expired");
            }
            File.Delete(this.path);
            fileLock.ReleaseLock();
        }
        /// <summary>
        /// Renew the lock on this message for the specified timeout
        /// </summary>
        public void RenewLock()
        {
            Lock();
        }
        /// <summary>
        /// Release this message making it available to new Receive() calls.
        /// </summary>
        public void Release()
        {
            fileLock.ReleaseLock();
        }
        /// <summary>
        /// Retreive the contained object in this message.
        /// </summary>
        /// <returns></returns>
        public T Content()
        {
            return content;
        }
    }
}
