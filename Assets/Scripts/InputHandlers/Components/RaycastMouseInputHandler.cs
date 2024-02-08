using Unity.Entities;
using Unity.Physics.Authoring;
using UnityEngine;

namespace ZombiesShooter
{
    public class RaycastMouseInputHandler : IComponentData
    {
        public PhysicsCategoryTags belongsTo;
        public PhysicsCategoryTags interactsWith;
        public Camera              cameraForInput;
    }
}