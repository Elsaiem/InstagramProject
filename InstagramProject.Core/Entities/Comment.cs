using InstagramProject.Core.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Entities
{
	public class Comment
	{
		public int Id { get; set; }
		public string Content { get; set; } = default!;
		public int PostId { get; set; }
		public DateTime Time { get; set; } = DateTime.UtcNow;
		public string UserId { get; set; } = default!;
		public bool IsReported { get; set; }
		public ApplicationUser User { get; set; } = default!;
		public ICollection<Reaction> Reactions = new List<Reaction>();
	}
}
