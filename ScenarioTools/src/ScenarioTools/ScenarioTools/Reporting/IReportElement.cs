using System.Collections.Generic;
using System.Xml;

using ScenarioTools.ImageProvider;
using ScenarioTools.Util;

namespace ScenarioTools.Reporting
{
    public interface IReportElement : IImageProvider, IDeepCloneable<IReportElement>
    {
        string Name
        {
            get;
            set;
        }
        int NumDataSeries
        {
            get;
        }
        void AddDataSeries(DataSeries dataSeries);
        void RemoveDataSeries(DataSeries dataSeries);

        DataSeries GetDataSeries(int index);
        object GetDataSeries(string name);

        void ClearDataSeries();

        XmlNode GetXmlNode(XmlDocument xmlDocument, string targetFileName);
        void SaveXMLFile(string file);

        void InitFromXML(System.Xml.XmlElement xmlElement, string sourceFileName);

        void ValidateDataProviderKeys(List<long> uniqueIdentifiers);

        void UpdateCaches();

        bool IsUniqueName(string newName, DataSeries dataSeries);
    }
}
