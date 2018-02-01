using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkbook
{
    public class CategoryList : List<Category>
    {
		private TransactionList _transactionList;

		public CategoryList(TransactionList transactionList)
		{
			_transactionList = transactionList;
			Refresh();
		}

		public  void Refresh()
		{
			// Clear Categories List
			Clear();

			// Add Categories from Transactions
			foreach (Transaction t in _transactionList)
			{
				AddTransaction(t);
			}

			// Sort Categories
			Sort();
		}

		/// <summary>
		/// Add to Category by reviewing Transaction.
		/// (does not actually add a Transaction!)
		/// </summary>
		/// <param name="t"></param>
		public void AddTransaction(Transaction t)
		{
			bool newCategory = true;
			foreach (Category c in this)
			{
				if (c.Title == t.Category)
				{
					c.Amount += t.Amount;
					newCategory = false;
					break;
				}
			}

			if (newCategory) { base.Add(new Category(t.Category, t.Amount)); }
		}
	}
}
