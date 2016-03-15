using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System;

namespace System.Runtime.CompilerServices
{
	public class ExtensionAttribute : Attribute { }
}

namespace PDTUtils
{
	public static class Extension
	{
		/// <summary>
		/// http://stackoverflow.com/questions/10279092/how-to-get-child-by-type-in-wpf-container
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="depObj"></param>
		/// <returns></returns>
		public static List<T> GetChildOfType<T>(this DependencyObject depObj)
			where T : DependencyObject
		{
			if (depObj == null) return null;
			var elementList = new List<T>();

			for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
			{
				var child = VisualTreeHelper.GetChild(depObj, i);

				var result = (child as T) ?? null;//GetChildOfType<T>(child);
				if (result != null) //return result;
					elementList.Add(result);
			}
			//			if (elementList.Count > 0)
			//		return null;
			return elementList;
		}

		public static void RemoveAll<T>(this ObservableCollection<T> coll)
		{
			for (var i = coll.Count - 1; i >= 0; i--)
			{
				if (coll[i] != null)
					coll.RemoveAt(i);
			}
		}
	}
}
