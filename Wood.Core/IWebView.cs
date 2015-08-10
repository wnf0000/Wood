using System;
using System.Collections.Generic;
using System.Text;

namespace Wood.Core
{
    public interface IWebView
    {
        WoodCore WoodCore { get; }
        void ExecuteScript(string script);
        //void AttachScripts(string script);
        void AttachCoreScripts();
        object GetContext();
    }
}
