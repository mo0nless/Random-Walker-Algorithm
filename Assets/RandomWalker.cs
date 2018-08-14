using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


/*
 * https://medium.freecodecamp.org/how-to-make-your-own-procedural-dungeon-map-generator-using-the-random-walk-algorithm-e0085c8aa9a
 * 
 *  Next, let’s go through the map generation algorithm to see how it:

    1 - Makes a two dimensional map of walls
    2 - Chooses a random starting point on the map
    3 - While the number of tunnels is not zero
    4 - Chooses a random length from maximum allowed length
    5 - Chooses a random direction to turn to (right, left, up, down)
    6 - Draws a tunnel in that direction while avoiding the edges of the map
    7 - Decrements the number of tunnels and repeats the while loop
    8 - Returns the map with the changes

 *  This loop continues until the number of tunnels is zero.
 */

public class RandomWalker : MonoBehaviour
{

    // get the selected prefab
    public Transform pointPrefab;
    public int dimensions = 5; // width and height of the map
    public float maxTunnels = 3; // max number of tunnels possible
    public float maxLength = 3; // max length each tunnel can have
    public float distanceRange = 6;
    List<Transform> points;

    // once play button is clicked Instantiate the prefab
    void Awake()
    {
        //Instantiate(pointPrefab);
        CreateMap();
    }

    // create the array that defines
    int[,] CreateArray(int num, int dimensions)
    {
        int[,] array = new int[dimensions, dimensions];

        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                array[i, j] = num;
            }
        }
        return array;
    }

    public double GetRandomeNumber(double minimum, double maximum)
    {
        System.Random random = new System.Random();
        return random.NextDouble() * (maximum - minimum) + minimum;
    }

    void CreateMap()
    {
        int[,] map = CreateArray(1, dimensions);
        int currentRow = (int)Math.Round(GetRandomeNumber(0.0, 0.9) * (double)dimensions); // our current row - start at a random spot
        int currentColumn = (int)Math.Round(GetRandomeNumber(0.0, 0.9) * (double)dimensions); // our current column - start at a random spot
        List<int[]> directions = new List<int[]> { new int[] { -1, 0 }, new int[] { 1, 0 }, new int[] { 0, -1 }, new int[] { 0, 1 } }; // array to get a random direction from (left,right,up,down)
        List<int[]> lastDirection = new List<int[]> { new int[] { 0, 0 } };//(); // save the last direction we went
        int[] randomDirection; // next turn/direction - holds a value from directions

        // lets create some tunnels - while maxTunnels, dimentions, and maxLength  is greater than 0.
        while ((maxTunnels > 0) && (dimensions > 0) && (maxLength > 0))
        {
            // lets get a random direction - until it is a perpendicular to our lastDirection
            // if the last direction = left or right,
            // then our new direction has to be up or down,
            // and vice versa
            do
            {
                int choice = (int)Math.Round(GetRandomeNumber(0.0, 1.0) * (double)(directions.Count - 1));
                randomDirection = directions[choice];
            } while ((randomDirection[0] == -lastDirection[0][0] && randomDirection[1] == -lastDirection[0][1]) ||
                    (randomDirection[0] == lastDirection[0][0] && randomDirection[1] == lastDirection[0][1]));

            // After choosing a randomDirection that satisfies the conditions, set a variable to randomly choose a length from maxLength. 
            // Set tunnelLength variable to zero to server as an iterator.
            int randomLength = (int)Math.Ceiling(GetRandomeNumber(0.0, 0.9) * (double)maxLength); //length the next tunnel will be (max of maxLength)
            int tunnelLength = 0; //current length of tunnel being created

            // Make a tunnel by turning the value of cells from one to zero while the tunnelLength is smaller than randomLength. 
            // If within the loop the tunnel hits the edges of the map, the loop should break.
            while (tunnelLength < randomLength)
            {

                //break the loop if it is going out of the map
                if (((currentRow == 0) && (randomDirection[0] == -1)) ||
                    ((currentColumn == 0) && (randomDirection[1] == -1)) ||
                    ((currentRow == dimensions - 1) && (randomDirection[0] == 1)) ||
                    ((currentColumn == dimensions - 1) && (randomDirection[1] == 1)))
                {
                    break;
                }
                else
                {
                    map[currentRow, currentColumn] = 0; //set the value of the index in map to 0 (a tunnel, making it one longer)
                    currentRow += randomDirection[0]; //add the value from randomDirection to row and col (-1, 0, or 1) to update our location
                    currentColumn += randomDirection[1];
                    tunnelLength++; //the tunnel is now one longer, so lets increment that variable
                }
            }

            if (tunnelLength > 0)
            { // update our variables unless our last loop broke before we made any part of a tunnel
                lastDirection.Clear();
                lastDirection.Add(randomDirection); //set lastDirection, so we can remember what way we went
                maxTunnels--; // we created a whole tunnel so lets decrement how many we have left to create
            }
        }

        float step = distanceRange / dimensions;
        points = new List<Transform>();

        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                if (map[i, j] == 0)
                {
                    Vector3 scale = Vector3.one * step;
                    Vector3 position;
                    //System.Random random = new System.Random();

                    Transform point = Instantiate(pointPrefab);
                    position.x = (i + 0.5f) * step - 1f;
                    position.y = 0;
                    position.z = (j + 0.5f) * step - 1f;

                    point.localPosition = position;
                    point.localScale = scale;

                    point.SetParent(transform, false);
                    points.Add(point);
                }
            }
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < points.Count; i++)
        {
            Transform point = points[i];
            Vector3 position = point.localPosition;
            position.y = Mathf.Sin(Mathf.PI * (position.z + Time.time));
            point.localPosition = position;
        }
    }
}
