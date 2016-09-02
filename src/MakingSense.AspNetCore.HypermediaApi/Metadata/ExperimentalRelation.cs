using System;

namespace MakingSense.AspNetCore.HypermediaApi.Linking.VirtualRelations
{
	public class ExperimentalRelation : IRelation
	{
		public string RelationName => "/docs/rels/experimental";
		public HttpMethod? Method => null;
		public Type InputModel => null;
		public Type OutputModel => null;
		public virtual bool IsVirtual => false;
	}
}
