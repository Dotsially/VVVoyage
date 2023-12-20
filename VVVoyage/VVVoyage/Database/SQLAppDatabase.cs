using SQLite;
using VVVoyage.Models;
using System.Diagnostics;
using System.Text;
using Location = Microsoft.Maui.Devices.Sensors.Location;

namespace VVVoyage.Database;

public class SQLAppDatabase : IAppDatabase
{
    private const string FILENAME = "vvvoyage.db";
    private readonly SQLiteAsyncConnection _database;

    public SQLAppDatabase()
    {
        const SQLiteOpenFlags flags = SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache |
                                      SQLiteOpenFlags.FullMutex;

        string path = Path.Combine(FileSystem.AppDataDirectory, FILENAME);

        _database = new SQLiteAsyncConnection(path, flags);
    }

    public async Task Init()
    {
        try
        {
            await _database.CreateTableAsync<Landmark>()
                .ContinueWith((res) => Debug.WriteLine("Created table Landmarks"));
            await _database.CreateTableAsync<Route>()
                .ContinueWith((res) => Debug.WriteLine("Created table Route"));
            //resLandmark.Start();
            //resRoute.Start();
            //await Task.WhenAll(resLandmark, resRoute);
        }
        catch (Exception e)
        {
            Debug.WriteLine("{msg}", e.StackTrace);
            Debug.WriteLine("{msg}", e.Message);
        }
    }

    public async Task Close()
    {
        await _database.CloseAsync();
    }

    public async Task AddLandmarkAsync(Sight sight)
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
        }
        catch (Exception e)
        {
            Debug.WriteLine("{msg}", e.StackTrace);
            Debug.WriteLine("{msg}", e.Message);
        }
    }

    public async Task AddTourAsync(Tour tour)
    {
        Route r = new Route();
        r.Name = tour.Name;
        r.Description = tour.Description;
        StringBuilder builder = new StringBuilder();
        //for (int i = 0; i < tour.Landmarks.Count; ++i)
        foreach (Sight sight in tour.Landmarks)
        {
            //Sight sight = tour.Landmarks[i];
            List<Landmark> lm;
            try
            {
                lm = await _database.QueryAsync<Landmark>("SELECT * FROM Landmarks WHERE name = ?",
                    sight.SightPin.Address);
            }
            catch (SQLiteException e)
            {
                Debug.WriteLine("{msg}", e.StackTrace);
                Debug.WriteLine("{msg}", e.Message);
                throw;
            }

            if (lm.Count != 1) throw new Exception("Invalid landmark in tour");
            builder.Append(lm[0].ID).Append(';');
        }

        r.Landmarks = builder.ToString();
        try
        {
            _ = await _database.InsertAsync(r);
        }
        catch (SQLiteException e)
        {
            Debug.WriteLine("{msg}", e.StackTrace);
            Debug.WriteLine("{msg}", e.Message);
            throw;
        }
    }

    public async Task AddDescriptionAsync(Sight sight, string locale, string description)
    {
        await InsertDescriptionAsync("DescriptionL_" + locale, true, sight.SightPin.Address, description);
    }

    public async Task AddDescriptionAsync(Tour tour, string locale, string description)
    {
        await InsertDescriptionAsync("DescriptionR_" + locale, false, tour.Name, description);
    }

    private async Task InsertDescriptionAsync(string tableName, bool isLandmark, string itemName, string description)
    {
        try
        {
            _ = await _database.QueryAsync<int>(
                @"CREATE TABLE IF NOT EXISTS " + tableName + @" (
ID INTEGER PRIMARY KEY,
Text TEXT
);");

            int Id = 0;
            if (isLandmark)
            {
                List<Landmark> res = await _database.QueryAsync<Landmark>(
                    @"SELECT * FROM Landmarks WHERE Name = ?",
                    itemName);
                // TODO improve exception
                if (res.Count != 1) throw new Exception("Invalid landmark");
                Id = res[0].ID;
            }
            else
            {
                List<Route> res = await _database.QueryAsync<Route>(
                    @"SELECT * FROM Routes WHERE Name = ?",
                    itemName);
                // TODO improve exception
                if (res.Count != 1) throw new Exception("Invalid Route");
                Id = res[0].ID;
            }


            _ = await _database.QueryAsync<int>(@"INSERT INTO " + tableName + @" (ID, Text) VALUES (?, ?)",
                Id, description);
        }
        catch (SQLiteException e)
        {
            Debug.WriteLine("{msg}", e.StackTrace);
            Debug.WriteLine("{msg}", e.Message);
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
                await _database.QueryAsync<Description>(@"SELECT * FROM " + tableNameRoute + @" WHERE ID = ?", r[0].ID);
            if (desc.Count == 1) description = desc[0].Text;
            string[] lms = r[0].Landmarks.Split(";");
            foreach (string id in lms)
            {
                if (string.IsNullOrEmpty(id)) continue;
                List<Landmark> lm = await _database.QueryAsync<Landmark>(@"SELECT * FROM Landmarks WHERE ID = ?", id);
                List<Description> lmDesc =
                    await _database.QueryAsync<Description>(@"SELECT * FROM " + tableNameLandmark + @" WHERE ID = ?",
                        id);
                if (lm.Count != 1) throw new Exception("Invalid Landmark (route)");
                Sight sight = new Sight(lm[0].Name, new Location(lm[0].Latitude, lm[0].Longitude),
                    lmDesc.Count != 1 ? "" : lmDesc[0].Text, lm[0].ImagePath);
                sights.Add(sight);
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine("{msg}", e.StackTrace);
            Debug.WriteLine("{msg}", e.Message);
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
        [PrimaryKey, AutoIncrement] public int ID { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    [Table("Routes")]
    private class Route
    {
        [PrimaryKey, AutoIncrement] public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Landmarks { get; set; }
    }
}