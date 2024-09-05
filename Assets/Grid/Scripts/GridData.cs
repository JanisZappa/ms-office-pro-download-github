using GeoMath;
using UnityEngine;

namespace GridData
{
    [System.Serializable]
    public struct ItemData
    {
        public Vector2 qMin, qMax, uvMin, uvMax, yRange, zRange;

        public ItemData(Vector2 qMin, Vector2 qMax, Vector2 uvMin, Vector2 uvMax, Vector2 yRange, Vector2 zRange)
        {
            this.qMin   = qMin;
            this.qMax   = qMax;
            this.uvMin  = uvMin;
            this.uvMax  = uvMax;
            this.yRange = yRange;
            this.zRange = zRange;
        }
    }
    
    
    [System.Serializable]
    public struct ScatterObject
    {
        public Vector3 pos;
        public int id;
        public float scale, flip;

        public ScatterObject(Vector3 pos, int id, float scale, float flip)
        {
            this.pos   = pos;
            this.id    = id;
            this.scale = scale;
            this.flip = 0;//flip;
        }


        public void CreateCollider()
        {
            switch (ItemInfo.GetSpriteItemName(id))
            {
                default: return;
                case "tree": GridPhysics.AddEllipse(pos, .065f * scale); break;
                case "peak": GridPhysics.AddEllipse(pos, .525f * scale); break;
            }
        }


        public Bounds2D GetBounds
        {
            get
            {
                ItemData data = ItemInfo.itemData[id];
                return new Bounds2D(pos.x + data.qMax.x * scale,
                                    pos.x + data.qMin.x * scale, 
                                    pos.z + data.qMax.y * scale, 
                                    pos.z + data.qMin.y * scale);
            }
        }
    }
}
