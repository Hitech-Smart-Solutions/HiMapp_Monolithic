namespace Himapp.PM.Application.DTOs;

public class DynamicsAssetMasterIntegrationDTO
    {
        public string OBJECTID { get; set; } = null!;
        public string DataAreaId { get; set; }
        public decimal? ACQUISITIONCOST { get; set; }
        public DateTime? ACQUISITIONDATE { get; set; }
        public DateTime? ACTIVEFROM { get; set; }
        public DateTime? ACTIVETO { get; set; }
        public string? CATEGORYLEVEL1 { get; set; }
        public string? CATEGORYLEVEL2 { get; set; }
        public string? CATEGORYLEVEL3 { get; set; }
        public int? CRITICALITY { get; set; }
        public string? FIXEDASSETID { get; set; }
        public string? FIXEDASSETNAME { get; set; }
        public string? FUNCTIONALLOCATION { get; set; }
        public string? ISPLBATCHNUMBER { get; set; }
        public DateTime? ISPLHIREDDATE { get; set; }
        public string ISPLISHIREDASSET { get; set; }
        public string? ISPLISSYNC { get; set; }
        public string? ISPLISUPDATED { get; set; }
        public string? ISPLITEMID { get; set; }
        public string? ISPLITEMNAME { get; set; }
        public string? ISPLPONUMBER { get; set; }
        public string? ISPLVEHICLENUMBER { get; set; }
        public string? ISPLVENDORACCOUNT { get; set; }
        public string? MANUFACTURER { get; set; }
        public string? MODEL { get; set; }
        public long? MODELRECID { get; set; } // Mapping bigint to long
        public string? MODELYEAR { get; set; }
        public string? NAME { get; set; }
        public string OBJECTACTIVE { get; set; }
        public string? OBJECTIDPARENT { get; set; }
        public string? OBJECTIDPARENTNAME { get; set; }
        public string? OBJECTLIFECYCLESTATE { get; set; }
        public string? OBJECTTYPE { get; set; }
        public string? PURCHID { get; set; }
        public string? RESOURCE { get; set; }
        public string? SERIALID { get; set; }
        public int? SERVICELEVEL { get; set; }
        public float? STDFUELCONSUMPTION_ISPL { get; set; }
        public string? VENDACCOUNT { get; set; }
        public string? VENDNAME { get; set; }
        public DateTime? WARRANTYDATEFROMVEND { get; set; }
        public DateTime? WARRANTYDATETOVEND { get; set; }
        public string? WARRANTYVEND { get; set; }
        public string? WRKCTRLDESCRIPTION { get; set; }
    }