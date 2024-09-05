using GeoMath;
using UnityEngine;


[System.Serializable]
public struct Arc
{
	public const float LineTipPathRatio = 2.265039f;
	
	public Vector2 center;
	public float  radius, angle, bend;

	
	public Arc(Vector2 center, float radius, float angle, float bend)
	{
		this.center = center;
		this.radius = radius;
		this.angle  = angle;
		this.bend   = bend;
	}

	
	public Arc(Line line, float bend, float lerp = .5f)
	{
		float   dir_M = line.dir.magnitude;
		Vector2 lineM = line.l1 + line.dir * lerp;
		
		this.bend  = bend;
		float bendSign = Mathf.Sign(bend);
		
		radius = Mathf.Abs(1 / bend / 2 / Mth.π * dir_M);
		center = lineM + (Rot.Z(90) * line.dir * ((dir_M > 0? 1f / dir_M : 0) * radius * bendSign)).V2();
		angle  = (lineM - center).Angle_Sign(Vector2.up) - 180 * Mathf.Abs(bend) + Mathf.Lerp(180, -180, lerp) * bend + 180 * (1 - (bendSign * .5f + .5f));
	}

	
	public Vector2 GetClosestPoint(Vector2 point)
	{
		return LerpPos(Mathf.Clamp01(GetClosestLerp(point)));
	}
	
	
	public float GetClosestLerp(Vector2 point)
	{
		float   absBend   = Mathf.Abs(bend);
		Vector2 centerDir = Vector2.up.Rot(angle + absBend * 180) * Mathf.Sign(bend);
		float   radAngle  = centerDir.RadAngle(point - center);
		float   lerp      = (absBend * Mth.π + radAngle) / (absBend * Mth.FullRad);

		return bend < 0 ? 1 - lerp : lerp;
	}
	
	
	public Vector2 LerpPos(float lerp)
	{
		if (bend < 0)
			lerp = 1 - lerp;
		
		float rad          = lerp * Mth.FullRad * Mathf.Abs(bend) + angle * Mathf.Deg2Rad;
		float signedRadius = SignedRadius;
		
		return new Vector2(-Mathf.Sin(rad) * signedRadius + center.x, 
			                Mathf.Cos(rad) * signedRadius + center.y);
	}
	
	
	public Vector2 LerpDir(float lerp)
	{
		if (bend < 0)
			lerp = 1 - lerp;
		
		float rad = lerp * Mth.FullRad * Mathf.Abs(bend) + (angle + 180) * Mathf.Deg2Rad;
		return rad.ToRadDir();
	}
	
	
	public Arc SetPos(Vector2 pos, float posLerp = .5f)
	{
		Vector2 lerpPos   = LerpPos(posLerp);
		Vector2 newCenter = new Vector2(pos.x + (center.x - lerpPos.x), pos.y + (center.y - lerpPos.y));
		return new Arc(newCenter, radius, angle, bend);
	}
	
	
	public Arc SetCenterPos(Vector2 pos)
	{
		return new Arc(pos, radius, angle, bend);
	}
	
	
	public Arc Rotate(float rotAngle, float rotLerp = .5f)
	{
		Vector2 lerpPos  = LerpPos(rotLerp);
		Vector2 toCenter = (center - lerpPos).Rot(rotAngle);
		
		return new Arc(lerpPos + toCenter, radius, angle + rotAngle, bend);
	}
	
	
	public Arc RotateCenter(float rotAngle)
	{
		return new Arc(center, radius, angle + rotAngle, bend);
	}


	public Arc SetBend(float bend, float lerp = .5f)
	{
		return new Arc(new Line(this, lerp), bend, lerp);
	}
	
	
	public int Contact(Line line, out Vector2 i1, out Vector2 i2, bool unclamped = false)
	{
		Vector2 c1, c2;
		int circleHits = new Circle(center, radius).Contact(line, out c1, out c2, unclamped);

		const float thresh = .000005f, upper = 1 + thresh, lower = -thresh;
	//  Check if Points are On Arc  //
		switch (circleHits)
		{
			case 2:
			{
				float l2 = GetClosestLerp(c2);
				if (l2 < lower || l2 > upper)
					circleHits--;

				float l1 = GetClosestLerp(c1);
				if (l1 < lower || l1 > upper)
				{
					if (circleHits == 2)
						c1 = c2;

					circleHits--;
				}
			}
				break;

			case 1:
			{
				float l1 = GetClosestLerp(c1);
				if (l1 < lower || l1 > upper)
					circleHits = 0;
			}
				break;
		}

		
		i1 = c1;
		i2 = circleHits == 2 ? c2 : c1;

		return circleHits;
	}


	public bool RayCast(Vector2 root, Vector2 dir, out Vector2 hitPoint)
	{
		Vector2 i1, i2;
		int arcHits = Contact(new Line(root, root + dir), out i1, out i2, true);

	//  Check if hits are in right Direction  //
		switch (arcHits)
		{
			case 2:
				if(Vector2.Dot(i2 - root, dir) < 0)
					arcHits--;

				if (Vector2.Dot(i1 - root, dir) < 0)
				{
					if (arcHits == 2)
						i1 = i2;

					arcHits--;
				}
				break;
			
			case 1:
				if (Vector2.Dot(i1 - root, dir) < 0)
					arcHits = 0;
				break;
		}
		
	
		hitPoint = arcHits == 2 ? (i1 - root).sqrMagnitude < (i2 - root).sqrMagnitude ? i1 : i2 : arcHits == 1 ? i1 : root;
		return arcHits > 0;
	}


	public Arc Shift(float amount)
	{
		return new Arc(center, radius + amount * -Mathf.Sign(bend), angle, bend);
	}
	
	
	
	
	
	public float Get_Length { get { return Mathf.Abs(radius * Mth.FullRad * bend); } }


	public float Get_ChordLength { get { return 2 * radius * Mathf.Sin(Mathf.Abs(bend * Mathf.PI)); } }
	
	
	public float Get_Height
	{
		get
		{
			float chord = Get_ChordLength;
			return radius - Mathf.Sqrt(radius * radius - chord * chord * .25f);
		}
	}
	
	
	public float SignedRadius { get { return radius * Mathf.Sign(bend); } }
}
