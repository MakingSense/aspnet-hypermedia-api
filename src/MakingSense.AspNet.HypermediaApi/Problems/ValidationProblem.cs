using MakingSense.AspNet.HypermediaApi.Metadata;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using MakingSense.AspNet.HypermediaApi.ExceptionHandling;
using MakingSense.AspNet.Abstractions;

namespace MakingSense.AspNet.HypermediaApi.Problems
{
	public class ValidationProblem : Problem
	{
		private string _detail;
		private Maybe<ActionRelationAttribute> _relation;
		private Maybe<SchemaAttribute> _schema;

		public override string title => "Validation error";
		public override int status => StatusCodes.Status400BadRequest;
		public override int errorCode => 1;
		public override string detail => _detail;

		public ErrorItem[] errors { get; set; }

		public ValidationProblem(IEnumerable<ErrorItem> errors)
			: this(null, null, errors)
		{
		}

		public ValidationProblem(Maybe<ActionRelationAttribute> relation, Maybe<SchemaAttribute> schema, IEnumerable<ErrorItem> errors)
		{
			_relation = relation;
			_schema = schema;

			if (relation.HasValue && schema.HasValue)
			{
				_detail = $"Validation error validating `{schema.Value.SchemaFilePath}` for `{relation.Value.Name}`. See `errors` field for more details.";
			}
			else if (relation.HasValue)
			{
				_detail = $"Validation error validating data for `{relation.Value.Name}`. See `errors` field for more details.";
			}
			else
			{
				_detail = $"Validation error. See `errors` field for more details.";
			}

			this.errors = errors.ToArray();

			//TODO: add links for schema and relation
		}

		public class ErrorItem
		{
			public string key { get; set; }
			public string detail { get; set; }
		}
	}
}
