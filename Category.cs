using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkbook
{
    public class Category : IComparable<Category>
    {
		public Category(string title, decimal amount)
		{
			Title = title;
			Amount = amount;
		}

		public string Title { get; set; }

		public decimal Amount { get; set; }

		public int CompareTo(Category other)
		{
			return this.Title.CompareTo(other.Title);
		}

		public override string ToString()
		{
			return Title + ' ' + Amount.ToString("C");
		}
	}
}
