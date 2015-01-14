using System;
using System.Reflection;

namespace SoftwareProductions.Utilities.Undo
{
	/// <summary>
	/// PropertyChangeUndoItem records changes to objects' properties,
	/// and stores the original value so that change can be reversed.
	/// </summary>
	public class PropertyChangeUndoItem : UndoItemBase
	{

		private object _oldValue;
		private object _newValue;
		private object _object;
		private string _propertyName;

		/// <summary>
		/// Initializes a new instance of the PropertyChangeUndoItem class.
		/// </summary>
		public PropertyChangeUndoItem() 
		{

		}

		/// <summary>
		///  Initializes a new instance of the PropertyChangeUndoItem class with the 
		///  specified values for Item, OldValue, NewValue, and PropertyName.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="oldValue"></param>
		/// <param name="newValue"></param>
		/// <param name="propertyName"></param>
		public PropertyChangeUndoItem(object item, object oldValue, object newValue, string propertyName)
		{
			_object = item;
			_oldValue = oldValue;
			_newValue = newValue;
			_propertyName = propertyName;
		}

		/// <summary>
		/// Gets or sets the object reference that was modified 
		/// by the change that is recorded by this UndoItem.
		/// </summary>
		public object Item 
		{
			get { return _object; }
			set { _object = value; }
		}

		/// <summary>
		/// Gets or sets the original value of the property before it was modified.
		/// </summary>
		public object OldValue 
		{
			get { return _oldValue; }
			set { _oldValue = value; }
		}

		/// <summary>
		/// Gets or sets the value that the property was changed to.
		/// </summary>
		public object NewValue 
		{
			get { return _newValue; }
			set { _newValue = value; }
		}

		/// <summary>
		/// Gets or sets the name of the Property whose modification 
		/// is recorded in this UndoItem.
		/// </summary>
		public string PropertyName 
		{
			get { return _propertyName; }
			set { _propertyName = value; }
		}

		/// <summary>
		/// Reverses the modification that this UndoItem represents.
		/// </summary>
		public override void Undo()
		{

			if (_object == null) 
			{
				return;		//<--- Early Exit
			}

			BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

			PropertyInfo propertyInfo = _object.GetType().GetProperty(_propertyName, flags);

			if (propertyInfo != null) 
			{
				if (propertyInfo.CanWrite) 
				{
					propertyInfo.SetValue(_object, _oldValue, null);
				}
			}
			else 
			{
				FieldInfo fieldInfo = _object.GetType().GetField(_propertyName, flags);
				fieldInfo.SetValue(_object, _oldValue);
			}
		}

		/// <summary>
		/// Re-performs the previously undone modification that this UndoItem represents.
		/// </summary>
		public override void Redo()
		{
			if (_object == null) 
			{
				return;		//<--- Early Exit
			}

			PropertyInfo propertyInfo = _object.GetType().GetProperty(_propertyName);

			if (propertyInfo != null) 
			{
				if (propertyInfo.CanWrite) 
				{
					propertyInfo.SetValue(_object, _newValue, null);
				}
			}
			else 
			{
				FieldInfo fieldInfo = _object.GetType().GetField(_propertyName);
				fieldInfo.SetValue(_object, _newValue);
			}
		}

        /// <summary>
        /// Gives the undo item a chance to cancel itself being added to the undo chain.
        /// This could be used for example if the original value and the new value are identical,
        /// or to merge the changes represented by this undo item into the previous one, if 
        /// the same object is being modified.
        /// </summary>
        /// <param name="previousUndoItem">The undo item currently at the top of the undo chain before this item is added.</param>
        /// <returns>A value indicating if this undo item should be added to the undo chain.</returns>
        public override bool CheckValues(IUndoItem previousUndoItem) 
        {
            if ((this.NewValue == null && this.OldValue == null) ||
                (this.NewValue != null && this.NewValue.Equals(this.OldValue)))
            {
                return false;
            }

            PropertyChangeUndoItem lastItem = previousUndoItem as PropertyChangeUndoItem;

            if (lastItem != null && lastItem.Item == this.Item && lastItem.PropertyName == this.PropertyName)
            {
                if (this.NewValue != null && TypeChecker.TypeIsAtomic(this.NewValue.GetType()))
                {
                    if (this.NewValue != null && lastItem.NewValue != null &&
                        this.NewValue.GetType() == lastItem.NewValue.GetType())
                    {
                        lastItem.NewValue = this.NewValue;
                        return false;
                    }
                }
            }

            return true;
        }
	}
}
