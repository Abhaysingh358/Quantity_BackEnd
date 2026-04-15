using QuantityMeasurementApp.Models.Entities;

namespace QuantityMeasurementApp.Repositories.Interfaces
{
    // UC15 - i defined this interface now even though repo layer is not implemented yet
    // the reason is — service constructor takes this as parameter (dependency injection)
    // if i don't define it now, when repo comes later i have to change the service constructor
    // and that would break console app, tests, future API — everywhere service is created

    // by defining the interface now, the service is ready for repo
    // console app passes a NoOp implementation that does nothing
    // when real repo comes, we just swap NoOp with the real implementation
    // service layer doesn't change at all — that's the benefit of doing this now
    public interface IQuantityMeasurementRepository
    {
        // saves a measurement record after every operation
        // userId is optional — null means unauthenticated or anonymous call
        void Save(QuantityMeasurementEntity entity, int? userId = null);

        // returns all saved records — useful for admin/audit
        List<QuantityMeasurementEntity> GetAll();

        // returns only records belonging to a specific user
        List<QuantityMeasurementEntity> GetByUserId(int userId);

        // clears all records — mainly for testing
        void Clear();
    }
}