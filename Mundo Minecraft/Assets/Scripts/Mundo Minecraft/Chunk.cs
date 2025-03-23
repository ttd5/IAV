using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Chunk
{
    public Block[,,] chunkdata;
    public GameObject goChunk;
    public enum ChunkStatus { DRAW, DONE };
    public ChunkStatus status;
    Material material;

    public Chunk(Vector3 pos, Material material)
    {
        goChunk = new GameObject(World.CreateChunkName(pos));
        goChunk.transform.position = pos;
        this.material = material;
        BuildChunk();
    }

    void BuildChunk() 
    {
        chunkdata = new Block[World.chunkSize, World.chunkSize, World.chunkSize];
        for(int z = 0; z < World.chunkSize; z++) 
        {
            for(int y = 0; y < World.chunkSize; y++)
            {
                for(int x = 0; x < World.chunkSize; x++) 
                {
                Vector3 pos = new Vector3(x, y, z); //colocar nesta posição
                int worldX = (int)goChunk.transform.position.x + x;
                int worldY = (int)goChunk.transform.position.y + y;
                int worldZ = (int)goChunk.transform.position.z + z;
                int h = Utils.GenerateHeight(worldX, worldZ);
                int hs = Utils.GenerateStoneHeight(worldX, worldZ);

                Grapher.Log(Utils.fBM3D(worldX, worldY, worldZ, 1, 0.5f), "noise3D", Color.red);
                if(worldY <= hs)
                {
                    float below = Utils.fBM3D(worldX, worldY, worldZ, 1, 0.5f);
                    if (below < 0.40f && worldY <= hs - 2)
                    {
                        chunkdata[x, y, z] = new Block(Block.BlockType.DIRT, pos, this, material);
                    }
                    else if (below < 0.5f)
                    {
                        chunkdata[x, y, z] = new Block(Block.BlockType.STONE, pos, this, material);
                    }
                    else if (below < 0.515f)
                    {
                        chunkdata[x, y, z] = new Block(Block.BlockType.LAVA, pos, this, material);
                    }
                    else
                    {
                        chunkdata[x, y, z] = new Block(Block.BlockType.AIR, pos, this, material);
                    }
                }
                else if(worldY == h)
                {
                    chunkdata[x, y, z] = new Block(Block.BlockType.GRASS, pos, this, material);

                    float cactus = Utils.fBM(worldX * 0.1f, worldZ * 0.1f, 3, 0.9f);

                    if (cactus > 0.54 && cactus < 0.58)
                        GenerateCactus(chunkdata, x, y, z);

                        if (y + 1 < World.chunkSize && Random.Range(0f, 1f) < 0.2f)
                    {
                        Vector3 bushPos = new Vector3(x, y + 1, z);
                        chunkdata[x, y + 1, z] = new Block(Block.BlockType.BUSH, bushPos, this, material);
                        Debug.Log($"Bush spawned at {bushPos}");
                    }
                }

                else if (worldY < h)
                chunkdata[x, y, z] = new Block(Block.BlockType.DIRT, pos, this, material);

                else
                    chunkdata[x, y, z] = new Block(Block.BlockType.AIR, pos, this, material);
                }
            }
        }
        status = ChunkStatus.DRAW;
    }

    void GenerateCactus(Block[,,] chunkData, int x, int y, int z)
    {
        int maxHeight = 3; // Cactus max height
        int cactusHeight = Random.Range(2, maxHeight + 1); // Random height between 2 and 3

        if (chunkData[x, y, z] == null)
        {
            chunkData[x, y, z] = new Block(Block.BlockType.GRASS, new Vector3(x, y, z), this, material);
        }

        // Prevent cactus from exceeding chunk size
        if (y + cactusHeight >= World.chunkSize)
        {
            cactusHeight = World.chunkSize - y - 1;
        }

        // Generate the cactus vertically
        for (int i = 0; i < cactusHeight; i++)
        {
            int currentY = y + i;
            if (currentY < World.chunkSize)
            {
                placeBlock(chunkData, x, currentY, z, Block.BlockType.CACTUS);
            }
        }
    }

    void placeBlock(Block[,,] chunkData, int x, int y, int z, Block.BlockType btype)
    {
        if (chunkData[x, y, z] == null)
        {
            chunkData[x, y, z] = new Block(btype, new Vector3(x, y, z), this, material);
        }
        else if (chunkData[x, y, z].GetType() == Block.BlockType.AIR)
        {
            chunkData[x, y, z].SetType(btype);
        }
    }

    public void DrawChunk()
    {
        for(int z = 0; z < World.chunkSize; z++)
            for(int y = 0; y < World.chunkSize; y++)
                for(int x = 0; x < World.chunkSize; x++)
                    chunkdata[x, y, z].Draw();
        CombineQuads();
        MeshCollider collider = goChunk.AddComponent<MeshCollider>();
        collider.sharedMesh = goChunk.GetComponent<MeshFilter>().mesh;
        status = ChunkStatus.DONE;
    }
    
    void CombineQuads() {
        MeshFilter[] meshFilters = goChunk.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length) 
        {
            combine[i].mesh = meshFilters[i].sharedMesh; // Correctly assign the mesh
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix; // Assign the transformation matrix
            i++;
        }

        MeshFilter mf = goChunk.AddComponent<MeshFilter>();
        mf.mesh = new Mesh();

        mf.mesh.CombineMeshes(combine);

        MeshRenderer renderer = goChunk.AddComponent<MeshRenderer>();
        renderer.material = material;

        foreach(Transform quad in goChunk.transform) {
            GameObject.Destroy(quad.gameObject);
        }

    }
}
