using System.Collections.Generic;

namespace MunicipalServicesApp.Models
{
    public sealed class ServiceRequestProfile
    {
        public string RequestId { get; set; }

        public string Area { get; set; }

        public List<string> DependsOn { get; set; }

        public string DependencyNote { get; set; }

        public List<StatusChange> History { get; set; }
    }
}
