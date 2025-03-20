using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public GameObject player;
    public Material material;
    public static int chunkSize = 16;
    public static int radius = 2;
    public static Dictionary<string, Chunk> chunkDict;
    Vector3 lastBuildPos;
    public static string CreateChunkName(Vector3 v) 
    {
        return (int)v.x + " " + (int)v.y + " " + (int)v.z;
    }

    void BuildRecursiveWorld(Vector3 chunkPos, int rad)
    {
        int x = (int)chunkPos.x;
        int y = (int)chunkPos.y;
        int z = (int)chunkPos.z;

        BuildChunkAt(chunkPos);

        if(--rad < 0) return;
        BuildRecursiveWorld(new Vector3(x, y, z + chunkSize), rad);
        BuildRecursiveWorld(new Vector3(x, y, z - chunkSize), rad);
        BuildRecursiveWorld(new Vector3(x + chunkSize, y, z ), rad);
        BuildRecursiveWorld(new Vector3(x - chunkSize, y, z), rad);
        BuildRecursiveWorld(new Vector3(x, y + chunkSize, z), rad);
        BuildRecursiveWorld(new Vector3(x, y - chunkSize, z), rad);
    }
    
    //Construção dos chunk
    void BuildChunkAt(Vector3 chunkPos)
    {
        string name = CreateChunkName(chunkPos);
        Chunk c;
        if(!chunkDict.TryGetValue(name, out c))
        {
            c = new Chunk(chunkPos, material);
            c.goChunk.transform.parent = this.transform;
            chunkDict.Add(c.goChunk.name, c);
        }
    }

    void DrawChunks()
    {
        //Desenho do Chunk
        foreach(KeyValuePair<string, Chunk> c in  chunkDict) 
        {
            if(c.Value.status == Chunk.ChunkStatus.DRAW) {
                c.Value.DrawChunk();
            }

        }
    }

    Vector3 WhichChunk(Vector3 position)
    {
        Vector3 chunkPos = new Vector3();
        chunkPos.x = (int)(position.x / chunkSize) * chunkSize;
        chunkPos.y = (int)(position.y / chunkSize) * chunkSize;
        chunkPos.z = (int)(position.z / chunkSize) * chunkSize;
        return chunkPos;
    }

    // Start is called before the first frame update
    void Start()
    {
        player.SetActive(false);
        chunkDict = new Dictionary<string, Chunk>();
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
        Vector3 ppos = player.transform.position;
        player.transform.position = new Vector3(ppos.x, Utils.GenerateHeight(ppos.x, ppos.z) + 1, ppos.z);
        lastBuildPos = player.transform.position;
        BuildRecursiveWorld(WhichChunk(lastBuildPos), radius);
        DrawChunks();
        player.SetActive(true);
    }

    private void Update()
    {
        Vector3 movement = player.transform.position - lastBuildPos;
        if(movement.magnitude > chunkSize)
        {
            lastBuildPos = player.transform.position;
            BuildRecursiveWorld(WhichChunk(lastBuildPos), radius);
            DrawChunks();
        }
    }
}