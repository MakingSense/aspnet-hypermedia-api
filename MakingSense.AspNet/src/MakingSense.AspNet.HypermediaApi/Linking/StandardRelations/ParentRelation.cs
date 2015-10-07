using System;

namespace MakingSense.AspNet.HypermediaApi.Linking.StandardRelations
{
	public class ParentRelation : IRelation
	{
		public Type InputModel => null;

		public bool IsVirtual => false;

		public HttpMethod? Method => null;

		public Type OutputModel => null;

		public string RelationName => "parent";
	}
}
