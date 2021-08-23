using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Structure
{
    public static void MakeTree(Vector3 position, Queue<VoxelMod> queue, int minTrunkHeight, int maxTrunkHeight, float radius)
    {
        int height = (int) (maxTrunkHeight * Noise.GetStructurPerlin(new Vector2(position.x, position.z), 250f, 3f));
        int leaveState = 0;
        
        if (height < minTrunkHeight)
            height = minTrunkHeight;

        int cRadius = (int)Mathf.Ceil(radius);

        for (int y = cRadius - 1; y > 0 ; y--)
        {
            for (int x = -cRadius; x <= cRadius; x++)
            {
                for (int z = -cRadius; z <= cRadius; z++)
                {
                    if ((Mathf.Abs(x) + Mathf.Abs(z)) <= y)
                        queue.Enqueue(new VoxelMod(
                            new Vector3(position.x + x, position.y + height + (cRadius - y), position.z + z), 11));

                    leaveState++;
                }
            }
        }


        for(int y = height; y > height - 2; y--)
        {
            for(int z = -cRadius ; z <= cRadius; z++)
            {
                for(int x = -cRadius; x <= cRadius; x++)
                {
                    if((radius * radius) >= (x * x + z * z))
                        queue.Enqueue(new VoxelMod(new Vector3(position.x + x, position.y + y, position.z + z), 11));
                }
            }
        }

        for (int i = 1; i < height; i++)
            queue.Enqueue(new VoxelMod(new Vector3(position.x, position.y + i, position.z), 8));

        
    }
}
