// Given code


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkbook
{
	public class Debit : Transaction
	{
		public Debit(DateTime dateTime, string description, string category, decimal
		 amount) : base(dateTime, TransactionType.Debit, description, category, amount)
		{
		}
		override public decimal CalculationAmount { get { return 0 - Amount; } }
	}
}
