using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriangleNet.Geometry;
using TriangleNet.Topology;

public class room_generator : MonoBehaviour
{
    public GameObject Temp_Room;
    public struct Room_info
    {
        public GameObject room;
        public int width;
        public int height;
        public int ID;
        public string tag;
    }


    private Room_info[] roomArray;
    private Room_info[] stayingRoomArray;

    [SerializeField]
    private int totalRooms = 20;
    private int totalBossRooms = 6;
    private int totalSpawnRooms = 4;
    private int BossSizeWidth = 24;
    private int BossSizeHeight = 24;
    private int MinSize = 8;
    private int MaxSize = 20;

    private List<int> SpawnRoomsList;
    private List<int> AllStayingRooms;
    [SerializeField]
    private int totalStayingRooms = 20;

    public float radius = 2f;

    private Vector3 startPos;
    private Quaternion startRot;

    
    private float waitTime = 3f;
    private float startTime;
    private bool stillAwake = true;
    private bool corrected = false;
    private bool setupStageDone = false;

    Polygon polygon = new Polygon();
    private TriangleNet.Mesh mesh = null;
    private bool DelaunayDone = false;
    private bool MSTDone = false;

    class MST
    {
        static int V;

        public void SetV(int v)
        {
            V = v;
        }

        static int minKey(int[] key, bool[] mstSet)
        {
            int min = int.MaxValue, min_index = -1;

            for (int v = 0; v < V; v++)
                if (mstSet[v] == false && key[v] < min)
                {
                    min = key[v];
                    min_index = v;
                }

            return min_index;
        }

        static void printMST(int[] parent, int[,] graph)
        {
            Debug.Log("Edge \tWeight");
            for (int i = 1; i < V; i++)
                Debug.Log(parent[i] + " - " + i + "\t" + graph[i, parent[i]]);
        }

        static int[,] primMST(int[,] graph)
        {

            // Array to store constructed MST
            int[] parent = new int[V];

            // Key values used to pick
            // minimum weight edge in cut
            int[] key = new int[V];

            // To represent set of vertices
            // included in MST
            bool[] mstSet = new bool[V];

            // Initialize all keys
            // as INFINITE
            for (int i = 0; i < V; i++)
            {
                key[i] = int.MaxValue;
                mstSet[i] = false;
            }

            // Always include first 1st vertex in MST.
            // Make key 0 so that this vertex is
            // picked as first vertex
            // First node is always root of MST
            key[0] = 0;
            parent[0] = -1;

            // The MST will have V vertices
            for (int count = 0; count < V - 1; count++)
            {

                // Pick thd minimum key vertex
                // from the set of vertices
                // not yet included in MST
                int u = minKey(key, mstSet);

                // Add the picked vertex
                // to the MST Set
                mstSet[u] = true;

                // Update key value and parent
                // index of the adjacent vertices
                // of the picked vertex. Consider
                // only those vertices which are
                // not yet included in MST
                for (int v = 0; v < V; v++)

                    // graph[u][v] is non zero only
                    // for adjacent vertices of m
                    // mstSet[v] is false for vertices
                    // not yet included in MST Update
                    // the key only if graph[u][v] is
                    // smaller than key[v]
                    if (graph[u, v] != 0 && mstSet[v] == false
                        && graph[u, v] < key[v])
                    {
                        parent[v] = u;
                        key[v] = graph[u, v];
                    }
            }

            int[,] ans = new int[V, 1];
            for (int i = 1; i < V; i++)
            {
                ans[i, 0] = parent[i];

            }

            return (ans);

            // print the constructed MST
            //printMST(parent, graph);
        }
        public int[,] runMST(int[,] graph)
        {

            int[,] ans = primMST(graph);
            return (ans);
        }
    };

    private void Start()
    {
        roomArray = new Room_info[totalRooms];
        startPos = new Vector3(0f, 0f, 0f);
        startRot = new Quaternion(0f, 0f, 0f, 0f);

        SpawnRoomsList = new List<int>();
        AllStayingRooms = new List<int>();
        if(totalStayingRooms > totalRooms)
        {
            totalStayingRooms = totalRooms;
        }
        stayingRoomArray = new Room_info[totalStayingRooms];

        CreateRooms();
        spawnRooms();
        startTime = Time.time;
    }

    private void Update()
    {
        if(setupStageDone == false)
        {
            if (Time.time >= (startTime + waitTime) && stillAwake == true)
            {
                stillAwake = awakeCheck();
            }

        
            if (stillAwake == false && corrected == false)
            {
                correctPosRooms();
            }
        
            if(corrected == true)
            {
                tagRooms();
                pickRooms();
            }
        }
        else
        {
            if(DelaunayDone == false)
                Delaunay();
            if(MSTDone == false)
                find_MST();
        }

        if(roomArray != null)
            drawOutline(roomArray);
        else
            drawOutline(stayingRoomArray);
    }

    void CreateRooms()
    {
        int x, y;
        for(int i = 0; i < roomArray.Length; i++)
        {
            if(i < totalBossRooms)
            {
                x = BossSizeWidth;
                //x = Random.Range(MinSize, MaxSize);
                //if (x % 2 != 0)
                //    x += 1;
                y = BossSizeHeight;
                //y = Random.Range(MinSize, MaxSize);
                //if (y % 2 != 0)
                //    y += 1;
            }
            else
            {
                x = Random.Range(MinSize, MaxSize);
                if (x % 2 != 0)
                    x += 1;
                y = Random.Range(MinSize, MaxSize);
                if (y % 2 != 0)
                    y += 1;
            }

            //Debug.Log("x: " + x + "y: " + y);
            roomArray[i].width = x;
            roomArray[i].height = y;
            roomArray[i].ID = i;
            
        }
    }

    void spawnRooms()
    {
        float x, y;
        
        for(int i = 0; i < roomArray.Length; i++)
        {
            x = Random.Range(-radius, radius);
            y = Random.Range(-radius, radius);
            Vector2 pos = new Vector2(x, y);
            pos = Vector2.ClampMagnitude(pos, radius);
            GameObject r;
            r = Instantiate(Temp_Room, pos, startRot);
            r.transform.parent = gameObject.transform;

            BoxCollider2D bc2D = r.GetComponent<BoxCollider2D>();
            bc2D.size = new Vector2(roomArray[i].width + 4f, roomArray[i].height + 4f);
            

            roomArray[i].room = r;
            roomArray[i].room.name = "Room " + i.ToString();
        }

        for (int i = 0; i < roomArray.Length; i++)
        {
            BoxCollider2D bc2D = roomArray[i].room.GetComponent<BoxCollider2D>();
            bc2D.enabled = !bc2D.enabled;
        }

        //doneSpawning = true;
    }

    void tagRooms()
    {
        int count = 0;
        for (int i = 0; i < roomArray.Length; i++)
        {
            
            if (i < totalBossRooms)
            {
                roomArray[i].tag = "BossRoom";
            }
            else
            {
                if( count < totalSpawnRooms && roomArray[i].width >= 14 && roomArray[i].height >= 14)
                {
                    roomArray[i].tag = "SpawnRoom";
                    count++;
                    SpawnRoomsList.Add(i);
                    //Debug.Log(i);
                }
                else
                    roomArray[i].tag = "Room";
            }
            roomArray[i].room.tag = roomArray[i].tag;
        }
    }

    void pickRooms()
    {
        float maxDist = 0, curDist, count;
        int pickB = 0, pickS = 0, pick = 0;
        for(int i = 0; i<totalBossRooms; i++)
        {
            curDist = Vector3.Distance(startPos, roomArray[i].room.transform.position);
            if ( curDist > maxDist)
            {
                maxDist = curDist;
                pickB = i;
            }
        }
        AllStayingRooms.Add(pickB);
        maxDist = 0;
        List<int> tempList = SpawnRoomsList;
        for (int i = 0; i<totalSpawnRooms; i++)
        {
            int index = tempList[0];
            tempList.RemoveAt(0);
            curDist = Vector3.Distance(roomArray[pickB].room.transform.position, roomArray[index].room.transform.position);

            if (curDist > maxDist)
            {
                maxDist = curDist;
                pickS = index;
            }
        }
        AllStayingRooms.Add(pickS);
        count = 2;
        while(count < totalStayingRooms)
        {
            pick = (int)Random.Range(totalBossRooms, totalRooms);
            if (!AllStayingRooms.Contains(pick))
            {
                AllStayingRooms.Add(pick);
                count++;
                //Debug.Log(count);
            }
        }
        cleanArray();
        setupStageDone = true;
    }

    void cleanArray()
    {
        //List<int> tempList = SpawnRoomsList;
        int j = 0;
        for (int i = 0; i < totalRooms; i++)
        {
            if (AllStayingRooms.Contains(i))
            {
                stayingRoomArray[j] = roomArray[i];
                j++;
            }
            else
            {
                Destroy(roomArray[i].room);
            }
        }
        roomArray = null;
    }

    void Delaunay()
    {
        for (int i = 0; i < stayingRoomArray.Length; i++)
        {
            polygon.Add(new Vertex(stayingRoomArray[i].room.transform.position.x, stayingRoomArray[i].room.transform.position.y));
        }
        TriangleNet.Meshing.ConstraintOptions options = new TriangleNet.Meshing.ConstraintOptions() { Convex = false, ConformingDelaunay = false };
        //TriangleNet.Meshing.QualityOptions quality = new TriangleNet.Meshing.QualityOptions() { SteinerPoints = 0, MinimumAngle = 90 };
        //Debug.Log(polygon.Triangulate(options));
        mesh = (TriangleNet.Mesh)polygon.Triangulate(options);

        foreach (Edge edge in mesh.Edges)
        {
            Vertex v0 = mesh.vertices[edge.P0];
            Vertex v1 = mesh.vertices[edge.P1];
            Vector3 p0 = new Vector3((float)v0.x, (float)v0.y, 0.0f);
            Vector3 p1 = new Vector3((float)v1.x, (float)v1.y, 0.0f);
            Debug.Log("p0: " + p0 + " p1: " + p1);
        }

        DelaunayDone = true;
    }

    void find_MST()
    {
        MST spanningTree = new MST();

        spanningTree.SetV(AllStayingRooms.Count);

        int[,] graph = graph_Gen();

        int[,] ans = spanningTree.runMST(graph);

        //draws line connections
        for (int i = 1; i < AllStayingRooms.Count; i++)
        {

            Transform pos1 = stayingRoomArray[i].room.transform;
            Transform pos2 = stayingRoomArray[ans[i, 0]].room.transform;

            Debug.DrawLine(new Vector3(pos1.position.x, pos1.position.y, pos1.position.z),
                new Vector3(pos2.position.x, pos2.position.y, pos2.position.z), Color.green, 2000f, true);


        }
        createPath(ans);
        MSTDone = true;
    }

    void createPath(int[,] ans)
    {
        for (int i = 1; i < AllStayingRooms.Count; i++)
        {
            Transform pos1 = stayingRoomArray[i].room.transform;
            Transform pos2 = stayingRoomArray[ans[i, 0]].room.transform;
            Debug.Log("From: x: " + (int)pos1.transform.position.x + "y: " + (int)pos1.transform.position.y + "z: " + (int)pos1.transform.position.z);
            Debug.Log("TO: x: " + (int)pos2.transform.position.x + "y: " + (int)pos2.transform.position.y + "z: " + (int)pos2.transform.position.z);
            //child1.GetComponent<Tilemap>();
        }
    }

    int[,] graph_Gen()
    {

        //int length = AllStayingRooms.Count;
        int length = stayingRoomArray.Length;
        int[,] graph = new int[(length), (length)];
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < length; j++)
            {
                if (i == j)
                {
                    graph[i, j] = 0;
                    continue;
                }

                //GameObject child1 = stayingRoomArray[i].room.transform.GetChild(0).gameObject;
                //GameObject child2 = stayingRoomArray[j].room.transform.GetChild(0).gameObject;
                float x1 = stayingRoomArray[i].room.transform.position.x;
                float x2 = stayingRoomArray[j].room.transform.position.x;
                float y1 = stayingRoomArray[i].room.transform.position.y;
                float y2 = stayingRoomArray[j].room.transform.position.y;
                float calc = Mathf.Sqrt(((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1)));
                graph[i, j] = (int)calc;
            }

        }

        return graph;
    }

    public void OnDrawGizmos()
    {
        if (mesh == null)
        {
            // We're probably in the editor
            return;
        }

        
        Gizmos.color = Color.red;
        foreach (Edge edge in mesh.Edges)
        {
            Vertex v0 = mesh.vertices[edge.P0];
            Vertex v1 = mesh.vertices[edge.P1];
            Vector3 p0 = new Vector3((float)v0.x, (float)v0.y, 0.0f);
            Vector3 p1 = new Vector3((float)v1.x, (float)v1.y, 0.0f);
            Gizmos.DrawLine(p0, p1);
        }
    }

    void correctPosRooms()
    {
        
        //Debug.Log("still running");
        for (int i = 0; i < roomArray.Length; i++)
        {
            BoxCollider2D bc2D = roomArray[i].room.GetComponent<BoxCollider2D>();
            bc2D.enabled = !bc2D.enabled;
        }
        for (int i = 0; i < roomArray.Length; i++)
        {
            Vector3 tmp = roomArray[i].room.transform.position;
            tmp.x = Mathf.Round(tmp.x);
            tmp.y = Mathf.Round(tmp.y);
            roomArray[i].room.transform.position = tmp;
        }
        corrected = true;
    }

    bool awakeCheck()
    {
        Vector2 v2 = new Vector2(0, 0);
        for (int i = 0; i < roomArray.Length; i++)
        {
            Rigidbody2D rb = roomArray[i].room.GetComponent<Rigidbody2D>();
            //Debug.Log(rb.IsSleeping());
            
            if(rb.IsSleeping() == false)
            {
                startTime = Time.time;
                return true;
            }
            //stillAwake = false;
            
        }
        return false;
    }

    void drawOutline(Room_info[] rm)
    {
        for (int i = 0; i < rm.Length; i++)
        {
            float x, y;
            int width, height;
            width = rm[i].width;
            height = rm[i].height;

            x = rm[i].room.transform.position.x;
            y = rm[i].room.transform.position.y;

            Debug.DrawLine(new Vector3(x - (float)(width / 2), y - (float)(height / 2), 0), new Vector3(x + (float)(width / 2), y - (float)(height / 2), 0), Color.white, .02f, true);
            Debug.DrawLine(new Vector3(x + (float)(width / 2), y - (float)(height / 2), 0), new Vector3(x + (float)(width / 2), y + (float)(height / 2), 0), Color.white, .02f, true);
            Debug.DrawLine(new Vector3(x + (float)(width / 2), y + (float)(height / 2), 0), new Vector3(x - (float)(width / 2), y + (float)(height / 2), 0), Color.white, .02f, true);
            Debug.DrawLine(new Vector3(x - (float)(width / 2), y + (float)(height / 2), 0), new Vector3(x - (float)(width / 2), y - (float)(height / 2), 0), Color.white, .02f, true);

        }
    }
}
