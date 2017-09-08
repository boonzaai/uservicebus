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

namespace Icris.uServiceBus.Core.Messages
{

    public class FileSystemMessage<T> : IMessage<T>
    {
        string path;
        T content;
        public bool IsValid = true;
        DateTime lockTime = DateTime.Now;

        public FileSystemMessage(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    IsValid = false;
                    return;
                }
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
            if (File.Exists(path + ".lock") &&                                              //The file was locked by *someone*
                File.GetCreationTime(path + ".lock") > DateTime.Now.AddSeconds(-300) &&     //The lock is still valid
                File.GetCreationTime(path + ".lock") != this.lockTime)                      //The lock is not mine
                throw new Exception("Unable to aqcuire lock");
            using (var f = File.CreateText(path + ".lock"))
                this.lockTime = File.GetCreationTime(path + ".lock");
        }
        /// <summary>
        /// Complete this message, remove from queue.
        /// </summary>
        public void Complete()
        {
            if (File.GetCreationTime(this.path + ".lock") <= DateTime.Now.AddSeconds(-300))
            {
                throw new Exception("Lock expired");
            }
            File.Delete(this.path);
            File.Delete(this.path + ".lock");
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
            File.Delete(path + ".lock");
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
