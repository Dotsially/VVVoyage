using SQLite;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Text;
using VVVoyage.Database;
using Microsoft.VisualBasic.FileIO;
using TableAttribute = SQLite.TableAttribute;

namespace TestProject.Class
{
    public class SQLAppDatabase : IAppDatabase
    {
        private const string FILENAME = "vvvoyage.db";
        private SQLiteAsyncConnection _database;

        public async Task Init()
        {
            if (_database != null)
                return;

            const SQLiteOpenFlags flags = SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache |
                                          SQLiteOpenFlags.FullMutex;

            string path = Path.Combine(Microsoft.VisualBasic.FileIO.FileSystem.CurrentDirectory, FILENAME);

            if (File.Exists(path))
                File.Delete(path);

            _database = new SQLiteAsyncConnection(path, flags);

            try
            {
                _ = await _database.CreateTableAsync<Landmark>();
                _ = await _database.CreateTableAsync<Route>();
            }
            catch (Exception e)
            {
                Debug.WriteLine("{msg}", e.StackTrace);
                Debug.WriteLine("{msg}", e.Message);
            }
        }

        public async Task close()
        {
            await _database.CloseAsync();
        }

        public async Task<bool> AddLandmarkAsync(Sight sight)
        {
            Landmark lm = new Landmark();
            lm.Name = sight.SightPin.Address;
            lm.ImagePath = sight.ImagePath;
            lm.Longitude = sight.SightPin.Location.Longitude;
            lm.Latitude = sight.SightPin.Location.Latitude;
            try
            {
                var res = await _database.InsertAsync(lm);
                Debug.WriteLine("insertion {0}", res);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine("{msg}", e.StackTrace);
                Debug.WriteLine("{msg}", e.Message);
                return false;
            }
        }

        public async Task<bool> AddTourAsync(Tour tour)
        {
            Route r = new Route();
            r.Name = tour.Name;
            r.Description = tour.Description;
            StringBuilder builder = new StringBuilder();
            foreach (Sight sight in tour.Landmarks)
            {
                List<Landmark> lm;
                try
                {
                    lm = await _database.QueryAsync<Landmark>("SELECT * FROM Landmarks WHERE name = ?",
                        sight.SightPin.Address);
                }
                catch (SQLiteException e)
                {
                    throw;
                }

                if (lm.Count != 1) throw new Exception("Invalid landmark in tour");
                builder.Append(lm[0].ID).Append(';');
            }

            r.Landmarks = builder.ToString();
            try
            {
                _ = await _database.InsertAsync(builder);
                return true;
            }
            catch (SQLiteException e)
            {
                throw;
            }
        }

        public async Task AddDescriptionAsync(Sight sight, string locale, string description)
        {
            await InsertDescriptionAsync("DescriptionL_" + locale, sight.SightPin.Address, description);
        }

        public async Task AddDescriptionAsync(Tour tour, string locale, string description)
        {
            await InsertDescriptionAsync("DescriptionR_" + locale, tour.Name, description);
        }

        private async Task InsertDescriptionAsync(string tableName, string itemName, string description)
        {
            try
            {
                _ = await _database.QueryAsync<int>(
                    @"CREATE TABLE IF NOT EXISTS ? (
ID INTEGER PRIMARY KEY,
Text TEXT
);", tableName);

                List<Landmark> res = await _database.QueryAsync<Landmark>(@"SELECT * FROM ? WHERE Name = ?",
                    tableName, itemName);
                // TODO improve exception
                if (res.Count != 1) throw new Exception("Invalid landmark");

                _ = await _database.QueryAsync<int>(@"INSERT INTO ? (ID, Text) VALUES (?, ?)",
                    tableName, res[0].ID, description);
            }
            catch (SQLiteException e)
            {
                throw;
            }
        }

        public async Task<Tour> GetRandomRouteAsync(string locale)
        {
            List<Route> r = await _database.QueryAsync<Route>(@"SELECT * FROM Routes");
            Random random = new Random();
            int randInt = random.Next(0, r.Count);

            return await GetRouteAsync(locale, r[randInt].Name);
        }

        public async Task<Tour> GetRouteAsync(string locale, string name)
        {
            string tableNameRoute = "DescriptionR_" + locale;
            string tableNameLandmark = "DescriptionL_" + locale;
            List<Sight> sights = new List<Sight>();
            string description = "";
            try
            {
                List<Route> r = await _database.QueryAsync<Route>(@"SELECT * FROM Routes WHERE name = ?", name);
                if (r.Count != 1) throw new Exception("Invalid route");
                List<Description> desc =
                    await _database.QueryAsync<Description>(@"SELECT * FROM ? WHERE ID = ?", tableNameRoute, r[0].ID);
                if (desc.Count == 1) description = desc[0].Text;
                string[] lms = r[0].Landmarks.Split(";");
                foreach (string id in lms)
                {
                    List<Landmark> lm = await _database.QueryAsync<Landmark>(@"SELECT * FROM Landmarks WHERE ID = ?", id);
                    List<Description> lmDesc =
                        await _database.QueryAsync<Description>(@"SELECT * FROM ? WHERE ID = ?", tableNameLandmark, id);
                    if (lm.Count != 1) throw new Exception("Invalid route");
                    Sight sight = new Sight(lm[0].Name, new Location(lm[0].Latitude, lm[0].Longitude),
                        lmDesc.Count != 1 ? "" : lmDesc[0].Text, lm[0].ImagePath);
                    sights.Add(sight);
                }
            }
            catch
            {
                throw;
            }

            return new Tour(name, description, sights.ToArray());
        }

        private class Description
        {
            [PrimaryKey] public int ID { get; set; }
            public string Text { get; set; }
        }

        [Table("Landmarks")]
        private class Landmark
        {
            [PrimaryKey] public int ID { get; set; }
            public string Name { get; set; }
            public string ImagePath { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

        [Table("Routes")]
        private class Route
        {
            [PrimaryKey] public int ID { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Landmarks { get; set; }
        }
    }
}
