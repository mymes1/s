using UnityEngine;

public static class SonicRoleInputHandler
{
    public static void HandleInput()
    {
        if (PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || PlayerControl.LocalPlayer.Data.IsDead)
            return;

        // TODO: Replace with your role logic
        SonicRoleType role = GetLocalPlayerRole();

        if (Input.GetKeyDown(KeyCode.Q))
        {
            switch (role)
            {
                case SonicRoleType.Sonic:
                    Sonic.TrySpeedBoost();
                    break;
                case SonicRoleType.Shadow:
                    Shadow.TryTeleport();
                    break;
                case SonicRoleType.SuperShadow:
                    SuperShadow.TryRewind();
                    break;
                case SonicRoleType.Knuckles:
                    Knuckles.TryStun(GetNearestPlayer());
                    break;
                case SonicRoleType.Tails:
                    Tails.TryScanForKillers();
                    break;
                case SonicRoleType.Amy:
                    if (!Amy.linked) Amy.LinkToTarget(GetNearestPlayer());
                    break;
                case SonicRoleType.SuperSonic:
                    SuperSonic.TryInvincibility();
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.F) || IsKillButtonPressed())
        {
            switch (role)
            {
                case SonicRoleType.HyperSonic:
                    HyperSonic.TryDashKill();
                    break;
                case SonicRoleType.Sonic:
                    Sonic.TryHomingAttack();
                    break;
                case SonicRoleType.DarkSonic:
                    DarkSonic.TryRageAttack();
                    break;
                case SonicRoleType.Silver:
                    Silver.TryFreeze(GetNearestPlayer());
                    break;
            }
        }
    }

    // 🔁 Replace this stub with your actual role management logic
    private static SonicRoleType GetLocalPlayerRole()
    {
        // You should hook this to your real role assignment system
        return SonicRoleType.Sonic; // placeholder for testing
    }

    private static bool IsKillButtonPressed()
    {
        return HudManager.Instance?.KillButton?.isActiveAndEnabled == true && HudManager.Instance.KillButton.isPressed;
    }

    private static PlayerControl GetNearestPlayer()
    {
        PlayerControl nearest = null;
        float minDist = float.MaxValue;

        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player == PlayerControl.LocalPlayer || player.Data.IsDead || player.Data.Disconnected)
                continue;

            float dist = Vector2.Distance(PlayerControl.LocalPlayer.transform.position, player.transform.position);
            if (dist < 3f && dist < minDist)
            {
                minDist = dist;
                nearest = player;
            }
        }

        return nearest;
    }
}
