using System.Collections.Generic;

namespace POEMgr.Repository.CommonQuery
{
    public enum Operators
    {
        Contains = 0,
        Equal = 1,
        GreaterThan = 2,
        GreaterThanOrEqual = 3,
        LessThan = 4,
        LessThanOrEqual = 5,
        None = 6,
        StartWith = 7,
        EndWidth = 8,
        Range = 9,
        NotEqual = 10
    }
    public enum Condition
    {
        AndAlso = 0,
        OrElse = 1
    }

    public class CommonQueryParam
    {
        public Condition Condition { get; set; } = Condition.AndAlso;

        public List<ColumnQueryParam> ColumnQueryParams { get; set; }

        public int? PageIndex { get; set; } = 1;

        public int? PageSize { get; set; } = 9999;
    }

    public class ColumnQueryParam
    {
        public string Name { get; set; }
        public Operators Operator { get; set; } = Operators.Contains;
        public string Value { get; set; }
        public string ValueMin { get; set; }
        public string ValueMax { get; set; }
    }
}