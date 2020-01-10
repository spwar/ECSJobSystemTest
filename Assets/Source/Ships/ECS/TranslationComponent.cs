using Unity.Entities;

namespace Ships.ECS
{
    public class TranslationComponent : IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager,
            GameObjectConversionSystem gameObjectConversionSystem)
        {
        
        }
    }
}
