using UnityEngine;

public class PlayerSaveable : ISaveable
{
    public void OnSave(SaveData data)
    {
        if (Player.Instance == null) return;

        var t = Player.Instance.transform;
        data.px = t.position.x;
        data.py = t.position.y;
        data.pz = t.position.z;
    }

    public void OnLoad(SaveData data)
    {
        if (Player.Instance == null) return;

        var rb = Player.Instance._rb;
        if (rb == null) return;

        // Write the physics body's position (not transform.position), so a
        // FixedUpdate can't overwrite the teleport with the body's stale position.
        rb.position = new Vector3(data.px, data.py, data.pz);

        // Clear carried physics velocity so the teleport does not cause a lurch.
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
