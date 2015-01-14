using System;
using System.Collections.Generic;

using System.Text;

namespace ScenarioTools.DataClasses
{
    public class TemporalReference
    {
        public enum ModflowTimeUnit
        {
            seconds = 1,
            minutes = 2,
            hours = 3,
            days = 4,
            years = 5
        }

        #region Fields
        private DateTime _simulationStartDateTime;
        private ModflowTimeUnit _modelTimeUnit;
        #endregion Fields

        #region Constructors
        public TemporalReference(TemporalReference temporalReference)
        {
            _simulationStartDateTime = temporalReference.SimulationStartTime;
            _modelTimeUnit = temporalReference.ModelTimeUnit;
        }
        public TemporalReference(DateTime simulationStartDateTime, ModflowTimeUnit modelTimeUnit)
        {
            _simulationStartDateTime = simulationStartDateTime;
            _modelTimeUnit = modelTimeUnit;
        }
        public TemporalReference(DateTime simulationStartDateTime)
        {
            _simulationStartDateTime = simulationStartDateTime;
            _modelTimeUnit = new ModflowTimeUnit();
        }
        public TemporalReference(ModflowTimeUnit modelTimeUnit)
        {
            _simulationStartDateTime = TimeHelper.DefaultDateTime();
            _modelTimeUnit = modelTimeUnit;
        }
        public TemporalReference()
        {
            _simulationStartDateTime = TimeHelper.DefaultDateTime();
            _modelTimeUnit = new ModflowTimeUnit();
        }
        #endregion Constructors

        #region Properties
        public ModflowTimeUnit ModelTimeUnit
        {
            get
            {
                return this._modelTimeUnit;
            }
            set
            {
                this._modelTimeUnit = value;
            }
        }
        public DateTime SimulationStartTime
        {
            get
            {
                return _simulationStartDateTime;
            }
            set
            {
                this._simulationStartDateTime = value;
            }
        }
        #endregion Properties

        #region Public methods
        public DateTime GetDate(double simulationTime)
        {
            DateTime origDateTime = new DateTime(_simulationStartDateTime.Ticks);
            switch (_modelTimeUnit)
            {
                case ModflowTimeUnit.seconds:
                    return origDateTime.AddSeconds(simulationTime);
                case ModflowTimeUnit.minutes:
                    return origDateTime.AddMinutes(simulationTime);
                case ModflowTimeUnit.hours:
                    return origDateTime.AddHours(simulationTime);
                case ModflowTimeUnit.days:
                    return origDateTime.AddDays(simulationTime);
                case ModflowTimeUnit.years:
                    return origDateTime.AddDays(simulationTime * 365.25);
                default:
                    return origDateTime;
            }
        }
        public bool Equals(TemporalReference otherTemporalReference)
        {
            if (otherTemporalReference.ModelTimeUnit != ModelTimeUnit)
            {
                return false;
            }
            if (otherTemporalReference.SimulationStartTime.Ticks != SimulationStartTime.Ticks)
            {
                return false;
            }
            return true;
        }
        #endregion Public methods
    }
}
