using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkbook
{
    public class Category : IComparable<Category>, INotifyPropertyChanged
	{
		private string _title;
		private decimal _amount;

		public Category(string title, decimal amount)
		{
			Title = title;
			Amount = amount;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyChanged(string property)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged.Invoke(this, new PropertyChangedEventArgs(property));
			}
		}

		public string Title {
			get { return _title; }
			set { _title = value; NotifyChanged("Title"); }
		}

		public decimal Amount {
			get { return _amount; }
			set { _amount = value; NotifyChanged("Amount"); }
		}

		public int CompareTo(Category other)
		{
			return string.CompareOrdinal(Title, other.Title);
		}

		public override string ToString()
		{
			return Title + ' ' + Amount.ToString("C");
		}
	}
}
