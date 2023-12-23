using System.Security.Cryptography.X509Certificates;
using hasheous_server.Classes.Metadata.IGDB;
using NuGet.Protocol.Core.Types;

namespace hasheous_server.Models
{
    public class SignatureLookupItem
    {
        public SignatureResult Signature { get; set; }
        public List<MetadataResult> MetadataResults { get; set; }

        public class SignatureResult
        {
            public SignatureResult(Signatures_Games RawSignature)
            {
                this.Game = RawSignature.Game;
                this.Rom = RawSignature.Rom;
            }

            public Signatures_Games.GameItem Game { get; set; } = new Signatures_Games.GameItem();
            public Signatures_Games.RomItem Rom { get; set; } = new Signatures_Games.RomItem();
        }

        public class MetadataResult
        {
            public object PlatformId { get; set; }
            public BackgroundMetadataMatcher.BackgroundMetadataMatcher.MatchMethod PlatformMatchMethod { get; set; }
            public object GameId { get; set; }
            public BackgroundMetadataMatcher.BackgroundMetadataMatcher.MatchMethod GameMatchMethod { get; set; }
            public Communications.MetadataSources Source { get; set; }
        }
    }
}