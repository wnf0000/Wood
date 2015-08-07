using System;
using System.Collections.Generic;
using System.Text;

namespace Wood.Core
{
   public interface IServiceExecuter
    {
        void Execute(WoodCore core,ServiceArgs args);
        object ExecuteHasReturn(WoodCore core, ServiceArgs args);
    }
}
