using System;
using System.Collections.Generic;
using System.Text;

namespace ScenarioTools.LogAndErrorProcessing
{
    public interface ILogger
    {
        void Update(string message, int priority);
    }
}
