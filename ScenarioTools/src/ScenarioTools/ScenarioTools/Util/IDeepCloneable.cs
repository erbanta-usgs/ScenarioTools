using System;
using System.Collections.Generic;
using System.Text;

namespace ScenarioTools.Util
{
    public interface IDeepCloneable
    {
        object DeepClone();
        void AssignParent(object parent);
    }

    /// <summary>
    /// Supports creation of a deep clone
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDeepCloneable<T> : IDeepCloneable
    {
        new T DeepClone();
    }
}
