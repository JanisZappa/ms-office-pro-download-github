using UnityEngine;


namespace GeoMath
{
     [System.Serializable]
     public partial struct Line
     {
         public Vector2 l1, dir;
         
         public float Length { get { return dir.magnitude; } }


         public Line(Vector2 l1, Vector2 l2)
         {
             this.l1 = l1;
             dir = new Vector2(l2.x - l1.x, l2.y - l1.y);
         }

         
         public Vector2 ClosestPoint(Vector2 point, bool unclamped = false)
         {
             Vector2 AP = new Vector2(point.x - l1.x, point.y - l1.y);

             float ABAPproduct = Vector2.Dot(AP, dir);    
         //  The normalized "distance" from a to your closest point  //
             float lerp = unclamped? ABAPproduct / dir.sqrMagnitude : Mathf.Clamp01(ABAPproduct / dir.sqrMagnitude);

             return new Vector2(l1.x + dir.x * lerp, l1.y + dir.y * lerp);
         }
         
         
         public float GetClosestLerp(Vector2 point)
         {
             Vector2 AP = new Vector2(point.x - l1.x, point.y - l1.y);

             float ABAPproduct = Vector2.Dot(AP, dir);    
             //  The normalized "distance" from a to your closest point  //
             return ABAPproduct / dir.sqrMagnitude;
         }
         
         
         public float SqrDistance(Vector2 point, bool unclamped = false)
         {
             return (ClosestPoint(point, unclamped) - point).sqrMagnitude;
         }
         
         
         public bool Contact(Line other, bool unclamped = false)
         {
             Vector2 dummy;
             return Contact(other, out dummy, unclamped);
         }
         
         
         public bool Contact(Line other, out Vector2 point, bool unclamped = false)
         {
             point = Vector2.zero;
            
             if (!unclamped && !BoundsContact(other))
                 return false;
            
             Vector2 dirB = other.dir;

             float compareDot = Vector2.Dot(dir.normalized, dirB.normalized);
             if (!(compareDot > -1 && compareDot < 1))
                 return false;
             /*if (Mathf.Approximately(Vector2.Dot(dir, dirB), 0))
                 return false;*/

             float denominator = dir.y * dirB.x - dir.x * dirB.y;

             float t1 = ((l1.x - other.l1.x) * dirB.y + (other.l1.y - l1.y) * dirB.x) / denominator;
             if (!unclamped && (t1 < 0 || t1 > 1))
                 return false;

             float t2 = ((other.l1.x - l1.x) * dir.y + (l1.y - other.l1.y) * dir.x) / -denominator;
             if (!unclamped && (t2 < 0 || t2 > 1))
                 return false;

             point = new Vector2(l1.x + dir.x * t1, l1.y + dir.y * t1);
             return true;
         }
         
         
         private bool BoundsContact(Line other)
         {
             Vector2 a1 = l1, a2 = l1 + dir;
             Vector2 b1 = other.l1, b2 = other.l1 + other.dir;
             
             return Mathf.Min(a1.x, a2.x) <= Mathf.Max(b1.x, b2.x) && 
                    Mathf.Min(a1.y, a2.y) <= Mathf.Max(b1.y, b2.y) && 
                   
                    Mathf.Min(b1.x, b2.x) <= Mathf.Max(a1.x, a2.x) &&
                    Mathf.Min(b1.y, b2.y) <= Mathf.Max(a1.y, a2.y);
         }
         
         
         public bool RayCast(Vector2 root, Vector2 dir, out Vector2 hitPoint)
         {
             return Contact(new Line(root, root + dir * 10000), out hitPoint);
         }
         
         
         public bool LineIsCloserSqr(Line other, float sqrDist, bool unclamped = false)
         {
             if (Contact(other, unclamped))
                 return true;
            
             {
                 if (SqrDistance(other.l1, unclamped) <= sqrDist || 
                     SqrDistance(other.l1 + other.dir, unclamped) <= sqrDist)
                     return true;
             }
             {
                 if (other.SqrDistance(l1, unclamped) <= sqrDist || 
                     other.SqrDistance(l1 + dir, unclamped) <= sqrDist)
                     return true;
             }

             return false;
         }
         
         
         public float LineDistSqr(Line other, bool unclamped = false)
         {
             if (Contact(other, unclamped))
                 return 0;

             return Mathf.Min(
                 Mathf.Min(SqrDistance(other.l1, unclamped), SqrDistance(other.l1 + other.dir, unclamped)),
                 Mathf.Min(other.SqrDistance(l1, unclamped), other.SqrDistance(l1 + dir, unclamped))
             );
         }


         public Vector2 GetL2()
         {
             return l1 + dir;
         }


         public Line(Arc arc, float lerp = .5f)
         {
             Vector2 arcPos = arc.LerpPos(lerp);
             Vector2 arcDir = arc.LerpDir(lerp);

             float length = arc.Get_Length;

             l1 = arcPos - arcDir * lerp * length;
             dir = arcDir * length;
         }
         
         
         public float GetBend(float radius)
         {
             //  radius = 1 / bend / 2 / Mth.π * length;
             return 1 / radius / 2 / Mth.π * dir.magnitude;
         }
         
         
         public Line Rotate(float angle, float turnLerp = .5f)
         {
             Vector2 turnPoint = new Vector2(l1.x + dir.x * turnLerp, l1.y + dir.y * turnLerp);
             Vector2 turnDir   = dir.Rot(angle);
             
             Vector2 newL1 = new Vector2(turnPoint.x - turnDir.x * turnLerp, turnPoint.y - turnDir.y * turnLerp);
             Vector2 newL2 = new Vector2(newL1.x + turnDir.x, newL1.y + turnDir.y);
             
             return new Line(newL1, newL2);
         }
         
         
         public Line RotateAround(Vector2 point, float angle)
         {
             Vector2 turnDir   = dir.Rot(angle);
             
             Vector2 newL1 = (l1 - point).Rot(angle) + point;
             Vector2 newL2 = new Vector2(newL1.x + turnDir.x, newL1.y + turnDir.y);
             
             return new Line(newL1, newL2);
         }


         public Line SetPos(Vector2 pos, float posLerp = .5f)
         {
             Vector2 newL1 = new Vector2(pos.x - dir.x * posLerp, pos.y - dir.y * posLerp);
             Vector2 newL2 = new Vector2(newL1.x + dir.x, newL1.y + dir.y);
             
             return new Line(newL1, newL2);
         }


         public Line Move(Vector2 move)
         {
             Vector2 newL1 = new Vector2(l1.x + move.x, l1.y + move.y);
             return new Line(newL1, new Vector2(newL1.x + dir.x, newL1.y + dir.y));
         }


         public Line Shift(float amount)
         {
             return Move(dir.Rot90().normalized * amount);
         }
         

         public Vector2 LerpPos(float lerp)
         {
             return new Vector2(l1.x + dir.x * lerp, l1.y + dir.y * lerp);
         }


         public Line GetPerpendicular(float lineLerp = 0)
         {
             return Rotate(-90, lineLerp);
         }


         public float GetAngle(Line other)
         {
             return Vector2.Angle(dir, other.dir);
         }


     //  Creation Helper  //
         public static Line NewMidDirLine(Vector2 center, Vector2 dir)
         {
             return new Line(new Vector2(center.x - dir.x * .5f, center.y - dir.y * .5f), 
                             new Vector2(center.x + dir.x * .5f, center.y + dir.y * .5f));
         }


         public static Line Ray(Vector2 root, Vector2 dir)
         {
             return new Line(root, root + dir.SetLength(short.MaxValue));
         }
         
         
    //  FZRO  //
        public Vector2 GetPoint(float multi)
        {
            return l1 + dir * multi;
        }
        
        
        public float PointDot(Vector2 point)
        {
            return Vector2.Dot(point - l1, dir);
        }


        public Bounds2D GetBounds
        {
            get
            {
                return new Bounds2D(l1).Add(l1 + dir);
            }
        }
    }


     public static class Line3D
     {
         public static float LineLerp(Vector3 l1, Vector3 l2, Vector3 point)
         {
             Vector3 AP  = point - l1;
             Vector3 dir = l2 - l1;
             float ABAPproduct = Vector3.Dot(AP, dir);    
             return Mathf.Clamp01(ABAPproduct / dir.sqrMagnitude);
         }
     }
}