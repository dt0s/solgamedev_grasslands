using System;
using UnityEngine;

[System.Serializable] 
public static class GameData
{
    public static string PortalName { get; set; }
    public static string TransitionName { get; set; }
    public static string currentScene { get; set; }
    public static bool loadDataForPortalTransition { get; set; }
    public static readonly Vector3 cameraAdjustment = new Vector3(0, 2, -10f);
    
    // ALL POSSIBLE STARTING POSITIONS
    // RESPAWN (PORTAL ROOM BOTTOM MID)
    public static readonly Vector3 startPos_respawn = new Vector3(0.5f, -9.8f, 0);
    //public static readonly Vector3 startPos_respawn = new Vector3(-8.58f, 35f, 0);
    // MAIN GAME
    public static readonly Vector3 startPos_gameStart = new Vector3(0, -5.5f, 0);
    //public static readonly Vector3 startPos_gameStart = new Vector3(-8.58f, 35f, 0);
    
    public static readonly Vector3 startPos_main_to_desert = new Vector3(-12.36f, 30.3f, 0f);

    // DESERT LEVEL
    public static readonly Vector3 startPos_desert_to_main = new Vector3(-8.58f, 34.6f, 0f);
        
    // PORTAL LOCATIONS IN ALL LEVELS (1 PORTAL PER LEVEL)
    public static readonly Vector3 location_basePortal_mainGame = new Vector3(-2.47f, 34.98f, 0f);
    public static readonly Vector3 location_desertPortal_desertLevel = new Vector3(-149.6f, 68.22f, 0f);
    // PORTAL ROOM LOCATIONS (1 PORTAL PER LEVEL)
    public static readonly Vector3 location_basePortal_portalRoom = new Vector3(0.565f, -6.8f, 0f);
    public static readonly Vector3 location_desertPortal_portalRoom = new Vector3(-5.4f, -6.8f, 0f);
    // SAVE PORTAL ROOM PROGRESS
    public static bool hasBasePortal { get; set; }
    public static bool hasDesertPortal { get; set; }
    // SAVE TROPHIES
}
