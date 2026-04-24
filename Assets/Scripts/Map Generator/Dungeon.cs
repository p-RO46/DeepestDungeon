public class Dungeon
{
    public MapType type;
    public bool left_door;
    public bool right_door;
    public bool down_door;
    public bool up_door;

    public int R;
    public int C;

    public Dungeon(MapType _type, int _row, int _col)
    {
        type = _type;
        left_door = false;
        right_door = false;
        down_door = false;
        up_door = false;

        R = _row;
        C = _col;
    }
}
