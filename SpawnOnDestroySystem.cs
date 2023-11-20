using Client.Scripts.DataStorage;
using Client.Scripts.GoldAndZombies.Components;
using Client.Scripts.Tools.Services;

using Leopotam.EcsLite;

using UnityEngine;


namespace Client.Scripts.GoldAndZombies.Systems
{
    public class SpawnOnDestroySystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld _world;
        private EcsFilter _spawners;

        public void Init(EcsSystems systems)
        {
            _world = systems.GetWorld();
            _spawners = _world.Filter<SpawnOnDestroyComponent>().Inc<DestroyedComponent>().Exc<DestroyedFromSaveComponent>().End();
        }

        public void Run(EcsSystems systems)
        {
            foreach (int entity in _spawners)
            {
                ref SpawnOnDestroyComponent spawnComponent = ref Data.Battle.Pools.SpawnOnDestroyPool.Get(entity);
                GameObject spawnedObj = Service.Pool.SpawnObject(spawnComponent.PoolItem);

                if (!spawnedObj)
                {
                    continue;
                }
                
                spawnedObj.transform.position = spawnComponent.Position;
            }
        }
    }
}