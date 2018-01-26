// Given code


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkbook
{
	public class Deposit : Transaction
	{
		public Deposit(DateTime dateTime, string description, string category, decimal amount) :
		   base(dateTime, TransactionType.Deposit, description, category, amount)
		{
		}
		override public decimal CalculationAmount { get { return Amount; } }
	}
}
