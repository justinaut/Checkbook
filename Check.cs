// Given code


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkbook
{
	public class Check : Transaction
	{
		protected const decimal checkCharge = .10M;
		public Check(DateTime dateTime, string description, string category, decimal
		  amount, String checknum) : base(dateTime, TransactionType.Check,
		  description, category, amount, checknum)
		{
		}
		public override decimal CalculationAmount
		{
			get { return 0 - (Amount + checkCharge); }
		}
	}
}
