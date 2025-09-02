﻿namespace Shared.RequestFeatures
{
    public class EmployeeRequestParameters:RequestParameters
    {
        public EmployeeRequestParameters() => OrderBy = "name";
        
        public uint MinAge { get; set; }
        public uint MaxAge { get; set; }=int.MaxValue;
        public bool ValidateAge =>MinAge<=MaxAge;
        public string? SearchTerm { get; set; }
    }
}
