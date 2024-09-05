using UnityEngine;


namespace GeoMath
{
	public static class Rectangle
	{
		public static Vector2 FitRotationSize(Vector2 size, float angle)
		{
			float factor = 1;

			for (int corner = 0; corner < 2; corner++)
			{
				Vector2 point = V2.zero;
				Vector2 toCorner = new Vector2(size.x * (corner == 0 ? .5f : -.5f), size.y * .5f).Rot(angle);
				Line line = new Line(V2.zero, toCorner);

				for (int side = 0; side < 4; side++)
				{
					Line other;
					switch (side)
					{
						default:
							other = new Line(new Vector2(-size.x * .5f, size.y * .5f), new Vector2(size.x * .5f, size.y * .5f));
							break;
						case 1:
							other = new Line(new Vector2(size.x * .5f, size.y * .5f), new Vector2(size.x * .5f, -size.y * .5f));
							break;
						case 2:
							other = new Line(new Vector2(size.x * .5f, -size.y * .5f), new Vector2(-size.x * .5f, -size.y * .5f));
							break;
						case 3:
							other = new Line(new Vector2(-size.x * .5f, -size.y * .5f), new Vector2(-size.x * .5f, size.y * .5f));
							break;
					}

					if (line.Contact(other, out point))
						break;
				}

				float newFactor = point.magnitude / toCorner.magnitude;
				if (newFactor < factor)
					factor = newFactor;
			}

			return new Vector2(size.x * factor, size.y * factor);
		}
	}
}
    
    
    
    
