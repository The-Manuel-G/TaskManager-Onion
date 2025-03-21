﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer
{
        public interface ITaskQueue
        {
            void Enqueue(Func<IServiceProvider, Task> task);
            Task DequeueAsync();
        }
    

}
