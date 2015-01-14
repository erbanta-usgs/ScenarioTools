using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

using ScenarioTools.Util;

namespace ScenarioTools.Data_Providers
{
    public interface IDataProvider : IHasUniqueIdentifier, IDeepCloneable
    {

        #region Properties
        long Key { get; }
        DateTime DataModificationTime { get; set; }
        bool ConvertFlowToFlux { get; set; }
        # endregion Properties

        #region Methods
        bool SupportsDataConsumerType(DataConsumerTypeEnum dataConsumerType);
        int GetDataStatus();
        object GetData(out int dataStatus);
        object GetDataSynchronous();
        string[] GetMetadata();
        string[] GetResultMessage();
        void InitFromXml(XmlElement element, string sourceFileName);
        void InvalidateDataset();
        void LoadCacheFromStream(Stream stream, DateTime streamDateTime);
        XmlNode GetXmlNode(XmlDocument document, string targetFileName);
        #endregion Methods
    }
}