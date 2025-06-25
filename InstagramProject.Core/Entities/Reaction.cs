using InstagramProject.Core.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Entities
{
	public class Reaction
	{
		public int Id { get; set; }
		public bool IsReaction { get; set; }
		public DateTime Time { get; set; } = DateTime.UtcNow;
		public int PostId { get; set; }
		public int? CommentId { get; set; }
		public string UserId { get; set; } = default!;
		public ApplicationUser User { get; set; } = default!;
	}
}
