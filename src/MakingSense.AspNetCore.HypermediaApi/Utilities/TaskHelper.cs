using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MakingSense.AspNetCore.Utilities
{
    public static class TaskHelper
    {
		public static T WaitAndGetValue<T>(this Task<T> task)
		{
			try
			{
				task.Wait();
				return task.Result;
			}
			catch (Exception e)
			{
				throw e.InnerException ?? e;
			}
		}
	}
}
