namespace Data;

public enum DbErrorType
{
    Unexpected,
    NotFound,
    UniqueConstraintViolation, // 2601, 2627
    ForeignKeyViolation // 547
}
