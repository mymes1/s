using BepInEx;
using UnityEngine;

[BepInPlugin("com.mymes.sonicroles", "Sonic Roles Mod", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
    public void Start()
    {
        Logger.LogInfo("Sonic Roles Mod Loaded");
        SonicRolesRegistry.RegisterAllRoles();
    }

    public void Update()
    {
        SonicRolesRegistry.UpdateAllRoles();
        SonicRoleInputHandler.HandleInput(); // optional: if you want to handle Q/F keys, etc.
    }
}
