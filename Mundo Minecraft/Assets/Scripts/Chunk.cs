using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Chunk
{
    public Block[,,] chunkdata;
    public GameObject goChunk;
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
        for(int z = 0; z < World.chunkSize; z++) {
            for(int y = 0; y < World.chunkSize; y++) {
                for(int x = 0; x < World.chunkSize; x++) {
                Vector3 pos = new Vector3(x, y, z); //colocar nesta posição
                if(Random.Range(0f, 1f) < 1f) 
                {
                    if(Random.Range(0f, 1f) < 0.5f)
                        chunkdata[x, y, z] = new Block(Block.BlockType.STONE, pos, this, material);
                    else
                        chunkdata[x, y, z] = new Block(Block.BlockType.DIRT, pos, this, material);
                } 
                else
                    chunkdata[x, y, z] = new Block(Block.BlockType.AIR, pos, this, material);
                }
                
            }
        }
    }

    public void DrawChunk()
    {
        for(int z = 0; z < World.chunkSize; z++) {
            for(int y = 0; y < World.chunkSize; y++) {
                for(int x = 0; x < World.chunkSize; x++) {
                    chunkdata[x, y, z].Draw();
                }
            }
        }
        CombineQuads();
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
