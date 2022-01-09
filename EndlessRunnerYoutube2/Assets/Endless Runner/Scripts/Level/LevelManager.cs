using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    private int randomInt;                          //Used for Random Generation of Levels
    private int prevRandomInt;                      // check for last trackpiece selected
    private int randomIntTrans;                     // Used for Random generation of transition pieces
    private int prevRandomIntTrans;                     // Used for Random generation of transition pieces
    private const int INITIAL_SEGMENTS = 20;        // How many Initial segmets to create
    public int trackpooledAmount = 25;                   // How many objects to add to pool
    private const int INITIAL_TRANSITION_SEGMENTS = 2;  // how many trasition segmests to inital create
    public int TranspooledAmount = 6;                   // how many transition pool pieces to create
    public int TrackPieceCount = 0;                     // Define inital count of Track pieces
    private float trackLength = 30.4f;                     // how long is the track piece


    public GameObject[] LevelSegments;                          // Create array of level pieces defined in inspector for pool to pick from
    public GameObject[] TransSegments;                          // Create array of transition pieces defined in inspector for pool to pick from
    List<GameObject> trackPiecesLevel1EASY;                     // Create a list of Game objects called trackPiecesLevel1EASY to be used as the pool
    List<GameObject> transitionPieces;                          // Create a list of Game objects to be used as transition pieces


    public Vector3 spawnOrigin;                          // Vector3 of origin point for platform pieces
    private Vector3 spawnPosition;                          // Vector3 of where to spawn track pieces


    void OnEnable()
    {


        ObjectDestroy.OnLevelExited += OnLevelExited;         // Event Listener for trackpieces to be recycled enable
    }

    private void OnDisable()
    {


        ObjectDestroy.OnLevelExited += OnLevelExited;         // Event Listener for trackpieces to be recycled disabling
    }



    void Start()
    {
        // Initilize the random values for the track pieces
        randomInt = 0;                                               // set inital values for random track selction
        prevRandomInt = 1;                                           // set inital values for random track selction previous selection 
        randomIntTrans = 0;                                         // Define initail random number for transitions track pieces
        prevRandomIntTrans = 1;                                     // Record of previosu transition track peice selected

        // Initilize normal track pieces
        trackPiecesLevel1EASY = new List<GameObject>();               // DEFINE track pieces as game object list
        for (int i = 0; i < trackpooledAmount; i++)              // for all the trackpooledAmount generate a pooled piece at start
        {
            while (randomInt == prevRandomInt)                                  // keep checking to reduce the number of duplicate track pieces
            {
                randomInt = Random.Range(0, LevelSegments.Length);              // Random number between 0 and how manay pieces defined in LevelSegments

            }
            prevRandomInt = randomInt;                                          // Record what the last track piece was 
            GameObject obj = Instantiate(LevelSegments[randomInt]) as GameObject;  // instatiate it into the pool
            obj.SetActive(false);                                                   // disable the game object 
            trackPiecesLevel1EASY.Add(obj);                                                   // add obj to the list of pooled objects for trackPiecesLevel1EASY
        }

        // Initilize transition pieces     
        transitionPieces = new List<GameObject>();                  // DEFINE track pieces as game object list for transitions
        for (int i = 0; i < TranspooledAmount; i++)
        {
            while (randomIntTrans == prevRandomIntTrans)                                  // keep checking to reduce the number of duplicate track pieces
            {
                randomIntTrans = Random.Range(0, TransSegments.Length);                 // Random number between 0 and how manay pieces defined in TransSegments
            }
            prevRandomIntTrans = randomIntTrans;                                        // Record what the last track piece was
            GameObject transObj = Instantiate(TransSegments[randomIntTrans]) as GameObject;     // instatiate it into the pool transitionPieces
            transObj.SetActive(false);                                                          // disable the game object 
            transitionPieces.Add(transObj);                                                     // add obj to the list of pooled objects for transitionPieces
        }


        // Create the inital track pieces and place them in the world
        for (int i = 0; i < INITIAL_SEGMENTS; i++)                                                  // for number of Inital segemts create track pieces and transition pieces in world
        {
            if (i < INITIAL_TRANSITION_SEGMENTS)
            {
                CreateTransition();                                                                     // Create Transition Piece
            }
            else
            {
                CreateSegment();                                                                        // Create Track segments
            }
        }
        TrackPieceCount = 0;                                                                            // Set the inital number of trackpieces to 0 to start off the count

    }




    private void CreateSegment()
    {
        TrackPieceCount++;                                                                          // creating a new track piece increment count
        for (int i = 0; i < trackPiecesLevel1EASY.Count; i++)                                       // for every track piece run the below
        {
            if (!trackPiecesLevel1EASY[i].activeInHierarchy)                                        // If the track piece is not active in the game world
            {
                spawnOrigin = spawnOrigin + new Vector3(0, 0, trackLength);                                 // calcuate the spawn point for the track peice  plus track length
                trackPiecesLevel1EASY[i].transform.position = spawnPosition + spawnOrigin;                  // set track position to be at spawn position plus the spawn origion
                trackPiecesLevel1EASY[i].SetActive(true);                                                   // Set the piece to be active in the world
                break;                                                                                      // we found a piece, no need to contrinue so break out
            }
        }
    }

    public void CreateTransition()
    {
        TrackPieceCount++;                                                                          // creating a new track piece so increment the count
        for (int i = 0; i < transitionPieces.Count; i++)                                                // for every transition piece run the below
        {
            if (!transitionPieces[i].activeInHierarchy)                                                 // If the trnsition piece is not active in the game world
            {
                spawnOrigin = spawnOrigin + new Vector3(0, 0, trackLength);                                 // calcuate the spawn point for the transition peice plus track length
                transitionPieces[i].transform.position = spawnPosition + spawnOrigin;                       // set track position to be at spawn position plus the spawn origion
                transitionPieces[i].SetActive(true);                                                         // Set the piece to be active in the world
                break;                                                                                      // we found a piece, no need to contrinue so break out
            }
        }
    }

    void OnLevelExited()
    {
        if (TrackPieceCount == 10)                          // for every 10th track piece create a transition piece
        {
            CreateTransition();                             // create a transition piece
            TrackPieceCount = 0;                            // reset counter back to 0 
        }
        else
        {
            CreateSegment();                                // if we are not at TrackPieceCount, then create a new segment
        }
    }





}
