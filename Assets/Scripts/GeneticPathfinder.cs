using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticPathfinder : MonoBehaviour
{
    public float creatureSpeed;
    public float pathMultiplier;
    public float roationSpeed;
    int pathIndex = 0;
    int backtrackedTimes = 1;
    public DNA dna;
    public bool hasFinished = false;
    public LayerMask obstacleLayer;
    bool hasBeenInit = false;
    bool hasCrashed = false;
    List<Vector2> travelledPath = new List<Vector2>();
    Vector2 target;
    Vector2 nextPoint;
    Quaternion targetRotation;
    LineRenderer lr;

    public void InitCreature(DNA newDna, Vector2 _target)
    {
        travelledPath.Add(transform.position);
        lr = GetComponent<LineRenderer>();
        dna = newDna;
        target = _target;
        nextPoint = transform.position;
        travelledPath.Add(nextPoint);
        hasBeenInit = true;
    }

    private void Update()
    {
        // Is the Creature still alive and not done
        if (hasBeenInit && !hasFinished)
        {
            // Reached end of genes or is within range of end
            if (pathIndex == dna.genes.Count || Vector2.Distance(transform.position, target) < 0.5f)
            {
                hasFinished = true;
            }
            // Next point needs to be updated
            if ((Vector2)transform.position == nextPoint)
            {
                nextPoint = (Vector2)transform.position + dna.genes[pathIndex] * pathMultiplier;
                
                // Checking to see if the creature has back tracked (not best solution still testing)
                for (int i = 0; i < travelledPath.Count; i++)
                {
                    if (Mathf.Abs(nextPoint.x - travelledPath[i].x) < 0.25 && Mathf.Abs(nextPoint.y - travelledPath[i].y) < 0.25)
                    {
                        backtrackedTimes++;
                    }
                }
                travelledPath.Add(nextPoint);
                targetRotation = LookAt2D(nextPoint);
                pathIndex++;
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, nextPoint, creatureSpeed * Time.deltaTime);
            }
            if (transform.rotation != targetRotation)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, roationSpeed * Time.deltaTime);
            }
            RenderLine();
        }
    }

    public void RenderLine()
    {
        List<Vector3> linePoints = new List<Vector3>();
        if (travelledPath.Count > 2)
        {
            for (int i = 0; i < travelledPath.Count - 1; i++)
            {
                linePoints.Add(travelledPath[i]);
            }
            linePoints.Add(transform.position);
        }
        else
        {
            linePoints.Add(travelledPath[0]);
            linePoints.Add(transform.position);
        }
        lr.positionCount = linePoints.Count;
        lr.SetPositions(linePoints.ToArray());
    }

    public float fitness
    {
        get
        {
            float dist = Vector2.Distance(transform.position, target);
            if (dist == 0)
            {
                // set to small value to avoid division by zero below
                dist = float.MinValue;
            }
            RaycastHit2D[] obstacles = Physics2D.RaycastAll(transform.position, target, obstacleLayer);
            float obstacleMultiplier = 1f - (0.1f * obstacles.Length);

            // formula for calculating fitness
            return ((60 / dist) * (hasCrashed ? 0.2f : 1f) * obstacleMultiplier) * (travelledPath.Count * 150 / backtrackedTimes);
        }
    }

    public Quaternion LookAt2D(Vector2 target, float angleoffset = -90)
    {
        Vector2 fromTo = (target - (Vector2)transform.position).normalized;
        float zRotation = Mathf.Atan2(fromTo.y, fromTo.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0, 0, zRotation + angleoffset);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Stops the creature when they crash into walls
        if (collision.gameObject.layer == 8)
        {
            hasFinished = true;
            hasCrashed = true;
        }
    }
}
