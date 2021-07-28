using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureMapping
{
    private const float maxTexturWidht = 16;

    public static readonly Vector2[] VoxelTextureUV = new Vector2[]
    {
        new Vector2(0.0f, 0.0f),
        new Vector2(0.0f, 1.0f / maxTexturWidht),
        new Vector2(1.0f / maxTexturWidht, 0.0f),
        new Vector2(1.0f / maxTexturWidht, 1.0f / maxTexturWidht)
    };

    public static Vector2 GetUVPos(int i)
    {
        if(i > 256)
            return new Vector2(0.0f,0.0f);
        
        float x = i % maxTexturWidht / maxTexturWidht;
        float y = (i - (i % maxTexturWidht) )/ (maxTexturWidht * maxTexturWidht);
        
        return new Vector2(x, y);
    }
}
