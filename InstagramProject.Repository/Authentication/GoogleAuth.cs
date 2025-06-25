using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Repository.Authentication
{
	public class GoogleAuth
	{
		public static string SectionName = "Google";
		public string? ClientId { get; set; }
		public string? ClientSecret { get; set; }
	}
}
