﻿using AssetRipper.SourceGenerated.Classes.ClassID_208;
using AssetRipper.SourceGenerated.Enums;

namespace AssetRipper.SourceGenerated.Extensions
{
	public static class NavMeshObstacleExtensions
	{
		public static NavMeshObstacleShape GetShape(this INavMeshObstacle obstacle)
		{
			return obstacle.Has_Shape_C208()
				? (NavMeshObstacleShape)obstacle.Shape_C208
				: NavMeshObstacleShape.Capsule;
		}
	}
}
