// Given code


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkbook
{
	public enum TransactionType { Check, Debit, Deposit }

	abstract public class Transaction
	{
		private int id; // Transaction ID
		private TransactionType type; // Type of transaction
		private string category; // Category (Budget)
		private DateTime date; // Date of transaction
		private string description; // Paid to, taken from...
		private decimal amount; // The amount (always >0)
		private string checknum; // Check # if appropriate

		public int Id
		{
			get { return id; }
			private set { id = value; }
		}
		public TransactionType Type
		{
			get { return type; }
			set { type = value; }
		}
		public string Category
		{
			get { return category; }
			set { category = value; }
		}
		public DateTime Date
		{
			get { return date; }
			set { date = value; }
		}
		public string Description
		{
			get { return description; }
			set { description = value; }
		}
		public decimal Amount
		{
			get { return amount; }
			set { amount = value; }
		}
		public string Checknum
		{
			get { return checknum; }
			set { checknum = value; }
		}

		private static int lastId = 0;
		int nextId() { return ++lastId; }

		public override string ToString()
		{
			return Date.ToShortDateString() + "\t" + Type.ToString() + "\t" + Amount.ToString("C");
		}

		public String TransactionSummary { get { return ToString(); } }

		public Transaction(DateTime dateTime, TransactionType type, string
		  description, string category, decimal amount, string checknum = "")
		{
			id = nextId();
			Date = dateTime;
			Type = type;
			Description = description;
			Category = category;
			Amount = amount;
			Checknum = checknum;
		}

		public abstract decimal CalculationAmount { get; }
	}
}
