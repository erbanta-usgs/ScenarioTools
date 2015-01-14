using System.Collections.Generic;

namespace ScenarioTools.Data_Providers
{
    public interface IHasUniqueIdentifier
    {
        long GetUniqueIdentifier();
        void UpdateUniqueIdentifier();
        void ValidateUniqueIdentifier(List<long> uniqueIdentifiers);
    }
}
