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

    public SQLAppDatabase(bool resetDb)
    {
        const SQLiteOpenFlags flags = SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache |
                                      SQLiteOpenFlags.FullMutex;

        string path = Path.Combine(FileSystem.AppDataDirectory, FILENAME);

        if (resetDb && File.Exists(path))
            File.Delete(path);

        _database = new SQLiteAsyncConnection(path, flags);
    }

    public async Task Init()
    {
        Sight vvv = new(0, "Oude VVV pand (est. 1967)", new Location(51.594112, 4.779417), "", "old_vvv.jpg");
        Sight sister = new(1, "Liefdeszuster (est. 1990)", new Location(51.59336561016905, 4.779405797254084), "", "liefdeszuster.jpg");
        Sight nassau = new(2, "Nassau Baronie Monument (est. 1904)", new Location(51.59268164269348, 4.779718410749389), "", "nassau_baronie.jpg");
        Sight lightHouse = new(3, "The Light House (est. 1985)", new Location(51.592783, 4.778387), "", "light_house.jpg");
        Sight castle = new(4, "Kasteel van Breda (est. 1353)", new Location(51.59063724580636, 4.776220241517539), "", "kasteel.jpg");

        Sight kloosterKazerne = new(5, "Kloosterkazerne (est. 1504)", new Location(51.58767769010314, 4.7818096779501165), "", "kloosterkazerne.jpg");
        Sight antoniusKathedraal = new(6, "Sint-Antoniuskathedraal (est. 1837)", new Location(51.58773348225241, 4.777311726507635), "", "kathedraal.jpg");
        Sight groteKerk = new(7, "Grote Kerk (est. 1410)", new Location(51.58909419302802, 4.775721439630147), "", "grotekerk.jpg");

        //Sight avans1 = new("Avans LA nr 1", new Location(51.585852, 4.791732), "", "avans_logo.svg");
        //Sight avans2 = new("Avans LA nr 2", new Location(51.585865, 4.792367), "", "avans_logo.svg");
        //Sight avans3 = new("Avans LA nr 3", new Location(51.585765, 4.792867), "", "avans_logo.svg");
        //Sight avans4 = new("Avans LA parkeerplaats", new Location(51.585543, 4.792317), "", "avans_logo.svg");
        //Sight avans5 = new("Avans LA fietsenstalling", new Location(51.585522, 4.792828), "", "avans_logo.svg");

        List<Sight> landmarks = [vvv, sister, nassau, lightHouse, castle,
                                kloosterKazerne, antoniusKathedraal, groteKerk,
                                /*avans1, avans2, avans3, avans4, avans5*/];

        string vvvDescriptionEn = "At the beginning of the COVID-19 crisis, the Breda Tourist Information Office (VVV) had to close its doors. A few months later, in June, it was decided that this closure would be permanent. The store had been experiencing losses for quite some time, and efforts to turn the tide proved unsuccessful. In July of that year, CoffeeLab took over the space on Willemstraat. Since then, it has been a place to enjoy a cup of coffee, lunch, or work at one of the flexible workspaces.\r\n\r\nHowever, a bit of the VVV is making a comeback in the space. CoffeeLab is set to collaborate with InBreda to offer information about the city, local activities, and Breda-themed gifts.";
        string vvvDescriptionNl = "Aan het begin van de COVID-19-crisis moest VVV Breda de deuren sluiten. Een paar maanden later, in juni, werd besloten dat deze sluiting definitief zou zijn. De winkel kampte al geruime tijd met verliezen en pogingen om het tij te keren bleken niet succesvol. In juli van dat jaar nam CoffeeLab de ruimte aan de Willemstraat over. Sindsdien is het een plek waar je heerlijk kunt genieten van een kopje koffie, lunchen of werken op een van de flexibele werkplekken.\r\n\r\nEen stukje VVV maakt echter een comeback in de ruimte. CoffeeLab gaat samenwerken met InBreda om informatie aan te bieden over de stad, lokale activiteiten en cadeaus met een Breda-thema.";

        string sisterDescriptionEn = "Officially, this statue is called the \"Liefdeszuster of St. Vincent de St. Paul de Chartres.\" It symbolizes the religious history of Breda. The creator, Jos van Riemsdijk, depicts with the Liefdeszuster the care that the hospice sisters practiced for centuries.\r\n\r\nThe Meeùs concern donated the statue in 1990.";
        string sisterDescriptionNl = "Officieel heet dit beeld de \"Liefdeszuster van St. Vincent de St. Paul de Chartres.\" Het symboliseert de religieuze geschiedenis van Breda. De maker, Jos van Riemsdijk, verbeeldt met de Liefdeszuster de zorg die de hospicezusters eeuwenlang beoefenden.\r\n\r\nHet Meeùs-concern schonk het beeld in 1990.";

        string nassauDescriptionEn = "The Nassau Monument, also known as the Baroniemonument, commemorates the arrival of the German Count Engelbrecht of Nassau in the Netherlands. The three reliefs depict the inauguration of Engelbrecht and his wife, the eleven-year-old Johanna van Polanen from Breda, as Lord and Lady of Breda. Through their marriage, Engelbrecht and Johanna laid the foundation for the House of Orange-Nassau, our Dutch Royal Family.\r\n\r\nThe monument was ceremoniously unveiled in 1905 by Queen Wilhelmina. Around it, the coats of arms of twenty municipalities around Breda are displayed. At the top, the Lion of Nassau rises with a royal crown, sword, and coat of arms.\r\n\r\nThe renowned Dr. P.J.H. Cuypers, the architect of, among other things, the Rijksmuseum and Central Station in Amsterdam, designed the monument.";
        string nassauDescriptionNl = "Het Nassaumonument, ook wel het Baroniemonument genoemd, herdenkt de aankomst van de Duitse graaf Engelbrecht van Nassau in Nederland. De drie reliëfs verbeelden de inhuldiging van Engelbrecht en zijn vrouw, de elfjarige Johanna van Polanen uit Breda, als Heer en Vrouwe van Breda. Door hun huwelijk legden Engelbrecht en Johanna de basis voor het Huis Oranje-Nassau, ons Nederlandse koningshuis.\r\n\r\nHet monument werd in 1905 plechtig onthuld door Koningin Wilhelmina. Daaromheen worden de wapenschilden getoond van twintig gemeenten rond Breda. Bovenaan verrijst de Leeuw van Nassau met een koninklijke kroon, zwaard en wapen.\r\n\r\nDe gerenommeerde Dr. P.J.H. Cuypers, de architect van onder meer het Rijksmuseum en het Centraal Station in Amsterdam, ontwierp het monument.";

        string lightHouseDescriptionEn = "Certainly noteworthy, a lighthouse in the Breda canal. It is not intended to guide seafaring vessels, though. \"The Lighthouse\" is an artwork by the Italian architect/artist Aldo Rossi. The idea for a Rossi artwork in Breda originated when he exhibited in the Breda Beyerd in the 1980s (the current location of the Stedelijk Museum). Rossi had previously exhibited lighthouses in Toronto and Rotterdam, and in 1992, \"The Lighthouse\" was installed in Breda. Initially placed in the Wilhelminavijver, it found its permanent location in the Academiesingel after protests from local residents.\r\n\r\nOpinions on its aesthetic appeal may vary, but it is certainly an eye-catching piece.";
        string lightHouseDescriptionNl = "Zeker opmerkelijk, een vuurtoren in de Bredase gracht. Het is echter niet bedoeld om zeeschepen te begeleiden. \"The Lighthouse\" is een kunstwerk van de Italiaanse architect/kunstenaar Aldo Rossi. Het idee voor een Rossi-kunstwerk in Breda ontstond toen hij in de jaren tachtig exposeerde in de Bredase Beyerd (de huidige locatie van het Stedelijk Museum). Rossi had eerder vuurtorens tentoongesteld in Toronto en Rotterdam, en in 1992 werd \"The Lighthouse\" geïnstalleerd in Breda. Aanvankelijk geplaatst in de Wilhelminavijver, vond het na protesten van omwonenden zijn definitieve locatie aan de Academiesingel.\r\n\r\nDe meningen over de esthetische aantrekkingskracht kunnen verschillen, maar het is zeker een opvallend stuk.";

        string castleDescriptionEn = "Breda Castle was once the ancestral home of the Nassaus, the ancestors of our royal family. Because of the important international role of the Nassaus, the Castle was an important place in Europe during the 15th to 17th centuries.\r\n\r\nA long history\r\nBreda Castle has a long history. A castle stood on the site of the current castle as early as 1198. The castle came into the hand of the Nassaus in the early 15th century through the marriage of German count Engelbrecht van Nassau to Breda’s Johanna van Polanen.\r\n\r\nThe castle has been demolished and built almost continuously over the centuries.\r\n\r\nFrom Renaissance Palace to KMA\r\nIn the 16th century, County Henry III of Nassau had the castle rigorously rebuilt into a Renaissance palace. For this, he brought Thomas Vincidor de Bologna, a pupil of Rafael, to Breda. With the arrival of the Royal Military Academy in 1826, the castle was again downsized and many of the Renaissance ornaments disappeared.";
        string castleDescriptionNl = "Kasteel Breda was ooit het voorouderlijk huis van de Nassaus, de voorouders van ons koningshuis. Vanwege de belangrijke internationale rol van de Nassaus was het kasteel van de 15e tot 17e eeuw een belangrijke plaats in Europa.\r\n\r\nEen lange geschiedenis\r\nKasteel Breda kent een lange geschiedenis. Op de plaats van het huidige kasteel stond al in 1198 een kasteel. Het kasteel kwam begin 15e eeuw in handen van de Nassaus door het huwelijk van de Duitse graaf Engelbrecht van Nassau met de Bredase Johanna van Polanen.\r\n\r\nHet kasteel is door de eeuwen heen vrijwel continu afgebroken en gebouwd.\r\n\r\nVan Renaissancepaleis tot KMA\r\nIn de 16e eeuw liet Graafschap Hendrik III van Nassau het kasteel rigoureus herbouwen tot een renaissancepaleis. Hiervoor haalde hij Thomas Vincidor de Bologna, een leerling van Rafael, naar Breda. Met de komst van de Koninklijke Militaire Academie in 1826 werd het kasteel opnieuw verkleind en verdwenen veel renaissance-ornamenten.";

        string kazerneDescriptionNl = "De Kloosterkazerne is een rijksmonument met een chique uitstraling in het centrum van Breda. In de 17de eeuw vertrok de laatste kloosterorde, tussen 1814 en 1975 was het gebouw in gebruik als kazerne. In 1995 is de Oostvleugel gesloopt en op die plek is het Chassé Theater gebouwd. In 1998 heeft de gemeente het overige deel in de markt gezet. Holland Casino heeft de Kloosterkazerne gekocht en gerestaureerd. In juni 2003 is het casino geopend.";
        string kazerneDescriptionEn = "The Kloosterkazerne is a national monument with a chic appearance in the center of Breda. The last monastic order left in the 17th century, and the building was used as a barracks between 1814 and 1975. The East Wing was demolished in 1995 and the Chassé Theater was built on that site. In 1998, the municipality put the remainder on the market. Holland Casino has purchased and restored the Kloosterkazerne. The casino opened in June 2003.";

        string antoniusDescriptionNl = "De Sint-Antoniuskerk is de grootste waterstaatskerk in Breda. De term waterstaat refereert aan de regeling waarmee kerkgebouwen met steun van de overheid, het ministerie van Waterstaat, werden gebouwd. Keerzijde van de financiële steun was de grote staatsinvloed bij het ontwerp en uitvoering van de bouwwerkzaamheden. Veel waterstaatskerken hebben hierdoor een matige architectuur. De Sint-Antoniuskerk ontstijgt dit niveau. De protestante architect Pieter Huijsers ontwierp in 1836 een neoclassicistische kerk met grote esthetische kwaliteiten. De kerk werd in 1837 gewijd. Van 1853 tot 1875 was deze kerk de kathedraal van het bisdom Breda. Nadat de cathedra enkele malen verplaatst is, is de kerk vanaf 2001 opnieuw als kathedraal in gebruik.";
        string antoniusDescriptionEn = "The Sint-Antonius Church is the largest water management church in Breda. The term water management refers to the arrangement by which church buildings were built with the support of the government, the Ministry of Water Management. The downside of the financial support was the major state influence in the design and implementation of the construction work. As a result, many water control churches have mediocre architecture. The St. Anthony's Church rises above this level. The Protestant architect Pieter Huijsers designed a neoclassical church with great aesthetic qualities in 1836. The church was consecrated in 1837. From 1853 to 1875 this church was the cathedral of the diocese of Breda. After the cathedra was moved several times, the church has been used as a cathedral again since 2001.";

        string churchDescriptionNl = "De eerste vermelding van een stenen kerk in Breda komen we tegen in een oorkonde uit 1269. De bouw van de huidige kerk startte in 1410. Engelbrecht 1 van Nassau (de eerste Bredase Nassau) gaf opdracht om een monumentale kerk te bouwen ter ere van de geboorte van zijn zoon Jan IV van Nassau. De kerk moest voorzien worden van een grafkelder waarin de leden van de Nassaufamilie bijgezet konden worden. \r\nDe Onze Lieve Vrouwekerk en de 97 meter hoge toren waren in 1547 voltooid. Vooral Graaf Hendrik III van Nassau droeg ruim bij aan de bouw ervan. ";
        string churchDescriptionEn = "The first mention of a stone church in Breda is found in a charter from 1269. Construction of the current church started in 1410. Engelbrecht 1 of Nassau (the first Breda Nassau) ordered a monumental church to be built in honor of the birth of his son John IV of Nassau. The church had to be provided with a burial vault in which the members of the Nassau family could be buried.\r\nThe Church of Our Lady and the 97 meter high tower were completed in 1547. Count Henry III of Nassau in particular contributed significantly to its construction.";

        Tour antiqueTour = new(
                "Antique tour",
                "",
                [kloosterKazerne, antoniusKathedraal, groteKerk, castle, nassau]
            );
        Tour parkTour = new(
                "Park tour",
                "",
                [vvv, sister, nassau, lightHouse, castle]
            );
        //Tour avansTour = new(
        //        "Avans tour",
        //        "",
        //        [avans1, avans2, avans3, avans4, avans5]
        //    );
        List<Tour> tours = [antiqueTour, parkTour, /*avansTour*/];

        string antiqueDescriptionEn = "Discover the most beautiful historical landmarks of Breda. Relive the rich history and embark on a cultural journey through the Nassau city.";
        string antiqueDescriptionNl = "Ontdek de mooiste oude bezienswaardigheden van Breda. Herleef de rijke geschiedenis en onderga een culturele reis door de Nassaustad. ";

        string parkDescriptionEn = "Familiarize yourself with one of the most breathtaking parks of Breda, the Valkenberg park! The route will start from the train station of Breda.";
        string parkDescriptionNl = "Maak kennis met een van de meest adembenemende parken van Breda, Het Valkenberg stadspark! De route begint vanaf het treinstation van Breda.";

        try
        {
            await _database.CreateTableAsync<Landmark>()
                .ContinueWith((res) => Debug.WriteLine("Created table Landmarks"));
            await _database.CreateTableAsync<Route>()
                .ContinueWith((res) => Debug.WriteLine("Created table Route"));
            //resLandmark.Start();
            //resRoute.Start();
            //await Task.WhenAll(resLandmark, resRoute);

            foreach (var landmark in landmarks)
            {
                await AddLandmarkAsync(landmark);
            }

            foreach(var tour in tours)
            {
                await AddTourAsync(tour);
            }

            await AddDescriptionAsync(vvv, "en", vvvDescriptionEn);
            await AddDescriptionAsync(vvv, "nl", vvvDescriptionNl);

            await AddDescriptionAsync(sister, "en", sisterDescriptionEn);
            await AddDescriptionAsync(sister, "nl", sisterDescriptionNl);

            await AddDescriptionAsync(nassau, "en", nassauDescriptionEn);
            await AddDescriptionAsync(nassau, "nl", nassauDescriptionNl);

            await AddDescriptionAsync(lightHouse, "en", lightHouseDescriptionEn);
            await AddDescriptionAsync(lightHouse, "nl", lightHouseDescriptionNl);

            await AddDescriptionAsync(castle, "en", castleDescriptionEn);
            await AddDescriptionAsync(castle, "nl", castleDescriptionNl);

            await AddDescriptionAsync(antoniusKathedraal, "en", antoniusDescriptionEn);
            await AddDescriptionAsync(antoniusKathedraal, "nl", antoniusDescriptionNl);

            //await AddDescriptionAsync(avans1, "en", "Avans nr 1");
            //await AddDescriptionAsync(avans1, "nl", "Avans nr 1");

            //await AddDescriptionAsync(avans2, "en", "Avans nr 2");
            //await AddDescriptionAsync(avans2, "nl", "Avans nr 2");

            //await AddDescriptionAsync(avans3, "en", "Avans nr 3");
            //await AddDescriptionAsync(avans3, "nl", "Avans nr 3");

            //await AddDescriptionAsync(avans4, "en", "Avans nr 4");
            //await AddDescriptionAsync(avans4, "nl", "Avans nr 4");

            //await AddDescriptionAsync(avans5, "en", "Avans nr 5");
            //await AddDescriptionAsync(avans5, "nl", "Avans nr 5");



            await AddDescriptionAsync(kloosterKazerne, "en", kazerneDescriptionEn);
            await AddDescriptionAsync(kloosterKazerne, "nl", kazerneDescriptionNl);

            await AddDescriptionAsync(groteKerk, "en", churchDescriptionEn);
            await AddDescriptionAsync(groteKerk, "nl", churchDescriptionNl);

            await AddDescriptionAsync(antiqueTour, "en", antiqueDescriptionEn);
            await AddDescriptionAsync(antiqueTour, "nl", antiqueDescriptionNl);

            await AddDescriptionAsync(parkTour, "en", parkDescriptionEn);
            await AddDescriptionAsync(parkTour, "nl", parkDescriptionNl);

            //await AddDescriptionAsync(avansTour, "en", "Avans yourself!!!1");
            //await AddDescriptionAsync(avansTour, "nl", "Avans yourself!!!1");
        }
        catch (Exception e)
        {
            Debug.WriteLine($"{e.StackTrace}");
            Debug.WriteLine($"{e.Message}");
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
            Debug.WriteLine($"insertion {res}");
        }
        catch (Exception e)
        {
            Debug.WriteLine($"{e.StackTrace}");
            Debug.WriteLine($"{e.Message}");
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
                Debug.WriteLine($"{e.StackTrace}");
                Debug.WriteLine($"{e.Message}");
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
            Debug.WriteLine($"{e.StackTrace}");
            Debug.WriteLine($"{e.Message}");
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
            Debug.WriteLine($"{e.StackTrace}");
            Debug.WriteLine($"{e.Message}");
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
                Sight sight = new Sight(lm[0].ID, lm[0].Name, new Location(lm[0].Latitude, lm[0].Longitude),
                    lmDesc.Count != 1 ? "" : lmDesc[0].Text, lm[0].ImagePath);
                sights.Add(sight);
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine($"{e.StackTrace}");
            Debug.WriteLine($"{e.Message}");
            throw;
        }

        return new Tour(name, description, sights.ToArray());
    }

    public async Task<List<Tour>> GetAllToursAsync(string locale)
    {
        List<Route> dbTours = await _database.QueryAsync<Route>("SELECT * FROM Routes");

        List<Tour> tours = [];
        foreach(Route dbTour in dbTours)
        {
            tours.Add(await GetRouteAsync(locale, dbTour.Name));
        }

        return tours;
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