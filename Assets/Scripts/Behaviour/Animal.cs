using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : LivingEntity {

    public const int maxViewDistance = 10;

    [EnumFlags]
    public Species diet;

    public CreatureAction currentAction;
    public Genes genes;
    public Color maleColour;
    public Color femaleColour;

    // Settings:
    float timeBetweenActionChoices = 1;
    float moveSpeed = 1.5f;
    float timeToDeathByHunger = 200;
    float timeToDeathByThirst = 200;

    public float drinkDuration { get; private set; } = 6;
    public float eatDuration { get; private set; } = 10;

    float criticalPercent = 0.7f;

    // Visual settings:
    float moveArcHeight = .2f;

    // State:
    [Header ("State")]
    public float hunger;
    public float thirst;

    protected LivingEntity foodTarget;
    protected Coord waterTarget;

    // Move data:
    bool animatingMovement;
    Coord moveFromCoord;
    Coord moveTargetCoord;
    Vector3 moveStartPos;
    Vector3 moveTargetPos;
    float moveTime;
    float moveSpeedFactor;
    float moveArcHeightFactor;
    Coord[] path;
    int pathIndex;

    // Other
    float lastActionChooseTime;
    const float sqrtTwo = 1.4142f;
    const float oneOverSqrtTwo = 1 / sqrtTwo;

    public override void Init (Coord coord) {
        base.Init (coord);
        moveFromCoord = coord;
        genes = Genes.RandomGenes (1);

        material.color = (genes.isMale) ? maleColour : femaleColour;

        ChooseNextAction ();
    }

    protected virtual void Update () {

        // Increase hunger and thirst over time
        hunger += Time.deltaTime * 1 / timeToDeathByHunger;
        thirst += Time.deltaTime * 1 / timeToDeathByThirst;

        // Animate movement. After moving a single tile, the animal will be able to choose its next action
        if (animatingMovement) {
            AnimateMove ();
        } else {
            // Handle interactions with external things, like food, water, mates
            HandleInteractions ();
            float timeSinceLastActionChoice = Time.time - lastActionChooseTime;
            if (timeSinceLastActionChoice > timeBetweenActionChoices) {
                ChooseNextAction ();
            }
        }

        if (hunger >= 1) {
            Die (CauseOfDeath.Hunger);
        } else if (thirst >= 1) {
            Die (CauseOfDeath.Thirst);
        }
    }

    // Animals choose their next action after each movement step (1 tile),
    // or, when not moving (e.g interacting with food etc), at a fixed time interval
    protected virtual void ChooseNextAction () {
        lastActionChooseTime = Time.time;
        // Get info about surroundings

        // Decide next action:
        // Eat if (more hungry than thirsty) or (currently eating and not critically thirsty)
        bool currentlyEating = currentAction == CreatureAction.Eating && foodTarget && hunger > 0;
        if (hunger >= thirst || currentlyEating && thirst < criticalPercent) {
            FindFood ();
        }
        // More thirsty than hungry
        else {
            FindWater ();
        }

        Act ();

    }

    protected virtual void FindFood () {
        LivingEntity foodSource = Environment.SenseFood (coord, this, FoodPreferencePenalty);
        if (foodSource) {
            currentAction = CreatureAction.GoingToFood;
            foodTarget = foodSource;
            CreatePath (foodTarget.coord);

        } else {
            currentAction = CreatureAction.Exploring;
        }
    }

    protected virtual void FindWater () {
        Coord waterTile = Environment.SenseWater (coord);
        if (waterTile != Coord.invalid) {
            currentAction = CreatureAction.GoingToWater;
            waterTarget = waterTile;
            CreatePath (waterTarget);

        } else {
            currentAction = CreatureAction.Exploring;
        }
    }

    // When choosing from multiple food sources, the one with the lowest penalty will be selected
    protected virtual int FoodPreferencePenalty (LivingEntity self, LivingEntity food) {
        return Coord.SqrDistance (self.coord, food.coord);
    }

    protected void GoingToDo(Coord target, CreatureAction thenDoing) {
        if (Coord.AreNeighbours (coord, target)) {
            LookAt (target);
            currentAction = thenDoing;
        } else {
            StartMoveToCoord (path[pathIndex]);
            pathIndex++;
        }        
    }

    protected void Act () {
        switch (currentAction) {
            case CreatureAction.Exploring:
                StartMoveToCoord (Environment.GetNextTileWeighted (coord, moveFromCoord));
                break;
            case CreatureAction.GoingToFood:
                GoingToDo (foodTarget.coord, CreatureAction.Eating);
                break;
            case CreatureAction.GoingToWater:
                GoingToDo (waterTarget, CreatureAction.Drinking);
                break;
        }
    }

    protected void CreatePath (Coord target) {
        // Create new path if current is not already going to target
        if (path == null) goto CreateNewPath;
        if (pathIndex >= path.Length) goto CreateNewPath;
        if (path[path.Length - 1] != target || path[pathIndex - 1] != moveTargetCoord) goto CreateNewPath;
        CreateNewPath: {
            path = EnvironmentUtility.GetPath (coord.x, coord.y, target.x, target.y);
            pathIndex = 0;
        }
    }

    protected void StartMoveToCoord (Coord target) {
        moveFromCoord = coord;
        moveTargetCoord = target;
        moveStartPos = transform.position;
        moveTargetPos = Environment.tileCentres[moveTargetCoord.x, moveTargetCoord.y];
        animatingMovement = true;

        bool diagonalMove = Coord.SqrDistance (moveFromCoord, moveTargetCoord) > 1;
        moveArcHeightFactor = (diagonalMove) ? sqrtTwo : 1;
        moveSpeedFactor = (diagonalMove) ? oneOverSqrtTwo : 1;

        LookAt (moveTargetCoord);
    }

    protected void LookAt (Coord target) {
        if (target == coord) return;
        Coord offset = target - coord;
        transform.eulerAngles = Vector3.up * Mathf.Atan2 (offset.x, offset.y) * Mathf.Rad2Deg;
    }

    void HandleInteractions () {
        switch (currentAction) {
            case CreatureAction.Eating:
                if (!foodTarget || hunger <= 0) return;
                Eat ();
                break;
            case CreatureAction.Drinking:
                if (thirst <= 0) return;
                Drink ();
                break;
        }
    }

    public virtual void Eat() {
        float eatAmount = Mathf.Min (hunger, Time.deltaTime * 1 / eatDuration);
        eatAmount = ((Plant) foodTarget).Consume (eatAmount);
        hunger -= eatAmount;
    }

    public virtual void Drink() {
        thirst -= Time.deltaTime * 1 / drinkDuration;
        thirst = Mathf.Clamp01 (thirst);
    }

    void AnimateMove () {
        // Move in an arc from start to end tile
        moveTime = Mathf.Min (1, moveTime + Time.deltaTime * moveSpeed * moveSpeedFactor);
        float height = (1 - 4 * (moveTime - .5f) * (moveTime - .5f)) * moveArcHeight * moveArcHeightFactor;
        transform.position = Vector3.Lerp (moveStartPos, moveTargetPos, moveTime) + Vector3.up * height;

        // Finished moving
        if (moveTime < 1) return;
        Environment.RegisterMove (this, moveFromCoord, moveTargetCoord);
        coord = moveTargetCoord;

        animatingMovement = false;
        moveTime = 0;
        ChooseNextAction ();
    }

    void OnDrawGizmos () {
        if (!Application.isPlaying) return;
        if (dead) return;

        var surroundings = Environment.Sense (coord, diet);
        Gizmos.color = Color.red;
        if (surroundings.nearestFoodSource != null)
            Gizmos.DrawLine (transform.position, surroundings.nearestFoodSource.transform.position);
        Gizmos.color = Color.blue;
        if (surroundings.nearestWaterTile != Coord.invalid)
            Gizmos.DrawLine (transform.position, Environment.tileCentres[surroundings.nearestWaterTile.x, surroundings.nearestWaterTile.y]);

        if (currentAction == CreatureAction.GoingToFood) {
            var path = EnvironmentUtility.GetPath (coord.x, coord.y, foodTarget.coord.x, foodTarget.coord.y);
            Gizmos.color = Color.black;
            if (path == null) return;
            for (int i = 0; i < path.Length-1; i++)
                Gizmos.DrawSphere (Environment.tileCentres[path[i].x, path[i].y], .2f);
        }

        if (currentAction == CreatureAction.GoingToWater) {
            var path = EnvironmentUtility.GetPath (coord.x, coord.y, waterTarget.x, waterTarget.y);
            Gizmos.color = Color.white;
            for (int i = 0; i < path.Length-1; i++)
                Gizmos.DrawSphere (Environment.tileCentres[path[i].x, path[i].y], .2f);
        }

        // draw circle around creature to show vision radius
        Gizmos.color = Color.white;
        float translateHeight = 0.0001f;
        float theta = 0f;
        float x = maxViewDistance * Mathf.Cos(theta);
        float y = maxViewDistance * Mathf.Sin(theta);
        Vector3 pos = transform.position + new Vector3(x, translateHeight, y);
        Vector3 newPos = pos;
        Vector3 lastPos = pos;
        for(theta = 0.1f; theta < Mathf.PI * 2; theta += 0.1f){
            x = maxViewDistance * Mathf.Cos(theta);
            y = maxViewDistance * Mathf.Sin(theta);
            newPos = transform.position + new Vector3(x, translateHeight, y);
            Gizmos.DrawLine(pos,newPos);
            pos = newPos;
        }
        Gizmos.DrawLine(pos,lastPos);
    }

}