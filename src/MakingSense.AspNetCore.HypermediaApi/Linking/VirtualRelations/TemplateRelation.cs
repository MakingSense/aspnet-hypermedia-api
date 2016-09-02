using System;

namespace MakingSense.AspNetCore.HypermediaApi.Linking.VirtualRelations
{
	public class TemplateRelation : IRelation
	{
		public Type InputModel => null;

		public HttpMethod? Method => null;

		public Type OutputModel => null;

		public string RelationName => "virtual/template";

		public bool IsVirtual => true;
	}
}
