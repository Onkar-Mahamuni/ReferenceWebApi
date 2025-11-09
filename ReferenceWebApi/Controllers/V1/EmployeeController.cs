using Microsoft.AspNetCore.Mvc;
using ReferenceWebApi.Application.Dtos;
using ReferenceWebApi.Application.Dtos.Employee;
using ReferenceWebApi.Application.Interfaces;

namespace ReferenceWebApi.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    public class EmployeesController : BaseApiController
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(
            IEmployeeService employeeService,
            ILogger<EmployeesController> logger)
        {
            _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets all employees with pagination
        /// </summary>
        /// <param name="parameters">Pagination parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of employees</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResultDto<EmployeeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResultDto<EmployeeDto>>> GetEmployees(
            [FromQuery] PaginationParametersDto parameters,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting employees with pagination: Page {PageNumber}, Size {PageSize}",
                parameters.PageNumber, parameters.PageSize);

            var result = await _employeeService.GetPagedAsync(parameters, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Gets an employee by ID
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Employee details</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmployeeDto>> GetEmployee(
            int id,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting employee with ID: {EmployeeId}", id);

            var employee = await _employeeService.GetByIdAsync(id, cancellationToken);
            return Ok(employee);
        }

        /// <summary>
        /// Gets an employee by email
        /// </summary>
        /// <param name="email">Employee email</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Employee details</returns>
        [HttpGet("by-email/{email}")]
        [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmployeeDto>> GetEmployeeByEmail(
            string email,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting employee with email: {Email}", email);

            var employee = await _employeeService.GetByEmailAsync(email, cancellationToken);

            if (employee is null)
            {
                return NotFound(new { message = $"Employee with email '{email}' not found" });
            }

            return Ok(employee);
        }

        /// <summary>
        /// Gets employees by department
        /// </summary>
        /// <param name="department">Department name</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of employees in the department</returns>
        [HttpGet("by-department/{department}")]
        [ProducesResponseType(typeof(IEnumerable<EmployeeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployeesByDepartment(
            string department,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting employees in department: {Department}", department);

            var employees = await _employeeService.GetByDepartmentAsync(department, cancellationToken);
            return Ok(employees);
        }

        /// <summary>
        /// Creates a new employee
        /// </summary>
        /// <param name="createDto">Employee creation data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Created employee</returns>
        [HttpPost]
        [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EmployeeDto>> CreateEmployee(
            [FromBody] CreateEmployeeDto createDto,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating new employee with email: {Email}", createDto.Email);

            var employee = await _employeeService.CreateAsync(createDto, cancellationToken);

            return CreatedAtAction(
                nameof(GetEmployee),
                new { id = employee.Id },
                employee);
        }

        /// <summary>
        /// Updates an existing employee
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <param name="updateDto">Employee update data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Updated employee</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EmployeeDto>> UpdateEmployee(
            int id,
            [FromBody] UpdateEmployeeDto updateDto,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating employee with ID: {EmployeeId}", id);

            var employee = await _employeeService.UpdateAsync(id, updateDto, cancellationToken);
            return Ok(employee);
        }

        /// <summary>
        /// Deletes an employee (soft delete)
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>No content</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteEmployee(
            int id,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting employee with ID: {EmployeeId}", id);

            await _employeeService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
