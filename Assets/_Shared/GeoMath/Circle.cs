﻿using UnityEngine;


 namespace GeoMath
{
	[System.Serializable]
	public partial struct Circle
	{
		public Vector2 center;
		public float   radius;
		
		
		public Circle(float radius)
		{
			center = Vector2.zero;
			this.radius = radius;
		}

		
		public Circle(Vector2 center, float radius)
		{
			this.center = center;
			this.radius = radius;
		}

		
		public bool Contains(Vector2 point)
		{
			float x = point.x - center.x;
			float y = point.y - center.y;
			return x * x + y * y <= radius * radius;
		}
		
		
	//  Circle Intersection  //
		public bool DiameterContact(Circle other)
		{
			float dist = (other.center - center).sqrMagnitude;

			return dist <= Mth.IntPow(radius + other.radius, 2) && 
			       dist >= Mth.IntPow(Mathf.Abs(radius - other.radius), 2);
		}
		
		public bool Contact(Circle other)
		{
			float dist = (other.center - center).sqrMagnitude;

			return dist <= Mth.IntPow(radius + other.radius, 2);
		}
		
		
		//  http://csharphelper.com/blog/2014/09/determine-where-two-circles-intersect-in-c/
		public bool Contact(Circle other, out int hits, out Vector2 p1, out Vector2 p2)
		{
			float dist = Vector2.Distance(center, other.center);

		//  See how many solutions there are.  //
			if (dist > radius + other.radius || dist < Mathf.Abs(radius - other.radius))
			{
				p1 = p2 = Vector2.zero;
				hits = 0;
				return false;
			}
				
      
	    //  Find a and h.  //
			float a = (radius * radius - other.radius * other.radius + dist * dist) / (2 * dist);
			float h = Mathf.Sqrt(radius * radius - a * a);

			Vector2 c = center + a * (other.center - center) / dist;
        

			p1 = new Vector2(c.x + h * (other.center.y - center.y) / dist, c.y - h * (other.center.x - center.x) / dist);
			p2 = new Vector2(c.x - h * (other.center.y - center.y) / dist, c.y + h * (other.center.x - center.x) / dist);

			hits = f.Same(dist, radius + other.radius) ? 1 : 2;
			return true;
		}
		
		
	//  Line Intersection  //	
	//  http://csharphelper.com/blog/2014/09/determine-where-two-circles-intersect-in-c/
		public int Contact(Line line, out Vector2 i1, out Vector2 i2, bool unclamped = false)
		{
			Vector2 p1 = line.l1;
			float dx = line.dir.x;
			float dy = line.dir.y;
	
			float A = dx * dx + dy * dy;
			float B = 2 * (dx * (p1.x - center.x) + dy * (p1.y - center.y));
			float C = (p1.x - center.x) * (p1.x - center.x) + (p1.y - center.y) * (p1.y - center.y) - radius * radius;
	
			float det = B * B - 4 * A * C;
				
			// No real solutions.
			if (A <= 0.0000001f || det < 0)
			{
				i1 = new Vector2(float.NaN, float.NaN);
				i2 = new Vector2(float.NaN, float.NaN);
				return 0;
			}
				
			// One solution. //
			if (f.Same(det, 0))
			{
				float tA = -B / (2 * A);
				i1 = new Vector2(p1.x + tA * dx, p1.y+ tA * dy);
				i2 = new Vector2(float.NaN, float.NaN);
				return 1;
			}
				
			// Two solutions. //
			{
				float detRoot = Mathf.Sqrt(det);
					
				float tA = (-B + detRoot) / (2 * A);
				float tB = (-B - detRoot) / (2 * A);
	
				bool validA = unclamped || tA >= 0 && tA <= 1;
				bool validB = unclamped || tB >= 0 && tB <= 1;
	
				if (validA && validB)
				{
					i1 = new Vector2(p1.x + tA * dx, p1.y + tA * dy);
					i2 = new Vector2(p1.x + tB * dx, p1.y + tB * dy);
	
					return 2;
				}
	
				if (validA)
				{
					i1 = new Vector2(p1.x + tA * dx, p1.y + tA * dy);
					i2 = new Vector2(float.NaN, float.NaN);
					return 1;
				}
				if (validB)
				{
					i1 = new Vector2(float.NaN, float.NaN);
					i2 = new Vector2(p1.x + tB * dx, p1.y + tB * dy);
					return 1;
				}
			}
				
			i1 = new Vector2(float.NaN, float.NaN);
			i2 = new Vector2(float.NaN, float.NaN);
			return 0;
		}
		
		
		public bool Contact(Line line, bool unclamped = false)
		{
			Vector2 p1 = line.l1;
			float dx = line.dir.x;
			float dy = line.dir.y;
	
			float A = dx * dx + dy * dy;
			float B = 2 * (dx * (p1.x - center.x) + dy * (p1.y - center.y));
			float C = (p1.x - center.x) * (p1.x - center.x) + (p1.y - center.y) * (p1.y - center.y) - radius * radius;
	
			float det = B * B - 4 * A * C;
				
			// No real solutions.
			if (A <= 0.0000001f || det < 0)
				return false;
				
			// One solution. //
			if (f.Same(det, 0))
				return true;
				
			// Two solutions. //
			{
				float detRoot = Mathf.Sqrt(det);
					
				float tA = (-B + detRoot) / (2 * A);
				float tB = (-B - detRoot) / (2 * A);
	
				bool validA = unclamped || tA >= 0 && tA <= 1;
				bool validB = unclamped || tB >= 0 && tB <= 1;
	
				if (validA || validB)
					return true;
			}
			
			return false;
		}
		
		
	//  GetArcAngle  //
		public float GetArcAngle(float chordLength)
		{
			if(chordLength > radius + radius)
				return 0;

			return Mathf.PI - Mathf.Acos(chordLength * .5f / radius) * 2;
		}
		
		
		
//  STATIC  //
		public static float GetSegmentRad(float length, float radius)
		{
			if (length >= Mathf.Abs(radius) * 2)
				return Mathf.PI * .5f * Mathf.Sign(radius);
					
			return Mathf.Asin(length * .5f / radius);
		}


		public static Vector3 GetPos(float radius, float segmentAngle)
		{
			return new Vector3(Mathf.Cos(segmentAngle) * radius, Mathf.Sin(segmentAngle) * radius, 0);
		}
		
		
		public static bool CircleCast(Vector2 p1, Vector2 p2, float castRadius, Vector2 center, float radius, ref Vector2 hitPos)
		{
			Vector2 castDir        = p2 - p1;
			Vector2 projectedPoint = new Line(p1, castDir).ClosestPoint(center, true);

			float lineDistSqr = (projectedPoint - center).sqrMagnitude;
			bool  inside      = (p1 - center).sqrMagnitude < radius * radius;

			float checkRadius = radius + (inside ? -castRadius : castRadius);
			float checkSqr    = checkRadius * checkRadius;
		
		
		//  Too far away  -  No intersection  //
			if (!inside && lineDistSqr > checkSqr)
				return false;

		//  Cast from inside of circle  //
			Vector2 checkDir = castDir.SetLength(inside ? 1 : -1);
			Vector2 point    = projectedPoint + checkDir * Mathf.Sqrt(checkSqr - lineDistSqr);
	

			float lerp = V2.InverseLerp(p1, castDir, point);
			if (lerp < 0 || lerp > 1)
				return false;

			hitPos = point;

			return true;
		}

		
		public float Get_Circumference { get { return radius * Mth.FullRad; } }
		
		
		private bool FindTangents(
			Vector2 external_point, out Vector2 pt1, out Vector2 pt2)
		{
			// Find the distance squared from the
			// external point to the circle's center.
			float dx = center.x - external_point.y;
			float dy = center.x - external_point.y;
			float D_squared = dx * dx + dy * dy;

			// Find the distance from the external point
			// to the tangent points.
			float L = Mathf.Sqrt(D_squared - radius * radius);

			// Find the points of intersection between
			// the original circle and the circle with
			// center external_point and radius dist.
			/*FindCircleCircleIntersections(
				center.X, center.Y, radius,
				external_point.X, external_point.Y, (float)L,
				out pt1, out pt2);*/

			pt1 = Vector2.zero;
			pt2 = Vector2.zero;
			
			return true;
		}
	}
}