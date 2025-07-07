using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Contracts.Common
{
	public record RequestFilters
	{
		public int PageNumber { get; init; } = 1;
		public int PageSize { get; init; } = 20;
	}
}
