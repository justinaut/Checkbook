﻿// Given code


using System;
using System.ComponentModel;
using System.Text;

namespace Checkbook
{
	public enum TransactionType { Check, Debit, Deposit }

	abstract public class Transaction : INotifyPropertyChanged
	{
		private int _id; // Transaction ID
		private TransactionType _type; // Type of transaction
		private string _category; // Category (Budget)
		private DateTime _date; // Date of transaction
		private string _description; // Paid to, taken from...
		private decimal _amount; // The amount (always >0)
		private string _checknum; // Check # if appropriate

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyChanged(string property)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged.Invoke(this, new PropertyChangedEventArgs(property));
			}
		}

		public int Id
		{
			get { return _id; }
			// set for Id was private but it interfered with the binding mode
			private set { _id = value; NotifyChanged("Id"); }
		}
		public TransactionType Type
		{
			get { return _type; }
			set { _type = value; NotifyChanged("Type"); NotifyChanged("TransactionSummary"); }
		}
		public string Category
		{
			get { return _category; }
			set { _category = value; NotifyChanged("Category"); }
		}
		public DateTime Date
		{
			get { return _date; }
			set { _date = value; NotifyChanged("Date"); NotifyChanged("TransactionSummary"); }
		}
		public string Description
		{
			get { return _description; }
			set { _description = value; NotifyChanged("Description"); }
		}
		public decimal Amount
		{
			get { return _amount; }
			set {
				_amount = value;
				NotifyChanged("Amount");
				NotifyChanged("AmountString");
				NotifyChanged("CalculationAmount");
				NotifyChanged("TransactionSummary");
			}
		}
		public string Checknum
		{
			get { return _checknum; }
			set { _checknum = value; NotifyChanged("Checknum"); }
		}

		private static int LastId = 0;
		int NextId() { return ++LastId; }

		public override string ToString()
		{
			return Date.ToShortDateString() + "\t" + Type.ToString() + "\t" + Amount.ToString("C");
		}

		public String TransactionSummary { get { return ToString(); } }

		public Transaction(DateTime dateTime, TransactionType type, string
		  description, string category, decimal amount, string checknum = "")
		{
			_id = NextId();
			Date = dateTime;
			Type = type;
			Description = description;
			Category = category;
			Amount = amount;
			Checknum = checknum;
		}

		public abstract decimal CalculationAmount { get; }

		// Property creates and returns the amount string using the functions below
		public string AmountString
		{
			get
			{
				StringBuilder strAmt = new StringBuilder();
				int dollars = (int)Amount;
				int cents = (int)(Amount * 100 % 100);
				strAmt.Append(AmtToString(dollars) + " and " + AmtToString(cents, true) + "/100s Dollars");
				strAmt[0] = char.ToUpper(strAmt[0]);
				return strAmt.ToString();
			}
		}

		// Return dollars in text ("Three hundred seventy two"), or if useDigits, return "372" 
		private string AmtToString(int amt, bool useDigits = false)
		{
			if (amt == 0) return (useDigits ? "00" : "zero");     // If amount is zero, no need to go further
			if (useDigits) return amt.ToString();                 // If using digits return just the digits

			StringBuilder strAmt = new StringBuilder();           // The full string goes in here
			int[] groups = { 1000000000, 1000000, 1000, 1 };       // Our words repeat in 3s (thousand, million)

			foreach (int grp in groups)                            // For each sub-group of 3
			{
				if (amt < grp) continue;                            // If nothing in this sub-group, skip it
				int v = amt / grp;                                  // Separate the 0-999 portion of grouping
				hundredsToString(v, strAmt, grp);                   // Append the current grouping
				amt = amt % grp;                                    // Remove value we're printing from left to do
			}
			return strAmt.ToString();                             // Return the stringified amount
		}

		// Covert a "hundreds" group into text
		// Our number system repeats every 3 digits (thousands, millions, billions, etc)
		// This processes one of those 3 digit groups.
		// params: amt = The amount to print. Must be 0-999
		//         strAmt = The StringBuilder object in which to write the text
		//         group = The grouping (1,1000,1000000,1000000000)
		private void hundredsToString(int amt, StringBuilder strAmt, int grouping)
		{
			String[] label = { "", "thousand", "million", "billion" };  // The grouping labels
			int group = 0;                        // Determine which group to use...
			if (grouping >= 1000) group++;
			if (grouping >= 1000000) group++;
			if (grouping >= 1000000000) group++;

			if (amt >= 100)                       // Do we need a "hundreds" value for this number
			{
				int h = amt / 100;                  // Yes. Strip out the hundreds from the rest of the nbr
				NumberToString(h, strAmt);           // Add the hundreds value
				AppendWithSpace(strAmt, "hundred");
				amt = amt % 100;                    // Remove what we've already printed
			}
			NumberToString(amt, strAmt);          // Print the remaining value along with the grouping
			AppendWithSpace(strAmt, label[group]);
			return;
		}

		// Convert a number (0-999) to a string.
		// Params: amt = The number to convert
		//         strAmt = The StringBuilder where the string value should be added
		private void NumberToString(int amt, StringBuilder strAmt)
		{
			String[] nums = { "", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
			String[] tens = { "", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

			if (amt < 20)       // We have special cases for numbers under twenty
			{
				AppendWithSpace(strAmt, nums[amt]);
			}
			else          // Otherwise use the tens grouping and the remaining number (like sixty five)
			{
				AppendWithSpace(strAmt, tens[amt / 10]);
				AppendWithSpace(strAmt, nums[amt % 10]);
			}
		}

		// Append a string to the StringBuilder, adding a space first if there's any existing text
		private void AppendWithSpace(StringBuilder sb, String s)
		{
			if (s.Length == 0) return;              // If no text to append, return
			if (sb.Length > 0) sb.Append(" ");      // If there's any existing text, add a space
			sb.Append(s);                           // Append the text to the stringBuilder
		}
	}
}
