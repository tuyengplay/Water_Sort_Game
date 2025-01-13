using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaterSort
{
    public static class RhythmManager
    {
        private static readonly float velocity = 2f;
        private static readonly float timeWater = 0.3f;

        public static float TimeMove(Vector2 _start, Vector2 _pos)
        {
            float distance = Vector2.Distance(_start, _pos);
            return distance / velocity;
        }

        public static float TimeDrop(int _count)
        {
            return timeWater * _count;
        }
    }
}