using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ScenarioTools.Data_Providers
{
    public interface IDataProviderMenu
    {
        bool UpdateDataProvider(out string errorMessage);
    }
}
