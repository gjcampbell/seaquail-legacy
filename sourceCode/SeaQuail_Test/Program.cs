using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.FileIO;
using NLog;
using SeaQuail;
using SeaQuail_SQLServer;
using SeaQuail_MySQL;
using SeaQuail.Schema;
using SeaQuail.Data;
using SeaQuail_SQLite;
using System.Data;

namespace SeaQuail_Test
{
    class Program
    {
        static Logger Log = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            // Get all data out of the vehicles file
            Log.Info("Parsing import data...");
            TextFieldParser txt = new TextFieldParser(AppDomain.CurrentDomain.BaseDirectory + "\\Test Data\\veh.psv");
            txt.Delimiters = new string[] { "|" };
            List<string[]> vehicleData = new List<string[]>();
            txt.ReadFields();
            while (!txt.EndOfData)
            {
                vehicleData.Add(txt.ReadFields());
            }
            Log.Info("Finished parsing import data.");

            Log.Info("------------------------------------------------------");
            Log.Info("Begin Testing Adapters");
            Log.Info("------------------------------------------------------");

            try
            {
                SQAdapter[] adapters = new SQAdapter[] 
                { 
                    new SQLiteAdapter(Settings.Default.SQLiteConn),
                    new SQLServerAdapter(Settings.Default.SQLServerConn),
                    new MySQLAdapter(Settings.Default.MySQLConn)
                };

                foreach (SQAdapter adp in adapters)
                {
                    try
                    {
                        Log.Info("------------------------------------------------------");
                        Log.Info("Testing adapter type: " + adp.GetType().Name);
                        Log.Info("------------------------------------------------------");

                        TestAdapter(adp, vehicleData);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("An error occurred while testing adapter " + adp.GetType().Name + "\r\n" + ex);
                    }
                    Log.Info("------------------------------------------------------");
                    Log.Info("Finished testing adapter: " + adp.GetType().Name);
                    Log.Info("------------------------------------------------------");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            Log.Info("------------------------------------------------------");
            Log.Info("Adapter Testing Complete");
            Log.Info("------------------------------------------------------");
        }

        static void TestAdapter(SQAdapter adp, List<string[]> data)
        {
            SQTable tMake, tModel, tBodyType, tTrans, tVehicle;

            #region Create Tables
            SQTable[] tables = new SQTable[]
            {
                tMake = new SQTable()
                {
                    Name = "Make",
                    Columns = new SQColumnList()
                    {
                        new SQColumn() { Name = "MK_ID", DataType = SQDataTypes.Int64, IsPrimary = true },
                        new SQColumn() { Name = "MK_Name", DataType = SQDataTypes.String, Length = 250 }
                    }
                },
                tModel = new SQTable()
                {
                    Name = "Model",
                    Columns = new SQColumnList()
                    {
                        new SQColumn() { Name = "MD_ID", DataType = SQDataTypes.Int64, IsPrimary = true },
                        new SQColumn() { Name = "MD_MKID", DataType = SQDataTypes.Int64 },
                        new SQColumn() { Name = "MD_Name", DataType = SQDataTypes.String, Length = 250 }
                    }
                },
                tBodyType = new SQTable()
                {
                    Name = "BodyType",
                    Columns = new SQColumnList()
                    {
                        new SQColumn() { Name = "BT_ID", DataType = SQDataTypes.String, Length = 20, IsPrimary = true },
                        new SQColumn() { Name = "BT_Name", DataType = SQDataTypes.String, Length = 250 },
                        new SQColumn() { Name = "BT_Dummy", DataType = SQDataTypes.String, Length = 36 }
                    }
                },
                tTrans = new SQTable()
                {
                    Name = "TransmissionType",
                    Columns = new SQColumnList()
                    {
                        new SQColumn() { Name = "TR_ID", DataType = SQDataTypes.String, Length = 20, IsPrimary = true },
                        new SQColumn() { Name = "TR_Name", DataType = SQDataTypes.String, Length = 250 }
                    }
                },
                tVehicle = new SQTable()
                {
                    Name = "TestedVehicle",
                    Columns = new SQColumnList()
                    {
                        new SQColumn() { Name = "TV_ID", DataType = SQDataTypes.Int64, IsIdentity = true, IsPrimary = true },
                        new SQColumn() { Name = "TV_VIN", DataType = SQDataTypes.String, Length = 25 },
                        new SQColumn() { Name = "TV_MDID", DataType = SQDataTypes.Int64 },
                        new SQColumn() { Name = "TV_BTID", DataType = SQDataTypes.String, Length = 20 },
                        new SQColumn() { Name = "TV_TRID", DataType = SQDataTypes.String, Length = 20 },
                        new SQColumn() { Name = "TV_Year", DataType = SQDataTypes.Int32 }
                    }
                }
            };

            Log.Info("------------------------------------------------------");
            Log.Info("Creating Tables");
            foreach (SQTable t in tables.Reverse<SQTable>())
            {
                if (adp.GetTable(t.Name) != null)
                {
                    Log.Info("Table '" + t.Name + "' exists. Deleting...");
                    Log.Info("SQL: " + adp.WriteRemoveTable(t.Name));
                    adp.RemoveTable(t.Name);
                }
                Log.Info("Creating table '" + t.Name + "'");
                Log.Info("SQL: " + adp.WriteCreateTable(t));
                adp.CreateTable(t);
            }
            #endregion


            #region Add Foreign Keys
            Log.Info("------------------------------------------------------");
            Log.Info("Adding Foreign Keys");
            Log.Info("Creating Foreign Key Model -> Make");
            adp.AddForeignKey(tModel.GetColumnByName("MD_MKID"), tMake.GetColumnByName("MK_ID"));
            Log.Info("Creating Foreign Key Vehicle -> Model");
            adp.AddForeignKey(tVehicle.GetColumnByName("TV_MDID"), tModel.GetColumnByName("MD_ID"));
            Log.Info("Creating Foreign Key Vehicle -> Body");
            adp.AddForeignKey(tVehicle.GetColumnByName("TV_BTID"), tBodyType.GetColumnByName("BT_ID"));
            Log.Info("Creating Foreign Key Vehicle -> Transmission");
            adp.AddForeignKey(tVehicle.GetColumnByName("TV_TRID"), tTrans.GetColumnByName("TR_ID"));
            #endregion


            #region Import Data
            Log.Info("------------------------------------------------------");
            Log.Info("Importing Data");
            List<string> makeIDs = new List<string>();
            List<string> modelIDs = new List<string>();
            List<string> bodyIDs = new List<string>();
            List<string> transIDs = new List<string>();

            string varMakeID = adp.CreateVariable("MakeID");
            string varMake = adp.CreateVariable("Make");
            string varModelID = adp.CreateVariable("ModelID");
            string varModel = adp.CreateVariable("Model");
            string varTransID = adp.CreateVariable("TransID");
            string varTrans = adp.CreateVariable("Trans");
            string varBodyID = adp.CreateVariable("BodyID");
            string varBody = adp.CreateVariable("Body");
            string varVIN = adp.CreateVariable("VIN");
            string varYear = adp.CreateVariable("Year");
            string varDummy = adp.CreateVariable("Dummy");

            SQInsertQuery iMake = new SQInsertQuery()
            {
                Table = new SQAliasableObject(tMake.Name),
                SetPairs = new List<SQSetQueryPair>
                {
                    new SQSetQueryPair("MK_ID", varMakeID),
                    new SQSetQueryPair("MK_Name", varMake)
                },
                Parameters = new List<SQParameter> { new SQParameter(varMakeID, ""), new SQParameter(varMake, "") }
            };

            SQInsertQuery iModel = new SQInsertQuery()
            {
                Table = new SQAliasableObject(tModel.Name),
                SetPairs = new List<SQSetQueryPair>
                {
                    new SQSetQueryPair("MD_ID", varModelID),
                    new SQSetQueryPair("MD_Name", varModel),
                    new SQSetQueryPair("MD_MKID", varMakeID)
                },
                Parameters = new List<SQParameter> { new SQParameter(varModelID, ""), new SQParameter(varModel, ""), new SQParameter(varMakeID, "") }
            };

            SQInsertQuery iBodyType = new SQInsertQuery()
            {
                Table = new SQAliasableObject(tBodyType.Name),
                SetPairs = new List<SQSetQueryPair>
                {
                    new SQSetQueryPair("BT_ID", varBodyID),
                    new SQSetQueryPair("BT_Name", varBody),
                    new SQSetQueryPair("BT_Dummy", varDummy)
                },
                Parameters = new List<SQParameter> { new SQParameter(varBodyID, ""), new SQParameter(varBody, ""), new SQParameter(varDummy, "") }
            };

            SQInsertQuery iTrans = new SQInsertQuery()
            {
                Table = new SQAliasableObject(tTrans.Name),
                SetPairs = new List<SQSetQueryPair>
                {
                    new SQSetQueryPair("TR_ID", varTransID),
                    new SQSetQueryPair("TR_Name", varTrans)
                },
                Parameters = new List<SQParameter> { new SQParameter(varTransID, ""), new SQParameter(varTrans, "") }
            };

            SQInsertQuery iVehicle = new SQInsertQuery()
            {
                Table = new SQAliasableObject(tVehicle.Name),
                SetPairs = new List<SQSetQueryPair>
                {
                    new SQSetQueryPair("TV_VIN", varVIN),
                    new SQSetQueryPair("TV_MDID", varModelID),
                    new SQSetQueryPair("TV_BTID", varBodyID),
                    new SQSetQueryPair("TV_TRID", varTransID),
                    new SQSetQueryPair("TV_Year", varYear)
                },
                Parameters = new List<SQParameter> { 
                    new SQParameter(varVIN, ""), 
                    new SQParameter(varModelID, ""), 
                    new SQParameter(varBodyID, ""), 
                    new SQParameter(varTransID, ""), 
                    new SQParameter(varYear, "")
                }
            };




            Log.Info("------------------------------------------------------");
            Log.Info("Testing Inserts...");
            Log.Info("Make SQL: " + iMake.Write(adp));
            Log.Info("Model SQL: " + iModel.Write(adp));
            Log.Info("BodyType SQL: " + iBodyType.Write(adp));
            Log.Info("Transmission SQL: " + iTrans.Write(adp));
            Log.Info("TestedVehicle SQL: " + iVehicle.Write(adp));
            int vehicleCount = 0;
            SQTransaction trn = adp.OpenTransaction();
            foreach (string[] dataline in data)
            {
                string makeID = dataline[2];
                string make = dataline[3];
                // model id's are unique with respect to the make
                string modelID = makeID + dataline[4];
                string model = dataline[5];
                string transID = dataline[14];
                string trans = dataline[15];
                string bodyID = dataline[8];
                string body = dataline[9];
                string VIN = dataline[10];
                string year = dataline[6];

                int numYear;
                // if there's no year, it's not a valid vehicle. skip this line
                if (!int.TryParse(year, out numYear) || numYear <= 0)
                {
                    continue;
                }

                long numMakeID = Convert.ToInt64(makeID.TrimStart('0'));
                long numModelID = Convert.ToInt64(modelID.TrimStart('0'));

                if (!makeIDs.Contains(makeID))
                {
                    makeIDs.Add(makeID);
                    iMake.Parameters[0].Value = numMakeID;
                    iMake.Parameters[1].Value = make;
                    iMake.Execute(trn);
                }

                if (!modelIDs.Contains(modelID))
                {
                    modelIDs.Add(modelID);
                    iModel.Parameters[0].Value = numModelID;
                    iModel.Parameters[1].Value = model;
                    iModel.Parameters[2].Value = numMakeID;
                    iModel.Execute(trn);
                }

                if (!transIDs.Contains(transID))
                {
                    transIDs.Add(transID);
                    iTrans.Parameters[0].Value = transID;
                    iTrans.Parameters[1].Value = trans;
                    iTrans.Execute(trn);
                }

                if (!bodyIDs.Contains(bodyID))
                {
                    bodyIDs.Add(bodyID);
                    iBodyType.Parameters[0].Value = bodyID;
                    iBodyType.Parameters[1].Value = body;
                    iBodyType.Parameters[2].Value = Guid.NewGuid().ToString();
                    iBodyType.Execute(trn);
                }

                iVehicle.Parameters[0].Value = VIN;
                iVehicle.Parameters[1].Value = numModelID;
                iVehicle.Parameters[2].Value = bodyID;
                iVehicle.Parameters[3].Value = transID;
                iVehicle.Parameters[4].Value = numYear;
                long id = iVehicle.ExecuteReturnID<Int64>(trn);
                vehicleCount++;

                if (vehicleCount > 0 && vehicleCount % 500 == 0)
                {
                    Log.Info("Records Inserted: " + (vehicleCount + bodyIDs.Count + transIDs.Count + modelIDs.Count + makeIDs.Count));
                }
            }
            trn.Commit();
            Log.Info("------------------------------------------------------");
            Log.Info(string.Format(@"Insert Test Complete. 
    Makes: {0}; 
    Models: {1}; 
    BodyTypes: {2}; 
    TransmissionTypes: {3}; 
    TestedVehicle: {4}; ", makeIDs.Count, modelIDs.Count, bodyIDs.Count, transIDs.Count, vehicleCount));
            #endregion


            Log.Info("------------------------------------------------------");
            Log.Info("Select Test");

            SQSelectQuery q = new SQSelectQuery()
            {
                From = new SQFromClause(
                    new SQFromTable(tVehicle.Name),
                    new SQFromTable(tModel.Name),
                    new SQFromTable(tTrans.Name),
                    new SQFromTable(tBodyType.Name),
                    new SQFromTable(tMake.Name)
                ),
                Columns = new List<SQAliasableObject>{ new SQAliasableObject("Model.*, make.*") },
                Condition = new SQConditionGroup(
                    new SQCondition("MD_ID", SQRelationOperators.Equal, "TV_MDID")
                    .And("BT_ID", SQRelationOperators.Equal, "TV_BTID")
                    .And("MD_MKID", SQRelationOperators.Equal, "MK_ID")
                    .And("TV_TRID", SQRelationOperators.Equal, "TR_ID"))
            };

            Log.Info("Selecting with joinless joins types");
            Log.Info("SQL: " + q.Write(adp));


            DataTable dt = new DataTable();
            SQSelectResult res = q.Execute(adp);

            int count = 0;
            while (res.Reader.Read())
            {
                count++;
            }
            res.Close();

            Log.Info("Rows Found: " + count);



            Log.Info("------------------------------------------------------");
            Log.Info("Rename Column");
            Log.Info("Rename BT_Dummy to BT_DummyOK");
            SQColumn dummyColumn = tBodyType.GetColumnByName("BT_Dummy");
            string oldname = dummyColumn.Name;
            dummyColumn.Name = "BT_DummyOK";
            Log.Info("SQL: " + adp.WriteRenameColumn(dummyColumn, oldname));
            adp.RenameColumn(dummyColumn, oldname);



            Log.Info("------------------------------------------------------");
            Log.Info("Remove Column");
            Log.Info("Removing column BT_DummyOK");
            Log.Info("SQL: " + adp.WriteRemoveColumn(dummyColumn));
            adp.RemoveColumn(dummyColumn);




            Log.Info("------------------------------------------------------");
            Log.Info("Insert From Test");
            SQTable tVehicleList = new SQTable()
            {
                Name = "SimpleVehicle",
                Columns = new SQColumnList()
                {
                    new SQColumn() { Name = "SV_ID", DataType = SQDataTypes.Int64, IsIdentity = true, IsPrimary = true },
                    new SQColumn() { Name = "SV_VIN", DataType = SQDataTypes.String, Length = 25 },
                    new SQColumn() { Name = "SV_Year", DataType = SQDataTypes.Int32 },
                    new SQColumn() { Name = "SV_Make", DataType = SQDataTypes.String, Length = 250 },
                    new SQColumn() { Name = "SV_Model", DataType = SQDataTypes.String, Length = 250 }
                }
            };
            Log.Info("Create new table");
            if (adp.GetTable(tVehicleList.Name) != null)
            {
                Log.Info("Table '" + tVehicleList.Name + "' exists. Deleting...");
                Log.Info("SQL: " + adp.WriteRemoveTable(tVehicleList.Name));
                adp.RemoveTable(tVehicleList.Name);
            }
            Log.Info("Creating table '" + tVehicleList.Name + "'");
            Log.Info("SQL: " + adp.WriteCreateTable(tVehicleList));
            adp.CreateTable(tVehicleList);

            SQInsertFromQuery insertFrom = new SQInsertFromQuery(new SQAliasableObject(tVehicleList.Name),
                new string[] { "SV_VIN", "SV_Year", "SV_Make", "SV_Model" },
                new string[] { "TV_VIN", "TV_Year", "MK_Name", "MD_Name" })
            {
                From = new SQFromClause(
                    new SQFromTable(tVehicle.Name)
                    {
                        Join = new SQJoin(tModel.Name)
                        {
                            JoinType = SQJoinTypes.Inner,
                            Predicate = new SQCondition("TV_MDID", SQRelationOperators.Equal, "MD_ID"),
                            Join = new SQJoin(tMake.Name)
                            {
                                JoinType = SQJoinTypes.Inner,
                                Predicate = new SQCondition("MD_MKID", SQRelationOperators.Equal, "MK_ID")
                            }
                        }
                    }),
                Condition = new SQCondition("TV_Year", SQRelationOperators.GreaterThanOrEqual, varYear),
                Parameters = new List<SQParameter> { new SQParameter(varYear, 1990) }
            };

            Log.Info("Do Insert From");
            Log.Info("SQL: " + insertFrom.Write(adp));
            insertFrom.Execute(adp);

            q = new SQSelectQuery()
            {
                Columns = new List<SQAliasableObject> { new SQAliasableObject(SQAggregates.COUNT.Create(adp, "*"), "RecordCount") },
                From = new SQFromClause(new SQFromTable(tVehicleList.Name))
            };
            Log.Info("Get record count for " + tVehicleList.Name);
            Log.Info("SQL: " + q.Write(adp));
            res = q.Execute(adp);

            if (res.Reader.Read())
            {
                Log.Info("Count: " + res.Reader.GetValue(0));
            }
            res.Close();





            Log.Info("------------------------------------------------------");
            Log.Info("Rollback Test");
            trn = adp.OpenTransaction();
            SQDeleteQuery delete = new SQDeleteQuery()
            {
                DeleteTable = new SQAliasableObject(tVehicleList.Name),
                Condition = new SQCondition("SV_Year", SQRelationOperators.LessThan, varYear),
                Parameters = new List<SQParameter> { new SQParameter(varYear, 2000) }
            };
            Log.Info("Deleting data from " + tVehicleList.Name);
            Log.Info("SQL: " + delete.Write(adp));
            delete.Execute(trn);

            res = q.Execute(trn);
            if (res.Reader.Read())
            {
                Log.Info("Get record count for " + tVehicleList.Name + ": " + res.Reader.GetValue(0));
            }
            res.Close();

            Log.Info("Rolling Back");
            trn.RollBack();
            
            res = q.Execute(adp);
            if (res.Reader.Read())
            {
                Log.Info("Get record count for " + tVehicleList.Name + ": " + res.Reader.GetValue(0));
            }
            res.Close();
        }

        private static void FillTest(object sender, FillErrorEventArgs e)
        {

        }
    }
}
