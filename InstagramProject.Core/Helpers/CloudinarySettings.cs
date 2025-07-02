using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Helpers
{
    public class CloudinarySettings
    {
		public static string SectionName = "CloudinarySettings";
		public string CloudName { get; set; } = default!;
        public string ApiKey { get; set; } = default!;
        public string ApiSecret { get; set; } = default!;
    }
}