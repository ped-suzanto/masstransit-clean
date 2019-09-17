using System;

namespace MessageContracts
{
    public interface UpdateIndex
    {
        Guid CorrelationId { get; }
        string Database { get; }
        string Table { get; }
        string Type { get; }
        DateTime Ts { get; }
        int Xid { get; }
        bool Commit { get; }
        Ads Data { get; }
    }

    public class YourMessageCommand : UpdateIndex
    {
        public Guid CorrelationId { get; set; }
        public string Database { get; set; }
        public string Table { get; set; }
        public string Type { get; set; }
        public DateTime Ts { get; set; }
        public int Xid { get; set; }
        public bool Commit { get; set; }
        public Ads Data { get; set; }
        
    }

    public class Ads
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
    }

}
