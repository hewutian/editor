struct Vector3
{
	float x;
	float y;
	float z;
};

struct CustomItemInfo
{
   
    int lefttopsite;
	int tid;
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
    string prefabName;
	int tid;
    Vector3 center;

    //CustomItemInfo[] itemList = new CustomItemInfo[0];
    CustomItemInfo itemlist[];
    int unreachable[];
	NodeInfo designerNode[];
	AreaInfo designerArea[];
};

MAPDATA mapdata[];
#pragma import("MAPDATA.data.txt")
