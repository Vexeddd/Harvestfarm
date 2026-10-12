using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class VehicleSeat
{
    public static Dictionary<Transform, PlayerVehicleController> seatMap = new Dictionary<Transform, PlayerVehicleController>();

    public static bool IsOccupied(Transform seat)
    {
        return seatMap.ContainsKey(seat);
    }

    public static void Attack(PlayerVehicleController player, Transform seat)
    {
        seatMap[seat] = player;
    }

    public static void Detack(PlayerVehicleController player)
    {
        var seat = seatMap.FirstOrDefault(p => p.Value == player).Key;

        if (seat != null)
        {
             seatMap.Remove(seat);
        }
    }
}
