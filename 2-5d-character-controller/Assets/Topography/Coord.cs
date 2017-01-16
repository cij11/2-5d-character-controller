using UnityEngine;
using System.Collections;

public class Coord
{
	public int xCoord;
	public int yCoord;

	public int X
	{
		get
		{
			return this.xCoord;
		}
		set
		{
			this.xCoord = value;
		}
	}

	public int Y
	{
		get
		{
			return this.yCoord;
		}
		set
		{
			this.yCoord = value;
		}
	}

	public Coord(int x, int y){
		xCoord = x;
		yCoord = y;
	}

	public static Coord operator + (Coord coord1, Coord coord2){
		return new Coord (coord1.xCoord + coord2.xCoord, coord1.yCoord + coord2.yCoord);
	}
	public static Coord operator - (Coord coord1, Coord coord2){
		return new Coord (coord1.xCoord - coord2.xCoord, coord1.yCoord - coord2.yCoord);
	}

	public int DistSquared(Coord targetCoord){
		Coord difference = targetCoord - this;
		return difference.xCoord * difference.xCoord + difference.yCoord * difference.yCoord;
	}

	public float Dist(Coord targetCoord){
		Coord difference = targetCoord - this;
		return Mathf.Sqrt ((float)difference.X * (float)difference.X + (float)difference.Y * (float)difference.Y);
	}

	public int IninityNormDist(Coord targetCoord){
		Coord difference = targetCoord - this;
		return Mathf.Max (Mathf.Abs(difference.xCoord), Mathf.Abs(difference.yCoord));
	}

	public bool Equals(Coord targetCoord){
		if ((xCoord == targetCoord.xCoord) && (yCoord == targetCoord.yCoord))
			return true;
		else
			return false;
	}

	public Coord ConvertTileCoordToChunkCoord(int chunkSize){
		int xChunkCoord = Mathf.FloorToInt( ((float) xCoord)/ (float)chunkSize);
		int yChunkCoord = Mathf.FloorToInt (((float)yCoord) / (float)chunkSize);

		return new Coord (xChunkCoord, yChunkCoord);
	}

	public void Bound(Coord lowerBound, Coord upperBound){
		xCoord = Mathf.Max (lowerBound.X, xCoord);
		yCoord = Mathf.Max (lowerBound.Y, yCoord);
		xCoord = Mathf.Min (upperBound.X, xCoord);
		yCoord = Mathf.Min (upperBound.Y, yCoord);
	}

	public Coord Clone(){
		return new Coord (this.xCoord, this.yCoord);
	}
}