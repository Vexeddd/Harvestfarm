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

        Player.Instance.transform.position = new Vector3(data.px, data.py, data.pz);

        // Clear carried physics velocity so the teleport does not cause a lurch.
        if (Player.Instance._rb != null)
        {
            Player.Instance._rb.velocity = Vector3.zero;
            Player.Instance._rb.angularVelocity = Vector3.zero;
        }
    }
}
