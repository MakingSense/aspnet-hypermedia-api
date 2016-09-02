using MakingSense.AspNetCore.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace MakingSense.AspNetCore.HypermediaApi.Linking
{
	public interface ILinkHelper
	{
		Maybe<Link> ToAction<T>(Expression<Func<T, Task>> expression) where T : Controller;
		Maybe<Link> ToAction<T>(Expression<Action<T>> expression) where T : Controller;
		Maybe<Link> ToSelf(object values = null);
		Maybe<Link> ToAbsolute(string href);
		Maybe<Link> ToAbsolute(Uri uri);
		Maybe<Link> ToHomeAccount();
	}
}
