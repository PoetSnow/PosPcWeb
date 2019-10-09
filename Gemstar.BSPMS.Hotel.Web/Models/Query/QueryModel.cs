using Gemstar.BSPMS.Common.Services.EntityProcedures;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Web.Models.Query
{
    public class QueryModel
    {
        public List<UpQueryProcedureParametersResult> ProcedureParameters { get; set; }
        public string ParameterValues { get; set; }
    }
}