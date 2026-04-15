// using QuantityMeasurementApp.Models.Entities;
// using QuantityMeasurementApp.Repositories.Interfaces;
// using System.Linq;

// namespace QuantityMeasurementApp.Repositories.Implementations
// {
//     public class QuantityMeasurementCacheRepository : IQuantityMeasurementRepository
//     {
//         private static readonly QuantityMeasurementCacheRepository _instance
//             = new QuantityMeasurementCacheRepository();

//         private readonly List<QuantityMeasurementEntity> _cache;

//         private QuantityMeasurementCacheRepository()
//         {
//             _cache = new List<QuantityMeasurementEntity>();
//         }

//         public static QuantityMeasurementCacheRepository GetInstance()
//         {
//             return _instance;
//         }

//         public void Save(QuantityMeasurementEntity entity)
//         {
//             if (entity == null)
//                 throw new ArgumentNullException(nameof(entity));

//             _cache.Add(entity);
//         }

//         public List<QuantityMeasurementEntity> GetAll()
//         {
//             return _cache.ToList();
//         }

//         public void Clear()
//         {
//             _cache.Clear();
//         }
        
//     }
// }