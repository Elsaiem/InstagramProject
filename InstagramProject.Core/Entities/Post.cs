using InstagramProject.Core.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Entities
{
	public class Post
	{
		public int Id { get; set; }
		public DateTime Time { get; set; } = DateTime.UtcNow;
		public string Content { get; set; } = default!;
		public ApplicationUser User { get; set; } = default!;
		public string? Image { get; set; } = default!;
		public string UserId { get; set; } = default!;
		public bool IsReported { get; set; }
		public ICollection<Reaction> Reactions = new List<Reaction>();
		public ICollection<UserSavedPost> Saved = new List<UserSavedPost>();
		public ICollection<Comment> Comments = new List<Comment>();
	}
}
