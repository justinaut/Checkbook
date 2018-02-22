using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Checkbook
{
    public class CategoryList : ObservableCollection<Category>
    {
		private TransactionList _transactionList;

		public CategoryList(TransactionList transactionList)
		{
			_transactionList = transactionList;
			Refresh();
		}

		public  void Refresh()
		{
			Clear();
			foreach (Transaction t in _transactionList)
			{
				AddTransaction(t);
			}
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
