using System;

namespace MakingSense.AspNetCore.HypermediaApi.Linking.StandardRelations
{
	public class SelfRelation : IRelation
	{
		public Type InputModel => null;

		public bool IsVirtual => false;

		public HttpMethod? Method => null;

		public Type OutputModel => null;

		public string RelationName => "self";
	}
}
