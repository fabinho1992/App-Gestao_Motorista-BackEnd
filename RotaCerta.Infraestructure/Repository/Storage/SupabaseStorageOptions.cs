using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotaCerta.Infrastructure.Repository.Storage
{
    public class SupabaseStorageOptions
    {
        public string Url { get; set; } = string.Empty;
        public string Bucket { get; set; } = string.Empty;
        public string ServiceRoleKey { get; set; } = string.Empty;
    }
}
