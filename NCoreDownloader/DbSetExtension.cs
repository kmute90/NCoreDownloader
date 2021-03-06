﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NCoreDownloader
{
	public static class DbSetExtensions
	{
		public static async Task<T> AddIfNotExists<T>(this DbSet<T> dbSet, T entity, Expression<Func<T, bool>> predicate = null) where T : class, new()
		{
			var exists = predicate != null ? await dbSet.AnyAsync(predicate) : await dbSet.AnyAsync();
			return !exists ? dbSet.Add(entity).Entity : null;
		}
	}
}
