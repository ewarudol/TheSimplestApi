using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kotlinEndPoint.Controllers {
    public class HomeController : Controller {

        private string myPath = "";

        public static void CreateDb(string name, string path) {

            SQLiteConnection.CreateFile(path + ".sqlite");
        }

        public static SQLiteConnection OpenConnection(string path) {

            SQLiteConnection dbConnection = new SQLiteConnection("Data Source="+ path + ".sqlite;Version=3;");
            dbConnection.Open();
            return dbConnection;
        }

        public static int ExecuteQueryNoResult(string query, SQLiteConnection dbConnection) {
            SQLiteCommand command = new SQLiteCommand(query, dbConnection);
            int numRows = command.ExecuteNonQuery();
            return numRows;
        }

        private static Dictionary<string, object> SerializeRow(IEnumerable<string> cols, SQLiteDataReader reader) {
            Dictionary<string, object> result = new Dictionary<string, object>();
            foreach (string col in cols)
                result.Add(col, reader[col]);
            return result;
        }

        public static List<Dictionary<string, object>> ExecuteQueryAndDisplay(string query, SQLiteConnection dbConnection) {

            SQLiteCommand command = new SQLiteCommand(query, dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            List<Dictionary<string, object>> results = new List<Dictionary<string, object>>();
            List<string> cols = new List<string>();
            for (int i = 0; i < reader.FieldCount; i++)
                cols.Add(reader.GetName(i));

            while (reader.Read())
                results.Add(SerializeRow(cols, reader));

            return results;
        }

        public ActionResult Index() {

            return View();
        }

        public ActionResult CreateDataBase(string dbName, string tabName) {
            try {
                string path = Server.MapPath($"~/bin/{dbName}");
                CreateDb("ewadb", path);

                SQLiteConnection dbConnection = OpenConnection(path);
                ExecuteQueryNoResult($"CREATE TABLE {tabName} (name VARCHAR(50), pass VARCHAR(50), someval VARCHAR(500))", dbConnection);
                dbConnection.Cancel();

                return Content("DB created. Look at you, you're so freaking amazing.");
            }
            catch {
                return Content("Oh no! Something went wrong.");
            }
        }

        public ActionResult AddUser(string dbName, string tabName, string name, string pass) {
            try {
                string path = Server.MapPath($"~/bin/{dbName}");

                SQLiteConnection dbConnection = OpenConnection(path);
                ExecuteQueryNoResult($"insert into {tabName} (name, pass, someval) values('{name}', '{pass}', '0')", dbConnection);
                dbConnection.Cancel();

                return Content("OMG, new person in your fantastic app!");
            }
            catch {
                return Content("Oh no! Something went wrong.");
            }
        }

        public ActionResult AddRow(string dbName, string tabName, string name, string pass, string someval) {
            try {
                string path = Server.MapPath($"~/bin/{dbName}");

                SQLiteConnection dbConnection = OpenConnection(path);
                ExecuteQueryNoResult($"insert into {tabName} (name, pass, someval) values('{name}', '{pass}', '{someval}')", dbConnection);
                dbConnection.Cancel();

                return Content("OMG, new sth in your fantastic app!");
            }
            catch {
                return Content("Oh no! Something went wrong.");
            }
        }

        public ActionResult UpdatePoints(string dbName, string tabName, string name, string someval) {
            try {
                string path = Server.MapPath($"~/bin/{dbName}");

                SQLiteConnection dbConnection = OpenConnection(path);
                ExecuteQueryNoResult($"UPDATE {tabName} SET someval = '{someval}' WHERE name = '{name}' ; ", dbConnection);
                dbConnection.Cancel();

                return Content("Tasty, new value arrived!");
            }
            catch {
                return Content("Oh no! Something went wrong.");
            }
        }

        public ActionResult GetTableJson(string dbName, string tabName) {
            try {
                string path = Server.MapPath($"~/bin/{dbName}");

                SQLiteConnection dbConnection = OpenConnection(path);
                List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
                data = ExecuteQueryAndDisplay($"select * from {tabName}", dbConnection);
                dbConnection.Cancel();

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch {
                return Content("Oh no! Something went wrong.");
            }
        }

        public ActionResult CreateTable(string dbName, string tabName) {
            try {
                string path = Server.MapPath($"~/bin/{dbName}");

                SQLiteConnection dbConnection = OpenConnection(path);
                ExecuteQueryNoResult($"CREATE TABLE {tabName} (name VARCHAR(50), pass VARCHAR(50), someval VARCHAR(500))", dbConnection);
                dbConnection.Cancel();

                return Content("Table created. Look at you, you're so freaking amazing.");
            }
            catch {
                return Content("Oh no! Something went wrong.");
            }
        }
    }
}