using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Structures;
using Economy;
using Controls;
using Saving;

/// <summary>
/// holds static objects for use by other scripts
/// </summary>
public static class StaticObjectHolder
{
    public static VisibilityManager theVisibilityManager;
    public static Core theCore;
    public static StructureSelectionPanel theStructureSelectionPanel;
    public static StructurePlacer theStructurePlacer;
    public static CameraMovement theCameraMovement;
    public static EconomyManager theEconomyManager;

    public static SaveLoadSystem theSaveLoadSystem;
    public static ScoreSystem theScoreSystem;
}