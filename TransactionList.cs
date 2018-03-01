// Given code


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
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
			// Rather than use XML, use a database source
			//AddTransactionsFromXml();

			string connectionString = "Data Source=.; Integrated Security=True";

			if (DatabaseExists(connectionString, "Checkbook"))
			{
				ExecuteNonQuery(connectionString, "DROP DATABASE Checkbook;");
			}

			ExecuteNonQuery(connectionString, "CREATE DATABASE Checkbook;");

			connectionString += "; Initial Catalog=Checkbook";

			ExecuteNonQuery(connectionString, @"CREATE TABLE CheckingTransaction (
								TransactionId INT PRIMARY KEY,
								TransactionType INT,
								Category VARCHAR(50),
								TransactionDate DATETIME,
								Description VARCHAR(50),
								Amount MONEY,
								CheckNum VARCHAR(10)
							);");
			ExecuteNonQuery(connectionString, 
				@"INSERT INTO CheckingTransaction (
					TransactionId, 
					TransactionType, 
					Category, 
					TransactionDate,
					Description, 
					Amount, 
					Checknum
				) 
				VALUES 
					( 1, 2,  'Income',  '2014-11-23', 'Pay',    1327.00, ''), 
					( 2, 0,  'Food',    '2014-11-24', 'Food',   62.00,   '2021'),
					( 3, 1,  'Misc',    '2014-11-24', 'ATM',    40.00,   ''), 
					( 4, 0,  'Rent',    '2014-11-25', 'Rent',   713.00,  '2022'),
					( 5, 1,  'Friends', '2014-11-26', 'Dinner', 37.29,   ''), 
					( 6, 1,  'Friends', '2014-11-26', 'Movie',  12.50,   ''), 
					( 7, 2,  'Income',  '2014-11-27', 'Mom',    100.00,  ''), 
					( 8, 0,  'Misc',    '2014-11-28', 'Costco', 15.72, '2024'),
					( 9, 1,  'Gas',     '2014-11-29', 'Gas',    43.83,   ''),
					( 10, 1, 'Food',    '2014-11-30', 'Market', 35.11,   '');"
			);
			LoadTransactionsFromDatabase(connectionString);
		}

		private bool DatabaseExists(string connectionString, string databaseName)
		{
			if (!string.IsNullOrWhiteSpace(databaseName))
			{
				SqlConnection connection = new SqlConnection(connectionString);
				SqlCommand command = new SqlCommand("SELECT COALESCE(DB_ID(@databaseName), -1)", connection);
				command.Parameters.AddWithValue("databaseName", databaseName);
				connection.Open();
				int id = (int)command.ExecuteScalar();
				connection.Close();

				if (id > 0)
				{
					return true;
				}
			}

			return false;
		}

		public void InsertTransaction(string connectionString, Transaction t)
		{
			string insert = "INSERT INTO CheckingTransaction (TransactionId, TransactionType, Category, TransactionDate, Description, Amount, Checknum) ";
			insert += "VALUES (@Id, @Type, @Category, @TransactionDate, @Description, @Amount, @CheckNum)";

			SqlConnection connection = new SqlConnection(connectionString);
			SqlCommand command = new SqlCommand(insert, connection);
			command.Parameters.AddWithValue("Id", t.Id);
			command.Parameters.AddWithValue("Type", t.Type);
			command.Parameters.AddWithValue("Category", t.Category);
			command.Parameters.AddWithValue("TransactionDate", t.Date);
			command.Parameters.AddWithValue("Description", t.Description);
			command.Parameters.AddWithValue("Amount", t.Amount);
			command.Parameters.AddWithValue("CheckNum", t.Checknum);
			try
			{
				connection.Open();
				command.ExecuteNonQuery();
			}
			catch (SqlException se)
			{
				Console.WriteLine($"Exception: " + se.Message);
			}
			finally
			{
				connection.Close();
			}
		}

		public void PrintTransactions(string connectionString)
		{
			string select = "SELECT * FROM CheckingTransaction ORDER BY TransactionId";
			SqlConnection connection = new SqlConnection(connectionString);
			SqlCommand command = new SqlCommand(select, connection);
			try
			{
				connection.Open();
				SqlDataReader reader = command.ExecuteReader();

				while (reader.Read())
				{
					Console.WriteLine($"{(int)reader["TransactionId"]}, {(int)reader["TransactionType"]}, '{(string)reader["Category"]}', '{(DateTime)reader["TransactionDate"]}', '{(string)reader["Description"]}', {(decimal)reader["Amount"]}, {(string)reader["CheckNum"]}");
				}
			}
			catch (SqlException se)
			{
				Console.WriteLine($"Exception: " + se.Message);
			}
			finally
			{
				connection.Close();
			}
		}

		private void ExecuteNonQuery(string connectionString, string sql)
		{
			SqlConnection connection = new SqlConnection(connectionString);
			SqlCommand command = new SqlCommand(sql, connection);
			try
			{
				connection.Open();
				command.ExecuteNonQuery();
			}
			catch (SqlException se)
			{
				Console.WriteLine($"Exception: " + se.Message);
			}
			finally
			{
				connection.Close();
			}
		}

		/// <summary>
		/// Records the state of the given Transaction in given database
		/// </summary>
		/// <param name="connectionString"></param>
		public static bool SaveTransaction(string connectionString, Transaction t)
		{
			if (t == null || string.IsNullOrWhiteSpace(connectionString))
			{
				return false;
			}

			bool updated = true;

			string sql = @"UPDATE CheckingTransaction 
						SET TransactionType = @Type, 
							Category = @Category, 
							TransactionDate = @Date, 
							Description = @Description, 
							Amount = @Amount,	
							Checknum = @Checknum
						WHERE TransactionId = @Id";

			SqlConnection connection = new SqlConnection(connectionString);
			SqlCommand command = new SqlCommand(sql, connection);
			command.Parameters.AddWithValue("Type", t.Type);
			command.Parameters.AddWithValue("Category", t.Category);
			command.Parameters.AddWithValue("Date", t.Date);
			command.Parameters.AddWithValue("Description", t.Description?? "");
			command.Parameters.AddWithValue("Amount", t.Amount);
			command.Parameters.AddWithValue("Checknum", t.Checknum ?? "");
			command.Parameters.AddWithValue("Id", t.Id);

			try
			{
				connection.Open();
				command.ExecuteNonQuery();
			}
			catch (SqlException se)
			{
				Console.WriteLine($"Exception: " + se.Message);
				updated = false;
			}
			finally
			{
				connection.Close();
			}

			return updated;
		}

		private void LoadTransactionsFromDatabase(string connectionString)
		{
			string select = "SELECT * FROM CheckingTransaction ORDER BY TransactionId";
			SqlConnection connection = new SqlConnection(connectionString);
			SqlCommand command = new SqlCommand(select, connection);
			try
			{
				connection.Open();
				SqlDataReader reader = command.ExecuteReader();

				while (reader.Read())
				{
					Transaction t;
					if (0 == (int)reader["TransactionType"])
					{
						t = new Check((DateTime)reader["TransactionDate"], (string)reader["Description"], (string)reader["Category"], (decimal)reader["Amount"], (string)reader["CheckNum"]);
						base.Add(t);
					}
					if (1 == (int)reader["TransactionType"])
					{
						t = new Debit((DateTime)reader["TransactionDate"], (string)reader["Description"], (string)reader["Category"], (decimal)reader["Amount"]);
						base.Add(t);
					}
					if (2 == (int)reader["TransactionType"])
					{
						t = new Deposit((DateTime)reader["TransactionDate"], (string)reader["Description"], (string)reader["Category"], (decimal)reader["Amount"]);
						base.Add(t);
					}
				}
			}
			catch (SqlException se)
			{
				Console.WriteLine($"Exception: " + se.Message);
			}
			finally
			{
				connection.Close();
			}
		}

		#region Transactions_From_XML_Lab
		// Code supporting a lab to use RegEx to read transactions data from XML

		private void AddTransactionsFromXml()
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
		#endregion
	}
}
