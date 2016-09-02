using System;

namespace MakingSense.AspNetCore.HypermediaApi.Linking
{
	public interface IRelation
	{
		string RelationName { get; }
		HttpMethod? Method { get; }
		Type InputModel { get; }
		Type OutputModel { get; }
		bool IsVirtual { get; }
	}
}
