using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Structure
{
    public static void MakeTree(Vector3 position, Queue<VoxelMod> queue, int minTrunkHeight, int maxTrunkHeight, int radius)
    {
        double thickness = 0.4;
        int height = (int) (maxTrunkHeight * Noise.GetStructurPerlin(new Vector2(position.x, position.z), 250f, 3f));
        double rIn =radius- thickness, rOut = radius + thickness;
        
        
        if (height < minTrunkHeight)
            height = minTrunkHeight;
        
        /*for (int x = -2; x < 3; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                for (int z = -2; z < 3; z++)
                {
                    queue.Enqueue(new VoxelMod(new Vector3(position.x + x, position.y + height + y, position.z + z), 11));
                }
            }
        }*/

        for (int z = 0; z < height; z++)
        {
            for (float y = radius; y >= radius; ++y)
            {
                for (float x = -radius; x < rOut; x += 0.5f)
                {
                    float value = x * x + y * y;
                    if (value >= rIn * rIn && value <= rOut * rOut)
                    {
                        queue.Enqueue(new VoxelMod(new Vector3(position.x + x, position.y + height + y, position.z + z), 11));
                    }
                }
            }
        }

        for (int i = 1; i < height; i++)
            queue.Enqueue(new VoxelMod(new Vector3(position.x, position.y + i, position.z), 8));

        
    }
}
