using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

namespace AplikacjaBazodanowa
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Eksplorator danych James Webb Space Telescope");
            Console.WriteLine("Podaj, z ktorej opcji chcesz skorzystac");
            Console.WriteLine("1 - Rekordy na podstawie numeru programu badawczego");
            Console.WriteLine("2 - Rekordy na podstawie typu pliku");
            Console.WriteLine("3 - [BAZA DANYCH] Pokaz posortowane rekordy (.jpg) zapisane lokalnie");
            Console.WriteLine("");

            Env.Load(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".env"));

            string choice = Console.ReadLine();

            if (choice == "1")
            {
                Console.WriteLine("Podaj numer programu badawczego (np. 2736): ");

                string programId = Console.ReadLine();
                Task apiTask = FetchAndDisplayJwstDataById(programId);

                apiTask.Wait();
            }
            else if (choice == "2")
            {
                Console.WriteLine("Podaj typ pliku (jpg, ecsv, fits, json): ");

                string fileType = Console.ReadLine();
                Task apiTask = FetchAndDisplayJwstDataByFileType(fileType);

                apiTask.Wait();
            }
            else if (choice == "3")
            {
                ShowLocalDatabaseRecords();
            }

            else
                    {
                Console.WriteLine("Podano niepoprawna opcje.");
                return;
            }

            Console.WriteLine("\nNacisnij dowolny przycisk aby wyjsc.");
            Console.ReadKey();
        }

        static async Task FetchAndDisplayJwstDataById(string programIdString)
        {
            int programId = int.Parse(programIdString);

            using JwstDbContext db = new JwstDbContext();

            var existingProgram = db.Programs
                                    .Include(p => p.Observations)
                                    .FirstOrDefault(p => p.ProgramNumber == programId);

            if (existingProgram != null && existingProgram.Observations.Any())
            {
                Console.WriteLine($"\n[BAZA LOKALNA] Znaleziono {existingProgram.Observations.Count} rekordow dla programu {programId}:");
                foreach (var obs in existingProgram.Observations)
                {
                    Console.WriteLine(obs.ToString());
                }
                return;
            }

            Console.WriteLine("\n[API] Brak danych w bazie. Pobieram dane z internetu...");

            string apiKey = Environment.GetEnvironmentVariable("JWST_API_KEY");

            Console.WriteLine(apiKey == null ? "NULL\n" : apiKey);

            string page = "1";
            string perPage = "10";
            string url = $"https://api.jwstapi.com/program/id/{programId}?page={page}&perPage={perPage}";

            using HttpClient request = new HttpClient();
            request.DefaultRequestHeaders.Add("X-API-KEY", apiKey);

            try
            {
                string jsonResponse = await request.GetStringAsync(url);
                JwstResponse data = JsonSerializer.Deserialize<JwstResponse>(jsonResponse);

                if (data != null && data.Body != null && data.Body.Count > 0)
                {
                    Console.WriteLine($"Pobrano {data.Body.Count} rekordow. Trwa zapis do bazy...\n");

                    DbProgram newProgram = new DbProgram { ProgramNumber = programId };

                    foreach (var observation in data.Body)
                    {
                        Console.WriteLine(observation.ToString());
                        Console.WriteLine(new string('-', 40));

                        newProgram.Observations.Add(new DbObservation
                        {
                            ApiObservationId = observation.Id,
                            FileType = observation.FileType,
                            Location = observation.Location
                        });
                    }

                    db.Programs.Add(newProgram);
                    db.SaveChanges();
                    Console.WriteLine("[SUKCES] Dane zostały pomyślnie zapisane w lokalnej bazie SQLite!");
                }
                else
                {
                    Console.WriteLine("Nie znaleziono danych dla podanego numeru programu.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nWystapil blad: {e.Message}");
            }
        }

        static void ShowLocalDatabaseRecords()
        {
            using JwstDbContext db = new JwstDbContext();

            var sortedRecords = db.Observations
                                  .Where(o => o.FileType == "jpg")
                                  .OrderByDescending(o => o.ApiObservationId)
                                  .ToList();

            if (sortedRecords.Count == 0)
            {
                Console.WriteLine("Brak obrazów jpg w lokalnej bazie. Najpierw pobierz jakieś programy z API.");
                return;
            }

            Console.WriteLine($"\n[BAZA LOKALNA] Znaleziono {sortedRecords.Count} rekordów typu JPG (posortowanych malejąco):");
            foreach (var record in sortedRecords)
            {
                Console.WriteLine(record.ToString());
            }
        }

        static async Task FetchAndDisplayJwstDataByFileType(string filetype)
        {
            string apiKey = REMOVED;
            string page = "1";
            string perPage = "10";
            string url = $"https://api.jwstapi.com/all/type/{filetype}" + $"?page={page}" + $"&perPage={perPage}";

            using HttpClient request = new HttpClient();

            request.DefaultRequestHeaders.Add("X-API-KEY", apiKey);

            try
            {
                string jsonResponse = await request.GetStringAsync(url);

                JwstResponse data = JsonSerializer.Deserialize<JwstResponse>(jsonResponse);

                if (data != null && data.Body != null && data.Body.Count > 0)
                {
                    Console.WriteLine($"Wyswietlono {data.Body.Count} plikow typu {filetype}:\n");

                    foreach (var observation in data.Body)
                    {
                        Console.WriteLine(observation.ToString());
                        Console.WriteLine(new string('-', 40));
                    }
                }
                else
                {
                    Console.WriteLine("Nie znaleziono danych dla podanego numeru programu.");
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"\nBlad polaczenia z API: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nWystapil blad: {e.Message}");
            }
        }
    }
}
