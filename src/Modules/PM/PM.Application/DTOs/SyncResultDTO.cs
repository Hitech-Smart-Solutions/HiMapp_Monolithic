namespace Himapp.Shared.Responses.PM;

public class SyncResultDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int Fetched { get; set; }
        public int Inserted { get; set; }
        public int Updated { get; set; }
    }