namespace MovieRental.Exceptions
{
	public class EntityNotFoundException : Exception
	{
		public EntityNotFoundException(string message) : base(message) { }
		public EntityNotFoundException(string entityName, int id) 
			: base($"{entityName} with ID {id} was not found.") { }
	}

	public class ValidationException : Exception
	{
		public ValidationException(string message) : base(message) { }
	}

	public class DuplicateEntityException : Exception
	{
		public DuplicateEntityException(string message) : base(message) { }
	}

	public class DatabaseOperationException : Exception
	{
		public DatabaseOperationException(string message) : base(message) { }
		public DatabaseOperationException(string message, Exception innerException) 
			: base(message, innerException) { }
	}
} 