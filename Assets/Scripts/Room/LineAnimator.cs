using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LineAnimator: Singleton<LineAnimator>
{
    public bool IsAnimating = false;

    List<Edge> animatingEdges;
    bool IsEdgeCInitialized;
    bool IsPrependHalfAnimated = false;
    bool IsAppendHalfAnimated = false;

    public void AnimateEdges(List<Edge> edges, EdgeCollider2D edgeC)
    {
        animatingEdges = edges;
        IsAnimating = true;
            IsPrependHalfAnimated = false;
        IsAppendHalfAnimated = false;
        IsEdgeCInitialized = false;

        ToggleAnimationState(true);

        AnimateEdgesSequentially(edges, edgeC);
    }

    private void AnimateEdgesSequentially(List<Edge> edges, EdgeCollider2D edgeC)
    {
        // Calculate middle edge (or close to middle)
        int middleIndex = edges.Count / 2;

        // Split the list of edges into two parts
        List<Edge> edgesFromMiddleToEnd = edges.GetRange(middleIndex, edges.Count - middleIndex);
        List<Edge> edgesFromStartToMiddle = edges.GetRange(0, middleIndex);
        edgesFromStartToMiddle.Reverse(); // from middle to start

        // Animate edges from middle to end
        StartCoroutine(ExpandEdges(edgesFromMiddleToEnd, false, edgeC));

        // Animate edges from middle to start
        StartCoroutine(ExpandEdges(edgesFromStartToMiddle, true, edgeC));
    }

    private void addPointToEdgeC(Edge newEdge, bool IsPrepending, EdgeCollider2D edgeC)
    {
        Vector2[] longerArr = IsEdgeCInitialized ? new Vector2[edgeC.points.Length + 2] : new Vector2[2];

        if (IsPrepending)
        {
            if (newEdge.start == edgeC.points[0])
            {
                longerArr[0] = newEdge.worldEnd;
                longerArr[1] = newEdge.worldStart;
            }
            else
            {
                longerArr[0] = newEdge.worldStart;
                longerArr[1] = newEdge.worldEnd;
            }

            for (int i=2;i<longerArr.Length;i++)
            {
                longerArr[i] = edgeC.points[i - 2];
            }
        }
        else
        {
            if (newEdge.start == edgeC.points[0])
            {
                longerArr[longerArr.Length - 1] = newEdge.worldEnd;
                longerArr[longerArr.Length - 2] = newEdge.worldStart;
            }
            else
            {
                longerArr[longerArr.Length - 1] = newEdge.worldStart;
                longerArr[longerArr.Length - 2] = newEdge.worldEnd;
            }

            for (int i=0;i<longerArr.Length - 2; i++)
            {
                longerArr[i] = edgeC.points[i];
            }
        }

        edgeC.points = longerArr;
        IsEdgeCInitialized = true;
    }

    private void ToggleAnimationState(bool IsAnimating)
    {
        animatingEdges.ForEach((edge) =>
        {
            edge.IsAnimating = IsAnimating;
        });
    }

    private IEnumerator ExpandEdges(List<Edge> edges, bool IsPrepending, EdgeCollider2D edgeC)
    {
        foreach (Edge edge in edges)
        {
            if (edgeC != null)
                addPointToEdgeC(edge, IsPrepending, edgeC);

            edge.ToggleLine(true);
            //Debug.Log("Turn on edge with id " + edge.id + " is visible " + edge.IsVisible());
            yield return new WaitForSeconds(Consts.expandDuration);
        }

        if (edgeC != null)
            EdgeMaker.AddEdgeCollider(edgeC, edges);

        if (IsPrepending)
        {
            IsPrependHalfAnimated = true;
        }
        if (!IsPrepending)
        {
            IsAppendHalfAnimated = true;
        }

        if (IsAppendHalfAnimated && IsPrependHalfAnimated)
        {
            ToggleAnimationState(false);
            IsAnimating = false;
        }
    }
}
