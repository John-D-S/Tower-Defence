using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Structure;
using Economy;
using Controls;
using Saving;

public static class StaticObjectHolder
{
    public static Core theCore;
    public static StructureSelectionPanel theStructureSelectionPanel;
    public static StructurePlacer theStructurePlacer;
    public static CameraMovement theCameraMovement;
    public static EconomyManager theEconomyManager;

    public static SaveLoadSystem theSaveLoadSystem;
    public static ScoreSystem theScoreSystem;
}