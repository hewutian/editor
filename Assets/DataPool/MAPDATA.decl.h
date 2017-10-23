struct Vector3
{
	float x;
	float y;
	float z;
};

struct GameObject
{
	int instanceID;
};

struct CustomItemInfo
{
	int type;
    float posy;
    int lefttopsite;
    int width;
    int height;
    int isreachable;
    GameObject prefab;
};

struct NodeInfo
{
	int id;
	Vector3 site;
	string name;
};

struct AreaInfo
{
	Vector3 start;
	Vector3 end;
	string name;
	int id;
};

struct MAPDATA
{
	int mapwidth;
    int mapheight;
    int unitlength;
    int paintedgridlength;
    int tilelength;
    Vector3 dir;
    float max;
    GameObject scene;
    string prefabName;
    int hasGeneratedData;
    Vector3 center;

    //CustomItemInfo[] itemList = new CustomItemInfo[0];
    CustomItemInfo itemlist[];
    int unreachable[];
	NodeInfo designerNode[];
	AreaInfo designerArea[];
};

MAPDATA mapdata[];
#pragma import("MAPDATA.data.txt")
