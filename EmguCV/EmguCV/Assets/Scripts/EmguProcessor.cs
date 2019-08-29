using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Emgu.CV;
using Emgu.CV.UI;
using Emgu.CV.Util;
using Emgu.CV.Structure;
using System.Threading.Tasks;
using System;
using System.Drawing;
using System.Linq;
public delegate void RGBDelegate(ref byte r, ref byte g, ref byte b, int x, int y, int[] args);
public class EmguProcessor : MonoBehaviour
{
    const int INTENSITY_BLACK = 0;
    //204, 196, 165
    
    bool running;
    byte[,,] currentData;
    Image<Bgr, byte> img;
    int l0, l1, l2;
    

    public void Init(Image<Bgr, byte> img)
    {
        this.img = img;
        this.currentData = img.Data;
        this.l0 = currentData.GetLength(0);
        this.l1 = currentData.GetLength(1);
        this.l2 = currentData.GetLength(2);
    }

    /// <summary>
    /// set all channels for current color data to 0 except the blue channel
    /// </summary>
    /// <param name="blueThreshold"></param>
    public void IsolateBlue()
    {
        ForEachPixel(isolateBlue);
    }

    private void isolateBlue(ref byte r, ref byte g, ref byte b, int x, int y, int[] args)
    {
        r = 0;
        g = 0;
    }

    public void RemoveColor(Vector3Int colorVec, int t)
    {
        int[] args = new int[]
        {
            t,
            colorVec.x,
            colorVec.y,
            colorVec.z
        };
        ForEachPixel(removeColor, args);
    }

    private void removeColor(ref byte r, ref byte g, ref byte b, int x, int y, int[] args)
    {
        int r1 = Math.Abs(r - args[1]);
        int g1 = Math.Abs(g - args[2]);
        int b1 = Math.Abs(b - args[3]);
        int t = args[0];
        if (r1 < t && g1 < t && b1 < t)
        {
            r = 0;
            g = 0;
            b = 0;
        }
    }
    /*   Routine to find top left point
     * 
     * Pipeline:
     *  - Floodfill image from top left with black to find borders
     *  - Find candidate points
     *  - Floodfill from candidate points 
     *  - Select point where floodfill lasts the longest.
     * 
     * -> For now shelved because not performant enough
     * -> Should be optimizable with bucket based approach
     * */
    internal void FindTopLeftPointByFloodfill()
    {
        HashSet<Vector2Int> candidates = new HashSet<Vector2Int>();

        //Step 1
        CVFloodfill(7);

        /*  Step 2
         *  Get all candidate pixels where layout is:
         *  b = black pixel
         *  C = candidate
         *  x = non-black pixel
         *  
         *  b b b b
         *  b b c x
         *  b b x x
         * 
         * */
        for (int y = 0; y < l1; y++)
        {
            for (int x = 0; x < l0; x++)
            {
                //if (x > 1000 || y > 1000)
                //    continue;
                int intensity = GetIntensity(x, y);
                //if pixel is exactly black (was set by first floodfill)
                if (intensity == INTENSITY_BLACK)
                {
                    
                    TryAddCandidateForTopLeftPoint(candidates, x, y);
                }
            }
        }
        Debug.Log("Found " + candidates.Count + " candidates");
        Dictionary<Vector2Int, FloodfillPointBucket>pointToBucket = new Dictionary<Vector2Int, FloodfillPointBucket>();
        foreach (Vector2Int candidate in candidates)
        {
            pointToBucket.Add(candidate, new FloodfillPointBucket(candidate));
        }
        Debug.Log(candidates.Count);

        //Step 3
        int safety = 0;
        while (candidates.Count > 1)
        {
            HashSet<Vector2Int> toRemove = new HashSet<Vector2Int>();
            foreach (Vector2Int candidate in candidates)
            {
                safety++;
                if (pointToBucket.ContainsKey(candidate))
                {
                    if (!TickFloodfill(toRemove, pointToBucket[candidate], pointToBucket))
                        break;
                }
                //problem pointToBucket got removed but candidate didn't get removed yet.

            }
            Debug.Log(candidates.Count);

            foreach (Vector2Int remove in toRemove)
                candidates.Remove(remove);
            //weird! still 3000 buckets after 15m pixels have been iterated... should have converged since picture is only ~15m pixels in total!
            if (safety > 500000)
            {
                Debug.Log(candidates.Count);
                break;
            }
        }

        Vector2Int result = candidates.First();
        Debug.Log(result);
    }

    private bool TickFloodfill(HashSet<Vector2Int> toRemove, FloodfillPointBucket bucket, Dictionary<Vector2Int, FloodfillPointBucket> pointToBucket)
    {
        if (bucket.nextQueue.Count == 0)
        {
            toRemove.Add(bucket.initialPos);
            return false;
        }
        Vector2Int curr = bucket.nextQueue.Dequeue();
        if (pointToBucket.ContainsKey(curr) &&
            pointToBucket[curr] != bucket)
        {
            //handle merging
            Debug.Log("merge: " + bucket.initialPos + " with " + pointToBucket[curr].initialPos);
            Vector2Int obsoleteBucketIndex = MergeBuckets(bucket, pointToBucket[curr]);
            toRemove.Add(obsoleteBucketIndex);
            pointToBucket[curr] = bucket;
            return false;
        }
        FloodfillStepNotBlack(
        curr.x,
        curr.y,
        bucket.ogPos.x,
        bucket.ogPos.y,
        l0,
        l1,
        bucket.visitedPoints,
        bucket.nextQueue);

        if (!pointToBucket.ContainsKey(curr))
            pointToBucket.Add(curr, bucket);

        return true;

    }
    /* Routine to find top left point by a simple sweepline algorithm.
     * 
     * First floodfill from the outside of the map (floodfill stops at border)
     * then count non filled pixels -> if more than 80% of pixels in a line are not filled take that point as top left point of the map
     * 
     * problem: outer border has invalid pixels because of scan inaccuracy
     * -> optimize parameters...
     * Foodfill 10; Threshold .2f starts to work
     * 
     * .15 finds correct y coordinate. x coordinate too far.
     * -> .22 almost correct x coordinate
     * -> .25 wrong x coordinate because of inaccuracy.
     * 
     * Problem: 3 parameters to optimize. not very reliable
     * */
    public void FindTopLeftPointBySweepline()
    {
        CVFloodfill(10);

        Vector2Int result = new Vector2Int(l0, l1);
        for (int y = 1; y < l1; y++)
        {
            int rowCount = 0;
            for (int x = 0; x < l0; x++)
            {
                if (GetIntensity(x,y) == INTENSITY_BLACK)
                {
                    rowCount++;
                }
            }

            if ((float)rowCount / (float)l0 < .23f)
            {
                result.y = y;
                break;
            }
        }
        for (int x = 1; x < l0; x++)
        {
            int columnCount = 0;
            for (int y = 0; y < l1; y++)
            {
                if (GetIntensity(x, y) == INTENSITY_BLACK)
                {
                    columnCount++;
                }
            }

            if ((float)columnCount / (float)l1 < .15f)
            {
                result.x = x;
                break;
            }
        }

        currentData[result.x, result.y, 0] = 0;
        currentData[result.x, result.y, 1] = 0;
        currentData[result.x, result.y, 2] = 255;
        Debug.Log(result);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bucketA"></param>
    /// <param name="bucketB"></param>
    /// <returns>The obsolete bucket index (initial position of the bucket which got merged)</returns>
    private Vector2Int MergeBuckets(FloodfillPointBucket bucketA, FloodfillPointBucket bucketB)
    {
        bucketA.MergeWith(bucketB);
        return bucketB.initialPos;
    }

    private int GetIntensity(int x, int y)
    {
        return currentData[x, y, 0] + currentData[x, y, 1] + currentData[x, y, 2];
    }

    private void TryAddCandidateForTopLeftPoint(HashSet<Vector2Int> candidates, int x, int y)
    {
        Vector2Int candidate = new Vector2Int(x + 1, y + 1);
        Vector2Int neighbourA = new Vector2Int(x, y + 1);
        Vector2Int neighbourB = new Vector2Int(x + 1, y);
        if (PixelInBounds(ref candidate))
        {
            int intensityCandidate = GetIntensity(candidate.x, candidate.y);
            if (intensityCandidate != INTENSITY_BLACK)
            {
                int intensityNeighbourA = GetIntensity(neighbourA.x, neighbourA.y);
                int intensityNeighbourB = GetIntensity(neighbourB.x, neighbourB.y);
                if (intensityNeighbourA == INTENSITY_BLACK &&
                    intensityNeighbourB == INTENSITY_BLACK)
                {
                    candidates.Add(candidate);
                    currentData[candidate.x, candidate.y, 0] = 0;
                    currentData[candidate.x, candidate.y, 1] = 0;
                    currentData[candidate.x, candidate.y, 2] = 255;
                }
            }
        }
    }

    public void CVFloodfill(int cutoff)
    {
        Rectangle rect = new Rectangle();
        CvInvoke.FloodFill(img, null, new Point(18, 18), new MCvScalar(0, 0, 0), out rect, new MCvScalar(cutoff, cutoff, cutoff), new MCvScalar(cutoff, cutoff, cutoff));
    }
    internal void DrawGrid(int cellSize, int lineWidth)
    {
        lineWidth = (int)Mathf.Clamp(lineWidth, 0f, cellSize);
        for (int y = 0; y < l1; y += cellSize)
        {
            for (int x = 0; x < l0; x += cellSize)
            {
                for (int w = 0; w + x < l0 && w < cellSize; w++)
                {
                    for (int q = 0; q + y < l1 && q < lineWidth; q++)
                    {
                        currentData[x + w, y + q, 0] = 0;
                        currentData[x + w, y + q, 1] = 255;
                        currentData[x + w, y + q, 2] = 0;
                    }                    
                }
                for (int q = 0; q + y < l1 && q < cellSize; q++)
                {
                    for (int w = 0; w + x < l0 && w < lineWidth; w++)
                    {
                        currentData[x + w, y + q, 0] = 0;
                        currentData[x + w, y + q, 1] = 255;
                        currentData[x + w, y + q, 2] = 0;
                    }
                }
            }
        }
    }

    public void CustomFloodfill(int x, int y, ref Vector3Int offsetTargetRGB, ref Vector3Int newColRGB, ref HashSet<int> visited, ref double counter)
    {
        counter++;
        Queue<Vector2Int> nodes = new Queue<Vector2Int>();
        nodes.Enqueue(new Vector2Int(x, y));

        while (nodes.Count > 0)
        {
            Vector2Int curr = nodes.Dequeue();
            x = curr.x;
            y = curr.y;
            FloodfillStepNextColorDistance(
                x, 
                y,
                0,
                0,
                l0,
                l1,
                ref offsetTargetRGB, 
                visited, 
                nodes);
            visited.Add(l0 * y + x);
            currentData[x, y, 0] = (byte)newColRGB.z;
            currentData[x, y, 1] = (byte)newColRGB.y;
            currentData[x, y, 2] = (byte)newColRGB.x;
        }


        //Recursive Flood-Fill -> does not work in c#
        //if (x - 1 >= 0 &&
        //    IsInColorDistance(ref offsetTargetRGB, x, y, x - 1, y) &&
        //    !visited.Contains(l0 * y + x - 1))
        //{
        //    CustomFloodfill(x - 1, y, ref offsetTargetRGB, ref newColRGB, ref visited, ref counter);
        //}
        //if (x + 1 < l0 &&
        //    IsInColorDistance(ref offsetTargetRGB, x, y, x + 1, y) &&
        //    !visited.Contains(l0 * y + x + 1))
        //{
        //    CustomFloodfill(x + 1, y, ref offsetTargetRGB, ref newColRGB, ref visited, ref counter);
        //}
        //if (y - 1 >= 0 &&
        //    IsInColorDistance(ref offsetTargetRGB, x, y, x, y - 1) &&
        //    !visited.Contains(l0 * (y - 1) + x))
        //{
        //    CustomFloodfill(x, y - 1, ref offsetTargetRGB, ref newColRGB, ref visited, ref counter);
        //}
        //if (y + 1 < l1 &&
        //    IsInColorDistance(ref offsetTargetRGB, x, y, x, y + 1) &&
        //    !visited.Contains(l0 * (y + 1) + x))
        //{
        //    CustomFloodfill(x, y + 1, ref offsetTargetRGB, ref newColRGB, ref visited, ref counter);
        //}
        //    currentData[x, y, 0] = (byte)newColRGB.z;
        //    currentData[x, y, 1] = (byte)newColRGB.y;
        //    currentData[x, y, 2] = (byte)newColRGB.x;

    }

    private void FloodfillStepNextColorDistance(
        int x, 
        int y, 
        int minX,
        int minY,
        int maxX,
        int maxY,
        ref Vector3Int offsetTargetRGB, 
        HashSet<int> visited, 
        Queue<Vector2Int> nodes)
    {
        if (x - 1 >= minX &&
            IsInColorDistance(ref offsetTargetRGB, x, y, x - 1, y) &&
            !visited.Contains(l0 * y + x - 1))
        {
            nodes.Enqueue(new Vector2Int(x - 1, y));
        }
        if (x + 1 < maxX &&
            IsInColorDistance(ref offsetTargetRGB, x, y, x + 1, y) &&
            !visited.Contains(l0 * y + x + 1))
        {
            nodes.Enqueue(new Vector2Int(x + 1, y));
        }
        if (y - 1 >= minY &&
            IsInColorDistance(ref offsetTargetRGB, x, y, x, y - 1) &&
            !visited.Contains(l0 * (y - 1) + x))
        {
            nodes.Enqueue(new Vector2Int(x, y - 1));
        }
        if (y + 1 < maxY &&
            IsInColorDistance(ref offsetTargetRGB, x, y, x, y + 1) &&
            !visited.Contains(l0 * (y + 1) + x))
        {
            nodes.Enqueue(new Vector2Int(x, y + 1));
        }
    }
    private void FloodfillStepNotBlack(
            int x,
            int y,
            int minX,
            int minY,
            int maxX,
            int maxY,
            HashSet<Vector2Int> visited,
            Queue<Vector2Int> nodes)
    {
        if (x - 1 >= minX &&
            GetIntensity(x - 1, y) != INTENSITY_BLACK &&
            !visited.Contains(new Vector2Int(x - 1,y)))
        {
            nodes.Enqueue(new Vector2Int(x - 1, y));
        }
        if (x + 1 < maxX &&
            GetIntensity(x + 1, y) != INTENSITY_BLACK &&
            !visited.Contains(new Vector2Int(x + 1, y)))
        {
            nodes.Enqueue(new Vector2Int(x + 1, y));
        }
        if (y - 1 >= minY &&
            GetIntensity(x, y - 1) != INTENSITY_BLACK &&
            !visited.Contains(new Vector2Int(x, y - 1)))
        {
            nodes.Enqueue(new Vector2Int(x, y - 1));
        }
        if (y + 1 < maxY &&
            GetIntensity(x, y + 1) != INTENSITY_BLACK &&
            !visited.Contains(new Vector2Int(x, y + 1)))
        {
            nodes.Enqueue(new Vector2Int(x, y + 1));
        }
    }
    private bool IsInColorDistance(ref Vector3Int offsetTargetRGB, int ogX, int ogY, int x, int y)
    {
        Vector3Int delta = new Vector3Int(
            Math.Abs(currentData[ogX, ogY, 2] - currentData[x, y, 2]),
            Math.Abs(currentData[ogX, ogY, 1] - currentData[x, y, 1]),
            Math.Abs(currentData[ogX, ogY, 0] - currentData[x, y, 0]));

        return delta.magnitude < offsetTargetRGB.magnitude;
    }

    public Texture2D GetTex()
    {
        
        Texture2D tex = new Texture2D(img.Width, img.Height);
        for (int y = 0; y < l1; y++)    
        {
            for (int x = 0; x < l0; x++)
            {
                UnityEngine.Color col = new UnityEngine.Color(
                    currentData[x, y, 2] / 255.0f,
                    currentData[x, y, 1] / 255.0f,
                    currentData[x, y, 0] / 255.0f);
                tex.SetPixel(y, l0 - x - 1, col);
            }
        }
        tex.Apply();
        return tex;
    }

    public float DistanceRGB(byte[,,] referenceData)
    {
        float output = 0f;
        int tmp = 0;
        for (int y = l1 - 1; y >= 0; y--)
        {
            for (int x = 0; x < l0; x++)
            {
                tmp += currentData[x, y, 2] - referenceData[x, y, 2];
                tmp += currentData[x, y, 1] - referenceData[x, y, 1];
                tmp += currentData[x, y, 0] - referenceData[x, y, 0];
               
            }
        }
        output = tmp / 255.0f;
        return output;
    }
    /// <summary>
    /// Removes any  pixels surrounded by black pixels up to deltaMax by 
    /// </summary>
    /// <param name="t"></param>
    /// <param name="epsilon"></param>
    /// <param name="deltaMax"></param>
    public void RemoveNoiseNextToBlack(float t, float epsilon, byte maxLuminosity)
    {
        float currt = 0.0f;
        int lastSum = 0;
        for (int y = l1 - 1; y >= 0; y--)
        {
            for (int x = 0; x < l0; x++)
            {
                if (currentData[x, y, 0] == 0 &&
                    currentData[x, y, 1] == 0 &&
                    currentData[x, y, 2] == 0)
                    currt = 1.0f;
                int sum = (currentData[x, y, 0] + currentData[x, y, 1] + currentData[x, y, 2]);
                //if (Math.Abs(sum - lastSum) > maxLuminosity * 3)
                //    continue;
                if (currt > epsilon)
                {
                    currentData[x, y, 0] = 0;
                    currentData[x, y, 1] = 0;
                    currentData[x, y, 2] = 0;
                }
                currt = Mathf.Lerp(currt, 0f, t);
                lastSum = sum;
            }
        }
    }
    public void ForEachPixel(RGBDelegate rgbCallback, int[] args = null)
    {
        for (int y = l1 - 1; y >= 0; y--)
        {
            for (int x = 0; x < l0; x++)
            {
                rgbCallback.Invoke(
                    ref currentData[x, y, 2],
                    ref currentData[x, y, 1],
                    ref currentData[x, y, 0],
                    x,
                    y,
                    args);

            }
        }
    }

    public float NeighbourDist(byte[,,] referenceData)
    {
        float score = 0f;
        for (int y = l1 - 1; y >= 0; y--)
        {
            for (int x = 0; x < l0; x++)
            {
                int topIndex = y - 1;
                int rightIndex = x + 1;
                int bottomIndex = y + 1;
                int leftIndex = x - 1;

                if (topIndex >= 0)
                    score += MeasureDistance(currentData[x,topIndex,2], referenceData[x, topIndex, 2]);
                if (rightIndex < l0)
                    score += MeasureDistance(currentData[rightIndex, y, 2], referenceData[rightIndex, y, 2]);
                if (bottomIndex < l1)
                    score += MeasureDistance(currentData[x, bottomIndex, 2], referenceData[x, bottomIndex, 2]);
                if (leftIndex >= 0)
                    score += MeasureDistance(currentData[leftIndex, y, 2], referenceData[leftIndex, y, 2]);
            }
        }
        return score;
    }

    private float MeasureDistance(byte v1, byte v2)
    {
        if (v1 == v2)
            return 1.0f;
        else
            return -1.0f;
    }

    public bool TryGetPixelRGB (ref Vector2Int pixel, out Vector3Int color)
    {
        color = new Vector3Int(0, 0, 0);
        if (!PixelInBounds(ref pixel))
            return false;
        color = new Vector3Int(
            currentData[pixel.x, pixel.y, 2],
            currentData[pixel.x, pixel.y, 1],
            currentData[pixel.x, pixel.y, 0]);
        return true;
    }
    public bool PixelInBounds(ref Vector2Int pixel)
    {
        if (pixel.x >= l0 ||
             pixel.x < 0 ||
             pixel.y >= l1 ||
             pixel.y < 0)
            return false;

        return true;
    }

}

public class FloodfillPointBucket
{
    public Vector2Int ogPos
    {
        get; private set;
    }
    public Vector2Int currPos;
    public Vector2Int initialPos
    {
        get; private set;
    }
    public HashSet<Vector2Int> visitedPoints;
    public Queue<Vector2Int> nextQueue;

    public FloodfillPointBucket (Vector2Int topLeft)
    {
        this.ogPos = topLeft;
        this.initialPos = topLeft;
        visitedPoints = new HashSet<Vector2Int>();
        nextQueue = new Queue<Vector2Int>();
        nextQueue.Enqueue(topLeft);
    }
    //no better idea for this right now
    /*
     * Problem is: I need to have seperate queues, because candidates may be in enclosed spaces which should get discarded
     * However, candidates may also be in the same open space and need to get merged 
     * -> open and closed spaces can never have their queues mixed
     * */
    internal void MergeWith(FloodfillPointBucket bucketB)
    {
        Vector2Int newOg = new Vector2Int(Math.Min(this.ogPos.x, bucketB.ogPos.x), Math.Min(this.ogPos.y, bucketB.ogPos.y));
        
        visitedPoints.UnionWith(bucketB.visitedPoints);
        while (bucketB.nextQueue.Count > 0)
        {
            Vector2Int transferCandidate = bucketB.nextQueue.Dequeue();
            if (!visitedPoints.Contains(transferCandidate))
                nextQueue.Enqueue(transferCandidate);
        }
    }
}
