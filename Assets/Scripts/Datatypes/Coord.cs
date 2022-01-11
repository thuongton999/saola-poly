using UnityEngine;

// Replacement for Vector2Int, which was causing slowdowns in big loops due to x,y accessor overhead
[System.Serializable]
public struct Coord {

    public int x;
    public int y;

    public Coord (int x, int y) {
        this.x = x;
        this.y = y;
    }

    public static int SqrDistance (Coord a, Coord b) => (a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y);
    public static float Distance (Coord a, Coord b) => (float) System.Math.Sqrt (SqrDistance (a, b));
    public static bool AreNeighbours (Coord a, Coord b) => System.Math.Abs (a.x - b.x) <= 1 && System.Math.Abs (a.y - b.y) <= 1;

    public static Coord invalid => new Coord (-1, -1);
    public static Coord up => new Coord (0, 1);
    public static Coord down => new Coord(0, -1);
    public static Coord left => new Coord (-1, 0);

    public static Coord right => new Coord (1, 0);

    public static Coord operator + (Coord a, Coord b) => new Coord (a.x + b.x, a.y + b.y);
    public static Coord operator - (Coord a, Coord b) => new Coord (a.x - b.x, a.y - b.y);
    public static bool operator == (Coord a, Coord b) => a.x == b.x && a.y == b.y;
    public static bool operator != (Coord a, Coord b) => a.x != b.x || a.y != b.y;
    public static implicit operator Vector2 (Coord v) => new Vector2 (v.x, v.y);
    public static implicit operator Vector3 (Coord v) => new Vector3 (v.x, 0, v.y);

    public override bool Equals (object other) => (Coord) other == this;
    public override int GetHashCode () => x ^ y;
    public override string ToString () => "(" + x + " ; " + y + ")";
}