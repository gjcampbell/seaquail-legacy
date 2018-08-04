using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeaQuail;
using SeaQuail_SQLServer;
using SeaQuail_MySQL;
using SeaQuail.Data;
using SeaQuail.Schema;
using System.Data;

namespace SeaQuail_Example
{
    class Program
    {
        static void Main(string[] args)
        {
            // get an adapter
            SQAdapter adp = !string.IsNullOrEmpty(Settings.Default.SQLServerConn) ? (SQAdapter)new SQLServerAdapter(Settings.Default.SQLServerConn)
                : !string.IsNullOrEmpty(Settings.Default.MySQLConn) ? (SQAdapter)new MySQLAdapter(Settings.Default.MySQLConn)
                : null;

            if (adp == null)
            {
                return;
            }


            // Create Table 1
            // -------------------------------------------------------
            Console.WriteLine("Creating table:  FamousQuote");
            Console.WriteLine("ID               Int64");
            Console.WriteLine("FamousPersonID   Int64");
            Console.WriteLine("Quote            String(500)");
            SQTable quote = new SQTable()
            {
                Name = "FamousQuote",
                Columns = new SQColumnList()
                {
                    new SQColumn(){ Name = "ID", DataType = SQDataTypes.Int64, IsPrimary = true, IsIdentity = true },
                    new SQColumn(){ Name = "FamousPersonID", DataType = SQDataTypes.Int64, Nullable = false },
                    new SQColumn(){ Name = "Quote", DataType = SQDataTypes.String, Length = 500 }
                }
            };

            // delete table if it already exists
            if (adp.GetTable(quote.Name) != null)
            {
                Console.WriteLine("Table '" + quote.Name + "' exists. Deleting...");
                adp.RemoveTable(quote.Name);
            }

            adp.CreateTable(quote);
            Console.WriteLine("Table Created. ");






            // Create Table 2
            // -------------------------------------------------------
            Console.WriteLine();
            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine("Creating table:  FamousPerson");
            Console.WriteLine("ID               Int64");
            Console.WriteLine("FirstName        String(50)");
            Console.WriteLine("LastName         String(50)");


            SQTable person = new SQTable()
            {
                Name = "FamousPerson",
                Columns = new SQColumnList()
                {
                    new SQColumn(){ Name = "ID", DataType = SQDataTypes.Int64, IsPrimary = true, IsIdentity = true },
                    new SQColumn(){ Name = "FirstName", DataType = SQDataTypes.String, Length = 50 },
                    new SQColumn(){ Name = "LastName", DataType = SQDataTypes.String, Length = 50 }
                }
            };

            // delete table if it already exists
            if (adp.GetTable(person.Name) != null)
            {
                Console.WriteLine("Table '" + person.Name + "' exists. Deleting...");
                adp.RemoveTable(person.Name);
            }

            adp.CreateTable(person);
            Console.WriteLine("Table Created. ");








            // add a foreign key
            // -------------------------------------------------------
            Console.WriteLine();
            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine("Adding a foreign key.");
            Console.WriteLine("FamousQuote.FamousPersonID -> FamousPerson.ID");


            adp.AddForeignKey(quote.GetColumnByName("FamousPersonID"), person.GetColumnByName("ID"));
            






            // make a couple of insert query objects
            // -------------------------------------------------------
            string varFirstName = adp.CreateVariable("FirstName");
            string varLastName = adp.CreateVariable("LastName");
            SQInsertQuery personInsert = new SQInsertQuery()
            {
                Table = new SQAliasableObject(person.Name),
                ReturnID = true,
                SetPairs = new List<SQSetQueryPair>()
                {            
                    new SQSetQueryPair("FirstName", varFirstName),
                    new SQSetQueryPair("LastName", varLastName),
                }
            };

            string varPersonID = adp.CreateVariable("PersonID");
            string varQuote = adp.CreateVariable("Quote");
            SQInsertQuery quoteInsert = new SQInsertQuery()
            {
                Table = new SQAliasableObject(quote.Name),
                ReturnID = false,
                SetPairs = new List<SQSetQueryPair>()
                {            
                    new SQSetQueryPair("FamousPersonID", varPersonID),
                    new SQSetQueryPair("Quote", varQuote),
                }
            };






            // add some data using insert objects
            // -------------------------------------------------------
            Console.WriteLine();
            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine("Inserting Data");
            Console.WriteLine("Inserting Person. ");


            personInsert.Parameters = new List<SQParameter>() 
            { 
                new SQParameter(varFirstName, "I"), 
                new SQParameter(varLastName, "Asimov") 
            };
            Int64 id = personInsert.ExecuteReturnID<Int64>(adp);

            Console.WriteLine("Inserting Quote. ");
            quoteInsert.Parameters = new List<SQParameter>()
            {
                new SQParameter(varPersonID, id),
                new SQParameter(varQuote, "Never let your sense of morals get in the way of doing what's right. ")
            };
            quoteInsert.Execute(adp);

            Console.WriteLine("Inserting Quote. ");
            quoteInsert.Parameters = new List<SQParameter>()
            {
                new SQParameter(varPersonID, id),
                new SQParameter(varQuote, "I do not fear computers. I fear the lack of them. ")
            };
            quoteInsert.Execute(adp);








            // update data
            // -------------------------------------------------------
            new SQUpdateQuery()
            {
                UpdateTable = new SQAliasableObject(person.Name),
                SetPairs = new List<SQSetQueryPair>()
                {
                    new SQSetQueryPair("FirstName", varFirstName)
                },
                Condition = new SQCondition("LastName", SQRelationOperators.Equal, varLastName),
                Parameters = new List<SQParameter>()
                {
                    new SQParameter(varFirstName, "Isaac"),
                    new SQParameter(varLastName, "Asimov")
                }
            }.Execute(adp);








            // Select data
            // -------------------------------------------------------
            Console.WriteLine();
            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine("Selecting quotes by Asimov. ");
            SQSelectQuery select = new SQSelectQuery()
            {
                From = new SQFromClause(new SQFromTable(quote.Name, "q")
                {
                    Join = new SQJoin(person.Name, "p")
                    {
                        JoinType = SQJoinTypes.Inner,
                        Predicate = new SQCondition("p.ID", SQRelationOperators.Equal, "q.FamousPersonID")
                    }
                }),
                Columns = new List<SQAliasableObject>
                { 
                    new SQAliasableObject("q.Quote") 
                },
                Condition = new SQCondition("p.LastName", SQRelationOperators.Like, varLastName),
                Parameters = new List<SQParameter>
                {
                    new SQParameter(varLastName, "Asimov")
                }
            };
            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine("SQL: ");
            Console.Write(select.Write(adp));
            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine("Result: ");


            // get datatable instead of reader. this closes the reader
            // automatically
            foreach (DataRow row in select.Execute(adp).GetDataTable().Rows)
            {
                Console.WriteLine(row[0]);
            }







            // Select data joinless
            // -------------------------------------------------------
            Console.WriteLine();
            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine("Selecting quotes by Asimov. Joinless. ");
            select = new SQSelectQuery()
            {
                From = new SQFromClause(
                    new SQFromTable(quote.Name, "q"),
                    new SQFromTable(person.Name, "p")
                ),
                Columns = new List<SQAliasableObject>
                { 
                    new SQAliasableObject("q.Quote") 
                },
                Condition = new SQCondition("p.ID", SQRelationOperators.Equal, "q.FamousPersonID")
                    .And("p.LastName", SQRelationOperators.Like, varLastName),
                Parameters = new List<SQParameter>
                {
                    new SQParameter(varLastName, "Asimov")
                }
            };
            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine("SQL: ");
            Console.Write(select.Write(adp));
            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine("Result: ");


            SQSelectResult res = select.Execute(adp);
            while (res.Reader.Read())
            {
                Console.WriteLine(res.Reader.GetValue(0));
            }
            res.Close();







            // Delete data
            // -------------------------------------------------------
            new SQDeleteQuery()
            {
                DeleteTable = new SQAliasableObject(quote.Name),
                Join = new SQJoin(person.Name)
                {
                    JoinType = SQJoinTypes.Inner,
                    Predicate = new SQCondition("FamousPersonID", SQRelationOperators.Equal, "FamousPerson.ID")
                },
                Condition = new SQCondition("LastName", SQRelationOperators.Equal, varLastName),
                Parameters = new List<SQParameter>
                {
                    new SQParameter(varLastName, "Asimov")
                }
            }.Execute(adp);

            
            
            
            // Remove created tables
            // -------------------------------------------------------
            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine("Removing Table '" + quote.Name + "'");
            adp.RemoveTable(quote.Name);
            Console.WriteLine("Removing Table '" + person.Name + "'");
            adp.RemoveTable(person.Name);
            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine("Press any key to exit.");

            Console.ReadKey();
        }
    }
}
