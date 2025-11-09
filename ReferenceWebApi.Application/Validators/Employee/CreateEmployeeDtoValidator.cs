using FluentValidation;
using ReferenceWebApi.Application.Dtos.Employee;

namespace ReferenceWebApi.Application.Validators.Employee
{
    public class CreateEmployeeDtoValidator : AbstractValidator<CreateEmployeeDto>
    {
        public CreateEmployeeDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(100).WithMessage("First name must not exceed 100 characters")
                .Matches(@"^[a-zA-Z\s-']+$").WithMessage("First name contains invalid characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(100).WithMessage("Last name must not exceed 100 characters")
                .Matches(@"^[a-zA-Z\s-']+$").WithMessage("Last name contains invalid characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(255).WithMessage("Email must not exceed 255 characters");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^[\d\s\-\+\(\)]+$").WithMessage("Invalid phone number format")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            RuleFor(x => x.Department)
                .NotEmpty().WithMessage("Department is required")
                .MaximumLength(100).WithMessage("Department must not exceed 100 characters");

            RuleFor(x => x.Position)
                .NotEmpty().WithMessage("Position is required")
                .MaximumLength(100).WithMessage("Position must not exceed 100 characters");

            RuleFor(x => x.Salary)
                .GreaterThan(0).WithMessage("Salary must be greater than 0")
                .LessThanOrEqualTo(9999999.99m).WithMessage("Salary exceeds maximum allowed value");

            RuleFor(x => x.HireDate)
                .NotEmpty().WithMessage("Hire date is required")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Hire date cannot be in the future");
        }
    }
}
