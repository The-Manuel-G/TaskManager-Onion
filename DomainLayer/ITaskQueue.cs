using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer
{
        public interface ITaskQueue
        {
            // Cambiamos la firma para que reciba un IServiceProvider
            void Enqueue(Func<IServiceProvider, Task> task);
            Task DequeueAsync();
        }
    

}
