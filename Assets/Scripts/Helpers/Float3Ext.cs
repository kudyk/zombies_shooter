using Unity.Mathematics;

namespace ZombiesShooter
{
    public struct Float3Ext
    {
        public static float3 LocalToWorld(float4x4 localToWorldValue, float3 direction)
        {
            return math.rotate(localToWorldValue, direction);
        }

        public static float3 StraightRelativePositionTo(float3 selfWorldPosition, float3 targetWorldPosition)
        {
            float3 relativePosition = targetWorldPosition - selfWorldPosition;
            relativePosition.y = 0;

            return relativePosition;
        }

        public static float3 ClampMagnitude(float3 vector, float maxLength)
        {
            float sqrMagnitude = math.lengthsq(vector);
            if ((double)sqrMagnitude <= (double)maxLength * (double)maxLength)
                return vector;
            float num1 = (float)math.sqrt((double)sqrMagnitude);
            float num2 = vector.x / num1;
            float num3 = vector.y / num1;
            float num4 = vector.z / num1;
            return new float3(num2 * maxLength, num3 * maxLength, num4 * maxLength);
        }
    }
}