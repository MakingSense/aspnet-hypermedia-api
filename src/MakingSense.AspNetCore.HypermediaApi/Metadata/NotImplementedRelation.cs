using System;

namespace MakingSense.AspNetCore.HypermediaApi.Linking.VirtualRelations
{
	public class NotImplementedRelation : IRelation
	{
		public string RelationName => "/docs/rels/not-implemented";
		public HttpMethod? Method => null;
		public Type InputModel => null;
		public Type OutputModel => null;
		public virtual bool IsVirtual => false;
	}
}
