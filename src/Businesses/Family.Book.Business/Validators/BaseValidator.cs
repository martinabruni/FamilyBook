using Family.Book.Domain.Validators;

namespace Family.Book.Business.Validators;

public abstract class BaseValidator<T> : IValidator<T> where T : class
{
    protected BaseValidator(T instance)
    {
        Validate(instance);
    }

    public abstract void Validate(T instance);
}