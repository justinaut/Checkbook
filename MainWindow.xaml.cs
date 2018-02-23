using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Checkbook
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		TransactionList _transactionList = new TransactionList();
		CategoryList _categoryList;
		Transaction _selectedtransaction;

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyChanged(string property)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged.Invoke(this, new PropertyChangedEventArgs(property));
			}
		}

		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			lblBalance.Content = _transactionList.Balance.ToString("C");
		}

		private void lbTransactions_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (lbTransactions.SelectedIndex < 0) lbTransactions.SelectedIndex = 0;
			SelectedTransaction = _transactionList[lbTransactions.SelectedIndex];
		}

		private void btnEdit_Click(object sender, RoutedEventArgs e)
		{
			if (lbTransactions.SelectedItem != null)
			{
				EditTransaction editTransactionWindow = new EditTransaction(SelectedTransaction, Categories);
				if (editTransactionWindow.ShowDialog() ?? false)
				{
					Categories.Refresh();
					lblBalance.Content = _transactionList.Balance.ToString("C");
				}
			}
		}

		public TransactionList Transactions
		{
			get { return _transactionList; }
			set { _transactionList = value; }
		}

		public Transaction SelectedTransaction
		{
			get { return _selectedtransaction; }
			set { _selectedtransaction = value; NotifyChanged("SelectedTransaction"); }
		}

		public CategoryList Categories
		{
			get {
				if (_categoryList == null)
				{
					_categoryList = new CategoryList(Transactions);
				}
				return _categoryList;
			}
			set {
				_categoryList = value;
				_categoryList.Refresh();
				NotifyChanged("Categories");
			}
		}
	}
}
