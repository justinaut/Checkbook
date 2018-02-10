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
using System.Windows.Shapes;

namespace Checkbook
{
	/// <summary>
	/// Interaction logic for EditTransaction.xaml
	/// </summary>
	public partial class EditTransaction : Window
	{
		Transaction _transaction;
		CategoryList _categories;

		public EditTransaction(Transaction transaction, CategoryList categories)
		{
			InitializeComponent();
			_transaction = transaction;
			_categories = categories;
		}

		private void btnOK_Click(object sender, RoutedEventArgs e)
		{
			bool errorEncountered = false;

			DateTime date;
			if (!DateTime.TryParse(tbDate.Text, out date))
			{
				errorEncountered = true;
			}

			decimal amt = 0.0m;
			if (!decimal.TryParse(tbAmount.Text, out amt))
			{
				errorEncountered = true;
			}

			if (errorEncountered)
			{
				MessageBox.Show("Error! Please check your date or amount!");
				return;
			}

			_transaction.Date = date;
			_transaction.Amount = amt;
			_transaction.Description = tbDescription.Text;
			_transaction.Checknum = tbCheckNum.Text;
			_transaction.Category = cbCategory.Text;

			this.DialogResult = true;
			Close();
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
			Close();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			tbId.Text = _transaction.Id.ToString();
			tbType.Text = _transaction.Type.ToString();
			tbCheckNum.Text = _transaction.Checknum;
			tbCheckNum.Visibility = lblCheckNum.Visibility = (_transaction.Type == TransactionType.Check ? Visibility.Visible : Visibility.Hidden);
			tbDescription.Text = _transaction.Description;
			tbDate.Text = _transaction.Date.ToShortDateString();
			tbAmountString.Text = _transaction.AmountString;
			tbAmount.Text = _transaction.Amount.ToString("#.00");

			foreach (Category c in _categories)
			{
				cbCategory.Items.Add(c.Title);
			}
			cbCategory.Text = _transaction.Category;
		}
	}
}
