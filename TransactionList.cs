// Given code


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Checkbook
{
	public class TransactionList : ObservableCollection<Transaction>
	{
		public TransactionList()
		{
			AddTransactions();
		}

		public decimal Balance
		{
			get
			{
				decimal bal = 0;
				foreach (Transaction t in this)
				{
					bal += t.CalculationAmount;
				}
				return bal;
			}
		}

		private void AddTransactions()
		{
			Regex TransactionPattern = new Regex(@"<Transaction>(.*?)<\/Transaction>");
			var matches = TransactionPattern.Matches(GetXml());

			foreach (Match match in matches)
			{
				string Id = ExtractContentByTag(match.ToString(), "Id");
				DateTime Date = DateTime.Parse(ExtractContentByTag(match.ToString(), "Date"));
				string Type = ExtractContentByTag(match.ToString(), "Type");
				string Description = ExtractContentByTag(match.ToString(), "Description");
				string Category = ExtractContentByTag(match.ToString(), "Category");
				decimal Amount = decimal.Parse(ExtractContentByTag(match.ToString(), "Amount"));
				string CheckNum = ExtractContentByTag(match.ToString(), "Checknum");

				Transaction t;
				if (Type.Equals("Debit", StringComparison.InvariantCultureIgnoreCase))
				{
					t = new Debit(Date, Description, Category, Amount);
					base.Add(t);
				}
				if (Type.Equals("Deposit", StringComparison.InvariantCultureIgnoreCase))
				{
					t = new Deposit(Date, Description, Category, Amount);
					base.Add(t);
				}
				if (Type.Equals("Check", StringComparison.InvariantCultureIgnoreCase))
				{
					t = new Check(Date, Description, Category, Amount, CheckNum);
					base.Add(t);
				}
			}
		}

		private static string GetXml()
		{
			return @"<Transactions><Transaction><Id>1</Id><Date>11/23/2014</Date><Type>Deposit</Type><Description>Pay</Description><Category>Income</Category><Amount>1327</Amount></Transaction><Transaction><Id>2</Id><Date>11/24/2014</Date><Type>Check</Type><Description>Food</Description><Category>Food</Category><Amount>62</Amount><Checknum>2021</Checknum></Transaction><Transaction><Id>3</Id><Date>11/24/2014</Date><Type>Debit</Type><Description>ATM</Description><Category>Misc</Category><Amount>40</Amount></Transaction><Transaction><Id>4</Id><Date>11/25/2014</Date><Type>Check</Type><Description>Rent</Description><Category>Rent</Category><Amount>713</Amount><Checknum>2022</Checknum></Transaction><Transaction><Id>5</Id><Date>11/26/2014</Date><Type>Debit</Type><Description>Dinner</Description><Category>Friends</Category><Amount>37.29</Amount></Transaction><Transaction><Id>6</Id><Date>11/26/2014</Date><Type>Debit</Type><Description>Movie</Description><Category>Friends</Category><Amount>12.50</Amount></Transaction><Transaction><Id>7</Id><Date>11/27/2014</Date><Type>Deposit</Type><Description>Mom</Description><Category>Income</Category><Amount>100</Amount></Transaction><Transaction><Id>8</Id><Date>11/28/2014</Date><Type>Check</Type><Description>Costco</Description><Category>Misc</Category><Amount>15.72</Amount><Checknum>2024</Checknum></Transaction><Transaction><Id>9</Id><Date>11/29/2014</Date><Type>Debit</Type><Description>Gas</Description><Category>Gas</Category><Amount>43.83</Amount></Transaction><Transaction><Id>10</Id><Date>11/30/2014</Date><Type>Debit</Type><Description>Market</Description><Category>Food</Category><Amount>35.11</Amount></Transaction></Transactions>";
		}

		private static string ExtractContentByTag(string content, string tag)
		{
			Regex pattern = new Regex($"<{tag}>(.*?)<\\/{tag}>");
			Match match = pattern.Match(content.ToString());
			return match.Groups[1].ToString();
		}
	}
}
