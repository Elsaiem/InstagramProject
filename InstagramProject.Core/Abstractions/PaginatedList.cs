﻿using Microsoft.EntityFrameworkCore;

namespace InstagramProject.Core.Abstractions
{
	public class PaginatedList<T>
	{
		public List<T> Items { get; private set; }
		public int PageNumber { get; private set; }
		public int TotalPages { get; private set; }
		public bool HasPreviosPage => PageNumber > 1;
		public bool HasNextPage => PageNumber < TotalPages;

		public PaginatedList(List<T> items, int pageNumber, int count, int pageSize)
		{
			Items = items;
			PageNumber = pageNumber;
			TotalPages = (int)Math.Ceiling(count / (double)pageSize);
		}
		public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
		{
			var count = await source.CountAsync(cancellationToken);
			var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
			return new PaginatedList<T>(items, pageNumber, count, pageSize);
		}
		public static PaginatedList<T> Empty()
		{
			return new PaginatedList<T>(new List<T>(), 1, 0, 10);
		}
	}
}
