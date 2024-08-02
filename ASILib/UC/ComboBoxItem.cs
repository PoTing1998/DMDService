using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Lib.UC
{
	public class ComboBoxItem
	{
		public ComboBoxItem(string text, object value)
		{
			Text = text;
			Value = value;
		}
		
		public string Text
		{
			get;
			set;
		}

		public object Value
		{
			get;
			set;
		}

		public override string ToString()
		{
			return Text;
		}
	}
}
