using System;
using System.Collections.Generic;
using System.Text;

namespace JadaraITKnowledgeSystem.Application.Common.Models
{
    public record StorageFileInfo
    {
        public Guid Guid { get; init; }
        public string StorageZoneName { get; init; } = String.Empty;
        public string Path { get; init; } = String.Empty;
        public string ObjectName { get; init; } = String.Empty;
        public long Length { get; init; }
        public DateTime LastChanged { get; init; }
        public int ServerId { get; init; }
        public int ArrayNumber { get; init; }
        public bool IsDirectory { get; init; }
        public Guid UserId { get; init; }
        public string ContentType { get; init; } = String.Empty;
        public DateTime DateCreated { get; init; }
        public int StorageZoneId { get; init; }
        public string Checksum { get; init; } = String.Empty;
        public string ReplicatedZones { get; init; } = String.Empty;
    }
}
