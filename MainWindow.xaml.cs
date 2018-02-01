using System;
using System.Collections.Generic;
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
	public partial class MainWindow : Window
	{
		TransactionList transactionList = new TransactionList();
		CategoryList categoryList;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			lbTransactions.ItemsSource = transactionList;
			lblBalance.Content = transactionList.Balance.ToString("C");
			categoryList = new CategoryList(transactionList);
			lbCategories.ItemsSource = categoryList;
		}

		private void lbTransactions_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (lbTransactions.SelectedIndex < 0) lbTransactions.SelectedIndex = 0;
			int index = lbTransactions.SelectedIndex;
			Transaction tr = transactionList[index];
			tbId.Text = tr.Id.ToString();
			tbType.Text = tr.Type.ToString();
			tbDescription.Text = tr.Description;
			tbDate.Text = tr.Date.ToShortDateString();
			lblAmountString.Content = tr.AmountString;
			tbAmount.Text = tr.Amount.ToString("C");
			tbCategory.Text = tr.Category;
			tbCheckNum.Text = tr.Checknum;
		}
	}
}
