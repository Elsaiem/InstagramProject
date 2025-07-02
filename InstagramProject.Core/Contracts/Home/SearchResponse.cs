using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Contracts.Home
{
	public record SearchResponse
	(
		string Id,
		string Name,
		string Image
	);
}
