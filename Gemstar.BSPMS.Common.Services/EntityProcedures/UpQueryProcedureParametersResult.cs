namespace Gemstar.BSPMS.Common.Services.EntityProcedures
{
    public class UpQueryProcedureParametersResult
    {
        public int OrdinalPosition { get; set; }
        public string ParameterMode { get; set; }
        public string ParameterName { get; set; }
        public string DataType { get; set; }
        public int? CharacterMaximumLength { get; set; }
        public int? CharacterOctetLength { get; set; }
        public byte? NumericPrecision { get; set; }
        public int? NumericPrecisionRadix { get; set; }
        public int? NumericScale { get; set; }
        public int? DatetimePrecision { get; set; }
        public string DefaulValue { get; set; }
        public string DisplayParameterName { get; set; }
    }
}
