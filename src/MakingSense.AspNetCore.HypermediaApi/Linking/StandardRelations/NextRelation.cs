using System;

namespace MakingSense.AspNetCore.HypermediaApi.Linking.StandardRelations
{
	public class NextRelation : IRelation
	{
		public Type InputModel => null;

		public bool IsVirtual => false;

		public HttpMethod? Method => null;

		public Type OutputModel => null;

		public string RelationName => "next";
	}
}
