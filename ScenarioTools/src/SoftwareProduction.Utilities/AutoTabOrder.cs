using System;
using System.Windows.Forms;
using System.Collections;

namespace SoftwareProductions.Utilities
{
	/// <summary>
	/// Utility class to arrange the tab orders of the controls on a form.
	/// </summary>
	public sealed class AutoTabOrder
	{
		private AutoTabOrder()
		{
			
		}

		/// <summary>
		/// Arranges the Tab orders from left to right and top to bottom for the child controls
		/// on the specified parent control for form.
		/// </summary>
		/// <param name="parent"></param>
		public static void AutoCorrectTabOrders(Control parent) 
		{
			ArrayList controls = new ArrayList();

			foreach (Control child in parent.Controls) 
			{
				if (child is TabPage) 
				{
					AutoCorrectTabOrders(child);
				} 
				else if (! (child is Label)) 
				{
					controls.Add(child);

					if (child.Controls.Count > 0) 
					{
						AutoCorrectTabOrders(child);
					}
				}
			}

			controls.Sort(new TabOrderComparer());

			for (int i = 0; i < controls.Count; i++) 
			{
				((Control)controls[i]).TabIndex = i;
			}
		}

		private class TabOrderComparer : IComparer
		{
			public int Compare(object x, object y) 
			{
				Control cx = (Control)x; 
				Control cy = (Control)y; 

				int result = cx.Top.CompareTo(cy.Top);

				if (result != 0 && Math.Abs(cx.Top - cy.Top) <= 15) 
				{
					result = 0;
				}

				if (result == 0) 
				{
					result = cx.Left.CompareTo(cy.Left);
				}

				return result;
			}
		}

	}
}
