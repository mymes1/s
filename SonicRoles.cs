using System.Collections;
using UnityEngine;

// === Sonic Role Definitions ===
public enum SonicRoleType {
    Sonic,
    SuperSonic,
    DarkSonic,
    Tails,
    Amy,
    Knuckles,
    Shadow,
    SuperShadow,
    HyperSonic,
    Silver
}

// === Sonic Role Registry ===
public static class SonicRolesRegistry {
    public static void RegisterAllRoles() {
        Sonic.clearAndReload();
        SuperSonic.clearAndReload();
        DarkSonic.clearAndReload();
        Tails.clearAndReload();
        Amy.clearAndReload();
        Knuckles.clearAndReload();
        Shadow.clearAndReload();
        SuperShadow.clearAndReload();
        HyperSonic.clearAndReload();
        Silver.clearAndReload();
    }

    public static void UpdateAllRoles() {
        Sonic.Update();
        SuperSonic.Update();
        DarkSonic.Update();
        Tails.Update();
        Amy.Update();
        Knuckles.Update();
        Shadow.Update();
        SuperShadow.Update();
        HyperSonic.Update();
        Silver.Update();
    }
}

// === Sonic ===
public static class Sonic {
    public static float speedMultiplier = 1.75f;
    public static float boostDuration = 5f;
    public static float speedCooldown = 20f;
    public static float speedCooldownTimer = 0f;
    public static float boostTimer = 0f;

    public static float homingRange = 3.5f;
    public static float homingCooldown = 15f;
    public static float homingCooldownTimer = 0f;

    public static void clearAndReload() {
        speedCooldownTimer = 0f;
        boostTimer = 0f;
        homingCooldownTimer = 0f;
    }

    public static void Update() {
        if (speedCooldownTimer > 0f) speedCooldownTimer -= Time.deltaTime;
        if (homingCooldownTimer > 0f) homingCooldownTimer -= Time.deltaTime;
        if (boostTimer > 0f) {
            boostTimer -= Time.deltaTime;
            if (boostTimer <= 0f) PlayerControl.LocalPlayer.MyPhysics.Speed = 1f;
        }
    }

    public static void TrySpeedBoost() {
        if (speedCooldownTimer <= 0f) {
            PlayerControl.LocalPlayer.MyPhysics.Speed = speedMultiplier;
            boostTimer = boostDuration;
            speedCooldownTimer = speedCooldown;
        }
    }

    public static void TryHomingAttack() {
        if (homingCooldownTimer <= 0f) {
            var target = GetNearestPlayer();
            if (target != null) {
                PlayerControl.LocalPlayer.transform.position = target.transform.position;
                target.Murder(PlayerControl.LocalPlayer);
                homingCooldownTimer = homingCooldown;
            }
        }
    }

    private static PlayerControl GetNearestPlayer() {
        PlayerControl nearest = null;
        float minDist = float.MaxValue;
        foreach (var p in PlayerControl.AllPlayerControls) {
            if (p == PlayerControl.LocalPlayer || p.Data.IsDead) continue;
            float dist = Vector2.Distance(PlayerControl.LocalPlayer.transform.position, p.transform.position);
            if (dist < homingRange && dist < minDist) {
                nearest = p;
                minDist = dist;
            }
        }
        return nearest;
    }
}

// === Super Sonic ===
public static class SuperSonic {
    public static float invincibilityDuration = 5f;
    public static float cooldown = 30f;
    public static float timer = 0f;
    public static bool isInvincible = false;

    public static void clearAndReload() {
        timer = 0f;
        isInvincible = false;
    }

    public static void Update() {
        if (isInvincible) {
            timer -= Time.deltaTime;
            if (timer <= 0f) isInvincible = false;
        }
    }

    public static void TryInvincibility() {
        if (!isInvincible) {
            isInvincible = true;
            timer = invincibilityDuration;
        }
    }
}

// === Dark Sonic ===
public static class DarkSonic {
    public static float aoeRange = 2.5f;
    public static float cooldown = 35f;
    public static float cooldownTimer = 0f;

    public static void clearAndReload() {
        cooldownTimer = 0f;
    }

    public static void Update() {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;
    }

    public static void TryRageAttack() {
        if (cooldownTimer <= 0f) {
            foreach (var p in PlayerControl.AllPlayerControls) {
                if (p == PlayerControl.LocalPlayer || p.Data.IsDead) continue;
                float dist = Vector2.Distance(PlayerControl.LocalPlayer.transform.position, p.transform.position);
                if (dist <= aoeRange) p.Murder(PlayerControl.LocalPlayer);
            }
            cooldownTimer = cooldown;
        }
    }
}

// === Hyper Sonic ===
public static class HyperSonic {
    public static float dashDistance = 6f;
    public static float cooldown = 30f;
    public static float cooldownTimer = 0f;

    public static void clearAndReload() {
        cooldownTimer = 0f;
    }

    public static void Update() {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;
    }

    public static void TryDashKill() {
        if (cooldownTimer <= 0f) {
            Vector2 dashEnd = (Vector2)PlayerControl.LocalPlayer.transform.position + (Vector2)PlayerControl.LocalPlayer.transform.up * dashDistance;
            foreach (var p in PlayerControl.AllPlayerControls) {
                if (p == PlayerControl.LocalPlayer || p.Data.IsDead) continue;
                if (Vector2.Distance(p.transform.position, dashEnd) <= 1.5f) {
                    p.Murder(PlayerControl.LocalPlayer);
                    break;
                }
            }
            PlayerControl.LocalPlayer.NetTransform.SnapTo(dashEnd);
            cooldownTimer = cooldown;
        }
    }
}

// === Silver ===
public static class Silver {
    public static float freezeDuration = 3.5f;
    public static float cooldown = 25f;
    public static float cooldownTimer = 0f;

    public static void clearAndReload() {
        cooldownTimer = 0f;
    }

    public static void Update() {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;
    }

    public static void TryFreeze(PlayerControl target) {
        if (cooldownTimer <= 0f && target != null && !target.Data.IsDead) {
            target.moveable = false;
            CoroutineHelper.StartCoroutine(UnfreezeAfterDelay(target, freezeDuration));
            cooldownTimer = cooldown;
        }
    }

    private static IEnumerator UnfreezeAfterDelay(PlayerControl target, float delay) {
        yield return new WaitForSeconds(delay);
        if (target != null && !target.Data.IsDead) target.moveable = true;
    }
}

// === Tails ===
public static class Tails {
    public static float cooldown = 25f;
    public static float cooldownTimer = 0f;

    public static void clearAndReload() {
        cooldownTimer = 0f;
    }

    public static void Update() {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;
    }

    public static void TryScanForKillers() {
        if (cooldownTimer <= 0f) {
            // Show message or highlight dead bodies
            cooldownTimer = cooldown;
        }
    }
}

// === Amy ===
public static class Amy {
    public static PlayerControl loveTarget;
    public static bool linked = false;

    public static void clearAndReload() {
        loveTarget = null;
        linked = false;
    }

    public static void Update() {
        if (linked && loveTarget != null && loveTarget.Data.IsDead && !PlayerControl.LocalPlayer.Data.IsDead) {
            PlayerControl.LocalPlayer.Murder(PlayerControl.LocalPlayer);
        }
    }

    public static void LinkToTarget(PlayerControl target) {
        if (!linked && target != null && !target.Data.IsDead) {
            loveTarget = target;
            linked = true;
        }
    }
}

// === Knuckles ===
public static class Knuckles {
    public static float cooldown = 20f;
    public static float cooldownTimer = 0f;
    public static float stunDuration = 2f;

    public static void clearAndReload() {
        cooldownTimer = 0f;
    }

    public static void Update() {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;
    }

    public static void TryStun(PlayerControl target) {
        if (cooldownTimer <= 0f && target != null && !target.Data.IsDead) {
            target.moveable = false;
            CoroutineHelper.StartCoroutine(UnfreezeAfterDelay(target, stunDuration));
            cooldownTimer = cooldown;
        }
    }

    private static IEnumerator UnfreezeAfterDelay(PlayerControl target, float delay) {
        yield return new WaitForSeconds(delay);
        if (target != null && !target.Data.IsDead) target.moveable = true;
    }
}

// === Shadow ===
public static class Shadow {
    public static float cooldown = 25f;
    public static float cooldownTimer = 0f;
    public static float teleportDistance = 4f;

    public static void clearAndReload() {
        cooldownTimer = 0f;
    }

    public static void Update() {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;
    }

    public static void TryTeleport() {
        if (cooldownTimer <= 0f) {
            Vector2 newPos = (Vector2)PlayerControl.LocalPlayer.transform.position + (Vector2)PlayerControl.LocalPlayer.transform.up * teleportDistance;
            PlayerControl.LocalPlayer.NetTransform.SnapTo(newPos);
            cooldownTimer = cooldown;
        }
    }
}

// === Super Shadow ===
public static class SuperShadow {
    public static float rewindDuration = 3f;
    public static float cooldown = 35f;
    public static float cooldownTimer = 0f;

    public static void clearAndReload() {
        cooldownTimer = 0f;
    }

    public static void Update() {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;
    }

    public static void TryRewind() {
        if (cooldownTimer <= 0f) {
            Vector2 rewindPos = PlayerControl.LocalPlayer.transform.position - PlayerControl.LocalPlayer.transform.up * 2;
            PlayerControl.LocalPlayer.NetTransform.SnapTo(rewindPos);
            cooldownTimer = cooldown;
        }
    }
}
