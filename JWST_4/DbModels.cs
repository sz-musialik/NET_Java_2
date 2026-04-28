using System.Collections.Generic;

namespace AplikacjaBazodanowa
{
    // Tabela 1: Program badawczy
    internal class DbProgram
    {
        public int Id { get; set; } // Klucz główny
        public required int ProgramNumber { get; set; }

        // Relacja 1:N - Jeden program : wiele obserwacji
        public List<DbObservation> Observations { get; set; } = new List<DbObservation>();
    }

    // Tabela 2: Obserwacja
    internal class DbObservation
    {
        public int Id { get; set; } // Klucz główny
        public required string ApiObservationId { get; set; }
        public string? FileType { get; set; }
        public required string Location { get; set; }

        public int DbProgramId { get; set; }
        public DbProgram Program { get; set; }

        public override string ToString()
        {
            return $"ID: {ApiObservationId,-15} | Typ: {FileType,-5} | Link: {Location}";
        }
    }
}