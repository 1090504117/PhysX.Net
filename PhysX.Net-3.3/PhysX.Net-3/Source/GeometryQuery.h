#pragma once

#include "Geometry.h"
#include "SceneEnum.h"

namespace PhysX
{
	ref class SweepHit;

	public ref class GeometryQuery
	{
	public:
		static SweepHit^ Sweep(Vector3 unitDirection, float distance, Geometry^ geom0, Matrix pose0, Geometry^ geom1, Matrix pose1, [Optional] Nullable<HitFlag> hitFlags, [Optional] Nullable<float> inflation);
	};
}