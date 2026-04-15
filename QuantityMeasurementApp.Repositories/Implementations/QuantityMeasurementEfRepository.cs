using Microsoft.EntityFrameworkCore;
using QuantityMeasurementApp.Repositories.Interfaces;
using QuantityMeasurementApp.Models.DTO;
using QuantityMeasurementApp.Models.Entities;
using QuantityMeasurementApp.Repositories.Context;

namespace QuantityMeasurementApp.Repositories.Implementations
{
    // UC16 — EF Core implementation of IQuantityMeasurementRepository.
    //
    // This class does two things:
    //   1. Converts your domain QuantityMeasurementEntity -> flat MeasurementHistoryRecord (for Save)
    //   2. Converts flat MeasurementHistoryRecord -> domain QuantityMeasurementEntity (for GetAll)
    //
    // The service layer never knows this class exists — it only sees the interface.
    // Swap this in Program.cs and everything else stays the same.

    public class QuantityMeasurementEfRepository : IQuantityMeasurementRepository
    {
        private readonly AppDbContext _context;

        public QuantityMeasurementEfRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // SAVE
        // userId is optional — when called without login context, stays null
        public void Save(QuantityMeasurementEntity entity, int? userId = null)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var record    = MapEntityToRecord(entity);
            record.UserId = userId;   // links record to logged-in user, null if anonymous

            _context.MeasurementHistory.Add(record);
            _context.SaveChanges();   // INSERT into SQL Server
        }

        // GET ALL — for admin/audit purposes, returns every user's records
        public List<QuantityMeasurementEntity> GetAll()
        {
            return _context.MeasurementHistory
                           .AsNoTracking()           // read-only, no change tracking needed
                           .OrderBy(r => r.Id)
                           .ToList()
                           .Select(MapRecordToEntity)
                           .ToList();
        }

        // GET BY USER — returns only records belonging to this user, newest first
        public List<QuantityMeasurementEntity> GetByUserId(int userId)
        {
            return _context.MeasurementHistory
                           .AsNoTracking()
                           .Where(r => r.UserId == userId)
                           .OrderByDescending(r => r.CreatedAt)
                           .ToList()
                           .Select(MapRecordToEntity)
                           .ToList();
        }

        // CLEAR
        public void Clear()
        {
            _context.MeasurementHistory.ExecuteDelete();  // EF Core 7+ bulk delete, no SELECT first
        }

        //
        // PRIVATE: domain entity -> flat DB record
        //
        private static MeasurementHistoryRecord MapEntityToRecord(QuantityMeasurementEntity entity)
        {
            var record = new MeasurementHistoryRecord
            {
                Operation    = entity.Operation ?? "Unknown",
                IsError      = entity.IsError,
                ErrorMessage = entity.ErrorMessage,
                CreatedAt    = DateTime.UtcNow,

                // Operand 1
                Operand1Value           = entity.Operand1?.Value ?? 0,
                Operand1Unit            = entity.Operand1?.Unit ?? string.Empty,
                Operand1MeasurementType = entity.Operand1?.MeasurementType ?? string.Empty,

                // Operand 2 (optional)
                Operand2Value           = entity.Operand2?.Value,
                Operand2Unit            = entity.Operand2?.Unit,
                Operand2MeasurementType = entity.Operand2?.MeasurementType,
            };

            // Decide result type and fill matching columns
            if (entity.IsError)
            {
                record.ResultType = "Error";
            }
            else if (entity.ResultQuantity != null)
            {
                record.ResultType                    = "Quantity";
                record.ResultQuantityValue           = entity.ResultQuantity.Value;
                record.ResultQuantityUnit            = entity.ResultQuantity.Unit;
                record.ResultQuantityMeasurementType = entity.ResultQuantity.MeasurementType;
            }
            else if (entity.Operation == "Divide")
            {
                record.ResultType   = "Scalar";
                record.ResultScalar = entity.ResultScalar;
            }
            else if (entity.Operation == "Compare")
            {
                record.ResultType       = "Comparison";
                record.ResultComparison = entity.ResultComparison;
            }
            else
            {
                record.ResultType   = "Scalar";
                record.ResultScalar = entity.ResultScalar;
            }

            return record;
        }

        //
        // PRIVATE: flat DB record -> domain entity
        //
        private static QuantityMeasurementEntity MapRecordToEntity(MeasurementHistoryRecord record)
        {
            var operand1 = new QuantityDTO(
                record.Operand1Value,
                record.Operand1Unit,
                record.Operand1MeasurementType
            );

            QuantityDTO? operand2 = record.Operand2Value.HasValue
                ? new QuantityDTO(
                    record.Operand2Value.Value,
                    record.Operand2Unit!,
                    record.Operand2MeasurementType!)
                : null;

            return record.ResultType switch
            {
                "Quantity" => operand2 == null
                    ? new QuantityMeasurementEntity(operand1, record.Operation,
                        new QuantityDTO(record.ResultQuantityValue!.Value, record.ResultQuantityUnit!, record.ResultQuantityMeasurementType!))
                    : new QuantityMeasurementEntity(operand1, operand2, record.Operation,
                        new QuantityDTO(record.ResultQuantityValue!.Value, record.ResultQuantityUnit!, record.ResultQuantityMeasurementType!)),

                "Scalar"     => new QuantityMeasurementEntity(operand1, operand2!, record.Operation, record.ResultScalar ?? 0),
                "Comparison" => new QuantityMeasurementEntity(operand1, operand2!, record.Operation, record.ResultComparison ?? false),
                _            => new QuantityMeasurementEntity(operand1, operand2!, record.Operation, record.ErrorMessage ?? "Unknown error", true),
            };
        }
    }
}