using FileLock;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Icris.uServiceBus.Core.Queues
{
    

    public class FileSystemMessage<T>
    {
        static object creationlock = new object();

        SimpleFileLock fileLock;
        string path;
        public FileSystemMessage(string path)
        {
            lock(creationlock)
            {
                this.path = path;
                this.fileLock = SimpleFileLock.Create(path + ".lock", TimeSpan.FromMinutes(10));
                if (!fileLock.TryAcquireLock())
                {
                    throw new Exception("Unable to aqcuire lock");
                }                
            }
        }
        public void Complete()
        {
            File.Delete(this.path);
            fileLock.ReleaseLock();
        }

        public void Release()
        {
            fileLock.ReleaseLock();
        }

        public T Content()
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
        }
    }
}
