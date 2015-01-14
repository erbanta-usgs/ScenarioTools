using System;
using System.Collections.Generic;

using ScenarioTools.DataClasses;
using ScenarioTools.SMDataClasses;

namespace ScenarioTools.Scene
{
    public class SMProject
    // A class to hold all data related to a Scenario Manager project
    {
        #region Fields

        private List<SerializableScenarioElement> _scenarioElements = new List<SerializableScenarioElement>();
        private SerializableTreeNode _treeView = new SerializableTreeNode();
        private string _gridShapefileRelativePath;
        private DateTime _simulationStartTime;
        private SMProjectSettings _smProjectSettings;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public SMProject()
        {
            _gridShapefileRelativePath = "";
            _simulationStartTime = TimeHelper.DefaultDateTime();
            _smProjectSettings = new SMProjectSettings();
        }

        /// <summary>
        /// Constructor with parameters
        /// </summary>
        /// <param name="tagStore">TagStore to be stored</param>
        /// <param name="scenarioElements">List of ITaggable to be stored</param>
        public SMProject(List<SerializableScenarioElement> scenarioElements, 
                         SerializableTreeNode treeView, string gridShapefileRelativePath, 
                         DateTime simulationStartTime, 
                         SMProjectSettings smProjectSettings) : this()
        {
            _gridShapefileRelativePath = gridShapefileRelativePath;
            _simulationStartTime = simulationStartTime;
            foreach (SerializableScenarioElement element in scenarioElements)
            {
                _scenarioElements.Add((SerializableScenarioElement)element);
            }
            _treeView.Tag = treeView.Tag;
            _treeView.Text = treeView.Text;
            _treeView.ImageIndex = treeView.ImageIndex;
            foreach (SerializableTreeNode node in treeView.Items)
            {
                _treeView.Items.Add(node);
            }
            _smProjectSettings = (SMProjectSettings)smProjectSettings.Clone();
        }

        #endregion Constructors

        #region Properties

        public SMProjectSettings SmProjectSettings
        {
            get
            {
                return _smProjectSettings;
            }
            set
            {
                _smProjectSettings = (SMProjectSettings)value.Clone();
            }
        }

        public string GridShapefileRelativePath
        {
            get
            {
                return _gridShapefileRelativePath;
            }
            set
            {
                _gridShapefileRelativePath = value;
            }
        }

        public DateTime SimulationStartTime
        {
            get
            {
                return _simulationStartTime;
            }
            set
            {
                _simulationStartTime = value;
            }
        }

        public List<SerializableScenarioElement> ScenarioElements
        {
            get
            {
                return _scenarioElements;
            }
            set
            {
                _scenarioElements.Clear();
                foreach (SerializableScenarioElement element in value)
                {
                    _scenarioElements.Add((SerializableScenarioElement)element);
                }
            }
        }

        public SerializableTreeNode TreeView
        {
            get
            {
                return _treeView;
            }
            set
            {
                _treeView.Items.Clear();
                foreach (SerializableTreeNode node in value.Items)
                {
                    _treeView.Items.Add((SerializableTreeNode)node);
                }
            }
        }

        #endregion Properties
    }
}
